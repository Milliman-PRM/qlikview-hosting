<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReductionStep2.aspx.cs" Inherits="ClientPublisher.ReductionStep2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <style type="text/css">
        div.RadTabStrip .rtsLevel .rtsIn, 
        div.RadTabStrip .rtsLevel .rtsTxt { padding: 0; }
    </style>

</head>
<body style="overflow:hidden;" onload="Ready();">
    <form id="form1" runat="server" style="width:100%;height:100%">
         <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="true" />
        <header style="height:30px">
            <telerik:RadTabStrip ID="Menu" runat="server" MultiPageID="RadMultiPage1" Skin="MetroTouch" SelectedIndex="0" Align="Left" Font-Size="9px" Font-Italic="True" Width="98%">
                <Tabs>
                    <telerik:RadTab Text="Summary" ToolTip="Summary of actions/changes for the project."></telerik:RadTab>
                    <telerik:RadTab Text="Failed Selections Per User" ImageUrl="../images/Places-user-identity-icon.png" ToolTip="Client administrator visible - provides a list of all current selections that cannot be applied to new report."></telerik:RadTab>
                    <telerik:RadTab Text="New Selectable Items" ImageUrl="../images/Places-user-identity-icon.png" ToolTip="Client administrator visible - provides a list of all new items per new report that have not been associated with any users."></telerik:RadTab>
                    <telerik:RadTab Text="Reduction Status" ToolTip="Detail breakdown of reduction per user processing."></telerik:RadTab>
                    <telerik:RadTab Text="General Processing Status" ToolTip="Processing status of all items associated with processing chain."></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
        </header>
       <section>
           <div id="reportcontainer" style="height:555px;overflow:auto; background-color:white; border:1px solid gray; margin-bottom: 1em; padding: 5px">
                <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
                    <telerik:RadPageView ID="RadPageView1" runat="server" height="550px" ContentUrl="ComplexReporting/Summary.aspx"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView2" runat="server" height="550px" ContentUrl="ComplexReporting/NotSelectable.aspx"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView3" runat="server" height="550px"  ContentUrl="ComplexReporting/NewItems.aspx"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView5" runat="server" height="550px"  ContentUrl="ComplexReporting/ReductionStatus.aspx"></telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView6" runat="server" height="550px"  ContentUrl="ComplexReporting/ReductionAudit.aspx"></telerik:RadPageView>
                </telerik:RadMultiPage>
            </div>
        </section>

        <footer style="text-align:center;height:30px">
            <asp:Button ID="Publish" runat="server" Text="Publish to Production" OnClick="Publish_Click" OnClientClick="return confirm('By clicking \'OK\' you are acknowledging you have reviewed the results of processing your report and the information is correct to publish to a client.\n\nClicking \'OK\' will publish your report to the users.'); " />
        </footer>
    </form>
    <script type="text/javascript">

        function Ready() {
  
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) {
                oWindow = window.radWindow;
            }
            else if (window.frameElement.radWindow) {
                oWindow = window.frameElement.radWindow;
            }
            return oWindow;
        }

        var RW = GetRadWindow();
        if (RW.set_title)
            RW.set_title("Reports");
        if (RW.SetHeight)
            RW.SetHeight(800);
 </script>

</body>
</html>
