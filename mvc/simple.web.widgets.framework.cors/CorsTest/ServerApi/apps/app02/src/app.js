import psfy from 'psfy';
import Vue from 'Vue';
import jQuery from 'jq';

var app = function (id) {
    var self = this;
    var $root = null;

    this.ajax = psfy.core.ajax;
    this.id = id;
    this.v = null;

    this.options = function () {
        return psfy.core.app.instances[self.id].options;
    };

    this.root = function () {
        if (!$root) {
            $root = jQuery(self.options().target);
        }
        return $root;
    };

    this.init = function () {
        self.v = new Vue({
            el: self.options().target + ' .app02',
            data: {
                x: 3,
                y: 5,
                resultMsg: ''
            },
            methods: {
                doSum: function () {
                    var vself = this;
                    self.ajax.post({
                        Url: psfy.core.urls.api('calc/sum'),
                        data: {
                            x: this.x,
                            y: this.y
                        }
                    }).done(function (d) {
                        vself.resultMsg = "Result = " + d.result;
                    });
                }
            }
        });
    };
}

export default app;