using Microsoft.Extensions.Logging;
using SK.GraphRag.Application.Movies;
using SK.GraphRag.Application.Movies.Interfaces;

namespace SK.GraphRag.Application.UnitTests.Movies;

public class MoviesGraphQueryServiceTests
{
    //private readonly Mock<IDriver> _mockDriver;
    private readonly Mock<IMoviesDataAccess> _mockDataAccess;
    //private readonly Mock<IExecutableQuery<IRecord, IRecord>> _mockExecutableQuery;
    private readonly Mock<ILogger<MoviesGraphQueryService>> _mockLogger;

    private readonly MoviesGraphQueryService _sut;

    public MoviesGraphQueryServiceTests()
    {
        //_mockExecutableQuery = new Mock<IExecutableQuery<IRecord, IRecord>>();
        //_mockExecutableQuery.Setup(q => q.WithParameters(It.IsAny<object>())).Returns(_mockExecutableQuery.Object);
        //_mockExecutableQuery.Setup(q => q.WithConfig(It.IsAny<QueryConfig>())).Returns(_mockExecutableQuery.Object);

        _mockDataAccess = new Mock<IMoviesDataAccess>();

        //_mockDriver = new Mock<IDriver>();
        //_mockDriver.Setup(d => d.ExecutableQuery(It.IsAny<string>())).Returns(_mockExecutableQuery.Object);

        _mockLogger = new Mock<ILogger<MoviesGraphQueryService>>();

        _sut = new MoviesGraphQueryService(
            _mockDataAccess.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetMoviesForActor_ReturnsMovieTitles()
    {
        // Arrange
        var actorName = "Tom Hanks";
        var expectedTitles = new List<string> { "Forrest Gump", "Cast Away" };

        //var mockRecords = expectedTitles.Select(title =>
        //{
        //    var mockRecord = new Mock<IRecord>();
        //    mockRecord.Setup(r => r.Get<string>("movieTitle")).Returns(title);
        //    return mockRecord.Object;
        //}).ToList();

        //_mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(ConstructEagerResult(mockRecords));

        _mockDataAccess.Setup(x => x.ExecuteReadListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>?>()))
            .ReturnsAsync(expectedTitles);        

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

        //_mockExecutableQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(ConstructEagerResult(Enumerable.Empty<IRecord>().ToList()));

        _mockDataAccess.Setup(x => x.ExecuteReadListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>?>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetMoviesForActor(actorName, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
