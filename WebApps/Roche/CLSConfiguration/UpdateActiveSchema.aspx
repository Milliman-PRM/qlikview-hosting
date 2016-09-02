<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateActiveSchema.aspx.cs" Inherits="CLSConfiguration.UpdateActiveSchema" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 30px;
            width: 592px;
        }
        .auto-style2 {
            width: 592px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="position:fixed;top:50%;left:50%;width:600px;height:200px;margin-top:-100px;margin-left:-300px;border:1px solid #999999">

            <table style="padding:20px">
                     <tr>
                        <td class="auto-style1"></td>
                    </tr>
                    <tr>
                        <td style="text-align:center" class="auto-style2">
                            <asp:Label ID="Status" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style1"></td>
                    </tr>
                    <tr>
                        <td  style="text-align:center;" class="auto-style2">
                            <asp:Button ID="Refresh" runat="server" Text="Return to Main Page" Visible="False" OnClick="Refresh_Click" />
                        </td>
                    </tr>
                </table>
    </div>
    </form>
</body>
</html>
