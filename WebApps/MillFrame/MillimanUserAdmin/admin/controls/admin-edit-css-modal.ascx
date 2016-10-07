<%@ Control Language="C#" AutoEventWireup="true" CodeFile="admin-edit-css-modal.ascx.cs" Inherits="admin_controls_admin_edit_css_modal" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc2" %>
<div class="cssEditBox">
    <asp:Literal ID="ltlCssUrl" runat="server"></asp:Literal>
    <asp:TextBox ID="TextBox1" runat="server" Height="380px" TextMode="MultiLine" Width="99%" Wrap="False"></asp:TextBox>
    <asp:Button ID="btnSave" runat="server" CssClass="inputButton" Text="Save Changes" OnClick="btnSave_Click" OnClientClick="return confirm('SAVE changes to CSS file?');" ToolTip="SAVE changes to CSS file?" />
    <div class="clearBoth">
    </div>
    <span class="messageWrap">
        <asp:HyperLink ID="Msg" runat="server" Visible="false" EnableViewState="false"></asp:HyperLink>
    </span>
</div>

<%-- jquery js --%>
<uc2:jquery ID="jquery1" runat="server" />
