<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MillimanDev._Default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="https://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Milliman - PRM</title>
    <meta http-equiv="refresh" content="1080"/> <%--refresh page in 18 mins, session timeout is 15 mins--%>

    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta charset='utf-8'/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <script src="https://code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
 <%--   <script src="script.js"></script>--%>
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
</head>
<body>
    <form id="myform" runat="server">
        <asp:ScriptManager ID="ScriptMgr" runat="server"> </asp:ScriptManager>
        <div class="templatecontent">
            <div class="header">
                <table style="width:100%">
                    <tr >
                        <td >
                            <img src="Images/PRMLogo_height80.png" alt="Milliman Logo"  />
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
 
            <div  style="min-width:800px; border-bottom:1px solid #F4CB79;border-top:1px solid #FAE5BC; background-color: #F4F4F4;background-image: url('Images/header.gif'); background-repeat: repeat; ">
                <table style="width:100%">
                    <tr>
                        <td>
                            <telerik:RadMenu ID="RadMenu1" runat="server" EnableRoundedCorners="true" EnableShadows="true" OnItemClick="RadMenu1_ItemClick" ></telerik:RadMenu>
                        </td>
                        <td valign="middle">
                            <span style=" float:right;vertical-align: middle"><asp:Label runat="server" ID="FilterLabel" Visible="False">Filter By &nbsp;</asp:Label><asp:DropDownList ID="Groups" runat="server" Visible="False" AutoPostBack="True" OnSelectedIndexChanged="Groups_SelectedIndexChanged" ViewStateMode="Enabled"></asp:DropDownList>&nbsp;&nbsp;</span>
                        </td>
                    </tr>
                </table> 
           </div>
            <div>
            <br />
                <br />
                <h5><%=Session["patientid"] %>  </h5>
            <br />
          </div>   
            <div id="Div1" style="display:block;overflow:auto;position:absolute;top:200px;bottom:25px;left:5px; right:5px;  border:1px solid white">  
                <center>
                <asp:Table cellspacing="5" ID="ProductsPageTable" runat="server" style="border-color: #0C4977; width: 400px; background-color:white" CellPadding="5">
                    <asp:TableRow ID="FirstAndOnlyRow">
                        <%--projects--%>
                     <asp:TableCell style="vertical-align:top;" >
                            <div style="border:solid; border-width:1px;width: 400px;border-color:#0C4977">
                                <div style="line-height:25px; height:25px;vertical-align:middle;background-color:white; border-bottom:1px solid gray; "><center><b>Licensed Products</b></center></div>
                                <asp:Table runat="server" ID="Products" style="border-color: #0C4977; width: 400px; background-color:white" CellPadding="5" CellSpacing="0">
                                </asp:Table>
                           </div>
                        </asp:TableCell>

                       <%-- announcements--%>
                     <asp:TableCell style="vertical-align:top;" >
   
                        </asp:TableCell>

                    </asp:TableRow>
                </asp:Table>
                </center>
            </div>
        </div>

         <div id="footer" style="height:25px;bottom:0px;position:absolute;left:5px;left:10px;right:10px;overflow:hidden;vertical-align:bottom">
             <center>Copyright &copy Milliman 2016</center>
        </div>

        <telerik:RadWindowManager EnableShadow="true" Behaviors="Resize, Close, Reload, Move" ID="RadWindowManager" DestroyOnClose="true" Opacity="100" runat="server" Width="450" Height="400" VisibleStatusbar="False" Style="z-index: 20000;">
          <Windows>
               <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True" Behaviors="Close, Move, Reload" Modal="True" 
                    Height="800px" Width="570px" InitialBehaviors="Close" 
                   Title="User Profile/Password Settings" VisibleStatusbar="False"  VisibleTitlebar="False"
                   OnClientResize="clientShow" OnClientDragStart="clientShow" OnClientDragEnd="clientShow"  OnClientShow="clientShow" />
          </Windows>
     </telerik:RadWindowManager>
    </form>

    <script language="javascript" type="text/javascript">
        function OpenProfile()
        {
            var wnd = window.radopen("profile.aspx", "User Profile/Password Settings");
            //setting window size
            wnd.setSize(950, 570);
            wnd.set_modal(true);
            wnd.Center();
        }

        function clientShow(sender, args) {
            debugger;
            if ($telerik.isChrome) {
                sender.get_contentFrame().style.width = "";
                setTimeout(function () {
                    sender.get_contentFrame().style.width = "100%";
                }, 0);
            }
        }

        function SayHi() {
            
        }
    </script>

</body>
</html>
