<%@ Page Language="C#" AutoEventWireup="true" Inherits="EnhancedUploadView" CodeBehind="EnhancedUploadView.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link id="lnkBS" runat="server" rel="stylesheet" type="text/css" href="../Content/Style/bootstrap.css" />
    <link id="lnkBSD" runat="server" rel="stylesheet" type="text/css" href="../Content/Style/bootstrap-dialog.min.css" />
    <link id="Link1" runat="server" rel="stylesheet" type="text/css" href="~/Content/Style/MillframeStyle.css" />
    <style type="text/css">
        html {
            overflow: scroll;
        }

        html, button, input, select, textarea, label {
            font-family: arial,"Times New Roman",Times,serif,sans-serif;
            font-size: 12px;
            color: #222;
        }

        body {
            margin: 20px;
        }

        .containerWrap {
            width: 81%;
            margin: 0 auto;
        }

        .left-div {
            max-width: 800px;
        }

        .right-div {
            max-width: 800px;
        }

        #divGeneralProjectInfo {
            width: 534px;
        }

        #divGroupAssignment {
            width: 534px;
        }

        .roundShadowContainer ul, li {
            list-style-type: none;
        }
    </style>

    <script src="../Content/Script/jquery.v1.7.1.js" type="text/javascript"></script>
    <script src="../Content/Script/jquery.min.v2.1.1.js" type="text/javascript"></script>
    <script src="../Content/Script/bootstrap.min.v3.3.7.js" type="text/javascript"></script>
    <script src="../Content/Script/bootstrap-dialog.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function getRadWindow() {
            var oWindow = null;
            if (window.radWindow) {
                oWindow = window.radWindow;
            }
            else if (window.frameElement != null) {
                if (window.frameElement.radWindow) {
                    oWindow = window.frameElement.radWindow;
                }
            }
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
            if (getRadWindow()) {
                getRadWindow().autoSize();
            }
        }

    </script>
    <title>Project Upload</title>

</head>
<body style="font-size: 11pt; font-weight: 400; background-image: url( ../images/dialog-bg.png ); background-size: 100% 100%" onload="AutoSize();">
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />

        <div class="containerWrap">
            <%--divImportantHint hint--%>
            <div id="divImportant" class="divImportant" style="width: 23px; float: left;">
                <img id="img2" src="~/Content/Images/Info-blue.png" runat="server" width="18" height="18" alt="Info" />
                <i class="glyphicons glyphicons-info-sign"></i>
                <div id="divImportantHint">
                    <div class="alert alert-warning infoBox text-justify">
                        <strong>Important!</strong>
                        <br />
                        If you copy and paste data from Excel, Word, Notepad, or other program there might be hidden cotrol characters in text like Backspace, Tab or New Line Feed. 
                    <br />
                        Please make sure your text does not include any control characters.        
                    </div>
                </div>
            </div>
            <%--divImportantHint hint--%>
            <div class="page-header engravedHeader">
                <h2>Project Upload  <small>General Project Information</small></h2>
            </div>
            <div class="space"></div>
            <div class="left-div">
                <div id="divGeneralProjectInfo" class="roundShadowContainer">
                    <div class="row">
                        <div class="col-lg-12">
                            <h3>General Project Information</h3>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-sm-4">
                            <label for="ProjectName" class="labelweak">Project Name:</label>
                        </div>
                        <div class="col-sm-8">
                            <asp:TextBox ID="ProjectName" runat="server" Width="86%"
                                ToolTip="Name of this project - will be visible to client user as title"
                                OnTextChanged="ProjectName_TextChanged" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-sm-4">
                            <label for="QVWName" class="labelweak">QVW Name:</label>
                        </div>
                        <div class="col-sm-8">
                            <asp:TextBox ID="QVWName" runat="server" Width="86%"
                                ToolTip="Name of this project - will be visible to client user as title"
                                OnTextChanged="ProjectName_TextChanged" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-sm-4">
                            <label for="Description" class="labelweak">Project Description:</label>
                        </div>
                        <div class="col-sm-8">
                            <asp:TextBox Height="97px" ID="Description" runat="server" Width="86%"
                                TextMode="MultiLine"
                                ToolTip="Description of the problem this report solves - will be visible to client user."
                                class="form-control" />
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-sm-4">
                            <label for="Notes" class="labelweak">Project Notes:</label>
                        </div>
                        <div class="col-sm-8">
                            <asp:TextBox Height="97px" ID="Notes" runat="server" Width="86%" TextMode="MultiLine"
                                ToolTip="Notes associated with this project - not visible to client users"
                                class="form-control" />
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-sm-4">
                            <label for="Tooltip" class="labelweak">Project Tooltip:</label>
                        </div>
                        <div class="col-sm-8">
                            <asp:TextBox Height="97px" ID="Tooltip" runat="server" Width="86%"
                                TextMode="SingleLine"
                                ToolTip="Tooltip that will be displayed on mouse-over of the launch icon - visible to client users"
                                class="form-control" />
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-md-4">
                            <label for="PreviewImage" class="labelweak">Project Thumbnail(128x128)(*.jpg,*.gif, *.png):</label>
                        </div>
                        <div class="col-md-8">
                            <asp:Image ID="PreviewImage" runat="server" Width="105px" Height="105px" />
                            <br />
                            <telerik:RadAsyncUpload runat="server" ID="PresentationThumbnail" Style="width: 95%"
                                AllowedFileExtensions="png,gif,jpg,jpeg" MaxFileSize="1000000000" MaxFileInputsCount="1">
                            </telerik:RadAsyncUpload>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-md-4">
                            <label for="UserManualLabel" class="labelweak">User Manual:</label>
                        </div>
                        <div class="col-md-8">
                            <asp:HyperLink runat="server" ID="UserManualLabel" ToolTip="User manual associated with this QVW - click to launch"></asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
            <div class="space"></div>
            <div class="right-div">
                <div id="divGroupAssignment" class="roundShadowContainer">
                    <div class="row">
                        <div class="col-lg-12">
                            <h3>Group Assignment/Modification</h3>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="text-left">
                                <label for="NewGroup" class="labelweak">New Group:</label>
                                <asp:TextBox runat="server" ID="NewGroupName" Width="45%"></asp:TextBox>
                                <asp:Button runat="server" ID="Button1" Text="Create Group" ToolTip="Create a group on the production server."
                                    OnClick="CreateGroup_Click" CssClass="btn btn-secondary btn-sm" />
                                <asp:Button runat="server" ID="Button2" Text="Delete Group" ToolTip="Delete the 'checked' groups on the production server"
                                    OnClick="Delete_Click" CssClass="btn btn-secondary btn-sm" />

                            </div>
                            <div class="row">&nbsp;</div>
                            <div style="height: 400px; min-height: 50px; overflow-y: auto; margin: 0 auto; background: #eceaea;">
                                <asp:CheckBoxList runat="server" ID="SelectedGroups"
                                    RepeatColumns="1" onclick="SetTooltip()">
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="space"></div>
            <div class="row">
                <div class="center-block" style="float: none; width: 415px;">
                    <div class="col-md-9">
                        <asp:Button ID="Upload" runat="server" Text="Save" OnClick="Upload_Click" OnClientClick="Submitted()"
                            CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>
            <div id="ImBusy" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; display: none">
                <table style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; visibility: visible">
                    <tr>
                        <td style="vertical-align: middle; text-align: center;">
                            <img src="../images/ajax-loader.gif" style="border-width: 0px" /><br />
                            Project Upload and Configuration in Progress
                        </td>
                    </tr>
                </table>
            </div>
        </div>

    </form>
    <script type="text/javascript">
        jQuery(function () {
            jQuery('#tabs a:last').tab('show')
        })

        //***************** display the importatn hint next to the image ******************************//
        var moveLeft = 10;
        var moveDown = 20;
        $('.divImportant').hover(function (e) {
            $('#divImportantHint').show();
        }, function () {
            $('#divImportantHint').hide();
        });

        //disable all the button in the form while uploading
        function Submitted() {
            document.title = "Project Settings Modification";
            var Description = document.getElementById("Description");
            var Notes = document.getElementById("Notes");
            var Tooltip = document.getElementById("Tooltip");

            var badData = false;
            if (Description.value != '') {
                badData = checkInvalidChars(Description.value);
            }
            if (Notes.value != '') {
                badData = checkInvalidChars(Notes.value);
            }
            if (Tooltip.value != '') {
                badData = checkInvalidChars(Tooltip.value);
            }

            if (badData) {
                badDataError();
                return false;
            }

            return true;
        }

        function SetTooltip() {
            var CheckList = document.getElementById('<%= SelectedGroups.ClientID %>');
            if (CheckList) {
                var CheckBoxListArray = CheckList.getElementsByTagName('input');
                var TooltipText = '';
                for (var i = 0; i < CheckBoxListArray.length; i++) {
                    var checkBoxRef = CheckBoxListArray[i];

                    if (checkBoxRef.checked == true) {
                        if (TooltipText != '')
                            TooltipText += "\x0A";  //add a new line
                        TooltipText += checkBoxRef.value;
                    }
                }
                CheckList.title = TooltipText;
            }
        }


        var Description = document.getElementById("Description");
        Description.addEventListener('blur', function () {
            var inputVal = this.value;
            // Do whatever you want with the input
            checkInvalidChars(inputVal);
        });

        var Notes = document.getElementById("Notes");
        Notes.addEventListener('blur', function () {
            var inputVal = this.value;
            // Do whatever you want with the input
            checkInvalidChars(inputVal);
        });

        var Tooltip = document.getElementById("Tooltip");
        Tooltip.addEventListener('blur', function () {
            var inputVal = this.value;
            // Do whatever you want with the input
            checkInvalidChars(inputVal);
        });

        function checkInvalidChars(controlValue) {
            //not allowed chars
            if (controlValue != "") {
                //var regexInvalid = new RegExp(/[><#]/g);
                var regexInvalid = new RegExp(/[\x00-\x1F\x7F-\x9F]/u);//find control characters in utf-8 string
                if (controlValue.match(regexInvalid)) {
                    badDataError();
                    return false;
                }
            }
            return true;
        }

        function badDataError() {
            showErrorAlert('You have enered invalid character(s) in one of the text field. Please correct the entry before you save data.');
        }

        //show error
        function showErrorAlert(alertMessage) {
            BootstrapDialog.show({
                title: 'Data Entry Issue',
                message: alertMessage,
                type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
                closable: true, // <-- Default value is false
                draggable: true, // <-- Default value is false
                buttons: [{
                    label: 'OK',
                    hotkey: 13, // Keycode of keyup event of key 'A' is 65.
                    cssClass: 'btn-warning',
                    action: function (dialog) {
                        dialog.close();
                    }
                }],
            });
        }

    </script>
</body>
</html>
