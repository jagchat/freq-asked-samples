//var jq = require("jquery"); //another way
import jq from "jquery";
import _ from "underscore";
import ajax from "./ajax";
import vue from "vue";

// jq(function () {
//     console.log(jq.fn.jquery); //displays jquery version
// });

var core = core || {};

core.showMsg = function () {
    console.log(jq.fn.jquery);
    console.log("something");
}

core.externals = {
    jQuery: jq,
    _: _,
    Vue: vue
}

core.ajax = new ajax();

core.urls = {};
core.urls.web = function (ext) {
    var url = SERVICE_URL;
    if (ext) {
        url += '/' + ext;
    }
    return url;
}

core.urls.apps = function (ext) {
    var url = core.urls.web('apps');
    if (ext) {
        url += '/' + ext;
    }
    return url;
}

core.urls.api = function (ext) {
    var url = core.urls.web('api');
    if (ext) {
        url += '/' + ext;
    }
    return url;
}

core.app = {};
core.app.instances = {};

core.app.show = function (options) {

    var injectContent = function () {
        jq(targetElement).html(templateMarkup);
        var src = core.urls.apps(appName + '/dist/psfy.' + appName + '.js');
        var el1 = document.createElement('script');
        jq(targetElement).append(el1);
        el1.onload = function () {
            core.app.instances[options.id] = {
                options: options
            };
            var srcExcScript = '<script> ' +
                ' var ' + options.id + ' = new psfy.' + appName + '("' + options.id + '"); ' +
                options.id + '.init();' +
                '</script>';
            jq(targetElement).append(srcExcScript);
        };
        el1.src = src;
    };

    var appName = options.app;
    var targetElement = options.target;

    var scriptLoader = null;
    var templateMarkup = '';
    if (options.template) {
        var templateAjaxArray = [];
        var scriptLoader = jq.Deferred();
        if (Array.isArray(options.template)) {
            templateAjaxArray = _.map(options.template, function (path) {
                return core.ajax.get({
                    Url: core.urls.apps(path)
                })
            });
        } else {
            templateAjaxArray.push(core.ajax.get({
                Url: core.urls.apps(options.template)
            }));
        }
        jq.when.apply(jq, templateAjaxArray).then(function () {
            var resultObjects = arguments;
            _.each(resultObjects, function (ro) {
                templateMarkup += ro;
            });
            scriptLoader.resolve();
        });
    }

    if (scriptLoader) {
        scriptLoader.done(function () {
            injectContent();
        });
    } else {
        injectContent();
    }
}

//window.core = core; //without module export and no library config
//module.exports = core; //direct with library config
export default core; //would need "libraryExport: 'default'" in config