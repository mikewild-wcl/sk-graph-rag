using Agentic.GraphRag.Application.Data;
using Agentic.GraphRag.Application.Movies.Interfaces;
using Agentic.GraphRag.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Agentic.GraphRag.Application.Movies;

public class MoviesDataAccess : Neo4jDataAccess, IMoviesDataAccess
{
#pragma warning disable IDE0290 // Use primary constructor - disabled because of code passing options base class constructor call
    public MoviesDataAccess(
        IDriver driver,
        IOptions<GraphDatabaseSettings> options,
        ILogger<MoviesDataAccess> logger)
        : base(
            driver,
            options?.Value?.MoviesDb ?? GraphDatabaseSettings.DefaultDb,
            logger)
    {
    }
#pragma warning restore IDE0290 // Use primary constructor
}
