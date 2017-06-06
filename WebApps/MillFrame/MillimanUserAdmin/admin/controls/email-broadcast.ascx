<%@ Control Language="C#" AutoEventWireup="true" CodeFile="email-broadcast.ascx.cs" Inherits="admin_controls_email_broadcast" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register src="js-include2.ascx" tagname="js" tagprefix="uc1" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%-- gridview banner --%>
<div class="gvBanner">
  <span class="gvBannerUsers"><asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span>
  E-mail Broadcast<asp:Literal ID="categoryID" runat="server"></asp:Literal>
</div>
<%-- a-z repeater --%>
<div class="aToZWrap">
    <asp:Repeater ID="Repeater1" runat="server" DataSourceID="XmlDataSource1">
      <ItemTemplate>
        <div class="aTozNavigaion">
          <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#XPath("value")%>'>
            <asp:Label ID="Label1" runat="server" Text='<%#XPath("name")%>'></asp:Label>
          </asp:HyperLink>
        </div>
      </ItemTemplate>
    </asp:Repeater>
    <div class="clearBoth"></div>
</div>
<%-- a-z repeater datasource --%>
<asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="AtoZEmailRepeater.xml"></asp:XmlDataSource>
<%-- gridview default a-z membership users --%>
<asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" AllowSorting="True" DataSourceID="ObjectDataSource1" DataKeyNames="userName,Email" EmptyDataText="No records found." OnDataBound="GridView1_DataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnRowDataBound="GridView1_RowDataBound" CssClass="gv">
  <Columns>
    <asp:TemplateField>
      <HeaderStyle CssClass="gvHeader" Width="1px" />
      <ItemStyle CssClass="gvHeader" Width="1px" />
    </asp:TemplateField>
    <asp:BoundField DataField="RowNumber" HeaderText="#" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20px" />
    <asp:TemplateField HeaderText="Del">
      <HeaderTemplate>
        <input id="chkAll" onclick="SelectAllCheckboxes('chkRows',this.checked);" runat="server" type="checkbox" title="Check all checkboxes" />
      </HeaderTemplate>
      <ItemTemplate>
        <asp:CheckBox ID="chkRows" runat="server" ToolTip="Select user in this row." onclick="getElementById('ctl00_ContentPlaceHolder1_emailBroadcast_totalRecordCount').innerText = CountAllCheckboxesWithState('chkRows', true);" />
      </ItemTemplate>
      <ItemStyle Width="25px" HorizontalAlign="Center" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="USER NAME" SortExpression="UserName">
      <ItemTemplate>
        <span class="gvShortcut">
            <a href='edit_user_modal.aspx?username=<%# Server.HtmlEncode(Eval("UserName").ToString()) %>' rel="gb_page_center[750, 905]" title="Edit User Details">
          <%# Eval("UserName") %></a></span>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="EMAIL" SortExpression="Email">
      <EditItemTemplate>
        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Email") %>'></asp:TextBox>
      </EditItemTemplate>
      <ItemTemplate>
        »: <a href='Mailto:<%# Eval("Email") %>' title="click to email from your computer">
          <%#Eval("Email")%></a>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="createdate" HeaderText="ACCOUNT START" SortExpression="createdate" />
    <asp:BoundField DataField="lastlogindate" HeaderText="LAST LOGIN DATE" SortExpression="lastlogindate" />
    <asp:CheckBoxField DataField="IsApproved" HeaderText="APPROVED?" SortExpression="IsApproved">
      <ItemStyle HorizontalAlign="Center" />
    </asp:CheckBoxField>
    <asp:CheckBoxField DataField="IsLockedOut" HeaderText="LOCKED OUT?" SortExpression="IsLockedOut">
      <ItemStyle HorizontalAlign="Center" />
    </asp:CheckBoxField>
  </Columns>
  <RowStyle CssClass="gvRowStyle" />
  <AlternatingRowStyle CssClass="gvAlternateRowStyle" />
  <SelectedRowStyle CssClass="gvSelected" />
  <HeaderStyle CssClass="gvHeader" />
  <EditRowStyle CssClass="gvEdit" />
  <PagerTemplate>
    <div class="gvPagerFont">
      <asp:Label ID="Label2" runat="server" Text="Show rows:" />
      <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" CssClass="gvPagerFont">
        <asp:ListItem Value="5" />
        <asp:ListItem Value="10" />
        <asp:ListItem Value="15" />
        <asp:ListItem Value="20" />
        <asp:ListItem Value="40" />
        <asp:ListItem Value="80" />
        <asp:ListItem Value="1000" />
      </asp:DropDownList>
      &nbsp; Page
      <asp:TextBox ID="txtGoToPage" runat="server" AutoPostBack="true" OnTextChanged="GoToPage_TextChanged" Width="40px" CssClass="gvPagerFont" />&nbsp;of
      <asp:Label ID="lblTotalNumberOfPages" runat="server" />&nbsp;
      <asp:Button ID="btnFirst" runat="server" CommandName="Page" ToolTip="First" CommandArgument="First" Text="First" />
      <asp:Button ID="btnPrevious" runat="server" CommandName="Page" ToolTip="Previous Page" CommandArgument="Prev" Text="Prev" />
      <asp:Button ID="btnNext" runat="server" CommandName="Page" ToolTip="Next Page" CommandArgument="Next" Text="Next" />
      <asp:Button ID="btnLast" runat="server" CommandName="Page" ToolTip="Last" CommandArgument="Last" Text="Last" />
    </div>
  </PagerTemplate>
</asp:GridView>
<%-- gridview datasource --%>
<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="sp_wsat_GetUsersByName" TypeName="sp_wsat_Users" OnSelected="ObjectDataSource1_Selected" EnablePaging="True" SelectCountMethod="TotalNumberOfUsersByName">
  <SelectParameters>
    <asp:Parameter Name="UserName" Type="String" />
  </SelectParameters>
</asp:ObjectDataSource>
<%-- panel hide items if gridview empty --%>
<asp:Panel ID="pnlHideItems" runat="server" Visible="false">
  <%-- paging information label --%>
  <div class="messageWrap">
    <asp:HyperLink ID="PagingInformation" runat="server"></asp:HyperLink>
  </div>
  <%-- total record count --%>
  <div class="messageWrap">
    <asp:HyperLink ID="totalRecordCount" runat="server"></asp:HyperLink>
  </div>
  <%-- delete selected users button --%>
  <div class="buttonCSS">
    <asp:LinkButton ID="btnDeleteSelected" runat="server" OnClick="btnDeleteSelected_Click" OnClientClick="return confirm('DELETE selected users?');" ToolTip="DELETE the selected users." Visible="False">Delete</asp:LinkButton>
  </div>
  <%-- send email to selected users --%>
  <div class="buttonCSS">
    <asp:LinkButton ID="btnSendEmailToSelected" runat="server" OnClick="btnSendEmailToSelected_Click" OnClientClick="return confirm('Send E-MAIL to SELECTED users?');" ToolTip="Send E-MAIL to SELECTED users." ValidationGroup="vgEmail">Send E-mail</asp:LinkButton>
  </div>
  <%-- send email to all users --%>
  <div class="buttonCSS">
    <asp:LinkButton ID="btnSendEmailToAll" runat="server" OnClick="btnSendEmailToAll_Click" OnClientClick="return confirm('Send E-MAIL to ALL users?');" ToolTip="Send E-MAIL to ALL users." ValidationGroup="vgEmail">Send To All</asp:LinkButton>
  </div>
 
  <%-- send mail to users in selected role --%>
  <div class="ddlWrap">
    <asp:DropDownList ID="ddlSendMailToSelectedRole" runat="server" AutoPostBack="True" DataSourceID="RolesDataSource" DataTextField="RoleName" DataValueField="RoleName" AppendDataBoundItems="True" EnableViewState="False" Font-Size="11px" OnSelectedIndexChanged="ddlSendMailToSelectedRole_SelectedIndexChanged" ToolTip="E-MAIL ALL users in selected group." ValidationGroup="vgEmail">
      <asp:ListItem Selected="True">Send To Group</asp:ListItem>
    </asp:DropDownList>
  </div>
  <%-- dropdown list datasource --%>
  <asp:ObjectDataSource ID="RolesDataSource" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="wsat_GetRoles" TypeName="sp_wsatTableAdapters.wsat_RolesTableAdapter"></asp:ObjectDataSource>
  <%-- message label --%>
  <div class="messageWrap">
    <asp:HyperLink ID="Msg" runat="server" Visible="False" EnableViewState="false"></asp:HyperLink>
  </div>
</asp:Panel>
<%-- div padding --%>
<div class="padding"></div>
<%-- validation summary --%>
<asp:ValidationSummary ID="vsEmailValidation" runat="server" ValidationGroup="vgEmail" BackColor="#F2F2F2" />
<div>
  <%-- subject and from textfields  --%>
  <table cellpadding="3" cellspacing="0" style="width: 99%;">
    <tr>
      <td class="subjectAndFrom" style="text-align:right" >
            Template:
      </td>
        <td>
            <asp:DropDownList  style="width:250px" ID="EmailTemplates" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EmailTemplates_SelectedIndexChanged"></asp:DropDownList>
        </td>
      <td class="subjectAndFrom" style="text-align:right" >
        Subject:<asp:RequiredFieldValidator ID="rfvSubject" runat="server" ErrorMessage="Subject is required!" ControlToValidate="txb_Subject" Display="Dynamic" EnableViewState="False" SetFocusOnError="True" ValidationGroup="vgEmail">*</asp:RequiredFieldValidator>
      </td>
      <td >
        <asp:TextBox ID="txb_Subject" runat="Server" Width="250px" ToolTip="Type a subject for this email. Subject cannot be left empty." MaxLength="100">Milliman HCIntel Support</asp:TextBox>
      </td>
      <td class="subjectAndFrom" style="text-align:right">
        From:<asp:RequiredFieldValidator ID="rfvReturnAddress" runat="server" ErrorMessage="Return address is required!" ControlToValidate="txbMailFrom" Display="Dynamic" EnableViewState="False" ValidationGroup="vgEmail">*</asp:RequiredFieldValidator>
      </td>
      <td >
        <asp:TextBox ID="txbMailFrom" runat="server" Width="250px" MaxLength="100">HCIntel.Support@Milliman.com</asp:TextBox>
      </td>
          <td class="subjectAndFrom" style="text-align:right" >
            Macros:
      </td>
        <td>
            <asp:DropDownList  style="width:250px" ID="MacroList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="MacroList_SelectedIndexChanged" ></asp:DropDownList>
        </td>
    </tr>
  </table>
  <%-- email header  --%>
  <div class="padding2 emailTypeAndPriority">
    <asp:RadioButtonList ID="rbt_BodyTextType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="HTML or Palin Text Email?">
      <asp:ListItem Selected="True" Value="True">html</asp:ListItem>
      <asp:ListItem Value="False">plain text</asp:ListItem>
    </asp:RadioButtonList>
    &nbsp;|
    <asp:RadioButtonList ID="rbt_Importance" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="Select Email Priority">
      <asp:ListItem Value="High">high</asp:ListItem>
      <asp:ListItem Selected="True" Value="Normal">default</asp:ListItem>
      <asp:ListItem Value="Low">low</asp:ListItem>
    </asp:RadioButtonList>
  </div>
  <%-- fck editor  --%>
  <FCKeditorV2:FCKeditor ID="WYSIWYGEditor_EmailBody" runat="server" BasePath="~/js/FCKeditor/" Height="400px" Value="Start typing your email here..." ToolbarStartExpanded="False">
  </FCKeditorV2:FCKeditor>
</div>

<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<%-- js include  --%>
<uc1:js ID="js1" runat="server" />