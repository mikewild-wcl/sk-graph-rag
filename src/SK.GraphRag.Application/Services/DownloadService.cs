using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.Services;

public sealed class DownloadService(
    IOptions<DownloadSettings> downloadOptions,
    HttpClient httpClient,
    ILogger<DownloadService> logger) : IDownloadService
{
    private readonly DownloadSettings _downloadSettings = downloadOptions.Value;
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<DownloadService> _logger = logger;

    // LoggerMessage delegates for improved performance (CA1848)
    private static readonly Action<ILogger, string, string, Exception?> _fileExistsLog =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1, nameof(DownloadFileIfNotExists)),
            "The file {File} already exists in {Dir}");

    private static readonly Action<ILogger, Uri, object, Exception?> _downloadFailedLog =
        LoggerMessage.Define<Uri, object>(
            LogLevel.Error,
            new EventId(2, nameof(DownloadFileIfNotExists)),
            "Failed to download file from {Uri}. Status: {StatusCode}");

    private static readonly Action<ILogger, string, Exception?> _downloadedLog =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3, nameof(DownloadFileIfNotExists)),
            "Downloaded the file to {FilePath}");

    public async Task DownloadFileIfNotExists(Uri uri, string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_downloadSettings.DownloadDirectory, fileName);

        if (File.Exists(filePath))
        {
            _fileExistsLog(_logger, fileName, _downloadSettings.DownloadDirectory, null);
            return;
        }

        CreateDirectoryIfNotExists(_downloadSettings.DownloadDirectory);

        using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            _downloadFailedLog(_logger, uri, response.StatusCode, null);
            response.EnsureSuccessStatusCode(); // throw to signal failure
        }

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);

        await responseStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
        await fileStream.FlushAsync(cancellationToken).ConfigureAwait(false);

        _downloadedLog(_logger, filePath, null);
    }

    private void CreateDirectoryIfNotExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}