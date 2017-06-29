<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QVWImporter.aspx.cs" Inherits="MillimanProjectManConsole.Admin.QVWImporter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="refresh" content="3">  <%--refresh every 5 seconds--%>
    <title>Create Project from QVW Import - Importing</title>
    <style type="text/css">
    .CenterMeHorVer{
	    width:600px;
	    height:150px;
	    position:absolute;
	    left:50%;
	    top:50%;
	    margin:-75px 0 0 -300px;
        background-color:white;
        font-size:14px;
        border:1px solid black;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="CenterMeHorVer">
        <center>
            <br /><br />
            <img src="../images/ajax-loader.gif" />
            <br />
            <span><asp:Label runat="server" ID="StatusMessage"> Please wait - import in progresss....</asp:Label></span>
        </center>
    </div>
    </form>

    <script type="text/javascript">

        function getRadWindow() {
            var oWindow = null;

            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;

            return oWindow;
        }

        function clientClose(arg) {
            getRadWindow().close(arg);
        }

</script></body>
</html>
