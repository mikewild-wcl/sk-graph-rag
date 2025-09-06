# Dockerfile to run Neo4j with APOC and Graph Data Science plugins for local/dev use
FROM neo4j:5.11

# Build-time password (default 'test') can be overridden with --build-arg or at runtime via -e NEO4J_PASSWORD=...
ARG NEO4J_PASSWORD=test
ENV NEO4J_PASSWORD=${NEO4J_PASSWORD}
ENV NEO4J_AUTH=neo4j/${NEO4J_PASSWORD}
ENV NEO4J_ACCEPT_LICENSE_AGREEMENT=yes

# Ask Neo4j to install APOC and Graph Data Science on startup
ENV NEO4JLABS_PLUGINS='["apoc","graph-data-science"]'

# Allow APOC and GDS procedures to run without procedure-level blocking and set import dir
ENV NEO4J_dbms_security_procedures_unrestricted=apoc.*,gds.*
ENV NEO4J_dbms_directories_import=/import

# Expose HTTP and Bolt
EXPOSE 7474 7687

# Persist data/logs/plugins/import across container restarts
VOLUME ["/data", "/logs", "/import", "/plugins"]

# Healthcheck uses the configured password
HEALTHCHECK --interval=10s --timeout=5s --retries=10 \
  CMD-SHELL cypher-shell -u neo4j -p "$NEO4J_PASSWORD" "RETURN 1" >/dev/null 2>&1 || exit 1