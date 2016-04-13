<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralIssue.aspx.cs" Inherits="ClientPublisher.HTML.GeneralIssue" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Milliman Health Care Intelligence</title>
    <link rel="Stylesheet" href="Css/Styles.css" />
</head>
<body style="background-color:white;background-image:url("~images/watermark.png");background-repeat:repeat">
    <form id="form1" runat="server">
        <div style="position:fixed;top:50%;left:50%;width:400px;height:230px;margin-top:-115px;margin-left:-200px;border:1px solid #999999">

                    <table style="padding:20px">
                        <tr>
                            <td style="height:30px;"></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:center;">
                                <asp:Label ID="ErrorMsg" runat="server" Text="An unspecified error condition has halted processing.  Please contact a system administrator."></asp:Label>
                            </td>
                        </tr>
                    </table>
  
       </div>
   </form>
</body>
</html>