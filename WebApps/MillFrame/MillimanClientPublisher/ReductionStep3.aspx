<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReductionStep3.aspx.cs" Inherits="ClientPublisher.ReductionStep3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Publishing Project</title>
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
    div.progress {
        position: absolute;
        top: 0px;
        left: 0px;
        width: 550px;
        text-align:left;
    }

</style>
</head>
<body>
    <form id="form1" runat="server">
   <div class="CenterMeHorVer">
       <br />
    <center><asp:Image ID="Image1" runat="server" ImageUrl="~/images/Roundedblocks.gif" /><br />
    <asp:Label runat="server" ID="Status">Starting Project Update......</asp:Label>
        <div style="position:relative; margin-left:25px;" runat="server" id="ProgressBars" visible="false">
             <div class="progress"><asp:Image ID="GrayBar" runat="server" ImageUrl="~/images/graybar.png" Width="100%" style="opacity:0.5;"></asp:Image></div>
             <div class="progress"><asp:Image ID="Executing" runat="server" ImageUrl="~/images/greenbar.png" Width="80%" Height="15px"  ></asp:Image></div>
        </div>
        <asp:Label runat="server" ID="DetailedStatus" Visible="false"></asp:Label>
    </center><br /><br />
    </div>
    </form>
</body>
</html>
