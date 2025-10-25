using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SK.GraphRag.Application.Movies.Interfaces;

namespace SK.GraphRag.Application.Movies;

public sealed class MoviesGraphQueryService(
    IDriver driver,
    IMoviesDataAccess dataAccess,
    ILogger<MoviesGraphQueryService> logger) : IMoviesGraphQueryService
{
    private readonly IDriver _driver = driver;
    private readonly IMoviesDataAccess _dataAccess = dataAccess;
    private readonly ILogger<MoviesGraphQueryService> _logger = logger;

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, Exception?> _logQueryError =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, nameof(MoviesGraphQueryService)),
            "Error querying movies for actor {ActorName}");

    public async Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default)
    {
        var movieNames = new List<string>();

        try
        {
            await _driver.VerifyConnectivityAsync().ConfigureAwait(false);

            /* OLD CODE */
            var result = await _driver.ExecutableQuery(
                @"MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie) RETURN m.title AS movieTitle")
                .WithParameters(new { name = actorName })
                .WithConfig(new QueryConfig(database: "neo4j"))
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);
            var results = result.Result.Select(r => r.Get<string>("movieTitle")).ToList();
            
            movieNames = await _dataAccess.ExecuteReadListAsync(
                @"MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie) RETURN m.title AS movieTitle",
                "movieTitle",
                new Dictionary<string, object> { { "name", actorName } })
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logQueryError(_logger, actorName, ex);
            throw;
        }
        finally
        {
            // await _driver.CloseAsync().ConfigureAwait(false);
        }

        return movieNames;
    }
}
