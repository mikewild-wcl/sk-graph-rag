### # syntax=docker/dockerfile:1
FROM neo4j:2025.08.0
#FROM neo4j:latest

# https://stackoverflow.com/questions/59143685/consume-secret-inside-dockerfile
# ENV DOCKER_BUILDKIT=1

# Build-time user and password (defaults 'neo4j' and 'test') can be overridden with --build-arg or at runtime via 
#	-e NEO4J_USER=...
#	-e NEO4J_PASSWORD=...
#   --build-arg or at runtime via -e NEO4J_PASSWORD=...
#ARG NEO4J_USER=neo4j
#ARG NEO4J_PASSWORD=test
#ARG NEO4J_AUTH
## ARG USER=neo4j
## ARG PASSWORD=test
#ENV NEO4J_USER=${NEO4J_USER}
# ENV NEO4J_PASSWORD=${NEO4J_PASSWORD}

# ENV NEO4J_AUTH=${NEO4J_USER}/${NEO4J_PASSWORD}
#ENV NEO4J_dbms_security_auth__minimum__password__length=8
ENV NEO4J_ACCEPT_LICENSE_AGREEMENT=yes

#RUN echo "Oh dang look at that $NEO4J_USER"
#RUN echo "Oh dang look at that $NEO4J_PASSWORD"

RUN echo "NEO4J_AUTH=$NEO4J_AUTH"
RUN echo "NEO4J_AUTH=${NEO4J_AUTH}"

# Secret https://www.dudley.codes/posts/2020.12.28-dockerfile-build-args-exposed/
#RUN --mount=type=secret,id=bitbucket,dst=/root/.netrc; && \
#    cd /source &&\
#    go build -o myapp
# https://www.dudley.codes/posts/2020.12.28-dockerfile-build-args-exposed/
#RUN --mount=type=secret,id=mysecret cat /run/secrets/mysecret

#RUN --mount=type=secret,id=mysecret ENV my_secret=/run/secrets/mysecret

# ENV MYSECRET=/run/secrets/my_secret
# https://github.com/docker/build-push-action/issues/1210
# ENV OP_SERVICE_ACCOUNT_TOKEN=$(cat /run/secrets/my_secret)
  
#RUN --mount=type=secret,id=my_secret,env=SECRET_TOKEN cat /run/secrets/my_secret

#RUN echo "secret: $OP_SERVICE_ACCOUNT_TOKEN"
#RUN echo "secret: $my_secret"
# N echo "secret token: $SECRET_TOKEN"


# Listen on all ports
ENV NEO4J_server_default__listen__address=0.0.0.0
# ENV NEO4J_dbms.connectors.default_listen_address=0.0.0.0
# ENV NEO4J_dbms_connectors_default__listen__address=0.0.0.0

# Ask Neo4j to install APOC and Graph Data Science on startup
ENV NEO4J_PLUGINS='["apoc","graph-data-science"]'

# Allow APOC and GDS procedures to run without procedure-level blocking and set import dir
ENV NEO4J_dbms_security_procedures_unrestricted=apoc.*,gds.*
ENV NEO4J_dbms_directories_import=/import

# Expose HTTP and Bolt
EXPOSE 7474 7687

# Persist data/logs/plugins/import across container restarts
VOLUME ["/data", "/logs", "/import", "/plugins"]

# Healthcheck uses the configured password
#HEALTHCHECK --interval=10s --timeout=5s --retries=10 \
#  CMD-SHELL cypher-shell -u "$NEO4J_USER" -p "$NEO4J_PASSWORD" "RETURN 1" >/dev/null 2>&1 || exit 1

# RUN useradd -m -s /bin/bash $USER && echo "$USER:$PASS" | chpasswd
#RUN useradd $USER && echo "$USER:$PASSWORD" | chpasswd
