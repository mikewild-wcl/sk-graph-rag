namespace SK.GraphRag.Application.Services.Interfaces;

public interface IEinsteinQueryService
{
    Task<string> AskAsync(string question, CancellationToken cancellationToken = default);
}
