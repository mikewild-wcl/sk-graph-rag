namespace SK.GraphRag.Application.Movies.Interfaces;

public interface IMoviesGraphQueryService
{
    Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default);
}
