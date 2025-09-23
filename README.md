# sk-graph-rag
A space for learning GraphRAG with Semantic Kernel neo4j.


## Graph database - Neo4j

How to get started with Neo4j see [Build applications with Neo4j and .NET](https://neo4j.com/docs/dotnet-manual/current/). 
For a suggested approach to dependency injection see [Neo4j Data Access for Your .NET Core C# Microservice](https://neo4j.com/blog/developer/neo4j-data-access-for-your-dot-net-core-c-microservice/).

Neo4j can be run locally using Docker. See https://neo4j.com/docs/operations-manual/current/docker/introduction/

A dockerfile is included in the AppHost project and this will start Neo4j when the application starts.

To start Neo4j in docker from a command line use the following (replace the password!):
```
docker run -p 7474:7474 -p 7687:7687 -d -v $HOME/neo4j/data:/data -e NEO4J_AUTH=neo4j/password -e 'NEO4J_PLUGINS=["apoc", "graph-data-science"]' neo4j:2025.08.0 
```

Some of the code in this project was inspired by the book Essential GraphRAG. The code for that book
is at https://github.com/tomasonjo/kg-rag.

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
    
