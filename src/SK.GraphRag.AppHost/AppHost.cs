using k8s.KubeConfigModels;
using MessagePack.Resolvers;
using SK.GraphRag.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var neo4jConnection = builder.AddParameter("Neo4jConnection");
var neo4jUser = builder.AddParameter("Neo4jUser");
var neo4jPassword = builder.AddParameter("Neo4jPassword", secret: true);

var azureOpenAIEndpoint = builder.AddParameter("AzureOpenAIEndpoint");
var azureOpenAIApiKey = builder.AddParameter("AzureOpenAIApiKey", secret: true);
var azureOpenAIDeploymentName = builder.AddParameter("AzureOpenAIDeploymentName");
var azureOpenAIEmbeddingDeploymentName = builder.AddParameter("AzureOpenAIEmbeddingDeploymentName");
var azureOpenAITimeout = builder.AddParameter("AzureOpenAITimeout");

builder.AddDockerfile(
    "neo4j", "./", "Dockerfile")
    .WithEndpoint(7474, scheme:"http", targetPort:7474)
    .WithEndpoint(7687, scheme:"bolt", targetPort: 7687)
    .WithEnvironment("NEO4J_AUTH", $"{neo4jUser.GetValue()}/{neo4jPassword.GetValue()}");

builder.AddProject<Projects.SK_GraphRag>("sk-graphrag")
    .WithEnvironment("Neo4j:Connection", neo4jConnection)
    .WithEnvironment("Neo4j:User", neo4jUser)
    .WithEnvironment("Neo4j:Password", neo4jPassword)
    .WithEnvironment("AzureOpenAI:Endpoint", azureOpenAIEndpoint)
    .WithEnvironment("AzureOpenAI:ApiKey", azureOpenAIApiKey)
    .WithEnvironment("AzureOpenAI:DeploymentName", azureOpenAIDeploymentName)
    .WithEnvironment("AzureOpenAI:EmbeddingDeploymentName", azureOpenAIEmbeddingDeploymentName)
    .WithEnvironment("AzureOpenAI:Timeout", azureOpenAITimeout);

await builder.Build().RunAsync().ConfigureAwait(false);
