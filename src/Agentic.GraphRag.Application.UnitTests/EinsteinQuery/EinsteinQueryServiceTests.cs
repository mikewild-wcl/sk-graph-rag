using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SK.GraphRag.Application.EinsteinQuery;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;

namespace SK.GraphRag.Application.UnitTests.EinsteinQuery;

public class EinsteinQueryServiceTests
{
    private readonly Mock<IChatClient> _mockChatClient;
    private readonly Mock<IEinsteinQueryDataAccess> _mockDataAccess;
    private readonly Mock<IEmbeddingGenerator<string, Embedding<float>>> _mockEmbeddingGenerator;

    private readonly Mock<ILogger<EinsteinQueryService>> _mockLogger;

    private readonly EinsteinQueryService _sut;

    public EinsteinQueryServiceTests()
    {
        _mockChatClient = new Mock<IChatClient>();
        _mockDataAccess = new Mock<IEinsteinQueryDataAccess>();
        _mockEmbeddingGenerator = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();
        _mockLogger = new Mock<ILogger<EinsteinQueryService>>();

        _sut = new EinsteinQueryService(
            _mockChatClient.Object,
            _mockDataAccess.Object,
            _mockEmbeddingGenerator.Object,
            _mockLogger.Object);
    }


    [Fact]
    public void Constructor_Succeeds()
    {
        // Assert
        _sut.Should().NotBeNull();
    }
}
