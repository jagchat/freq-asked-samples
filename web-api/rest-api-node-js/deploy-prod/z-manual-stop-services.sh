docker compose -f docker-compose.prod.img.test.yml down
docker rmi ghcr.io/jagchat/demo-rest-api-node-js-prod:latest -f
docker rmi ghcr.io/jagchat/demo-rest-api-ops-db-prod:latest -f
