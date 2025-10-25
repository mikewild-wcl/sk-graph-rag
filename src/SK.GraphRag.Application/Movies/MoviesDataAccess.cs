using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Data;
using SK.GraphRag.Application.Movies.Interfaces;
using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.Movies;

public class MoviesDataAccess : Neo4jDataAccess, IMoviesDataAccess
{
    public MoviesDataAccess(
        IDriver driver,
        IOptions<GraphDatabaseSettings> options,
        ILogger<MoviesDataAccess> logger)
        : base(
            driver,
            options, 
            options?.Value?.MoviesDb ?? GraphDatabaseSettings.DefaultDb,
            logger)
    {
        // _database = options.Value.MoviesDb;
    }
}
