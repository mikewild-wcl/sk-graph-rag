using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Movies;
using SK.GraphRag.Application.Settings;
using System;
using System.Reflection;

namespace SK.GraphRag.Application.UnitTests.Movies;

public class MoviesDataAccessTests : IAsyncDisposable
{
    private const string DATABASE_NAME = "moviesDb";

    private readonly Mock<IDriver> _mockDriver;
    private readonly Mock<IExecutableQuery<IRecord, IRecord>> _mockExecutableQuery;
    private readonly Mock<ILogger<MoviesDataAccess>> _mockLogger;
    private readonly Mock<IResultCursor> _mockResultCursor;
    private readonly Mock<IAsyncSession> _mockSession;

    private readonly MoviesDataAccess _sut;

    public MoviesDataAccessTests()
    {
        _mockExecutableQuery = new Mock<IExecutableQuery<IRecord, IRecord>>();
        _mockExecutableQuery.Setup(q => q.WithParameters(It.IsAny<object>())).Returns(_mockExecutableQuery.Object);
        _mockExecutableQuery.Setup(q => q.WithConfig(It.IsAny<QueryConfig>())).Returns(_mockExecutableQuery.Object);

        _mockResultCursor = new Mock<IResultCursor>();

        // Mock the result cursor to return fake data
        _mockResultCursor
            .SetupSequence(cursor => cursor.FetchAsync())
            .ReturnsAsync(true)  // First record
            .ReturnsAsync(false); // No more records

        var mockMovie = new Mock<IRecord>();
        mockMovie.Setup(r => r.Get<string>("movieTitle"))
            .Returns("Forrest Gump");

        _mockResultCursor
            .Setup(cursor => cursor.Current)
            .Returns(mockMovie.Object);

        var expectedResult = new[] { "Node1", "Node2" };
        //_mockResultCursor
        //    .Setup(cursor => cursor
        //            .ToListAsync(record => AsString(record["name"])
        //                                   //.ToString()
        //                                   //.As<string>()
        //                                   , It.IsAny<int>(),
        //                                   It.IsAny<CancellationToken>())
        //    ; //.Returns(expectedResult);

        //_mockResultCursor
        //    .Setup(cursor => cursor["movieTitle"].As<string>())
        //    .Returns("Sample Title");

        _mockSession = new Mock<IAsyncSession>();
        /*
        _mockSession
            .Setup(session => session.ExecuteReadAsync(It.IsAny<Func<IAsyncQueryRunner, Task<List<string>>>>(), It.IsAny<Action<TransactionConfigBuilder>>()))
            .ReturnsAsync<Func<IAsyncQueryRunner, Task<List<string>>>, Task<Task<Action<TransactionConfigBuilder>>>>(async (func, _) =>
            {
                var mockTransaction = new Mock<IAsyncTransaction>();
                return await func(mockTransaction.Object)
                    .ConfigureAwait(false)
                    ;
            });

        _mockSession
            .Setup(session => session.ExecuteReadAsync(It.IsAny<Func<IAsyncQueryRunner, Task<IResultCursor>>>(), It.IsAny<Action<TransactionConfigBuilder>>()))
            //.ReturnsAsync<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>>> (async (func, _) =>
            //.ReturnsAsync(async (Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>> func, Task<Action<TransactionConfigBuilder>> act) =>
            //.ReturnsAsync<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>>> (async (func, act) =>
            .ReturnsAsync<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>>> (async (func, _) =>
            {
                var mockTransaction = new Mock<IAsyncTransaction>();
                return await func(mockTransaction.Object)
                    .ConfigureAwait(false)
                    ;
            })
        *****/
            /*
             * https://stackoverflow.com/questions/31527394/moq-returnsasync-with-parameters
             *
             *
    Task<TResult> ExecuteReadAsync<TResult>(Func<IAsyncQueryRunner, Task<TResult>> work, Action<TransactionConfigBuilder> action = null);
             *
             *
             *'ISetup<IAsyncSession, Task<IResultCursor>>' does not contain a definition for 'ReturnsAsync' and the best extension method overload 
             *'ReturnsExtensions.ReturnsAsync<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>>>(IReturns<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Task<Action<TransactionConfigBuilder>>>>, Task<Action<TransactionConfigBuilder>>)'
             *requires a receiver of type 
             *'Moq.Language.IReturns
             *  <System.Func<IAsyncQueryRunner, Task<IResultCursor>>, 
             *  Task<Task<System.Action<TransactionConfigBuilder>>>>'
             *
             *
'ISetup<IAsyncSession, Task<IResultCursor>>' does not contain a definition for 'ReturnsAsync' and the best extension method overload 'ReturnsExtensions.ReturnsAsync<Func<IAsyncQueryRunner, Task<IResultCursor>>, Action<TransactionConfigBuilder>>(IReturns<Func<IAsyncQueryRunner, Task<IResultCursor>>, Task<Action<TransactionConfigBuilder>>>, Action<TransactionConfigBuilder>)' 
            requires a receiver of type 
            'Moq.Language.IReturns<System.Func<Neo4j.Driver.IAsyncQueryRunner, System.Threading.Tasks.Task<Neo4j.Driver.IResultCursor>>, System.Threading.Tasks.Task<System.Action<Neo4j.Driver.TransactionConfigBuilder>>>'
            '<Func<IAsyncQueryRunner, IResultCursor>>, 
             Task<Action<TransactionConfigBuilder>>>'
            */
            //.Returns<Func<IAsyncQueryRunner, Task<IResultCursor>>, Action<TransactionConfigBuilder>> (async (func, _) =>
            //{
            //    var mockTransaction = new Mock<IAsyncTransaction>();
            //    return await func(mockTransaction.Object).ConfigureAwait(true);
            //})
            ;

        _mockDriver = new Mock<IDriver>();
        _mockDriver.Setup(d => d.ExecutableQuery(It.IsAny<string>())).Returns(_mockExecutableQuery.Object);
        _mockDriver.Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>())).Returns(_mockSession.Object);

        _mockLogger = new Mock<ILogger<MoviesDataAccess>>();

        IOptions<GraphDatabaseSettings> options = new OptionsWrapper<GraphDatabaseSettings>(
            new GraphDatabaseSettings
            {
                MoviesDb = DATABASE_NAME
            });

        _sut = new MoviesDataAccess(
            _mockDriver.Object,
            options,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_SetsDatabaseName()
    {
        // Assert
        _sut.Should().NotBeNull();

        const string databaseNameField = "_databaseName";
        var type = _sut.GetType();
        var field = type.GetField(databaseNameField, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? type.BaseType?.GetField(databaseNameField, BindingFlags.Instance | BindingFlags.NonPublic);

        field.Should().NotBeNull("expected a private field named '_database' on the type or its base type");

        var databaseValue = field!.GetValue(_sut) as string;
        databaseValue.Should().Be(DATABASE_NAME);
    }

    [Fact]
    public async Task ExecuteReadListAsync_ReturnsExpectedResult_ForSingleItem()
    {
        // Arrange
        var actorName = "Tom Hanks";
        var expectedTitles = new List<string> { "Forrest Gump" };

        var mockRecords = expectedTitles.Select(title =>
        {
            var mockRecord = new Mock<IRecord>();
            mockRecord.Setup(r => r.Get<string>("movieTitle")).Returns(title);
            return mockRecord.Object;
        }).ToList();

        _mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ConstructEagerResult(mockRecords));

        //_mockDataAccess.Setup(x => x.ExecuteReadListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>?>()))
        //    .ReturnsAsync(expectedTitles);

        // Act
        //var result = await _sut.ExecuteReadListAsync(actorName, TestContext.Current.CancellationToken);
        var result = await _sut.ExecuteReadListAsync(
            @"MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie) RETURN m.title AS movieTitle",
            "movieTitle",
            new Dictionary<string, object> { { "name", actorName } });
        //.ConfigureAwait(false);

        // Assert
        result.Should().BeEquivalentTo(expectedTitles);
    }

    [Fact]
    public async Task ExecuteReadListAsync_ReturnsExpectedResult_ForMultipleResults()
    {
        // Arrange
        var actorName = "Tom Hanks";
        var expectedTitles = new List<string> { "Forrest Gump", "Cast Away" };

        var mockRecords = expectedTitles.Select(title =>
        {
            var mockRecord = new Mock<IRecord>();
            mockRecord.Setup(r => r.Get<string>("movieTitle")).Returns(title);
            return mockRecord.Object;
        }).ToList();

        _mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ConstructEagerResult(mockRecords));

        //_mockDataAccess.Setup(x => x.ExecuteReadListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>?>()))
        //    .ReturnsAsync(expectedTitles);

        // Act
        //var result = await _sut.ExecuteReadListAsync(actorName, TestContext.Current.CancellationToken);
        var result = await _sut.ExecuteReadListAsync(
            @"MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie) RETURN m.title AS movieTitle",
            "movieTitle",
            new Dictionary<string, object> { { "name", actorName } });
        //.ConfigureAwait(false);

        // Assert
        result.Should().BeEquivalentTo(expectedTitles);
    }

    public async ValueTask DisposeAsync()
    {
        if (_sut is not null)
        {
            await _sut.DisposeAsync().ConfigureAwait(true);
        }

        GC.SuppressFinalize(this);
    }

    private static EagerResult<IReadOnlyList<IRecord>> ConstructEagerResult(
        IReadOnlyList<IRecord> records,
        IResultSummary? summary = null,
        string[]? keys = null)
    {
        var type = typeof(EagerResult<IReadOnlyList<IRecord>>);
        var ctor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            [typeof(IReadOnlyList<IRecord>), typeof(IResultSummary), typeof(string[])],
            modifiers: null
        ) ?? throw new InvalidOperationException("EagerResult constructor not found.");

        return (EagerResult<IReadOnlyList<IRecord>>)ctor.Invoke(
            [records, summary, keys ?? []]);
    }

    //The call is ambiguous between the following methods or properties: 'FluentAssertions.AssertionExtensions.As<TTo>(object)' and 'Neo4j.Driver.ValueExtensions.As<T>(object)'
    private static string AsString(object obj)
    {
        if (obj is null) return null;

        return Neo4j.Driver.ValueExtensions.As<string>(obj);
    }
}
