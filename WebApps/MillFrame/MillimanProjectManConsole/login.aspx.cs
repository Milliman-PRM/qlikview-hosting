using System;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Provider Location City 0001|Provider Location Name 0004

            //MillimanCommon.MillimanTreeNode MTN = MillimanCommon.MillimanTreeNode.GetMemoryTree(@"C:\RuntimeFiles\InstalledApplications\MillimanSite\QVDocuments\Demo\DemoProject.hierarchy_0_new");
            //System.Collections.Generic.List<string> DML1 = new System.Collections.Generic.List<string>() { "Provider Location City 0001" };
            //System.Collections.Generic.List<string> DM1 = MTN.FindDataModelFieldNames(DML1);

            //System.Collections.Generic.List<string> DML2 = new System.Collections.Generic.List<string>() { "Provider Location City 0001","Provider Location Name 0004" };
            //System.Collections.Generic.List<string> DM2 = MTN.FindDataModelFieldNames(DML2);


            //int C = RTV.Nodes.Count;
           //VWN
           // MillimanProjectManConsole.MultiPublisher MP = new MillimanProjectManConsole.MultiPublisher(@"C:\inetpub\wwwroot\InstalledApplications\MillimanSite\QVDocuments\0032ClinicalPath01\South\Medicare\alpha\ClinicalPathsUN.hciprj");

            //MillimanReportReduction.QVWHierarchy Hier = new MillimanReportReduction.QVWHierarchy();
            //Hier.QualifiedQVWNameToIndex = "C:\\DATA\\Hierarchy_NonPHI.qvw";
            //Hier.TaskName = "Tinga";
            //Hier.TaskDescription = "Test";
            //Hier.DeleteTaskOnCompletion = true;
            //if (Hier.ExtractHierarchyBlocking() == true)
            //{
            //    System.Collections.Generic.List<string> L = Hier.ExtractedHierarchyFiles;
            //    string Path = System.IO.Path.GetDirectoryName(L[0]);
            //    MillimanCommon.TreeBuilder TB = new MillimanCommon.TreeBuilder();
            //    System.Collections.Generic.List<string> IndexFiles = null;
            //    if (TB.BuildTree(Path, out IndexFiles) == true)
            //    {
            //        int Index = 0;
            //    }
            //}
            //else
            //{
            //    int i = 0;
            //}
        }
    }
}
