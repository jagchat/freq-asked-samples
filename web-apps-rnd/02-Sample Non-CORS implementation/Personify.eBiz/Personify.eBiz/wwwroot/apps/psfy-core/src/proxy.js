import jQuery from "jquery";
import core from "./app";

var proxy = {};

//->AJAX implementation (to work with host)
//TODO: enhance/refactor this
(function () {
    var ajaxProxy = function () {
        this.init = function () {
            var opener = window.opener || window.parent;
            psfy.core.postrobot.on('pushToProxyGET', function (event) {
                psfy.proxy.ajax.get(event.data)
                    .done(function (o) {
                        o.success = true;
                        psfy.core.postrobot.send(opener, 'pushToHost', o);
                    })
                    .fail(function (o) {
                        o.success = false;
                        psfy.core.postrobot.send(opener, 'pushToHost', o);
                    });
            });
            psfy.core.postrobot.on('pushToProxyPOST', function (event) {
                psfy.proxy.ajax.post(event.data)
                    .done(function (o) {
                        o.success = true;
                        psfy.core.postrobot.send(opener, 'pushToHost', o);
                    })
                    .fail(function (o) {
                        o.success = false;
                        psfy.core.postrobot.send(opener, 'pushToHost', o);
                    });
            });

            psfy.core.postrobot.send(opener, 'initProxyComplete');
        };
        this.get = function (options) {
            return psfy.core.ajax.get(options);
        };
        this.post = function (options) {
            return psfy.core.ajax.post(options);
        };
    };

    proxy.ajax = new ajaxProxy();
})(proxy);


jQuery(function () {
    proxy.ajax.init();
})

export default proxy;