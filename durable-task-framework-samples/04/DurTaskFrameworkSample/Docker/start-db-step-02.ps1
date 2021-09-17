. .\start-db--variables.ps1


# create the database with strict binary collation
docker exec -d $containername /opt/mssql-tools/bin/sqlcmd `
    -S . `
    -U sa `
    -P "$pw" `
    -Q "CREATE DATABASE [$dbname] COLLATE $collation"