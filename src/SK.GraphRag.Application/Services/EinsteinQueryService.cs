using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.Services;

public sealed class EinsteinQueryService(
    IDownloadService downloadService,
    IOptions<EinsteinQuerySettings> queryOptions,
    ILogger<EinsteinQueryService> logger,
    HttpClient httpClient) : IEinsteinQueryService
{
    private readonly IDownloadService _downloadService = downloadService;
    private readonly EinsteinQuerySettings _querySettings = queryOptions.Value;
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<EinsteinQueryService> _logger = logger;

    private static readonly Dictionary<string, string> _facts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["when was einstein born?"] = "Albert Einstein was born on March 14, 1879.",
        ["what is einstein famous for?"] = "He is famous for the theory of relativity and the equation E = mc^2.",
        ["when did einstein win the nobel prize?"] = "He received the Nobel Prize in Physics in 1921 for his explanation of the photoelectric effect.",
        ["where was einstein born?"] = "He was born in Ulm, in the Kingdom of Wrttemberg in the German Empire.",
        ["when did einstein die?"] = "He died on April 18, 1955 in Princeton, New Jersey, USA.",
    };

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, Exception?> _logLoadDataCalled =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(LoadData)),
            "LoadData called: {Message}");

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, string, Exception?> _logLoadDataComplete =
    LoggerMessage.Define<string, string>(
        LogLevel.Information,
        new EventId(1, nameof(LoadData)),
        "File load complete for uri {Uri} file {FileName}");

    public async Task<string> Ask(string question, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Please ask a question about Albert Einstein.";
        }

        var key = question.Trim();

        //TODO: Create embedding from question
        //TODO: Query Graph DB for relevant chunks
        //TODO: Call MAF chat completion

        if (_facts.TryGetValue(key, out var answer))
        {
            return answer;
        }

        return "I don't have an answer. Try asking about birth, Nobel Prize, or famous works.";
    }

    public async Task LoadData()
    {
        _logLoadDataCalled(_logger, "LoadData called", null);

        await _downloadService.DownloadFileIfNotExists(_querySettings.DocumentUri, _querySettings.DocumentFileName).ConfigureAwait(false);

        //TODO: Parse file, Chunk the file, create embeddings, and save to Graph DB
        //      Need a PdfParserService/ChunkingService/EmbeddingService, GraphDataService
        //      Consider making a EinsteinDataIngestionService to handle this workflow
        _logLoadDataComplete(_logger, _querySettings.DocumentUri.ToString(), _querySettings.DocumentFileName, null);
    }
}
