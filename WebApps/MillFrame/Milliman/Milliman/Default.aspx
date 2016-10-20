<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MillimanDev._Default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="https://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Milliman - PRM</title>
    <meta http-equiv="refresh" content="1080" />
    <%--refresh page in 18 mins, session timeout is 15 mins--%>

    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta charset='utf-8' />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script src="https://code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
    <%--   <script src="script.js"></script>--%>
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
            background-image: none !important;
            padding-left: 0 !important;
        }
        /* removes the excalamtion mark icon */
        .RadWindow .rwDialogText {
            margin-left: 10px !important;
        }

        div.menu {
            min-width: 846px;
            border-bottom: 1px solid #F4CB79;
            border-top: 1px solid #FAE5BC;
            height: 28px;
            background-color: #F4F4F4;
            background-image: url('Images/header.gif');
            background-repeat: repeat;
            padding: 2px;
        }

        .clear {
            clear: both;
            float: none;
        }

        div.RadMenu {
            float: left;
        }

        .menuDropdownFilter {
            float: right !important;
            margin-right: 26px;
        }

        .RadWindow .rwPopupButton {
            margin-left: 100px !important;
        }
    </style>
</head>
<body>
    <form id="myform" runat="server">
        <asp:ScriptManager ID="ScriptMgr" runat="server"></asp:ScriptManager>
        <div class="templatecontent">
            <div class="header">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <img src="Images/PRMLogo_height80.png" alt="Milliman Logo" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 20px" valign="middle"><%= DateTime.Now.ToLongDateString() %></td>
                        <td valign="middle" style="width: 100%" valign="bottom" align="right">
                            <b>Welcome! <%=Context.User.Identity.Name%></b> &nbsp&nbsp  
                        </td>
                    </tr>
                </table>
            </div>
            <div class="menu">
                <telerik:RadMenu ID="mnuOptions" runat="server" EnableRoundedCorners="true"
                    EnableShadows="true" OnItemClick="mnuOptions_ItemClick">
                    <CollapseAnimation Duration="200" Type="OutQuint" />
                </telerik:RadMenu>
                <div class="menuDropdownFilter">
                    <asp:Label runat="server" ID="FilterLabel" Visible="False">Filter By &nbsp;</asp:Label>
                    <asp:DropDownList ID="ddlGroups" runat="server" Visible="False" AutoPostBack="True"
                        OnSelectedIndexChanged="Groups_SelectedIndexChanged" ViewStateMode="Enabled" Width="400">
                    </asp:DropDownList>
                </div>
            </div>
            <div>
                <br />
                <br />
                <h5><%=Session["patientid"] %>  </h5>
                <br />
            </div>
            <div id="Div1" style="display: block; overflow: auto; position: absolute; top: 200px; bottom: 25px; left: 5px; right: 5px; border: 1px solid white">
                <center>
                    <asp:Table CellSpacing="5" ID="ProductsPageTable" runat="server" Style="border-color: #0C4977; width: 400px; background-color: white" CellPadding="5">
                        <asp:TableRow ID="FirstAndOnlyRow">
                            <%--projects--%>
                            <asp:TableCell Style="vertical-align: top;">
                                <div style="border: solid; border-width: 1px; width: 400px; border-color: #0C4977">
                                    <div style="line-height: 25px; height: 25px; vertical-align: middle; background-color: white; border-bottom: 1px solid gray;">
                                        <center><b>Licensed Products</b></center>
                                    </div>
                                    <asp:Table runat="server" ID="Products" Style="border-color: #0C4977; width: 400px; background-color: white" CellPadding="5" CellSpacing="0">
                                    </asp:Table>
                                </div>
                            </asp:TableCell>
                            <%-- announcements--%>
                            <asp:TableCell Style="vertical-align: top;">   
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </center>
            </div>
        </div>

        <div id="footer" style="height: 25px; bottom: 0px; position: absolute; left: 5px; left: 10px; right: 10px; overflow: hidden; vertical-align: bottom">
            <center>
                <div>
                    Copyright © Milliman &nbsp<asp:Label ID="lblcopyrightYear" runat="server"></asp:Label>
                    <script type="text/javascript">document.getElementById("lblcopyrightYear").innerHTML = new Date().getFullYear();</script>
                </div>

            </center>
        </div>

        <telerik:RadWindowManager EnableShadow="true" Behaviors="Resize, Close, Reload" ID="RadWindowManager"
            DestroyOnClose="true" Opacity="100" runat="server" Width="450" Height="400"
            VisibleStatusbar="False" Style="z-index: 20000;">
            <Windows>
                <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True"
                    Behaviors="Close, Reload" Modal="True"
                    Height="605px" Width="1022px" InitialBehaviors="Close"
                    Title="User Profile/Password Settings" VisibleStatusbar="False" VisibleTitlebar="False" />
            </Windows>
        </telerik:RadWindowManager>

    </form>

    <script language="javascript" type="text/javascript">
        function OpenProfile() {
            //$('#RadWindow1').html('<iframe border=0 width="100%" height ="100%" src="' + "profile.aspx.aspx" + '"> </iframe>');
            var wnd = window.radopen("profile.aspx", "User Profile/Password Settings", scrollbars = 1);
            ////setting window size
            wnd.setSize(1022, 605);
            wnd.set_modal(true);
            //windoScroll();
            wnd.Center();
        }

        function SayHi() { }

    </script>

</body>
</html>
