namespace SK.GraphRag.Application.Settings;

public record AzureOpenAISettings(
    string ApiKey,
    string Endpoint,
    string DeploymentName,
    string EmbeddingDeploymentName)
{
    public const string SectionName = "AzureOpenAI";

    public string? ModelId { get; init; }

    public string? EmbeddingModelId { get; init; }

    public int Timeout { get; init; } = 30; // Default timeout in seconds
}