import jQuery from "jquery";
import core from "./app";
import _ from "lodash";

var host = {};

//-> "ready" implementation
//TODO: enhance/refactor this
(function () {
    var readyQueue = [];
    var isReady = false;
    host.isReady = function () {
        return isReady;
    }
    host.ready = function (cb) {
        if (!isReady) {
            readyQueue.push(cb);
        } else {
            cb();
        }
    }

    host.setReady = function () {
        isReady = true;
        readyQueue.forEach(function (cb) {
            cb();
        });
    }
})(host);

//-> AJAX (non-cors) implementation (to work with proxy)
//TODO: enhance/refactor this
(function () {
    var ajaxHost = function () {
        var ajaxEventQueue = [];
        this.init = function () {
            psfy.core.postrobot.on('initProxyComplete', function (event) {
                psfy.host.setReady();
            });
            psfy.core.postrobot.on('pushToHost', function (event) {
                if (event.data.eventId) {
                    var currentEvent = _.find(ajaxEventQueue, function (item) {
                        return item.eventId == event.data.eventId;
                    });
                    if (event.data.success == true) {
                        currentEvent.deffered.resolve(event.data.data);
                    } else {
                        currentEvent.deffered.fail(event.data.data);
                    }
                    _.remove(ajaxEventQueue, function (item) {
                        return item.eventId == currentEvent.eventId;
                    });
                }
            });
        };
        this.get = function (options) {
            var d = jQuery.Deferred();
            var eventId = psfy.core.getUniqueId();
            options.eventId = eventId;
            ajaxEventQueue.push({
                eventId: options.eventId,
                deffered: d
            });
            psfy.core.postrobot.send(psfy.core.proxyWindow, 'pushToProxyGET', options);
            return d.promise();
        };
        this.post = function (options) {
            var d = jQuery.Deferred();
            var eventId = psfy.core.getUniqueId();
            options.eventId = eventId;
            ajaxEventQueue.push({
                eventId: options.eventId,
                deffered: d
            });
            psfy.core.postrobot.send(psfy.core.proxyWindow, 'pushToProxyPOST', options);
            return d.promise();
        };
    };

    host.ajax = new ajaxHost();
})(host);


jQuery(function () {
    core.createNonCorsProxy();
    host.ajax.init();
})

export default host;