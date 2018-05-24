var app = app || {};
app.portal = app.portal || {};

postRobot.CONFIG.LOG_LEVEL = 'error';

postRobot.on('pushToFrame', function (event) {
    app.portal.getSum(event.data).done(function (o) {
        var opener = window.opener || window.parent;
        //postRobot.send(opener, 'pushToParent', o).then(function (event) {
        //    console.log('app.proxy.03 - data sent to subscriber');
        //});
        postRobot.send(opener, 'pushToParent', o);
    });
});

app.portal.getSum = function (o) {
    return $.ajax({
        url: 'http://localhost:48724/api/calc/sum',
        method: 'POST',
        cache: 'false',
        data: { x: o.a, y: o.b }
    });
}