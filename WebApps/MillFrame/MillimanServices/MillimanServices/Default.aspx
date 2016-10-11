<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Milliman._Default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Milliman - Health Care Intelligence</title>
    <meta http-equiv="refresh" content="1200"/>
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <link rel="Stylesheet" href="Css/Styles.css" />
    <style type="text/css">
        h4 {
            text-align: right;
            vertical-align: bottom;
            background-color: red;
        }

        .tableRow {
            border-bottom: 1px solid gray;
        }

        /*restyle the radwindow alert box to make it not look bad*/
        .RadWindow .rwWindowContent .radalert {
            background-image: none !important; /* removes the excalamtion mark icon */
            padding-left: 0px !important;
        }

        .RadWindow .rwDialogText {
            margin-left: 10px !important;
        }

        .RadWindow .rwPopupButton {
            margin-left: 100px !important;
        }

        .auto-style1 {
            width: 600px;
            height: 17px;
        }

        .auto-style2 {
            height: 17px;
        }
    </style>
    <script type='text/javascript'>
        function showElement(id, display) {
            if (!display)
                display = 'inline';
            if (document.getElementById(id).style.display != 'none') {
                document.getElementById(id).style.display = 'none';
                return 'Show';
            }
            else {
                document.getElementById(id).style.display = display;
                return 'Hide';
            }
        }

        function Toggle(ShowID, HideID) {
            document.getElementById(ShowID).style.display = 'block';
            document.getElementById(HideID).style.display = 'none';
        }

</script>

</head>
<body >
    <form id="myform" runat="server">
        <asp:ScriptManager ID="ScriptMgr" runat="server"> </asp:ScriptManager>
        <div class="templatecontent">
            <div class="header">
                <table style="width:100%;min-width:800px"">
                    <tr >
                        <td>
                            &nbsp;
                        </td>
                        <td style="text-align:right">
                            <img src="Images/New_MillimanHeader.png" alt="Milliman Logo" />&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td  valign="bottom" style="padding-left:10px;" class="auto-style1" ><asp:Label style="width:98%" runat="server" ID="Breadcrumb" Font-Bold="True">Milliman Web Service Debug View</asp:Label></td>
                        <td valign="bottom" align="right" class="auto-style2" >
                             <b>Welcome! <%=Context.User.Identity.Name%></b> &nbsp&nbsp  
                        </td>
                    </tr>
                </table>
            </div>
  <%--          <div id='cssmenu' >
            <ul>
               <li><a href='#' onclick="Toggle('HomePage','ProductsPage'); return false;" ><span>Home</span></a></li>
               <li><a href='#' onclick="Toggle('ProductsPage','HomePage'); return false;" ><span>Assessment</span></a></li>
               <li><a href='#' onclick="return OpenWindow()" ><span>Patient History</span></a></li>
               <li><a href='http://www.milliman.com' target="_blank"><span>Profile</span></a></li>
               <li><a href='mailto:hcintel.support@milliman.com?Subject=Support%20Request' target="_blank"><span>Support</span></a></li>
               <li class='last'><a href='MillimanLogout.aspx'><span>Logout</span></a></li>
            </ul>
            </div>  --%>

            <div id="ContentDiv" runat="server" style="margin:10px;display:block;overflow:auto;position:absolute;top:120px;bottom:30px;left:5px; right:5px;  border:1px solid gray">  
               <iframe frameborder="0" src="DebugView.aspx" height="100px" width="100%" id="framecontent" onload="autoResize('framecontent');"></iframe> 
            </div>

        </div>
    
         <div id="footer" style="height:25px;bottom:0px;position:absolute;left:5px;left:10px;right:10px;overflow:hidden;vertical-align:bottom">
             <center>                            <div>
                    Copyright © Milliman &nbsp
                                <asp:Label ID="lblcopyrightYear" runat="server"></asp:Label>
                    <script type="text/javascript">document.getElementById("lblcopyrightYear").innerHTML = new Date().getFullYear();</script>
                </div>
                 &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="Clear" runat="server" Text="Clear Debug Messages" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Height="20px" OnClick="Clear_Click" ToolTip="Click to clear all debug messages from the queue" /></center>
        </div>

        <telerik:RadWindowManager EnableShadow="true" Behaviors="Close, Reload, Move" ID="RadWindowManager" DestroyOnClose="true" Opacity="100" runat="server" Width="450" Height="400" VisibleStatusbar="False">
          <Windows>
               <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True" Behaviors="Close, Move, Reload" Modal="True"  Height="800px" Width="600px" InitialBehaviors="Close" Title="User Profile/Password Settings" VisibleStatusbar="False"  VisibleTitlebar="False" />
          </Windows>
     </telerik:RadWindowManager>
    </form>

    <script language="javascript" type="text/javascript">
        function OpenWindow() {
            var wnd = window.radopen("profile.aspx", "User Profile/Password Settings");
            wnd.setSize(900, 550);
            wnd.Center();
            return false;
        }

        function autoResize(id) {
            var newheight;
            var newwidth;

            if (document.getElementById) {
                newheight = document.getElementById(id).contentWindow.document.body.scrollHeight;
                newwidth = document.getElementById(id).contentWindow.document.body.scrollWidth;
            }

            document.getElementById(id).height = (newheight) + "px";
            document.getElementById(id).width = (newwidth) + "px";
        }

    </script>

</body>
</html>
