import jQuery from "jquery";
//import log from "./log";

var ajax = function () {
    //var logger = new log();

    this.get = function (options) {
        var d = jQuery.Deferred();
        jQuery.ajax({
                url: options.url,
                cache: false,
                data: options.data,
                beforeSend: function () {
                    //logger.debug("requesting GET " + options.url + "..");
                    //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay();
                }
            })
            .done(function (data) {
                //logger.debug("done!");
                var result = data;
                if (options.eventId) {
                    result = {
                        eventId: options.eventId,
                        data: result
                    }
                }
                d.resolve(result);
            })
            .fail(function (jQueryhr) {
                var errMsg = "Error occurred. ";
                var moreInfo = "ERROR occurred during GET operation for url: " + options.url + " with data: " + JSON.stringify(options.data);
                if (jQueryhr) {
                    errMsg += " Status: " + jQueryhr.status + " " + jQueryhr.statusText;
                    moreInfo += ", Response: " + jQueryhr.responseText;
                }
                //app.channel.publish(app.events.ERROR, { msg: errMsg, info: moreInfo });
                //logger.debug(moreInfo);
                console.error(moreInfo);
                var result = moreInfo;
                if (options.eventId) {
                    result = {
                        eventId: options.eventId,
                        data: result
                    }
                }
                d.fail(result);
            })
            .always(function () {
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay("remove");
            });
        return d.promise();
    };
    this.post = function (options) {
        var d = jQuery.Deferred();

        jQuery.ajax({
                url: options.url,
                cache: false,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                data: JSON.stringify(options.data),
                method: 'POST',
                beforeSend: function () {
                    //logger.debug("requesting POST " + options.url + "..");
                    //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay();
                }
            })
            .done(function (data) {
                //logger.debug("done!");
                var result = data;
                if (options.eventId) {
                    result = {
                        eventId: options.eventId,
                        data: result
                    }
                }
                d.resolve(result);
            })
            .fail(function (jQueryhr) {
                var errMsg = "Error occurred. ";
                var moreInfo = "ERROR occurred during POST operation for url: " + options.url + " with data: " + JSON.stringify(options.data);
                if (jQueryhr) {
                    errMsg += " Status: " + jQueryhr.status + " " + jQueryhr.statusText;
                    moreInfo += ", Response: " + jQueryhr.responseText;
                }
                //app.channel.publish(app.events.ERROR, { msg: errMsg, info: moreInfo });
                //logger.debug(moreInfo);
                console.error(moreInfo);
                var result = moreInfo;
                if (options.eventId) {
                    result = {
                        eventId: options.eventId,
                        data: result
                    }
                }
                d.fail(result);
            })
            .always(function () {
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay("remove");
            });
        return d.promise();
    };
};

export default ajax;