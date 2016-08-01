using MillimanCommon;
using Polenter.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

public partial class admin_controls_UserRolesSelector : System.Web.UI.UserControl
{
    #region "Properties"

    private string _userName;

    public string UserName
    {
        get { return _userName; }
        set { _userName = value; }
    }

    public List<string> CheckedRoles
    {
        get
        {
            return GetCheckedUserRolesList();
        }
        set
        {
            if (value != null)
            {
                CheckedRoles.Add(value.ToString());
            }
        }
    }

    public List<string> UnChekcedRoles
    {
        get
        {
            return GetUnCheckedUserRoles();
        }
        set
        {
            if (value != null)
            {
                UnChekcedRoles.Add(value.ToString());
            }
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            // check if username exists in the query string
            if (!string.IsNullOrEmpty(Request.QueryString["username"]))
            {
                UserName = Request.QueryString["username"];
            }
            PopulateUserRolesTreeview();
        }
    }

    #region Events

    /// <summary>
    /// Expands/Collapses a node when clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectedNodeChanged(object sender, EventArgs e)
    {
        if (((TreeView)sender).SelectedNode.ChildNodes.Count > 0)
        {
            if ((bool)((TreeView)sender).SelectedNode.Expanded)
            {
                ((TreeView)sender).SelectedNode.Collapse();
            }
            else
            {
                ((TreeView)sender).SelectedNode.Expand();
            }
        }

        // Deselects the SelectedNode so it can be toggled without clicking on another node first.
        ((TreeView)sender).SelectedNode.Selected = false;
    }

    /// <summary>
    /// Checks or unchecks child nodes when a parent node is checked or unchecked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnTreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
    {
        if (e != null && e.Node != null)
        {
            // Determine if checked Node is a root node.
            if (e.Node.Parent.ChildNodes.Count > 0)
            {
                // Check or uncheck all of the child nodes based on status of parent node.
                if (e.Node.Checked)
                    ChangeChecked(e.Node, true);
                else
                    ChangeChecked(e.Node, false);
            }
        }
    }

    #endregion

    #region Custom
    public void LoadUserRoles()
    {
        PopulateUserRolesTreeview();
    }
    public void UnAllcheckUserRoles()
    {
        if (tvUserRoles.Nodes.Count > 0)
        {
            foreach (TreeNode parentNode in tvUserRoles.Nodes)
            {
                foreach (TreeNode childNode in parentNode.ChildNodes)
                {
                    if (childNode.Checked)
                    {
                        childNode.Checked = false;
                    }
                }
                if (parentNode.Expanded == true)
                {
                    parentNode.CollapseAll();
                }
            }
        }
    }

    private void PopulateUserRolesTreeview()
    {
        try
        {
            var ds = new DataSet();

            var dtparent = new DataTable();
            dtparent = FillParentTable();
            dtparent.DefaultView.Sort = "[colGroupCategory] ASC";
            dtparent.TableName = "dtparent";

            var dtchild = new DataTable();
            dtchild = FillParentTable();
            dtchild.DefaultView.Sort = "[colRoleName] ASC";
            dtchild.TableName = "dtchild";

            ds.Tables.Add(dtparent);
            ds.Tables.Add(dtchild);

            //create parent child relationship
            ds.Relations.Add("children", dtparent.Columns["colGroupCategory"], dtchild.Columns["colGroupCategory"], false);

            if (ds.Tables[0].Rows.Count > 0)
            {
                tvUserRoles.Nodes.Clear();
                foreach (DataRow masterRow in ds.Tables[0].Rows)
                {
                    //var masterNode = new TreeNode(text, value);
                    var categoryValue = !string.IsNullOrEmpty(masterRow["colGroupCategory"].ToString()) ? (masterRow["colGroupCategory"].ToString().ToUpper()) : "NO CATEGORY";
                    var categoryText = !string.IsNullOrEmpty(masterRow["colGroupCategory"].ToString()) ? (masterRow["colGroupCategory"].ToString().ToUpper()) : "NO CATEGORY";
                    var masterNode = new TreeNode(categoryText, categoryValue);

                    if (tvUserRoles.Nodes.Count > 0)
                    {
                        var bExist = false;
                        for (int i = 0; i < tvUserRoles.Nodes.Count; i++)
                        {
                            if (tvUserRoles.Nodes[i].Text == masterNode.Text)
                            {
                                bExist = true;
                                break;
                            }
                        }
                        if (!bExist)
                        {
                            tvUserRoles.Nodes.Add(masterNode);
                        }
                    }
                    else
                    {
                        tvUserRoles.Nodes.Add(masterNode);
                    }

                    tvUserRoles.CollapseAll();
                    masterNode.ShowCheckBox = false;

                    foreach (DataRow childRow in masterRow.GetChildRows("children"))
                    {
                        //var masterNode = new TreeNode(text, value);
                        var childNode = new TreeNode(childRow["colRoleName"].ToString(),
                                                                childRow["colGroupCategory"].ToString());
                        masterNode.ChildNodes.Add(childNode);
                        childNode.Value = childRow["colRoleName"].ToString();
                        childNode.ShowCheckBox = true;

                        //Get all roles associated with user & check that role
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            var userRoles = Roles.GetRolesForUser(UserName);
                            foreach (string role in userRoles)
                            {
                                if (childNode.Text == role)
                                {
                                    childNode.Checked = true;
                                    CheckedRoles.Add(childNode.Text);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
              
       }
        catch (Exception ex)
        {
            throw new Exception("Unable to populate treeview" + ex.Message);
        }
    }

    private DataTable FillParentTable()
    {
        var dt = new DataTable();
        //this gets called first
        var GroupMapFile = ConfigurationManager.AppSettings["MillimanGroupMap"];
        var serializer = new SharpSerializer(false);
        MillimanGroupMap groupMapInstance = null;
        if (System.IO.File.Exists(GroupMapFile))
        {
            groupMapInstance = serializer.Deserialize(GroupMapFile) as MillimanGroupMap;
        }
        // Create a DataTable and define its columns
        dt.Columns.Add("colRoleName");
        dt.Columns.Add("colGroupCategory");

        var allRoles = Roles.GetAllRoles();

        // Get the list of roles in the system and how many users belong to each role
        foreach (string roleName in allRoles)
        {
            MillimanGroupMap.MillimanGroups groups = null;
            if (groupMapInstance.MillimanGroupDictionary.ContainsKey(roleName))
                groups = groupMapInstance.MillimanGroupDictionary[roleName];

            var groupCategoryFromXml = "";
            if (groups != null)
            {
                groupCategoryFromXml = !string.IsNullOrEmpty(groups.GroupCategory) ? groups.GroupCategory.ToUpper() : "NO CATEGORY";
            }
            var groupCategory = groupCategoryFromXml;

            string[] roleRow = {
                                    roleName,
                                    groupCategory
                                };
            dt.Rows.Add(roleRow);
        }

        var newTable = from row in dt.AsEnumerable()
                       group row by new
                       {
                           colGroupCategory = row.Field<string>("colGroupCategory"),
                           colRoleName = row.Field<string>("colRoleName")
                       }
                      into grp
                       select new
                       {
                           colGroupCategory = grp.Key.colGroupCategory,
                           colRoleName = grp.Key.colRoleName
                       };

        var finalTable = newTable.OrderBy(a => a.colGroupCategory).ThenBy(b=>b.colRoleName);
        var dtUniqRecords = new DataTable();
        dtUniqRecords = ConvertToDataTable(finalTable);
        return dtUniqRecords;
    }

    DataTable ConvertToDataTable<TSource>(IEnumerable<TSource> source)
    {
        var props = typeof(TSource).GetProperties();

        var dt = new DataTable();
        dt.Columns.AddRange(
          props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray()
        );

        source.ToList().ForEach(
          i => dt.Rows.Add(props.Select(p => p.GetValue(i, null)).ToArray())
        );

        return dt;
    }
       
    /// <summary>
    /// Recursively checks or unchecks all child nodes for a given TreeNode.
    /// </summary>
    /// <param name="node">TreeNode to check or uncheck.</param>
    /// <param name="check">Desired value of TreeNode.Checked.</param>
    private void ChangeChecked(TreeNode node, bool check)
    {
        // "Queue" up child nodes to be checked or unchecked.
        //if (node.Parent.ChildNodes.Count > 0)
        //{
        //    for (int i = 0; i < node.Parent.ChildNodes.Count; i++)
        //        ChangeChecked(node.Parent.ChildNodes[i], check);
        //}
        node.Checked = check;
    }

    private List<string> GetCheckedUserRolesList()
    {
        var roleList = new List<string>();
        if (tvUserRoles.Nodes.Count > 0)
        {
            foreach (TreeNode parentNode in tvUserRoles.Nodes)
            {
                foreach (TreeNode childNode in parentNode.ChildNodes)
                {
                    if (childNode.Checked)
                    {
                        roleList.Add(childNode.Text); //add the role
                    }
                }
            }
        }
        return roleList;
    }

    public string GetCheckedUserRolesForNode(string checkedNode)
    {
        var role = string.Empty;
        if (!string.IsNullOrEmpty(checkedNode))
        {
            if (tvUserRoles.Nodes.Count > 0)
            {
                foreach (TreeNode parentNode in tvUserRoles.Nodes)
                {
                    foreach (TreeNode childNode in parentNode.ChildNodes)
                    {
                        if (childNode.Text == checkedNode)
                        {
                            if (childNode.Checked)
                            {
                                role = childNode.Text; //add the role
                                break;
                            }
                        }
                    }
                }
            }
        }
        return role;
    }

    private List<string> GetUnCheckedUserRoles()
    {
        var roleList = new List<string>();
        if (tvUserRoles.Nodes.Count > 0)
        {
            foreach (TreeNode parentNode in tvUserRoles.Nodes)
            {
                foreach (TreeNode childNode in parentNode.ChildNodes)
                {
                    if (!childNode.Checked)
                    {
                        roleList.Add(childNode.Text); //add the role
                    }
                }
            }
        }

        return roleList;
    }
    public string GetUnchekcedUserRoleForNode(string checkedNode)
    {
        var role = string.Empty;
        if (!string.IsNullOrEmpty(checkedNode))
        {
            if (tvUserRoles.Nodes.Count > 0)
            {
                foreach (TreeNode parentNode in tvUserRoles.Nodes)
                {
                    foreach (TreeNode childNode in parentNode.ChildNodes)
                    {
                        if (childNode.Text == checkedNode)
                        {
                            if (!childNode.Checked)
                            {
                                role = childNode.Text;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return role;
    }
    private void UpdateUserRoles()
    {
        // add or remove user from role based on selection
        foreach (TreeNode node in tvUserRoles.CheckedNodes)
        {
            if (node.Checked)
            {
                if (!Roles.IsUserInRole(UserName, node.Text))
                {
                    Roles.AddUserToRole(UserName, node.Text);
                }
            }
            else
            {
                if (Roles.IsUserInRole(UserName, node.Text))
                {
                    Roles.RemoveUserFromRole(UserName, node.Text);
                }
            }
        }
    }
    #endregion
}
