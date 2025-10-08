# sk-graph-rag
A space for learning GraphRAG with Semantic Kernel neo4j.

## Aspire Parameters

Parameters in the Aspire appsettings.json file `Parameters` section
follow a convention of `SectionName-SettingName` with the hyphen acting as a separator.
The same pattern is used when passing the resources , with : used as the seperator.

## Graph database - Neo4j

How to get started with Neo4j see [Build applications with Neo4j and .NET](https://neo4j.com/docs/dotnet-manual/current/). 
For a suggested approach to dependency injection see [Neo4j Data Access for Your .NET Core C# Microservice](https://neo4j.com/blog/developer/neo4j-data-access-for-your-dot-net-core-c-microservice/).

Neo4j can be run locally using Docker. See https://neo4j.com/docs/operations-manual/current/docker/introduction/

A dockerfile is included in the AppHost project and this will start Neo4j when the application starts.

To start Neo4j in docker from a command line use the following (*backticks used for multi-line commands in Windows Terminal*):
```
docker volume create neo4j_data `
  && docker volume create neo4j_logs `
  && docker volume create neo4j_import `
  && docker volume create neo4j_plugins

docker run -d --name neo4j `
  -p 7474:7474 -p 7687:7687 `
  -v neo4j_data:/data `
  -v neo4j_logs:/logs `
  -v neo4j_import:/import `
  -v neo4j_plugins:/plugins `
  -e NEO4J_AUTH=neo4j/password `
  -e 'NEO4J_PLUGINS=["apoc", "apoc-extended", "graph-data-science"]' `
  -e apoc.import.file.enabled=true `
  -e apoc.import.file.use_neo4j_config=false `
  neo4j:2025.09.0
```

The Neo4j browser can be accessed at http://localhost:7474/browser/

Delete data by running this in the Neo4j data browser:
```
MATCH (n) DETACH DELETE n;
CALL apoc.schema.assert({},{},true) YIELD label, key RETURN *
```

Some of the code in this project was inspired by the book Essential GraphRAG. The code for that book
is at https://github.com/tomasonjo/kg-rag.

Datasets for the earlier book can be found in https://github.com/tomasonjo/graphs-network-science/tree/main.

### Cypher

To set a parameter in Cypher in the neo4j browser:
```
:param name => 'Tom Hanks'
```

You can then use the parameter in a query:
```
MATCH (a:Person {name: $name})-[:ACTED_IN]->(m:Movie)
RETURN m.title AS movieTitle
```

## Graph database - Memgraph

If using Memgraph as a replacement for Neo4j, see:

- https://stackoverflow.com/questions/74528361/how-can-i-connect-to-memgraph-from-my-c-sharp-application
- https://memgraph.com/blog/how-to-build-a-flight-network-analysis-graph-asp-net-application-with-memgraph-c-sharp-and-d3-js


## Docker

See [Add Dockerfiles](https://learn.microsoft.com/en-us/dotnet/aspire/app-host/withdockerfile#add-a-dockerfile-to-the-app-model) 
for details on using Docker files with the AppHost project.

When the project was first run the dockerfile failed to build. A simple file was added to see if it would build and run; this
file has been renamed to `Dockerfile_simple_python` and it loads and runs a simple python script `main.py`.

https://neo4j.com/docs/operations-manual/current/docker/introduction/ 


## Code style and analysis

The ServiceDefaults project was created by the Aspire template and there are a number of code analysis warnings in the project.
Those warnings have been suppressed in the project file.


## TODO

- add a unit test project and start with theory tests for simple chunking
- make the chunking IAsyncEnumerable based
    - [link text](https://stackoverflow.com/questions/21136753/read-a-very-large-file-by-chunks-and-not-line-by-line/21137097#21137097)
- add a service to the AppHost project that uses the chunking
- add a service that takes the Neo4j driver directly and runs fixed queries based on user input
- implement the repository pattern for Neo4j access
- Look at extracting and using UFO dataset
    - See chapter 5 of [sql_book](https://github.com/cathytanimura/sql_book)
    - [SQL book to R](https://iangow.github.io/sql_book/)
    - [Import csv](https://stackoverflow.com/questions/15242757/import-csv-file-into-sql-server)
    
