using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;


namespace WebSqlUtility
{
	public class SqlHistory : System.Web.UI.Page
	{
		protected Repeater RepFullHistory;
		protected DbUtilConfig conf = (DbUtilConfig)ConfigurationSettings.GetConfig("DbUtilConfig");


		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}


		private void Page_Load(object sender, System.EventArgs e)
		{
            //if (conf.HistoryFolder != null && Directory.Exists(Server.MapPath(conf.HistoryFolder)))
            if (conf.HistoryFolder != null && Directory.Exists(conf.HistoryFolder))
                {
				if (Request.QueryString["del"] != null)
				{
					// check QS var
					if (Regex.IsMatch(Request.QueryString["del"], @"^\d+_\d+\.xml$", RegexOptions.IgnoreCase))
					{
						string relPath = Path.Combine(conf.HistoryFolder, Request.QueryString["del"]);
                        //File.Delete(Server.MapPath(relPath));
                        File.Delete(relPath);
                    }
					else if (Request.QueryString["del"] == "*")
					{
                        //foreach (string path in Directory.GetFiles(Server.MapPath(conf.HistoryFolder), "*.xml"))
                        foreach (string path in Directory.GetFiles(conf.HistoryFolder, "*.xml"))
                                File.Delete(path);
					}

					Response.Redirect(Request.Path);  // i.e. no QS
				}

                //ArrayList arr = new ArrayList(Directory.GetFiles(Server.MapPath(conf.HistoryFolder), "*.xml"));
                ArrayList arr = new ArrayList(Directory.GetFiles(conf.HistoryFolder, "*.xml"));

				if (arr.Count == 0)
					Response.Write("-- no history --");
				else
				{
					arr.Sort();
					arr.Reverse(); // newest to oldest

					RepFullHistory.DataSource = arr;
					RepFullHistory.ItemDataBound += new RepeaterItemEventHandler(RepFullHistory_ItemDataBound);
					DataBind();
				}
			}
		}


		protected string GetDate(string filename)
		{
			Match m = Regex.Match(filename, @"(\d{4})(\d{2})(\d{2})_(\d{2})(\d{2})(\d{2})");
			if (m.Success)
				return string.Format("{0}/{1}/{2} {3}:{4}:{5}", m.Groups[3], m.Groups[2], m.Groups[1], m.Groups[4], m.Groups[5], m.Groups[6]);
			else
				return filename;
		}


		private void RepFullHistory_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			DataSet ds = new DataSet();
			ds.ReadXml((string)e.Item.DataItem);

			Repeater rep = (Repeater)e.Item.FindControl("RepHistory");
			rep.DataSource = ds;
			rep.DataBind();
		}
	
	}
}
