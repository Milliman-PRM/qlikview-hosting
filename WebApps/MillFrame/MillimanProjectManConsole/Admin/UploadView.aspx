<%@ Page Language="C#" AutoEventWireup="true" Inherits="UploadView" Codebehind="UploadView.aspx.cs" %>
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
             ThisDialog.close();
             if (Parent) {
                 Parent.Closer();
             }
         }

         function AutoSize() {
                 getRadWindow().autoSize();
             }
         
</script>
    <title></title>
</head>
<body style="font-size:11pt; font-weight:400; background-image:url( ../images/dialog-bg.png ); background-size:100% 100%" onload="AutoSize();">
  <form id="form1" runat="server" >

    <div id="WorkingDiv"  >
                     <table style="width:580px;margin:5px 5px 5px 5px">
                        <tr><td></td></tr>
                         <tr><td>
                        <table style="border:1px solid gray;width:580px">
                        <tr >
                            <td colspan="2" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid green" >
                               <asp:Label ID="PresentationLabel" Text="Presentation Upload" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td  >Presentation Project (QVW)</td> 
                            <td > 
                            <asp:FileUpload id="PresentationUploadControl" runat="server" Width="237px" />

                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style5">Presentation Resources (*.*)</td>
                            <td>
                            <asp:FileUpload id="PresentationResoucesUploadControl" runat="server" Width="237px" />

                            </td>
                        </tr>
                          <tr>
                            <td class="auto-style5">Presentation Thumbnail(128x128)(*.jpg,*.gif, *.png)</td>
                            <td>
                            <asp:FileUpload id="PresentationThumbnail" runat="server" Width="237px" />
                            </td>
                        </tr>
                        <tr><td>&nbsp;</td><td></td></tr>
                        <tr><td>&nbsp;</td><td></td></tr>
                           <tr>
                            <td class="auto-style5">Presentation Description</td>
                            <td> 
                            <asp:TextBox Height="97px" ID="Description" runat="server" Width="241px" TextMode="MultiLine" />
                            </td>
                        </tr>
 
                         </table>
                         </td></tr>
                         <tr><td></td></tr>
                         <tr><td>
                         <table style="border:1px solid gray;width:580px">
                        <tr>
                            <td colspan="2" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid gray">
                             <asp:CheckBox ID="DataRestrictionCheckbox"  runat="server" Text="Data Restriction Map       " TextAlign="Left" ToolTip="Restrict a user to specific data determined at runtime" AutoPostBack="True" OnCheckedChanged="DataRestrictionCheckbox_CheckedChanged"  />
                            </td>
                        </tr>
                         <tr><td></td></tr>
                        <tr>                           
                            <td align="center"><asp:DropDownList ID="CovisintFieldID" runat="server" Width="150px" Enabled="False"></asp:DropDownList> </td>
                            <td align="center"><asp:DropDownList ID="QVFieldID" runat="server" Width="150px" Enabled="False"></asp:DropDownList> </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                        </table>
                         </td></tr>
                         <tr><td></td></tr>
                         <tr><td>
                        <table style="border:1px solid gray;width:580px">
                        <tr>
                            
                            <td colspan="2" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid gray">
                                <asp:CheckBox ID="UserDatabase" runat="server" Text="Power User Database" TextAlign="Left" ToolTip="Database this report is attached to for power user browsing." AutoPostBack="True" OnCheckedChanged="UserDatabase_CheckedChanged" />
                                
                            </td>
                         </tr>
                         <tr><td></td></tr>
                         <tr>                           
                            <td align="center" colspan="2"><asp:DropDownList ID="DatabaseConnection" runat="server" Width="200px" Enabled="False"></asp:DropDownList> </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                        <tr><td>&nbsp;</td></tr>
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="Upload" runat="server"  Text="Update System"  OnClick="Upload_Click" OnClientClick="Submitted()" />
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

      
    </script>
    </body>
</html>
