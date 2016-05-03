<%@ Page language="c#" Codebehind="SqlHistory.aspx.cs" AutoEventWireup="false" Inherits="WebSqlUtility.SqlHistory" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
<head>
<title>SQL History</title>
<style>
body, td, select, input, textarea, p { font:11px verdana; }
body { margin:10px }
a { color:blue; text-decoration:none; }
h3 { margin-bottom:4 }
a.delLink { color:red; font-size:12px; font-weight:normal; text-decoration:none; }
</style>
<script>
function confirmDelete(file)
{
	var msg = "Delete history file?      "
	if (file == "*")
		msg = "Delete *ALL* history files?      "

	if (confirm(msg))
		location.href = "SqlHistory.aspx?del=" + file
}
</script>
</head>
<body onload=focus()>


<asp:Repeater Runat=server ID=RepFullHistory>
	<ItemTemplate>
	
		
		<h3><%# GetDate((string)Container.DataItem) %> &nbsp;&nbsp;<a class=delLink href=# onclick="confirmDelete('<%# System.IO.Path.GetFileName((string)Container.DataItem) %>'); return false"><img src="images/remove.png" style="border:none;vertical-align:middle" /></a></h3>

		<asp:Repeater Runat=server ID=RepHistory>
			<HeaderTemplate>
				<div style="border:1px solid gray; border-bottom-width:0; background:#fafafa;">
			</HeaderTemplate>
			<ItemTemplate>
				<div style="padding:4 4 7 4; background-color:#eee; border-bottom:1px solid #999">
					<a href="#" onclick="window.opener.focus(); window.opener.setQuery(this); window.opener.setDB('<%# DataBinder.Eval(Container.DataItem, "DB") %>'); return false;"><%# DataBinder.Eval(Container.DataItem, "Query") %></a>
				</div>
			</ItemTemplate>
			<FooterTemplate>
					<div id=divPrevHist style="display:none; margin:4px; font-size:11px">&#187; <a href=#>Complete History</a></div>
				</div>
			</FooterTemplate>
		</asp:Repeater>
		

	</ItemTemplate>
</asp:Repeater>

<br><br>

<a class=delLink href=# onclick="confirmDelete('*'); return false"><img src="images/remove.png" style="border:none;vertical-align:middle" /> Delete All</a>


<br><br>


</body>
</html>
