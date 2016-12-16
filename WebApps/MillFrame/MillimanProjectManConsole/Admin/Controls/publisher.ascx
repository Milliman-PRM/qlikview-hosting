<%@ Control Language="C#" AutoEventWireup="true" Inherits="admin_controls_access_rules" CodeBehind="publisher.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc2" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<contenttemplate>
    <div  style="width:100%;height:100%;margin:5px" >

    <div style="width:100%;height:100%;border:solid 1px lightgray" id="RadFileExplorerContainer">
        <telerik:RadFileExplorer  ID="RadFileExplorer1"  runat="server" EnableCopy="False"  OnClientItemSelected="OnClientItemSelected" 
            Configuration-AllowFileExtensionRename="False" Configuration-AllowMultipleSelection="False" Width="100px"
             OnClientLoad="OnClientLoad" ViewStateMode="Enabled" Height="100px" BorderStyle="Solid" BorderWidth="1" BorderColor="LightGray" EnableOpenFile="False" EnableCreateNewFolder="True" />
        <asp:HiddenField ID="hidden" runat="server" /> <%--needed to push selected node in grid back to codebehind--%>
    </div>  

        <telerik:RadWindow ID="UploadWindow" runat="server" Animation="Fade" AutoSize="False" Behaviors="Close, Move, Reload" Modal="True"  
            InitialBehaviors="Close" VisibleStatusbar="False" OnClientClose="OnClientclose"  ShowContentDuringLoad="False" DestroyOnClose="False" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">  </asp:UpdatePanel>
    </div>

</contenttemplate>
<script language="javascript" type="text/javascript">

    var LastSelectedFilePath;
    function OnClientItemSelected(sender, args) {
        LastSelectedFilePath = args.get_path();

        var EE = document.getElementById('<%= hidden.ClientID %>');
        EE.value = args.get_item()._name;

        if (EE.value.indexOf(".hciprj") > 0) {
            //launch rad window somehow
        }
    }

    function OnClientLoad(explorer, args) {
        setTimeout(function () { ResizeExplorer(); }, 0);
        explorer.refresh();
    }

    function Closer() {
        var oExplorer = $find("<%=RadFileExplorer1.ClientID%>");
         if (oExplorer)
             oExplorer.refresh();

     }

     function OnClientclose(sender, eventArgs) {
         sender.hide();
         Closer(); //call to refresh
     }

     //resize voodoo
     var resized = false;
     /* * Resize the RadFileExplorer */
     function ResizeExplorer() {
         if (resized == false) {
             resized = true;
             var ClientHeight = 0;
             var ClientWidth = 0;
             var h = document.getElementById('RadFileExplorerContainer');
             if (h) {
                 var footer = document.getElementById('footer');
                 var footerHeight = footer ? footer.clientHeight : 25;
                 // ClientWidth = h.clientWidth;
                 // ClientHeight = h.clientHeight;
                 ClientWidth = h.offsetWidth - 5;
                 ClientHeight = h.offsetHeight - footerHeight;
             }

             var explorer = $find("<%=RadFileExplorer1.ClientID%>");
             if (explorer && h) {
                 var div = explorer.get_element();
                 //resize explorer container div 
                 div.style.height = ClientHeight + "px";
                 div.style.width = ClientWidth + "px";
                 div.style.border = "1px";

                 var toolbar = explorer.get_toolbar();
                 var grid = explorer.get_grid();

                 //var domsplitter = document.getElementById('ctl00_ContentPlaceHolder1_access1_RadFileExplorer1_splitter');
                 explorer._splitter.Resize(ClientWidth, (ClientHeight - toolbar.get_element().offsetHeight));

                 grid.get_element().style.height = ((ClientHeight - toolbar.get_element().offsetHeight)) + "px";
                 grid.repaint();

             }
         }
     }

     var resizeTimer;
     function Resizer() {
         clearTimeout(resizeTimer);
         resized = false;
         resizeTimer = setTimeout(ResizeExplorer, 200);
     };



     //var ActiveTreeNodeText;
     //function TreeItemclicked(sender, args) {
     //    ActiveTreeNodeText = args.get_path();
     //    alert(ActiveTreeNodeText);
     //}
     function GridItemClicked(sender, args) {
         var explorer = $find("<%=RadFileExplorer1.ClientID%>");
         var SelectedGridItem = explorer.get_grid().get_selectedItems()[0].get_dataItem().Name;
         var ContextItem = args.get_item().get_value().toUpperCase();

         if (ContextItem == "EDIT PROJECT SETTINGS") {

             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                 if (MyUploadWindow) {
                     MyUploadWindow.Title = "Edit Project Settings";
                     MyUploadWindow.setUrl('EnhancedUploadView.aspx?QVPath=' + LastSelectedFilePath.toLowerCase().replace("qvdocuments/", ""));

                     MyUploadWindow.setSize(650, 540);
                     MyUploadWindow.show();
                 }
             }
             else {
                 alert("A project was not selected to update from the file list!");
             }
         }
         else if (ContextItem == "DIFF LOCAL/SERVER PROJECTS") {

             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                 if (MyUploadWindow) {
                     MyUploadWindow.Title = "Diff Local/Server Projects";
                     MyUploadWindow.setUrl('Diff.aspx?ProjectPath=' + LastSelectedFilePath);
                     MyUploadWindow.setSize(650, 540);
                     MyUploadWindow.show();
                 }
             }
             else {
                 alert("A project was not selected to update from the file list!");
             }
         }
         else if (ContextItem == "PUBLISHING HISTORY") {

             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                 if (MyUploadWindow) {
                     MyUploadWindow.Title = "Publishing History";
                     MyUploadWindow.setUrl('History.aspx?ProjectPath=' + LastSelectedFilePath);
                     MyUploadWindow.setSize(650, 540);
                     MyUploadWindow.show();
                 }
             }
             else {
                 alert("A project was not selected to update from the file list!");
             }
         }
         else if (ContextItem == "RE-POPULATE QVW") {
             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                 if (MyUploadWindow) {
                     MyUploadWindow.Title = "QVW Data Reload";
                     MyUploadWindow.setUrl('ReloadProject.aspx?ProjectPath=' + LastSelectedFilePath);
                     MyUploadWindow.setSize(800, 700);
                     MyUploadWindow.show();
                 }
             }
             else {
                 alert("A project to reload should be selected from the file list.");
             }
         }
         else if (ContextItem == "RESET PROJECT") {
             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 if (confirm("Resetting the project to match the state of the project on the production server will result in losing all local changes.  Do you wish to continue?") == true) {
                     var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                     if (MyUploadWindow) {
                         MyUploadWindow.Title = "Reset Project";
                         MyUploadWindow.setUrl('ResetProject.aspx?ProjectPath=' + LastSelectedFilePath);
                         MyUploadWindow.setSize(800, 700);
                         MyUploadWindow.show();
                     }
                 }
             }
             else {
                 alert("A project to reload should be selected from the file list.");
             }
         }
         else if (ContextItem == "PUSH TO PRODUCTION") {
             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                 if (MyUploadWindow) {
                     MyUploadWindow.Title = "Uploading to Production Server";

                     MyUploadWindow.setUrl('../ComplexReporting/ReportingShell.aspx?ProjectPath=' + LastSelectedFilePath);
                     MyUploadWindow.setSize(800, 700);
                     MyUploadWindow.show();
                 }
             }
             else {
                 alert("A project to publish should be selected from the file list.");
             }
         }
         else if (ContextItem == "MODIFY PROJECT FROM SIGNATURE") {
             if (LastSelectedFilePath.indexOf(".hciprj") != -1) {
                 var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
                     if (MyUploadWindow) {
                         MyUploadWindow.Title = "Rename from QVW Signature";
                         MyUploadWindow.setUrl('../ComplexRename/RenameFromQVW.aspx?ProjectPath=' + LastSelectedFilePath);
                         MyUploadWindow.setSize(800, 700);
                         MyUploadWindow.show();
                     }
                 }
                 else {
                     alert("A project to publish should be selected from the file list.");
                 }
             }
}

document.body.onload = function () {

    if (window.addEventListener) {
        window.addEventListener('resize', Resizer);
    }
    else {
        window.attachEvent('onresize', Resizer);
    }

    if (!window.onresize) {
        window.onresize = Resizer;
    }
    var explorer = $find("<%=RadFileExplorer1.ClientID%>");
    if (explorer) {
        explorer.get_gridContextMenu().add_itemClicked(GridItemClicked);
        explorer.get_toolbar().add_buttonClicked(toolbarClicked);
    }
}


function toolbarClicked(toolbar, args) {
    var buttonValue = args.get_item().get_value();
    if (buttonValue == "Import") {
        var MyUploadWindow = $find("<%=UploadWindow.ClientID%>");
             if (MyUploadWindow) {
                 MyUploadWindow.Title = "Create Project from QVW Import";
                 MyUploadWindow.setUrl('QVWImport.aspx');
                 MyUploadWindow.setSize(650, 540);
                 MyUploadWindow.show();
             }
         }
     }

</script>
<%-- jquery js --%>
<%--<uc2:jquery ID="jquery1" runat="server" />--%>
