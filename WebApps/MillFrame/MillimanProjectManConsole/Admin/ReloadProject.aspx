<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReloadProject.aspx.cs" Inherits="MillimanProjectManConsole.ReloadProject" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Project Update</title>
</head>
<body style="background-color:white;font-size:12px">
    <form id="form1" runat="server" style="text-align:center">
        	<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		        </Scripts>
	        </telerik:RadScriptManager>

          <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                <script type="text/javascript">
                    var uploadedFilesCount = 0;
                    var isEditMode;
                    function validateRadUpload(source, e) {
                        // When the RadGrid is in Edit mode the user is not obliged to upload file.
                        if (isEditMode == null || isEditMode == undefined) {
                            e.IsValid = false;
                            if (uploadedFilesCount > 0) {
                                e.IsValid = true;
                            }
                        }
                        isEditMode = null;
                    }

                    function OnClientFileUploaded(sender, eventArgs) {
                        uploadedFilesCount++;
                    }

                    function OnCommand(sender, eventArgs) {

                        var x = eventArgs.get_commandName().toLowerCase();
                        if (eventArgs.get_commandName().toLowerCase() == "initinsert") {
                            var grid = $find("<%=RadGrid1.ClientID %>");
                            var MasterTable = grid.get_masterTableView();
                            var Rows = MasterTable.get_dataItems();
                            if (Rows.length >= 6) {
                                alert("Warning! Only 6 items may be attached to a report for download by a user.\n\n Please delete/or edit an existing item.");
                                eventArgs.set_cancel(true);
                            }
                        }
                    }

                </script>

            </telerik:RadCodeBlock>
            <telerik:RadAjaxManager ID="RadAjaxManager2" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="RadGrid1">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="RadGrid1"></telerik:AjaxUpdatedControl>
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <asp:Label ID="QVWName" runat="server" Visible="false"></asp:Label>

            <table style="border:1px solid black;  width:100%">
            <tr >
                <td colspan="3" style="text-align:center; background-image:url( ../images/header.gif );border-bottom:1px solid black" >
                    <asp:Label ID="UpdateLabel" Text="Project Update" runat="server"></asp:Label>
                </td>
            </tr>
                <tr><td colspan="3">&nbsp;</td></tr>
             <tr>
                <td style="width:300px;text-align:right">New Report</td>
                <td ><asp:HyperLink runat="server" ID="HyperLink7" ToolTip="New QVW to be associated with this project."></asp:HyperLink></td>
                <td colspan="2">
<%--                    <asp:FileUpload ID="ReloadUpload" runat="server" width="100%"/>--%>
                    <telerik:RadAsyncUpload runat="server" ID="QVWUpload" 
                        AllowedFileExtensions="qvw" MaxFileSize="1000000000" MaxFileInputsCount="1">
                    </telerik:RadAsyncUpload>

                </td>
            </tr>
            <tr>
                <td style="text-align:right;">User Manual</td>
                <td ><asp:HyperLink runat="server" ID="UserManualLabel" ToolTip="User manual associated with this QVW - click to launch"></asp:HyperLink></td>
                <td colspan="2">
<%--                    <asp:FileUpload id="UserManualUpload" runat="server"  width="100%" />--%>
                   <telerik:RadAsyncUpload runat="server" ID="UserManual" 
                        AllowedFileExtensions="htm,html,txt,xml,zip,pdf,doc,docx" MaxFileSize="1000000000" MaxFileInputsCount="1">
                    </telerik:RadAsyncUpload>
                </td>
            </tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr>
                <td colspan="3" >
                    <telerik:RadGrid runat="server" ID="RadGrid1" AllowPaging="false" AllowSorting="false"
                        AutoGenerateColumns="False" ShowStatusBar="True" GridLines="None"
                        OnItemCreated="RadGrid1_ItemCreated" PageSize="6" OnInsertCommand="RadGrid1_InsertCommand"
                        OnNeedDataSource="RadGrid1_NeedDataSource" OnDeleteCommand="RadGrid1_DeleteCommand"
                        OnUpdateCommand="RadGrid1_UpdateCommand" OnItemCommand="RadGrid1_ItemCommand"  ClientSettings-ClientEvents-OnCommand="OnCommand" >
                        <PagerStyle Mode="NumericPages" AlwaysVisible="false"></PagerStyle>
                        <MasterTableView Width="100%" CommandItemDisplay="Top" DataKeyNames="ID" >
                            <CommandItemSettings AddNewRecordText="Add New Download Item"  />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton">
                                    <HeaderStyle Width="36px"></HeaderStyle>
                                </telerik:GridEditCommandColumn>
                                <telerik:GridTemplateColumn HeaderText="Name" UniqueName="DisplayName" SortExpression="Name" >
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblName" Text='<%# Eval("DisplayName") %>' visible="true" ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadTextBox runat="server" Width="200px" ID="txbName" Text='<%# Eval("DisplayName") %>' Enabled="true" Visible="false">
                                        </telerik:RadTextBox>
<%--                                        <asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server" ControlToValidate="txbName"
                                            ErrorMessage="Please, enter a name!" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                    </EditItemTemplate>
                                    <HeaderStyle Width="30%"></HeaderStyle>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Description" DataField="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# TrimDescription(Eval("Description") as string) %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadTextBox ID="txbDescription" Width="300px" runat="server" TextMode="MultiLine"
                                            Text='<%# Eval("Description") %>' Height="150px">
                                        </telerik:RadTextBox>
                                    </EditItemTemplate>
                                    <ItemStyle VerticalAlign="Top"></ItemStyle>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="Name" HeaderText="Upload Name" UniqueName="Upload" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFileName" runat="server" Text='<%# TrimDescription(Eval("Name") as string) %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                       <telerik:RadAsyncUpload runat="server" ID="AsyncUpload1" OnClientFileUploaded="OnClientFileUploaded"
                                            AllowedFileExtensions="txt,xml,zip,pdf,csv,gif,jpg,jpeg,png,bmp,doc,docx,xls,xlsx,xlsm,ppt,pps,pptx,html,htm" MaxFileSize="1000000000" OnFileUploaded="AsyncUpload1_FileUploaded" MaxFileInputsCount="1">
                                        </telerik:RadAsyncUpload>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton">
                                    <HeaderStyle Width="36px"></HeaderStyle>
                                </telerik:GridButtonColumn>
                            </Columns>
                            <EditFormSettings>
                                <EditColumn ButtonType="ImageButton">
                                </EditColumn>
                            </EditFormSettings>
                            <PagerStyle AlwaysVisible="True"></PagerStyle>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
  
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:center"><asp:Button runat="server" ID="Button1" Text="Update Project" OnClick="Upload_Click" OnClientClick="return Validate()"  /></td>
            </tr>
            </table>

    </form>

    <script type="text/javascript">
        function Validate() {
            return true;
        }

        var uploadedFilesCount = 0;
        var isEditMode;
        function validateRadUpload(source, e) {
            // When the RadGrid is in Edit mode the user is not obliged to upload file.
            if (isEditMode == null || isEditMode == undefined) {
                e.IsValid = false;
                if (uploadedFilesCount > 0) {
                    e.IsValid = true;
                }
            }
            isEditMode = null;
        }

        function OnClientFileUploaded(sender, eventArgs) {
            uploadedFilesCount++;
        }

        function OnCommand(sender, eventArgs) {

            var x = eventArgs.get_commandName().toLowerCase();
            if (eventArgs.get_commandName().toLowerCase() == "initinsert") {
                var grid = $find("<%=RadGrid1.ClientID %>");
                    var MasterTable = grid.get_masterTableView();
                    var Rows = MasterTable.get_dataItems();
                    if (Rows.length >= 6) {
                        alert("Warning! Only 6 items may be attached to a report for download by a user.\n\n Please delete/or edit an existing item.");
                        eventArgs.set_cancel(true);
                    }
                }
            }
    </script>
</body>
</html>
