# How to start

This is to build the docker images locally and test them locally. Nothing will be pushed to docker registry

### Steps

NOTE: Following steps are to use with Windows/Powershell

```
.\local-img-rebuild.ps1
.\local-img-test-start.ps1
```

This is how you can test:

```
curl http://localhost:3000/api/dept
curl http://localhost:3000/api/emp
curl http://localhost:3000/api/address
```

```
.\local-img-test-stop.ps1
```
