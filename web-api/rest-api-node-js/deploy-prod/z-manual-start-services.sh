#!/bin/bash

# copy this file to /home/<your-username>/docker-demo-rest-crud-api
# ensure docker-compose.prod.img.test.yml is also in that folder
docker login ghcr.io -u jagchat -p <your-personal-access-token>
docker compose -f docker-compose.prod.img.test.yml up -d
