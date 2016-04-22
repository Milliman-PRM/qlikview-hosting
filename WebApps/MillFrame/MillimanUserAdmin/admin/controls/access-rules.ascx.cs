#region using references

using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls;
using MillimanCommon;

#endregion

public partial class admin_controls_access_rules : System.Web.UI.UserControl
{
    // --------------------------------------------------------------------------------------
    // Code from Dan Clam's tutorial http://aspnet.4guysfromrolla.com/articles/052307-1.aspx
    // replaced UserList dropdown with simple textbox for scalability. Admin can type in 
    // username rather than select from dropdown of perhaps thousands of users. 
    // Also rearranged and modified existing code base.
    // --------------------------------------------------------------------------------------

    #region global variables

    private string VirtualSiteRoot; // set in Page Load
    string selectedFolderName;

    //if we want to show even hidden folders, then add a checkbox to the screen and set variable "ShowAllNodes"
    //based on the UI setting
    bool ShowAllNodes = false;
    string _HIDDENFOLDER_ = "reduceduserqvws";

    #endregion

    #region page_init - get roles and requests

    // get all roles and request - and the folder name only if it is not a postback
    protected void Page_Init(object sender, EventArgs e)
    {
        UserRoles.DataSource = Roles.GetAllRoles();
        UserRoles.DataBind();

        if(IsPostBack)
        {
            selectedFolderName = "";
        }
        else
        {
            selectedFolderName = Request.QueryString["selectedFolderName"];
        }
    }

    #endregion

    #region page_load - populate folder structure

    // populate folder structure on first page load, not on postback
    protected void Page_Load(object sender, EventArgs e)
    {
        VirtualSiteRoot = ConfigurationManager.AppSettings["QVDocumentRoot"]; //always get this,  need by several routines
        VirtualSiteRoot = VirtualSiteRoot.EndsWith(@"\") ? VirtualSiteRoot : VirtualSiteRoot + @"\";
        if(!IsPostBack)
        {
            PopulateTree();
            UserList.DataSource = Membership.GetAllUsers();
            UserList.DataBind();
        }
    }

    #endregion

    #region page_prerender - display access rules

    // on page prerender, call DisplayAccessRules
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if(FolderTree.SelectedNode != null)
        {
            //DisplayAccessRules(FolderTree.SelectedValue);
            if (FolderTree.SelectedValue.ToLower().IndexOf(".qvw") != -1)
            {
                DisplayAccessRules(FolderTree.SelectedValue);
                SecurityInfoSection.Visible = true;
            }
            else
            {
                SecurityInfoSection.Visible = false;
            }
        }
    }

    #endregion

    #region populate TreeView

    // Populate the tree based on the subfolders of the specified VirtualSiteRoot
    public void PopulateTree()
    {
        //DirectoryInfo rootFolder = new DirectoryInfo(Server.MapPath(VirtualSiteRoot));
        DirectoryInfo rootFolder = new DirectoryInfo(VirtualSiteRoot);
        TreeNode root = AddNodeAndDescendents(rootFolder, null);
        FolderTree.Nodes.Add(root);
        try
        {
            FolderTree.SelectedNode.ImageUrl = "~/admin/themes/default/images/treeview/folder-open.gif";
        }
        catch
        {
            // do nothing
        }
    }

    #endregion

    #region add treenode and descendants

    protected TreeNode AddNodeAndDescendents(DirectoryInfo folder, TreeNode parentNode)
    {

        if ( ( ShowAllNodes == false ) && ( folder.Name.ToLower().Contains(_HIDDENFOLDER_) == true ))
            return parentNode;


        // displaying the folder's name and storing the full path to the folder as the value
        string virtualFolderPath;
        if (parentNode == null)
        {
            virtualFolderPath = VirtualSiteRoot;
        }
        else
        {
            virtualFolderPath = parentNode.Value + folder.Name + "/";
        }

        TreeNode node = new TreeNode(folder.Name, virtualFolderPath);
        node.Selected = (folder.Name == selectedFolderName);

        // Recurse through this folder's subfolders
        DirectoryInfo[] subFolders = folder.GetDirectories();
        foreach (DirectoryInfo subFolder in subFolders)
        {
            if ( (subFolder.Name != "App_Data") && (string.Compare(subFolder.Name,"history",true) != 0) )
            {
                TreeNode child = AddNodeAndDescendents(subFolder, node);
                //foreach (FileInfo FI in subFolder.GetFiles("*.qvw"))
                //{
                //    child.ChildNodes.Add(new TreeNode(FI.Name, virtualFolderPath + FI.Name, "~/admin/themes/default/images/treeview/qv.gif"));
                //}
                node.ChildNodes.Add(child);
            }
        }
        //add files at this level that are qvw
        foreach (FileInfo FI in folder.GetFiles("*.qvw"))
        {
            string QVRelativePath = virtualFolderPath + FI.Name;
            QVRelativePath = QVRelativePath.Substring(VirtualSiteRoot.Length);
            node.ChildNodes.Add(new TreeNode(FI.Name, QVRelativePath, "~/admin/themes/default/images/treeview/qv.gif"));
        }
        // Return the new TreeNode
        node.Value = node.Value.Substring(VirtualSiteRoot.Length); //remove the root to make local to QV
        return node;
    }

    #endregion

    #region reset the add rule form fields

    // reset the Add Rule form field values whenever the user moves folders
    protected void FolderTree_SelectedNodeChanged(object sender, EventArgs e)
    {
        ActionDeny.Checked = false;
        ActionAllow.Checked = true;
        ApplyRole.Checked = true;
        ApplyUser.Checked = false;
        ApplyAllUsers.Checked = false;
        ApplyAnonUser.Checked = false;
        UserRoles.SelectedIndex = 0;
        RequiresPatientContext.Checked = false;
        SpecifyUser.Text = "";

        RuleCreationError.Visible = false;

        // Restore previously selected folder's ImageUrl
        ResetFolderImageUrls(FolderTree.Nodes[0]);

        // Set the newly selected folder's ImageUrl
        if ( FolderTree.SelectedNode.Text.ToLower().Contains(".qvw") == false )
            FolderTree.SelectedNode.ImageUrl = "~/admin/themes/default/images/treeview/folder-open.gif";
    }

    #endregion

    #region reset folder image urls

    // Recurse through this node's child nodes
    protected void ResetFolderImageUrls(TreeNode parentNode)
    {
        if ( parentNode.Text.ToLower().Contains(".qvw") == false )
            parentNode.ImageUrl = "~/admin/themes/default/images/treeview/folder.gif";

        TreeNodeCollection nodes = parentNode.ChildNodes;
        foreach (TreeNode childNode in nodes)
        {
            ResetFolderImageUrls(childNode);
        }
    }

    #endregion

    #region throw exceptionif access outside this directory is detected

    //public static Configuration OpenConfigFile(string configPath)
    //{
    //    FileInfo configFile = new FileInfo(configPath);
    //    var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
    //    var wcfm = new WebConfigurationFileMap();
    //    wcfm.VirtualDirectories.Add("/", vdm);
    //    return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/" );
    //    //return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/", "iis_website_name");

    //}

    // throw exception if access outside the directory is detected this isn't needed since IIS won't allow it anyway
    protected void DisplayAccessRules(string virtualFolderPath)
    {
        //if (!virtualFolderPath.StartsWith(VirtualSiteRoot) || virtualFolderPath.IndexOf("..") >= 0)
        //{
        //    //throw new ApplicationException("An attempt to access a folder outside of the website directory has been detected and blocked.");
        //}
        //Configuration config = OpenConfigFile(virtualFolderPath); // WebConfigurationManager.OpenWebConfiguration(virtualFolderPath);
        //SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)config.GetSectionGroup("system.web");
        //AuthorizationRuleCollection authorizationRules = systemWeb.Authorization.Rules;

        //make sure all slashes are \  not /
        AuthorizationRuleCollection authorizationRules = UserRepo.GetInstance().GetAuthorizationConfiguration(virtualFolderPath);

        GridView1.DataSource = authorizationRules;
        GridView1.DataBind();

        TitleOne.InnerText = "Rules applied to '" + virtualFolderPath.Replace('/', '\\') +"'";
        TitleTwo.InnerText = "Create new rule for '" + virtualFolderPath.Replace('/', '\\')  + "'";
    }

    #endregion

    #region display message if no authorization rule present

    // if no authorization rule present, display message
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //AuthorizationRule rule = (AuthorizationRule)e.Row.DataItem;
            //if (!rule.ElementInformation.IsPresent)
            //{
            //    e.Row.Cells[3].Text = "Inherited from higher level";
            //    e.Row.Cells[4].Text = "Inherited from higher level";
            //}
        }

        
    }

    #endregion

    #region authorization rules

    // authorization rules
    protected string GetAction(AuthorizationRule rule)
    {
        return rule.Action.ToString();
    }
    protected string GetRole(AuthorizationRule rule)
    {
        return rule.Roles.ToString();
    }
    protected string GetUser(AuthorizationRule rule)
    {
        return rule.Users.ToString();
    }

    #endregion



    // delete rule
    protected void DeleteRule(object sender, EventArgs e)
    {
        Button button = (Button)sender;
       
        GridViewRow item = (GridViewRow)button.Parent.Parent;
        //this is grossly stupid,  I can't seem to get ahold of the values in the grid since it is a template,
        //so for now, get the auth rules again
        AuthorizationRuleCollection ARC = UserRepo.GetInstance().GetAuthorizationConfiguration(FolderTree.SelectedValue);
        string Role = ARC[item.RowIndex].Roles.Count == 1 ? ARC[item.RowIndex].Roles[0] : "";
        string User = ARC[item.RowIndex].Users.Count == 1 ? ARC[item.RowIndex].Users[0] : "";
        bool IsRole = string.IsNullOrEmpty(Role) ? false : true;

        UserRepo.GetInstance().DeleteItem(FolderTree.SelectedValue.Replace('/', '\\'), User, Role, IsRole);

        DisplayAccessRules(FolderTree.SelectedValue);
        UserRepo.GetInstance().Save();

        //Configuration config = WebConfigurationManager.OpenWebConfiguration(virtualFolderPath);
        //SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)config.GetSectionGroup("system.web");
        //AuthorizationSection section = (AuthorizationSection)systemWeb.Sections["authorization"];
        //section.Rules.RemoveAt(item.RowIndex);
        //config.Save();
    }

    #region MOVE_UP_DOWN
    #region move rule
    // move up rule
    protected void MoveUp(object sender, EventArgs e)
    {
        MoveRule(sender, e, "up");
    }

    // move down rule
    protected void MoveDown(object sender, EventArgs e)
    {
        MoveRule(sender, e, "down");
    }

    // move up or down rule
    protected void MoveRule(object sender, EventArgs e, string upOrDown)
    {
        upOrDown = upOrDown.ToLower();

        if (upOrDown == "up" || upOrDown == "down")
        {
            Button button = (Button)sender;
            GridViewRow item = (GridViewRow)button.Parent.Parent;
            int selectedIndex = item.RowIndex;
            if ((selectedIndex > 0 && upOrDown == "up") || (upOrDown == "down"))
            {
                string virtualFolderPath = FolderTree.SelectedValue;
                Configuration config = WebConfigurationManager.OpenWebConfiguration(virtualFolderPath);
                SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)config.GetSectionGroup("system.web");
                AuthorizationSection section = (AuthorizationSection)systemWeb.Sections["authorization"];

                // Pull the local rules out of the authorization section, deleting them from same:
                ArrayList rulesArray = PullLocalRulesOutOfAuthorizationSection(section);
                if (upOrDown == "up")
                {
                    LoadRulesInNewOrder(section, rulesArray, selectedIndex, upOrDown);
                }
                else if (upOrDown == "down")
                {
                    if (selectedIndex < rulesArray.Count - 1)
                    {
                        LoadRulesInNewOrder(section, rulesArray, selectedIndex, upOrDown);
                    }
                    else
                    {
                        // DOWN button in last row was pressed. Load the rules array back in without resorting.
                        for (int x = 0; x < rulesArray.Count; x++)
                        {
                            section.Rules.Add((AuthorizationRule)rulesArray[x]);
                        }
                    }
                }
                // save configuration to file
                config.Save();
            }
        }
    }

    #endregion

    #region load rules in new order

    // load rules in new order
    protected void LoadRulesInNewOrder(AuthorizationSection section, ArrayList rulesArray, int selectedIndex, string upOrDown)
    {
        AddFirstGroupOfRules(section, rulesArray, selectedIndex, upOrDown);
        AddTheTwoSwappedRules(section, rulesArray, selectedIndex, upOrDown);
        AddFinalGroupOfRules(section, rulesArray, selectedIndex, upOrDown);
    }

    #endregion

    #region add first group of rules

    // add first group of rules
    protected void AddFirstGroupOfRules(AuthorizationSection section, ArrayList rulesArray, int selectedIndex, string upOrDown)
    {
        int adj;
        if (upOrDown == "up") adj = 1;
        else adj = 0;
        for (int x = 0; x < selectedIndex - adj; x++)
        {
            section.Rules.Add((AuthorizationRule)rulesArray[x]);
        }
    }

    #endregion

    #region add the two swapped rules

    // add two swapped rules
    protected void AddTheTwoSwappedRules(AuthorizationSection section, ArrayList rulesArray, int selectedIndex, string upOrDown)
    {
        if (upOrDown == "up")
        {
            section.Rules.Add((AuthorizationRule)rulesArray[selectedIndex]);
            section.Rules.Add((AuthorizationRule)rulesArray[selectedIndex - 1]);
        }
        else if (upOrDown == "down")
        {
            section.Rules.Add((AuthorizationRule)rulesArray[selectedIndex + 1]);
            section.Rules.Add((AuthorizationRule)rulesArray[selectedIndex]);
        }
    }

    #endregion

    #region add final group of rules

    // add final group of rules
    protected void AddFinalGroupOfRules(AuthorizationSection section, ArrayList rulesArray, int selectedIndex, string upOrDown)
    {
        int adj;
        if (upOrDown == "up") adj = 1;
        else adj = 2;
        for (int x = selectedIndex + adj; x < rulesArray.Count; x++)
        {
            section.Rules.Add((AuthorizationRule)rulesArray[x]);
        }
    }

    #endregion

    #region grab local rules from authorization section

    // use local rules out of authorization section
    protected ArrayList PullLocalRulesOutOfAuthorizationSection(AuthorizationSection section)
    {
        // First load the local rules into an ArrayList.
        ArrayList rulesArray = new ArrayList();
        foreach (AuthorizationRule rule in section.Rules)
        {
            if (rule.ElementInformation.IsPresent)
            {
                rulesArray.Add(rule);
            }
        }

        // Next delete the rules from the section.
        foreach (AuthorizationRule rule in rulesArray)
        {
            section.Rules.Remove(rule);
        }
        return rulesArray;
    }

    #endregion

    #region create rule 
    #endregion

    // create rule
    protected void CreateRule(object sender, EventArgs e)
    {
        AuthorizationRule newRule;
        if (ActionAllow.Checked)
        {
            newRule = new AuthorizationRule(AuthorizationRuleAction.Allow);
        }
        else
        {
            newRule = new AuthorizationRule(AuthorizationRuleAction.Deny);
        }

        if (ApplyRole.Checked && UserRoles.SelectedIndex > 0)
        {
            newRule.Roles.Add(UserRoles.Text);
            AddRule(newRule);
        }
        else if (ApplyUser.Checked && SpecifyUser.Text != String.Empty)
        {
            newRule.Users.Add(SpecifyUser.Text);
            AddRule(newRule);
        }
        else if (ApplyAllUsers.Checked)
        {
            newRule.Users.Add("*");
            AddRule(newRule);
        }
        else if (ApplyAnonUser.Checked)
        {
            newRule.Users.Add("?");
            AddRule(newRule);
        }
    }

    #endregion

    #region add rule

    // add rule
    protected void AddRule(AuthorizationRule newRule)
    {

        string virtualFolderPath = FolderTree.SelectedValue;  //remove site root to make virtual based on QV path
        try
        {
            UserRepo.GetInstance().AddAuthorizationConfiguration(virtualFolderPath.Replace('/', '\\'), newRule, RequiresPatientContext.Checked, SpecialCommand.Text);
            UserRepo.GetInstance().Save();
            RuleCreationError.Visible = false;
            RequiresPatientContext.Checked = false;
            SpecialCommand.Text = "";
        }
        catch (Exception ex)
        {
            RuleCreationError.Visible = true;
            RuleCreationError.Text = "<div class=\"alert\"><br />An error occurred and the rule was not added.<br /><br />Error message:<br /><br /><i>" + ex.Message + "</i></div>";
        }
        ////Configuration config = WebConfigurationManager.OpenWebConfiguration(virtualFolderPath);
        //Configuration config = OpenConfigFile(RulesPath);
        //SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)config.GetSectionGroup("system.web");
        //AuthorizationSection section = (AuthorizationSection)systemWeb.Sections["authorization"];
        //section.Rules.Add(newRule);
        //try
        //{
        //    config.Save();
            
        //    RuleCreationError.Visible = false;
        //}
        //catch (Exception ex)
        //{
        //    RuleCreationError.Visible = true;
        //    RuleCreationError.Text = "<div class=\"alert\"><br />An error occurred and the rule was not added.<br /><br />Error message:<br /><br /><i>" + ex.Message + "</i></div>";
        //}
    }

    #endregion
    protected void UserList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SpecifyUser.Text = UserList.SelectedItem.Text;
    }
}
