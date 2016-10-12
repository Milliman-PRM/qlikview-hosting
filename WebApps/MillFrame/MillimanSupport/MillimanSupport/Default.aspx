<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MillimanSupport._Default" %>
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
            vertical-align:bottom;
            background-color:red;
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
            margin-left:100px !important;
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
        function ChangeContent(ContentURL) {
            //if (ContentURL.indexOf("status.aspx") != -1) {
                var BusyDiv = document.getElementById("busy");
                BusyDiv.style.visibility = "visible";
                document.getElementById("PageContent").onload = function () {
                    var BusyDiv = document.getElementById("busy");
                    BusyDiv.style.visibility = "hidden";
                }
            //}
            document.getElementById("PageContent").src = "http://" + window.location.host + '/MillimanSupport/' + ContentURL;
        }
        function SetInitialFrame() {
            ChangeContent('status.aspx');
        }

      
</script>

</head>
<body onload="SetInitialFrame();" style="overflow:hidden;">
    <form id="myform" runat="server">
        <asp:ScriptManager ID="ScriptMgr" runat="server"> </asp:ScriptManager>
        <div class="templatecontent">
            <div class="header">
                <table style="width:100%">
                    <tr >
                        <td >
                            <img src="Images/PRMLogo_height80.png" alt="Milliman Logo" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:20px" valign="middle"><%= DateTime.Now.ToLongDateString() %></td>
                        <td valign="middle" style="width:100%" valign="bottom" align="right" >
                             <b>Welcome! <%=Context.User.Identity.Name%></b> &nbsp&nbsp  
                        </td>
                    </tr>
                </table>
            </div>
            <div id='cssmenu'>
            <ul>
               <li><a href='#' onclick="ChangeContent('status.aspx'); return false;" ><span>System Status</span></a></li>
               <li><a href='#' onclick="ChangeContent('tickets.aspx'); return false;" ><span>Tickets</span></a></li>
               <li><a href='#' onclick="ChangeContent('userstatus.aspx'); return false;" ><span>External Users</span></a></li>
               <li><a href='#' onclick="ChangeContent('troubleshooting.aspx'); return false;" ><span>Troubleshooting</span></a></li>
               <li><a href='#' onclick="ChangeContent('events.aspx'); return false;" ><span>Calendar</span></a></li>
            <%--   <li><a href='#' onclick="return OpenWindow()" ><span>Profile</span></a></li>--%>
        <%--       <li><a href='http://www.milliman.com' target="_blank"><span>About Milliman</span></a></li>
               <li><a href='mailto:hcintel.support@milliman.com?Subject=Support%20Request' target="_blank"><span>Support</span></a></li>--%>
               <li class='last'><a href='MillimanLogout.aspx'><span>Logout</span></a></li>
                
            </ul>
            </div>

<%--            Chit Chat div--%>
            <div>
            <br />
            
                <br />
                <h5><%=Session["patientid"] %>  </h5>
            <br />
          </div>   
            <div id="PageFrame" style="display:block;overflow:hidden;position:absolute;top:200px;bottom:25px;left:5px; right:5px;  border:1px solid white">  
                <iframe id="PageContent" frameborder="0" height="100%" width="100%" scrolling="auto">  ></iframe>
 
            </div>
            <center>
            </center>
        </div>

         <div id="footer" style="height:25px;bottom:0px;position:absolute;left:5px;left:10px;right:10px;overflow:hidden;vertical-align:bottom">
             <center>
                  <div>
                    Copyright © Milliman &nbsp
                                <asp:Label ID="lblcopyrightYear" runat="server"></asp:Label>
                    <script type="text/javascript">document.getElementById("lblcopyrightYear").innerHTML = new Date().getFullYear();</script>
                </div>
             </center>
        </div>

        <telerik:RadWindowManager EnableShadow="true" Behaviors="Close, Reload, Move" ID="RadWindowManager" DestroyOnClose="true" Opacity="100" runat="server" Width="450" Height="400" VisibleStatusbar="False">
          <Windows>
               <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True" Behaviors="Close, Move, Reload" Modal="True"  Height="800px" Width="600px" InitialBehaviors="Close" Title="User Profile/Password Settings" VisibleStatusbar="False"  VisibleTitlebar="False" />
          </Windows>
     </telerik:RadWindowManager>
     <div id="busy" style="position:relative;top:300px;left:600px;visibility:hidden;">
         <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Roundedblocks.gif" />     </div>
    </form>

    <script language="javascript" type="text/javascript">
        function OpenWindow()
        {
            var wnd = window.radopen("profile.aspx", "User Profile/Password Settings");
            wnd.setSize(900, 550);
            wnd.Center();
            return false;
        }

    </script>

</body>
</html>
