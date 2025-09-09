using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class GraphDataService : IGraphDataService
{
    public Task<List<string>> GetNodesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new List<string> { "NodeA", "NodeB", "NodeC" });

    public Task<List<(string Source, string Target)>> GetEdgesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new List<(string, string)>
        {
            ("NodeA", "NodeB"),
            ("NodeB", "NodeC")
        });
}
