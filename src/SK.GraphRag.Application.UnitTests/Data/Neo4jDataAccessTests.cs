using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Data;
using SK.GraphRag.Application.Settings;
using SK.GraphRag.Application.UnitTests.TestExtensions;

namespace SK.GraphRag.Application.UnitTests.Data;

public class Neo4jDataAccessTests
{
    private sealed class TestNeo4jDataAccess(
        IDriver driver,
        IOptions<GraphDatabaseSettings> options,
        string databaseName,
        ILogger<Neo4jDataAccess> logger)
        : Neo4jDataAccess(
            driver,
            options,
            databaseName,
            logger)
    {
        // Expose public wrappers already available on base class; nothing else required.
    }

    [Fact]
    public async Task ExecuteReadTransactionAsync_CallbackInvokes_RunAsync_AndReturnsMappedValues()
    {
        // Arrange
        var query = "MATCH (n) RETURN n.name AS name";
        var returnKey = "name";
        var expectedValue = "Alice";

        // Mock a record that contains the expected value under returnKey
        var recordMock = new Mock<IRecord>();
        recordMock.Setup(r => r.Values).Returns(new Dictionary<string, object> { { returnKey, expectedValue } });

        // Mock result cursor to return our record list
        var cursorMock = new Mock<IResultCursor>();
        /*
        cursorMock.Setup(c => c.ToListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IRecord> { recordMock.Object });
        */
        //Try another way since ToListAsync with params is not available
        cursorMock
           .SetupSequence(cursor => cursor.FetchAsync())
           .ReturnsAsync(true)  // First record
           .ReturnsAsync(false); // No more records

        //TODO: Should be able to use the Values dictionary setup above
        var mockMovie = new Mock<IRecord>();
        mockMovie.Setup(r => r.Get<string>(returnKey))
            .Returns(() => expectedValue);

        // Do we still need this?
        //cursorMock
        //    .Setup(cursor => cursor.Current)
        //    //.Returns(mockMovie.Object);
        //    .Returns(() => mockMovie.Object);
        //end alternative

        //var mockStuff = TestHelpers.MockAsyncEnumerable(new List<IRecord> { mockMovie.Object });
        var mockStuff = TestHelpers.MockAsyncEnumerable(new List<IRecord> { recordMock.Object  });
        //cursorMock
        //    .Setup(cursor => cursor.Current)
        //    //.Returns(mockMovie.Object);
        //    .Returns(() => recordMock.Object);
        cursorMock
            .Setup(cursor => cursor.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => mockStuff.GetAsyncEnumerator());
        /*
        cursorMock
            .Setup(cursor => cursor.GetAsyncEnumerator)
            .Returns(() => new AsyncEnumeratorMock<IRecord>(new List<IRecord> { mockMovie.Object }));
        */

        // Mock transaction and ensure RunAsync is called with expected query
        var txMock = new Mock<IAsyncTransaction>();
        txMock
            .Setup(t => t.RunAsync(It.Is<string>(q => q == query), It.IsAny<IDictionary<string, object>?>()))
            //.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object>?>()))
            //.ReturnsAsync(cursorMock.Object);
            .ReturnsAsync(() => cursorMock.Object);

        // Mock session to capture and invoke the callback with our mocked transaction
        var sessionMock = new Mock<IAsyncSession>();
        sessionMock
            .Setup(s => s.ExecuteReadAsync(It.IsAny<Func<IAsyncQueryRunner, Task<List<string>>>>(), It.IsAny<Action<TransactionConfigBuilder>>()))
            .Returns<Func<IAsyncTransaction, Task<List<string>>>, Action<TransactionConfigBuilder>>((callback, _) => callback(txMock.Object));

        // Mock driver to return our mocked session
        var driverMock = new Mock<IDriver>();
        driverMock
            .Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
            .Returns(sessionMock.Object);

        var options = Options.Create(new GraphDatabaseSettings());
        var loggerMock = new Mock<ILogger<Neo4jDataAccess>>();

        var dataAccess = new TestNeo4jDataAccess(driverMock.Object, options, "testdb", loggerMock.Object);

        try
        {
            // Act
            var result = await dataAccess.ExecuteReadListAsync(query, returnKey);

            // Assert
            result.Should().ContainSingle().Which.Should().Be(expectedValue);
            txMock.Verify(t => t.RunAsync(query, It.IsAny<IDictionary<string, object>?>()), Times.Once);
        }
        finally
        {
            await dataAccess.DisposeAsync().ConfigureAwait(true);
        }
    }
}