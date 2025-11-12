namespace Agentic.GraphRag.Application.Movies.Interfaces;

public interface IMoviesQueryService
{
    Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default);
}
