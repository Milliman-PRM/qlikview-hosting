<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectEditor.aspx.cs" Inherits="ClientPublisher.ProjectEditor" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="jQuery/jquery.min.js" type="text/javascript"></script>
    <script src="Content/Script/jquery.min.v2.1.1.js"></script>
    <script src="Content/Script/bootstrap.min.v3.3.7.js"></script>
    <link href="Content/Style/bootstrap.css" rel="stylesheet" />
    <link href="Content/Style/MillframeStyle.css" rel="stylesheet" />

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

        /*bootstra specifc*/
        td{padding:4px!important}
        .form-control{height:30px}
        .table{margin-bottom:5px}
        .page-header   {padding-bottom: 2px; margin: 5px 0 7px;border-bottom: 1px solid #eee;}
        .RadUpload .ruInputs li {list-style: none;}
        .borderless td, .borderless th {border: none;}

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

            function onClientFileUploading(sender, args) {
                var data = args.get_data();
                var percents = data.percent;
                var fileSize = data.fileSize;
                var fileName = data.fileName;
                var uploadedBytes = data.uploadedBytes;

                if (percents <= 100) {
                    $('#myPleaseWait').modal('show');
                }
                else {
                    $('#myPleaseWait').modal('hide');
                }

            }

        </script>
    </telerik:RadCodeBlock>
</head>
<body style="overflow: hidden;" onload="SizeToFit()">

    <form id="form1" runat="server">


        <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
        <telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all" DecorationZoneID="decorationZone" ControlsToSkip="All" Skin="Silk"></telerik:RadFormDecorator>

        <!-- Modal Start here-->
        <div class="modal fade bs-example-modal-sm" id="myPleaseWait" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
            <div class="modal-dialog modal-sm">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Please Wait
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="progress">
                            <div class="progress-bar progress-bar-info  progress-bar-striped active" style="width: 100%"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal ends Here -->
        
        <div class="containerWrap">
            <div class="page-header engravedHeader">
                <h2>Edit Project Settings</h2>
            </div>
            <div class="left-div">
                <%--window for the user profile info--%>
                <div id="divUserProfileSettingsContainer" class="roundShadowContainer">
                    <h3>Client Visible Project Settings</h3>
                    <table class="table borderless">
                        <tbody>
                            <tr>
                                <td>
                                    <label for="QVWUpdatePanel" class="labelweak">Report QVW:</label></td>
                                <td>
                                    <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                    <asp:UpdatePanel runat="server" ID="QVWUpdatePanel">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="QVWbtnUpload" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:Button runat="server" ID="QVWbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" OnClick="QVWbtnUpload_Click" Style="display: none" />
                                            <asp:Label runat="server" ID="QVW" BorderStyle="Solid" BorderWidth="1px" BorderColor="LightGray" Width="100%"></asp:Label>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td style="width: 75px">
                                    <telerik:RadAsyncUpload Width="75px" runat="server" ID="QVWImage"
                                        OnClientFilesUploaded="QVWonFilesUploaded" HideFileInput="true" MultipleFileSelection="Disabled"
                                        AllowedFileExtensions=".qvw" MaxFileInputsCount="1" OnClientProgressUpdating="onClientFileUploading">
                                        <FileFilters>
                                            <telerik:FileFilter Description="Qlikview QVW files" Extensions="qvw" />
                                        </FileFilters>
                                    </telerik:RadAsyncUpload>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="QVWUpdatePanel" class="labelweak">User Manual:</label></td>
                                <td>
                                    <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                    <asp:UpdatePanel runat="server" ID="ManualUpdatePanel">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ManualbtnUpload" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:Button runat="server" ID="ManualbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" OnClick="ManualbtnUpload_Click" Style="display: none" />
                                            <asp:Label Width="100%" runat="server" ID="ManualLabel" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px">&nbsp;</asp:Label>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <telerik:RadAsyncUpload Width="75px" runat="server" ID="ManualImage" OnClientFilesUploaded="ManualonFilesUploaded" HideFileInput="true" MultipleFileSelection="Disabled" AllowedFileExtensions=".docx,.txt,.xlsx,.pdf,.html" MaxFileInputsCount="1">
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
                                <td>
                                    <label for="QVWUpdatePanel" class="labelweak">Report Icon:</label></td>
                                <td>
                                    <%-- have to put an update panel on each element to get unique behavior per post back -blah...--%>
                                    <asp:UpdatePanel runat="server" ID="ICONPanel">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ICONbtnUpload" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:Button runat="server" ID="ICONbtnUpload" UseSubmitBehavior="false" ClientIDMode="Static" OnClick="ICONbtnUpload_Click" Style="display: none" />
                                            <div style="width: 128px; height: 90px; border: 1px solid #ebeaea; text-align: center">
                                                <asp:Image ID="Icon" runat="server" Style="display: block; margin: auto; width: 100%" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <telerik:RadAsyncUpload Width="75px" runat="server" ID="ICONImage" OnClientFilesUploaded="ICONonFilesUploaded"
                                        HideFileInput="true" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpg,.jpeg,.png,.gif">
                                    </telerik:RadAsyncUpload>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <label for="QVWUpdatePanel" class="labelweak">Tool Tip:</label></td>
                                <td colspan="2">
                                    <asp:TextBox ID="ToolTipTextBox" runat="server" Width="100%" Enabled="true" ViewStateMode="Enabled"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="QVWUpdatePanel" class="labelweak">Description:</label></td>
                                <td rowspan="2" colspan="2">
                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="100%" Height="90px" Enabled="true" TextMode="MultiLine" ViewStateMode="Enabled"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="right-div">
                <%--window for the user Password Verification info--%>
                <div class="roundShadowContainer">
                    <h3>Project Settings</h3>
                    <table class="table borderless">
                        <tbody>
                            <tr>
                                <td>
                                    <label for="RestrictedViews" class="labelweak">Restricted Views:</label>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="RestrictedViews" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="True">Enabled</asp:ListItem>
                                        <asp:ListItem Value="False">Disabled</asp:ListItem>
                                    </asp:RadioButtonList></td>
                            </tr>
                            <tr>

                                <td>
                                    <label for="AutoInclusion" class="labelweak">Auto Inclusion:</label>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="AutoInclusion" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="True">Enabled</asp:ListItem>
                                        <asp:ListItem Value="False">Disabled</asp:ListItem>
                                    </asp:RadioButtonList></td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="Notes" class="labelweak">Notes:</label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Notes" runat="server" Width="100%" Height="150px" Enabled="true" TextMode="MultiLine" ViewStateMode="Enabled"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label Style="font-style: italic" ID="AutoInclusionMsg" runat="server" Text="*&quot;Auto Inclusion&quot; cannot be modified due to administrative settings!" Visible="False"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label Style="font-style: italic" ID="RestrictedViewsMsg" runat="server" Text="*&quot;Restricted Views&quot; cannot be modified due to administrative settings!" Visible="False"></asp:Label></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="space"></div>
                <div class="row">
                    <div class="center-block" style="float: none; width: 415px;">
                        <div class="space"></div>
                        <div class="col-md-8" style="float: none;">
                            <asp:Button ID="ApplyChanges" runat="server" Text="Apply Changes" ViewStateMode="Enabled" OnClick="ApplyChanges_Click" CssClass="btn btn-primary" />
                        </div>
                    </div>
                </div>
        </div>


        <div id="LockPane" class="LockOn">
            <iframe frameborder="0" style="border: none; overflow: hidden;" width="50" height="50" src="Images/frameanimation.aspx" name="imgbox" id="imgbox" seamless="seamless"></iframe>
            <br />
            <br />
            Loading Project Editor.....
        </div>
    </form>

</body>
</html>
