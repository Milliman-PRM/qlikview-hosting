<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QVWReloadError.aspx.cs" Inherits="MillimanProjectManConsole.Admin.QVWReloadError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <title>QVW Reload Assertion</title>
    <style type="text/css">
    .CenterMeHorVer{
	    width:600px;
	    height:150px;
	    position:absolute;
	    left:50%;
	    top:50%;
	    margin:-75px 0 0 -300px;
        background-color:white;
        font-size:14px;
        border:1px solid black;
    }
</style>

</head>
<body>
   <form id="form1" runat="server" style="text-align:center">
        <div class="CenterMeHorVer"> 
            <asp:Label runat="server" ID="errormsg" ></asp:Label>
        </div>
    </form>

    
</body></html>
