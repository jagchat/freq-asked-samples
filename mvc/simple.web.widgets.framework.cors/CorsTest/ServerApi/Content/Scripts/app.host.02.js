var app = app || {};
app.portal = app.portal || {};
app.portal.proxyWindow = null;

postRobot.CONFIG.LOG_LEVEL = 'error';

postRobot.on('pushToParent', function (event) {
    var el = document.getElementById('messages');
    el.innerHTML += '\n' + event.data.message;
    return {
        message: event.data.message
    }
});

app.portal.init = function () {
    app.portal.proxyWindow = document.createElement('iframe');
    //iframe.style = "width:0px;height:0px;border:none;visibility:hidden;display:none";
    app.portal.proxyWindow.src = "http://localhost:48724/proxy02.html";
    document.body.appendChild(app.portal.proxyWindow);
}

app.portal.send = function(o) {
    return postRobot.send(app.portal.proxyWindow, 'pushToFrame', o);
}

$(function () {
    app.portal.init();
});
