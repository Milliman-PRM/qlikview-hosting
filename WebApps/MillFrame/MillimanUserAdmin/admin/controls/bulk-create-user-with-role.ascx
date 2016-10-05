<%@ Control Language="C#" AutoEventWireup="true" CodeFile="bulk-create-user-with-role.ascx.cs" Inherits="bulk_admin_controls_create_user_with_role" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%@ Register Src="~/admin/controls/UserRolesSelector.ascx" TagName="userRoleSelector" TagPrefix="urs" %>

<style>
    .roundShadowContainer {
        margin-top: 10px;
    }

    .containerWrap {
        padding: 4px;
        width: 75%;
        background-color: #fdfdfd;
    }

    .left {
        float: left;
        margin: 3px 0 0 6px;
        text-align: left;
        padding: 2px;
    }

    .right {
        float: right;
        margin: -5px -23px 0 0;
        text-align: left;
    }

    .engravedHeader {
        font-size: 14px;
    }

    #divOuter {
        width: 770px;
    }

    #divLoginType {
        height: 31px;
        width: 400px;
        margin: 7px 10px 9px 59px;
        padding: 2px 42px 5px 5px;
        border: 2px dashed #eee;
        font-weight: 400;
    }

    #divUserRole {
        margin: 8px 9px 10px 58px;
            width: 650px;
    }

    #divResults {
        width: 750px;
    }

    #divUserAddList {
        width: 750px;
        margin: 0 auto;
    }

    #divCreate {
        margin-bottom: 8px;
    }

    .radioButtonlabel {
        font-weight: 400;
        font-size: 12px;
    }

    .userList {
        margin: 0 -11px -2px 2px;
        padding: 2px;
    }
    /*#divImportantHint{float:right;top:110px;width:550px}*/
    .imageContainer {
        position: relative;
    }

    .tag {
        float: left;
        position: absolute;
        left: 0px;
        top: 0px;
        z-index: 1000;
        background-color: #92AD40;
        padding: 5px;
        color: #FFFFFF;
        font-weight: bold;
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

<div class="containerWrap center-block">

    <div id="divOuter" class="roundShadowContainer">

        <div id="divLoginType">
            <div class="left"><span class="engravedHeader">Login Type:</span></div>
            <div class="space"></div>
            <div class="right">
                <%--panel for UserType--%>
                <asp:UpdatePanel ID="updPanelUserType" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="UserType" runat="server" AutoPostBack="True"
                            OnSelectedIndexChanged="UserType_SelectedIndexChanged" RepeatDirection="Horizontal" CssClass="radioButtonlabel">
                            <asp:ListItem Selected="true">Milliman Login</asp:ListItem>
                            <asp:ListItem>External SSO Login</asp:ListItem>
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="space"></div>
        <div class="space"></div>
        <div id="divUserRole">
            <%--panel for updPanelUserRoles--%>
            <asp:UpdatePanel ID="updPanelUserRoles" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <urs:userRoleSelector ID="ctrlUserRoles" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="space"></div>

        <div id="divResults" class="roundShadowContainer">
            <telerik:RadGrid runat="server" ID="RadGrid1" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="5" GridLines="None"
                OnItemCommand="RadGrid1_ItemCommand"
                AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True" ClientIDMode="AutoID" ClientSettings-ClientEvents-OnRowDeleting="RowDeleting">
                <MasterTableView EditMode="Batch" CommandItemDisplay="Top" TableLayout="Fixed">
                    <CommandItemTemplate>
                        <asp:LinkButton ID="Add" runat="server" CommandName="Add" Visible="true"><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Office-Girl-icon.png"/>Add New</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton Width="100px" ID="Validate" runat="server" CommandName="Validate" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server"  style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/process-icon.png"/>Validate</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton ID="Clear" runat="server" CommandName="Clear" ToolTip="Clears the selected group, User Accounts in grid and CSV list" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Actions-edit-delete-icon16x16.png"/>Clear All</asp:LinkButton>&nbsp;&nbsp;
                    </CommandItemTemplate>
                    <Columns>
                        <telerik:GridTemplateColumn DataField="ValidationImage" UniqueName="ValidationImageStatus" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:Image ID="ValidationStatusImage" runat="server" ImageUrl='<%#Eval("ValidationImage") %>' ToolTip='<%#Eval("ErrorMsg") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="Account_Name" HeaderText="Account Name" UniqueName="AccountNameText" HeaderStyle-Width="100%">
                            <ItemTemplate>
                                <asp:TextBox ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="90%" Height="27px" CssClass="standardTextBox"></asp:TextBox>
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

        <div id="divUserAddList" class="roundShadowContainer">
            <telerik:RadPanelBar ID="RadPanelBar1" runat="server" Width="100%" CollapseDelay="100" ExpandDelay="100" ExpandMode="MultipleExpandedItems" AllowCollapseAllItems="True">
                <Items>
                    <telerik:RadPanelItem ID="UserPanel" Value="UserPanel" Text="Add New by CSV List" Expanded="False" runat="server" ImagePosition="Left" ToolTip="Click to expand and paste new user information here" ExpandedImageUrl="~/Images/User-Group-icon.png" DisabledImageUrl="~/Images/User-Group-icon.png" ImageUrl="~/Images/User-Group-icon.png">
                        <ContentTemplate>
                            <asp:Table ID="Table1" runat="server" Height="218px" Width="100%">
                                <asp:TableRow Width="100%">
                                    <asp:TableCell Width="100%">
                                        <asp:UpdatePanel ID="updPanelUserList" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:TextBox runat="server" ID="UserList" TextMode="MultiLine" Rows="20" Width="98%" Height="150px" CssClass="userList"></asp:TextBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <%--divImportantHint hint--%>
                                        <div id="divImportant" class="divImportant" style="float: left;">
                                            <img id="img2" src="~/Content/Images/Info-blue.png" runat="server" width="18" height="18" style="margin: 2px 6px 6px 2px;" />
                                            <div id="divImportantHint" style="position: fixed; display: none;" onmouseover="hoverdiv(event,'divImportantHint')"
                                                onmouseout="hoverdiv(event,'divImportantHint')">
                                                <div class="alert alert-warning infoBox text-justify">
                                                    <strong>Important!</strong>
                                                    <br />
                                                    <span>&nbsp;&nbsp;Paste new user information using comma delimited format such as;</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>Email Address,[True/False]</span>
                                                    <br />
                                                    <span><i>&nbsp;&nbsp;*Only 1 entry per line</i></span>
                                                </div>
                                            </div>
                                        </div>
                                        <%--divImportantHint hint--%>
                                        <div id="divSubmit" class="center-block" style="margin: 0px 30px 2px 2px;">
                                            <asp:Button ID="Button1" runat="server" Text="Submit List" OnClick="Submit_Click" CssClass="btn btn-primary" />
                                        </div>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
        </div>
        <div class="space"></div>
        <div id="divCreate" class="center-block">
            <asp:Button ID="CreateNewUsers" runat="server" CommandName="CreateNewUsers" Text="Save Changes" CssClass="btn btn-primary"
                OnClick="CreateNewUsers_Click" />
        </div>
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

<script src="../../Content/Script/jquery.v1.7.1.js"></script>
<script src="../../Content/Script/jquery.min.v2.1.1.js"></script>
<script src="../../Content/Script/bootstrap.min.v3.3.7.js"></script>
<telerik:RadScriptBlock ID="radscript3" runat="server">
    <script type="text/javascript">

        //***************** display the importatn hint next to the image ******************************//
        var moveTop = "125px";
        $('.divImportant').hover(function (e) {
            $('#divImportantHint').show();
        }, function () {
            $('#divImportantHint').hide();
        });

        //$('.divImportant').mousemove(function (e) {
        //    $("#divImportantHint").css('top', moveTop);
        //});


        function hoverdiv(e, divid) {

            var left = e.clientX + "px";
            var top = e.clientY + "px";

            var div = document.getElementById(divid);

            div.style.left = left;
            div.style.top = top;

            $("#" + divid).toggle();
            return false;
        }


        //top: 125px;
        function EnableDisable() {
            return false;
        }

        function ConfirmAction() {
            return window.confirm("Are you certain you want to create these users?");
        }
        //This method is used when deleting the 'last' row in the grid, it cancels the removal of the 
        //last row and clear then values in the row.  Event is attached to deleting row of RadGrid
        function RowDeleting(sender, eventArgs) {
            var grid = $find('<%=RadGrid1.ClientID %>');
            if (grid) {
                var MasterTable = grid.get_masterTableView();
                if (MasterTable) {
                    var Rows = MasterTable.get_dataItems();
                    if (Rows.length == 1) { //only when 1 row
                        //clear the values
                        Rows[0].get_cell("AccountNameText").childNodes[1].value = "";
                        Rows[0].get_cell("SendWelcome").childNodes[1].checked = false;
                        eventArgs.set_cancel(true);
                    }
                }
            }
        }

    </script>
</telerik:RadScriptBlock>
