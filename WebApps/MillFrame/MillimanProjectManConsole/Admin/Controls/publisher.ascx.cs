#region using references

using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls;
using MillimanCommon;
using Telerik.Web.UI;
#endregion

public partial class admin_controls_access_rules : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        RemoveUploadCommand(RadFileExplorer1);

        RadFileExplorer1.Configuration.ViewPaths = new string[] { ConfigurationManager.AppSettings["QVDocumentRoot"] };

        RadFileExplorer1.Configuration.UploadPaths = RadFileExplorer1.Configuration.ViewPaths;
        RadFileExplorer1.Configuration.DeletePaths = RadFileExplorer1.Configuration.ViewPaths;
        RadFileExplorer1.Configuration.SearchPatterns = new[] { "*.hciprj" };
        RadFileExplorer1.Configuration.ContentProviderTypeName = typeof(CustomFileSystemProvider).AssemblyQualifiedName;

        RadFileExplorer1.Grid.ClientSettings.Resizing.AllowColumnResize = true;

        RadToolBarButton ToolbarSep = new RadToolBarButton();
        ToolbarSep.IsSeparator = true;
        RadFileExplorer1.ToolBar.Items.Add(ToolbarSep);

        RadToolBarButton Import = new RadToolBarButton("");
        
        Import.Value = "Import";
        Import.ToolTip = "Create a project from a signed QVW";
        Import.ImageUrl = "../../images/qlikview16x16.png";
        Import.ImagePosition = ToolBarImagePosition.Left;
        RadFileExplorer1.ToolBar.Items.Add(Import);

        Telerik.Web.UI.RadMenuItem Sep = new RadMenuItem();
        Sep.IsSeparator = true;
        RadFileExplorer1.TreeView.ContextMenus[0].Items.Add(Sep);
      
        RadFileExplorer1.TreeView.ContextMenus[0].Items[0].ImageUrl = "../../images/delete.png";
        RadFileExplorer1.TreeView.ContextMenus[0].Items[1].ImageUrl = "../../images/rename.png";
        RadFileExplorer1.TreeView.ContextMenus[0].Items[2].ImageUrl = "../../images/new.gif";
        RadFileExplorer1.TreeView.ContextMenus[0].Items[3].ImageUrl = "../../images/copy.png";
        RadFileExplorer1.TreeView.ContextMenus[0].Items[4].ImageUrl = "../../images/paste.png";


        RadFileExplorer1.GridContextMenu.Items[0].ImageUrl = "../../images/open.gif";
        RadFileExplorer1.GridContextMenu.Items[1].ImageUrl = "../../images/delete.png";
        RadFileExplorer1.GridContextMenu.Items[2].ImageUrl = "../../images/rename.png";
        RadFileExplorer1.GridContextMenu.Items[3].ImageUrl = "../../images/new.gif";
        RadFileExplorer1.GridContextMenu.Items[3].Enabled = false;  //turn this one off for now
        RadFileExplorer1.GridContextMenu.Items[4].ImageUrl = "../../images/copy.png";
        RadFileExplorer1.GridContextMenu.Items[5].ImageUrl = "../../images/paste.png";

        //never reload all
        //Telerik.Web.UI.RadMenuItem RMI = new Telerik.Web.UI.RadMenuItem("Reload All");
        //RMI.Value = "Reload";
        //RMI.Enabled = false;
        //RMI.ImageUrl = "../../images/reload.gif";
        //RadFileExplorer1.TreeView.ContextMenus[0].Items.Add(RMI);
        //

        Telerik.Web.UI.RadMenuItem RMI4a = new Telerik.Web.UI.RadMenuItem("Diff Local/Server Projects");
        RMI4a.Value = "Diff Local/Server Projects";
        RMI4a.ImageUrl = "../../images/diff.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4a);

        Telerik.Web.UI.RadMenuItem RMI4b = new Telerik.Web.UI.RadMenuItem("Publishing History");
        RMI4b.Value = "Publishing History";
        RMI4b.ImageUrl = "../../images/history.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4b);

        Telerik.Web.UI.RadMenuItem Sep2 = new RadMenuItem();
        Sep2.IsSeparator = true;
        RadFileExplorer1.GridContextMenu.Items.Add(Sep2);

        Telerik.Web.UI.RadMenuItem RMI4 = new Telerik.Web.UI.RadMenuItem("Edit Project Settings");
        RMI4.Value = "Edit Project Settings";
        RMI4.ImageUrl = "../../images/upload.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4);


        Telerik.Web.UI.RadMenuItem RMI3 = new Telerik.Web.UI.RadMenuItem("Create Project");
        RMI3.Value = "Create Project";
        RMI3.ImageUrl = "../../images/upload.png";
        RadFileExplorer1.TreeView.ContextMenus[0].Items.Add(RMI3);


        //Telerik.Web.UI.RadMenuItem Sep = new RadMenuItem();
        //Sep.IsSeparator = true;
        //RadFileExplorer1.GridContextMenu.Items.Add(Sep);

        Telerik.Web.UI.RadMenuItem RMI3Reset = new Telerik.Web.UI.RadMenuItem("Reset Project");
        RMI3Reset.Value = "Reset Project";
        RMI3Reset.ToolTip = "Reset the local project settings to be the same as the production server settings.";
        RMI3Reset.ImageUrl = "../../images/refresh-icon.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI3Reset);

        Telerik.Web.UI.RadMenuItem RMI2 = new Telerik.Web.UI.RadMenuItem("Re-populate QVW");
        RMI2.Value = "Re-populate QVW";
        RMI2.ImageUrl = "../../images/reload.gif";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2);

        Telerik.Web.UI.RadMenuItem RMI2a = new Telerik.Web.UI.RadMenuItem("Modify Project From Signature");
        RMI2a.Value = "Modify Project From Signature";
        RMI2a.ImageUrl = "../../images/magnify.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2a);

        Telerik.Web.UI.RadMenuItem RMI2b = new Telerik.Web.UI.RadMenuItem("Push to Production");
        RMI2b.Value = "Push to Production";
        RMI2b.ImageUrl = "../../images/publish.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2b);

        // RadFileExplorer1.TreeView.NodeClick += TreeView_NodeClick;
        RadFileExplorer1.TreeView.ContextMenuItemClick += TreeView_ContextMenuItemClick;
        //RadFileExplorer1.GridContextMenu.ItemClick += GridContextMenu_ItemClick;
  
        // 2nd parm - UniqueID value must match the id given to the column of data in 'CustomFileSystemProvider' GetFiles routine
        AddColumnToGrid("Local Saved", "LocalSaved", 150);
        AddColumnToGrid("Local/Server", "LocalServer", 80);
        AddColumnToGrid("Server Updated", "ServerUpdated", 100);
        AddColumnToGrid("<center>Server Updated by</center>", "ServerUpdatedBy", 150);
        //AddColumnToGrid("Resource", "Resource", 80);
        //AddColumnToGrid("Thumbnail", "Thumbnail", 120);
        AddColumnToGrid("<center>Designated Group</center>", "Group", 150);
        AddColumnToGrid("Description", "Description",300);
        AddColumnToGrid("Covisint Field", "CovisintField",150);
        AddColumnToGrid("Milliman Field", "MillimanField",150);
        AddColumnToGrid("Database", "FriendlyDBName", 150);
        RadFileExplorer1.Grid.Columns[0].HeaderStyle.Width = new Unit(250);
        RadFileExplorer1.Grid.Columns[1].HeaderStyle.Width = new Unit(80);
       
        RadFileExplorer1.ItemCommand += RadFileExplorer1_ItemCommand;
        RadFileExplorer1.Grid.Width = System.Web.UI.WebControls.Unit.Percentage(100.0);

        
    }

    void AddColumnToGrid(string HeaderText, string UniqueID, int Width)
    {
        GridTemplateColumn GTC = new GridTemplateColumn();
        
        GTC.HeaderText = HeaderText;
        GTC.SortExpression = GTC.HeaderText;
        GTC.UniqueName = UniqueID;
        GTC.DataField = GTC.HeaderText;
        GTC.Resizable = true;
        GTC.HeaderStyle.Width = new Unit(Width);
        //GTC.HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(12.5);
        RadFileExplorer1.Grid.Columns.Add(GTC);
    }

    void RadFileExplorer1_ItemCommand(object sender, RadFileExplorerEventArgs e)
    {
        string t = e.Path;
    }


    /// <summary>
    /// Remove the built in Upload command - does not have the behavior we need
    /// and cannot be customized
    /// </summary>
    /// <param name="fileExplorer"></param>
    private void RemoveUploadCommand(RadFileExplorer fileExplorer)
    {
        int i;// Global variable for that function 
        RadToolBar toolBar = fileExplorer.ToolBar;
        // Remove commands from the ToolBar control;
        i = 0;
        while (i < toolBar.Items.Count)
        {
            if (toolBar.Items[i].Value == "Upload")
            {
                toolBar.Items.RemoveAt(i);
                continue; // Next item
            }
            i++;// Next item
        }

        RadContextMenu treeViewContextMenu = fileExplorer.TreeView.ContextMenus[0];
        // Remove commands from the TreeView's ContextMenus control;
        i = 0;
        while (i < treeViewContextMenu.Items.Count)
        {
            if (treeViewContextMenu.Items[i].Value == "Upload")
            {
                treeViewContextMenu.Items.RemoveAt(i);
                continue;// Next item
            }
            i++;// Next item
        }

        RadContextMenu gridContextMenu = fileExplorer.GridContextMenu;
        // Remove commands from the GridContextMenu control;
        i = 0;
        while (i < gridContextMenu.Items.Count)
        {
            if (gridContextMenu.Items[i].Value == "Upload")
            {
                gridContextMenu.Items.RemoveAt(i);
                continue;// Next item
            }
            i++;// Next item
        }
    }


    void TreeView_ContextMenuItemClick(object sender, Telerik.Web.UI.RadTreeViewContextMenuEventArgs e)
    {
        if (string.Compare(e.MenuItem.Value, "Reload", true) == 0)
        {
            //"Selected context menu item has not been implemented"
            string script = "function NoImpl() { alert('Selected context menu item has not been implemented'); } ";
            //script += "Sys.Application.remove_load(NoImpl);}Sys.Application.add_load(NoImpl);";
            script += "NoImpl();";
            System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "keyalert", script, true);
        }
        else if (string.Compare(e.MenuItem.Value, "Create Project", true) == 0)
        {
            string NodePath = e.Node.Text;
            RadTreeNode RTN = e.Node.ParentNode;
            while (RTN != null)
            {
                NodePath = RTN.Text + @"\" + NodePath;
                RTN = RTN.ParentNode;
            }
            string script = "function f(){ $find(\"" + UploadWindow.ClientID + "\").setUrl('EnhancedUploadView.aspx?QVPath=" + System.Web.HttpUtility.UrlEncode(NodePath.ToLower().Replace("qvdocuments\\", "")) + "'); ";
            //script += " alert( $find(\"" + UploadWindow.ClientID + "\"); ";
            //script += " $find(\"" + UploadWindow.ClientID + "\").setSize(615,535);";
            script += " $find(\"" + UploadWindow.ClientID + "\").setSize(650, 540);";

            script += " $find(\"" + UploadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";

            System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "key", script, true);
            
        }
    }

    void GridContextMenu_ItemClick(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        if (string.Compare(e.Item.Value, "Re-populate QVW", true) == 0)
        {

        }
        else if (string.Compare(e.Item.Value, "Reset Project", true) == 0)
        {
            MillimanCommon.Alert.Show("Reset PRoject");
        }
        else if (string.Compare(e.Item.Value, "Edit Project Settings", true) == 0)
        {
            //stupid fileexplorer does almost everything client side - thus we need this hidden field voodoo
            string QVProject = hidden.Value;
            string NodePath = RadFileExplorer1.TreeView.SelectedNode.Text;
            RadTreeNode RTN = RadFileExplorer1.TreeView.SelectedNode.ParentNode;
            while (RTN != null)
            {
                NodePath = RTN.Text + @"\" + NodePath;
                RTN = RTN.ParentNode;
            }

           // Page.ClientScript.RegisterClientScriptBlock(GetType(), "WinLaunchScript", "Launch(Hi)", true);

            string script = "function f(){ $find(\"" + UploadWindow.ClientID + "\").setUrl('EnhancedUploadView.aspx?QVPath=" + System.Web.HttpUtility.UrlEncode(NodePath.ToLower().Replace("qvdocuments\\", "")) + "&QVName=" + QVProject + "'); ";
            //script += " $find(\"" + UploadWindow.ClientID + "\").setSize(650,535);";
            script += " $find(\"" + UploadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);f();";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "key", script, true);
        }
    }
}
