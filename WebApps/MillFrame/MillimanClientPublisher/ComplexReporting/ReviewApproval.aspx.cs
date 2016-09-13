using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher.ComplexReporting
{
    public partial class ReviewApproval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            if (!IsPostBack)
            {
                if (Session["ProjectPath"] != null)
                {
                    //string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());

                    List<List<string>> ComparedValues = Comparisons(System.IO.Path.GetDirectoryName(Session["ProjectPath"].ToString()));
                    MillimanCommon.UI.DynamicTable DT = new MillimanCommon.UI.DynamicTable();
                    List<string> ColumnNames = new List<string>() { "Account", "Administrator Selected Values", "Viewable Values", "Valid", "QVW Available" };
                    DT.CreateTable2DBinder("User Access Verification", ColumnNames, ComparedValues, PlaceHolder1, System.Drawing.Color.White);
                    //MillimanCommon.QVWReportBankProcessor RBP = new MillimanCommon.QVWReportBankProcessor(Session["ProjectPath"].ToString());
                    //Dictionary<string, List<MillimanCommon.QVWReportBank.NotSelectableClass>> UnselectableItems = RBP.NotSelectableByUser();
                    //if (UnselectableItems.Count > 0)
                    //{
                    //    foreach (KeyValuePair<string, List<MillimanCommon.QVWReportBank.NotSelectableClass>> Unselectable in UnselectableItems)
                    //    {
                    //        List<string> Values = new List<string>();
                    //        foreach (MillimanCommon.QVWReportBank.NotSelectableClass NI in Unselectable.Value)
                    //            Values.Add("'" + NI.ConceptFieldName + "' <b>missing value</b> '" + NI.FieldName + "'");
                    //        MillimanCommon.UI.DynamicTable.CreateTable("User " + Unselectable.Key + " un-selectable values", Values, PlaceHolder1);
                    //    }
                    //}
                    //else
                    //{  //no new items
                    //    Label UserMessage = new Label();
                    //    UserMessage.Style.Add("width", "100%");
                    //    UserMessage.Text = "<br><br><center>Approval</center>";
                    //    PlaceHolder1.Controls.Add(UserMessage);
                    //}
                }
            }
        }

        private List<List<string>> Comparisons( string WorkingDirectory)
        {
            string RedMsg = "<font color=\"red\">_MSG_</font>";
            string OrangeMsg = "<font color=\"orange\">_MSG_</font>";
            List<List<string>> RetValue = new List<List<string>>();

            string[] SelectionFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(WorkingDirectory, "reduceduserqvws"), "*.selections", System.IO.SearchOption.AllDirectories);
            foreach( string SelectionFile in SelectionFiles )
            {
                List<string> AdminSelected = GetSelectionsForUser(SelectionFile);
                string ReferenceFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), "[REFERENCE].redirect");
                bool QVWExists = false;
                List<string> Reduced = GetUniqueSelections(ReferenceFile, out QVWExists);

                //lets get the account name encoded in the directory name
                string DirectoryWithoutRoot = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(SelectionFile));
                string Account = MillimanCommon.Utilities.ConvertHexToString(DirectoryWithoutRoot);

                List<string> MatchingValues = new List<string>();
                for( int Index = AdminSelected.Count-1; Index >= 0; Index--)
                {
                    string Item = AdminSelected[Index];
                    int ReducedIndex = Reduced.IndexOf(Item);
                    if (ReducedIndex != -1)
                    {
                        MatchingValues.Add(Item);
                        AdminSelected.RemoveAt(Index);
                        Reduced.RemoveAt(ReducedIndex);
                    }
                }

                List<List<string>> ThisUserInfo = new List<List<string>>();
                foreach (string PHILeak in Reduced)
                {
                    if ( string.IsNullOrEmpty(PHILeak) == false ) //dont add empty entries
                        ThisUserInfo.Add(new List<string>() { "", "", RedMsg.Replace("_MSG_", PHILeak), RedMsg.Replace("_MSG_", "PHI BREACH"), "" });
                }
                foreach (string Missing in AdminSelected)
                {
                    if ( string.IsNullOrEmpty(Missing) == false )
                        ThisUserInfo.Add(new List<string>() { "", Missing, "", OrangeMsg.Replace("_MSG_", "MISSING"), "" });
                }
                foreach (string Matching in MatchingValues)
                {
                    if ( string.IsNullOrEmpty(Matching) == false )
                        ThisUserInfo.Add(new List<string>() { "", Matching, Matching, "Valid", "" });
                }
                if ( ThisUserInfo.Count == 0 )
                    ThisUserInfo.Add(new List<string>() { "", "","", "No Values", "" });

                ThisUserInfo[0][0] = Account;
                ThisUserInfo[0][4] = QVWExists ? "Yes" : RedMsg.Replace("_MSG_", "No");

                RetValue.AddRange(ThisUserInfo);
            }
            return RetValue;
        }

        private List<string> GetUniqueSelections( string ForReferenceFile, out bool QVWExists )
        {
            List<string> AllUniqueValues = new List<string>();

            QVWExists = false;
            string RedirectToQVW = System.IO.Path.GetFileName(System.IO.File.ReadAllText(ForReferenceFile));

            //correct QVW cache is back 2 dirs - even though it takes 3 getdirnames...
            string RelativeRoot = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(ForReferenceFile)));
            //then down one directory into allways present reducedcachedqvws
            string QVWCache = System.IO.Path.Combine(RelativeRoot, "ReducedCachedQVWs");
            //Look for QVW here
            string QVWPathDir = System.IO.Path.Combine(QVWCache, RedirectToQVW);
            QVWExists = System.IO.File.Exists(QVWPathDir);

            string[] UniqueValueFiles = System.IO.Directory.GetFiles(QVWCache, System.IO.Path.GetFileNameWithoutExtension( RedirectToQVW ) + ".uniquevalues_*", System.IO.SearchOption.TopDirectoryOnly);
            foreach( string UniqueValueFile in UniqueValueFiles )
            {
                string[] UniqueValues = System.IO.File.ReadAllLines(UniqueValueFile);
                if ( UniqueValues.Length > 0 )
                {
                    List<string> Values = UniqueValues.ToList();
                    Values.RemoveAt(0);  ///header value, get rid of it
                    AllUniqueValues.AddRange(Values);
                }
            }
            return AllUniqueValues ;
        }

        private List<string> GetSelectionsForUser( string UserSelectionFile )
        {
            List<string> AdminSelections = new List<string>();
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            List<string> Selections = SS.Deserialize(UserSelectionFile) as List<string>;
            if (Selections != null)
            {
                foreach (string Selection in Selections)
                {
                    AdminSelections.Add(Selection.Substring(Selection.IndexOf('|') + 1)); //if we have a multiple level it will show all as different for now....
                }
            }
            return AdminSelections;
        }
    }
}