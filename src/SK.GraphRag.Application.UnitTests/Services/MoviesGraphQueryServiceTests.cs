using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Neo4j.Driver;
using SK.GraphRag.Application.Services;
using System.Reflection;

namespace SK.GraphRag.Application.UnitTests.Services;

public class MoviesGraphQueryServiceTests
{
    private readonly Mock<IDriver> _mockDriver;
    private readonly Mock<ILogger<MoviesGraphQueryService>> _mockLogger;

    private readonly MoviesGraphQueryService _sut;

    public MoviesGraphQueryServiceTests()
    {
        _mockDriver = new Mock<IDriver>();
        _mockLogger = new Mock<ILogger<MoviesGraphQueryService>>();
        _sut = new MoviesGraphQueryService(
            _mockDriver.Object,
            new NullLogger<MoviesGraphQueryService>());
    }
       
    [Fact]
    public async Task GetMoviesForActor_ReturnsMovieTitles()
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

        var eagerResult = CreateEagerResult(mockRecords);

        var mockExecutableQuery = new Mock<IExecutableQuery<IRecord, IRecord>>();
        mockExecutableQuery.Setup(q => q.WithParameters(It.IsAny<object>())).Returns(mockExecutableQuery.Object);
        mockExecutableQuery.Setup(q => q.WithConfig(It.IsAny<QueryConfig>())).Returns(mockExecutableQuery.Object);
        
        mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(eagerResult);

        var mockDriver = new Mock<IDriver>();
        mockDriver.Setup(d => d.VerifyConnectivityAsync()).Returns(Task.CompletedTask);
        mockDriver.Setup(d => d.ExecutableQuery(It.IsAny<string>())).Returns(mockExecutableQuery.Object);

        var service = new MoviesGraphQueryService(mockDriver.Object, _mockLogger.Object);

        // Act
        var result = await service.GetMoviesForActor(actorName, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedTitles);
    }

    [Fact]
    public async Task GetMoviesForActor_ReturnsEmptyMovieTitles_ForUnknownActor()
    {
        // Arrange
        var actorName = "Unkown";
        var mockRecords = Enumerable.Empty<IRecord>().ToList();
        var eagerResult = CreateEagerResult(mockRecords);

        var mockExecutableQuery = new Mock<IExecutableQuery<IRecord, IRecord>>();
        mockExecutableQuery.Setup(q => q.WithParameters(It.IsAny<object>())).Returns(mockExecutableQuery.Object);
        mockExecutableQuery.Setup(q => q.WithConfig(It.IsAny<QueryConfig>())).Returns(mockExecutableQuery.Object);

        mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(eagerResult);

        var mockDriver = new Mock<IDriver>();
        mockDriver.Setup(d => d.VerifyConnectivityAsync()).Returns(Task.CompletedTask);
        mockDriver.Setup(d => d.ExecutableQuery(It.IsAny<string>())).Returns(mockExecutableQuery.Object);

        var service = new MoviesGraphQueryService(mockDriver.Object, _mockLogger.Object);

        // Act
        var result = await service.GetMoviesForActor(actorName, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

    }

    private static EagerResult<IReadOnlyList<IRecord>> CreateEagerResult(
        IReadOnlyList<IRecord> records,
        IResultSummary summary = null,
        string[] keys = null)
    {
        var type = typeof(EagerResult<IReadOnlyList<IRecord>>);
        var ctor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            [typeof(IReadOnlyList<IRecord>), typeof(IResultSummary), typeof(string[])],
            modifiers: null
        ) ?? throw new InvalidOperationException("EagerResult constructor not found.");

        return (EagerResult<IReadOnlyList<IRecord>>)ctor.Invoke(new object[] { records, summary, keys ?? Array.Empty<string>() });
    }
}
