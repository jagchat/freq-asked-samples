import psfy from 'psfy';
//var psfy = require("psfy"); // or this
import jQuery from 'jq';

var app = app || {};

app.showMsg = function () {
    console.log(jQuery.fn.jquery);
    console.log("something from app1");
    psfy.core.showMsg();
}

export default app;