////NOTE: use following command to execute the script
//> node .\app.js
//OR
//> $env:PORT="9000" ; node .\app.js

const express = require('express');
const bodyParser = require('body-parser');
const app = express();
//app.use(bodyParser.urlencoded({ extended: true }));
app.use(
    express.urlencoded({
        extended: true,
    })
);
app.use(express.text());
app.get('/asis', (req, res) => {
    console.log(req.url);
    res.send('in GET /asis...');
});

app.post('/asis', (req, res) => {
    console.log('Got body:', req.body);
    console.log('headers:', JSON.stringify(req.headers))
    res.send('in POST /asis...');
});

app.get('/', (req, res) => {
    res.send('use "HTTP GET /asis" or "HTTP POST /asis" to proceed...');
});

const port = process.env.PORT || 8000;
app.listen(port, () => {
    console.log(`Server started on port ${port}`);
});