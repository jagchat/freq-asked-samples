. .\start-db--variables.ps1

# stop the container
docker stop $containername

# remove the container
docker rm $containername

# remove image
docker image rm mcr.microsoft.com/mssql/server:$tag

