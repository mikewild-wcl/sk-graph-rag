using System.Diagnostics.CodeAnalysis;

namespace Agentic.GraphRag.Components;

[SuppressMessage("Performance", "CA1812", Justification = "Activated by DI container")]
internal sealed class MoviesState
{
    public string LastActorName { get; private set; } = string.Empty;
    public List<string> Movies { get; } = [];
    public bool HasSearched { get; private set; }

    public void SetResults(string actorName, IEnumerable<string> movies)
    {
        LastActorName = actorName;
        Movies.Clear();
        Movies.AddRange(movies);
        HasSearched = true;
    }

    public void Clear()
    {
        LastActorName = string.Empty;
        Movies.Clear();
        HasSearched = false;
    }
}
