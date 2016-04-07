<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotSelectable.aspx.cs" Inherits="MillimanClientUserAdmin.NotSelectable" %>

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

        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
