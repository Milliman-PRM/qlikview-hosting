function RefreshPage(appTimeOut) {
    //alert(appTimeOut);

    var meta = document.createElement('meta');
    meta.httpEquiv = "refresh";
    meta.content = appTimeOut * 60;
    document.getElementsByTagName('head')[0].appendChild(meta);
    //alert("appTimeOut" + meta.content);

    //<meta http-equiv="refresh" content="60" />
}

