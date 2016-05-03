<%@ Page language="c#" Codebehind="SqlSchema.aspx.cs" AutoEventWireup="false" Inherits="WebSqlUtility.SqlSchema" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
<head>
<title>SQL Schema</title>
<style>
body, td, select, input, button, textarea, p { font:11px verdana; }
h2 { font-size:16px; font-weight:bold; margin-bottom:6px; color:navy; }
a { color:blue; text-decoration:none; }
a:hover { color:cc0000;  text-decoration:underline; }
span.insertLink { cursor:hand; font-weight:bold; }
span.insertLinkHover { cursor:hand; font-weight:bold; color:#cc0000 }
a.topLink { font-size:10px; font-weight:normal; }


    tr.ForumTableHeader
    {
    text-align: center;
    font-weight: bold;
    background-image: url(images/header.gif);
    background-color:Red;
    }


</style>
<script>
function insertSql(o)
{
	var queryTextArea = window.opener.document.getElementById("TxtQuery")

	window.opener.focus()
	queryTextArea.focus()
			
	if (document.all) // IE
		window.opener.document.selection.createRange().text = o.innerHTML
	else // FF
	{
		var selStart = queryTextArea.selectionStart
		queryTextArea.value = queryTextArea.value.substring(0, selStart) + o.innerHTML + queryTextArea.value.substring(queryTextArea.selectionEnd)
		queryTextArea.selectionStart = selStart + o.innerHTML.length
		queryTextArea.selectionEnd = queryTextArea.selectionStart
	}
}
</script>
</head>
<body>

<form id="DbSchema" method="post" runat="server">


	<asp:PlaceHolder Runat=server ID=PlcGrids />


	<br><br><br><br>


</form>

</body>
</html>
