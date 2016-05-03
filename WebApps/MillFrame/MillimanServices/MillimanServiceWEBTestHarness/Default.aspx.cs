using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanServiceWEBTestHarness
{
    public partial class _Default : Page
    {
        static List<SyncClass> Syncs = new List<SyncClass>();

        protected void Page_Load(object sender, EventArgs e)
        {


            MillimanServices.Administration Admin = new MillimanServices.Administration();
            Admin.SystemCredentialsValue = new MillimanServices.SystemCredentials();
            Admin.SystemCredentialsValue.UserName = "MillimanStagingGuest";
            Admin.SystemCredentialsValue.UserPassword = "MSG123454321";

            //System.Threading.Thread.Sleep(20000);

            Guid AlphaTestGuid = Guid.Parse("671F33CD-839F-4256-8752-F14AEA6EA2D4");
            Guid BetaTestGuid = Guid.Parse("Baaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            Guid GammaTestGuid = Guid.Parse("Caaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            //List<string> Providers = new List<string>() { "6E9A983A-A0F9-41EF-BB2E-1CE6CAA13E77" };
            List<string> Providers = new List<string>() { "" };
            List<Guid> ClientsIDs = new List<Guid>() { AlphaTestGuid, BetaTestGuid };

            //test the async calls
            AutoAsyncTest(60, AlphaTestGuid, Providers, 500);
            return;

            //test sync alls
            //bool RValue = Admin.ClientAccess("van.nanney@milliman.com", AlphaTestGuid, "AlphaX", true);
            //List<MillimanServices.ClientID_NameMap> AllClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.ALL).ToList();
            //List<MillimanServices.ClientID_NameMap> EnabledClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.ENABLED).ToList();
            //List<MillimanServices.ClientID_NameMap> DisabledClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.DISABLED).ToList();
            //RValue = Admin.AddUser("van.nanney@milliman.com", AlphaTestGuid, AlphaTestGuid.ToString() + "_3", "CARECOORD", Providers.ToArray(), true);
            //List<MillimanServices.MillimanUser> Users = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.AddUser("van.nanney@milliman.com", AlphaTestGuid, "qbert.machine@somewhere.com", "COSTMODEL", Providers.ToArray(), true);
            //List<MillimanServices.MillimanUser> Users3 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.RemoveUserReportAccess("van.nanney@milliman.com", AlphaTestGuid, "qbert.machine@somewhere.com", "COSTMODEL");
            //List<MillimanServices.MillimanUser> Users44 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.ModifyUserProviders("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com", "CARECOORD", Providers.ToArray());
            //RValue = Admin.DisableUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //RValue = Admin.DeleteUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //List<MillimanServices.MillimanUser> Users2 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //MillimanServices.MillimanUser MU = Admin.FindUser("van.nanney@milliman.com", AlphaTestGuid, "qbert");
            //string Error = Admin.GetLastError();
            //List<string> ReportIDs = Admin.GetValidReportIDs(AlphaTestGuid).ToList();

            //Admin.AddUserCompleted += Admin_AddUserCompleted;
            //Admin.AddUserAsync("van.nanney@milliman.com", AlphaTestGuid, AlphaTestGuid.ToString() + "_3", "CARECOORD", Providers.ToArray(), true);

        }

        void Admin_AddUserCompleted(object sender, MillimanServices.AddUserCompletedEventArgs e)
        {
            bool Res = e.Result;
            System.Diagnostics.Debug.WriteLine("     " + System.DateTime.Now.ToString() + ":" + Res.ToString());
        }

        void AutoAsyncTest(int NumUsers, Guid GroupID, List<string> Providers, int Sleep = 0)
        {
            Syncs.Clear();
            System.Diagnostics.Debug.WriteLine("          Started:" + DateTime.Now.ToString());
            for (int Index = 0; Index < NumUsers; Index++)
            {
                Syncs.Add(new SyncClass(Index, GroupID, Providers));
                Syncs.Last().Run();
                //MillimanServices.Administration Admin2 = new MillimanServices.Administration();
                //Admin2.SystemCredentialsValue = new MillimanServices.SystemCredentials();
                //Admin2.SystemCredentialsValue.UserName = "MillimanStagingGuest";
                //Admin2.SystemCredentialsValue.UserPassword = "MSG123454321";
                //Admin2.AddUserCompleted += Admin_AddUserCompleted;
                //Admin2.AddUserAsync("van.nanney@milliman.com", GroupID, GroupID.ToString() + "_" + Index.ToString(), "POPULATION", Providers.ToArray(), true);

                System.Threading.Thread.Sleep(Sleep);
            }
        }
    }

    public class SyncClass
    {
        private int ID { get; set; }
        private Guid _GroupID { get; set; }
        private List<string> _Providers { get; set; }
        public SyncClass(int MyID, Guid GroupID, List<string> Providers)
        {
            ID = MyID;
            _GroupID = GroupID;
            _Providers = Providers;
        }
        public void Run()
        {
            MillimanServices.Administration Admin2 = new MillimanServices.Administration();
            Admin2.SystemCredentialsValue = new MillimanServices.SystemCredentials();
            Admin2.SystemCredentialsValue.UserName = "MillimanStagingGuest";
            Admin2.SystemCredentialsValue.UserPassword = "MSG123454321";
            Admin2.AddUserCompleted += Admin_AddUserCompleted;
            Admin2.AddUserAsync("van.nanney@milliman.com", _GroupID, _GroupID.ToString() + "_" + ID.ToString(), "POPULATION", _Providers.ToArray(), true);
        }
        void Admin_AddUserCompleted(object sender, MillimanServices.AddUserCompletedEventArgs e)
        {
            bool Res = e.Result;
            System.Diagnostics.Debug.WriteLine("     " + ID.ToString() + " completed @" + System.DateTime.Now.ToString() + ":" + Res.ToString());
        }
    }
}