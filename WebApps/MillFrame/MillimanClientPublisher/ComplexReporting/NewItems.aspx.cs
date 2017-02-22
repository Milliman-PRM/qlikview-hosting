using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ClientPublisher.ComplexReporting
{
    public partial class NewItems : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;
            if (!IsPostBack)
            {
                if (Session["ProjectPath"] != null)
                {

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());
                    string NewDataHierarchy = System.IO.Path.Combine(LocalPS.AbsoluteProjectPath, LocalPS.ProjectName + ".hierarchy_0");
                    string OldDataHierarchy = System.IO.Path.Combine(LocalPS.AbsoluteProjectPath, "Legacy", LocalPS.ProjectName + ".hierarchy_0");

                    Telerik.Web.UI.RadTreeView RTV = DiffTrees(LocalPS, NewDataHierarchy, OldDataHierarchy);

                    if (RTV != null)
                    {
                        PlaceHolder1.Controls.Add(RTV);
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>The newer report does not contain any new fields.</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }
                
                }
            }
        }

        /// <summary>
        /// Create a difference of the trees for display, we want to highlight the NEW data that is coming into the system
        /// </summary>
        /// <param name="Settings"></param>
        /// <param name="NewDataHierarchy"></param>
        /// <param name="OldDataHierarchy"></param>
        /// <returns></returns>
        private Telerik.Web.UI.RadTreeView DiffTrees( MillimanCommon.ProjectSettings Settings,  string NewDataHierarchy,  string OldDataHierarchy )
        {
            //Note: this is overly simplistic, we should perform a diff of the 2 in memory trees and show the difference of the
            //NewData - OldData - that will make data a bit more manageable since client admins see this as a tree view

            Telerik.Web.UI.RadTreeView RTV = new RadTreeView();
            MillimanCommon.MillimanTreeNode NewData = MillimanCommon.MillimanTreeNode.GetMemoryTree(NewDataHierarchy);
            MillimanCommon.MillimanTreeNode OldData = MillimanCommon.MillimanTreeNode.GetMemoryTree(OldDataHierarchy);

            MillimanCommon.TreeBuilder TB = new MillimanCommon.TreeBuilder();
            List<string> UniqueNewLeafs = TB.GetUniqueLeafNodes(NewData);
            List<string> UniqueOldLeafs = TB.GetUniqueLeafNodes(OldData);

            foreach( string LeafText in UniqueOldLeafs )
            {
                for(int Index = UniqueNewLeafs.Count()-1; Index >= 0; Index--)
                {
                    if ( string.Compare( LeafText, UniqueNewLeafs[Index], true) == 0 )
                    {
                        UniqueNewLeafs.RemoveAt(Index);
                        break;
                    }
                }
            }
            if (UniqueNewLeafs.Count() == 0)
                return null;  //no differences

            string Template = "< Node Text =\"_TEXT_\" Value=\"_VALUE_\"></Node>";
            string ContainerXML = string.Empty;
            foreach( string LeafText in UniqueNewLeafs)
            {
                string NewItem = Template.Replace("_TEXT_", LeafText);
                NewItem = NewItem.Replace("_VALUE_", LeafText);
                ContainerXML += NewItem;
            }
            RTV.LoadXml("<Tree>" + ContainerXML + "</Tree>");
            return RTV;

        }
     }
}