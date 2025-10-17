using SK.GraphRag.Application.Services;
using Neo4j.Driver;

namespace SK.GraphRag.Application.UnitTests.Services;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", 
    Justification = "<Pending> - can remove this attribute when tests are implemented")]
public class GraphDataServiceTests
{
    private readonly Mock<IDriver> _mockDriver;

    private readonly GraphDataService _sut;

    public GraphDataServiceTests()
    {
        _mockDriver = new Mock<IDriver>();
        _sut = new GraphDataService(_mockDriver.Object);
    }
}
