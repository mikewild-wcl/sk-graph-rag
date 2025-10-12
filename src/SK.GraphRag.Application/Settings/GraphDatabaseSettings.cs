namespace SK.GraphRag.Application.Settings;

public record GraphDatabaseSettings(
    Uri Connection,
    string User,
    string Password)
{
    public const string SectionName = "GraphDatabase";

    public string Provider { get; init; } = "neo4j";

    public string MoviesDb { get; init; } = "neo4j";

    public string EinsteinVectorDb { get; init; } = "neo4j";

    public string UfoDb { get; init; } = "neo4j";

    public int Timeout { get; init; } = 30; // Default timeout in seconds
}