<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="MillimanClientUserAdmin.AddUser" Async="true" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link href="Content/Style/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="Content/Style/MillframeStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .LockOff {
            display: none;
            visibility: hidden;
        }

        .LockOn {
            display: block;
            visibility: visible;
            position: absolute;
            z-index: 999;
            top: 0px;
            left: 0px;
            width: 110%;
            height: 110%;
            background-color: #ccc;
            text-align: center;
            padding-top: 20%;
            filter: alpha(opacity=95);
            opacity: 0.95;
            font: 500 12px italic;
            font-family: 'Segoe UI';
            overflow: hidden;
        }

        .roundShadowContainer {
            margin-top: 10px;
            margin-left: 0;
            margin-bottom: 0;
            margin-right: 0;
        }

        .containerWrap {
            padding: 4px;
        }

        .engravedHeader {
            font-size: 14px;
        }

        .imageButtonClass {
            height: 15px;
        }

        #divSubmit {
            margin: 4px;
            padding: 4px;
        }
        /*//remove expandable image*/
        .rpExpandHandle {
            background-image: none !important;
        }
        /*//remove thead gird lines*/
        .RadGrid_Office2010Silver .rgHeader, .RadGrid_Office2010Silver th.rgResizeCol, .RadGrid_Office2010Silver .rgHeaderWrapper {
            border: none;
        }

        .RadGrid_Office2010Silver .rgRow > td, .RadGrid_Office2010Silver .rgAltRow > td, .RadGrid_Office2010Silver .rgEditRow > td, .RadGrid_Office2010Silver .rgFooter > td {
            border: none;
        }

        .top-buffer {
            margin-top: 40px;
        }

        .windowScroll {
            overflow: auto;
            overflow-y: scroll;
            overflow-x: hidden;
        }
    </style>
</head>
<body onresize="FullSize('MainTable');" class="windowScroll">

    <form id="form1" runat="server">

        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <script type="text/javascript">
            //Put your JavaScript code here.
        </script>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server"></telerik:RadAjaxManager>

        <div class="containerWrap outerWrap">

            <div id="MainTable">

                <div id="divRadPableBar" class="roundShadowContainer">
                    <div id="divTotalRecs">
                        <span>License Info:</span> <b>
                            <asp:Label ID="License" runat="server" Text=""></asp:Label></b>
                    </div>
                    <div class="row top-buffer">&nbsp;</div>
                    <telerik:RadPanelBar runat="server" ID="RadPanelBar1" Height="55%" Width="100%" ExpandMode="FullExpandedItem">
                        <Items>
                            <telerik:RadPanelItem runat="server" Expanded="True" Text="Data Restriction Selections" Value="panItemAccessTree">
                                <Items>
                                    <telerik:RadPanelItem runat="server" Value="TreeHolder">
                                        <ItemTemplate>
                                            <telerik:RadTreeView runat="server" ID="AccessTree" CheckBoxes="True" CheckChildNodes="True"
                                                MultipleSelect="False" TriStateCheckBoxes="True" ClientIDMode="Static">
                                            </telerik:RadTreeView>
                                        </ItemTemplate>
                                    </telerik:RadPanelItem>
                                </Items>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem runat="server" Text="User Download Selections" Expanded="false" Visible="False" Value="panItemDownloadTree">
                                <Items>
                                    <telerik:RadPanelItem runat="server" Value="DownloadHolder">
                                        <ItemTemplate>
                                            <telerik:RadTreeView runat="server" ID="DownloadTree" CheckBoxes="True" CheckChildNodes="True"
                                                MultipleSelect="False" TriStateCheckBoxes="True">
                                            </telerik:RadTreeView>
                                        </ItemTemplate>
                                    </telerik:RadPanelItem>
                                </Items>
                            </telerik:RadPanelItem>
                        </Items>
                    </telerik:RadPanelBar>
                </div>
                <div class="space"></div>
                <div id="divResults" class="roundShadowContainer">
                    <telerik:RadGrid runat="server" ID="RadGrid1" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="5" GridLines="None"
                        OnItemCommand="RadGrid1_ItemCommand" AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True"
                        ClientIDMode="AutoID" ClientSettings-ClientEvents-OnRowDeleting="RowDeleting">
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
                                        &nbsp;
                                        <asp:TextBox ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="90%" Height="27px" CssClass="standardTextBox"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle Width="100%"></HeaderStyle>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="SendWelcomeEmail" HeaderText="Send Welcome" UniqueName="SendWelcome" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="SendWelcomeCheckbox" runat="server" AutoPostBack="false" ViewStateMode="Enabled" Checked='<%#Eval("SendWelcomeEmail") %>' />
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
                                <%--<telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" ConfirmText="Delete this user from the list?" ConfirmDialogType="Classic">
                                    <HeaderStyle Width="32px" />
                                </telerik:GridButtonColumn>--%>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
                <div class="space"></div>
                <div class="space"></div>
                <div id="divCreate">
                    <asp:Button ID="CreateUsers" runat="server" Text="Save" OnClientClick="return StartProcessing();" OnClick="CreateUsers_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="Clear" runat="server" CommandName="Clear" Text="Reset" CssClass="btn btn-primary" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'
                        OnClick="Reset_Click" />
                </div>
                <div class="space"></div>
                <div class="alert alert-warning infoBox text-justify">
                    <p>
                        Multiple accounts can be created by entering comma, semi-colon, space or newline delimited emails.  You can also assign a password to a user by enclosing it in square brackets immediately after the user name. (eg. someuser@email.com[UserP@ssword1], anotheruser@email.com)
                    </p>
                </div>
            </div>

            <div id="LockPane" class="LockOff" style="overflow: hidden">
                <div id="progressBackgroundFilter"></div>
                <div id="progressBarWindow" class="progressBarWindow center-block" style="top: 26px;">
                    <asp:Image ID="loaderImage" runat="server" ImageUrl="~/Content/Images/ajax-loader-bar.gif" Width="248px" Height="30px" />
                    <div class="space"></div>
                    <span class="engravedHeader">Please Wait....</span>
                    <div class="space"></div>
                </div>
            </div>
        </div>
        <script>
            if (window.clipboardData) {
                $('#RadGrid1_ctl00_ctl04_AccountNameTextBox').bind('paste', function (e) {
                    var clipped = window.clipboardData.getData('Text');
                    clipped = clipped.replace(/(\r\n|\n|\r)/gm, " "); //replace newlines with spaces
                    $(this).val(clipped);
                    return false; //cancel the pasting event
                });
            }
        </script>
        <script type="text/javascript">

            //**************************************************************************************************************************************//        
            //this code block check for the Rad Active winodw on page and if there is none, then refresh page after the time defined in web.config
            var refreshPageInterval = "<%= ConfigurationManager.AppSettings["ApplicationRefreshTime"].ToString()%>";
            //convert to milliseconds since the set interval consume milliseconds
            var totalRefreshInterval = refreshPageInterval * 60 * 1000;
            //set interval to refresh page auto
            setInterval(RefreshPage, totalRefreshInterval);
            function RefreshPage() {
                    //Getting rad window manager
                    var rad_manager = GetRadWindowManager();
                    //Call GetActiveWindow to get the active window
                    var rad_active_window = rad_manager.getActiveWindow();
                    if (rad_active_window == null) {
                        window.location.reload();
                    }
            }
           //-------------------------------------------------------------------------------------------------------------------------------------//

            function StartProcessing() {

                //Issue1569 - we no longer require a selection in the tree
                //var AccessTree = $find("AccessTree");
                //if (AccessTree) {
                //    //we do not allow a user to be added if access rights are not selected
                //    if (AccessTree.get_checkedNodes().length == 0) {
                //        alert("Selecting the user access rights from the 'Data Restrictions Selections' tree is required to add user accounts.");
                //        return false;
                //    }
                //}
                LockScreen("");
                return true;
            }

            function RefreshIFrame() {
                var MyIFrame = document.getElementById("imgbox");
                if (MyIFrame) {
                    MyIFrame.src = MyIFrame.src;
                    MyIFrame.scrolling = "no";
                }
                var lock = document.getElementById('LockPane');
                if (lock.className == 'LockOn')
                    setTimeout(function () { RefreshIFrame(); }, 1000);
            }

            function LockScreen(str) {
                var lock = document.getElementById('LockPane');
                if (lock)
                    lock.className = 'LockOn';

                //start refreshing iframe so it animates
                RefreshIFrame();
                //lock.innerHTML = str;
            }

            function getRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            // Reload parent page
            function CloseDialog(Msg) {
                var ThisDialog = getRadWindow();
                var Parent = getRadWindow().BrowserWindow;
                // Parent.alert("Profile/Password information has been saved.");
                if (Parent.CloseAndRefresh)
                    Parent.CloseAndRefresh(Msg);
                //Parent.radalert(Msg, 350, 150, "Users Added");
                //window.setTimeout(function () { Parent.location.reload(); }, 5000);
                ThisDialog.close();
            }

            function ErrorDialog() {
                alert('There was an issue saving your information.  An email has been automatically sent to the system administrator on this issue.');
            }

            function FullSize(element) {
                var height = 0;
                var width = 0;
                var body = window.document.body;
                if (window.innerHeight) {
                    height = window.innerHeight;
                    width = window.innerWidth;
                } else if (body.parentElement.clientHeight) {
                    height = body.parentElement.clientHeight;
                    width = body.parentElement.clientWidth;
                } else if (body && body.clientHeight) {
                    height = body.clientHeight;
                    width = body.clientWidth;
                }
                //margines
                height = height - 15;
                width = width - 15;

                document.getElementById(element).style.height = height + "px";
                document.getElementById(element).style.width = width + "px";
                document.getElementById(element).style.visibility = "visible";
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
                            if (Rows[0]._element.cells[1].childNodes[1].id == "RadGrid1_ctl00_ctl04_ValidationStatusImage") {
                                var eleImage = Rows[0]._element.cells[1].childNodes[1];
                                eleImage.style.visiblity = 'hidden';
                                eleImage.style.display = 'none';
                            }
                            //AccountNameText
                            if (Rows[0]._element.cells[2].childNodes[3].id == "RadGrid1_ctl00_ctl04_AccountNameTextBox") {
                                Rows[0]._element.cells[2].childNodes[3].value = "";

                            }
                            //SendWelcome
                            if (Rows[0]._element.cells[3].childNodes[1].id == "RadGrid1_ctl00_ctl04_SendWelcomeCheckbox") {
                                Rows[0]._element.cells[3].childNodes[1].checked = false;
                            }
                            eventArgs.set_cancel(true);
                        }
                    }
                }
            }
        </script>
    </form>
    <script type="text/javascript">
        FullSize("MainTable");
    </script>
</body>
</html>
