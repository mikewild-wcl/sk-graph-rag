using Neo4j.Driver;
using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class MoviesGraphQueryService(
    IDriver driver) : IMoviesGraphQueryService
{
    private readonly IDriver _driver = driver;

    public async Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default)
    {
        var movieNames = new List<string>();
        var query = @"
            MATCH (a:Actor {name: $actorName})-[:ACTED_IN]->(m:Movie)
            RETURN m.title AS movieTitle
        ";

        //await using var session = _driver.AsyncSession().ConfigureAwait(false);

        //var result = await session.RunAsync(query, new { actorName }).ConfigureAwait(false);
        //await result.ForEachAsync(record =>
        //{
        //    movieNames.Add(record["movieTitle"].As<string>());
        //}, cancellationToken).ConfigureAwait(false);

        return movieNames;
    }
}
