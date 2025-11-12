namespace SK.GraphRag.Application.Services.Interfaces;

public interface IDownloadService
{
    Task DownloadFileIfNotExists(Uri uri, string fileName, CancellationToken cancellationToken = default);

    bool TryGetDownloadedFilePath(string fileName, out string? filePath);
}