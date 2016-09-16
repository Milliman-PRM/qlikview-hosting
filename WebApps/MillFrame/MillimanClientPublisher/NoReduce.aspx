<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoReduce.aspx.cs" Inherits="ClientPublisher.NoReduce"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title>Project Update</title>
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
        text-align:center;
    }

        body {
            background:url("../images/watermark.png");
        }
</style>
</head>
<body>
    <form id="form1" runat="server">
   <div class="CenterMeHorVer">
       <br />
    <center><asp:Image ID="Image1" runat="server" ImageUrl="~/images/Roundedblocks.gif" /></center><br />
    <asp:Label runat="server" ID="Status">Starting Project Update......</asp:Label><br /><br />
    </div>
    </form>
</body>
</html>
