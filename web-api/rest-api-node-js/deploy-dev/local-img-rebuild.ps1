docker rmi demo-rest-api-node-js:test demo-rest-api-ops-db:test -f
cd ..\src
.\local-img-build.ps1
cd ..\storage\ops-db
.\local-img-build.ps1
cd ..\..\deploy-dev

