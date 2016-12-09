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
        margin: 5px 0 1px 10px;
        padding: 2px 42px 5px 5px;
        border: 2px dashed #eee;
        font-weight: 400;
    }

    #divUserRole {
        width: 748px;
    }

    #divResults {
        width: 750px;
    }

    #divUserAddList {
        width: 750px;
        margin: 0 auto;
    }

    .userList {
        margin: -11px -4px 1px 11px;
    }

    .imageButtonClass {
        height: 15px;
    }

    .labelweak {
        margin: 8px 7px -4px;
    }

    #divUserList {
        margin: 4px;
        padding: 4px;
    }

    #divSubmit {
        margin: 4px;
        padding: 4px;
    }

    .centerDiv {
        margin: 0 auto;
        width: 100px;
    }

    .RadGrid_Office2010Silver .rgHeader, .RadGrid_Office2010Silver th.rgResizeCol, .RadGrid_Office2010Silver .rgHeaderWrapper {
        border: none;
    }

    .RadGrid_Office2010Silver .rgRow > td, .RadGrid_Office2010Silver .rgAltRow > td, .RadGrid_Office2010Silver .rgEditRow > td, .RadGrid_Office2010Silver .rgFooter > td {
        border: none;
    }

    .msg {
        display: none;
    }

    .error {
        color: red;
    }
</style>


<%-- gridview banner --%>
<%--<div class="gvBanner">
    <span class="gvBannerUsers">
        <asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> 
    Add Users With Group(s)
</div>--%>

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

<div class="containerWrap center-block outerWrap">
    <div class="page-header engravedHeader">
        <h2>Add Users with Group(s)</h2>
    </div>
    <div class="space"></div>
    <div id="divOuter" class="roundShadowContainer">
        <div id="divLoginType" runat="server" visible="false">
            <div class="left"><span class="engravedHeader">Login Type:</span></div>
            <div class="space"></div>
            <div class="right">
                <%--panel for UserType--%>
                <asp:UpdatePanel ID="updPanelUserType" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="UserType" runat="server" AutoPostBack="True"
                            OnSelectedIndexChanged="UserType_SelectedIndexChanged" RepeatDirection="Horizontal" CssClass="radioButtonlabel">
                            <asp:ListItem Selected="true">Milliman Login</asp:ListItem>
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
                OnItemCommand="RadGrid1_ItemCommand" CssClass="MyRadGrid"
                AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True" ClientIDMode="AutoID"
                ClientSettings-ClientEvents-OnRowDeleting="RowDeleting">
                <MasterTableView EditMode="Batch" CommandItemDisplay="Top" TableLayout="Fixed">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <telerik:GridButtonColumn Text="Add new row" CommandName="Add" ButtonType="ImageButton"
                            UniqueName="Add"
                            ButtonCssClass="imageButtonClass" HeaderTooltip="Add new row"
                            ImageUrl="~/Content/Images/Add-Blue.png" HeaderStyle-Width="25px" Resizable="false">
                            <HeaderStyle Width="25px" />
                            <ItemStyle Width="25px" />
                        </telerik:GridButtonColumn>
                        <telerik:GridTemplateColumn DataField="ValidationImage" UniqueName="ValidationImageStatus" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:Image ID="ValidationStatusImage" runat="server" ImageUrl='<%#Eval("ValidationImage") %>' ToolTip='<%#Eval("ErrorMsg") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="Account_Name" HeaderText="Account Name" UniqueName="AccountNameText" HeaderStyle-Width="100%">
                            <ItemTemplate>
                                <label id="lblAccountNameTextBox" for="AccountNameTextBox" class="labelweak required"></label>
                                <asp:TextBox ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="90%" Height="27px" CssClass="accountNameTextBox required standardTextBox"></asp:TextBox>
                                <%--<asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="AccountNameTextBox" ErrorMessage="Account name is required" ForeColor="Red" />--%>
                                <span class="msg error">Invalid email address</span>
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
                        <telerik:GridButtonColumn Text="Delete entry" CommandName="Delete" ButtonType="ImageButton"
                            ImageUrl="~/Content/Images/Delete.png" ButtonCssClass="imageButtonClass" HeaderTooltip="Delete entry"
                            ConfirmText="Delete this user from the list?" ConfirmDialogType="Classic">
                            <HeaderStyle Width="25px" />
                            <ItemStyle Width="25px" />
                        </telerik:GridButtonColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <div class="space"></div>

        <div id="divUserAddList" class="roundShadowContainer">
            <telerik:RadPanelBar ID="RadPanelBar1" runat="server" Width="100%" CollapseDelay="100" ExpandDelay="100" ExpandMode="MultipleExpandedItems"
                AllowCollapseAllItems="True" OnClientItemClicked="OnClientItemClicked">
                <Items>
                    <telerik:RadPanelItem ID="UserPanel" Value="UserPanel" Text="Add New by CSV List" Expanded="False" runat="server" ImagePosition="Left" ToolTip="Click to expand and paste new user information here" ExpandedImageUrl="~/Images/User-Group-icon.png" DisabledImageUrl="~/Images/User-Group-icon.png" ImageUrl="~/Images/User-Group-icon.png">
                        <ContentTemplate>
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
                            <div class="space"></div>
                            <div id="divUserList">
                                <asp:UpdatePanel ID="updPanelUserList" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TextBox runat="server" ID="UserList" TextMode="MultiLine" Rows="20" Width="98%" Height="150px"></asp:TextBox>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="space"></div>
                            <div class="space"></div>
                            <div id="divSubmit" class="center-block centerDiv" style="margin: 0 auto; width: 100px;">
                                <asp:Button ID="Button2" runat="server" Text="Submit List" OnClick="Submit_Click" CssClass="btn btn-primary" />
                            </div>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
        </div>
        <div class="space"></div>
        <div id="divCreate" class="center-block">
            <asp:Button ID="CreateNewUsers" runat="server" CommandName="CreateNewUsers" Text="Save" CssClass="btn btn-primary"
                OnClick="CreateNewUsers_Click" />
            <asp:Button ID="Reset" runat="server" CommandName="Reset" Text="Reset" CssClass="btn btn-primary"
                OnClick="Reset_Click" />
        </div>
        <div class="space"></div>
    </div>
</div>


<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />

<script src="../../Content/Script/jquery.v1.9.1.js"></script>
<script src="../../Content/Script/jquery.min.v2.1.1.js"></script>

<telerik:RadScriptBlock ID="radscript3" runat="server">
    <script type="text/javascript">

        //***************** display the importatn hint next to the image ******************************//
        var moveTop = "125px";
        $('.divImportant').hover(function (e) {
            $('#divImportantHint').show();
        }, function () {
            $('#divImportantHint').hide();
        });

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
                        //ValidationStatusImage
                        if (Rows[0]._element.cells[1].childNodes[1].id == "ctl00_ContentPlaceHolder1_create1_RadGrid1_ctl00_ctl04_ValidationStatusImage") {
                            var eleImage = Rows[0]._element.cells[1].childNodes[1];
                            eleImage.style.visiblity = 'hidden';
                            eleImage.style.display = 'none';
                        }
                        //AccountNameText
                        if (Rows[0]._element.cells[2].childNodes[3].id == "ctl00_ContentPlaceHolder1_create1_RadGrid1_ctl00_ctl04_AccountNameTextBox") {
                            Rows[0]._element.cells[2].childNodes[3].value = "";

                        }
                        //SendWelcome
                        if (Rows[0]._element.cells[3].childNodes[1].id == "ctl00_ContentPlaceHolder1_create1_RadGrid1_ctl00_ctl04_SendWelcomeCheckbox") {
                            Rows[0]._element.cells[3].childNodes[1].checked = false;
                        }
                        eventArgs.set_cancel(true);
                    }
                }
            }
        }

        function OnClientItemClicked(sender, args) {
            var panelbar = $find("<%= RadPanelBar1.ClientID %>");
            var item = panelbar.findItemByValue("UserPanel");
            var UserList = '<%=UserList.ClientID %>';

            var uList = "[CSV Format]\n";
            uList += "\n\n[Excel Format]\n";
            $('#' + UserList).val(uList);

            var updPanelUserList = '<%=updPanelUserList.ClientID%>';//ctl00_ContentPlaceHolder1_create1_RadPanelBar1_i0_updPanelUserLis
            __doPostBack(updPanelUserList, '');
        }

        //***************** All Special Characters ******************************//
        //if there are values for allwoed character then display it
        var AllSpecialChars = "<%= ConfigurationManager.AppSettings["AllSpecialChars"].ToString() %>";
        //if there are values for allwoed character then display it
        var AllowedCharsInLocalStringUserName = "<%= ConfigurationManager.AppSettings["AllowedCharsLocalPartUserName"].ToString() %>";
        //if there are values for allwoed character then display it
        var AllowedCharsServerStringUserName = "<%= ConfigurationManager.AppSettings["AllowedCharsServerPartUserName"].ToString() %>";


        //this function executes when edit individual cell
        $(".accountNameTextBox").blur(function () {
            var $controlValue = $(this).val();
            //create regex
            if ($controlValue.length == 0) {
                enableLinkButton();
                $('.msg').hide();
                $('.success').show();
            }
            else {
                CheckUserNameInput($controlValue);
            }
        });

        $(".accountNameTextBox").keyup(function () {
            debugger;
            var $controlValue = $(this).val();
            //create regex
            if ($controlValue.length==0)
            {
                enableLinkButton();
                $('.msg').hide();
                $('.success').show();
            }
            else
            {
                CheckUserNameInput($controlValue);
            }
        });

        //funciton to check the avalibility of special allowed chars, string and numbers
        function verifyUserNameInput(elementFocusEvent) {
            var elementValue = elementFocusEvent.value;

            if (elementValue.length > 0)
                CheckUserNameInput(elementValue);

            return true;
        }
        
        function CheckUserNameInput(elementValue) {
            var badData = false;
            
            //get the occurance of @ - if there is no @ then ignore
            if (elementValue.indexOf('@') > -1) {
                //do nothing, this condition is true
            }
            else {
                disableLinkButton();
                $('.msg').hide();
                $('.error').show();
                return false;
            }

            //check how many '@' exist
            var countofAtSign = (elementValue.match(/@/g) || []).length;
            if (countofAtSign > 1) {
                $('.msg').hide();
                $('.error').show();
                disableLinkButton();
                return false;
            }

            //first part of email before @ 
            var localString = elementValue.split('@')[0];

            //there is empty string before @
            if (localString.length === 0) {
                $('.msg').hide();
                $('.error').show();
                disableLinkButton();
                return false;
            }

            //second part of email
            var serverString = elementValue.split('@')[1];

            //serverString must have few chars
            if (serverString.length === 0) {
                $('.msg').hide();
                $('.error').show();
                disableLinkButton();
                return false;
            }

            //chcek for number
            var bHasNumbers = checkInputForNumbers(elementValue);
            //check for alphabets
            var bHasAlphabets = checkInputForAlphabets(elementValue);
            //check to see at least one character exist or one number is in string. either 1 number or 1 alphabet must exist
            if (!bHasNumbers && !bHasAlphabets) {
                $('.msg').hide();
                $('.error').show();
                disableLinkButton();
                return false;
            }

            //check to see if there are any specail chars in string then check if those special chars are allowed
            if (elementValue.match(new RegExp(AllSpecialChars, "gi"))) //  new RegExp(AllSpecialChars)=== /[~!@#$%^&*;?+_.`<,>':/[\]|{}()=-]/
            {
                var regex = /[~!@#$%^&*;?+_.`<,>':/[\]|{}()=-]/gi;
                //****************** Local String Check **********************************/
                var isSplChar = regex.test(localString);
                var badFound = [];
                if (isSplChar) {
                    var allPresentCharactersInName = localString.match(regex);
                    badFound = checkAllowedChars(allPresentCharactersInName, AllowedCharsInLocalStringUserName);
                    //find all special characters that matches with the web.config file key in local part of the string
                    if (badFound.length > 0) {
                        badData = true;
                    }
                }
                //****************** Server String Check **********************************/
                isSplChar = regex.test(serverString);
                if (isSplChar) {
                    allPresentCharactersInName = serverString.match(regex);
                    badFound = checkAllowedChars(allPresentCharactersInName, AllowedCharsServerStringUserName);
                    //find all special characters that matches with the web.config file key in local part of the string
                    if (badFound.length > 0) {
                        badData = true;
                    }

                }


                if (badData) {
                    $('.msg').hide();
                    $('.error').show();
                    disableLinkButton();
                    return false;
                }
                else {
                    enableLinkButton();
                    $('.msg').hide();
                    $('.success').show();
                }

                return true;
            }

        }

        var checkAllowedChars = function (charsInString, specialChars) {
            var matchfound = [];
            var nomatchfound = [];
            for (i = 0; i < charsInString.length; i++) {
                if (typeof charsInString[i] != 'undefined') {
                    if (specialChars.indexOf(charsInString[i]) !== -1) {
                        matchfound.push(charsInString[i]);
                    }
                    else {
                        nomatchfound.push(charsInString[i]);
                        return nomatchfound;
                    }
                }
            }

            return nomatchfound;
        }

        function checkInputForNumbers(string) {
            //the regular expression checks for any number in string, so if there is at least one number then it will be true
            return /\d/.test(string);
        }

        function checkInputForAlphabets(string) {
            //the regular expression checks for any alphabet in string, lower or upper,so if there is at least one number then it will be true
            return /[a-zA-Z]/.test(string);
        }

        function enableLinkButton() {
            document.getElementById('<%= CreateNewUsers.ClientID %>').disabled = false;
        }

        function disableLinkButton() {
            document.getElementById('<%= CreateNewUsers.ClientID %>').disabled = true;
    }

    </script>
</telerik:RadScriptBlock>

