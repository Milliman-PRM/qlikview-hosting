<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserStatus.aspx.cs" Inherits="MillimanSupport.UserStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" Text="Active Covisint User Identifiers"></asp:Label>
        <br />
        <asp:BulletedList ID="CUIDS" runat="server">
        </asp:BulletedList>
        <asp:Label ID="NUIDS" runat="server" Text="Active NextGen User Identifiers"></asp:Label>
        <br />
        <asp:BulletedList ID="BulletedList1" runat="server">
        </asp:BulletedList>
    </form>
</body>
</html>
