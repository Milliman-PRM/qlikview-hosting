<%@ Control Language="C#" AutoEventWireup="true" CodeFile="bulk-create-user-with-role.ascx.cs" Inherits="bulk_admin_controls_create_user_with_role" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%@ Register Src="~/admin/controls/UserRolesSelector.ascx" TagName="userRoleSelector" TagPrefix="urs" %>

<style type="text/css">
    .cuwWrap {
        width: 754px;
    }
    .cuwWrap-editor {
        background-color: white;
        border: 1px solid #B0AEAE;
        height: 200px;
        margin: 3px;
        overflow: hidden;
        padding: 10px;
        width: 293px;
    }

    .cuwWrap-editor {
        -moz-transition: all 0.5s ease-in-out 0s;
        -moz-box-shadow: 0 0 52px #B0AEAE;
        -moz-border-radius-bottomleft: 15px;
        -moz-border-radius-bottomright: 15px;
        -moz-border-radius-topleft: 15px;
        -moz-border-radius-topright: 15px;
        -webkit-border-radius: 15px;
    }

    .mainTable {
        -moz-transition: all 0.5s ease-in-out 0s;
        background-color: #e8e8e8;
        border-collapse: collapse;
        margin-bottom: 5px;
        width: 750px;
    }

    .boxStyle {
        border: solid 2px #fff;
        border-radius: 4px;
        box-shadow: 0px 0px 5px #888;
    }
</style>



<div class="adminHelp">
    1.) Minimum Required Password Length = 7 char.<br />
    2.) Minimum Required Non-Alphanumeric char = 1.<br />
    3.) Passwords are case sensitive.
</div>
<%-- gridview banner --%>
<div class="gvBanner">
    <span class="gvBannerUsers">
        <asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Create Users With Group
</div>
<%-- create user wizard with roles --%>
<div class="cuwWrap boxStyle">

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
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

    <table class="mainTable boxStyle">

        <tr style="background-position: top; background-image: url(images/graygradient.png); background-repeat: repeat-x">
            <th>Login Type</th>
            <th>Group Selection</th>
            <%--         <th >Account Lifespan</th>--%>
        </tr>
        <tr>
            <td>
                <div id="divLogInType" class="cuwWrap-editor boxStyle">
                    <asp:RadioButtonList ID="UserType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="UserType_SelectedIndexChanged">
                        <asp:ListItem Selected="true">Milliman Login</asp:ListItem>
                        <asp:ListItem>External SSO Login</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </td>
            <td>
                <div id="divUserRoleSelector" class="cuwWrap-editor boxStyle" style="width: 392px; overflow-y: scroll;">
                    <%--<div style="width: 214px; height: 200px; overflow: auto; border: 1px solid gray; padding: 10px; background-color: white;">--%>
                    <%--<asp:CheckBoxList ID="Groups" runat="server"> </asp:CheckBoxList>--%>
                    <asp:UpdatePanel ID="updateStatus" runat="server" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <urs:userRoleSelector ID="ctrlUserRoles" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
            <%--<td >
            <div style="width:157px; height:200px;overflow:hidden;border:1px solid gray;padding:10px;background-color:white;">
                 <asp:CheckBox ID="AccountExpires" runat="server" Text="Accounts Expire" AutoPostBack="True" OnCheckedChanged="AccountExpires_CheckedChanged" />
                 <br />
                 <br />
                 Account lifespan is <br /> <asp:DropDownList ID="DateValue" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="DateValue_SelectedIndexChanged"></asp:DropDownList>
                                     <asp:DropDownList ID="DateType" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="DateType_SelectedIndexChanged">
                                         <asp:ListItem Text="Year(s)" Value="year" Selected="True"></asp:ListItem>
                                         <asp:ListItem Text="Month(s)" Value="month" Selected="False"></asp:ListItem>
                                         <asp:ListItem Text="Week(s)" Value="week" Selected="False"></asp:ListItem>
                                         <asp:ListItem Text="Day(s)" Value="day" Selected="False"></asp:ListItem>
                                     </asp:DropDownList>
                 <br />
                <br />
                 Expires on <telerik:RadDatePicker ID="DatePicker" runat="server" Enabled="False"></telerik:RadDatePicker>
            </div>
         </td>--%>
        </tr>
    </table>



    <%--    <telerik:RadGrid runat="server" ID="Issues" AllowPaging="true" AllowSorting="true" OnSortCommand="Issues_SortCommand" OnPageIndexChanged="Issues_PageIndexChanged" OnPageSizeChanged="Issues_PageSizeChanged" OnPreRender="Issues_PreRender"></telerik:RadGrid>          --%>
    <telerik:RadGrid runat="server" ID="RadGrid1" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="5" GridLines="None" OnItemCommand="RadGrid1_ItemCommand" AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True" ClientIDMode="AutoID">
        <MasterTableView EditMode="Batch" CommandItemDisplay="Top" TableLayout="Fixed">
            <CommandItemTemplate>
                <asp:LinkButton ID="Add" runat="server" CommandName="Add" Visible="true"><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Office-Girl-icon.png"/>Add List Entry</asp:LinkButton>&nbsp;&nbsp;
                <%--               <asp:LinkButton ID="AutoCompleted" runat="server" CommandName="Autocomplete" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image ID="Image2" runat="server"  style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/process-icon.png"/>Auto-Complete</asp:LinkButton>&nbsp;&nbsp;--%>
                <asp:LinkButton Width="100px" ID="Validate" runat="server" CommandName="Validate" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server"  style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/process-icon.png"/>Validate</asp:LinkButton>&nbsp;&nbsp;
                <%--               <asp:LinkButton ID="Create" runat="server" CommandName="Create" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>' OnClientClick="return ConfirmAction();"><asp:Image runat="server"  style="border:0px;vertical-align:middle;" alt=""  ImageUrl="~/Images/process-accept-icon.png"/>Create Users</asp:LinkButton>&nbsp;&nbsp;--%>

                <asp:LinkButton ID="Clear" runat="server" CommandName="Clear" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Actions-edit-delete-icon16x16.png"/>Clear List</asp:LinkButton>&nbsp;&nbsp;
            </CommandItemTemplate>
            <Columns>
                <%--             <telerik:GridImageColumn HeaderStyle-Width="20px" DataType="System.String" DataImageUrlFields="ValidationImage"
                            ImageAlign="Middle" ImageHeight="16px" ImageWidth="16px" HeaderText="" UniqueName="ValidationImageStatus">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridImageColumn>--%>

                <telerik:GridTemplateColumn DataField="ValidationImage" UniqueName="ValidationImageStatus" HeaderStyle-Width="20px">
                    <ItemTemplate>
                        <asp:Image ID="ValidationStatusImage" runat="server" ImageUrl='<%#Eval("ValidationImage") %>' ToolTip='<%#Eval("ErrorMsg") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="Account_Name" HeaderText="Account" UniqueName="AccountNameText" HeaderStyle-Width="100%">
                    <ItemTemplate>
                        <asp:TextBox ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="100%"></asp:TextBox>
                    </ItemTemplate>

                    <HeaderStyle Width="100%"></HeaderStyle>
                </telerik:GridTemplateColumn>

                <%--              <telerik:GridTemplateColumn DataField="Email" HeaderText="Email" UniqueName="EmailText" HeaderStyle-Width="130px">
                    <ItemTemplate>
                        <asp:TextBox ID="EmailTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Email") %>' Width="100%"></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="130px"></HeaderStyle>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="Password" HeaderText="Password" UniqueName="PasswordText"   >
                   <ItemTemplate>
                        <asp:TextBox ID="PasswordTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Password") %>' Width="100%" ></asp:TextBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="Confirm_Password" HeaderText="Confirm Password" UniqueName="ConfirmPasswordText" >
                    <ItemTemplate>
                        <asp:TextBox ID="ConfirmPasswordTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Confirm_Password") %>' Width="100%"></asp:TextBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>--%>

                <telerik:GridTemplateColumn DataField="SendWelcomeEmail" HeaderText="Send Welcome" UniqueName="SendWelcome" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="SendWelcomeCheckbox" runat="server" AutoPostBack="false" />
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
    <telerik:RadPanelBar ID="RadPanelBar1" runat="server" Width="100%" CollapseDelay="100" ExpandDelay="100">
        <Items>
            <telerik:RadPanelItem ID="UserPanel" Text="CSV List Entries" Expanded="False" runat="server" ImagePosition="Left" ToolTip="Click to expand and paste new user information here" ExpandedImageUrl="~/Images/User-Group-icon.png" DisabledImageUrl="~/Images/User-Group-icon.png" ImageUrl="~/Images/User-Group-icon.png">
                <ContentTemplate>
                    <asp:Table ID="Table1" runat="server" Height="350px" Width="100%">
                        <asp:TableRow ToolTip="Paste new user information using comma delimited format:<br> Email Address, [True/False] with *1 entry per line">
                            <asp:TableCell>
                                <asp:Label ID="Label1" runat="server" Text="Paste new user information using comma delimited format:<br> Email Address,[True/False] <br> *1 entry per line"></asp:Label>
                            </asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow Width="100%">
                            <asp:TableCell Width="100%">
                                <asp:TextBox runat="server" ID="UserList" TextMode="MultiLine" Rows="20" Width="100%"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Submit_Click" />
                            </asp:TableCell>

                        </asp:TableRow>
                    </asp:Table>
                </ContentTemplate>
            </telerik:RadPanelItem>

        </Items>
    </telerik:RadPanelBar>
    <br />
    <center>  <asp:ImageButton ID="CreateNewUsers" runat="server"  ImageUrl="~/images/CreateUsersButton.png" ToolTip="Add the listed users to the HCIntel system." OnClick="CreateNewUsers_Click" /></center>
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
        //var grid = document.getElementById("RadGrid1");
        //var MasterTable = grid.get_masterTableView();
        //var Rows = MasterTable.get_dataItems();

        //if (Rows.length == 1)
        //    return Confirm("Are you certain you want to create this user?");
        //return Confirm("Are you certain you want to create these " + Rows.length + " users?");

        return window.confirm("Are you certain you want to create these users?");
    }


</script>

<%--<script type="text/javascript">
    window.BlockingRadConfirm = function (text, mozEvent, oWidth, oHeight, callerObj, oTitle) {
        var ev = mozEvent ? mozEvent : window.event;
        ev.cancelBubble = true;
        ev.returnValue = false;
        if (ev.stopPropagation) ev.stopPropagation();
        if (ev.preventDefault) ev.preventDefault();
        var callerObj = ev.srcElement ? ev.srcElement : ev.target;
        if (callerObj) {
            var callBackFn = function (arg) {
                if (arg) {
                    callerObj["onclick"] = "";
                    if (callerObj.tagName == "A") {
                        try {
                            eval(callerObj.href)
                        }
                        catch (e) { }
                    }
                    else if (callerObj.click) {
                        callerObj.click();
                    }
                }
            }
            radconfirm(text, callBackFn, oWidth, oHeight, callerObj, oTitle);
        }
        return false;
    }
</script> --%>  