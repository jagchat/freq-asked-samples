import jq from "jquery";

var test = {};
test.showMsg = function () {
    console.log(jq.fn.jquery);
    console.log("something from test");
};

export default test;