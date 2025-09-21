namespace SK.GraphRag.Application.Services.Interfaces;

public interface IMoviesGraphQueryService
{
    Task<List<string>> GetMoviesForActor(string actorName, CancellationToken cancellationToken = default);
}
