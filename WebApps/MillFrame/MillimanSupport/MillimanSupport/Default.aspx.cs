using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Xml;
using System;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Collections;

namespace MillimanSupport
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["patientid"] = "147027976Z"; //test

           //MillimanDev2.Announcements.Announcements.CreateTemplate();

            if (!IsPostBack)
            {
                //if (IsNewUser(Membership.GetUser().ProviderUserKey.ToString()) == true)
                //{
                //    Response.Redirect("profile.aspx?newuser=true");
                //    return;
                //}
                //LoadAnnouncements();
                //LoadProducts();
            }
        }

        /// <summary>
        /// given the users ID,  do they have to reset thier password and enter info
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private bool IsNewUser(string UserID)
        {
            try
            {
                string ResetFile = System.IO.Path.Combine( WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst" );
                return System.IO.File.Exists(ResetFile);
            }
            catch (Exception)
            {

            }
  
            return false;
        }

        private string CreateCacheEntry(string ConnectionStringFriendlyName, string ConnectionString )
        {
            string CacheDir = WebConfigurationManager.AppSettings["HCIntelCache"];  //should be full path in web.config
            string CacheFileName = Guid.NewGuid().ToString().Replace('-', '_');
            string CachePathFileName = System.IO.Path.Combine(CacheDir, CacheFileName);
            string UserKey = Membership.GetUser().ProviderUserKey.ToString();
            MillimanCommon.CacheEntry CE = new MillimanCommon.CacheEntry(ConnectionStringFriendlyName, ConnectionString, UserKey, Membership.GetUser().UserName, "", DateTime.Now.AddDays(1.0));
            CE.Save(CachePathFileName);

            return System.IO.Path.GetFileNameWithoutExtension(CacheFileName);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {

        }

      
    }
}