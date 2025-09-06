using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var neo4jUri = builder.AddParameter("Neo4jUri");
var neo4jPassword = builder.AddParameter("Neo4jPassword", secret: true);

builder.AddDockerfile(
    "container", "relative/context/path")
    .WithBuildArg("NEO4J_PASSWORD", neo4jPassword);

builder.AddProject<Projects.SK_GraphRag>("sk-graphrag")
    .WithEnvironment("Neo4jUri", neo4jUri)
    .WithEnvironment("Neo4jPassword", neo4jPassword)
    ;

await builder.Build().RunAsync().ConfigureAwait(false);
