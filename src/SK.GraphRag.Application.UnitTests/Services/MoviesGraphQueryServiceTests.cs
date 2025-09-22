using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SK.GraphRag.Application.Services;
using System.Reflection;

namespace SK.GraphRag.Application.UnitTests.Services;

public class MoviesGraphQueryServiceTests
{
    private readonly Mock<IDriver> _mockDriver;

    private readonly Mock<IExecutableQuery<IRecord, IRecord>> _mockExecutableQuery;

    private readonly Mock<ILogger<MoviesGraphQueryService>> _mockLogger;

    private readonly MoviesGraphQueryService _sut;

    public MoviesGraphQueryServiceTests()
    {
        _mockExecutableQuery = new Mock<IExecutableQuery<IRecord, IRecord>>();
        _mockExecutableQuery.Setup(q => q.WithParameters(It.IsAny<object>())).Returns(_mockExecutableQuery.Object);
        _mockExecutableQuery.Setup(q => q.WithConfig(It.IsAny<QueryConfig>())).Returns(_mockExecutableQuery.Object);

        _mockDriver = new Mock<IDriver>();

        _mockDriver.Setup(d => d.ExecutableQuery(It.IsAny<string>())).Returns(_mockExecutableQuery.Object);

        _mockLogger = new Mock<ILogger<MoviesGraphQueryService>>();
        _sut = new MoviesGraphQueryService(
            _mockDriver.Object,
            _mockLogger.Object);
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

        _mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ConstructEagerResult(mockRecords));

        // Act
        var result = await _sut.GetMoviesForActor(actorName, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedTitles);
    }

    [Fact]
    public async Task GetMoviesForActor_ReturnsEmptyMovieTitles_ForUnknownActor()
    {
        // Arrange
        var actorName = "Unkown";
        var mockRecords = Enumerable.Empty<IRecord>().ToList();

        _mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ConstructEagerResult(mockRecords));

        // Act
        var result = await _sut.GetMoviesForActor(actorName, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
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
}
