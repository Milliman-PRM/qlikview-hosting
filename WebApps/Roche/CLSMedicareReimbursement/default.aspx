<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CLSMedicareReimbursement._default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script src="scripts/jquery-1.12.1.min.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
    <script type="text/javascript">
        if (typeof jQuery == 'undefined') {
            alert("jQuery could not be loaded.....");
        }
    </script>
    <title>Roche Lab Systems Handbook</title>
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW" />
    <style type="text/css">
        body{background-color:white;margin:0px;overflow:hidden}
        #ContainerHeaderSection{height:75px;color:black;text-align:center;padding:5px;border-bottom:3px solid #0072C6;font-family:Arial;font-size:20px}
        #ContainerMenuList{text-align:left;font-family:Arial;font-size:12px;height:400px;width:1121px;top:91px;left:104px;position:absolute;visibility:visible;background-color:white;padding:0px;margin:0px;border:3px solid #0072C6;z-index:2000}
        #footer{text-align:left;font-family:Arial;font-size:12px;height:133px;bottom:0px;left:48px;position:absolute;padding:2px}
        #RatesGrid_GridData{overflow-x:hidden !important}
        .RadSearchBox .rsbEmptyMessage{color:#046EBC !important;opacity:1.0;font-weight:bolder !important;font-style:normal !important;outline:none}
        #ContainerUIMain{margin:0px auto;padding:6px;border-image:none;width:95%;color:rgb(88, 88, 88);height:890px}
        #ContainerGrid{padding:3px 3px 3px 3px;margin:0 auto}
        .clear{height:8px}        
        .ContainerCheckBox{overflow:scroll;height:382px;width:265px}
        .ContainerAnalyzerCheckList{border:1px solid #808080;color:white;overflow:hidden;position:absolute;top:5px;left:5px;height:390px;width:265px}
        .ContainerAssayDescriptionList{border:1px solid #808080;overflow:hidden;color:white;position:absolute;top:5px;left:275px;height:390px;width:265px}
        .ContainerLocalityList{border:1px solid #808080;overflow:hidden;color:white;position:absolute;top:5px;left:710px;height:390px;width:265px}
        .ContainerCptCodeList{border:1px solid #808080;overflow:hidden;color:white;position:absolute;top:5px;left:544px;height:390px;width:162px}
        .ContainerViewReport{overflow:hidden;color:white;position:absolute;top:308px;left:981px;height:43px;width:136px}
        .ContainerClearButton{overflow:hidden;color:white;position:absolute;top:351px;left:981px;height:43px;width:136px}
        .CustomButton{margin:4px 3px 0px 6px;border:solid 1px #bbb;padding:8px;-moz-border-radius:3px;-webkit-border-radius:3px;border-radius:3px;-moz-box-shadow:0px 0px 7px #bbb;-webkit-box-shadow:#bbb 0px 0px 7px;box-shadow:0px 0px 7px #bbb;color:#444;font-size:105%;float:left;background:#ddd;text-shadow:1px 1px #bbb}
        .CustomButton:hover{background-color:#ddd;color:#333;border:solid 1px #0072C6;cursor:pointer;outline:none}
        .closeImageMenu{cursor:pointer;float:right;height:15px;padding:0;width:14px;-moz-border-radius:8px;-webkit-border-radius:8px;border-radius:8px;position:absolute;left:1104px;top:1px}
        .searchImage{left:70px;position:absolute;top:60px;width:90px;height:31px;cursor:pointer}
        .ContainerInfoImage{position:absolute;top:66px;width:24px;height:21px;cursor:pointer;float:right;right:161px}
        select option:checked{background:linear-gradient(#0066FF,#0066FF);background-color:#0066FF !important}
        .messagePopUpWindow{background:none repeat scroll 0 0 white;border:14px solid #CCC;height:520px;left:586px;overflow-x:hidden;padding:17px;top:80px;width:580px;z-index:12000;line-height:14px;position:absolute;-webkit-border-bottom-right-radius:5px;-webkit-border-bottom-left-radius:5px;-webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-bottomleft:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;border-bottom-right-radius:5px;border-bottom-left-radius:5px;border-top-left-radius:5px;border-top-right-radius:5px}
        .acc_container{margin:0 0 5px;padding:0;overflow:hidden;clear:both;background:#f0f0f0;border:1px solid #d6d6d6;border-top:none}
        .acc_container .block{padding:4px 0px 0px 17px}
        .clsoeImageInfo{border-radius:8px 8px 8px 8px;cursor:pointer;float:right;height:15px;margin:-18px -19px 0 0;padding:3px;width:14px;-moz-border-radius:8px;-webkit-border-radius:8px;border-radius:8px}
        .notify{background:none repeat scroll 0 0 #FFFCCF;border:1px solid #F4E889;display:block;padding:5px;width:217px;margin:0 auto;text-align:center}
        .tableLineItems{list-style:none;margin:2px 0;padding:0;width:541px;float:left;background:#E7E8E9}
        #tableCurrOutlines tr{border:0}
        .shadowFont{text-shadow:0 1px 1px #F2F2F2;font-weight:bold;color:#333;text-decoration:none;font-family:Sans-Serif;font-size:12px}
        .description{font-size:11px;line-height:15px;font-family:arial,sans-serif;font-weight:normal}
        .lablePointer{color:#5f5f5f;content:" â˜›";font-size:13px;font-weight:700;line-height:18px;margin-left:29px}
        .RadSearchBox .rsbEmptyMessage{color:#0072C6 !important}
        .table-condensed th,.table-condensed td{padding:4px 5px}
        #containerCopyright{right:50px;bottom:17px;width:154px;text-align:center;font-size:small;float:right;position:absolute}
        .RadGrid_Outlook{border:1px solid #0072C6}
        .rgHeaderDivForChrome{margin-right:15px !important}
        .rgHeaderDivForIE{margin-right:17px !important}
        .rgHeaderDivForSpartan{margin-right:12px !important}
        .rgHeaderDivForFireFox{margin-right:21px !important}
    </style>
</head>
<body onload="Ready()">

    <form id="form1" runat="server">

        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="false" PersistenceKey="Telerik.Skin" Skin="Metro" />
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RatesGrid" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting>
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="menu_controls_panel"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Transparency="60" BackColor="lightgray" Skin="Default"></telerik:RadAjaxLoadingPanel>

        <div id="ContainerUIMain">
            <div class="ContainerInfoImage">
                <img id="imgInformation" width="24px" height="21px" src="Images/info_icon.png" alt="..." onclick="return showInformationWindow('ContainerInformation');" />
            </div>
            <%--header section--%>
            <div id="ContainerHeaderSection">
                <table style="width: 100%">
                    <tr>
                        <td style="vertical-align: bottom; padding-bottom: 5px">
                            <asp:ImageButton ID="LaunchMenu" runat="server" CssClass="searchImage" ImageUrl="~/Images/search_icon.png" OnClick="LaunchMenu_Click" />
                        </td>
                        <td>
                            <h2>Roche Lab Systems Handbook</h2>
                        </td>
                    </tr>
                </table>
            </div>

            <div id="ContainerInformation" class="messagePopUpWindow" style="display: none;">
                <img id="imageClose" class="clsoeImageInfo" src="Images/cancel.png" alt="..." onclick="return hideInformationWindow('ContainerInformation');" />
                <div id="ContainerShowMessage" class="acc_container">
                    <div class="block">
                        <label class="notify"><b>How to</b></label>
                        <table id="tableCurrOutlines" class="table table-hover table-condensed">
                            <tr class="tableLineItems">
                                <td>
                                    <em class="shadowFont">Narrowing the results</em>
                                    <p class="description">
                                        Upon entering the Roche Lab Systems Handbook, the search menu will be displayed.  From this menu the user can narrow the selections by Analyzer, Assay Description, CPT Code, Locality, or any combination of these fields.  Once the desired selections have been made, the results can be viewed by either clicking on the “View Report” button on the right side of the menu, or by clicking on the exit button in the upper right-hand corner.  To make further selections, the user should open the search menu by clicking on the “Search” button at the upper left-hand corner of the tool.  Additionally, while viewing the result, it is possible to narrow the selections by selecting values directly in the rates table.
                                    </p>
                                </td>
                            </tr>
                            <tr class="tableLineItems">
                                <td>
                                    <em class="shadowFont">Viewing Rates from Previous Time Periods</em>
                                    <p class="description">
                                        To view the reimbursement rates from a previous time period, the user can select the desired period from the drop-down menu on the upper right-hand corner of the tool.  The rates displayed in the table below will be automatically updated based on that new selection.
                                    </p>
                                </td>
                            </tr>
                            <tr class="tableLineItems">
                                <td>
                                    <em class="shadowFont">Sorting the Results</em>
                                    <p class="description">
                                        To sort the results in the rates table, the user should click on the column header text.  Clicking on the header once will cause the column to sort in ascending order, while clicking a second time will cause the column to sort in descending order.
                                    </p>
                                </td>
                            </tr>
                            <tr class="tableLineItems">
                                <td>
                                    <em class="shadowFont">Clearing Selections</em>
                                    <p class="description">
                                        To reset the selections that have been made in the tool, the user should open up the search menu by clicking on the button at the upper left-hand corner of the tool that says “Search”.  Once the search menu has been opened, clicking the “Clear Selections” button in the lower right-hand corner of the menu will clear any selections that have been made, allowing the user to make new selections as desired.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>

            <%--menu section--%>
            <div id="ContainerMenuList" runat="server">
                <telerik:RadAjaxPanel ID="menu_controls_panel" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
                    <div class="ContainerAnalyzerCheckList">
                        <telerik:RadSearchBox runat="server" ID="AnalyzerSearch" EmptyMessage="Analyzer" Width="100%" DataTextField="AnalyzerName" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="265px" OnSearch="AnalyzerSearch_Search" OnClientSearch="OnClientSearch" RenderMode="Lightweight">
                        </telerik:RadSearchBox>
                        <div class="ContainerCheckBox">
                            <asp:CheckBoxList ID="AnalyzerCheckList" runat="server" BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AnalyzerCheckList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="ContainerAssayDescriptionList">
                        <telerik:RadSearchBox runat="server" ID="AssayDescriptionSearch" EmptyMessage="Assay Description" Width="100%" DataTextField="SearchDesc" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="265px" RenderMode="Lightweight" OnSearch="AssayDescriptionSearch_Search" OnClientSearch="OnClientSearch">
                        </telerik:RadSearchBox>
                        <div class="ContainerCheckBox">
                            <asp:CheckBoxList ID="AssayDescriptionList" runat="server" BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AssayDescriptionList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="ContainerCptCodeList">
                        <telerik:RadSearchBox runat="server" ID="CptCodeSearch" Width="100%" EmptyMessage="CPT Code" DataTextField="Code1" DataValueField="Id" DropDownSettings-Height="368px" DropDownSettings-Width="162px" RenderMode="Lightweight" OnSearch="CptCodeSearch_Search" OnClientSearch="OnClientSearch">
                        </telerik:RadSearchBox>
                        <div class="ContainerCheckBox" style="width: 162px;">
                            <asp:CheckBoxList ID="CptCodeList" runat="server" BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="CptCodeList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="ContainerLocalityList">
                        <telerik:RadSearchBox runat="server" ID="LocalitySearch" EmptyMessage="Locality" Width="100%" DataTextField="LocalityDescription" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="265px" RenderMode="Lightweight" OnSearch="LocalitySearch_Search" OnClientSearch="OnClientSearch">
                        </telerik:RadSearchBox>
                        <div class="ContainerCheckBox">
                            <asp:CheckBoxList ID="LocalityList" runat="server" BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="LocalityList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="ContainerViewReport">
                        <%--When user click on this button, it refreses the screen and close div. The Client side code is executed. if jquery function would return true, server side code would process. Returning false prevents the page from posting back to the server.--%>
                        <asp:Button ID="btnViewReport" runat="server" Text="View Report" CssClass="CustomButton" Width="126px" ForeColor="Black" Font-Bold="true" BackColor="LightGray"
                            OnClientClick="return closeWindow('ContainerMenuList');" />
                    </div>
                    <div class="ContainerClearButton">
                        <asp:Button ID="btnClearSelection" runat="server" Text="Clear Selections" CssClass="CustomButton" Width="126px" ForeColor="Black" Font-Bold="true" BackColor="LightGray"
                            OnClick="btnClearSelection_Click" />
                    </div>
                    <div id="ContainerCloseMenu" style="display: block;">
                        <img id="imgCancel" class="closeImageMenu" src="Images/cancel.png" alt="..." onclick="return closeWindow('ContainerMenuList');" />
                    </div>
                </telerik:RadAjaxPanel>
            </div>

            <%--grid section--%>
            <div id="section" style="height: 693px;">
                <div style="margin-right: 40px;">
                    <asp:DropDownList ID="YearDropdown" Style="float: right;" runat="server" Width="100px" AutoPostBack="True"
                        OnSelectedIndexChanged="YearDropdown_SelectedIndexChanged">
                    </asp:DropDownList>
                    <h2>Medicare Reimbursment Rates</h2>
                </div>
                <telerik:RadAjaxPanel ID="RadGridPanel" runat="server" OnAjaxRequest="RadGridPanel_AjaxRequest" LoadingPanelID="RadAjaxLoadingPanel1" ClientEvents-OnRequestStart="pnlRequestStarted">

                    <div id="ContainerGrid">
                        <telerik:RadGrid RenderMode="Lightweight" ID="RatesGrid" runat="server" ClientIDMode="AutoID"
                            GridLines="None" AutoGenerateColumns="False" Skin="Outlook"
                            Width="100%"
                            AllowSorting="true" AllowPaging="true" PageSize="250" AllowCustomPaging="true" PagerStyle-ShowPagerText="True" PagerStyle-Visible="True"
                            OnNeedDataSource="RatesGrid_NeedDataSource"
                            OnSelectedCellChanged="RatesGrid_SelectedCellChanged"
                            OnSortCommand="RatesGrid_SortCommand">
                            <PagerStyle Visible="false" />
                            <ClientSettings ReorderColumnsOnClient="false" AllowColumnsReorder="false" EnablePostBackOnRowClick="True">
                                <ClientEvents OnGridCreated="GridCreated" OnCellSelected="CellSelected" />
                                <Virtualization EnableVirtualization="false" InitiallyCachedItemsCount="2000" LoadingPanelID="RadAjaxLoadingPanel1" ItemsPerView="500" />
                                <Selecting CellSelectionMode="SingleCell" />
                                <Scrolling ScrollHeight="480px" AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" EnableVirtualScrollPaging="true"></Scrolling>
                                <Resizing AllowColumnResize="True" ClipCellContentOnResize="true" EnableRealTimeResize="true" ResizeGridOnColumnResize="true" />
                            </ClientSettings>

                            <MasterTableView AllowMultiColumnSorting="false" PagerStyle-AlwaysVisible="True" Width="100%" TableLayout="Fixed" Name="ownertableviewRatesGrid">
                                <Columns>
                                    <telerik:GridBoundColumn UniqueName="analyzer_name" DataField="analyzer_name" HeaderText="Analyzer" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="18%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="description" DataField="description" HeaderText="Assay Description" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="30%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="code" DataField="code" HeaderText="CPT Code" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="9%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="locality_description" DataField="locality_description" HeaderText="Locality" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="16%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="notes" DataField="notes" HeaderText="Notes" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="8%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="rate" DataField="rate" HeaderText="Medicare Reimbursement Rate"  DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="19%" HorizontalAlign="Right" />
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                            <SortingSettings SortedBackColor="#FFFAED" EnableSkinSortStyles="true"></SortingSettings>
                            <HeaderStyle Font-Bold="True"></HeaderStyle>
                        </telerik:RadGrid>
                    </div>
                </telerik:RadAjaxPanel>
            </div>
            <div class="clear"></div>
            <div id="footer">
                <asp:BulletedList Style="margin-left: 20px; padding-left: 0px" ID="FooterList" runat="server" BulletStyle="Numbered"></asp:BulletedList>
                <div style="height: 3px;"></div>
                <asp:Label ID="lblFooterLink" AssociatedControlID="FooterLink" runat="server" Text=""></asp:Label>
                <asp:HyperLink ID="FooterLink" CssClass="lane-link" runat="server" Target="_blank"></asp:HyperLink>
            </div>
            <div class="clear"></div>
            <div id="containerCopyright">
                <asp:Label ID="lblCopyrightYear" runat="server">Milliman PRM Analytics®</asp:Label>
            </div>
            <telerik:RadNotification RenderMode="Lightweight" ID="Toast" runat="server" VisibleOnPageLoad="false" Position="BottomRight"
                Width="330" Height="160" Animation="Slide" EnableRoundedCorners="true" EnableShadow="true"
                Title="Selections Restored" Text=""
                Style="z-index: 100000" AnimationDuration="5000" TitleIcon="">
            </telerik:RadNotification>
        </div>
    </form>
    <script type="text/javascript">

        function Ready() {LoadPageData();}

        function LoadPageData() {
            /*we have to check where the mouse is,  due to the implementation of some of the controls*/  
            /*we will get a mouseleave event, like when the search dropdown opens*/
            /*we need to check these to make sure the drop down is not active*/
            var AnalyzerSearchNode = $telerik.$($find("AnalyzerSearch").get_childListElement());
            var AssaySearchNode = $telerik.$($find("AssayDescriptionSearch").get_childListElement());
            var LocalitySearchNode = $telerik.$($find("LocalitySearch").get_childListElement());
            var CptCodeSearchNode = $telerik.$($find("CptCodeSearch").get_childListElement());

            if ((AnalyzerSearchNode.length == 0) && (AssaySearchNode.length == 0) && (LocalitySearchNode.length == 0) && (CptCodeSearchNode.length == 0)) {
                var currentLoadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                var currentGrid = $find("<%= RadGridPanel.ClientID%>")
                currentLoadingPanel.show(currentGrid);
                $find("<%= RadGridPanel.ClientID%>").ajaxRequestWithTarget("<%= RadGridPanel.UniqueID %>", "refresh");
            }
        }

        var PaddingForFooter = 5;  /*this is padding between bottom of grid and footer*/
        var GridDataSize = 0;  /*set on resize and then used to restore on virtual scroll*/
        function ResizeGrid() {
            var grid = $get("RatesGrid_GridData");
            var GridTop = $(grid).offset().top;
            var FooterTop = $(footer).offset().top;
            $(grid).height((FooterTop - GridTop) - PaddingForFooter);
            /*get the size of the data area*/
            var griddata = $get("RatesGrid_GridData");
            GridDataSize = Math.floor((FooterTop - $(griddata).offset().top) - PaddingForFooter);
        }

        /*restore the grid data to correct size*/
        function ResizeGridDataArea() {
            if (GridDataSize > 0) {
                var griddata = $get("RatesGrid_GridData");
                $(griddata).height(GridDataSize);
            }
        }

        function GridCreated(sender, args) 
        {
            var scrollArea = sender.GridDataDiv;
            var parent = $get("RatesGrid");            
            var gridHeader = sender.GridHeaderDiv;
            scrollArea.style.height = parent.clientHeight - gridHeader.clientHeight + "px";

            /*settings for grid margin for each browiser*/
            if ($telerik.isIE) 
            {
                $('.rgHeaderWrapper .rgHeaderDiv').addClass('rgHeaderDivForIE');
            }
            else if ($telerik.isSpartan) {
                $('.rgHeaderWrapper .rgHeaderDiv').addClass('rgHeaderDivForSpartan');
            }
            else if ($telerik.isFirefox)
            {
                $('.rgHeaderWrapper .rgHeaderDiv').addClass('rgHeaderDivForFireFox');
            }
            else if ($telerik.isChrome) {
                $('.rgHeaderWrapper .rgHeaderDiv').addClass('rgHeaderDivForChrome');
            }
        }

        /*dont allow a post back if nothing is selected*/
        function OnClientSearch(sender, args) {
            if (sender.get_text().length < 1) {sender._element.control._postBackOnSearch = false;}
        }

        /*resize the grid when window resizes*/
        $(window).resize(function () { ResizeGrid(); });
        $(document).ready(function () { ResizeGrid(); });

        /*this is ugly but keeps the grid the correct size*/
        setInterval(function () { ResizeGrid(); }, 333);

        /*close the menu item*/
        function closeWindow(divControl) {
            var Control = document.getElementById(divControl);
            LoadPageData();
            $(Control).toggle();
        }

        function showInformationWindow(PopUpWindowDiv) {
            var popUpControl = document.getElementById(PopUpWindowDiv);
            $(popUpControl).show();
            $(popUpControl).draggable({ cursor: "move" });
        }

        function hideInformationWindow(PopUpWindowDiv) {
            var popUpControl = document.getElementById(PopUpWindowDiv);
            $(popUpControl).hide();
        }

        var columnCellSelected = "";
        /*This event fires first*/
        function CellSelected(sender, args) {
            /*get column name*/
            columnCellSelected = args.get_column().get_uniqueName();
        }

        /*this event is fired right after the above*/
        function pnlRequestStarted(ajaxPanel, eventArgs) {
            try {
                if (eventArgs._eventTargetElement.control.UniqueID == "RatesGrid") {
                    if (columnCellSelected == "rate" || columnCellSelected == "notes") {                        
                        eventArgs.set_cancel(true)
                    }
                }
                columnCellSelected = "";
            }
            catch (ex) {
                alert("Error while data refresh: " + ex.message);
            }
        }

        function PrintRatesGrid() {
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
    </script>
</body>
</html>
