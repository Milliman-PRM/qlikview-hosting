<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CLSMedicareReimbursement._default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
      
     <script src="scripts/jquery-1.12.1.min.js" type="text/javascript"></script>
     <script type="text/javascript" >
         if (typeof jQuery == 'undefined') {
             alert("jQuery could not be loaded.....");
         }
     </script>

    <title>Clinical Lab Systems Medicare Reimbursement</title>
    <style type="text/css">
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
        width:98%;
        height:100%;
        float:left;
        padding:10px; 
    }
    #footer {
        text-align:left;
        font-family: Arial;
        font-size:12px;
        height:120px;
        bottom:0px;
        left:10px;
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
 
    .RadSearchBox .rsbEmptyMessage {
       color:#046EBC !important;
       opacity:1.0;
       font-weight:bolder !important;
       font-style:normal !important;
       outline:none; /* remove chrome defalt textbox outline */
    }
    </style> 
</head>
<body onload="Ready()">
    <form id="form1" runat="server">
     <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="false" PersistenceKey="Telerik.Skin" Skin="Metro" />
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <%--<telerik:AjaxSetting AjaxControlID="RatesGrid">--%>
                <telerik:AjaxSetting >
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RatesGrid"></telerik:AjaxUpdatedControl>
                        <telerik:AjaxUpdatedControl ControlID="menu_controls_panel"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Transparency="60" BackColor="lightgray" Skin="Default" ></telerik:RadAjaxLoadingPanel>
     <div id="header">
         <table style="width:100%">
             <tr>
<%--                <td style="vertical-align:bottom;padding-bottom:5px"><asp:ImageButton ID="LaunchMenu" runat="server" ImageUrl="~/Images/settings.png" OnClientClick="ShowMenu(); return false;" /></td>--%>
                <td style="vertical-align:bottom;padding-bottom:5px"><asp:ImageButton ID="LaunchMenu" runat="server" ImageUrl="~/Images/settings.png" OnClick="LaunchMenu_Click" /></td>

                <td><h2>Clinical Lab Systems Medicare Reimbursement</h2></td>
                <td style="vertical-align:bottom;padding-bottom:5px" > <asp:Image ID="Brand" runat="server" ImageUrl="~/Images/roche.png"  Height="50px" /></td>
            </tr>
         </table>
    </div>

    <div id="menu" runat="server" visible="false" >
            <telerik:RadAjaxPanel ID="menu_controls_panel" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
                 <div style="border:1px solid black;color:white;overflow:hidden;background-color:#046EBC;position:absolute;top:5px;left:5px;height:390px;width:256px">
                     <telerik:RadSearchBox runat="server" ID="AnalyzerSearch" EmptyMessage="Analyzer" Width="100%" DataTextField="AnalyzerName" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="255px" OnSearch="AnalyzerSearch_Search" OnClientSearch="OnClientSearch" RenderMode="Lightweight" >
                     </telerik:RadSearchBox>
                     <div style="overflow:auto;width:100%;height:100%">
                        <asp:CheckBoxList ID="AnalyzerCheckList" runat="server"  BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AnalyzerCheckList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                    </div>
                </div>
 
                <div style="border:1px solid black;overflow:hidden;background-color:#046EBC;color:white;position:absolute;top:5px;left:266px;height:390px;width:512px">
                     <telerik:RadSearchBox runat="server" ID="AssayDescriptionSearch" EmptyMessage="Assay Description" Width="100%" DataTextField="SearchDesc" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="511px"  RenderMode="Lightweight" OnSearch="AssayDescriptionSearch_Search" OnClientSearch="OnClientSearch" >
                     </telerik:RadSearchBox>
                   <asp:ListBox ID="AssayDescriptionList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AssayDescriptionList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled"></asp:ListBox>
                </div>

                 <div style="border:1px solid black;overflow:hidden;background-color:#046EBC;color:white;position:absolute;top:5px;left:783px;height:390px;width:256px">
                     <telerik:RadSearchBox runat="server" ID="LocalitySearch" EmptyMessage="Locality" Width="100%" DataTextField="LocalityDescLong" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="255px" RenderMode="Lightweight" OnSearch="LocalitySearch_Search" OnClientSearch="OnClientSearch" >
                     </telerik:RadSearchBox>
                     <asp:ListBox ID="LocalityList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="LocalityList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled"></asp:ListBox>
                </div>
        </telerik:RadAjaxPanel>
    </div>

    <div id="section">
        <div style="margin-right:40px">
        <asp:DropDownList ID="YearDropdown"  style="float:right;" runat="server" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="YearDropdown_SelectedIndexChanged"></asp:DropDownList>
<%--        &nbsp;&nbsp;&nbsp;
        <asp:ImageButton style="float:right;" ID="ExportToExcel" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/excel.png" ToolTip="Export to Excel" />
        <asp:ImageButton style="float:right;" ID="ExportToWord" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/word.png" ToolTip="Export to Word" />
        <asp:ImageButton style="float:right;" ID="ExportToPDF" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/pdf.png" ToolTip="Export to PDF" />
        <asp:ImageButton style="float:right;" ID="PrintRates" runat="server" OnClientClick="PrintRatesGrid()" ImageUrl="~/Images/print.png" ToolTip="Print Displayed Rates" />--%>
        <h2>Medicare Reimbursment Rates</h2>
        </div>
        <telerik:RadAjaxPanel ID="RadGridPanel" runat="server" OnAjaxRequest="RadGridPanel_AjaxRequest" LoadingPanelID="RadAjaxLoadingPanel1">
            <div  style="width: 99%;height:100%">
                    <telerik:RadGrid RenderMode="Classic" ID="RatesGrid" runat="server" GridLines="Both" AllowSorting="true" AllowPaging="true" PageSize="250" Width="100%"  AllowCustomPaging="true" OnNeedDataSource="RatesGrid_NeedDataSource" PagerStyle-ShowPagerText="True" PagerStyle-Visible="True" OnSortCommand="RatesGrid_SortCommand" ClientIDMode="AutoID" AutoGenerateColumns="False" OnSelectedCellChanged="RatesGrid_SelectedCellChanged" >
                        <ClientSettings EnableRowHoverStyle="True" Resizing-AllowColumnResize="true" Resizing-EnableRealTimeResize="true" Resizing-ResizeGridOnColumnResize="false" EnablePostBackOnRowClick="True">
                            <Selecting CellSelectionMode="SingleCell" />
                            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" EnableVirtualScrollPaging="true" ></Scrolling>
                            <ClientEvents OnGridCreated="GridCreated" />
                        </ClientSettings>
                        <MasterTableView AllowMultiColumnSorting="false" PagerStyle-AlwaysVisible="true" >
                            <Columns>
                                <telerik:GridBoundColumn UniqueName="analyzer_name" DataField="analyzer_name" HeaderText="Analyzer" ReadOnly="True"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="description" DataField="description" HeaderText="Assay Description" ReadOnly="True"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="code" DataField="code" HeaderText="CPT Descriptor" ReadOnly="True"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="notes" DataField="notes" HeaderText="Notes" ReadOnly="True"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="locality_desc_shrt" DataField="locality_desc_shrt" HeaderText="Locality" ReadOnly="True"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="rate" DataField="rate" HeaderText="Medicare Reimbursement Rate" DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" ReadOnly="True"></telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                       <%-- <SortingSettings SortedBackColor="#FFF6D6" EnableSkinSortStyles="true"></SortingSettings>--%>
                         <SortingSettings SortedBackColor="#FFFAED" EnableSkinSortStyles="true"></SortingSettings>
                        <HeaderStyle Font-Bold="True"></HeaderStyle>
                    </telerik:RadGrid>
            </div>
        </telerik:RadAjaxPanel>
    </div>

    <div id="footer">
        <asp:BulletedList style="margin-left:20px; padding-left:0px" ID="FooterList" runat="server" BulletStyle="Numbered"></asp:BulletedList>
        <br />
        <asp:HyperLink ID="FooterLink" runat="server" Target="_blank"></asp:HyperLink>
    </div>

    <telerik:RadNotification RenderMode="Lightweight" ID="Toast" runat="server" VisibleOnPageLoad="false" Position="BottomRight"
                             Width="330" Height="160" Animation="Slide" EnableRoundedCorners="true" EnableShadow="true"
                             Title="Selections Restored" Text=""
                             Style="z-index: 100000" AnimationDuration="5000" TitleIcon="">

    </telerik:RadNotification>

    </form>

    <script type="text/javascript" >
        //must hide menu via JQuery, or it may not un-hide when needed
       // $(menu).hide();

        function MenuEvents()
        {
            if (document.getElementById("menu") !== null) {

                $(menu).mouseleave(function () {
                    //we have to check where the mouse is,  due to the implementation of some of the controls
                    //we will get a mouseleave event, like when the search dropdown opens
                    
                    //we need to check these to make sure the drop down is not active
                    var AnalyzerSearchNode = $telerik.$($find("AnalyzerSearch").get_childListElement());
                    var AssaySearchNode = $telerik.$($find("AssayDescriptionSearch").get_childListElement());
                    var LocalitySearchNode = $telerik.$($find("LocalitySearch").get_childListElement());
                    if ((AnalyzerSearchNode.length == 0) && (AssaySearchNode.length == 0) && (LocalitySearchNode.length == 0)) {
                        $(menu).hide();

                        var currentLoadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                        var currentGrid = $find("<%= RadGridPanel.ClientID%>")
                        currentLoadingPanel.show(currentGrid);
                        $find("<%= RadGridPanel.ClientID%>").ajaxRequestWithTarget("<%= RadGridPanel.UniqueID %>", "refresh");

                    }

       <%--             var currentLoadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                    var currentGrid = $find("<%= RadGridPanel.ClientID%>")
                    currentLoadingPanel.show(currentGrid);--%>
                    //__doPostBack('__Page', 'MenuClose');
                })
            }
        }
        
        var PaddingForFooter = 25;  //this is padding between bottom of grid and footer
        var GridDataSize = 0;  //set on resize and then used to restore on virtual scroll
        function ResizeGrid()
        {
            var grid = document.getElementById('<%=RatesGrid.ClientID %>');
            var GridTop = $(grid).offset().top;
            var FooterTop = $(footer).offset().top;
            $(grid).height((FooterTop - GridTop) - PaddingForFooter);
            //get the size of the data area
            var griddata = $get("RatesGrid_GridData");
            GridDataSize = Math.floor((FooterTop - $(griddata).offset().top) - PaddingForFooter);
        }
        //restore the grid data to correct size
        function ResizeGridDataArea()
        {
            if (GridDataSize > 0) {
                var griddata = $get("RatesGrid_GridData");
                $(griddata).height(GridDataSize);
            }
        }

        function GridCreated(sender, args) {
            var scrollArea = sender.GridDataDiv;
            var parent = $get("RatesGrid");
            //alert(parent.clientHeight)
            var gridHeader = sender.GridHeaderDiv;
            scrollArea.style.height = parent.clientHeight - gridHeader.clientHeight + "px";
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


        //dont allow a post back if nothing is selected
        function OnClientSearch(sender, args) {

            if (sender.get_text().length < 1) {

                sender._element.control._postBackOnSearch = false;
            }

        }
  
        function Ready()
        {
            MenuEvents();
        }

        //intercept ajax request to keep grid from going to an odd size
        (function (open) {
            XMLHttpRequest.prototype.open = function (method, url, async, user, pass) {
                this.addEventListener("readystatechange", function () {
                    ResizeGridDataArea();  //resize grid data
                }, false);
                open.call(this, method, url, async, user, pass);
            };
        })(XMLHttpRequest.prototype.open);


        //resize the grid when window resizes
        $(window).resize(function () { ResizeGrid(); });
        $(document).ready(function () { ResizeGrid(); });

    </script>
</body>
</html>
