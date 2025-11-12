using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Movies;
using SK.GraphRag.Application.Settings;
using SK.GraphRag.Application.UnitTests.TestExtensions;

namespace SK.GraphRag.Application.UnitTests.Movies;

public class MoviesDataAccessTests
{
    private const string DATABASE_NAME = "moviesDb";
    private const string DATABASE_NAME_FIELD = "_databaseName";        

    [Fact]
    public async Task Constructor_SetsDatabaseName()
    {
        // Arrange
        IOptions<GraphDatabaseSettings> options = new OptionsWrapper<GraphDatabaseSettings>(
            new GraphDatabaseSettings
            {
                MoviesDb = DATABASE_NAME
            });

        var sut = new MoviesDataAccess(Mock.Of<IDriver>(),
            options,
            NullLogger<MoviesDataAccess>.Instance);

        await using (sut.ConfigureAwait(false))
        {
            // Assert
            var databaseValue = sut.GetPrivateField(DATABASE_NAME_FIELD);
            databaseValue.Should().Be(DATABASE_NAME);
        }
    }
}
