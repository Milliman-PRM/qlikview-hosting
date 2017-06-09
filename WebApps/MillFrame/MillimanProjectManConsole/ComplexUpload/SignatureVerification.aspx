<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignatureVerification.aspx.cs" Inherits="MillimanProjectManConsole.ComplexUpload.SignatureVerification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title>Signature Verification</title>
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
</style>
</head>
<body>
    <form id="form1" runat="server">
   <div class="CenterMeHorVer">
       <br />
   <center><asp:Image ID="Image1" runat="server" ImageUrl="~/images/Roundedblocks.gif" /></center><br />
    <asp:Label runat="server" ID="SignatureVerifications">Starting verification......</asp:Label><br /><br />
    <center>
        <asp:Button ID="RenameGroup" runat="server" Text="Rename Group" Visible="false" OnClick="RenameGroup_Click"></asp:Button>
        <asp:Button ID="PublishContent" runat="server" Text="Publish Content" Visible="false" OnClick="PublishContent_Click"></asp:Button>
    </center>
    </div>
    </form>
</body>
</html>
