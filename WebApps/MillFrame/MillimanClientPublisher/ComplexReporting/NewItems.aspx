<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewItems.aspx.cs" Inherits="ClientPublisher.ComplexReporting.NewItems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
