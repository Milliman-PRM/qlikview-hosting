using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
	public partial class PasswordExpired : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblNoticeText.Text = string.Format(@"Your password was last changed more than {0} days ago and is expired. Please click the &quot;Next&quot; button to reset your password.", Session["passwordagedays"]);
			Session.Remove("passwordagedays");
		}

		protected void Next_Click(object sender, ImageClickEventArgs e)
		{
			Response.Redirect("LostPassword.aspx");
		}
	}
}