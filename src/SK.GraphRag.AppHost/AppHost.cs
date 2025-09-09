var builder = DistributedApplication.CreateBuilder(args);

var neo4jConnection = builder.AddParameter("Neo4jConnection");
var neo4jUser = builder.AddParameter("Neo4jUser");
var neo4jPassword = builder.AddParameter("Neo4jPassword", secret: true);

var azureOpenAIEndpoint = builder.AddParameter("AzureOpenAIEndpoint");
var azureOpenAIApiKey = builder.AddParameter("AzureOpenAIApiKey", secret: true);
var azureOpenAIDeploymentName = builder.AddParameter("AzureOpenAIDeploymentName");
var azureOpenAIEmbeddingDeploymentName = builder.AddParameter("AzureOpenAIEmbeddingDeploymentName");
var azureOpenAITimeout = builder.AddParameter("AzureOpenAITimeout");

var s = neo4jUser.Resource.ToString();
var userName = await neo4jUser.Resource.GetValueAsync(default).ConfigureAwait(false);
var password = await neo4jPassword.Resource.GetValueAsync(default).ConfigureAwait(false);
//var auth = $"NEO4J_AUTH={userName}/{password}";
var auth = $"{userName}/{password}";
//var auth = $"NEO4J_AUTH={neo4jUser.Resource.Value}/{neo4jPassword.Resource.Value}";
//var auth2 = $"NEO4J_AUTH={neo4jUser.GetValueAsync(default).AsTask().GetAwaiter().GetResult()!}/{neo4jPassword}";

builder.AddDockerfile(
    "neo4j", "./", "Dockerfile")
    .WithEnvironment("NEO4J_AUTH", auth)
    //.WithEnvironment("DOCKER_BUILDKIT", "1")
    .WithEndpoint(7474, scheme:"http", targetPort:7474)
    .WithEndpoint(7687, scheme:"bolt", targetPort: 7687)
    //.WithBuildArg("USER", neo4jUser)
    //.WithBuildSecret("PASSWORD", neo4jPassword)
    //.WithBuildArg("NEO4J_USER", neo4jUser)
    //.WithBuildArg("NEO4J_AUTH", auth)
    //.WithBuildSecret("NEO4J_PASSWORD", neo4jPassword)
    //.WithBuildSecret("my_secret", neo4jPassword)
    //.WithBuildArg("NEO4J_PASSWORD", neo4jPassword)
    //.WithContainerRuntimeArgs("--env", $"NEO4J_AUTH={auth}")
    ;

builder.AddProject<Projects.SK_GraphRag>("sk-graphrag")
    .WithEnvironment("Neo4j:Connection", neo4jConnection)
    .WithEnvironment("Neo4j:User", neo4jUser)
    .WithEnvironment("Neo4j:Password", neo4jPassword)
    .WithEnvironment("AzureOpenAI:Endpoint", azureOpenAIEndpoint)
    .WithEnvironment("AzureOpenAI:ApiKey", azureOpenAIApiKey)
    .WithEnvironment("AzureOpenAI:DeploymentName", azureOpenAIDeploymentName)
    .WithEnvironment("AzureOpenAI:EmbeddingDeploymentName", azureOpenAIEmbeddingDeploymentName)
    .WithEnvironment("AzureOpenAI:Timeout", azureOpenAITimeout)
    ;

await builder.Build().RunAsync().ConfigureAwait(false);
