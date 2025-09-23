using Neo4j.Driver;
using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class GraphDataService(
    IDriver driver) : IGraphDataService
{
    // Intentionally left for future expansion; suppress unused field warning.
#pragma warning disable CA1823 // Used in future implementations
    private readonly IDriver _driver = driver;
#pragma warning restore CA1823
}
