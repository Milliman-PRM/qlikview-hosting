<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QVWImportVerify.aspx.cs" Inherits="MillimanProjectManConsole.Admin.QVWImportVerify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Project from QVW Import - Verification</title>
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
        text-align:center;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="CenterMeHorVer">
    <asp:Label runat="server" ID="Verify"></asp:Label>
        <br />
    <asp:Button runat="server" ID="VerifiedClick" Text="Close" OnClick="VerifiedClick_Click" Width="95px" Visible="False" />
        <asp:HiddenField runat="server" ID="VerificationAction" />
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

</script>
</body>
</html>
