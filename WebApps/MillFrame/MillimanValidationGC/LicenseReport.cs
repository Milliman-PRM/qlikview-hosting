using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace PRMValidationGC
{
    public class LicenseReport
    {
        public class KeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public KeyValue(string _Key, string _Value)
            {
                Key = _Key;
                Value = _Value;
            }

            public static KeyValue Create(string _Key, string _Value)
            {
                return new KeyValue(_Key, _Value);
            }

        }
        private List<string> EmailReportsTo { get; set; }
        private string LicenseReportDir { get; set; }

        //for security purpose, don't show everything like admin status and admin accounts...
        private bool SecureMode { get; set; }
        private int NumInstalledDocCals { get; set; }
        private int NumRequiredDocCals { get; set; }
        private int NumInstalledUserCals { get; set; }
        private int NumRequiredUserCals { get; set; }
        private int NumberActiveUsers { get; set; }
        private int NumberSuspendedUsers { get; set; }
        public LicenseReport()
        {
            string Emails = System.Configuration.ConfigurationManager.AppSettings["LicenseReportEmailDestinations"];
            EmailReportsTo = Emails.Split(new char[] { '~' }).ToList();
            LicenseReportDir = System.Configuration.ConfigurationManager.AppSettings["LicenseReportDirectory"];

            NumInstalledDocCals = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["NumberDocCals"]);
            NumRequiredDocCals = 0;
            NumInstalledUserCals = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["NumberUserCals"]);
            NumRequiredUserCals = 0;
            SecureMode = false;
        }

        public void Process()
        {
            //Summary as [datetime]: Installed Document CALS:   Required Document Cals:   Availate Document Cals
            //                       Installed Named User CALS: Required Named User Cals: Available Named User Cals
            //                       Active User Count:
            //                       Suspended User Count:

            //Active User List ( Break even at 1 NUC($) to 6 DC($) )
            //Email Admin ClientAdmin Can Access # Docs DC or NUC

            //Suspended User List( users in system, but login is suspended )

            //generate in reverse order, they calculate info for each other, but in reverse order ONLY
            string Suspended = CreateSuspendedTable();
            string Details = CreateDetailsTable();
            string Summary = CreateSummaryTable();
            string Body = string.Empty;
            Body += "<br><center><b>PRM/Qlikview Maximum License Usage Report</b><center><br><i>This report shows the maximum license usage based on the scenario where EVERY user accesses ALL available reports within a 24 hour period.</i> <br><br>";
            Body += "<br>" + Summary;
            Body += "<br>" + Details;
            Body += "<br>" + Suspended;
            

            string TemplateDir = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "EmailTemplates");
            string LicenseEmail = System.IO.File.ReadAllText(System.IO.Path.Combine(TemplateDir, "LicenseReport.html"));
            LicenseEmail = LicenseEmail.Replace("_BODY_", Body);

            foreach (string User in EmailReportsTo)
            {
                MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();

                ME.Send(User, "PRM.Support@milliman.com", LicenseEmail,  "PRM/Qlikview License Report", true, false);
            }
        }


        private string CreateSummaryTable()
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("<table class='gridtable'>");
            SB.AppendLine("<th colspan='6'>PRM License Summary (" + System.DateTime.Now.ToShortDateString() + ")</th>");

            KeyValue InstalledDocsCals = new KeyValue("Installed Document Cals", NumInstalledDocCals.ToString());
            KeyValue RequiredDocCals = new KeyValue("Required Document Cals", NumRequiredDocCals.ToString());
            KeyValue AvailableDocCals = new KeyValue("Available Document Cals", (NumInstalledDocCals - NumRequiredDocCals).ToString());
            SB.AppendLine( GenerateRow(new List<KeyValue>() { InstalledDocsCals, RequiredDocCals, AvailableDocCals} ));

            KeyValue InstalledUserCals = new KeyValue("Installed Named User Cals", NumInstalledUserCals.ToString());
            KeyValue RequiredUserCals = new KeyValue("Required Named User Cals", NumRequiredUserCals.ToString());
            KeyValue AvailableUserCals = new KeyValue("Available Named User Cals", (NumInstalledUserCals - NumRequiredUserCals).ToString());
            SB.AppendLine( GenerateRow(new List<KeyValue>() { InstalledUserCals, RequiredUserCals, AvailableUserCals} ));

            SB.AppendLine(GenerateSimpleRow("Active Users", NumberActiveUsers.ToString()));
            SB.AppendLine(GenerateSimpleRow("Suspended Users", NumberSuspendedUsers.ToString()));
            SB.AppendLine(GenerateSimpleRow("Total Users", (NumberActiveUsers + NumberSuspendedUsers).ToString()));

            SB.AppendLine("</table>");

            return SB.ToString();
        }

        private string CreateDetailsTable()
        {
            int LicenseBreakEvenLimit = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDocsPerDocumentCAL"]);
            string DocCalCost = System.Configuration.ConfigurationManager.AppSettings["DocCALCost"];
            string NamedCALCost = System.Configuration.ConfigurationManager.AppSettings["NamedCALCost"];

            //Active User List ( Break even at 1 NUC($) to 6 DC($) )
            //Email Admin ClientAdmin Can Access # Docs DC or NUC
            StringBuilder SB = new StringBuilder();
            int Limit = (int)(System.Convert.ToDouble(NamedCALCost) / System.Convert.ToDouble(DocCalCost));
            SB.AppendLine("***Any user with access to " + Limit.ToString() + " or more documents will be allocated a Named User License.<br>");
            SB.AppendLine("<table class='gridtable'>");
            SB.AppendLine("<th colspan='8'>PRM License Usage Details</th>");
            int Index = 1;
            KeyValue Col0 = new KeyValue("", "");
            KeyValue Col1_2 = new KeyValue("Account", "System Admin");
            KeyValue Col3_4 = new KeyValue("Client Admin", "Accessable Documents");
            KeyValue Col5_6 = new KeyValue("Document License", "Named User License");
            List<KeyValue> Keys = new List<KeyValue>() {Col0, Col1_2, Col3_4, Col5_6 };
            SB.Append( GenerateRow( Keys ));

            MillimanCommon.UserRepo Repo = MillimanCommon.UserRepo.GetInstance();
            string X = "<center>X</center>";
            MembershipUserCollection MUC = Membership.GetAllUsers(); 
            foreach( MembershipUser MU in MUC)
            {
                if (MU.IsApproved)
                {
                    NumberActiveUsers++;
                    string[] UserRoles = Roles.GetRolesForUser(MU.UserName);
                    //filter users based on roles
                    bool IsNYLive = false;
                    foreach (string R in UserRoles)
                    {
                        if ((R.ToLower().IndexOf("newyork_live") >= 0) || (R.ToLower().IndexOf("newyork_bpci") >= 0))
                            IsNYLive = true;
                    }
                    if (IsNYLive == false)
                        continue;
                    //end filter
                    bool isAdmin = false;
                    bool isClientAdmin = IsClientUserAdmin(MU.ProviderUserKey.ToString());
                    foreach (string Role in UserRoles)
                    {
                        if (string.Compare(Role, "administrator", true) == 0)
                            isAdmin = true;
                    }
                    var Reports = Repo.FindAllQVProjectsForUser(MU.UserName, UserRoles, false);
                    Col0.Key = Index.ToString();
                    Index++;
                    Col1_2.Key = MU.UserName;
                    if (SecureMode)
                    {
                        Col1_2.Value = "";
                        Col3_4.Key = "";
                        if ((string.Compare(Col1_2.Key, "admin", true) == 0) || (string.Compare(Col1_2.Key, "admininstrator", true) == 0))
                            Col1_2.Key = "[PRM_SYSTEM]";
                    }
                    else
                    {
                        Col1_2.Value = isAdmin ? X : "";
                        Col3_4.Key = isClientAdmin ? X : "";
                    }

                    if (isAdmin)
                    {
                        Col3_4.Value = "<center>*</center>";
                        Col5_6.Key = "";
                        Col5_6.Value = X;
                    }
                    else
                    {
                        Col3_4.Value = Reports.Count.ToString();
                        Col5_6.Key = Reports.Count < LicenseBreakEvenLimit ? X : "";
                        Col5_6.Value = Reports.Count >= LicenseBreakEvenLimit ? X : "";
                    }
                    
                    if ( (Reports.Count < LicenseBreakEvenLimit) && (isAdmin == false))
                        NumRequiredDocCals += Reports.Count;  //either using 0 to X doc cals
                    else
                        NumRequiredUserCals++;    //or 1 user cal



                    SB.Append(GenerateRow(Keys));

                }

            }
            SB.AppendLine("</table>");

            return SB.ToString();
        }

        private bool IsClientUserAdmin(string UserId)
        {
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbMyCMSConnectionString"].ConnectionString;
                System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand();
                comm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
                String sql = @"SELECT IsClientAdministrator from aspnet_customprofile where UserId='" + UserId.ToUpper() + "'";
                comm.CommandText = sql;
                comm.Connection.Open();
                System.Data.SqlClient.SqlDataReader cursor = comm.ExecuteReader();
                while (cursor.Read())
                {
                    string Value = cursor["IsClientAdministrator"].ToString();
                    if (string.IsNullOrEmpty(Value) == false)
                        return System.Convert.ToBoolean(Value);
                }
                comm.Connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }
        private string CreateSuspendedTable()
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("<table class='gridtable'>");
            SB.AppendLine("<th colspan='2'>PRM Suspended Users</th>");
            MembershipUserCollection MUC = Membership.GetAllUsers();
            int Index = 1;
            foreach (MembershipUser MU in MUC)
            {
                if (MU.IsApproved == false)
                {
                    SB.AppendLine(GenerateSimpleRow(Index.ToString(), MU.UserName));
                    Index++;
                    NumberSuspendedUsers++;
                }
            }

            SB.AppendLine("</table>");

            return SB.ToString();
        }


        private string GenerateRow(List<KeyValue> KeyValues)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("<tr>");
            string Key;
            string Value;
            foreach (KeyValue KVP in KeyValues)
            {
                Key = KVP.Key;
                Value = KVP.Value;
                if ( string.IsNullOrEmpty(Key))
                    Key = "&nbsp;";
                if (string.IsNullOrEmpty(Value))
                    Value = "&nbsp;";
                SB.Append("<td>" + Key + "</td><td>" + Value + "</td>");
            }
            SB.Append("</tr>");
            return SB.ToString();
        }

        private string GenerateSimpleRow(string _Key, string _Value)
        {
            KeyValue KV = new KeyValue(_Key, _Value);
            return GenerateRow(new List<KeyValue>() { KV });
        }
    }
}
