<%@ Control Language="C#" AutoEventWireup="true" CodeFile="bulk-create-user-with-role.ascx.cs" Inherits="bulk_admin_controls_create_user_with_role" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%@ Register Src="~/admin/controls/UserRolesSelector.ascx" TagName="userRoleSelector" TagPrefix="urs" %>

<style>
    .roundShadowContainer {
        width: 754px;
        margin-top: 10px;
    }

    .containerWrap {
        margin: 0 auto;
        padding: 2px;
        width: 20%;
    }

    .left {
        float: left;
        margin: 0px -7px 0px 4px;
        text-align: left;
        padding: 2px;
    }

    .right {
        float: right;
        margin: 0px -7px 0px 4px;
        text-align: left;
    }
</style>


<%-- gridview banner --%>
<div class="gvBanner">
    <span class="gvBannerUsers">
        <asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Create Users With Group
</div>

<telerik:RadAjaxManager ID="RadAjaxManager2" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGrid1"></telerik:AjaxUpdatedControl>
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>

<telerik:RadWindowManager EnableShadow="true" ID="RadWindowManager" runat="server">
</telerik:RadWindowManager>

<div class="centerElement roundShadowContainer">

    <div style="height: 23px; width: 333px;">
        <div class="left"><span class="engravedHeader">Login Type:</span></div>
        <div class="clearfix"></div>
        <div class="right">
            <%--panel for UserType--%>
            <asp:RadioButtonList ID="UserType" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="UserType_SelectedIndexChanged" RepeatDirection="Horizontal">
                <asp:ListItem Selected="true">Milliman Login</asp:ListItem>
                <asp:ListItem>External SSO Login</asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </div>
    <div class="space"></div>
    <div id="divUserRole" class="softRoundContainerStyle">
        <%--panel for updPanelUserRoles--%>
        <asp:UpdatePanel ID="updPanelUserRoles" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <urs:userRoleSelector ID="ctrlUserRoles" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="space"></div>
    <div class="softRoundContainerStyle">
        <telerik:RadGrid runat="server" ID="RadGrid1" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="5" GridLines="None"
            OnItemCommand="RadGrid1_ItemCommand"
            AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True" ClientIDMode="AutoID">
            <MasterTableView EditMode="Batch" CommandItemDisplay="Top" TableLayout="Fixed">
                <CommandItemTemplate>
                    <asp:LinkButton ID="Add" runat="server" CommandName="Add" Visible="true"><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Office-Girl-icon.png"/>Add List Entry</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton Width="100px" ID="Validate" runat="server" CommandName="Validate" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server"  style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/process-icon.png"/>Validate</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton ID="Clear" runat="server" CommandName="Clear" ToolTip="Clears the selected group, User Accounts in grid and CSV list" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Actions-edit-delete-icon16x16.png"/>Clear List</asp:LinkButton>&nbsp;&nbsp;
                </CommandItemTemplate>
                <Columns>
                    <telerik:GridTemplateColumn DataField="ValidationImage" UniqueName="ValidationImageStatus" HeaderStyle-Width="20px">
                        <ItemTemplate>
                            <asp:Image ID="ValidationStatusImage" runat="server" ImageUrl='<%#Eval("ValidationImage") %>' ToolTip='<%#Eval("ErrorMsg") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="Account_Name" HeaderText="Account" UniqueName="AccountNameText" HeaderStyle-Width="100%">
                        <ItemTemplate>
                            <asp:TextBox ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="90%" Height="20px" CssClass="standardTextBox"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Width="100%"></HeaderStyle>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="SendWelcomeEmail" HeaderText="Send Welcome" UniqueName="SendWelcome" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="SendWelcomeCheckbox" runat="server" AutoPostBack="false" Checked='<%#Eval("SendWelcomeEmail") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="DataAccess_Required" HeaderText="<center>Database Access</center>" UniqueName="DataAccessRequiredText">
                        <ItemTemplate>
                            <center><asp:CheckBox ID="DataAccessRequiredTextBox" AutoPostBack="false" runat="server" Checked='<%#Eval("DataAccess_Required") %>'/></center>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" ConfirmText="Delete this user from the list?" ConfirmDialogType="Classic">
                        <HeaderStyle Width="32px" />
                    </telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
    <div class="space"></div>

    <div class="softRoundContainerStyle">

        <telerik:RadPanelBar ID="RadPanelBar1" runat="server" Width="100%" CollapseDelay="100" ExpandDelay="100" ExpandMode="MultipleExpandedItems">
            <Items>
                <telerik:RadPanelItem ID="UserPanel" Value="UserPanel" Text="CSV List Entries" Expanded="False" runat="server" ImagePosition="Left" ToolTip="Click to expand and paste new user information here" ExpandedImageUrl="~/Images/User-Group-icon.png" DisabledImageUrl="~/Images/User-Group-icon.png" ImageUrl="~/Images/User-Group-icon.png">
                    <ContentTemplate>
                        <asp:Table ID="Table1" runat="server" Height="241px" Width="100%">
                            <asp:TableRow ToolTip="Paste new user information using comma delimited format:<br> Email Address, [True/False] with *1 entry per line">
                                <asp:TableCell>
                                <div class="softRoundContainerStyle infoBox" style="width:700px;"><span>&nbsp;&nbsp;Paste new user information using comma delimited format such as;</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>Email Address,[True/False]</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="right" style="margin-top: 1px;margin-right:5px!important;"><i>Note</i>: *1 entry per line</span></div>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow Width="100%">
                                <asp:TableCell Width="100%">
                                    <asp:UpdatePanel ID="updPanelUserList" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:TextBox runat="server" ID="UserList" TextMode="MultiLine" Rows="20" Width="98%" Height="150px"></asp:TextBox>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Center">
                                    <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Submit_Click" CssClass="buttonGray" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>

        <div class="space"></div>
        <div class="containerWrap">
            <asp:ImageButton ID="CreateNewUsers" runat="server" ImageUrl="~/images/CreateUsersButton.png" ToolTip="Add the listed users to the HCIntel system." OnClick="CreateNewUsers_Click" />
        </div>
        <div class="space"></div>
    </div>
</div>

<%-- help sidebar --%>
<div id="helpSidebarShow" class="helpSidebarShow">
    <a onclick="ShowHide(); return false;" href="#">H<br />
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
        <asp:XmlDataSource ID="xmlHelp" runat="server" DataFile="~/admin/help/create-user-with-role.xml"></asp:XmlDataSource>
    </div>
</div>
<%-- sidebar help js --%>
<uc3:js ID="js3" runat="server" />
<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<script type="text/javascript">

    function EnableDisable() {
        return false;
    }

    function ConfirmAction() {
        return window.confirm("Are you certain you want to create these users?");
    }


</script>
