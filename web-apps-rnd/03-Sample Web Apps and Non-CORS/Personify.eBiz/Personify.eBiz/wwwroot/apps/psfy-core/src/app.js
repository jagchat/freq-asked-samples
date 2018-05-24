import postrobot from "post-robot";
import jq from "jquery";
import ajaxCors from "./ajax-cors";
import pubsub from './pubsub';
import vue from 'vue';

var core = {};

core.ajax = new ajaxCors();

core.pubsub = pubsub;

core.externals = {
    jQuery: jq,
    Vue: vue
}

core.info = function () {
    console.log('Service Url: ' + SERVICE_URL);
    console.log('PostRobot Log Config Level: ' + POSTROBOT_LOG_LEVEL);
    console.log('jQuery version: ' + jq.fn.jquery);
    console.log(pubsub);
    //console.log(postrobot);
}

postrobot.CONFIG.LOG_LEVEL = POSTROBOT_LOG_LEVEL;
core.postrobot = postrobot;

core.getUniqueId = function () {
    var d = new Date().getTime();
    if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
        d += performance.now(); //use high-precision timer if available
    }
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

core.createNonCorsProxy = function () {
    core.proxyWindow = document.createElement('iframe');
    core.proxyWindow.id = "psfy-host-iframe";
    core.proxyWindow.style = "width:0px;height:0px;border:none;visibility:hidden;display:none";
    core.proxyWindow.src = SERVICE_URL + "apps/psfy-core/dist/proxy.htm";
    document.body.appendChild(core.proxyWindow);
}

export default core;