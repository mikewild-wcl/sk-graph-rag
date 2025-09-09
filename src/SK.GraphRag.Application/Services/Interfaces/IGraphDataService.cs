namespace SK.GraphRag.Application.Services.Interfaces;

public interface IGraphDataService
{
    Task<List<string>> GetNodesAsync(CancellationToken cancellationToken = default);

    Task<List<(string Source, string Target)>> GetEdgesAsync(CancellationToken cancellationToken = default);
}
