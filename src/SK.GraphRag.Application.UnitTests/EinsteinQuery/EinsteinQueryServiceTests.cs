using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.GraphRag.Application.Chunkers.Interfaces;
using SK.GraphRag.Application.EinsteinQuery;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.UnitTests.EinsteinQuery;

public class EinsteinQueryServiceTests
{
    private const string DOCUMENT_FILE_NAME = "Test.pdf";
    private readonly Uri DOCUMENT_URI = new Uri("http://localhost/test");

    private readonly Mock<IEinsteinQueryDataAccess> _mockDataAccess;
    private readonly Mock<IDownloadService> _mockDownloadService;
    private readonly Mock<IDocumentChunker> _mockDocumentChunker;
    private readonly Mock<IEmbeddingGenerator<string, Embedding<float>>> _mockEmbeddingGenerator;

    private readonly Mock<ILogger<EinsteinQueryService>> _mockLogger;

    private readonly EinsteinQueryService _sut;

    public EinsteinQueryServiceTests()
    {
        _mockDataAccess = new Mock<IEinsteinQueryDataAccess>();
        _mockDownloadService = new Mock<IDownloadService>();
        _mockDocumentChunker = new Mock<IDocumentChunker>();
        _mockEmbeddingGenerator = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();

        IOptions<EinsteinQuerySettings> options = new OptionsWrapper<EinsteinQuerySettings>(
            new EinsteinQuerySettings
            {
                DocumentUri = DOCUMENT_URI,
                DocumentFileName = DOCUMENT_FILE_NAME
            });

        _mockLogger = new Mock<ILogger<EinsteinQueryService>>();

        _sut = new EinsteinQueryService(
            _mockDataAccess.Object,
            _mockDownloadService.Object,
            _mockDocumentChunker.Object,
            _mockEmbeddingGenerator.Object,
            options,
            _mockLogger.Object);
    }
}
