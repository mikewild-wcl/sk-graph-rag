namespace SK.GraphRag.Application.Services.Interfaces;

public interface IDownloadService
{
    //Task DownloadEinsteinData(CancellationToken cancellationToken = default);

    Task DownloadFileIfNotExists(Uri uri, string fileName, CancellationToken cancellationToken = default);
}