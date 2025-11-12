namespace SK.GraphRag.Application.EinsteinQuery;

public record EinsteinQueryResult
{
    public required string StandardResponse { get; init; }

    public required string RewrittenQuery { get; init; }

    public required string StepBackResponse { get; init; }

    public required IReadOnlyList<RankedSearchResult> StandardSearchResults { get; init; }

    public required IReadOnlyList<RankedSearchResult> StepBackSearchResults { get; init; }

    public static EinsteinQueryResult Empty => new()
    {
        StandardResponse = string.Empty,
        RewrittenQuery = string.Empty,
        StepBackResponse = string.Empty,
        StandardSearchResults = Array.Empty<RankedSearchResult>(),
        StepBackSearchResults = Array.Empty<RankedSearchResult>()
    };
}