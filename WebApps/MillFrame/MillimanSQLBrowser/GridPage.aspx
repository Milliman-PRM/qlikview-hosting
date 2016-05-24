<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridPage.aspx.cs" Inherits="WebSqlUtility.GridPage" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="margin:0px">
    <form id="form1" runat="server">
           <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
        </telerik:RadStyleSheetManager>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
                </asp:ScriptReference>
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>

        <div ID="GridDiv" style="position:absolute;right:10px;left:10px;bottom:10px;top:10px;">
           <asp:Label ID="ErrorMsg" runat="server" Visible="False" ForeColor="Red" Font-Bold="True" Font-Italic="True"></asp:Label>
           <telerik:RadGrid width="100%" Height="100%" ID="RadGrid1" runat="server" AllowPaging="True" CellSpacing="0" GridLines="None" OnPageIndexChanged="RadGrid1_PageIndexChanged" OnPageSizeChanged="RadGrid1_PageSizeChanged" PageSize="25" HeaderStyle-Width="200px" ShowStatusBar="True" >
           <MasterTableView Width="100%" />
                   <ClientSettings>
                   <Selecting AllowRowSelect="True" />
                   <Scrolling AllowScroll="True" UseStaticHeaders="True" />
               </ClientSettings>

           </telerik:RadGrid>
        </div>
    </form>
</body>
</html>
