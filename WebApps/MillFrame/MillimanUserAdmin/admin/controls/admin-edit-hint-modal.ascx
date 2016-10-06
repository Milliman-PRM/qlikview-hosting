<%@ Control Language="C#" AutoEventWireup="true" CodeFile="admin-edit-hint-modal.ascx.cs" Inherits="admin_controls_admin_edit_hint_modal" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register src="~/js/js/jquery.ascx" tagname="jquery" tagprefix="uc2" %>
<div class="hintEditBox">
    <asp:Literal ID="ltlHintUrl" runat="server"></asp:Literal>
    <asp:TextBox ID="txbHint" runat="server" Height="380px" TextMode="MultiLine" Width="100%" Wrap="False"></asp:TextBox>
    <asp:Button ID="btnSave" runat="server" CssClass="inputButton" Text="Save Changes" onclick="btnSave_Click" OnClientClick="return confirm('SAVE changes to Hint file?');" ToolTip="SAVE changes to Hint file?" />
    <div class="clearBoth"></div>
    <span class="messageWrap"><asp:HyperLink ID="Msg" runat="server" Visible="false" EnableViewState="false"></asp:HyperLink></span>
</div>

<%-- jquery js --%>
<uc2:jquery ID="jquery1" runat="server" />