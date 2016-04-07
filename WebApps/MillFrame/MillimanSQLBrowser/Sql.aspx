<%@ Page language="c#" Codebehind="Sql.aspx.cs" AutoEventWireup="false" Inherits="WebSqlUtility.SqlBrowser" ValidateRequest="False" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxcontroltoolkit" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head >
<title>Web SQL Utility</title>
<script language=javascript src=script/DbUtil.js></script>
<style>
body, td, select, input, textarea, p { font:11px verdana; overflow:hidden; }
a { color:blue; text-decoration:none; }
a:hover { color:cc0000;  text-decoration:underline; }
div.grdRowCount { color:gray; margin-top:2px; margin-bottom:10px; font-size:10px;  }

.templatecontent
{
	margin:auto;
	width: 98%;
	background-color: White;
	/*border: #999999 1px solid;*/
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
    }

</style>
</head>
<body>


<%--<div style="padding:18px 10px 10px 10px;" align=left>--%>

	<form id="Form1" method="post" runat="server">

    <div class="templatecontent">
            <div class="header">
                <table style="width:100%">
                    <tr >

                         <td style="width:240px">
                            <a href="http://www.milliman.com"><img src="s" style="border:none;width:75%;height:75%"/></a>
                        </td>
                       <td  valign="middle" align="right" >
                             <b>Welcome! <asp:Label ID="UserNameLabel" runat=server ></asp:Label></b> &nbsp&nbsp  
                        </td>
                    </tr>
                    <tr>
                        <td style="width:240px" valign="middle"><%= DateTime.Now.ToLongDateString() %></td>
                        <td  valign="middle" align="right" >
                             <b>Connected to: <asp:Label ID="DatabaseName" runat=server ></asp:Label></b> &nbsp&nbsp  
                        </td>
                    </tr>
                </table>
            </div>

		<table cellpadding=0 cellspacing=0 style="width:100%">
		<tr>
		    <td valign=bottom>
	    <%--		<asp:RadioButtonList Runat=server ID=RblWhichDb RepeatDirection=Vertical CellPadding=0 CellSpacing=0 />--%>
			    <a href=# onclick="showSchema('SQL Server'); return false">SQL Server(TSQL) - Show Schema Details</a>
		    </td>
             <td valign=bottom unselectable=on>
			    <asp:CheckBox Runat=server ID=CbxHtmlEncode Text="<span unselectable=on>HTML encode results</span>" Checked=True Visible="false" /><br>
			    <asp:CheckBox Runat=server ID=CbxWrap Text="<span unselectable=on>Wrap query textbox</span>" Checked=True onclick="toggleWrap(this.checked)"  Visible="false" />
		    </td>
		    <td>&nbsp;
		    </td>
            <td valign=bottom style="width:400px">
			    <a href=# onclick="showPanel(1); return false" id=aHist unselectable=on>History</a>
			    &nbsp;&nbsp;&nbsp;
			    <a href=# onclick="showPanel(2); return false" id=aFav style="font-weight:bold" unselectable=on>Favorites</a>
    		</td>
		    <td>&nbsp;</td>

		</tr>
<%--		</table>


		<table width=98% border=0 cellpadding=0 cellspacing=0>--%>
		<tr>
		<td colspan="2" valign=top>
		
			<asp:TextBox ID=TxtQuery Runat=server TextMode=MultiLine Width=100% Height=120 Columns=80 Rows=10 TabIndex=1 /><br>

		</td>
		<td width=1>&nbsp;</td>
		<td colspan="2" valign=top style="padding-top:1px;">

			<asp:Repeater Runat=server ID=RepHistory>
				<HeaderTemplate>
					<div id=divHistoryQueries style="height:120px; display:none; overflow:auto; border:1px solid gray; background:#fafafa;">
				</HeaderTemplate>
				<ItemTemplate>
					<div style="padding:4 4 7 4; background-color:<%# (IsOddRow()) ? "#EEEFF3" : "#F4F5F8" %>; border-bottom:1px solid #999">
						<a href="#" onclick="setQuery(this); setDB('<%# DataBinder.Eval(Container.DataItem, "DB") %>'); return false;"><%# DataBinder.Eval(Container.DataItem, "Query") %></a>
					</div>
				</ItemTemplate>
				<FooterTemplate>
						<div id=divPrevHist style="display:none; margin:4px; font-size:11px">&#187; <a href=# onclick="openFullHistory(); return false">Complete History</a></div>
					</div>
				</FooterTemplate>
			</asp:Repeater>

			
			<asp:Repeater Runat=server ID=RepFavourites>
				<HeaderTemplate>
					<div id=divFavQueries style="height:120px; overflow:auto; border:1px solid gray; background:#fafafa;">
				</HeaderTemplate>
				<ItemTemplate>
					<div style="padding:4 4 7 4; background-color:<%# (IsOddRow()) ? "#EEEFF3" : "#F4F5F8" %>; border-bottom:1px solid #999">  
						<a href="#" onclick="setQuery(this); setDB('<%# DataBinder.Eval(Container.DataItem, "DB") %>'); return false;"><%# DataBinder.Eval(Container.DataItem, "Query") %></a>
						&nbsp;<a href=# onclick="if (confirm('Delete favourite?   ')) __doPostBack('RemoveFav', '<%# GetRowIndex() %>')" style="color:#cc0000"><img src="images/remove.png" style="border:none;vertical-align:middle" /></a>
					</div>
				</ItemTemplate>
				<FooterTemplate>
					</div>
				</FooterTemplate>
			</asp:Repeater>
   
			<% if (RepHistory.Items.Count > 0) { %><script>showPanel(1)</script><% } %>
			<% if (HistoryFileCount > 0) { %><script>document.getElementById("divPrevHist").style.display="block"</script><% } %>  
		</td>
        <td>&nbsp;</td>
		</tr>
<%--		</table>

 


		<table cellpadding=0 cellspacing=0 style="margin-top:4px">--%>
		<tr>
		<td>
            <table cellpadding=0 cellspacing=5>
                <tr>
                    <td><asp:Button ID=BtnExecute Runat=server Text="Execute Query" AccessKey=e OnClick=ExecuteSQL TabIndex=1 /></td>
                    <td><asp:CheckBox Runat=server ID=CbxAddFav EnableViewState=False TabIndex=2 /></td>
                    <td><label for=CbxAddFav>Add to favorites</label></td>
                    <td><span style=color:#555555>***Separate multiple queries with "<%= conf.QuerySeparator %>"</span><br></td>
                </tr>
            </table>

			
		</td>
         <td>
            &nbsp;
		</td>
            <td>&nbsp;</td>
            <td align="left">

<%--			<asp:Button Runat=server Text="INSERTs" OnClick=GetQueries TabIndex=3 Visible="false"/>--%>
 <%--               <table cellpadding=5 cellspacing=5  width="400px" >
                    <tr align="right">
                        <td align="right"><asp:Button Runat=server Text="Save Results as XML" ID=x OnClick=Download TabIndex=4 /></td>
                        <td align="right"><asp:Button Runat=server Text="Save Results as CSV" ID=e OnClick=Download TabIndex=5 /></td>
                    </tr>
                </table>--%>
		</td>
            <td>
			
		</td>
		</tr>
		
        <tr>
            <td>
		    <%-- dummy control required so asp.net outputs __dopostback js --%>
		    <asp:DropDownList AutoPostBack=True Runat=server style="display:none" />
            <asp:TextBox Runat=server ID=TxtQueries TextMode=MultiLine Width=100% Height=180 Visible=False Wrap=False />
		   
            </td>
		</tr>
            <tr>
                <td colspan="4">
                    <asp:Literal ID=LitStatus Runat=server EnableViewState=False />
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

       <div style="overflow:auto;position:absolute;top:290px;bottom:43px;left:5px;width:98%;border:1px solid lightgray">
			    <asp:PlaceHolder Runat=server ID=PlcResultGrids />
        </div>
        <div style="height:40px;bottom:0;margin:auto;position:absolute;left:5px;width:98%">
             <table >
                    <tr >
                        <td style="width:100%">  Copyright &copy Milliman 2013</td>
                        <td><asp:Button Runat=server Text="Save Results as XML" ID=BtnDownloadXml OnClick=Download TabIndex=4 /></td>
                        <td><asp:Button Runat=server Text="Save Results as CSV" ID=BtnDownloadCSV OnClick=Download TabIndex=5 /></td>
                    </tr>
              </table>
            
        </div>
  
	</form>
<%--    </div>--%>
    
</body>
</html>
