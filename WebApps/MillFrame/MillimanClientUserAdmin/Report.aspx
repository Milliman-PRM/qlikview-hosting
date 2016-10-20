<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="MillimanClientUserAdmin.Report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 90%">
<head id="Head1" runat="server">
    <title>PRM Reports</title>
    
    <style type="text/css">
 
    </style>

</head>
<body style="overflow:hidden;background-color:white;width:100%;height:100%" onload="Ready();">
    <form id="form1" runat="server" style="width:100%;height:100%">
         <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="true" />
        <table style="width:100%;height: 100%;">
        <tr>
        <td style="height:30px;min-width:800px">
            <telerik:RadTabStrip  ID="Menu" runat="server" MultiPageID="RadMultiPage1" Skin="MetroTouch" SelectedIndex="0" Align="Left" Font-Size="10px" Font-Italic="False">
                <Tabs >
                    <telerik:RadTab Text="User Access Times" ToolTip="Displays the login times and sessions duration of users of this report"></telerik:RadTab>
                    <telerik:RadTab Text="User Assignments" ToolTip="Information describing the data set users are allowed to view."></telerik:RadTab>
                    <telerik:RadTab Text="User Download Assignments" ImageUrl="images/Places-user-identity-icon.png"  ToolTip="Items (per user) assigned for download."></telerik:RadTab>
                    <telerik:RadTab Text="Failed Selections Per User" ImageUrl="images/Places-user-identity-icon.png" ToolTip="Provides a list of all current selections that cannot be applied to new report."></telerik:RadTab>
                    <telerik:RadTab Text="New Selectable Items" ImageUrl="images/Places-user-identity-icon.png" ToolTip="Provides a list of all new items per new report that have not been associated with any users."></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
        </td>
        </tr>
       <tr style="height:100%">
       <td style="height:100%">
           <div id="reportcontainer" style="height:100%;overflow:auto; background-color:white; border:1px solid gray; margin-bottom: 1em; padding: 5px">
                <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" Height="100%">
                    <telerik:RadPageView ID="RadPageView5" runat="server"  ContentUrl="UserLogins.aspx" Height="100%"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView1" runat="server"  ContentUrl="UserAssignments.aspx" Height="100%"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView2" runat="server"  ContentUrl="UserDownloads.aspx" Height="100%"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView3" runat="server"  ContentUrl="NotSelectable.aspx" Height="100%"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView4" runat="server"   ContentUrl="NewItems.aspx" Height="100%"></telerik:RadPageView>
                </telerik:RadMultiPage>
            </div>
        </td>
        </tr>
        <tr>
        <td style="text-align:center;height:20px;font-size:12px;font-family:segoe ui">
                    Copyright © Milliman &nbsp
                                <asp:Label ID="lblcopyrightYear" runat="server"></asp:Label>
                    <script type="text/javascript">document.getElementById("lblcopyrightYear").innerHTML = new Date().getFullYear();</script>
        </td>
        </tr>
      </table>      
    </form>
    <script type="text/javascript">

        function Ready() {

        }

</script>

</body>
</html>