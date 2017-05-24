<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReductionStep1.aspx.cs" Inherits="ClientPublisher.ReductionStep1"%>

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
             <div class="progress"><asp:Image ID="Finished" runat="server" ImageUrl="~/images/bluebar.png" Width="60%" Height="15px" ></asp:Image></div>
             <div class="progress"><asp:Image ID="Errored" runat="server" ImageUrl="~/images/redbar.png" Width="10%" Height="15px"  ></asp:Image></div>
        </div>
        <div style="position:relative; margin-left:25px;width:525px;text-align:left;font-size:x-small" runat="server" id="ProgressBarLegends" visible="False">
            <span style=" float:left"><asp:Image runat="server" ImageUrl="~/images/redbar.png" Width="10px" Height="10px"  ></asp:Image>&nbsp;<asp:Label ID="NumberFailed" runat="server" Text="0"></asp:Label>&nbsp;Failed&nbsp;&nbsp;
            <asp:Image runat="server" ImageUrl="~/images/bluebar.png" Width="10px" Height="10px"  ></asp:Image>&nbsp;<asp:Label ID="NumberCompleted" runat="server" Text="0"></asp:Label>&nbsp;Completed&nbsp;&nbsp;
            <asp:Image runat="server" ImageUrl="~/images/greenbar.png" Width="10px" Height="10px"  ></asp:Image>&nbsp;<asp:Label ID="NumberProcessing" runat="server" Text="0"></asp:Label>&nbsp;Processing &nbsp;&nbsp;
            <asp:Image runat="server" ImageUrl="~/images/graybar.png" Width="10px" Height="10px"  ></asp:Image>&nbsp;<asp:Label ID="NumberPending" runat="server" Text="0"></asp:Label>&nbsp;Pending 
            </span>
            <span style="float:right;margin-right:15px;margin-top:5px"><asp:Label ID="TotalUsers" runat="server" Text="Processing X Users" ></asp:Label></span>
            <div style="clear:both;"></div>
        </div>
    </center><br /><br />
    </div>
    </form>
</body>
</html>
