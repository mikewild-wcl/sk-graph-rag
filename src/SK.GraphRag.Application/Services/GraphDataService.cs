using Neo4j.Driver;
using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class GraphDataService(
    IDriver driver) : IGraphDataService
{
    private readonly IDriver _driver = driver;

    public Task<List<string>> GetNodesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new List<string> { "NodeA", "NodeB", "NodeC" });

    public Task<List<(string Source, string Target)>> GetEdgesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new List<(string, string)>
        {
            ("NodeA", "NodeB"),
            ("NodeB", "NodeC")
        });

    public async Task<List<string>> SimpleQuery(CancellationToken cancellationToken = default)
    {
        //await using var session = _driver.AsyncSession(s => s.WithDatabase("system")).ConfigureAwait(false);
        await using var session = _driver.AsyncSession().ConfigureAwait(false);

        return [];
    }
}
