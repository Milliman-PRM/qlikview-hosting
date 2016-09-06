<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClientPublisher._Default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="https://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" >
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
<body style="background-color:white;background-image:url(images/watermark.png);background-repeat:repeat">
    <form id="myform" runat="server">
       <asp:ScriptManager ID="ScriptMgr" runat="server"> </asp:ScriptManager>


        <div class="templatecontent">
            <div class="header">
                <table style="width:100%;min-width:600px">
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
             
            <div id="Div1" style="display:block;overflow:auto;position:absolute;top:130px;bottom:25px;left:5px; right:5px;  border:1px solid white;background-color:transparent">  
                <center>
 
<%--                    update panel is need to update main windows without causing refresh of open DIV windows--%>
                <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="UpdatePanel_Button" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button runat="server" ID="UpdatePanel_Button" UseSubmitBehavior="false" ClientIDMode="Static" onClick="UpdatePanel_Button_Click" style="display:none" />
                        <div style="width:800px">
                           <telerik:RadListView Skin="Silk" ID="RadProjectList"  runat="server" ItemPlaceholderID="ProjectContainer"  AllowPaging="False" >
                            <LayoutTemplate>
                                <fieldset id="FieldSet1" >
                                    <legend ><p style="font-weight:700">PRM Project Listing</p></legend>
                                    <asp:PlaceHolder ID="ProjectContainer" runat="server"></asp:PlaceHolder>
                                </fieldset>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <fieldset style="text-align:left; background-color:white; width:95% ">
                                    <legend>
                                        <p style="font-weight:700">Project: <%#Eval("ProjectName")%> </p>
                                    </legend>
                                    <table cellpadding="0" cellspacing="0" width="95%" style=" background-color:white; ">
                                        <tr>
                                            <td style="width: 75%;">
                                                <table cellpadding="6" cellspacing="0">
                                                    <tr>
                                                        <td style="width: 25%;">
                                                           Tool Tip:
                                                        </td>
                                                        <td style="width: 75%">
                                                            <%#Eval("QVTooltip")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 25%;">
                                                           Description:
                                                        </td>
                                                        <td style="width: 75%">
                                                            <%#Eval("QVDescription")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Last Updated:
                                                        </td>
                                                        <td>
                                                            <%#Eval("LastEditedDate")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Updated By:
                                                        </td>
                                                        <td>
                                                            <%# Eval("LastEditedBy")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Notes:
                                                        </td>
                                                        <td>
                                                            <%# Eval("Notes")%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                             <td >
                                                <table style="width:150px" >
                                                    <tr>
                                                        <td> <img src="<%# Eval("QVIconReflector")%>" atl="Project Icon"></img></td>
                                                    </tr>
                                                   <tr>
                                                        <td style="text-align:center">Icon(128x90)</td>
                                                   </tr>
                                                </table>
                                            </td>
                                            <td >
                                               <table >
                                                    <tr>
                                                        <td><telerik:RadButton Icon-PrimaryIconCssClass="rbEdit" ID="EditProject" runat="server" Text="Edit Project"  OnClientClicked="function(sender,args){Click(sender, args);}" AutoPostBack="false" CommandArgument= <%#Eval("ProjectIndex")%> RenderMode="Lightweight" Width="60px" Height="60px"/> </td>
                                                    </tr>
                                                   <tr>
                                                        <td>
                                                            <telerik:RadButton Icon-PrimaryIconCssClass="rbNext" ID="RadButton1" runat="server" Text="View QVW" onClick="ViewQVW_Click" ButtonType="LinkButton" NavigateUrl=<%# Eval("QVLauncher") %> Target="_blank"  RenderMode="Lightweight" Width="60px" Height="60px"/>
                                                        </td>
                                                    </tr>
                                                   <tr>
                                                       <td>
                                                          <telerik:RadButton Icon-PrimaryIconCssClass="rbConfig" ID="ToggleAvailability" runat="server" OnClick="ToggleAvailability_Click" OnClientClicking="VerifyStateChange" Text=<%# Eval("Availability")%> ToolTip=<%# Eval("AvailabilityTooltip")%>      RenderMode="Lightweight" Width="60px" Height="60px" />
                                                      </td>
                                                   </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>

                                </fieldset>
                            </ItemTemplate>

                           <AlternatingItemTemplate>
                                <fieldset style="text-align:left; background-color:#efeeee; width:95% ">
                                    <legend>
                                        <p style="font-weight:700">Project: <%#Eval("ProjectName")%> </p>
                                    </legend>
                                    <table cellpadding="0" cellspacing="0" width="95%" style=" background-color:#efeeee; ">
                                        <tr>
                                            <td style="width: 75%;">
                                                <table cellpadding="6" cellspacing="0">
                                                    <tr>
                                                        <td style="width: 25%;">
                                                           Tool Tip:
                                                        </td>
                                                        <td style="width: 75%">
                                                            <%#Eval("QVTooltip")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 25%;">
                                                           Description:
                                                        </td>
                                                        <td style="width: 75%">
                                                            <%#Eval("QVDescription")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Last Updated:
                                                        </td>
                                                        <td>
                                                            <%#Eval("LastEditedDate")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Updated By:
                                                        </td>
                                                        <td>
                                                            <%# Eval("LastEditedBy")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Notes:
                                                        </td>
                                                        <td>
                                                            <%# Eval("Notes")%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                             <td >
                                               <table style="width:150px" >
                                                    <tr>
                                                        <td>  <img src="<%# Eval("QVIconReflector")%>" atl="Project Icon"></img> </td>
                                                    </tr>
                                                   <tr>
                                                        <td style="text-align:center">Icon(128x90)</td>
                                                   </tr>
                                                </table>
                                            </td>
                                            <td >
                                               <table >
                                                    <tr>
                                                        <td><telerik:RadButton  runat="server" Text="Edit Project"  OnClientClicked="function(sender,args){Click(sender, args);}" AutoPostBack="false" CommandArgument= <%#Eval("ProjectIndex")%> Icon-PrimaryIconCssClass="rbEdit" RenderMode="Lightweight" Width="60px" Height="60px"/> </td>
                                                    </tr>
                                                   <tr>
                                                        <td>
                                                            <telerik:RadButton Icon-PrimaryIconCssClass="rbNext" ID="ViewQVW" runat="server" Text="View QVW" onClick="ViewQVW_Click" ButtonType="LinkButton" NavigateUrl=<%# Eval("QVLauncher")%> RenderMode="Lightweight" Width="60px" Height="60px"/>
                                                        </td>
                                                    </tr>
                                                   <tr>
                                                       <td>
                                                          <telerik:RadButton Icon-PrimaryIconCssClass="rbConfig" ID="ToggleAvailability" runat="server" OnClick="ToggleAvailability_Click" OnClientClicking="VerifyStateChange" Text=<%# Eval("Availability")%> ToolTip=<%# Eval("AvailabilityTooltip")%>      RenderMode="Lightweight" Width="60px" Height="60px"/>
                                                       </td>
                                                   </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </AlternatingItemTemplate>

 

                        </telerik:RadListView>
                        </div>
                </ContentTemplate>
                </asp:UpdatePanel>
                </center>
            </div>
        </div>

         <div id="footer" style="height:25px;bottom:0px;position:absolute;left:5px;left:10px;right:10px;overflow:hidden;vertical-align:bottom">
             <center>Copyright &copy Milliman 2016</center>
        </div>

        <asp:PlaceHolder runat="server" ID="test"></asp:PlaceHolder>
        <telerik:RadWindowManager EnableShadow="true" Behaviors=" Close, Move, Resize" ID="RadWindowManager" DestroyOnClose="true" Opacity="100" runat="server" VisibleStatusbar="False" Style="z-index: 20000;">
          <Windows>
               <telerik:RadWindow ID="RadWindow1" runat="server" Animation="Fade" AutoSize="True" Behaviors="Close, Move" Modal="True"   InitialBehaviors="Close"  VisibleStatusbar="False"  VisibleTitlebar="False" />
          </Windows>
     </telerik:RadWindowManager>
    </form>

    <script language="javascript" type="text/javascript">
   
        function VerifyStateChange(button, args)
        {
            var ButtonLabel = button.get_text().toUpperCase();
            if (ButtonLabel == "OFFLINE")
            {
                args.set_cancel(true);
                alert(button.get_toolTip());
            }
            else if (ButtonLabel == "TAKE OFFLINE")
            {
                args.set_cancel(!confirm("Selecting this option will make the report UNAVAILABLE to users. Continue?"));
            }
            else if ( ButtonLabel == "TAKE ONLINE")
            {
                args.set_cancel(!confirm("Selecting this option will make the report AVAILABLE to users. Continue?"));
            }
        }

        function OpenProfile()
        {

        }
        function Click(button, args) {
            var FoundWinodw = $find("Project " + args._commandArgument);
            if (FoundWinodw) {
                FoundWinodw.show();
            }
            else {
                var wnd = window.radopen("ProjectEditor.aspx?key=" + args._commandArgument, "Project " + args._commandArgument);
                wnd.setSize(930, 500);
                wnd.Center();
                wnd.set_title(args._commandArgument);
                wnd.add_beforeClose(OnBeforeClose);
                wnd.add_close(OnWindowClose)
            }
        }

        function OnBeforeClose(sender, eventArgs)
        {
            eventArgs.set_cancel(confirm("Are you sure you want to close this window?") == false);
            //update main view again to keep in sync with changes in editor windows, but keep all other editor windows open
            var BTN = document.getElementById("UpdatePanel_Button");
            if (BTN)
                BTN.click();

        }
        function OnWindowClose(sender, eventArgs)
        {

        }


    </script>

</body>
</html>
