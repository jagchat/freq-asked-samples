//var jq = require("jquery"); //another way
import jq from "jquery";
import test from "./test";

// jq(function () {
//     console.log(jq.fn.jquery);
// });

var core = core || {};

core.showMsg = function () {
    console.log(jq.fn.jquery);
    console.log("something");
}

core.test = test;
core.externals = {
    jQuery: jq
}
//window.core = core; //without module export and no library config
//module.exports = core; //direct with library config
export default core; //would need "libraryExport: 'default'" in config