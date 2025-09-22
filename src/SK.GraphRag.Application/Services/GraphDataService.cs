using Neo4j.Driver;
using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class GraphDataService(
    IDriver driver) : IGraphDataService
{
    private readonly IDriver _driver = driver;
}
