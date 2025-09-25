# How to start

NOTE:

- tested on windows with node.js v21
- Docker is required to run following

### Steps

- create `.env` from `.env.dev` (or just rename!)
- ensure postgres db is up and running

```
./local-ops-db-start.ps1
./local-ops-db-refresh.ps1
```

- run following command

```
npm start
```

- ensure http://localhost:3000/api/dept is working
