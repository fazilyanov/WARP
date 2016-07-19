function pinger() {
    $.post('/site/pinger.aspx', function (result) { });
}
setInterval(pinger, 300000);