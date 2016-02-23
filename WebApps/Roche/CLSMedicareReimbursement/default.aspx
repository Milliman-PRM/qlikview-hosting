<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CLSMedicareReimbursement._default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <noscript>Javascript must be enabled to run the Clinical Lab Systems Medicare Reimbursement user interface.</noscript>

    <script src="scripts/jquery-2.2.0.min.js"></script>

    <title>Clinical Lab Systems Medicare Reimbursement</title>
    <style>
    body {
        background-color:white;
        margin:0px;
        overflow:hidden;
    }
    #header {
        height:75px;
        color:black;
        text-align:center;
        padding:5px;
        border-bottom:3px solid #046EBC;
        font-family: Arial;
        font-size:20px;
            }
    #section {
        width:99%;
        height:100%;
        float:left;
        padding:10px; 
    }
    #footer {
        text-align:left;
        font-family: Arial;
        font-size:12px;
        height:120px;
        bottom:0;
        position:absolute;
        padding:5px; 
    }
    #menu {
        text-align:left;
        font-family: Arial;
        font-size:12px;
        height:400px;
        width:1044px;
        top:85px;
        left:30px;
        position:absolute;
        visibility:visible;
        background-color:white;
        padding:0px; 
        margin:0px;
        border:3px solid #046EBC;

    }

    /*hide radgrid horizontal scroll*/ 
    #RatesGrid_GridData   
    {   
       overflow-x:hidden !important;   
    }         
 

    </style> 
</head>
<body>
    <form id="form1" runat="server">
     <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="false" PersistenceKey="Telerik.Skin" Skin="Metro" />
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RatesGrid">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RatesGrid"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1"></telerik:RadAjaxLoadingPanel>
     <div id="header">
         <table style="width:100%">
             <tr>
                <td style="vertical-align:bottom;padding-bottom:5px"><asp:ImageButton ID="LaunchMenu" runat="server" ImageUrl="~/Images/settings.png" OnClientClick="ShowMenu(); return false;" /></td>
                <td><h2>Clinical Lab Systems Medicare Reimbursement</h2></td>
                <td style="vertical-align:bottom;padding-bottom:5px" > <asp:Image ID="Brand" runat="server" ImageUrl="~/Images/roche.png"  Height="50px" /></td>
            </tr>
         </table>
    </div>

    <div id="menu">
            <telerik:RadAjaxPanel ID="menu_controls_panel" runat="server">
                 <div style="border:1px solid black;color:white;overflow:hidden;background-color:#046EBC;position:absolute;top:5px;left:5px;height:390px;width:256px">
                     <img src="Images/search.png" style="vertical-align: middle;float:right" />Analyzer
                     <div style="overflow:auto;width:100%;height:100%">
                        <asp:CheckBoxList ID="AnalyzerCheckList" runat="server"  BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AnalyzerCheckList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                    </div>
                </div>
 
                <div style="border:1px solid black;overflow:hidden;background-color:#046EBC;color:white;position:absolute;top:5px;left:266px;height:390px;width:512px">
                    <img src="Images/search.png" style="vertical-align: middle;float:right" />Assay Description<br />
                   <asp:ListBox ID="AssayDescriptionList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AssayDescriptionList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled"></asp:ListBox>
                </div>

                 <div style="border:1px solid black;overflow:hidden;background-color:#046EBC;color:white;position:absolute;top:5px;left:783px;height:390px;width:256px">
                     <img src="Images/search.png" style="vertical-align: middle;float:right" />Locality
                     <asp:ListBox ID="LocalityList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="LocalityList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled"></asp:ListBox>
                </div>
        </telerik:RadAjaxPanel>
    </div>

    <div id="section">
        <div style="margin-right:40px">
        <asp:DropDownList ID="YearDropdown"  style="float:right;" runat="server" Width="100px"></asp:DropDownList>
<%--        &nbsp;&nbsp;&nbsp;
        <asp:ImageButton style="float:right;" ID="ExportToExcel" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/excel.png" ToolTip="Export to Excel" />
        <asp:ImageButton style="float:right;" ID="ExportToWord" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/word.png" ToolTip="Export to Word" />
        <asp:ImageButton style="float:right;" ID="ExportToPDF" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/pdf.png" ToolTip="Export to PDF" />
        <asp:ImageButton style="float:right;" ID="PrintRates" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/print.png" ToolTip="Print Displayed Rates" />--%>
        <h2>Medicare Reimbursment Rates</h2>
        </div>
        <div  style="width: 99%;height:100%">
                <telerik:RadGrid RenderMode="Classic" ID="RatesGrid" runat="server" GridLines="Both" AllowSorting="true" AllowPaging="true" PageSize="250" Width="100%"  AllowCustomPaging="true" OnNeedDataSource="RatesGrid_NeedDataSource" PagerStyle-ShowPagerText="True" PagerStyle-Visible="False" OnSortCommand="RatesGrid_SortCommand">
                    <ClientSettings EnableRowHoverStyle="True" Resizing-AllowColumnResize="true" Resizing-EnableRealTimeResize="true" Resizing-ResizeGridOnColumnResize="false">
                        <Selecting CellSelectionMode="SingleCell" />
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" EnableVirtualScrollPaging="true" ></Scrolling>
                        <ClientEvents OnGridCreated="GridCreated" />
                    </ClientSettings>
                    <MasterTableView AllowMultiColumnSorting="false" ></MasterTableView>
                    <SortingSettings SortedBackColor="#FFF6D6" EnableSkinSortStyles="false"></SortingSettings>
                    <HeaderStyle Font-Bold="True"></HeaderStyle>
                </telerik:RadGrid>
        </div>
    </div>

    <div id="footer">
        <asp:BulletedList style="margin-left:20px; padding-left:0px" ID="FooterList" runat="server" BulletStyle="Numbered"></asp:BulletedList>
        <br />
        <asp:HyperLink ID="FooterLink" runat="server" Target="_blank"></asp:HyperLink>
    </div>

    </form>

    <script type="text/javascript">
        //must hide menu via JQuery, or it may not un-hide when needed
        $(menu).hide();

        function ShowMenu()
        {
            $(menu).show('slow');
            $(menu).mouseleave(function () { $(menu).hide(); })
        }
        
        function ResizeGrid()
        {
            var GridTop = $(RatesGrid).offset().top;
            var FooterTop = $(footer).offset().top;
            $(RatesGrid).height((FooterTop - GridTop) - 10);
 <%--           var scrollArea = document.getElementById("<%= RatesGrid.ClientID %>" + "_GridData");
            if(scrollArea)
            {
                scrollArea.style.height = $(RatesGrid).height() + "px";
            }--%>
        }

        function GridCreated(sender, args) {
            var scrollArea = sender.GridDataDiv;
            var parent = $get("RatesGrid");
            //alert(parent.clientHeight)
            var gridHeader = sender.GridHeaderDiv;
            scrollArea.style.height = parent.clientHeight -
              gridHeader.clientHeight + "px";

            //alert(scrollArea.style.height)
        }

        function PrintRatesGrid() 
        { 
            var previewWnd = window.open('about:blank', '', '', false); 
            var sh = '<%= ClientScript.GetWebResourceUrl(RatesGrid.GetType(),String.Format("Telerik.Web.UI.Skins.{0}.Grid.{0}.css",RatesGrid.Skin)) %>'; 
            var styleStr = "<html><head><link href = '" + sh + "' rel='stylesheet' type='text/css'></link></head>"; 
            var htmlcontent = styleStr + "<body>" + $find('<%= RatesGrid.ClientID %>').get_element().outerHTML + "</body></html>"; 
            previewWnd.document.open(); 
            previewWnd.document.write(htmlcontent); 
            previewWnd.document.close(); 
            previewWnd.print();
            previewWnd.close();
        }


        //resize the grid when window resizes
        $(window).resize(function () { ResizeGrid(); });
        $(document).ready(function () { ResizeGrid(); });

        if (typeof jQuery == 'undefined') {
            alert("jQuery could not be loaded.....");
        } 

    </script>
</body>
</html>
