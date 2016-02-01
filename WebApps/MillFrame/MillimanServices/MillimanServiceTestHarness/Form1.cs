using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MillimanServiceTestHarness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MillimanServices.Administration Admin = new MillimanServices.Administration();
            Admin.SystemCredentialsValue = new MillimanServices.SystemCredentials();
            Admin.SystemCredentialsValue.UserName = "MillimanStagingGuest";
            Admin.SystemCredentialsValue.UserPassword = "MSG123454321";

            Guid AlphaTestGuid = Guid.Parse("444ff1ff-6d5f-461e-bde8-aa3aa91e0043");
            //Guid BetaTestGuid = Guid.Parse("Baaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            //Guid GammaTestGuid = Guid.Parse("Caaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            bool RValue = Admin.ClientAccess("van.nanney@milliman.com", AlphaTestGuid, "AlphaX", true);

            List<MillimanServices.ClientID_NameMap> AllClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.ALL).ToList();
            List<MillimanServices.ClientID_NameMap> EnabledClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.ENABLED).ToList();
            List<MillimanServices.ClientID_NameMap> DisabledClients = Admin.GetClients("van.nanney@milliman.com", MillimanServices.StatusEnum.DISABLED).ToList();
            List<string> Providers = new List<string>() { "p1", "p2" };
            List<Guid> ClientsIDs = new List<Guid>() { AlphaTestGuid };
            RValue = Admin.DeleteUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            RValue = Admin.AddUser("van.nanney@milliman.com", AlphaTestGuid, "tron@somewhere.com", "CARECOORD", Providers.ToArray(), true);
            List<MillimanServices.MillimanUser> Users = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.AddUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com", "COSTMODEL", Providers.ToArray(), true);
            //MillimanServices.MillimanUser MU = Admin.FindUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //List<MillimanServices.MillimanUser> Users = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.AddUser("van.nanney@milliman.com", AlphaTestGuid, "qbert.machine@somewhere.com", "COSTMODEL", Providers.ToArray(), true);
            //List<MillimanServices.MillimanUser> Users3 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.RemoveUserReportAccess("van.nanney@milliman.com", AlphaTestGuid, "qbert.machine@somewhere.com", "COSTMODEL");
            //List<MillimanServices.MillimanUser> Users44 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //RValue = Admin.ModifyUserProviders("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com", "CARECOORD", Providers.ToArray());
            //RValue = Admin.DisableUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //RValue = Admin.DeleteUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //List<MillimanServices.MillimanUser> Users2 = Admin.GetUsers("van.nanney@milliman.com", ClientsIDs.ToArray(), MillimanServices.StatusEnum.ALL).ToList();
            //MillimanServices.MillimanUser MU2 = Admin.FindUser("van.nanney@milliman.com", AlphaTestGuid, "qbert@somewhere.com");
            //string Error = Admin.GetLastError();
           // List<string> ReportIDs = Admin.GetValidReportIDs(AlphaTestGuid).ToList();

        //    Admin.AddUserCompleted += Admin_AddUserCompleted;
        //    Admin.AddUserAsync("van.nanney@milliman.com", Guid.NewGuid(), "qbert", "CARECOORD", Providers.ToArray(), true);
        }

        void Admin_AddUserCompleted(object sender, MillimanServices.AddUserCompletedEventArgs e)
        {
            MessageBox.Show("Add user:" + e.Result.ToString());
        }

  
    }
}
