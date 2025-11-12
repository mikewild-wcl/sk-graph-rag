namespace SK.GraphRag.Application.EinsteinQuery.Interfaces;

public interface IEinsteinDataIngestionService
{
    Task LoadData(CancellationToken cancellationToken = default);
}
