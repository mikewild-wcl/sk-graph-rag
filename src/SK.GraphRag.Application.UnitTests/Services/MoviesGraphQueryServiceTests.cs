using Microsoft.Extensions.Logging.Abstractions;
using Neo4j.Driver;
using SK.GraphRag.Application.Services;

namespace SK.GraphRag.Application.UnitTests.Services;

public class MoviesGraphQueryServiceTests
{
    private readonly Mock<IDriver> _mockDriver;

    private readonly MoviesGraphQueryService _sut;

    public MoviesGraphQueryServiceTests()
    {
        _mockDriver = new Mock<IDriver>();
        _sut = new MoviesGraphQueryService(
            _mockDriver.Object,
            new NullLogger<MoviesGraphQueryService>());
    }

    [Fact]
    public async Task GetMoviesForActor_ReturnsEmptyList_WhenUnknown()
    {
        var movies = await _sut.GetMoviesForActor("Unknown", TestContext.Current.CancellationToken);
        movies.Should().NotBeNull();
        movies.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMoviesForActor_ReturnsEmptyList_Known()
    {
        var movies = await _sut.GetMoviesForActor("Well known actor", TestContext.Current.CancellationToken);
        movies.Should().NotBeNull();
        //movies.Should().BeEquivalentTo(["Movie 1", "Movie 2", "Movie 3"]);
    }

}
