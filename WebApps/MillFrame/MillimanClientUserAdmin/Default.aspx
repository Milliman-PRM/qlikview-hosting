<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Default" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Milliman - PRM Client Adminstration</title>
    <meta http-equiv="refresh" content="1200" />
    <%--refresh page in 20 mins, session timeout is 15 mins--%>
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server" />
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

        /*restyle the radwindow alert box to make it not look bad*/
        .RadWindow .rwWindowContent .radalert {
            background-image: none !important; /* removes the excalamtion mark icon */
            padding-left: 0px !important;
        }

        .RadWindow .rwDialogText {
            margin-left: 10px !important;
        }

        .RadWindow .rwPopupButton {
            margin-left: 100px !important;
        }
        /*//remove expandable image*/
        .rpExpandHandle {
            background-image: none !important;
        }
    </style>
</head>
<body onresize="FullSize('MainTable');" style="background-image: url(Imates/watermark.png); overflow: hidden">
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <script type="text/javascript">
	   
        
        </script>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>

        
        <table id="MainTable" name="MainTable" style="position: absolute; top: 10px; left: 10px; width: 100px; height: 100px; visibility: hidden">
            <tr style="height: 25px">
                <td>
                    <asp:HiddenField runat="server" ID="hfEmailDelimiter" ></asp:HiddenField>
                </td>
                <td>
                    <center><asp:Label runat="server" ID="LicenseMessage" Font-Names="segoe ui" Font-Size="12px"></asp:Label></center>
                </td>
                <td>
                    <center><asp:Label runat="server" ID="UserMessage" Font-Names="segoe ui" Font-Size="12px"></asp:Label></center>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="">
                    <div id="SplitterDiv" style="width: 100%; height: 100%;">
                        <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%">
                            <telerik:RadPane ID="RadPane1" runat="server" MinWidth="250" Scrolling="None">
                                <telerik:RadGrid ID="UserGrid" AllowMultiRowSelection="false" runat="server" AllowAutomaticDeletes="False" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="5" GridLines="None" Height="100%" MasterTableView-AllowAutomaticDeletes="False" OnItemCommand="RadGrid1_ItemCommand" Skin="Silk" ViewStateMode="Enabled" Width="100%" OnItemDataBound="UserGrid_ItemDataBound" OnPreRender="UserGrid_PreRender" OnDataBound="UserGrid_DataBound">
                                    <ClientSettings>
                                        <Selecting AllowRowSelect="false" />
                                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                        <ClientEvents OnRowClick="RowClick" />
                                    </ClientSettings>
                                    <MasterTableView AllowAutomaticDeletes="True" CommandItemDisplay="Top" CommandItemStyle-BorderColor="Black" CommandItemStyle-BorderStyle="Solid" CommandItemStyle-BorderWidth="1px" CommandItemStyle-Height="25px" CommandItemStyle-VerticalAlign="Middle" CommandItemStyle-Wrap="False" EditMode="Batch" NoMasterRecordsText="No users have been added to the PRM system for display." TableLayout="Fixed">
                                        <CommandItemStyle BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="25px" VerticalAlign="Middle" Wrap="False" />
                                        <CommandItemTemplate>
                                            <div style="width: 400px; white-space: nowrap; min-width: 400px">
                                                <asp:LinkButton ID="Add" runat="server" CausesValidation="False" CommandName="Add" OnClientClick="OpenAddUser(); return false;" ToolTip="Add a new user to the PRM system." Visible="true">
                                                    <asp:Image ID="Image1" runat="server" alt="" ImageUrl="~/Images/AddUser.png" Style="border: 0px; vertical-align: middle;" />
                                                    Add User
                                                </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="Email" runat="server" CommandName="Email" ToolTip="Email selected user." Visible="<%# UserGrid.EditIndexes.Count == 0 %>" OnClientClick="SendEmail(); return false;">
                                                        <asp:Image ID="Image5" runat="server" alt="" ImageUrl="~/Images/Email.png" Style="border: 0px; vertical-align: middle;" />
                                                        Email
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="EmailAll" runat="server" CommandName="EmailAllUsers" ToolTip="Email all users." Visible="<%#UserGrid.EditIndexes.Count == 0 %>" OnClientClick="SendEmailAll(); return false;">
                                                        <asp:Image ID="Image9" runat="server" alt="" ImageUrl="~/Images/EmailAll.png" Style="border: 0px; vertical-align: middle;" />
                                                        Email All
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="Report" runat="server" CommandName="Report" ToolTip="Generate a report showing access rights and user logins." Visible="<%# UserGrid.EditIndexes.Count == 0 %>" OnClientClick="Navigate(); return false;">
                                                        <asp:Image ID="Image10" runat="server" alt="" ImageUrl="~/Images/Report.png" Style="border: 0px; vertical-align: middle;" />
                                                        Report
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="Reset" runat="server" CommandName="Reset" ToolTip="Reset selected users password." Visible="<%# UserGrid.EditIndexes.Count == 0 %>" OnClientClick="return ResetPassword();">
                                                        <asp:Image ID="Image6" runat="server" alt="" ImageUrl="~/Images/Reset.png" Style="border: 0px; vertical-align: middle;" />
                                                        Reset Password
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="Suspend" runat="server" CommandName="Suspend" ToolTip="Suspend( or Un-suspend) the selected user." Visible="<%# UserGrid.EditIndexes.Count == 0 %>" OnClientClick="return SuspendUser();">
                                                        <asp:Image ID="Image8" runat="server" alt="" ImageUrl="~/Images/Suspend.png" Style="border: 0px; vertical-align: middle;" />
                                                        (Un)Suspend
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                                    <asp:LinkButton ID="Delete" runat="server" CommandName="Delete" ToolTip="Delete the selected user." Visible="<%#UserGrid.EditIndexes.Count == 0 %>" OnClientClick="return DeleteUser();">
                                                        <asp:Image ID="Image7" runat="server" alt="" ImageUrl="~/Images/Delete.png" Style="border: 0px; vertical-align: middle;" />
                                                        Delete
                                                    </asp:LinkButton>
                                                &#160;&#160;
                                            </div>
                                        </CommandItemTemplate>
                                        <Columns>
                                            <telerik:GridTemplateColumn DataField="SuperUser" HeaderStyle-Width="25px" HeaderText="" ReadOnly="True" UniqueName="SuperUserText">
                                                <ItemTemplate>
                                                    <asp:Image ID="SuperUserImage" runat="server" AutoPostBack="false" ToolTip='Global Administrator' Width="16px" Height="16px" ImageUrl="~/Images/admin.png" Visible='<%#Eval("IsAdmin") %>'></asp:Image>
                                                </ItemTemplate>
                                                <HeaderStyle Width="25px"></HeaderStyle>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridButtonColumn HeaderStyle-Width="35px" ButtonType="ImageButton" ImageUrl="~/Images/reportlaunch.png" CommandName="LaunchQVW" UniqueName="LaunchQVW"></telerik:GridButtonColumn>
                                            <telerik:GridTemplateColumn DataField="Account_Name" HeaderStyle-Width="100%" HeaderText="Email Address" ReadOnly="True" UniqueName="AccountNameText">
                                                <ItemTemplate>
                                                    <asp:Label ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("AccountName") %>' ToolTip='<%#Eval("Tooltip") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100%" HorizontalAlign="Center"></HeaderStyle>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="Notes" HeaderStyle-Width="250px" HeaderText="Notes" UniqueName="NotesText">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="NotesTextBox" runat="server" OnTextChanged="NotesTextBox_TextChanged" AutoPostBack="true" Text='<%#Eval("Notes") %>' Width="100%" TextMode="MultiLine" Rows="3" BorderStyle="Solid" BackColor="Transparent" BorderColor="#E1E1E1" BorderWidth="1px"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100%" HorizontalAlign="Center"></HeaderStyle>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="Status" HeaderStyle-Width="100px" HeaderText="Account Status" UniqueName="StatusText">
                                                <ItemTemplate>
                                                    <asp:Label ID="StatusTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Status") %>' ToolTip='<%#Eval("Tooltip") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100%" HorizontalAlign="Center"></HeaderStyle>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <HeaderStyle Height="20px" VerticalAlign="Middle" />
                                </telerik:RadGrid>
                            </telerik:RadPane>
                            <telerik:RadSplitBar ID="RadSplitbar1" runat="server">
                            </telerik:RadSplitBar>

                            <telerik:RadPane ID="RadPane2" runat="server" MinWidth="400">
                                <asp:Table runat="server" ID="AccessTable" Style="width: 100%; height: 100%; background-color: #E8EAEC" Font-Names="segoe ui" Font-Size="12px">
                                    <asp:TableRow Style="height: 20px">
                                        <asp:TableCell>
                                            <asp:Table runat="server" Width="100%">
                                                <asp:TableRow>
                                                    <asp:TableCell Style="text-align: left; font: normal 12px Segoe UI">
                                                        <asp:CheckBox ID="AutomaticInclusion" runat="server" Text="Automatic Inclusion" AutoPostBack="true" OnCheckedChanged="AutomaticInclusion_CheckedChanged" ToolTip="New items will be automatically selected if all items at same level were checked in previous version." />
                                                    </asp:TableCell>
                                                    <asp:TableCell Style="text-align: right; font: normal 12px Segoe UI">
                                                        <asp:CheckBox ID="ShowCheckedOnly" runat="server" Text="Only show checked items." AutoPostBack="true" OnCheckedChanged="ShowCheckedOnly_CheckedChanged" ToolTip="Only show the checkbox items that are currently 'checked'." />
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <telerik:RadPanelBar ID="RadPanelBar1" runat="server" BackColor="White" ExpandMode="FullExpandedItem" Height="100%" Width="100%">
                                                <Items>
                                                    <telerik:RadPanelItem Expanded="True" Text="Data Restriction Selections">
                                                        <Items>
                                                            <telerik:RadPanelItem Value="TreeHolder">
                                                                <ItemTemplate>
                                                                    <telerik:RadTreeView ID="AccessTree" runat="server" CheckBoxes="True" CheckChildNodes="True" MultipleSelect="False" TriStateCheckBoxes="True" OnClientNodeChecked="NodeWasChecked" ClientIDMode="Static"></telerik:RadTreeView>
                                                                </ItemTemplate>

                                                            </telerik:RadPanelItem>
                                                        </Items>

                                                    </telerik:RadPanelItem>
                                                    <telerik:RadPanelItem Expanded="false" Text="User Download Selections" Visible="False">
                                                        <Items>
                                                            <telerik:RadPanelItem Value="DownloadHolder">
                                                                <ItemTemplate>
                                                                    <telerik:RadTreeView ID="DownloadTree" runat="server" CheckBoxes="True" CheckChildNodes="True" MultipleSelect="True" TriStateCheckBoxes="True" OnClientNodeChecked="NodeWasChecked"></telerik:RadTreeView>
                                                                </ItemTemplate>

                                                            </telerik:RadPanelItem>
                                                        </Items>
                                                    </telerik:RadPanelItem>
                                                </Items>

                                            </telerik:RadPanelBar>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow Style="height: 25px; text-align: center">
                                        <asp:TableCell>
                                            <asp:Button runat="server" ID="ApplyChangesButton" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Text="Apply Changes" Width="200px" ClientIDMode="Static" OnClick="ApplyChangesButton_Click" OnClientClick="return ApplyClient();" Font-Names="segoe ui" Font-Size="12px" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </telerik:RadPane>
                        </telerik:RadSplitter>
                    </div>
                </td>
            </tr>
            <tr style="height: 25px; text-align: center">
                <td colspan="2">
                    <img src="Images/PoweredBy.png" /></td>
            </tr>
        </table>

        <telerik:RadWindowManager EnableShadow="true" Behaviors="Resize, Close, Maximize, Move, Reload" ID="RadWindowManager" DestroyOnClose="true" Opacity="100" runat="server" Width="450" Height="400" VisibleStatusbar="False" Style="z-index: 20000;">
            <Windows>
                <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True" Behaviors="Resize, Close, Move, Reload, Maximize" Modal="True" Height="800px" Width="600px" InitialBehaviors="Close" Title="Add User(s)" VisibleStatusbar="False" VisibleTitlebar="False" />
            </Windows>
        </telerik:RadWindowManager>

        <div id="LockPane" class="LockOff">
            <iframe frameborder="0" style="border: none; overflow: hidden;" width="50" height="50" src="Images/frameanimation.aspx" name="imgbox" id="imgbox" seamless="seamless"></iframe>
            <br />
            <br />
            Account modifications in progress.....
        </div>

    </form>
    <script type="text/javascript">
                
        //var EmailDelimiter = document.getElementById("hfEmailDelimiter").value;
        //alert(EmailDelimiter);

        function CloseAndRefresh(Msg) {
            alert(Msg);
            window.location.reload();
        }

        function StartProcessing() {
            var AccessTree = $find("AccessTree");
            if (AccessTree) {
                //we do not allow a user to be added if access rights are not selected
                if (AccessTree.get_checkedNodes().length == 0) {
                    alert("Selecting the user access rights from the 'Data Restriction Selections' tree is required to modify user accounts.");
                    return false;
                }
            }
            LockScreen("");
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

        function NodeWasChecked(sender, args) {
            var ApplyMe = document.getElementById("ApplyChangesButton");
            if (ApplyMe) {
                ApplyMe.disabled = false;
                ApplyMe.onclick = StartProcessing; //have to add handler, since we created button in disabled state
            }
        }

        //Find the emails in the spanned text
        function ParseEmail(innerHTML) {
            return innerHTML.match(/([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9._-]+)/gi);

        }

        function SendEmail() {
            var grid = $find("<%= UserGrid.ClientID %>");
            var Selected = grid.MasterTableView.get_selectedItems();
            if (Selected.length == 0)
                return false;  //nothing selected to do
            var AllEmails = "";
            for (var i = 0; i < Selected.length; i++) {
                var row = Selected[i];

                var cellID = grid.MasterTableView.getCellByColumnUniqueName(row, "AccountNameText");
                
                //window.location.href = "mailto:" + cellID.innerText;
                if (AllEmails != "")
                    if (document.getElementById("hfEmailDelimiter").value != "") {
                        AllEmails += document.getElementById("hfEmailDelimiter").value;
                    }
                    else {
                        alert("System can not send an email because there is no email delimiter set for group.")
                        return;
                    }
                else
                    AllEmails = ParseEmail(cellID.innerHTML);
            }

            window.location.href = "mailto:" + AllEmails;
        }

        function SendEmailAll() {
            var grid = $find("<%= UserGrid.ClientID %>");
            var Selected = grid.MasterTableView.get_dataItems();
            if (Selected.length == 0)
                return false;  //no items in grid
            var AllEmails = "";
            for (var i = 0; i < Selected.length; i++) {
                var row = Selected[i];
                var cellID = grid.MasterTableView.getCellByColumnUniqueName(row, "AccountNameText");

                //alert(cellID.innerText);
                if (AllEmails != "")
                    if (document.getElementById("hfEmailDelimiter").value!= "") {
                        AllEmails += document.getElementById("hfEmailDelimiter").value;
                    }
                    else {
                        alert("System can not send an email because there is no email delimiter set for group.")
                        return;
                    }
                AllEmails += ParseEmail(cellID.innerHTML);
            }
            window.location.href = "mailto:" + AllEmails
        }

        function Navigate() {
            javascript: window.open("Report.aspx");
        }

        function ResetPassword() {
            var grid = $find("<%= UserGrid.ClientID %>");
            var Selected = grid.MasterTableView.get_selectedItems();
            if (Selected.length == 0)
                return false; //nothing selected to do
            var AllEmails = "";
            for (var i = 0; i < Selected.length; i++) {
                var row = Selected[i];
                var cellID = grid.MasterTableView.getCellByColumnUniqueName(row, "AccountNameText");
                //alert(cellID.innerText);
                if (AllEmails != "")
                    AllEmails += "\n";
                AllEmails += ParseEmail(cellID.innerHTML);
            }

            AllEmails = "Are you sure you want to reset the passwords for:\n\n" + AllEmails;
            return confirm(AllEmails);
        }
        function SuspendUser() {
            var grid = $find("<%= UserGrid.ClientID %>");
            var Selected = grid.MasterTableView.get_selectedItems();
            if (Selected.length == 0)
                return false; //nothing selected to do
            var SuspendedEmails = "";
            var ActiveEmails = "";
            var IndentString = "     ";
            for (var i = 0; i < Selected.length; i++) {
                var row = Selected[i];
                var cellID = grid.MasterTableView.getCellByColumnUniqueName(row, "AccountNameText");
                var statusCellID = grid.MasterTableView.getCellByColumnUniqueName(row, "StatusText");
                if (statusCellID.innerText == "Suspended") {
                    if (SuspendedEmails != "")
                        SuspendedEmails += "\n";
                    SuspendedEmails += IndentString + ParseEmail(cellID.innerHTML);
                }
                else {
                    //alert(cellID.innerText);
                    if (ActiveEmails != "")
                        ActiveEmails += "\n";
                    ActiveEmails += IndentString + ParseEmail(cellID.innerHTML);
                }
            }

            var ActiveEmailMsg = "";
            if (ActiveEmails.length > 0) {
                ActiveEmailMsg += "Following users will be suspended:\n\n" + ActiveEmails;
            }
            var SuspendEmailMsg = "";
            if (SuspendedEmails.length > 0) {
                SuspendEmailMsg += "Following users will be un-suspended:\n\n" + SuspendedEmails;
            }

            var Msg = "Are you sure you wish to perform the following actions.";
            if (ActiveEmailMsg.length > 0)
                Msg += "\n\n" + ActiveEmailMsg;
            if (SuspendEmailMsg.length > 0)
                Msg += "\n\n" + SuspendEmailMsg;

            return confirm(Msg);
        }
        function DeleteUser() {
            var grid = $find("<%= UserGrid.ClientID %>");
            var Selected = grid.MasterTableView.get_selectedItems();
            if (Selected.length == 0)
                return false; //nothing selected to do
            var AllEmails = "";
            for (var i = 0; i < Selected.length; i++) {
                var row = Selected[i];
                var cellID = grid.MasterTableView.getCellByColumnUniqueName(row, "AccountNameText");
                //alert(cellID.innerText);
                if (AllEmails != "")
                    AllEmails += "\n";
                AllEmails += ParseEmail(cellID.innerHTML);
            }

            AllEmails = "Are you sure you want to delete users:\n\n" + AllEmails;
            return confirm(AllEmails);
        }

        function OpenAddUser() {
            var wnd = window.radopen("AddUser.aspx", "Add User(s)");
            wnd.setSize(935, 700);
            wnd.set_modal(true);
            wnd.moveTo(0, 0);
            //wnd.Center();
            return false;
        }

        ///needed to select row if use click in text box
        function Select(index) {
            var grid = $find("<%= UserGrid.ClientID %>");
            //grid.MasterTableView.get_dataItems()[index].set_selected(true);

        }

        function RowClick(sender, eventArgs) {
            var gridItem = sender.get_masterTableView().get_dataItems()[eventArgs.get_itemIndexHierarchical()];
            var ApplyMe = document.getElementById("ApplyChangesButton");
            if (ApplyMe.disabled == false) {
                if (confirm("You have made changes to access rights and/or document downloads. Selecting a different user will result in losing these settings.\n\n Do you wish to continue?")) {
                    if (!gridItem.get_isInEditMode()) {
                        __doPostBack("<%= UserGrid.UniqueID %>", "RowClick;" + eventArgs.get_itemIndexHierarchical());
                   }
               }
           }
           else {
               if (!gridItem.get_isInEditMode()) {
                   __doPostBack("<%= UserGrid.UniqueID %>", "RowClick;" + eventArgs.get_itemIndexHierarchical());
                }
            }
        }
        function RowDoubleClick(sender, eventArgs) {
            var gridItem = sender.get_masterTableView().get_dataItems()[eventArgs.get_itemIndexHierarchical()];
            if (!gridItem.get_isInEditMode()) {
                __doPostBack("<%= UserGrid.UniqueID %>", "RowDoubleClick;" + eventArgs.get_itemIndexHierarchical());
            }
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


        FullSize("MainTable");
    </script>
</body>
</html>
