using SK.GraphRag.AppHost.Extensions;
using SK.GraphRag.SharedConstants;

var builder = DistributedApplication.CreateBuilder(args);

var graphDBProvider = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.DatabaseProvider}");
var neo4jConnection = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.Connection}");
var neo4jUser = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.User}");
var neo4jPassword = builder.AddParameter($"{ResourceNames.GraphDatabaseSection}-{ResourceNames.Password}", secret: true);

var azureOpenAIEndpoint = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.Endpoint}");
var azureOpenAIApiKey = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.ApiKey}", secret: true);
var azureOpenAIDeploymentName = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.DeploymentName}");
var azureOpenAIEmbeddingDeploymentName = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.EmbeddingDeploymentName}");
var azureOpenAITimeout = builder.AddParameter($"{ResourceNames.AzureOpenAISection}-{ResourceNames.Timeout}");

if(graphDBProvider.GetValue() == "neo4j")
{
    builder.AddDockerfile(
        "neo4j", "./", "Dockerfile")
        .WithEndpoint(7474, scheme:"http", targetPort:7474)
        .WithEndpoint(7687, scheme:"bolt", targetPort: 7687)
        .WithEnvironment("NEO4J_AUTH", $"{neo4jUser.GetValue()}/{neo4jPassword.GetValue()}");
}
else if (graphDBProvider.GetValue() == "memgraph")
{
    // Compose memgraph 
}

builder.AddProject<Projects.SK_GraphRag>(ProjectNames.GraphRagBlazorApp)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.DatabaseProvider}", graphDBProvider)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Connection}", neo4jConnection)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.User}", neo4jUser)
    .WithEnvironment($"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Password}", neo4jPassword)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.Endpoint}", azureOpenAIEndpoint)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.ApiKey}", azureOpenAIApiKey)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.DeploymentName}", azureOpenAIDeploymentName)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.EmbeddingDeploymentName}", azureOpenAIEmbeddingDeploymentName)
    .WithEnvironment($"{ResourceNames.AzureOpenAISection}:{ResourceNames.Timeout}", azureOpenAITimeout);

await builder.Build().RunAsync().ConfigureAwait(false);