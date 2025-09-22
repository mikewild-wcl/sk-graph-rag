using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class MoviesGraphQueryService(
    IDriver driver,
    ILogger<MoviesGraphQueryService> logger) : IMoviesGraphQueryService
{
    private readonly IDriver _driver = driver;
    private readonly ILogger<MoviesGraphQueryService> _logger = logger;

    public async Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default)
    {
        var movieNames = new List<string>();

        try
        {
            await _driver.VerifyConnectivityAsync().ConfigureAwait(false);

            //await using var session = _driver.AsyncSession().ConfigureAwait(false);

            var result = await driver.ExecutableQuery(@"
                MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie)
                RETURN m.title AS movieTitle
                ")
                .WithParameters(new { name = actorName })
                .WithConfig(new QueryConfig(database: "neo4j"))
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            // Loop through results and print people's name
            foreach (var record in result.Result)
            {
                Console.WriteLine(record.Get<string>("name"));
            }

            /*
            // Using mapping - 
            var result = await driver.ExecutableQuery(@"
                MATCH (p:Person)-[:KNOWS]->(:Person)
                RETURN p.name AS name
                ")
                .WithConfig(new QueryConfig(database: "<database-name>"))
                .WithMap(record => record["name"].As<string>())
                .ExecuteAsync();
            foreach (var name in result.Result) {
                Console.WriteLine(name);
            }             
            */

            movieNames.AddRange(result.Result.Select(r => r.Get<string>("movieTitle")));
            //var result = await session.RunAsync(query, new { actorName }).ConfigureAwait(false);
            //await result.ForEachAsync(record =>
            //{
            //    movieNames.Add(record["movieTitle"].As<string>());
            //}, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying movies for actor {ActorName}", actorName);
            throw;
        }
        finally
        {
            /* _driver.CloseAsync(); */
        }

        return movieNames;
    }
}
