namespace SK.GraphRag.Application.EinsteinQuery.Interfaces;

public interface IEinsteinQueryService
{
    Task<EinsteinQueryResult> Ask(string question, CancellationToken cancellationToken = default);
}
