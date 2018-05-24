import psfy from 'psfy';
//var psfy = require("psfy"); // or this
import jQuery from 'jq';

var app = function (id) {
    var self = this;
    var $root = null;

    this.ajax = psfy.core.ajax;
    this.id = id;

    this.options = function () {
        return psfy.core.app.instances[self.id].options;
    };

    this.root = function () {
        if (!$root) {
            $root = jQuery(self.options().target);
        }
        return $root;
    };

    this.showMsg = function () {
        console.log(jQuery.fn.jquery);
        console.log("something from app 01");
        console.log(self.options);
        //psfy.core.showMsg();
    };

    this.init = function () {
        self.root().find("#btnSum").on("click", self.doSum);
    };

    this.doSum = function () {
        self.ajax.post({
            Url: psfy.core.urls.api('calc/sum'),
            data: { x: self.root().find("#x").val(), y: self.root().find("#y").val() }
        }).done(function (d) {
            self.root().find("#result").text("Result = " + d.result);
        });
    };
}

export default app;
