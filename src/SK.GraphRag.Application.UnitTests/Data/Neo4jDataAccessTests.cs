using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Data;
using SK.GraphRag.Application.Settings;
using SK.GraphRag.Application.UnitTests.TestExtensions;

namespace SK.GraphRag.Application.UnitTests.Data;

public class Neo4jDataAccessTests
{
    private const string DATABASE_NAME_FIELD = "_databaseName";
    private const string TEST_DATABASE_NAME = "testdb";

    /* Test class required because Neo4jDataAccess is abstract */
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
    public async Task Constructor_SetsDatabaseName()
    {
        // Arrange
        IOptions<GraphDatabaseSettings> options = new OptionsWrapper<GraphDatabaseSettings>(new GraphDatabaseSettings());

        var sut = new TestNeo4jDataAccess(Mock.Of<IDriver>(),
            options,
            TEST_DATABASE_NAME,
            NullLogger<Neo4jDataAccess>.Instance);

        await using (sut.ConfigureAwait(false))
        {
            // Assert
            var databaseValue = sut.GetPrivateField(DATABASE_NAME_FIELD);
            databaseValue.Should().Be(TEST_DATABASE_NAME);
        }
    }

    [Fact]
    public void Constructor_WithEmptyDatabaseName_ThrowsException()
    {
        // Arrange
        IOptions<GraphDatabaseSettings> options = new OptionsWrapper<GraphDatabaseSettings>(new GraphDatabaseSettings());

        FluentActions.Invoking(() => new TestNeo4jDataAccess(Mock.Of<IDriver>(),
            options,
            string.Empty,
            NullLogger<Neo4jDataAccess>.Instance))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullDatabaseName_ThrowsException()
    {
        // Arrange
        IOptions<GraphDatabaseSettings> options = new OptionsWrapper<GraphDatabaseSettings>(new GraphDatabaseSettings());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        FluentActions.Invoking(() => new TestNeo4jDataAccess(Mock.Of<IDriver>(),
            options,
            null,
            NullLogger<Neo4jDataAccess>.Instance))
            .Should().Throw<ArgumentNullException>();
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public async Task ExecuteReadListAsync_Invokes_RunAsync_AndReturnsMappedValues()
    {
        // Arrange
        var query = "MATCH (n) RETURN n.name AS name";
        var returnKey = "name";
        var expectedValue = "Alice";

        var recordMock = new Mock<IRecord>();
        recordMock.Setup(r => r.Values).Returns(new Dictionary<string, object> { { returnKey, expectedValue } });

        var mockAsyncEnumerableRecords = TestMocks.MockAsyncEnumerable([recordMock.Object]);

        var cursorMock = new Mock<IResultCursor>();
        cursorMock
            .Setup(cursor => cursor.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => mockAsyncEnumerableRecords.GetAsyncEnumerator());

        var txMock = new Mock<IAsyncTransaction>();
        txMock
            .Setup(t => t.RunAsync(It.Is<string>(q => q == query), It.IsAny<IDictionary<string, object>?>()))
            .ReturnsAsync(() => cursorMock.Object);

        var sessionMock = new Mock<IAsyncSession>();
        sessionMock
            .Setup(s => s.ExecuteReadAsync(It.IsAny<Func<IAsyncQueryRunner, Task<List<string>>>>(), It.IsAny<Action<TransactionConfigBuilder>>()))
            .Returns<Func<IAsyncTransaction, Task<List<string>>>, Action<TransactionConfigBuilder>>((callback, _) => callback(txMock.Object));

        var driverMock = new Mock<IDriver>();
        driverMock
            .Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
            .Returns(sessionMock.Object);

        var options = Options.Create(new GraphDatabaseSettings());
        var loggerMock = new Mock<ILogger<Neo4jDataAccess>>();

        var sut = new TestNeo4jDataAccess(driverMock.Object, options, TEST_DATABASE_NAME, loggerMock.Object);

        // Act
        await using (sut.ConfigureAwait(false))
        {
            var result = await sut.ExecuteReadListAsync(query, returnKey);

            // Assert
            result.Should().ContainSingle().Which.Should().Be(expectedValue);
            txMock.Verify(t => t.RunAsync(query, It.IsAny<IDictionary<string, object>?>()), Times.Once);
            txMock.Verify(t => t.RunAsync(query, It.Is<IDictionary<string, object>?>(dic => dic != null && dic.Count == 0)), Times.Once);
        }
    }

    [Fact]
    public async Task ExecuteReadListAsync_WithParameters_Invokes_RunAsync_AndReturnsMappedValues()
    {
        // Arrange
        var query = "MATCH (p:Person {alias: $name}) RETURN p.name AS name";
        var parameter = "name";
        var parameterValue = "Bob";
        var returnKey = "name";
        var expectedValue = "Alice";

        var parameters = new Dictionary<string, object> { { parameter, parameterValue } };

        var recordMock = new Mock<IRecord>();
        recordMock.Setup(r => r.Values).Returns(new Dictionary<string, object> { { returnKey, expectedValue } });

        var mockAsyncEnumerableRecords = TestMocks.MockAsyncEnumerable([recordMock.Object]);

        var cursorMock = new Mock<IResultCursor>();
        cursorMock
            .Setup(cursor => cursor.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => mockAsyncEnumerableRecords.GetAsyncEnumerator());

        var txMock = new Mock<IAsyncTransaction>();
        txMock
            .Setup(t => t.RunAsync(It.Is<string>(q => q == query), It.IsAny<IDictionary<string, object>?>()))
            .ReturnsAsync(() => cursorMock.Object);

        var sessionMock = new Mock<IAsyncSession>();
        sessionMock
            .Setup(s => s.ExecuteReadAsync(It.IsAny<Func<IAsyncQueryRunner, Task<List<string>>>>(), It.IsAny<Action<TransactionConfigBuilder>>()))
            .Returns<Func<IAsyncTransaction, Task<List<string>>>, Action<TransactionConfigBuilder>>((callback, _) => callback(txMock.Object));

        var driverMock = new Mock<IDriver>();
        driverMock
            .Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
            .Returns(sessionMock.Object);

        var options = Options.Create(new GraphDatabaseSettings());
        var loggerMock = new Mock<ILogger<Neo4jDataAccess>>();

        var sut = new TestNeo4jDataAccess(driverMock.Object, options, TEST_DATABASE_NAME, loggerMock.Object);

        // Act
        await using (sut.ConfigureAwait(false))
        {
            var result = await sut.ExecuteReadListAsync(query, returnKey, parameters);

            // Assert
            result.Should().ContainSingle().Which.Should().Be(expectedValue);
            txMock.Verify(t => t.RunAsync(query, It.IsAny<IDictionary<string, object>?>()), Times.Once);
            txMock.Verify(t => t.RunAsync(query, It.Is<IDictionary<string, object>?>(dic => dic != null && dic.Count == 1)), Times.Once);
            txMock.Verify(t => t.RunAsync(query, It.Is<IDictionary<string, object>?>(dic => dic != null && dic.ContainsKey(parameter) && dic[parameter] as string == parameterValue)), Times.Once);
        }
    }
}