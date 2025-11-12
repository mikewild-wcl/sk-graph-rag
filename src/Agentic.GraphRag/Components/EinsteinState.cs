using System.Diagnostics.CodeAnalysis;
using SK.GraphRag.Application.EinsteinQuery;

namespace SK.GraphRag.Components;

[SuppressMessage("Performance", "CA1812", Justification = "Created via DI")]
internal sealed class EinsteinState
{
    public string CurrentQuestion { get; set; } = string.Empty;
    public List<(string Question, EinsteinQueryResult Answer)> History { get; } = [];
    public bool IsLoading { get; private set; }
    public bool HasHistory => History.Count > 0;

    public void SetLoading(bool value) => IsLoading = value;

    public void AddExchange(string question, EinsteinQueryResult answer) => History.Add((question, answer));

    public void Clear()
    {
        CurrentQuestion = string.Empty;
        History.Clear();
        IsLoading = false;
    }
}