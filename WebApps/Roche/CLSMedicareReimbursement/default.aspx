<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CLSMedicareReimbursement._default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script src="scripts/jquery-1.12.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        if (typeof jQuery == 'undefined') {
            alert("jQuery could not be loaded.....");
        }
    </script>

    <title>Clinical Lab Systems Medicare Reimbursement</title>
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW" />
    <style type="text/css">
        body {
            background-color: white;
            margin: 0px;
            overflow: hidden;
        }

        #header {
            height: 75px;
            color: black;
            text-align: center;
            padding: 5px;
            border-bottom: 3px solid #046EBC;
            font-family: Arial;
            font-size: 20px;
        }

        #footer {
            text-align: left;
            font-family: Arial;
            font-size: 12px;
            height: 133px;
            bottom: 0px;
            left: 48px;
            position: absolute;
            padding: 2px;
        }

        #menu {
            text-align: left;
            font-family: Arial;
            font-size: 12px;
            height: 400px;
            width: 1193px;
            top: 92px;
            left: 80px;
            position: absolute;
            visibility: visible;
            background-color: white;
            padding: 0px;
            margin: 0px;
            border: 3px solid #046EBC;
            z-index: 2000;
        }

        #RatesGrid_GridData {
            /*hide radgrid horizontal scroll*/
            overflow-x: hidden !important;
        }

        .RadSearchBox .rsbEmptyMessage {
            color: #046EBC !important;
            opacity: 1.0;
            font-weight: bolder !important;
            font-style: normal !important;
            /* remove chrome defalt textbox outline */
            outline: none;
        }

        #divMain {
            margin: 0px auto;
            padding: 6px;
            border-image: none;
            width: 95%;
            color: rgb(88, 88, 88);
            height: 890px;
        }

        .clear {
            height: 8px;
        }

        #gridContainer {
            width: 99%;
            height: 640px;
            padding: 3px 3px 3px 3px;
            margin: 0 auto;
        }

        .divAnalyzerCheckList {
            border: 1px solid black;
            color: white;
            overflow: hidden;
            background-color: #046EBC;
            position: absolute;
            top: 5px;
            left: 5px;
            height: 389px;
            width: 256px;
        }

        .divCB {
            overflow: scroll;
            height: 382px;
        }

        .divAssayDescriptionList {
            border: 1px solid black;
            overflow: hidden;
            background-color: #046EBC;
            color: white;
            position: absolute;
            top: 5px;
            left: 266px;
            height: 390px;
            width: 512px;
        }

        .divLocalityList {
            border: 1px solid black;
            overflow: hidden;
            background-color: #046EBC;
            color: white;
            position: absolute;
            top: 5px;
            left: 783px;
            height: 390px;
            width: 256px;
        }

        .divClearButton {
            overflow: hidden;
            color: white;
            position: absolute;
            top: 351px;
            left: 1048px;
            height: 43px;
            width: 136px;
        }

        .btnCustom {
            margin: 4px 3px 0px 6px;
            border: solid 1px #bbb;
            padding: 8px;
            -moz-border-radius: 3px;
            -webkit-border-radius: 3px;
            border-radius: 3px;
            -moz-box-shadow: 0px 0px 7px #bbb;
            -webkit-box-shadow: #bbb 0px 0px 7px;
            box-shadow: 0px 0px 7px #bbb;
            color: #444;
            font-size: 105%;
            float: left;
            background: #ddd;
            text-shadow: 1px 1px #bbb;
        }

            .btnCustom:hover {
                background-color: #ddd;
                color: #333;
                border: solid 1px #046ebc;
                cursor: pointer;
                outline: none;
            }

        /*this css will prevent thead rad grid column alignment issue*/
        .rgHeaderWrapper .rgHeaderDiv {
            margin-right: 16px !important;
        }

        /*P.S. To make the max-height cross-browser compatible, to set it as follows:*/
        .checkBoxList {
            max-height: 100px;
            height: auto !important;
            height: 100px;
        }

        .close
        {
            border: 1px solid #858585;
            border-radius: 8px 8px 8px 8px;
            cursor: pointer;
            float: right;
            height: 15px;
            margin: -16px -14px 6px 8px;
            padding: 0;
            width: 14px;
            -moz-border-radius: 8px;
            -webkit-border-radius: 8px;
            border-radius: 8px;
        }
        /*select{
          width: 150px;
          height: 30px;
          padding: 5px;
          background-color:#046EBC;
        }
        select option {
             color: green; 
             background-color:green;
        }
        select option:first-child{
          color: green;
        }*/
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


        <div id="divMain">
            <%--header section--%>
            <div id="header">
                <table style="width: 100%">
                    <tr>
                        <td style="vertical-align: bottom; padding-bottom: 5px">
                            <asp:ImageButton ID="LaunchMenu" runat="server" ImageUrl="~/Images/settings.png" OnClick="LaunchMenu_Click" /></td>
                        <td>
                            <h2>Clinical Lab Systems Medicare Reimbursement</h2>
                        </td>
                    </tr>
                </table>
            </div>

            <%--menu section--%>
            <div id="menu" runat="server" visible="false">
                <telerik:RadAjaxPanel ID="menu_controls_panel" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
                    <div class="divAnalyzerCheckList">
                        <telerik:RadSearchBox runat="server" ID="AnalyzerSearch" EmptyMessage="Analyzer" Width="100%" DataTextField="AnalyzerName" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="255px" OnSearch="AnalyzerSearch_Search" OnClientSearch="OnClientSearch" RenderMode="Lightweight">
                        </telerik:RadSearchBox>
                        <%--<div style="height: 2px;"></div>--%>
                        <div class="divCB">
                            <asp:CheckBoxList ID="AnalyzerCheckList" runat="server" BackColor="white" Width="100%" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AnalyzerCheckList_SelectedIndexChanged" ViewStateMode="Enabled"></asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="divAssayDescriptionList">
                        <telerik:RadSearchBox runat="server" ID="AssayDescriptionSearch" EmptyMessage="Assay Description" Width="100%" DataTextField="SearchDesc" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="511px" RenderMode="Lightweight" OnSearch="AssayDescriptionSearch_Search" OnClientSearch="OnClientSearch">
                        </telerik:RadSearchBox>
                      <%--  <div style="height: 2px;"></div>--%>
                        <asp:ListBox ID="AssayDescriptionList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="AssayDescriptionList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled" CssClass="mySelect"></asp:ListBox>
                    </div>
                    <div class="divLocalityList">
                        <telerik:RadSearchBox runat="server" ID="LocalitySearch" EmptyMessage="Locality" Width="100%" DataTextField="LocalityDescription" DataValueField="Id" DropDownSettings-Height="370px" DropDownSettings-Width="255px" RenderMode="Lightweight" OnSearch="LocalitySearch_Search" OnClientSearch="OnClientSearch">
                        </telerik:RadSearchBox>
                        <%--<div style="height: 2px;"></div>--%>
                        <asp:ListBox ID="LocalityList" runat="server" Width="100%" Height="375px" BackColor="White" ForeColor="Black" AutoPostBack="True" OnSelectedIndexChanged="LocalityList_SelectedIndexChanged" SelectionMode="Multiple" ViewStateMode="Enabled" CssClass="mySelect"></asp:ListBox>
                    </div>
                    <div class="divClearButton">
                        <asp:Button ID="btnClearLoad" runat="server" Text="Clear Selections" CssClass="btnCustom" Width="126px" ForeColor="Black" Font-Bold="true" BackColor="LightGray"
                            OnClick="btnClearLoad_Click" />
                    </div>
<%--                    <div id="divCloseMenu" style="display: none;cursor: move;" onclick="hidePopup('menu'); __doPostBack('divCloseMenu','')">
                        <img id="imgCancel" class="close" src="Images/cancel.png" alt="..."
                                onclick="document.getElementById('divHowScreenMsg').style.display = 'none';" />
                    </div>--%>
                </telerik:RadAjaxPanel>
            </div>

            <%--grid section--%>
            <div id="section">
                <div style="margin-right: 40px;">
                    <asp:DropDownList ID="YearDropdown" Style="float: right;" runat="server" Width="100px" AutoPostBack="True"
                        OnSelectedIndexChanged="YearDropdown_SelectedIndexChanged">
                    </asp:DropDownList>
                    <h2>Medicare Reimbursment Rates</h2>
                </div>
                <telerik:RadAjaxPanel ID="RadGridPanel" runat="server" OnAjaxRequest="RadGridPanel_AjaxRequest" LoadingPanelID="RadAjaxLoadingPanel1">
                    <div id="gridContainer">
                        <telerik:RadGrid RenderMode="Classic" ID="RatesGrid" runat="server" GridLines="None"
                            AllowSorting="true" AllowPaging="true" PageSize="250"
                            AllowCustomPaging="true" OnNeedDataSource="RatesGrid_NeedDataSource"
                            PagerStyle-ShowPagerText="True" PagerStyle-Visible="True"
                            OnSortCommand="RatesGrid_SortCommand" ClientIDMode="AutoID"
                            AutoGenerateColumns="False" OnSelectedCellChanged="RatesGrid_SelectedCellChanged">
                            <PagerStyle Visible="false" />
                            <ClientSettings ReorderColumnsOnClient="false" AllowColumnsReorder="false" EnablePostBackOnRowClick="True">
                                <ClientEvents OnGridCreated="GridCreated" />
                                <Virtualization EnableVirtualization="false" InitiallyCachedItemsCount="2000" LoadingPanelID="RadAjaxLoadingPanel1" ItemsPerView="500" />
                                <Selecting CellSelectionMode="SingleCell" />
                                <Scrolling ScrollHeight="480px" AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" EnableVirtualScrollPaging="true"></Scrolling>
                                <Resizing AllowColumnResize="True" ClipCellContentOnResize="true" EnableRealTimeResize="true" ResizeGridOnColumnResize="true" />
                            </ClientSettings>

                            <MasterTableView AllowMultiColumnSorting="false" PagerStyle-AlwaysVisible="True" Width="100%" TableLayout="Fixed">
                                <Columns>
                                    <telerik:GridBoundColumn UniqueName="analyzer_name" DataField="analyzer_name" HeaderText="Analyzer" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="20%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="description" DataField="description" HeaderText="Assay Description" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="35%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="code" DataField="code" HeaderText="CPT Code" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="10%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="notes" DataField="notes" HeaderText="Notes" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="7%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="locality_description" DataField="locality_description" HeaderText="Locality" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="12%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn UniqueName="rate" DataField="rate" HeaderText="Medicare Reimbursement Rate" DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" ReadOnly="True" SortedBackColor="Transparent">
                                        <HeaderStyle Width="15%" HorizontalAlign="Right" />
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

            <telerik:RadNotification RenderMode="Lightweight" ID="Toast" runat="server" VisibleOnPageLoad="false" Position="BottomRight"
                Width="330" Height="160" Animation="Slide" EnableRoundedCorners="true" EnableShadow="true"
                Title="Selections Restored" Text=""
                Style="z-index: 100000" AnimationDuration="5000" TitleIcon="">
            </telerik:RadNotification>
        </div>
    </form>
    <script type="text/javascript">
        //must hide menu via JQuery, or it may not un-hide when needed
        // $(menu).hide();

        //CHange the highlight color
        $('#AssayDescriptionList').click(function () {
            $("#AssayDescriptionList option:selected").css("background-color", "#046EBC");
        });

        $('#LocalityList').click(function () {
            $("#LocalityList option:selected").css("background-color", "#046EBC");
        });

        var sel = document.getElementById('AssayDescriptionList');
        sel.addEventListener('click', function (el) {
            var options = this.children;
            var selected = this.children[this.selectedIndex];
            selected.style.backgroundColor = 'yellow';
        }, false);

        var sel = document.getElementById('LocalityList');
        sel.addEventListener('click', function (el) {
            var options = this.children;
            var selected = this.children[this.selectedIndex];
            selected.style.backgroundColor = 'yellow';
        }, false);

        function MenuEvents() {
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

                })
            }
        }

        var PaddingForFooter = 5;  //this is padding between bottom of grid and footer
        var GridDataSize = 0;  //set on resize and then used to restore on virtual scroll
        function ResizeGrid() {
            var grid = $get("RatesGrid_GridData");
            var GridTop = $(grid).offset().top;
            var FooterTop = $(footer).offset().top;
            $(grid).height((FooterTop - GridTop) - PaddingForFooter);
            //get the size of the data area
            var griddata = $get("RatesGrid_GridData");
            GridDataSize = Math.floor((FooterTop - $(griddata).offset().top) - PaddingForFooter);
        }

        //restore the grid data to correct size
        function ResizeGridDataArea() {
            if (GridDataSize > 0) {
                var griddata = $get("RatesGrid_GridData");
                $(griddata).height(GridDataSize);
            }
        }

        function GridCreated(sender, args) {
            //debugger;
            var scrollArea = sender.GridDataDiv;
            var parent = $get("RatesGrid");
            //alert(parent.clientHeight)
            var gridHeader = sender.GridHeaderDiv;
            scrollArea.style.height = parent.clientHeight - gridHeader.clientHeight + "px";
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

        //dont allow a post back if nothing is selected
        function OnClientSearch(sender, args) {
            if (sender.get_text().length < 1) {
                sender._element.control._postBackOnSearch = false;
            }
        }

        function Ready() {
            MenuEvents();
            //for menu items, scroll selections into view
            //ScrollSelectionsIntoViewCheckBoxList('<%= AnalyzerCheckList.ClientID %>');  //check box list does not function same as listbox, needs research....
            //scrollIntoView('<%= AnalyzerCheckList.ClientID %>', "#divAnalyzerCheckBox");
            ScrollSelectionsIntoView('<%= AssayDescriptionList.ClientID %>');
            ScrollSelectionsIntoView('<%= LocalityList.ClientID %>');
        }

        //resize the grid when window resizes
        $(window).resize(function () { ResizeGrid(); });
        $(document).ready(function () { ResizeGrid(); });

        //this is ugly but keeps the grid the correct size
        setInterval(function () { ResizeGrid(); }, 333);

        //this function will scroll the listbox selections into view automatically
        function ScrollSelectionsIntoView(ControlID) {
            var listbox = document.getElementById(ControlID);
            if (listbox != null) {
                for (var i = 0; i < listbox.options.length; i++) {
                    if (listbox.options[i].selected) {
                        listbox.options[i].selected = false;
                        listbox.options[i].selected = true;
                    }
                }
            }
        }

        ////this function will scroll the listbox selections into view automatically
        //function ScrollSelectionsIntoViewCheckBoxList(ControlID) {
        //    debugger;
        //    var listbox = document.getElementById(ControlID);
        //    if ($('#AnalyzerCheckList :checkbox:checked').length > 0) {
        //        listbox.scrollIntoView(false);
        //    }
        //}


        //function scrollIntoView(element, container) {
        //    debugger;
        //    var containerTop = $(container).scrollTop();
        //    var containerBottom = containerTop + $(container).height();
        //    var elemTop = element.offsetTop;
        //    var elemBottom = elemTop + $(element).height();
        //    if (elemTop < containerTop) {
        //        $(container).scrollTop(elemTop);
        //    } else if (elemBottom > containerBottom) {
        //        $(container).scrollTop(elemBottom - $(container).height());
        //    }
        //}

        <%--function clearAll() {
            debugger;
            //clear test boxes
            var searchBoxAnalyzerSearchinputBox = $find("<%=AnalyzerSearch.ClientID%>").get_inputElement();
            searchBoxAnalyzerSearchinputBox.value = '';

            var searchBoxAssayDescriptionSearchinputBox = $find("<%=AssayDescriptionSearch.ClientID%>").get_inputElement();
            searchBoxAssayDescriptionSearchinputBox.value = '';

            var searchBoxLocalitySearchinputBox = $find("<%= LocalitySearch.ClientID %>").get_inputElement();
            searchBoxLocalitySearchinputBox.value = '';

            //$('#AnalyzerCheckList:checked').removeAttr('checked');

            //if ($('#AnalyzerCheckList :checkbox:checked').length > 0) {
            //    $('#AnalyzerCheckList').attr('checked', 'unchecked');
            //    //OR
            //    $('#AnalyzerCheckList').prop('checked', false);
            //}

            $('#AnalyzerCheckList').attr('checked', false); // Unchecks it
            $('#myCheckbox').prop('checked', false); // Unchecks it

            // un select the element:
            $("#AssayDescriptionList")[0].selectedIndex = -1;

            // un select the element:
            $("#LocalityList")[0].selectedIndex = -1;

            //initiate postback to refresh
            __doPostBack('YearDropdown', '');

            return false;
        }--%>

        //function hidePopup(divControl) {
        //    var Control = document.getElementById(divControl);
        //    //Slowly hide the Div
        //    //Remove the div by slightly moving the div towards left
        //    $(Control).hide(2000, function () {
        //        $(Control).remove();
        //    });
        //}
    </script>
</body>
</html>
