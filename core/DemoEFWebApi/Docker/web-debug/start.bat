dotnet build ../../DemoEFWebApi.sln
dotnet publish ../../app.webapi/app.webapi.csproj -c Debug
docker-compose up
pause
