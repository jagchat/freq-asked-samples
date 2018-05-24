import jq from "jquery";
import log from "./log";

var ajax = function () {
    var logger = new log();

    this.get = function (options) {
        var d = jq.Deferred();
        jq.ajax({
            url: options.Url,
            cache: false,
            data: options.data,
            beforeSend: function () {
                logger.debug("requesting GET " + options.Url + "..");
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay();
            }
        })
            .done(function (data) {
                logger.debug("done!");
                d.resolve(data);
            })
            .fail(function (jqhr) {
                debugger;
                var errMsg = "Error occurred. ";
                var moreInfo = "ERROR occurred during GET operation for Url: " + options.Url + " with data: " + JSON.stringify(options.data);
                if (jqhr) {
                    errMsg += " Status: " + jqhr.status + " " + jqhr.statusText;
                    moreInfo += ", Response: " + jqhr.responseText;
                }
                //app.channel.publish(app.events.ERROR, { msg: errMsg, info: moreInfo });
                logger.debug(moreInfo);

                d.fail();
            })
            .always(function () {
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay("remove");
            });
        return d.promise();
    };
    this.post = function (options) {
        var d = jq.Deferred();

        jq.ajax({
            url: options.Url,
            cache: false,
            data: options.data,
            method: 'POST',
            beforeSend: function () {
                logger.debug("requesting POST " + options.Url + "..");
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay();
            }
        })
            .done(function (data) {
                logger.debug("done!");
                d.resolve(data);
            })
            .fail(function (jqhr) {
                debugger;
                var errMsg = "Error occurred. ";
                var moreInfo = "ERROR occurred during POST operation for Url: " + options.Url + " with data: " + JSON.stringify(options.data);
                if (jqhr) {
                    errMsg += " Status: " + jqhr.status + " " + jqhr.statusText;
                    moreInfo += ", Response: " + jqhr.responseText;
                }
                //app.channel.publish(app.events.ERROR, { msg: errMsg, info: moreInfo });
                logger.debug(moreInfo);

                d.fail();
            })
            .always(function () {
                //if (options.OverlayTarget) options.OverlayTarget.loadingOverlay("remove");
            });
        return d.promise();
    };
};

export default ajax;