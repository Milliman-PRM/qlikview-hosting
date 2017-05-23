<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoReductionVerify.aspx.cs" Inherits="ClientPublisher.NoReductionVerify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Project Update Verification</title>
    <link href="Content/Style/bootstrap.css" rel="stylesheet" />
    <link href="Content/Style/MillframeStyle.css" rel="stylesheet" />
    <style type="text/css">
        .CenterMeHorVer {
            width: 600px;
            height: 160px;
            position: absolute;
            left: 50%;
            top: 50%;
            margin: -80px 0 0 -300px;
            background-color: white;
            font-size: 14px;
            border: 1px solid black;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
        <div class="CenterMeHorVer">
            <br />
            <table style="width: 600px">
                <tr>
                    <td colspan="3">
                        <asp:Panel runat="server">Update project '
                            <asp:Label runat="server" ID="ProjectName"></asp:Label>' with the following new items</asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%">&nbsp;</td>
                    <td style="width: 40%">
                        <asp:BulletedList runat="server" ID="NewItems"></asp:BulletedList>
                    </td>
                    <td style="width: 30%">&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button runat="server" ID="ApplyUpdatesNow" Text="Apply Now*" CssClass="btn btn-primary"
                            ToolTip="Apply the changes now.  Closing the window will result in updating the items for the project again."
                            OnClick="ApplyUpdatesNow_Click"></asp:Button></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align:center"><p style="font: bold 9px times new roman; color: darkred">*Files related to previous restricted views will be removed.</p></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
