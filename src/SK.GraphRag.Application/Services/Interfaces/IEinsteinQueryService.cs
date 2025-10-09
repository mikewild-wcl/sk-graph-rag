namespace SK.GraphRag.Application.Services.Interfaces;

public interface IEinsteinQueryService
{
    Task<string> Ask(string question, CancellationToken cancellationToken = default);

    Task LoadData();
}
