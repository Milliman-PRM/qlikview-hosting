using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;


namespace WebSqlUtility
{
	public class SqlSchema : System.Web.UI.Page
	{
		protected PlaceHolder PlcGrids;

		Color[] rowColors = new Color[] { Color.LightGoldenrodYellow, Color.FromArgb(240, 240, 240) };

		protected DbUtilConfig conf = (DbUtilConfig)ConfigurationSettings.GetConfig("DbUtilConfig");


		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}


		private void Page_Load(object sender, System.EventArgs e)
		{
			IDbConnection conn = conf.GetConnection(Request.QueryString["DB"]);

			if (conn is OleDbConnection)
				SchemaOleDb();
			else if (conn is SqlConnection)
				SchemaSqlServer();
			else if (conn is OdbcConnection && conn.ConnectionString.ToLower().IndexOf("mysql") != -1)
				SchemaMySQL();
			else
				Response.Write("[ERROR] Can only show schema for OleDB connections, SQL Server, and MySQL.");
		}

      

		void SchemaSqlServer()
		{
			DataSet ds = new DataSet();
			SqlConnection conn = (SqlConnection)conf.GetConnection(Request.QueryString["DB"]);
			conn.Open();

			SqlDataAdapter adap = new SqlDataAdapter("SELECT * FROM sysobjects WHERE xtype='U' AND name<>'dtproperties' order by name", conn);
			adap.Fill(ds, "Tables");

			PlcGrids.Controls.Add(new LiteralControl("<p style='line-height:17px;'>"));
			foreach (DataRow row in ds.Tables["Tables"].Rows)
			{
				PlcGrids.Controls.Add(new LiteralControl( String.Format("- <a href='#tbl{0}'>{0}</a><br>", row["name"]) ));
			}
			PlcGrids.Controls.Add(new LiteralControl("</p>"));


			foreach (DataRow row in ds.Tables["Tables"].Rows)
			{
				DataGrid grd = new DataGrid();
				grd.CellPadding = 3;
				grd.HeaderStyle.BackColor = rowColors[0];
                grd.HeaderStyle.CssClass = "ForumTableHeader";
				grd.AlternatingItemStyle.BackColor = rowColors[1];
				grd.ItemDataBound += new DataGridItemEventHandler(Grd_DataBound2);
				
				DataTable schemaTable = new DataTable();
				string query = "";
				try
				{
					adap = new SqlDataAdapter(string.Format("sp_MShelpcolumns '{0}'", row["name"].ToString().Replace("'", "''")), conn);
					adap.Fill(schemaTable);
				}
				catch (Exception err)
				{
					Response.Write(err.Message + " (" + query + ")<br>");
					continue;
				}

				PlcGrids.Controls.Add(new LiteralControl( String.Format("<a name='tbl{0}'></a><h2><span class=insertLink onmouseover=this.className='insertLinkHover' onmouseout=this.className='insertLink' onclick=insertSql(this)>{0}</span> &nbsp;&nbsp;<a href=#top class=topLink>(Top)</a></h2>", row["name"]) ));

				grd.DataSource = schemaTable;
				grd.DataBind();
				PlcGrids.Controls.Add(grd);
				
				PlcGrids.Controls.Add(new LiteralControl("<br>"));
			}

			conn.Close();
		}


		void SchemaOleDb()
		{
			OleDbConnection conn = (OleDbConnection)conf.GetConnection(Request.QueryString["DB"]);
			conn.Open();

			DataTable tblDbTables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "TABLE"} );

			PlcGrids.Controls.Add(new LiteralControl("<p style='line-height:17px;'>"));
			foreach (DataRow row in tblDbTables.Rows)
			{
				PlcGrids.Controls.Add(new LiteralControl( String.Format("- <a href='#tbl{0}'>{0}</a><br>", row["TABLE_NAME"]) ));
			}
			PlcGrids.Controls.Add(new LiteralControl("</p>"));


			foreach (DataRow row in tblDbTables.Rows)
			{
				DataGrid grd = new DataGrid();
				grd.ItemDataBound += new DataGridItemEventHandler(Grd_DataBound);
				grd.ItemDataBound += new DataGridItemEventHandler(Grd_DataBound2);
				grd.CellPadding = 3;
				grd.HeaderStyle.BackColor = rowColors[0];
				grd.AlternatingItemStyle.BackColor = rowColors[1];

				
				OleDbDataReader reader;
				DataTable schemaTable;
				string query = "";
				try
				{
					query = "SELECT TOP 1 * FROM " + row["TABLE_NAME"].ToString();
					reader = new OleDbCommand(query, conn).ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
					schemaTable = reader.GetSchemaTable();
				}
				catch (Exception err)
				{
					Response.Write(err.Message + " (" + query + ")<br>");
					continue;
				}


				PlcGrids.Controls.Add(new LiteralControl( String.Format("<a name='tbl{0}'></a><h2><span class=insertLink onmouseover=this.className='insertLinkHover' onmouseout=this.className='insertLink' onclick=insertSql(this)>{0}</span> &nbsp;&nbsp;<a href=#top class=topLink>(Top)</a></h2>", row["TABLE_NAME"]) ));

				grd.DataSource = schemaTable;
				grd.DataBind();
				PlcGrids.Controls.Add(grd);
				
				reader.Close();

				PlcGrids.Controls.Add(new LiteralControl("<br>"));
			}

            
			conn.Close();
		}


		void SchemaMySQL()
		{
			DataSet ds = new DataSet();

			OdbcConnection conn = (OdbcConnection)conf.GetConnection(Request.QueryString["DB"]);
			conn.Open();

			OdbcDataAdapter adap = new OdbcDataAdapter("SHOW TABLES", conn);
			adap.Fill(ds, "Tables");

			PlcGrids.Controls.Add(new LiteralControl("<p style='line-height:17px;'>"));
			foreach (DataRow row in ds.Tables["Tables"].Rows)
			{
				PlcGrids.Controls.Add(new LiteralControl( String.Format("- <a href='#tbl{0}'>{0}</a><br>", row[0]) ));
			}
			PlcGrids.Controls.Add(new LiteralControl("</p>"));


			foreach (DataRow row in ds.Tables["Tables"].Rows)
			{
				DataTable tblSchema = new DataTable();

				DataGrid grd = new DataGrid();
				grd.DataSource = tblSchema;
				grd.CellPadding = 3;
				grd.HeaderStyle.BackColor = rowColors[0];
				grd.AlternatingItemStyle.BackColor = rowColors[1];
				grd.ItemDataBound += new DataGridItemEventHandler(Grd_DataBound2);

				try
				{
					adap = new OdbcDataAdapter("SHOW COLUMNS IN `" + row[0] + "`", conn);
					adap.Fill(tblSchema);
				}
				catch (Exception err)
				{
					Response.Write(err.Message + " (" + row[0] + ")<br>");
					continue;
				}

				PlcGrids.Controls.Add(new LiteralControl( String.Format("<a name='tbl{0}'></a><h2><span class=insertLink onmouseover=this.className='insertLinkHover' onmouseout=this.className='insertLink' onclick=insertSql(this)>{0}</span> &nbsp;&nbsp;<a href=#top class=topLink>(Top)</a></h2>", row[0]) ));
				
				grd.DataBind();
				PlcGrids.Controls.Add(grd);
				PlcGrids.Controls.Add(new LiteralControl("<br>"));
			}

			conn.Close();
		}


		// display the meaningful name of the column type 
		private void Grd_DataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				int typeNum = Int32.Parse(e.Item.Cells[5].Text);
				OleDbType t = (OleDbType) typeNum;
				e.Item.Cells[5].Text = t.ToString();
			}
		}


		// Make column name into link
		private void Grd_DataBound2(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
				e.Item.Cells[0].Text = string.Format("<span class=insertLink onmouseover=this.className='insertLinkHover' onmouseout=this.className='insertLink' onclick=insertSql(this)>{0}</span>", e.Item.Cells[0].Text);
		}

	}
}
