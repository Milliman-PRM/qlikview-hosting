<%@ Page language="c#" Codebehind="SqlBrowser.aspx.cs" AutoEventWireup="false" Inherits="WebSqlUtility.Sql" ValidateRequest="False" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxcontroltoolkit" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head >
<title>Web SQL Utility</title>
<script language="javascript" type="text/javascript" src="script/DbUtil.js"></script>
<style type="text/css">
body, td, select, input, textarea, p { font:11px verdana; overflow:hidden; }
a { color:blue; text-decoration:none; }
a:hover { color:cc0000;  text-decoration:underline; }
/*div.grdRowCount { color:gray; margin-top:2px; margin-bottom:10px; font-size:10px;  }*/


.RadGrid td 
{ 
    padding-top:0; 
    padding-bottom:0; 
    height:20px; 
    vertical-align:middle; 
} 


/*.templatecontent
{
	margin:auto;
	width: 98%;
	background-color: White;
	border: #999999 1px solid;
	padding: 5px;
    height: 360px;
    overflow:hidden;
}

.header
{
	padding-bottom: 10px;
	border-bottom: #999999 1px solid;
	margin-bottom: 10px;
}

  tr.ForumTableHeader
    {
    text-align: center;
    font-weight: bold;
    background-image: url(images/header.gif);
    background-color:Red;
    }*/


</style>
</head>
<body style="margin:10px;background-color:white;" >


<%--<div style="padding:18px 10px 10px 10px;" align=left>--%>

	<form id="Form1" method="post" runat="server">

        <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
        </telerik:RadStyleSheetManager>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
                </asp:ScriptReference>
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>

    <div class="templatecontent" style="overflow:hidden">
            <div class="header">
                <table style="width:100%">
                    <tr >

                         <td style="width:290px;height:80px;overflow:hidden">
                            <a href="http://www.milliman.com"><img src="images/New_MillimanHeader.png" style="border:none;width:75%;height:75%"/></a>
                        </td>
                       <td  valign="middle" align="right" >
                              <b>Connected to: <asp:Label ID="DatabaseName" runat=server ></asp:Label></b> &nbsp&nbsp  
                       </td>
                    </tr>
                    <tr>
                        <td style="width:240px" valign="middle"><%= DateTime.Now.ToLongDateString() %></td>
                        <td  valign="middle" align="right" >
                             <b>Welcome! <asp:Label ID="UserNameLabel" runat=server ></asp:Label></b> &nbsp&nbsp  
                        </td>
                    </tr>
                </table>
            </div>

		<table cellpadding=0 cellspacing=5 style="width:100%">
		<tr >
		    <td valign=bottom  nowrap="nowrap" width="320px" title="Tables and fields from the database schema.">
                <b>Table View</b> &nbsp;&nbsp;
			 <%--   <asp:HyperLink ID="SchemaDiagram" runat="server" Target="_blank">Schema Diagram</asp:HyperLink>--%>
<%--             <a href=# onclick="showVisualSchema(); return false" title="Click to see a database diagram of the current schema.">Schema Diagram</a>--%>
                &nbsp;&nbsp;&nbsp;&nbsp;
			    <a href=# onclick="showSchema('SQL Server'); return false" title="Click to review details of the current schema.">Schema Details</a>
		    </td>
             <td title="Enter a query to execute against the database." nowrap="nowrap">
                 <b>Current Query(T-SQL syntax)</b>
		    </td>
		    <td>
                &nbsp;
                <asp:HiddenField ID="VisualSchemaKey" runat="server" />
		    </td>
            <td valign=bottom style="width:400px" title="Previous queries issued." nowrap="nowrap">
                <b>Query History</b>
   		    </td>
		    <td>&nbsp;</td>
		</tr>

		<tr>
        
        <td > <telerik:RadTreeView style="height:120px;overflow:auto" ID="RadTreeView1" runat="server" BorderColor="Black" BorderWidth="1" BorderStyle="Solid"></telerik:RadTreeView></td>
		<td colspan="2" valign=top>
		
			<asp:TextBox ID=TxtQuery Runat=server TextMode=MultiLine Width=100% Height="120px" TabIndex=1 Text="" ToolTip="Enter a query to execute against the database." /><br>

		</td>

		<td colspan="2" valign=top style="padding-top:1px;">
            <%--<asp:ListBox ID="HistoryListbox" runat="server" Width="100%" Height="120px" ></asp:ListBox>--%>
            <asp:Repeater Runat=server ID=RepHistory >
				<HeaderTemplate>
					<div id=divHistoryQueries style="height:120px; display:block; overflow:auto; border:1px solid gray; background:#fafafa;">
				</HeaderTemplate>
				<ItemTemplate>
					<div style="display:block;padding:4 4 7 4; background-color:<%# (IsOddRow()) ? "#EEEFF3" : "#F4F5F8" %>; border-bottom:1px solid #999;">
						<a href="#" style="visibility:visible;display:block" onclick="setQuery(this); setDB('<%# DataBinder.Eval(Container.DataItem, "DB") %>'); return false;"><%# DataBinder.Eval(Container.DataItem, "Query") %></a>
					</div>
				</ItemTemplate>
				<FooterTemplate>
						<div id=divPrevHist style="display:block; margin:4px; font-size:11px">&#187; <a href=# onclick="openFullHistory(); return false">Complete History</a></div>
					</div>
				</FooterTemplate>
			</asp:Repeater>
		</td>
        <td>&nbsp;</td>
		</tr>

		<tr>
		<td>
           &nbsp;

		</td>
         <td>
             <center>
            <table cellpadding=0 cellspacing=5>
                <tr>
  
                    <td><asp:Button ID=BtnExecute Runat=server Text="Query Preview" AccessKey=e OnClick=ExecuteSQL TabIndex=1 OnClientClick="Busy();return true;" ToolTip="Execute a query and view the results in this webpage." /></td>
                    <td><asp:Button Runat=server Text="Query to XML" ID=BtnDownloadXml OnClick=Download TabIndex=4 OnClientClick="Busy();return true;" ToolTip="Execute a query and save the results as XML" /></td>
                    <td><asp:Button Runat=server Text="Query to CSV" ID=BtnDownloadCSV OnClick=Download TabIndex=5 OnClientClick="Busy();return true;" ToolTip="Execute a query and save the results as comma seperated values." /></td>
                    <td><asp:Button Runat=server   Text="Query to Excel" ID=BtnDownloadExcel OnClick=Download TabIndex=5 OnClientClick="Busy();return true;" ToolTip="Execute a query and push the results into MS Excel." /></td>
                </tr>
            </table>
            </center>
		</td>
            <td>&nbsp;</td>
            <td align="left">
		</td>
            <td>
			
		</td>
		</tr>
		
        <tr>
            <td>
		    <%-- dummy control required so asp.net outputs __dopostback js --%>
		<%--    <asp:DropDownList AutoPostBack=True Runat=server style="display:none" />
            <asp:TextBox Runat=server ID=TxtQueries TextMode=MultiLine Width=100% Height=180 Visible=False Wrap=False />--%>
		   
            </td>
		</tr>
            <tr>
                <td colspan="4">
                    <%--<asp:Literal ID=LitStatus Runat=server EnableViewState=False />--%>
                </td>
        </tr>
		<tr>
            <td colspan="4" >

  <%--              <div style="overflow:auto;background-color:lightgoldenrodyellow;position:absolute;top:300px;bottom:30px;">
			    <asp:PlaceHolder Runat=server ID=PlcResultGrids />

                </div>--%>
            </td>
        </tr>
        </table>
</div>
        <div ID="GridDiv" style="position:absolute;right:10px;left:10px;bottom:10px;top:300px;">
            <asp:PlaceHolder ID="IFrame" runat="server"></asp:PlaceHolder>
        </div>
        <div id="busy" style="position:absolute;top:50%;bottom:50%;left:50%;right:50%;visibility:visible;z-index:1000">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Roundedblocks.gif" />
        </div>
         <div ID="DownloaderDiv" style="position:absolute;top:0px;left:0px;width:30px;height:30px;visibility:hidden;">
            <asp:PlaceHolder ID="DownloaderIFrame" runat="server"></asp:PlaceHolder>
             </div>
	</form>
    
<script type="text/javascript">
    function Busy() {
        var BusyDiv = document.getElementById("busy");
        BusyDiv.style.visibility = "visible";
    }
    function NotBusy() {
        var BusyDiv = document.getElementById("busy");
        BusyDiv.style.visibility = "hidden";
    }

    function getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    }

    function CheckDownloadStatus() {
        var DownloadCompleted = getCookie("downloadstatus");
        if (DownloadCompleted != "") {
            NotBusy();
            document.cookie = "downloadstatus=; expires=Thu, 01 Jan 1970 00:00:00 GMT";
        }

    }
    window.setInterval(function () { CheckDownloadStatus(); }, 1000);
</script>

</body>
</html>
