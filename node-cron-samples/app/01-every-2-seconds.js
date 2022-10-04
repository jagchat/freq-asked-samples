////NOTE: use following command to execute the script
//> node .\01-every-2-seconds.js

var cron = require('node-cron');

cron.schedule('*/2 * * * * *', () => {
    console.log('running a task every two seconds');
});