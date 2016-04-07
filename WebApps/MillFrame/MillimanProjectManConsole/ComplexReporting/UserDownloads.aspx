<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDownloads.aspx.cs" Inherits="MillimanProjectManConsole.ComplexReporting.UserDownloads" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .RadGrid
            {
                border-radius: 10px;
                overflow: hidden;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadGrid ID="UserDownloadItems" runat="server"  ViewStateMode="Disabled" Skin="Silk" Width="98%" AllowPaging="false" AutoGenerateColumns="true">
            <MasterTableView Width="98%">

            </MasterTableView>
        </telerik:RadGrid>
    </form>
</body>
</html>
