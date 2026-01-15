#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM postgres:14
WORKDIR /src
COPY /scripts /scripts
COPY /db-init-scripts/init-all.sh /docker-entrypoint-initdb.d/01-init-all.sh
RUN chmod +x /docker-entrypoint-initdb.d/01-init-all.sh
