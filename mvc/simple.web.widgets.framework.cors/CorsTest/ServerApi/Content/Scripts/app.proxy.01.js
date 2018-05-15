var appproxy = appproxy || {};

appproxy.getSum = function (o) {
    return $.ajax({
        url: 'http://localhost:48724/api/calc/sum',
        method: 'POST',
        cache: 'false',
        data: { x: o.a, y: o.b }
    });
}

//parent.document.app = app;

function receiveMessage(event) {
    //console.log(event.data);
    appproxy.getSum(JSON.parse(event.data)).done(function (data) {
        event.source.postMessage(data, "*");
    });
}

window.addEventListener("message", receiveMessage, false);

//$(function () {
//    console.log(1);
//    appproxy.getSum({ a: 10, b: 20 }).done(function (data) { console.log(data); });    
//});
