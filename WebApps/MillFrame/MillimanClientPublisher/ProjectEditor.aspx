<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectEditor.aspx.cs" Inherits="ClientPublisher.ProjectEditor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="jQuery/jquery.min.js" type="text/javascript"></script>
    <link href="Styles/css.css" rel="stylesheet" type="text/css" />
    <style type="text/css">

       .LockOff { 
          display: none; 
          visibility: hidden; 
       } 

       .LockOn { 
          display: block; 
          visibility: visible; 
          position: absolute; 
          z-index: 999; 
          top: 0px; 
          left: 0px; 
          width: 110%; 
          height: 110%; 
          background-color: #ccc; 
          text-align: center; 
          padding-top: 20%; 
          filter: alpha(opacity=95); 
          opacity: 0.95; 
          font:500 12px italic;
          font-family:'Segoe UI';
          overflow:hidden;
       } 

	    .TFtableCol{
		    width:100%; 
		    border-collapse:collapse; 
	    }
	    .TFtableCol td{ 
		    padding:7px; border:#ebeaea; 1px solid;
	    }
	    /* improve visual readability for IE8 and below */
	    .TFtableCol tr{
		    background: #b8d1f3;
	    }
	    /*  Define the background color for all the ODD table columns  */
	    .TFtableCol tr td:nth-child(odd){ 
		    background: #b8d1f3;
	    }
	    /*  Define the background color for all the EVEN table columns  */
	    .TFtableCol tr td:nth-child(even){
		    background: #dae5f4;
	    }

        .demo-container fieldset {
            display: inline-block;
            *zoom: 1;
            *display: inline;
            vertical-align: top;
            margin-top: 20px;
            width: 400px;
            height:350px;
            background-color:white;
        }

        .fieldlabel {
            font-size:smaller;
            font-weight:bold;
            font-style:italic;
            text-align:right;
        }

        body {
            /*background gradient angrytools.com/gradient   */
            background: -moz-linear-gradient(45deg, rgba(192,192,192,1) 0%, rgba(255,255,255,1) 50%, rgba(128,128,128,1) 100%); /* ff3.6+ */ 
            background: -webkit-gradient(linear, left bottom, right top, color-stop(0%, rgba(192,192,192,1)), color-stop(50%, rgba(255,255,255,1)), color-stop(100%, rgba(128,128,128,1))); /* safari4+,chrome */ 
            background: -webkit-linear-gradient(45deg, rgba(192,192,192,1) 0%, rgba(255,255,255,1) 50%, rgba(128,128,128,1) 100%); /* safari5.1+,chrome10+ */ 
            background: -o-linear-gradient(45deg, rgba(192,192,192,1) 0%, rgba(255,255,255,1) 50%, rgba(128,128,128,1) 100%); /* opera 11.10+ */ 
            background: -ms-linear-gradient(45deg, rgba(192,192,192,1) 0%, rgba(255,255,255,1) 50%, rgba(128,128,128,1) 100%); /* ie10+ */ 
            background: linear-gradient(45deg, rgba(192,192,192,1) 0%, rgba(255,255,255,1) 50%, rgba(128,128,128,1) 100%); /* w3c */ 
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#808080', endColorstr='#c0c0c0',GradientType=1 ); /* ie6-9 */ 
        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function ICONonFilesUploaded(sender, args) {
                var BTN = document.getElementById("ICONbtnUpload");
                if (BTN)
                    BTN.click();

                var Uploader = $find("<%= ICONImage.ClientID %>");
                if (Uploader)
                    Uploader.deleteFileInputAt(0);
            }
            function QVWonFilesUploaded(sender, args) {
                var BTN = document.getElementById("QVWbtnUpload");
                if (BTN)
                    BTN.click();

                var Uploader = $find("<%= QVWImage.ClientID %>");
                if (Uploader)
                    Uploader.deleteFileInputAt(0);
            }
            function ManualonFilesUploaded(sender, args) {
                var BTN = document.getElementById("ManualbtnUpload");
                if (BTN)
                    BTN.click();

                var Uploader = $find("<%= ManualImage.ClientID %>");
                if (Uploader)
                    Uploader.deleteFileInputAt(0);
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function SizeToFit() {
                window.setTimeout(
                    function () {
                        var oWnd = GetRadWindow();
                        //this results in the window being way to large
                        //oWnd.SetWidth(document.body.scrollWidth + 70);
                        //oWnd.SetHeight(document.body.scrollHeight + 70);

                        var lock = document.getElementById('LockPane');
                        if (lock)
                            lock.className = 'LockOff';

                    }, 500);
            }


        </script>
    </telerik:RadCodeBlock>
</head>
<body style="overflow:hidden;" onload="SizeToFit()">
    <form id="form1" runat="server">
           <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
           <telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all" DecorationZoneID="decorationZone" ControlsToSkip="All" Skin="Silk"></telerik:RadFormDecorator>
        
        <table style="width:820px" >
            <tr>
                <td>
                    <div class="demo-container" id="decorationZone1" >
                     <fieldset>
                            <legend>Client Visible Project Settings</legend>
                             <table  style="width:97%;padding:10px">
                                <tr>
                                    <td class="fieldlabel">Report QVW</td>
                                    <td >
                                        <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                         <asp:UpdatePanel runat="server" ID="QVWUpdatePanel">
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="QVWbtnUpload" />
                                            </Triggers>
                                            <ContentTemplate>
                                                <asp:Button runat="server" ID="QVWbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" onClick="QVWbtnUpload_Click" style="display:none" />
                                                <asp:Label runat="server" ID="QVW" BorderStyle="Solid" BorderWidth="1px" BorderColor="LightGray" Width="100%"></asp:Label> 
                                           </ContentTemplate>
                                        </asp:UpdatePanel> 
                                    </td>
                                    <td style="width:75px"> 
                                        <telerik:RadAsyncUpload Width="75px" runat="server" ID="QVWImage" OnClientFilesUploaded="QVWonFilesUploaded" HideFileInput="true" MultipleFileSelection="Disabled" AllowedFileExtensions=".qvw" MaxFileInputsCount="1" >
                                            <FileFilters>
                                                <telerik:FileFilter Description="Qlikview QVW files" Extensions="qvw" />
                                            </FileFilters>
                                        </telerik:RadAsyncUpload>               
                                    </td>         
                                </tr>
                                <tr>
                                    <td class="fieldlabel">User Manual</td>
                                    <td >
                                        <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                         <asp:UpdatePanel runat="server" ID="ManualUpdatePanel">
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ManualbtnUpload" />
                                            </Triggers>
                                            <ContentTemplate>
                                                <asp:Button runat="server" ID="ManualbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" onClick="ManualbtnUpload_Click" style="display:none" />
                                                <asp:Label Width="100%" runat="server" ID="ManualLabel" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px">&nbsp;</asp:Label> 
                                           </ContentTemplate>
                                        </asp:UpdatePanel> 
                                    </td>
                                    <td> 
                                        <telerik:RadAsyncUpload Width="75px" runat="server" ID="ManualImage" OnClientFilesUploaded="ManualonFilesUploaded" HideFileInput="true" MultipleFileSelection="Disabled" AllowedFileExtensions=".docx,.txt,.xlsx,.pdf,.html" MaxFileInputsCount="1" >
                                            <FileFilters>
                                                <telerik:FileFilter Description="Microsoft Word Document" Extensions="docx" />
                                                <telerik:FileFilter Description="Microsoft Excel Document" Extensions="xlsx" />
                                                <telerik:FileFilter Description="Text File" Extensions="txt" />
                                                <telerik:FileFilter Description="PDF Document" Extensions="pdf" />
                                                <telerik:FileFilter Description="HTML Document" Extensions="html" />
                                           </FileFilters>
                                        </telerik:RadAsyncUpload>               
                                    </td>
                                </tr>
                                <tr>
                                     <td class="fieldlabel" >Report Icon</td>
                                    <td >
                                        <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                         <asp:UpdatePanel runat="server" ID="ICONPanel">
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ICONbtnUpload" />
                                            </Triggers>
                                            <ContentTemplate>
                                                <asp:Button runat="server" ID="ICONbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" onClick="ICONbtnUpload_Click" style="display:none" />
                                                <div style="width:128px;height:90px;border:1px solid #ebeaea;text-align:center"><asp:Image ID="Icon" runat="server" style="display: block; margin:auto;width:100%" /></div> 
                                           </ContentTemplate>
                                        </asp:UpdatePanel> 
                                    </td>
                                    <td >
                                        <telerik:RadAsyncUpload Width="75px" runat="server" ID="ICONImage" OnClientFilesUploaded="ICONonFilesUploaded" HideFileInput="true" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpg,.jpeg,.png,.gif" ></telerik:RadAsyncUpload>               
                                    </td>
                                </tr>

                                <tr>
                                     <td class="fieldlabel">Tool Tip</td>
                                     <td colspan="2"><asp:TextBox ID="ToolTipTextBox" runat="server" Width="100%" Enabled="true" ViewStateMode="Enabled"></asp:TextBox></td>
                                </tr>
                                <tr>
                                     <td class="fieldlabel" rowspan="2">Description</td>
                                     <td rowspan="2" colspan="2"><asp:TextBox ID="DescriptionTextBox" runat="server" Width="100%" Height="90px" Enabled="true" TextMode="MultiLine" ViewStateMode="Enabled"></asp:TextBox></td>
                                </tr>
                        </table>
                    </fieldset>
                </div>
                </td>

               

                <td>
                    <div class="demo-container" id="decorationZone2" >
                        <fieldset>
                            <legend>Project Settings</legend>
                              <table  style="width:97%;padding:10px">
                                <tr>
                                   <td class="fieldlabel" style="width:100px;" >Restricted Views</td>
                                   <td><asp:RadioButtonList ID="RestrictedViews" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True">Enabled</asp:ListItem><asp:ListItem Value="False">Disabled</asp:ListItem></asp:RadioButtonList></td>
                                </tr>
                                <tr>
 
                                    <td class="fieldlabel">Auto Inclusion:</td>
                                    <td colspan="2"><asp:RadioButtonList ID="AutoInclusion" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True">Enabled</asp:ListItem><asp:ListItem Value="False">Disabled</asp:ListItem></asp:RadioButtonList></td>                                    
                                </tr>
                                <tr>
                                   <td class="fieldlabel">Notes:</td>
                                   <td colspan="2" ><asp:TextBox ID="Notes" runat="server" Width="100%" Height="150px" Enabled="true" TextMode="MultiLine" ViewStateMode="Enabled"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td colspan="3"><asp:Label style="font-style:italic" ID="AutoInclusionMsg" runat="server" Text="*&quot;Auto Inclusion&quot; cannot be modified due to administrative settings!" Visible="False"></asp:Label></td>
                                </tr>
                                 <tr>
                                    <td colspan="3"><asp:Label style="font-style:italic" ID="RestrictedViewsMsg" runat="server" Text="*&quot;Restricted Views&quot; cannot be modified due to administrative settings!" Visible="False"></asp:Label></td>
                                </tr>
                             </table>
            
                        </fieldset>
                    </div>
                </td>
            </tr>
            <tr>
            
                <td colspan="3" style="text-align:center;height:50px" ><telerik:RadButton ID="ApplyChanges" runat="server" Text="Apply Changes" OnClick="ApplyChanges_Click" ViewStateMode="Enabled"></telerik:RadButton></td>

            </tr>
        </table>

     <div id="LockPane" class="LockOn">
         <iframe  frameBorder="0" style="border:none;overflow:hidden;" width="50" height="50" src="Images/frameanimation.aspx" name="imgbox" id="imgbox" seamless="seamless"></iframe><br />
         <br />
         Loading Project Editor.....
     </div> 
    </form>


    
</body>
</html>
