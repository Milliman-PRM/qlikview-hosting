<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordExpired.aspx.cs" Inherits="MillimanDev2.PasswordExpired" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <meta name="robots" content="noindex" />
    <title>Milliman Health Care Intelligence</title>
    <link rel="Stylesheet" href="Css/Styles.css" />
    <style type="text/css">
        .MenuWidth {
            width: 130px;
            text-align:center;
            font-size:11px;
            font-family: "Arial";
            font-weight:400;
            color:gray;
        }
        .Col1Style {
            width: 405px;
        }
        a:link {
            color: gray;
            text-decoration:none;
            font-size:inherit;
            vertical-align:middle;
        }
        .auto-style3 {
            height: 334px;
        }
        .auto-style5 {
            width: 700px;
        }
    </style>

 <%--  <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
      <script type="text/javascript" src="javascript/jquery.min.js"></script>
<script src="javascript/WaterMark.min.js" type="text/javascript"></script>

</head>
<body style="background-color:white;">
   

    <form id="myform" runat="server"  >
        <div class="logincontent">
            <table style="border-collapse:collapse;background-color:#3F3F3F;border-color:#3F3F3F;">
                <tr style="background-color:white">
                    <td style="text-align:left;border-right:1px solid #999999" class="Col1Style" colspan="5">
                        <img src="Images/PRMLogo_height80.png" alt="Milliman Logo" />
                    </td>
                </tr>
                <tr style="width:100%;height:20px;background-color:white">
                    <td class="Col1Style" >
                        &nbsp;
                    </td>
                    <td class="MenuWidth">
                       <a href="http://www.milliman.com" target="_blank">About Milliman</a>
                    </td>
                    <td class="MenuWidth">
                        <a href="http://us.milliman.com/legal/terms/" target="_blank">Terms of use</a>
                    </td>
                    <td class="MenuWidth">
                        <a href="http://us.milliman.com/legal/privacy/" target="_blank">Privacy policy</a>
                    </td>
                    <td class="MenuWidth" style="border-right:1px solid #999999">
                        <a href="http://us.milliman.com/legal/copyright/" target="_blank">Copyright</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="flex-item-align:center;background-color:#3F3F3F" class="auto-style3">
                        <img src="Images/New_LoginSplash.png" alt="Milliman" />
                    </td>
                </tr>

                <tr>
                    <td colspan="5">
                    <table width="100%" style="border-collapse:collapse;background-color:#3F3F3F;border-width:0px;border-color:#3F3F3F;">
                        <tr>
                            <td style="text-align:left;color:whitesmoke;white-space:nowrap" class="auto-style5" >
                               <asp:Label runat="server" ID="lblNoticeText">Your password was last changed more than xx days ago and is expired.  Please click the &quot;Next&quot; button to reset your password.</asp:Label>
                            </td>
                            <td style="text-align:right;padding-top:3px;padding-right:11px">
                                <asp:ImageButton runat="server" ImageUrl="~/Images/next.png" ID="Next" OnClick="Next_Click"  />
                            </td>
                        </tr>
                    </table>
                    </td>
                </tr>
            </table>
    </form>
</body>
</html>
