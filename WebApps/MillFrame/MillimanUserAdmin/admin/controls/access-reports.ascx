<%@ Control Language="C#" AutoEventWireup="true" CodeFile="access-reports.ascx.cs" Inherits="admin_controls_admin_reports" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="js-include1.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register src="js-include2.ascx" tagname="js" tagprefix="uc2" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register src="~/js/js/jquery.ascx" tagname="jquery" tagprefix="uc4" %>
<div class="gvBanner">
    <span class="gvBannerThemes">
        <asp:Image ID="Image2" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Reports</div>
<div class="cuwWrap" style="width:600px;bottom:20px">
    <table cellpadding="3" cellspacing="0" >
    <tr>
      <td class="subjectAndFrom" style="text-align:right" >
            Report Type
      </td>
        <td >
            <asp:DropDownList  style="width:150px" ID="ReportType" runat="server" AutoPostBack="True" AppendDataBoundItems="true" OnSelectedIndexChanged="ReportType_SelectedIndexChanged" >
                <asp:ListItem Text="User / Group" Value="UserGroup"></asp:ListItem>
                <asp:ListItem Text="User / QVWs" Value="UserQVWS"></asp:ListItem>
                <asp:ListItem Text="Group Contents" Value="Group"></asp:ListItem>
                <asp:ListItem Text="QVWs / User" Value="QVWSUser"></asp:ListItem>
                <asp:ListItem Text="QVWs / Group" Value="QVWSGroup"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td style="width:25px">

        </td>
       <td class="subjectAndFrom" style="text-align:right" >
            <asp:Label ID="SelectionLabel" runat="server">Select a user</asp:Label>
      </td>
      <td>
          <asp:DropDownList ID="UserSelections"  style="width:250px"  runat="server" AutoPostBack="False" ></asp:DropDownList>
     </td>
        <td>
            <asp:Button ID="Generate" runat="server" Text="Generate Report" OnClick="Generate_Click" />
        </td>
    </tr>
    </table>
    <center><hr style="width:99%" /></center>
    <telerik:RadTreeView ID="Report" runat="server"></telerik:RadTreeView>
</div>

<%-- help sidebar --%>
<div id="helpSidebarShow" class="helpSidebarShow">
    <a onclick="ShowHide(); return false;" href="#">
    H<br />
    I<br />
    N<br />
    T
    </a>
</div>
<div id="helpSidebar" class="helpSidebar" style="display: none;">
    <span class="helpSidebarClose">
        <a onclick="ShowHide(); return false;" href="#">CLOSE</a>
    </span>
    <div class="clearBoth2"></div>
    <div class="helpHintIcon"></div>
    <div>
        <asp:Repeater ID="rptHelp" runat="server" DataSourceID="xmlHelp">
            <ItemTemplate>
                <div class="helpTitle">
                    <asp:Literal ID="ltlTitle" runat="server" Text='<%#XPath("title")%>'></asp:Literal>
                </div>
                <div class="helpText">
                    <asp:Literal ID="ltlText" runat="server" Text='<%#XPath("text")%>'></asp:Literal>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:XmlDataSource ID="xmlHelp" runat="server" DataFile="~/admin/help/admin-reports.xml"></asp:XmlDataSource>
    </div>
</div>
<%-- sidebar help js --%>
<uc3:js ID="js3" runat="server" />
<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<uc1:js ID="js1" runat="server" />
<uc2:js ID="js2" runat="server" />