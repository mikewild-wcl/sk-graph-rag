# sk-graph-rag
A space for learning GraphRAG with Semantic Kernel neo4j.


## Graph database

How to get started with Neo4j see [Build applications with Neo4j and .NET](https://neo4j.com/docs/dotnet-manual/current/). 
For a suggested approach to dependency injection see [Neo4j Data Access for Your .NET Core C# Microservice](https://neo4j.com/blog/developer/neo4j-data-access-for-your-dot-net-core-c-microservice/).

Neo4j can be run locally using Docker. See https://neo4j.com/docs/operations-manual/current/docker/introduction/

A dockerfile is included in the AppHost project and this will start Neo4j when the application starts.

Some of the code in this project was inspired by the book Essential GraphRAG. The code for that book
is at 


## Docker

See [Add Dockerfiles](https://learn.microsoft.com/en-us/dotnet/aspire/app-host/withdockerfile#add-a-dockerfile-to-the-app-model) 
for details on using Docker files with the AppHost project.

When the project was first run the dockerfile failed to build. A simple file was added to see if it would build and run; this
file has been renamed to `Dockerfile_simple_python` and it loads and runs a simple python script `main.py`.

https://neo4j.com/docs/operations-manual/current/docker/introduction/ 


## Code style and analysis

The ServiceDefaults project was created by the Aspire template and there are a number of code analysis warnings in the project.
Those warnings have been suppressed in the project file.

