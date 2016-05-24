<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tickets.aspx.cs" Inherits="MillimanSupport.Tickets" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadSkinManager ID="QsfSkinManager" runat="server" ShowChooser="false" />
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="true" />

    <telerik:RadPanelBar ID="RadPanelBar1" runat="server"  Width="100%"   CollapseDelay="100" ExpandDelay="100">
        <Items>
            <telerik:RadPanelItem ID="NewIssuePanel" Text="Create Support Ticket" Expanded="False" runat="server" ImagePosition="Left" ToolTip="Create a new support ticket." ExpandedImageUrl="~/Images/math-add-icon16.png" DisabledImageUrl="~/Images/math-add-icon16.png" ImageUrl="~/Images/math-add-icon16.png">
                <ContentTemplate>
                    <asp:Table ID="Table1" runat="server" Height="350px">
                        <asp:TableRow ToolTip="Email address to notify when the status of the issue changes">
                            <asp:TableCell Width="150">
                                <asp:Label ID="Label1" runat="server" Text="Notification Account"></asp:Label></asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="NotificationAccount" runat="server"></asp:DropDownList></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow ToolTip="The Covisint ticket ID associated with this request">
                            <asp:TableCell>
                                <asp:Label ID="Label2" runat="server" Text="Covisint Ticket ID"></asp:Label></asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox ID="CovisintTicketID" runat="server"></asp:TextBox></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow ToolTip="The Covisint CUID or account name of the user which experienced the issue">
                            <asp:TableCell>
                                <asp:Label ID="Label3" runat="server" Text="User CUID/Account"></asp:Label></asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox ID="CUID" runat="server"></asp:TextBox></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow ToolTip="A description of the issue">
                            <asp:TableCell>
                                <asp:Label ID="Label4" runat="server" Text="Problem Description"></asp:Label></asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox ID="Description" runat="server" TextMode="MultiLine" Width="400" Height="200"></asp:TextBox></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                                <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Submit_Click" />
                            </asp:TableCell>
                                
                        </asp:TableRow>
                    </asp:Table>
                </ContentTemplate>
            </telerik:RadPanelItem>

        </Items>
    </telerik:RadPanelBar>

    <telerik:RadGrid runat="server" ID="Issues" AllowPaging="true" AllowSorting="true" OnSortCommand="Issues_SortCommand" OnPageIndexChanged="Issues_PageIndexChanged" OnPageSizeChanged="Issues_PageSizeChanged" OnPreRender="Issues_PreRender"></telerik:RadGrid>          
    </form>
</body>
</html>
