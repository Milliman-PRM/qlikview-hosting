<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReviewApproval.aspx.cs" Inherits="ClientPublisher.ComplexReporting.ReviewApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <style type="text/css">
        .RadGrid
            {
                border-radius: 10px;
                overflow: hidden;
            }
    </style>
    <script type="text/javascript">
        function ApprovedChecked()
        {
            if (parent.Approved)
                parent.Approved(document.getElementById("Approved").checked); 
        }
     </script>
</head>
<body>
    <form id="form1" runat="server">
       <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />

        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        <br />
        <asp:CheckBox  ID="Approved" runat="server"  onclick="ApprovedChecked();"  Text="I have reviewed the status of each account and approve the results for publishing." />
    </form>
</body>
</html>
