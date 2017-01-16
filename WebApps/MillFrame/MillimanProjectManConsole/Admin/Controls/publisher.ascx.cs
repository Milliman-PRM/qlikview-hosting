#region using references
using System;
using System.Configuration;
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

        RadToolBarButton ToolbarSep1 = new RadToolBarButton();
        ToolbarSep1.IsSeparator = true;
        RadFileExplorer1.ToolBar.Items.Add(ToolbarSep1);

        RadToolBarButton Import = new RadToolBarButton("");

        Import.Value = "Import";
        Import.ToolTip = "Create a project from a signed QVW";
        Import.ImageUrl = "../../images/qlikview16x16.png";
        Import.ImagePosition = ToolBarImagePosition.Left;
        RadFileExplorer1.ToolBar.Items.Add(Import);

        RadToolBarButton ToolbarSep2 = new RadToolBarButton();
        ToolbarSep2.IsSeparator = true;
        RadFileExplorer1.ToolBar.Items.Add(ToolbarSep2);

        RadToolBarButton UserGuide = new RadToolBarButton("");
        UserGuide.Value = "User Guide";
        UserGuide.ToolTip = "View the project management console user guide";
        UserGuide.ImageUrl = "../../images/publish.png";
        UserGuide.NavigateUrl = "../../UserGuide/PRM Project Management Console User Guide.html";
        UserGuide.Target = "_blank";
        UserGuide.ImagePosition = ToolBarImagePosition.Left;
        RadFileExplorer1.ToolBar.Items.Add(UserGuide);

        RadMenuItem Sep = new RadMenuItem();
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

        RadMenuItem RMI4a = new RadMenuItem("Diff Local/Server Projects");
        RMI4a.Value = "Diff Local/Server Projects";
        RMI4a.ImageUrl = "../../images/diff.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4a);

        RadMenuItem RMI4b = new RadMenuItem("Publishing History");
        RMI4b.Value = "Publishing History";
        RMI4b.ImageUrl = "../../images/history.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4b);

        RadMenuItem Sep2 = new RadMenuItem();
        Sep2.IsSeparator = true;
        RadFileExplorer1.GridContextMenu.Items.Add(Sep2);

        RadMenuItem RMI4 = new RadMenuItem("Edit Project Settings");
        RMI4.Value = "Edit Project Settings";
        RMI4.ImageUrl = "../../images/upload.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI4);

        RadMenuItem RMI3 = new RadMenuItem("Create Project");
        RMI3.Value = "Create Project";
        RMI3.ImageUrl = "../../images/upload.png";
        RadFileExplorer1.TreeView.ContextMenus[0].Items.Add(RMI3);

        RadMenuItem RMI3Reset = new RadMenuItem("Reset Project");
        RMI3Reset.Value = "Reset Project";
        RMI3Reset.ToolTip = "Reset the local project settings to be the same as the production server settings.";
        RMI3Reset.ImageUrl = "../../images/refresh-icon.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI3Reset);

        RadMenuItem RMI2 = new RadMenuItem("Re-populate QVW");
        RMI2.Value = "Re-populate QVW";
        RMI2.ImageUrl = "../../images/reload.gif";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2);

        RadMenuItem RMI2a = new RadMenuItem("Modify Project From Signature");
        RMI2a.Value = "Modify Project From Signature";
        RMI2a.ImageUrl = "../../images/magnify.png";
        RMI2a.Visible = false; //Story1827 - hide item
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2a);

        RadMenuItem RMI2b = new RadMenuItem("Push to Production");
        RMI2b.Value = "Push to Production";
        RMI2b.ImageUrl = "../../images/publish.png";
        RadFileExplorer1.GridContextMenu.Items.Add(RMI2b);

        RadFileExplorer1.TreeView.ContextMenuItemClick += TreeView_ContextMenuItemClick;

        // 2nd parm - UniqueID value must match the id given to the column of data in 'CustomFileSystemProvider' GetFiles routine
        AddColumnToGrid("Local Saved", "LocalSaved", 150);
        AddColumnToGrid("Local/Server", "LocalServer", 80);
        AddColumnToGrid("Server Updated", "ServerUpdated", 100);
        AddColumnToGrid("<center>Server Updated by</center>", "ServerUpdatedBy", 150);
        AddColumnToGrid("<center>Designated Group</center>", "Group", 150);
        AddColumnToGrid("Description", "Description", 300);
        AddColumnToGrid("Covisint Field", "CovisintField", 150, false); //Story1827 - hide item
        AddColumnToGrid("Milliman Field", "MillimanField", 150);
        AddColumnToGrid("Database", "FriendlyDBName", 150);
        RadFileExplorer1.Grid.Columns[0].HeaderStyle.Width = new Unit(250);
        RadFileExplorer1.Grid.Columns[1].HeaderStyle.Width = new Unit(80);

        RadFileExplorer1.ItemCommand += RadFileExplorer1_ItemCommand;
        RadFileExplorer1.Grid.Width = System.Web.UI.WebControls.Unit.Percentage(100.0);

        HideUnNeededControls();
    }

    private void HideUnNeededControls()
    {   //this is the implementation of Story 1827,  hiding un-needed control
        //almost all changes are here,  RMI2a control above is hidden "Modify Project From Signature" button (done in place to keep from FINDING again)
        //changes were made in the RadMenu attributes in aspx page to hide some of the default items globally

        RadFileExplorer1.ToolBar.Items[2].Visible = false;//Story1827 - hide item OPEN
        RadFileExplorer1.ToolBar.Items[6].Visible = false;//Story1827 - hide item LISTVIEW
        RadFileExplorer1.ToolBar.Items[7].Visible = false;//Story1827 - hide item ICONVIEW

        RadFileExplorer1.TreeView.ContextMenus[0].Items[1].Visible = false;  //Story1827 - hide item
        RadFileExplorer1.TreeView.ContextMenus[0].Items[3].Style.Add("display", "none");
        RadFileExplorer1.TreeView.ContextMenus[0].Items[4].Style.Add("display", "none");
        RadFileExplorer1.GridContextMenu.Items[0].Visible = false;//Story1827 - hide item
        RadFileExplorer1.GridContextMenu.Items[2].Visible = false;//Story1827 - hide item
        RadFileExplorer1.GridContextMenu.Items[3].Visible = false;//Story1827 - hide item
        RadFileExplorer1.GridContextMenu.Items[4].Visible = false;//Story1827 - hide item
        RadFileExplorer1.GridContextMenu.Items[5].Visible = false;//Story1827 - hide item

        RadFileExplorer1.GridContextMenu.Items.RemoveAt(3); //yikes, that is not hiding - yes tis true, however NEW (#3) is telerik default button, thus it cannot be hidden, only removed
    }

    void AddColumnToGrid(string HeaderText, string UniqueID, int Width, bool IsVisible = true)
    {
        GridTemplateColumn GTC = new GridTemplateColumn();

        GTC.HeaderText = HeaderText;
        GTC.SortExpression = GTC.HeaderText;
        GTC.UniqueName = UniqueID;
        GTC.DataField = GTC.HeaderText;
        GTC.Resizable = true;
        GTC.HeaderStyle.Width = new Unit(Width);
        GTC.Visible = IsVisible;  //Story1827 - added to hide column for covisent
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

    void TreeView_ContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
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

    void GridContextMenu_ItemClick(object sender, RadMenuEventArgs e)
    {
        if (string.Compare(e.Item.Value, "Re-populate QVW", true) == 0)
        {
        }
        else if (string.Compare(e.Item.Value, "Reset Project", true) == 0)
        {
            Alert.Show("Reset PRoject");
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

            string script = "function f(){ $find(\"" + UploadWindow.ClientID + "\").setUrl('EnhancedUploadView.aspx?QVPath=" + System.Web.HttpUtility.UrlEncode(NodePath.ToLower().Replace("qvdocuments\\", "")) + "&QVName=" + QVProject + "'); ";
            script += " $find(\"" + UploadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);f();";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "key", script, true);
        }
    }
}
