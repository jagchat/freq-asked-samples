var app = app || {};
app.portal = app.portal || {};

postRobot.CONFIG.LOG_LEVEL = 'error';

postRobot.on('pushToFrame', function (event) {
    var el = document.getElementById('messages');
    el.innerHTML += '\n' + event.data.message;
    return {
        message: event.data.message
    }
});