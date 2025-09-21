using SK.GraphRag.Application.Services;
using Neo4j.Driver;

namespace SK.GraphRag.Application.UnitTests.Services;

public class GraphDataServiceTests
{
    private readonly Mock<IDriver> _mockDriver;

    private readonly GraphDataService _sut;

    public GraphDataServiceTests()
    {
        _mockDriver = new Mock<IDriver>();
        _sut = new GraphDataService(_mockDriver.Object);
    }

    [Fact]
    public async Task GetNodesAsync_ReturnsSampleNodes()
    {
        var nodes = await _sut.GetNodesAsync(TestContext.Current.CancellationToken);
        nodes.Should().BeEquivalentTo(["NodeA", "NodeB", "NodeC"]);
    }
}
