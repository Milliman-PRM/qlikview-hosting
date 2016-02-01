using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Data.Odbc;
using System.Drawing;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;


namespace WebSqlUtility
{
    public partial class GridPage : System.Web.UI.Page
    {
        protected DbUtilConfig conf = (DbUtilConfig)ConfigurationSettings.GetConfig("DbUtilConfig");
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteSQL(null, null);
        }

        //Requires install of  sqlite-netFx40-setup-bundle-x64-2010-1.0.90.0.exe
        //using System.Data.SQLite;
        //Add reference to 
        //    SQLite.Designer
        //    System.Data.SQLite
        //    System.Data.SQLite.linq
        //Visual Studio must use IIS to debug,  since SQ assemblies are mixed mode and require 64x and VS cassini is 32x 
        //public DataTable GetSQLiteDataTable()
        //{
        //    string sql = (string)Session["SQLCommand"];
        //    string connectionString = @"Data Source=C:\WIP 17 - NewLoginGraphics_TEMP\Databases\3.037-AHN39-Member_Only.sqlite;Version=3;Read Only=True;";
        //    DataTable dt = new DataTable();
        //    SQLiteDataAdapter adapter;
        //    // Connect to database.
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    // Create database adapter using specified query
        //    using (adapter = new SQLiteDataAdapter(sql, connection))
        //    // Create command builder to generate SQL update, insert and delete commands
        //    using (SQLiteCommandBuilder command = new SQLiteCommandBuilder(adapter))
        //    {
        //        // Populate datatable to return, using the database adapter                
        //        adapter.Fill(dt);
        //    }
        //    return dt;
        //}

        public DataTable GetSQLDataTable()
        {
            try
            {
                ErrorMsg.Visible = false;
                RadGrid1.Visible = true;
                string query = (string)Session["SQLCommand"];  //TxtQuery.Text.Trim();  //"SELECT CustomerID, CompanyName, ContactName, ContactTitle, Address, PostalCode FROM Customers";
                String ConnString = conf.GetConnection("SQL Server").ConnectionString; //ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConnString);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, conn);
                DataTable myDataTable = new DataTable();
                conn.Open();
                try
                {
                    adapter.Fill(myDataTable);
                    //AddToHistory(TxtQuery.Text.Trim());
                }
                finally
                {
                    conn.Close();
                }
                return myDataTable;
            }
            catch (Exception ex)
            {
                ErrorMsg.Text = "*** " + ex.Message;
                ErrorMsg.Visible = true;
                RadGrid1.Visible = false;
            }
            return null;
        }

        protected void ExecuteSQL(object sender, System.EventArgs e)
        {
            //RadGrid1.DataSource = GetSQLiteDataTable();
            RadGrid1.DataSource = GetSQLDataTable();
            if (RadGrid1.DataSource != null)
                RadGrid1.DataBind();
        }

        protected void RadGrid1_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            ExecuteSQL(null, null);
        }

        protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            ExecuteSQL(null, null);
        }

    }
}