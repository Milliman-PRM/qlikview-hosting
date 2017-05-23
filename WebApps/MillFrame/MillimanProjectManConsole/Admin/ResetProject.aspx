<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetProject.aspx.cs" Inherits="MillimanProjectManConsole.Admin.ResetProject" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
            <br />
   <center><asp:Image ID="Image1" runat="server" ImageUrl="~/images/Roundedblocks.gif" /></center><br />
    <asp:Label runat="server" ID="IndexDownload"></asp:Label><br /><br />
    </div>
    </form>

    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) {
                oWindow = window.radWindow;
            }
            else if (window.frameElement.radWindow) {
                oWindow = window.frameElement.radWindow;
            }
            return oWindow;
        }

        var RW = GetRadWindow();
        if (RW.set_title)
            RW.set_title("Index/User Selections Retrieval");
    </script>
</body>
</html>
