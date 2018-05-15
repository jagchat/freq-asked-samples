var logger = function (type) {

    var getDateFormat = function () {
        var date = new Date();
        return date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds() + '.' + date.getMilliseconds();
    };

    this.trace = function () {
        Array.prototype.unshift.call(arguments, getDateFormat());
        console.log.apply(console, arguments);
    };

    this.debug = function () {
        Array.prototype.unshift.call(arguments, getDateFormat());
        console.log.apply(console, arguments);
    };

    this.warn = function () {
        Array.prototype.unshift.call(arguments, getDateFormat());
        console.warn.apply(console, arguments);
    };

    this.error = function () {
        Array.prototype.unshift.call(arguments, getDateFormat());
        console.error.apply(console, arguments);
    };

};

export default logger;