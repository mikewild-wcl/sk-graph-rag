using System.Diagnostics;

namespace SK.GraphRag.Application.EinsteinQuery;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record RankedSearchResult(
    string Text,
    double Score,
    int Index)
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay =>
        $$"""
        Score = {{Score:G}}, 
        Index = {{Index:#0}}}, 
        "{{Text}}"
        """;
}
