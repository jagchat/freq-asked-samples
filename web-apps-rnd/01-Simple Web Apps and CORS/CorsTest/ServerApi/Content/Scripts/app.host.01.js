var app = app || {};

function receiveMessage(event) {
    console.log(event.data);
}

window.addEventListener("message", receiveMessage, false);

var iframe = null;
function getServerContent() {
    //iframe = document.createElement('iframe');
    //iframe.style = "width:0px;height:0px;border:none;visibility:hidden;display:none";
    //var s = "";
    //s += "\<script src='https://code.jquery.com/jquery-3.3.1.min.js'\>\<\/script\>";
    //s += "\<script src='http://localhost:48724/Content/Scripts/app.proxy.js'\>\<\/script\>";
    //var html = '<body>' + s + '</body>';
    //document.body.appendChild(iframe);
    //iframe.contentWindow.document.open();
    //iframe.contentWindow.document.write(html);
    //iframe.contentWindow.document.close();

    iframe = document.createElement('iframe');
    iframe.style = "width:0px;height:0px;border:none;visibility:hidden;display:none";
    iframe.src = "http://localhost:48724/proxy01.html";
    document.body.appendChild(iframe);
}

getServerContent();

app.getSumProxy = function (a, b) {
    var msg = JSON.stringify({ a: a, b: b });
    iframe.contentWindow.postMessage(msg, "*");
}