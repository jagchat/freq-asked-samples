cd .\storage\ops-db
docker image rm --force demo-ops-db
docker compose build --no-cache
docker compose up -d
cd ..\..\
