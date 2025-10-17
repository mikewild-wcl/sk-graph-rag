namespace SK.GraphRag.Application.Settings;

public record EinsteinQuerySettings()
{
    public const string SectionName = "EinsteinQuery";

    public required Uri DocumentUri { get; init; }

    public required string DocumentFileName { get; init; }
}