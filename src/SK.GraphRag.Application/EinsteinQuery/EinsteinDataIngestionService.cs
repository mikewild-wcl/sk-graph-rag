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

public sealed class EinsteinDataIngestionService(
    IEinsteinQueryDataAccess dataAccess,
    IDownloadService downloadService,
    IDocumentChunker documentChunker,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IOptions<EinsteinQuerySettings> queryOptions,
    ILogger<EinsteinDataIngestionService> logger) : IEinsteinDataIngestionService
{
    private readonly IEinsteinQueryDataAccess _dataAccess = dataAccess;
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IDocumentChunker _documentChunker = documentChunker;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly EinsteinQuerySettings _querySettings = queryOptions.Value;
    private readonly ILogger<EinsteinDataIngestionService> _logger = logger;

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
        
    public async Task LoadData(CancellationToken cancellationToken = default)
    {
        //TODO: Move data load to EinsteinDataIngestionService to handle this workflow

#pragma warning disable CA1848 // Use the LoggerMessage delegates - can remove this when all logging is moved to delegates
        _logLoadDataCalled(_logger, "LoadData called", null);

        await _downloadService.DownloadFileIfNotExists(_querySettings.DocumentUri, _querySettings.DocumentFileName, cancellationToken).ConfigureAwait(false);

        if (!_downloadService.TryGetDownloadedFilePath(_querySettings.DocumentFileName, out var filePath) || filePath is null)
        {
            _fileNotFoundLog(_logger, _querySettings.DocumentFileName, null);
            return;
        }

        await _dataAccess.RemoveExistingData().ConfigureAwait(false);
        await _dataAccess.CreateVectorIndexIfNotExists().ConfigureAwait(false);

        var chunks = new List<string>();
        var embeddings = new List<ReadOnlyMemory<float>>();

        await foreach (var chunk in _documentChunker.StreamTextChunks(filePath, cancellationToken).ConfigureAwait(true))
        {
            if(string.IsNullOrWhiteSpace(chunk))
            {
                continue;
            }

            _logger.LogInformation("Chunk: {Chunk}", chunk);
            var embedding = await _embeddingGenerator.GenerateVectorAsync(chunk, cancellationToken: cancellationToken).ConfigureAwait(false);
            LogEmbedding(embedding);

            chunks.Add(chunk);
            embeddings.Add(embedding);
        }

        await _dataAccess.SaveTextChunks(chunks, embeddings).ConfigureAwait(false);

        await _dataAccess.CreateFullTextIndexIfNotExists().ConfigureAwait(false);

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
