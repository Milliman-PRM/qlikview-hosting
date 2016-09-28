<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="MillimanClientUserAdmin.AddUser" Async="true" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
          font:500 12px italic;
          font-family:'Segoe UI';
          overflow:hidden;
       } 
    </style> 
</head>
<body onresize="FullSize('MainTable');" style="overflow:hidden;">
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
	<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
	</telerik:RadAjaxManager>
    <table id="MainTable" style="position:absolute;top:10px;left:10px;width:100px;height:100px;visibility:hidden" >
        <tr style="height:20px;">
            <td>
               <center> <asp:Label ID="License" runat="server" Text=""></asp:Label></center>
            </td>
        </tr>
        <tr style="height:50%">
            <td>
                <telerik:RadPanelBar runat="server" ID="RadPanelBar1" Height="100%" Width="100%" ExpandMode="FullExpandedItem" >
                  <Items>
                    <telerik:RadPanelItem Expanded="True" Text="Data Restriction Selections" >
                            <Items>
                                <telerik:RadPanelItem Value="TreeHolder">
                                    <ItemTemplate>
                                        <telerik:RadTreeView runat="server" ID="AccessTree" CheckBoxes="True" CheckChildNodes="True" MultipleSelect="False" TriStateCheckBoxes="True" ClientIDMode="Static"></telerik:RadTreeView>
                                    </ItemTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                    </telerik:RadPanelItem>
                    <telerik:RadPanelItem Text="User Download Selections" Expanded="false" Visible="False">
                            <Items>
                               <telerik:RadPanelItem Value="DownloadHolder">
                                    <ItemTemplate>
                                        <telerik:RadTreeView runat="server" ID="DownloadTree" CheckBoxes="True" CheckChildNodes="True" MultipleSelect="False" TriStateCheckBoxes="True"></telerik:RadTreeView>
                                    </ItemTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                    </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
            </td>
        </tr>
        <tr style="">
            <td>
                <telerik:RadGrid runat="server" ID="RadGrid1" AllowSorting="True"  AutoGenerateColumns="False" CellSpacing="5"  GridLines="None" OnItemCommand="RadGrid1_ItemCommand" AllowAutomaticDeletes="True" ViewStateMode="Enabled" MasterTableView-AllowAutomaticDeletes="True" ClientIDMode="AutoID">
                    <MasterTableView EditMode="Batch" CommandItemDisplay="Top" TableLayout="Fixed">
                        <CommandItemTemplate>
                           <asp:LinkButton ID="Add" runat="server" CommandName="Add" Visible="true" ToolTip="Click to enter email addresses as comma, semi-colon, space or newline delimited."><asp:Image ID="Image1" runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/Office-Girl-icon.png"/>Add List Entry</asp:LinkButton>&nbsp;&nbsp;
                           <asp:LinkButton Width="100px" ID="Validate" runat="server" CommandName="Validate" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image ID="Image2" runat="server"  style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/process-icon.png"/>Validate</asp:LinkButton>&nbsp;&nbsp;
               
                           <asp:LinkButton ID="Clear"  runat="server" CommandName="Clear" Visible='<%# RadGrid1.EditIndexes.Count == 0 %>'><asp:Image ID="Image3" runat="server" style="border:0px;vertical-align:middle;" alt="" ImageUrl="~/Images/close_24.png"/>Clear List</asp:LinkButton>&nbsp;&nbsp;
                       </CommandItemTemplate>
                        <Columns>
  

                              <telerik:GridTemplateColumn DataField="ValidationImage"  UniqueName="ValidationImageStatus" HeaderStyle-Width="25px"  >
                                <ItemTemplate>
                                    <asp:Image ID="ValidationStatusImage" runat="server" ImageUrl='<%#Eval("ValidationImage") %>' ToolTip='<%#Eval("ErrorMsg") %>'/>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn DataField="Account_Name" HeaderText="Account" UniqueName="AccountNameText" HeaderStyle-Width="97%"  >
                                <ItemTemplate>
                                    <asp:TextBox style="overflow:hidden" ID="AccountNameTextBox" runat="server" AutoPostBack="false" Text='<%#Eval("Account_Name") %>' Width="100%" TextMode="MultiLine" Rows="1" ></asp:TextBox>
                                </ItemTemplate>

                                <HeaderStyle Width="97%"></HeaderStyle>
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

                            <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" ConfirmText="Delete this user from the list?" ConfirmDialogType="Classic">
                                <HeaderStyle Width="32px" />
                            </telerik:GridButtonColumn>
                        </Columns>

                    </MasterTableView>
                </telerik:RadGrid> 
            </td>
        </tr>
        <tr style="color: #FF9900;height:20px;font-size:14px;font-style:italic">
<%--            <td><center>Paste multiple emails as comma, semi-colon, space or newline delimited and click 'Add List Entry' to create multiple accounts.</center></td>--%>
            <td><center>To create multiple new accounts with the above Data Restriction Selections, enter email addresses as comma, semi-colon, space or newline delimited, then click 'Add List Entry' button.</center></td>
        </tr>
        <tr style="height:30px">
            <td><center><asp:Button ID="CreateUsers" runat="server" Text="Create Accounts" Width="200px" OnClientClick="return StartProcessing();" OnClick="CreateUsers_Click"/></center></td>
        </tr>
    </table>

   <div id="LockPane" class="LockOff" style="overflow: hidden">
         <iframe frameBorder="0" seamless="seamless" style="border:none;overflow:hidden;" width="50" height="50" src="Images/frameanimation.aspx" name="imgbox" id="imgbox" ></iframe><br />
         <br />
         Account modifications in progress.....
     </div> 

       <script type="text/javascript">

           function StartProcessing() {

               var AccessTree = $find("AccessTree");
               if (AccessTree) {
                   //we do not allow a user to be added if access rights are not selected
                   if (AccessTree.get_checkedNodes().length == 0) {
                       alert("Selecting the user access rights from the 'Data Restrictions Selections' tree is required to add user accounts.");
                       return false;
                   }
               }
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
    </script>
    </form>

    <script type="text/javascript">
        FullSize("MainTable");
    </script>
</body>
</html>
