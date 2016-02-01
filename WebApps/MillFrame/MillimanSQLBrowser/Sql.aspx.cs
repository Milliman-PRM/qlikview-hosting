using System;
using System.Configuration;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace WebSqlUtility
{
	public class SqlBrowser : System.Web.UI.Page
	{
		protected TextBox TxtQuery, TxtQueries;
		protected Button BtnExecute;
		protected Literal LitStatus;
		protected DataGrid GrdMain;
		protected RadioButtonList RblWhichDb;
		protected CheckBox CbxHtmlEncode, CbxAddFav, CbxWrap;
		protected Repeater RepHistory, RepFavourites;
		protected PlaceHolder PlcResultGrids;
		protected int HistoryFileCount = 0;
		protected DbUtilConfig conf = (DbUtilConfig)ConfigurationSettings.GetConfig("DbUtilConfig");
        protected Label UserNameLabel;
        protected Label DatabaseName;
		DataSet ds = new DataSet();
		DataSet dsFav = new DataSet();
		bool oddRow = true;
		int rowIndex = -1;
		

		override protected void OnInit(EventArgs e)
		{

            string MyKey = Request["key"] == null ? "" : "?key=" + Request["key"];
            Response.Redirect("SQLBrowser.aspx" + MyKey);  //this was the original, there is a SQL specific version
            // FAVOURITES DATASET
			try
			{
                //dsFav.ReadXml(Server.MapPath(conf.FavouritesFile));
                dsFav.ReadXml(conf.FavouritesFile);
            }
			catch
			{
				DataTable tblFav = new DataTable("Favourites");
				tblFav.Columns.Add("Query", typeof(string));
				tblFav.Columns.Add("DB", typeof(string));
				dsFav.Tables.Add(tblFav);
			}


			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);

		}

        private void CheckForLogin()
        {
            string Key = Request["key"];
             ///testing mode
             ///
            Key = "test";
            ///testing mode end
            if (string.IsNullOrEmpty(Key) == false)
            {
               string CacheDir = System.Configuration.ConfigurationSettings.AppSettings["HCIntelCache"];  //should be full path in web.config
               string CachePathFileName = System.IO.Path.Combine(CacheDir, Key);

               MillimanCommon.CacheEntry CE = MillimanCommon.CacheEntry.Load( CachePathFileName);
               if (CE != null)
               {
                   string DBCacheDir = System.Configuration.ConfigurationSettings.AppSettings["HCIntelDBBrowserCache"];

                   UserNameLabel.Text = CE.UserName;
                   DatabaseName.Text = CE.DBFriendlyName;
                   System.Data.SqlClient.SqlConnection SC = new System.Data.SqlClient.SqlConnection(CE.ConnectionString);
                   conf.Connections.SetValue(SC, 0);
                   conf.HistoryFolder = System.IO.Path.Combine(DBCacheDir, CE.UserName, CE.DBFriendlyName);
                   if (Directory.Exists(System.IO.Path.Combine(DBCacheDir, CE.UserName)) == false)
                       Directory.CreateDirectory(System.IO.Path.Combine(DBCacheDir, CE.UserName));
                   if (Directory.Exists(conf.HistoryFolder) == false)
                       Directory.CreateDirectory(conf.HistoryFolder);

                   conf.FavouritesFile = System.IO.Path.Combine(DBCacheDir, CE.UserName, CE.DBFriendlyName, @"favorites.xml");
               }
               else
               {
                   Response.Redirect("NoAccess.html");
               }
            }
        }

		private void Page_Load(object sender, System.EventArgs e)
		{

			// HISTORY
			if (ViewState["History"] != null)
				ds = (DataSet)ViewState["History"];
			else
			{
				DataTable tblHist = new DataTable("History");
				tblHist.Columns.Add("Query", typeof(string));
				tblHist.Columns.Add("DB", typeof(string));
				ds.Tables.Add(tblHist);
			}

			if (Request.Form["__EVENTTARGET"] == "RemoveFav")
			{
				dsFav.Tables["Favourites"].Rows.RemoveAt(int.Parse(Request.Form["__EVENTARGUMENT"]));
				dsFav.AcceptChanges();
                //dsFav.WriteXml(Server.MapPath(conf.FavouritesFile));
                dsFav.WriteXml(conf.FavouritesFile);

				PopulateFavourites();
			}

			if (!IsPostBack)
			{
                CheckForLogin();
                
				PopulateConnections();
				PopulateHistory();
				PopulateFavourites();
			}


			TxtQuery.Wrap = CbxWrap.Checked;
		}

		
		protected void ExecuteSQL(object sender, System.EventArgs e)
		{
			string fullQueryText = TxtQuery.Text.Trim();

			string[] queries;
			if (fullQueryText.IndexOf(conf.QuerySeparator) != -1)
				queries = Regex.Split(fullQueryText, Regex.Escape(conf.QuerySeparator));
			else
				// Supports SQL script (i.e. GO statements)
				queries = Regex.Split(fullQueryText + "\r\n", "\r\nGO\r\n");


			IDbCommand cmd = null;

			foreach (string curr in queries)
			{
				string query = curr.Trim();
				if (query.Length == 0)
					continue;


                //IDbConnection conn = conf.GetConnection(RblWhichDb.SelectedValue);
                IDbConnection conn = conf.GetConnection("SQL Server");
                cmd = conn.CreateCommand();
				cmd.CommandText = query;
				conn.Open();

				try
				{
					IDataReader reader = cmd.ExecuteReader();

					bool hasRows = false;
					if (conn is OleDbConnection)
						hasRows = ((OleDbDataReader)reader).HasRows;
					else if (conn is SqlConnection)
						hasRows = ((SqlDataReader)reader).HasRows;
					else if (conn is OdbcConnection)
						hasRows = ((OdbcDataReader)reader).HasRows;

					if (hasRows)
					{
						do
						{
							DataGrid grid = new DataGrid();
							grid.EnableViewState = false;
							grid.CellPadding = 3;
							grid.BorderWidth = 1;
							grid.BorderColor = Color.FromName(conf.ResultGridColors[0]);
							grid.HeaderStyle.BackColor = Color.FromName(conf.ResultGridColors[1]);
							grid.HeaderStyle.ForeColor = Color.FromName(conf.ResultGridColors[2]);
                            grid.HeaderStyle.CssClass = "ForumTableHeader";
							grid.ItemStyle.BackColor = Color.FromName(conf.ResultGridColors[3]);
							grid.AlternatingItemStyle.BackColor = Color.FromName(conf.ResultGridColors[4]);
							grid.DataSource = reader;
							grid.ItemDataBound += new DataGridItemEventHandler(GrdMain_ItemDataBound);
							grid.DataBind();
							PlcResultGrids.Controls.Add(grid);
							PlcResultGrids.Controls.Add(new LiteralControl("<div class=grdRowCount>(" + grid.Items.Count + " rows)</div>"));
						}
						while (reader.NextResult());
					}
					else
					{
						LitStatus.Text += reader.RecordsAffected + " rows affected.<br>";
					}

					cmd.Connection.Close();
				}
				catch (Exception err)
				{
					cmd.Connection.Close();
					LitStatus.Text += "<strong style=color:red>Query Error</strong>: " + err.Message + "<br>";
					return;
				}


				AddToHistory(fullQueryText);

				if (CbxAddFav.Checked)
					AddToFavourites(fullQueryText);

				CbxAddFav.Checked = false; // enableviewstate=false wasn't resetting it GRRR :(
			}
		}


		protected void Download(object sender, System.EventArgs e)
		{
			string fullQueryText = TxtQuery.Text.Trim();

			string[] queries;
			if (fullQueryText.IndexOf(conf.QuerySeparator) != -1)
				queries = Regex.Split(fullQueryText, Regex.Escape(conf.QuerySeparator));
			else
				queries = Regex.Split(fullQueryText + "\r\n", "\r\nGO\r\n");

			DataSet ds = new DataSet();

            //IDbConnection conn = conf.GetConnection(RblWhichDb.SelectedValue);
            IDbConnection conn = conf.GetConnection("SQL Server");
            conn.Open();

			int count = 0;

			foreach (string curr in queries)
			{
				string query = curr.Trim();
				if (query.Length == 0)
					continue;

				count++;

				if (conn is OleDbConnection)
				{
					OleDbDataAdapter adap = new OleDbDataAdapter(query, (OleDbConnection)conn);
					adap.Fill(ds, "Table" + count);
				}
				else if (conn is SqlConnection)
				{
					SqlDataAdapter adap = new SqlDataAdapter(query, (SqlConnection)conn);
					adap.Fill(ds, "Table" + count);
				}
				else if (conn is OdbcConnection)
				{
					OdbcDataAdapter adap = new OdbcDataAdapter(query, (OdbcConnection)conn);
					adap.Fill(ds, "Table" + count);
				}
			}

			conn.Close();


			if (((Button)sender).ID == "BtnDownloadXml")
			{
				Response.ContentType = "text/xml";
				Response.AddHeader("Content-Disposition", "attachment; filename=DataSet.xml");
				ds.WriteXml(Response.OutputStream, XmlWriteMode.WriteSchema);
				Response.End();
			}
			else if (((Button)sender).ID == "BtnDownloadCSV")
			{
				// create CSV

				StringBuilder sb = new StringBuilder();
				for (int i=1; i<=count; i++)
				{
					sb.Append("------------ Table" + i + " ------------\r\n");

					// header
					foreach (DataColumn col in ds.Tables["Table" + i].Columns)
						sb.AppendFormat("{0},", col.ColumnName);
					if (sb.ToString().EndsWith(","))
						sb.Remove(sb.Length - 1, 1);
					sb.Append("\r\n");

					// rows
					foreach (DataRow row in ds.Tables["Table" + i].Rows)
					{
						ArrayList arr = new ArrayList(row.ItemArray);
						for (int j=0; j<arr.Count; j++)
						{
							string val = arr[j].ToString();
							if (val.IndexOf(",") != -1 || val.IndexOf("\n") != -1) // quote values containing comma or line-break
								val = "\"" + val.Replace("\"", "\"\"") + "\"";
							arr[j] = val;
						}
						string line = string.Join(",", (string[])arr.ToArray(typeof(string)));
						sb.Append(line + "\r\n");
					}
				}

				Response.ContentType = "text/csv";
				Response.AddHeader("Content-Disposition", "attachment; filename=data.csv");
				Response.Write(sb.ToString());
				Response.End();
			}
		}


		void PopulateConnections()
		{
            //foreach (string name in conf.Names)
            //{
            //    string label = string.Format("{0}</label> <a href=# onclick=\"showSchema('{0}'); return false\">(Schema)</a>", name);
            //    ListItem item = new ListItem(label, name);
            //    RblWhichDb.Items.Add(item);
            //}

            //if (RblWhichDb.Items.Count > 0)
            //    RblWhichDb.SelectedIndex = 0;
		}


		// update repeater
		void PopulateHistory()
		{
			RepHistory.DataSource = ds.Tables["History"];
			RepHistory.DataBind();

			ViewState["History"] = ds;

			string path = conf.HistoryFolder;
			if (path != null)
			{
				try
				{
                    //if (!Directory.Exists(Server.MapPath(path)))
                    //    // Create the folder
                    //    Directory.CreateDirectory(Server.MapPath(path));
                    //else
                    //    // Get the number of history files
                    //    HistoryFileCount = Directory.GetFiles(Server.MapPath(path), "*.xml").Length;
                    if (!Directory.Exists(path))
                        // Create the folder
                        Directory.CreateDirectory(path);
                    else
                        // Get the number of history files
                        HistoryFileCount = Directory.GetFiles(path, "*.xml").Length;

				}
				catch {}
			}
		}


		void AddToHistory(string fullQueryText)
		{
			// remove previous entry of this query if there is
			for (int i=0; i<ds.Tables[0].Rows.Count; i++)
			{
				if ((string)ds.Tables[0].Rows[i][0] == fullQueryText)
				{
					ds.Tables[0].Rows.RemoveAt(i);
					break;
				}
			}
			// insert query at top of table
			DataRow row = ds.Tables[0].NewRow();
			row[0] = fullQueryText;
            //row[1] = RblWhichDb.SelectedValue;
            row[1] = @"SQL Server";
            ds.Tables[0].Rows.InsertAt(row, 0);


			// save to file if option configured
			if (conf.HistoryFolder != null)
			{
				string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";
				if (ViewState["HistoryFile"] != null)
					filename = (string)ViewState["HistoryFile"];
				else
					ViewState["HistoryFile"] = filename;

				string relPath = Path.Combine(conf.HistoryFolder, filename);

                //ds.WriteXml(Server.MapPath(relPath));
                ds.WriteXml(relPath);
            }
			
			
			PopulateHistory();
		}


		void PopulateFavourites()
		{
			RepFavourites.DataSource = dsFav.Tables["Favourites"];
			RepFavourites.DataBind();
		}

		
		void AddToFavourites(string fullQueryText)
		{
            //dsFav.Tables["Favourites"].Rows.Add(new object[] { fullQueryText, RblWhichDb.SelectedValue });
            dsFav.Tables["Favourites"].Rows.Add(new object[] { fullQueryText, "SQL Server" });
            //dsFav.WriteXml(Server.MapPath(conf.FavouritesFile));
            dsFav.WriteXml(conf.FavouritesFile);
            PopulateFavourites();
		}


		private void GrdMain_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (CbxHtmlEncode.Checked)
			{
				foreach (TableCell cell in e.Item.Cells)
				{
					if (cell.Text != "&nbsp;")
						cell.Text = Server.HtmlEncode(cell.Text).Replace("\n", "\n<br>");
				}
			}
		}

		
		protected bool IsOddRow()
		{
			return (oddRow = !oddRow);
		}

		
		protected int GetRowIndex()
		{
			rowIndex++;
			return rowIndex;
		}

		
		/// <summary>
		/// Generate INSERT queries for the result set
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GetQueries(object sender, System.EventArgs e)
		{
            //IDbConnection conn = conf.GetConnection(RblWhichDb.SelectedValue);
            IDbConnection conn = conf.GetConnection("SQL Server");
            conn.Open();

			string fullQueryText = TxtQuery.Text.Trim();

			string[] queries;
			if (fullQueryText.IndexOf(conf.QuerySeparator) != -1)
				queries = Regex.Split(fullQueryText, Regex.Escape(conf.QuerySeparator));
			else
				queries = Regex.Split(fullQueryText + "\r\n", "\r\nGO\r\n");

			StringBuilder sbQueries = new StringBuilder();

			foreach (string curr in queries)
			{
				string query = curr.Trim();
				if (query.Length == 0)
					continue;

				IDataReader reader = null;

				try
				{
					IDbCommand cmd = conn.CreateCommand();
					cmd.CommandText = query;

					reader = cmd.ExecuteReader();
				
					bool hasRows = false;
					if (conn is OleDbConnection)
						hasRows = ((OleDbDataReader)reader).HasRows;
					else if (conn is SqlConnection)
						hasRows = ((SqlDataReader)reader).HasRows;
					else if (conn is OdbcConnection)
						hasRows = ((OdbcDataReader)reader).HasRows;

					if (hasRows)
					{
						string fieldNames = "";
						bool first = true;
						while (reader.Read())
						{
							if (first)
							{
								// column names
								ArrayList columns = new ArrayList();
								for (int i=0; i<reader.FieldCount; i++)
									columns.Add(reader.GetName(i));
								fieldNames = String.Join(",", (string[])columns.ToArray(typeof(string)));
							}

							ArrayList values = new ArrayList();
							for (int i=0; i<reader.FieldCount; i++)
							{
								bool sqlServer = true;
								if (reader[i] == DBNull.Value)
									values.Add("NULL");
								else if (sqlServer && reader[i] is Boolean)
									values.Add( ((bool)reader[i])?"1":"0" );
								else
									values.Add("'" + reader[i].ToString().Replace("'", "''") + "'");
							}
							string fieldValues = String.Join(",", (string[])values.ToArray(typeof(string)));

							string tblName = "TABLE_NAME";
							// get table name from SELECT query
							Match m = Regex.Match(query, @"FROM\s\[?(.+?)\]?(\s|$)", RegexOptions.IgnoreCase);
							if (m.Success)
								tblName = m.Groups[1].Value;

							sbQueries.AppendFormat("INSERT INTO {0} ({1}) VALUES({2}){3}\r\n", tblName, fieldNames, fieldValues, conf.QuerySeparator);

							first = false;
						}
					}
					else
					{
						LitStatus.Text = "Query must return a resultset to generate queries";
					}
				}
				catch (Exception err)
				{
					LitStatus.Text = "<strong style=color:red>Query Error</strong>: " + err.Message;
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}

			conn.Close();

			
			TxtQueries.Text = sbQueries.ToString().TrimEnd();
			TxtQueries.Visible = true;

			// JS to select / focus the textbox
			string js = "<script>document.getElementById('TxtQueries').select(); document.getElementById('TxtQueries').focus()</script>";
			PlcResultGrids.Controls.Add(new LiteralControl(js));
		}

	}
}
