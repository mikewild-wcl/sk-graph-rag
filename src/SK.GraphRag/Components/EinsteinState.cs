using System.Diagnostics.CodeAnalysis;

namespace SK.GraphRag.Components;

[SuppressMessage("Performance", "CA1812", Justification = "Created via DI")]
internal sealed class EinsteinState
{
    public string CurrentQuestion { get; set; } = string.Empty;
    public List<(string Question, string Answer)> History { get; } = [];
    public bool IsLoading { get; private set; }
    public bool HasHistory => History.Count > 0;

    public void SetLoading(bool value) => IsLoading = value;

    public void AddExchange(string question, string answer) => History.Add((question, answer));

    public void Clear()
    {
        CurrentQuestion = string.Empty;
        History.Clear();
        IsLoading = false;
    }
}