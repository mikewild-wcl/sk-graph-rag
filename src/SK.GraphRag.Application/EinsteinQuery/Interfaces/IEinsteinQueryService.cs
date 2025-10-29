namespace SK.GraphRag.Application.EinsteinQuery.Interfaces;

public interface IEinsteinQueryService
{
    Task<string> Ask(string question, CancellationToken cancellationToken = default);
}
