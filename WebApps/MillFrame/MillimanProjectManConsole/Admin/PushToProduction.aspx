<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PushToProduction.aspx.cs" Inherits="MillimanProjectManConsole.Admin.PushToProduction" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="font-size: 14px;background-color:white;"  >
    <br /><br />
    <form id="form1" runat="server">
        <center>
    <div style="border: 1px solid #C0C0C0;  width: 500px; height: 150px; padding:20px">
    <br />
        <center><asp:Label runat="server" ID="Status"></asp:Label></center>
    
   <br />
        <div id="Container" runat="server" style="border: 1px solid #C0C0C0; overflow: hidden; text-align:left; background-color: #EEEEEE;vertical-align:middle;margin:1px 3px 1px 3px"   >
            <asp:Image  Height="20px" Width="0%" ID="ProgressBar" runat="server" ImageUrl="~/images/header.gif" BorderStyle="Inset" BorderWidth="1" />
        </div>
        <br />
        <center><asp:Label style="text-align:center;width:98%" runat="server" ID="DetailMessage" ForeColor="black" Font-Size="Small"></asp:Label> </center>
       </div>
        </center>
     </form>
</body>
</html>
