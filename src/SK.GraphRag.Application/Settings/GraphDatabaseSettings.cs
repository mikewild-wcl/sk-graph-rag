namespace SK.GraphRag.Application.Settings;

public record GraphDatabaseSettings(
    Uri Connection,
    string User,
    string Password)
{
    //TODO: Remove parameterless constructor when upgrading to .NET 10.0
    //Added as a workaround for failures when binding configuration to record types
    //https://stackoverflow.com/questions/64933022/can-i-use-c-sharp-9-records-as-ioptions
    public GraphDatabaseSettings() : this(default, default, default) { }

    public const string SectionName = "GraphDatabase";

    public string Provider { get; init; } = "neo4j";

    public string MoviesDb { get; init; } = "neo4j";

    public string EinsteinVectorDb { get; init; } = "neo4j";

    public string UfoDb { get; init; } = "neo4j";

    public int Timeout { get; init; } = 30; // Default timeout in seconds
}