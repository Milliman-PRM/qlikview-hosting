<%@ Page Language="C#" AutoEventWireup="true" Inherits="EnhancedUploadView" Codebehind="EnhancedUploadView.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <script language="javascript" type="text/javascript">
         function getRadWindow() {
             var oWindow = null;
             if (window.radWindow)
                 oWindow = window.radWindow;
             else if (window.frameElement.radWindow)
                 oWindow = window.frameElement.radWindow;
             return oWindow;
         }

         // Reload parent page
         function CloseDialog() {
             var ThisDialog = getRadWindow();
             var Parent = getRadWindow().BrowserWindow;
             //don't auto close
             //ThisDialog.close();
             //need to update parent when closing, may be new project
             if (Parent) {
                 Parent.Closer();
             }
         }

         function AutoSize() {
                 getRadWindow().autoSize();
             }
         
</script>
    <title></title>
     <style type="text/css">
         
.tftable {font-size:12px;color:#333333;width:100%;border-width: 1px;border-color: #a9a9a9;border-collapse: collapse;width:580px}
.tftable th {font-size:12px;background-color:#F5F5F5;border-width: 1px;padding: 8px;border-style: solid;border-color: #a9a9a9;text-align:left;}
.tftable tr {background-color:#ffffff;}
.tftable td {font-size:12px;border-width: 1px;padding: 8px;border-style: solid;border-color: #a9a9a9;}
.tftable tr:hover {background-color:#F5F5F5;}
    </style>
</head>
<body style="font-size:11pt; font-weight:400; background-image:url( ../images/dialog-bg.png ); background-size:100% 100%" onload="AutoSize();">
  <form id="form1" runat="server" >
       <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
   
     

    <div id="WorkingDiv" >
                     <table style="width:580px;margin:5px 5px 5px 5px">
                        <tr><td></td></tr>
                         <tr><td>

                        <table class="tftable" style="border:1px solid gray;width:580px;padding:10px">
                        <tr >
                            <td colspan="3" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid black" >
                               <asp:Label ID="Label1" Text="General Project Information" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr >
                            <td  >Project Name</td> 
                            <td colspan="2" > 
                            <asp:TextBox ID="ProjectName" runat="server" width="98%" ToolTip="Name of this project - will be visible to client user as title" OnTextChanged="ProjectName_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                       <tr >
                            <td  >QVW Name</td> 
                            <td colspan="2"> 
                            <asp:TextBox ID="QVWName" runat="server" Width="98%" ToolTip="The name of the QVW this project will create - not visible to client users" Enabled="False" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td  >Project Description</td> 
                            <td colspan="2"> 
                             <asp:TextBox Height="97px" ID="Description" runat="server" Width="98%" TextMode="MultiLine" ToolTip="Description of the problem this report solves - will be visible to client user." />
                            </td>
                        </tr>
                        <tr>
                            <td  >Project Notes</td> 
                            <td colspan="2"> 
                             <asp:TextBox Height="97px" ID="Notes" runat="server" Width="98%" TextMode="MultiLine" ToolTip="Notes associated with this project - not visible to client users" />
                            </td>
                        </tr>
                       <tr>
                            <td  >Project Tooltip</td> 
                            <td colspan="2"> 
                             <asp:TextBox Height="97px" ID="Tooltip" runat="server" Width="98%" TextMode="SingleLine" ToolTip="Tooltip that will be displayed on mouse-over of the launch icon - visible to client users" />
                            </td>
                        </tr>
                          <tr>
                            <td style="border-bottom-style:none;" class="auto-style5">Project Thumbnail(128x128)(*.jpg,*.gif, *.png)</td>
                              <td style="text-align:center;border:1px solid gray;border-bottom-width:0px" colspan="2" > <asp:Image ID="PreviewImage" runat="server" Width="128px" Height="128px" /></td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td colspan="2" style="text-align:center;border:1px solid gray;border-top-width:0px">
                                <telerik:RadAsyncUpload runat="server" ID="PresentationThumbnail" style="width:95%"
                                    AllowedFileExtensions="png,gif,jpg,jpeg" MaxFileSize="1000000000" MaxFileInputsCount="1">
                                </telerik:RadAsyncUpload>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style5" >User Manual</td>
                            <td colspan="2" style="text-align:center" ><asp:HyperLink runat="server" ID="UserManualLabel" ToolTip="User manual associated with this QVW - click to launch"></asp:HyperLink></td>
                        </tr>
                       </table>
                             </td></tr>
                       <tr><td></td></tr>
                         <tr><td>
                        <table class="tftable" style="border:1px solid gray;width:580px">
                        <tr >
                            <td colspan="4" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid black" >
                               <asp:Label ID="PresentationLabel" Text="Group Assignment/Modification" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                       <%--     <td rowspan="2">Add to Group(s)</td>--%>
                            <td colspan="4" style="text-align:center">
                                <table border="0">
                                    <tr style="border:0px">
                                        <td>New Group</td>
                                        <td><asp:TextBox runat="server" ID="NewGroupName"></asp:TextBox></td>
                                        <td><asp:Button runat="server" ID="CreateGroup" Text="Create Group" ToolTip="Create a group on the production server." OnClick="CreateGroup_Click"/></td>
                                        <td><asp:Button runat="server" ID="Delete" Text="Delete Group" ToolTip="Delete the 'checked' groups on the production server" OnClick="Delete_Click"/></td>
                                   </tr></table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4"><div style="height:400px;min-height:50px;overflow-y:auto;margin:auto;"><asp:CheckBoxList runat="server" ID="SelectedGroups"  RepeatColumns="1" onclick="SetTooltip()" ></asp:CheckBoxList></div> </td>
                        </tr>
 
                         </table>
                         </td></tr>
  
                         <tr><td>
                        <table class="tftable" style="border:1px solid gray;width:580px;">

                        <tr>
                            <td colspan="2" style="text-align:center">
                                <asp:Button  ID="Upload" runat="server"  Text="Save Defaults"  OnClick="Upload_Click" OnClientClick="Submitted()" />
                            </td>
                        </tr>
                            </table>
                         </td></tr>
                         <tr><td></td></tr>
                    </table>    
    </div>
      <div id="ImBusy" style="position:absolute;top:0;left:0;width:100%;height:100%; display:none">
          <table style="position:absolute;top:0;left:0;width:100%;height:100%; visibility:visible">
              <tr>
                  <td style="vertical-align:middle; text-align:center;">
                        <img src="../images/ajax-loader.gif" style="border-width:0px"/><br /> Project Upload and Configuration in Progress
                  </td>
              </tr>
          </table>
      </div>
    </form>
    <script type="text/javascript">
        //disable all the button in the form while uploading
        function Submitted() {
            var WorkingDiv = document.getElementById("WorkingDiv");
            WorkingDiv.style.display = 'none';
            var IAMBUSY = document.getElementById("ImBusy");
            IAMBUSY.style.display = 'block';
            document.title = "Project Settings Modification";
            //var inputs = document.getElementsByTagName("INPUT");
            //for (var i = 0; i < inputs.length; i++) {
            //    if (inputs[i].type === 'submit') {
            //        inputs[i].disabled = true;
            //    }
            //}

            return true;
        }

        function SetTooltip()
        {
            var CheckList = document.getElementById('<%= SelectedGroups.ClientID %>');
            if (CheckList)
            {
                var CheckBoxListArray = CheckList.getElementsByTagName('input');
                var TooltipText = '';
                for (var i = 0; i < CheckBoxListArray.length; i++) 
                {
                    var checkBoxRef = CheckBoxListArray[i];
 
                    if (checkBoxRef.checked == true)
                    {
                        if (TooltipText != '')
                            TooltipText += "\x0A";  //add a new line
                        TooltipText += checkBoxRef.value;
                    }
                }
                CheckList.title = TooltipText;
            }
        }
      
    </script>
    </body>
</html>
