////NOTE: use following command to execute the script
//> node .\02-using-task.js
//OR
//> $env:PORT="9000" ; node .\02-web-server.js

const express = require('express');
const cron = require('node-cron');

const app = express();

let task = cron.schedule('*/2 * * * * *', () => {
    console.log('running a task every two seconds');
}, {
    scheduled: false
})

app.get('/start', (req, res) => {
    task.start();
    res.send('task started!');
});

app.get('/stop', (req, res) => {
    task.stop();
    res.send('task stopped!');
});

app.get('/', (req, res) => {
    res.send('use "HTTP GET /start" or "HTTP GET /stop" to proceed...');
});

const port = process.env.PORT || 8000;
app.listen(port, () => {
    console.log(`Server started on port ${port}`);
    console.log(`use "http://localhost:${port}/start" or "http://localhost:${port}/stop" to proceed...`);
});