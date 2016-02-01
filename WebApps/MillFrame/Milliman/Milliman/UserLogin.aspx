<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="MillimanDev.UserLogin" %>

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
        a:visited {
            color: gray;
        }
        a:hover {
            color: #93B084;
        }
        a:active {
            color: gray;
        }
        .auto-style1 {
            width: 405px;
            height: 30px;
        }
        .auto-style2 {
            height: 30px;
        }
        .auto-style3 {
            height: 334px;
        }
        .auto-style4 {
            height: 24px;
        }
        .auto-style5 {
            width: 140px;
            height: 24px;
        }
    </style>

 <%--  <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
      <script type="text/javascript" src="javascript/jquery.min.js"></script>
<script src="javascript/WaterMark.min.js" type="text/javascript"></script>

<script type="text/javascript">
    $(function () {
        $("[id*=txtUserName], [id*=txtPassword], [id*=txtDetails]").WaterMark();
        //To change the color of Watermark
        $("[id*=Email]").WaterMark(
        {
            WaterMarkTextColor: '#000'
        });
    });

	</script>
</head>
<body style="background-color:white;background-image:url(images/watermark.png);background-repeat:repeat">
   

    <form id="myform" runat="server" defaultbutton="btnLogin" defaultfocus="txtUserName">
        <div class="logincontent">
            <table style="border-collapse:collapse;border-color:#3F3F3F;">
                <tr>
                    <td style="text-align:left;" class="Col1Style">
                        <img src="Images/PRMLogo_height80.png" alt="PRM Logo" />
                    </td>
                </tr>
                <tr style="width:100%;height:20px;">
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
                    <td class="MenuWidth">
                        <a href="http://us.milliman.com/legal/copyright/" target="_blank">Copyright</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="flex-item-align:center;background-color:#3F3F3F" class="auto-style3">
                        <img src="Images/New_LoginSplash.png" alt="Milliman" />
                    </td>
                </tr>
                <tr style="background-color:#3F3F3F">
                    <td class="auto-style1"  >
                        <table style="border-collapse:collapse;background-color:#3F3F3F">
                         <tr>
                        <td class="auto-style4"><asp:TextBox ID="txtUserName" Width="150px" runat="server" ToolTip="  Email"  ></asp:TextBox></td>
                        <td class="auto-style4"><asp:TextBox ID="txtPassword" Width="150px" runat="server" TextMode="Password" ToolTip="  Password"></asp:TextBox></td>
                        <td class="auto-style5"><a id="LostPassWord" href="LostPassword.aspx" target="_self"  style="padding: 0px; margin: 0px; text-align:right; vertical-align:middle; font:400 9px arial; color:white; width:130px; " >&nbsp;&nbsp;Lost&nbsp;Password</a></td>
                      
                         </tr>
                        </table>
                    </td>
                     <td class="auto-style2">
                         &nbsp;
                    </td>
                    <td class="auto-style2">
                        <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to verify Milliman's encrypted communication status">
                        <tr>
                        <td width="135px" align="center" valign="middle">
                             <img src="Images/Secured.png" alt="Secured" style="border:0px" title="2048 RSA Encryption" />
                        </td>
                        </tr>
                        </table>
                    </td>
                    <td class="auto-style2"></td>
                    <td style="text-align:center;" class="auto-style2">
                        <asp:ImageButton runat="server" ImageUrl="~/Images/SignIn.png" ID="btnLogin" OnClick="btnLogin_Click" />
                    </td>
                </tr>
                <tr >
                    <td style="text-align:center;background-color:#3D3D3D;vertical-align:middle;" colspan="" class="Col1Style">
                        <asp:Label ID="lblErrorMessage" CssClass="error-1" runat="server">*A valid email and password are required!</asp:Label>
                    </td>
                    <td title="SFv2.11" style="text-align:right;background-color:#3D3D3D;color:white;height:30px;vertical-align:bottom;" colspan="4">
                        &copy; Powered by Milliman 2015
                    </td>
                </tr>
            </table>

<%--                DIV required for controls -for now--%>
            <div style="display:block;line-height:0;height:0px;overflow:hidden;zoom:1">
                <asp:DropDownList ID="spToIdPBindingList" CssClass="textbox" runat="server">
                    <asp:ListItem Value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" >HTTP redirect</asp:ListItem>
                    <asp:ListItem Value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Selected="True">HTTP POST</asp:ListItem>
                    <asp:ListItem Value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact">HTTP artifact</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="idpToSPBindingList" CssClass="textbox" runat="server">
                    <asp:ListItem Value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Selected="True">HTTP POST</asp:ListItem>
                    <asp:ListItem Value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact">HTTP artifact</asp:ListItem>
                </asp:DropDownList> 
                <asp:Button runat="server" CssClass="okbutton" ID="btnIdPLogin" Text="Next" OnClick="btnIdPLogin_Click" />
            </div>
 
        </div>
    </form>
</body>
</html>
