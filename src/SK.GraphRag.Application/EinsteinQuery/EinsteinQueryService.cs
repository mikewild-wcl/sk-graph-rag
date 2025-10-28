using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.GraphRag.Application.Chunkers.Interfaces;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;
using System.Globalization;
using System.Text;

namespace SK.GraphRag.Application.EinsteinQuery;

public sealed class EinsteinQueryService(
    IDownloadService downloadService,
    IDocumentChunker documentChunker,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IOptions<EinsteinQuerySettings> queryOptions,
    ILogger<EinsteinQueryService> logger) : IEinsteinQueryService
{
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly EinsteinQuerySettings _querySettings = queryOptions.Value;
    private readonly ILogger<EinsteinQueryService> _logger = logger;

    private static readonly Dictionary<string, string> _facts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["when was einstein born?"] = "Albert Einstein was born on March 14, 1879.",
        ["what is einstein famous for?"] = "He is famous for the theory of relativity and the equation E = mc^2.",
        ["when did einstein win the nobel prize?"] = "He received the Nobel Prize in Physics in 1921 for his explanation of the photoelectric effect.",
        ["where was einstein born?"] = "He was born in Ulm, in the Kingdom of Wrttemberg in the German Empire.",
        ["when did einstein die?"] = "He died on April 18, 1955 in Princeton, New Jersey, USA.",
    };

    private static readonly Action<ILogger, string, Exception?> _fileNotFoundLog =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(LoadData)),
            "The file {File} was not found in the downloads directory");

    private static readonly Action<ILogger, string, Exception?> _logLoadDataCalled =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(LoadData)),
            "LoadData called: {Message}");

    private static readonly Action<ILogger, string, string, Exception?> _logLoadDataComplete =
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

        var embedding = await _embeddingGenerator.GenerateVectorAsync(question.Trim(), cancellationToken: cancellationToken).ConfigureAwait(false);
        LogEmbedding(embedding);

        //TODO: Query Graph DB for relevant chunks
        //TODO: Call MAF chat completion

        var key = question.Trim();
        if (_facts.TryGetValue(key, out var answer))
        {
            return answer;
        }

        return "I don't have an answer. Try asking about birth, Nobel Prize, or famous works.";
    }

    public async Task LoadData(CancellationToken cancellationToken = default)
    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates - can remove this when all logging is moved to delegates
        _logLoadDataCalled(_logger, "LoadData called", null);

        await _downloadService.DownloadFileIfNotExists(_querySettings.DocumentUri, _querySettings.DocumentFileName, cancellationToken).ConfigureAwait(false);

        if (!_downloadService.TryGetDownloadedFilePath(_querySettings.DocumentFileName, out var filePath) || filePath is null)
        {
            _fileNotFoundLog(_logger, _querySettings.DocumentFileName, null);
            return;
        }

        await foreach (var chunk in documentChunker.StreamTextChunks(filePath, cancellationToken).ConfigureAwait(true))
        {
            _logger.LogInformation("Chunk: {Chunk}", chunk);
            var embedding = await _embeddingGenerator.GenerateVectorAsync(chunk, cancellationToken: cancellationToken).ConfigureAwait(false);
            LogEmbedding(embedding);
        }

        //TODO: create embeddings, and save to Graph DB
        //      Need a PdfParserService/ChunkingService/EmbeddingService, GraphDataService
        //      Consider making a EinsteinDataIngestionService to handle this workflow
        _logLoadDataComplete(_logger, _querySettings.DocumentUri.ToString(), _querySettings.DocumentFileName, null);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
    }

    private void LogEmbedding(ReadOnlyMemory<float> embedding)
    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates - can remove this when all logging is moved to delegates
        if (embedding.Length > 0)
        {
            var arr = embedding.ToArray();
            _logger.LogInformation("Embedding array length: {Length}", arr.Length);
            var embeddingString = new StringBuilder("[");
            for (int i = 0; i < Math.Min(arr.Length, 8); i++)
            {
                embeddingString.Append(CultureInfo.InvariantCulture, $"{arr[i]}");
            }
            embeddingString.Append($" ... ]");
            _logger.LogInformation("Embedding {EmbeddingString}", embeddingString);
        }
#pragma warning restore CA1848 // Use the LoggerMessage delegates
    }
}
