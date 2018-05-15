var app = app || {};
app.portal = app.portal || {};
app.portal.proxyWindow = null;

postRobot.CONFIG.LOG_LEVEL = 'error';

postRobot.on('pushToParent', function (event) {
    console.log('app.host.03 - subscriber');
    console.log(event.data);
});

app.portal.init = function () {
    app.portal.proxyWindow = document.createElement('iframe');
    app.portal.proxyWindow.style = "width:0px;height:0px;border:none;visibility:hidden;display:none";
    app.portal.proxyWindow.src = "http://localhost:48724/proxy03.html";
    document.body.appendChild(app.portal.proxyWindow);
}

app.portal.send = function (o) {
    return postRobot.send(app.portal.proxyWindow, 'pushToFrame', o);
}

$(function () {
    app.portal.init();
});
