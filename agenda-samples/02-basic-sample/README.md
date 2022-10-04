### Basic Demo

- Displays "Hello World!" every 3 seconds
- Ensure MongoDb is running using:

`> ..\mongodb\start.bat`

- use following command to start and run the job

`> node app`

#### Check jobs using GUI

- while not too great, we can check jobs using GUI here:

```
npx agendash `
--db=mongodb://root:admin123@localhost:27017 `
--collection=agendaJobs `
--port=3002
```

- open browser and point to http://localhost:3002 after above command
