# sk-graph-rag
A space for learning GraphRAG with Semantic Kernel neo4j


## Graph database

How to get started with Neo4j: https://neo4j.com/docs/dotnet-manual/current/

Neo4j can be run locally using Docker. See https://neo4j.com/docs/operations-manual/current/docker/introduction/

A dockerfile is included in the AppHost project and this will start Neo4j when the application starts.

See [Add Dockerfiles](https://learn.microsoft.com/en-us/dotnet/aspire/app-host/withdockerfile#add-a-dockerfile-to-the-app-model) 
for details on using Docker files with the AppHost project.


## Code style and analysis

The ServiceDefaults project was created by the Aspire template and there are a number of code analysis warnings in the project.
Those warnings have been suppressed in the project file.

