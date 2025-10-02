using SK.GraphRag.AppHost.Extensions;
using SK.GraphRag.SharedConstants;

var builder = DistributedApplication.CreateBuilder(args);

var graphDBProvider = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.DatabaseProvider}");
var graphDBConnection = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.Connection}");
var createGraphDBInDocker = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.CreateInDocker}");
var graphDBUser = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.User}");
var graphDBPassword = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.Password}", secret: true);

var azureOpenAIEndpoint = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.Endpoint}");
var azureOpenAIApiKey = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.ApiKey}", secret: true);
var azureOpenAIDeploymentName = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.DeploymentName}");
var azureOpenAIEmbeddingDeploymentName = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.EmbeddingDeploymentName}");
var azureOpenAITimeout = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.Timeout}");

var addDockerContainers = 
    bool.TryParse(createGraphDBInDocker.GetValue(), out var createInDocker) 
    && createInDocker;

if (addDockerContainers && graphDBProvider.GetValue() == "neo4j")
{
    builder.AddDockerfile(
        "neo4j", "./", "Dockerfile")
        .WithEndpoint(7474, scheme:"http", targetPort:7474)
        .WithEndpoint(7687, scheme:"bolt", targetPort: 7687)
        .WithEnvironment("NEO4J_AUTH", $"{graphDBUser.GetValue()}/{graphDBPassword.GetValue()}");
}
else if (addDockerContainers && graphDBProvider.GetValue() == "memgraph")
{
    // Compose memgraph 
}

builder.AddProject<Projects.SK_GraphRag>(ProjectNames.GraphRagBlazorApp)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.DatabaseProvider}", graphDBProvider)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Connection}", graphDBConnection)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.User}", graphDBUser)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Password}", graphDBPassword)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.Endpoint}", azureOpenAIEndpoint)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.ApiKey}", azureOpenAIApiKey)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.DeploymentName}", azureOpenAIDeploymentName)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.EmbeddingDeploymentName}", azureOpenAIEmbeddingDeploymentName)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.Timeout}", azureOpenAITimeout);

await builder.Build().RunAsync().ConfigureAwait(false);