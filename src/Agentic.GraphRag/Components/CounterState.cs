using System.Diagnostics.CodeAnalysis;

namespace Agentic.GraphRag.Components;

[SuppressMessage("Performance", "CA1812", Justification = "Activated by dependency injection container")]
internal sealed class CounterState
{
    public int CurrentCount { get; private set; }

    public void Increment() => CurrentCount++;
}
