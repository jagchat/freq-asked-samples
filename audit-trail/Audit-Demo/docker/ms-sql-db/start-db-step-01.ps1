. .\start-db--variables.ps1

# pull the image from the Microsoft container registry
docker pull mcr.microsoft.com/mssql/server:$tag

# run the image, providing some basic setup parameters
docker run --name $containername `
    -e 'ACCEPT_EULA=Y' `
    -e "SA_PASSWORD=$pw" `
    -e "MSSQL_PID=$edition" `
    -p ${port}:1433 `
    -d mcr.microsoft.com/mssql/server:$tag
