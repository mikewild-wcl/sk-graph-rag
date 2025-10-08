# Dockerfile to run Neo4j with APOC and Graph Data Science plugins for local/dev use
FROM neo4j:2025.09.0

# Build-time user and password (defaults 'neo4j' and 'test') can be overridden with --build-arg or at runtime via 
ENV NEO4J_ACCEPT_LICENSE_AGREEMENT=yes

# Expose HTTP and Bolt
EXPOSE 7474 7687

# Persist data/logs/plugins/import across container restarts
VOLUME ["/data", "/logs", "/import", "/plugins"]
