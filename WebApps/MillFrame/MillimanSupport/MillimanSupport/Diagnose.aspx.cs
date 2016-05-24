using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanSupport
{
    public partial class Diagnose : System.Web.UI.Page
    {
        private string TestItems = "Test Results (" + System.DateTime.Now.ToString() + ")%0A";

        protected void AddItem(string Option, string Status, string Notes, string ValueTag = "")
        {
            TableRow TR = new TableRow();

            TableCell TC = new TableCell();
            TC.Text = Option;
            TR.Cells.Add(TC);

            TC = new TableCell();
            TC.Text = Status;
            TC.ID = ValueTag;

            TR.Cells.Add(TC);

            TC = new TableCell();
            TC.Text = Notes;
            TR.Cells.Add(TC);

            StatusTable.Rows.Add(TR);

            if (string.IsNullOrEmpty(TestItems) == false)
                TestItems += "%0A";
            TestItems += Option + " : " + Status;
        }

        private string SafeGet(dynamic Value)
        {
            try
            {
                return Value.ToString();
            }
            catch (Exception)
            {
            }
            return "Not Available";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddItem("PRM Server access", "---", "Length of time to access server", "ping");
                AddItem("ActiveX allowed", SafeGet(Request.Browser.ActiveXControls), "Are activeX controls allowed to execute");
                AddItem("Backgound audio allowed", SafeGet(Request.Browser.BackgroundSounds), "Is background audio enabled");
                AddItem("Browser name", SafeGet(Request.Browser.Browser), "The name of the current browser");
                AddItem("Can render after select", SafeGet(Request.Browser.CanRenderAfterInputOrSelectElement), "Does the browser re-render after input selection");
                AddItem("Can send email", SafeGet(Request.Browser.CanSendMail), "Does the browser integrate to an email client");
                AddItem("Allows webcast", SafeGet(Request.Browser.CDF), "Does the browser allow muti-media webcast(video/audio)");
                AddItem("Active CLR", SafeGet(Request.Browser.ClrVersion), "What is the current .Net runtime");
                AddItem("Allows cookies", SafeGet(Request.Browser.Cookies), "Are cookies enabled");
                AddItem("Allows crawlers", SafeGet(Request.Browser.Crawler), "Does the browser allow web crawlers to view content");
                AddItem("ECMA script version", SafeGet(Request.Browser.EcmaScriptVersion), "What version of ECMA script is available");
                AddItem("Allows frames", SafeGet(Request.Browser.Frames), "Does the browser allow frames/iframes");

                AddItem("Gateway version", SafeGet(Request.Browser.GatewayVersion), "");
                //string CLRs = "";
                //foreach (var Item in SafeGet(Request.Browser.GetClrVersions())
                //{
                //    if (string.IsNullOrEmpty(CLRs) == false)
                //        CLRs += ";";
                //    CLRs += Item.ToString();
                //}
                //AddItem("All CLRs", CLRs, "Availble browser CLRs");
                AddItem("Browser has back button", SafeGet(Request.Browser.HasBackButton), "Browser supports a 'back' button");
                AddItem("Browser is mobile", SafeGet(Request.Browser.IsMobileDevice), "Browser is a known mobile browser");
                AddItem("Allows applets", SafeGet(Request.Browser.JavaApplets), "Java applets are allowed");
                AddItem("Allows Jscript", SafeGet(Request.Browser.JScriptVersion), "Jscript(Javascript) is allowed");
                AddItem("Max Href length", SafeGet(Request.Browser.MaximumHrefLength), "Maximumn length of a link request");
                AddItem("Max page size", SafeGet(Request.Browser.MaximumRenderedPageSize), "Maximum size of page allowed by browser");

                AddItem("Browser version", SafeGet(Request.Browser.MajorVersion) + ":" + SafeGet(Request.Browser.MinorVersion), "Browser version");
                AddItem("Mobile device manufacturer", SafeGet(Request.Browser.MobileDeviceManufacturer), "Company that manufactured the device");
                AddItem("Mobile device model", SafeGet(Request.Browser.MobileDeviceModel), "Model of the device");
                AddItem("MS DOM version", SafeGet(Request.Browser.MSDomVersion), "The version of the MS document object model being used");
                AddItem("Number soft keys", SafeGet(Request.Browser.NumberOfSoftkeys), "Virtual keys ( on-screen-display )");
                AddItem("Base platform", SafeGet(Request.Browser.Platform), "The base operating system");

                AddItem("Supports CSS", SafeGet(Request.Browser.SupportsCss), "Is CSS supported");
                AddItem("Supports cookie redirect", SafeGet(Request.Browser.SupportsRedirectWithCookie), "Will the browser support redirects with cookies");
                AddItem("Supports XML via HTTP", SafeGet(Request.Browser.SupportsXmlHttp), "Does the browser support access to XML via HTTP");
                AddItem("Allows VBScript", SafeGet(Request.Browser.VBScript), "Is VB script allowed");
                AddItem("W3C DOM version", SafeGet(Request.Browser.W3CDomVersion), "Version of W3C document object model in use");
                AddItem("Is Win32", SafeGet(Request.Browser.Win32), "Is the browser Win32 based.");
                AddItem("User agent", SafeGet(Request.UserAgent), "Information related to the software artificats located at each end of the communication pipeline");
                AddItem("Host address", SafeGet(Request.UserHostAddress), "Address associated with the machine on which the browser is located( may be proxied )");
                string Languages = "";
                try
                {
                    foreach (string S in Request.UserLanguages)
                    {
                        if (string.IsNullOrWhiteSpace(S) == false)
                            Languages += ", ";
                        Languages += S;
                    }
                }
                catch (Exception)
                {
                    Languages = "Not Available";
                }
                AddItem("Culture", Languages, "The languages install for use by the browser");

                LaunchEmail1.NavigateUrl = "mailto:prm.support@milliman.com?subject=Server%20Access%20Test%20Results&body=" + TestItems;
                LaunchEmail2.NavigateUrl = "mailto:prm.support@milliman.com?subject=Server%20Access%20Test%20Results&body=" + TestItems;
            }
        }
    }
}