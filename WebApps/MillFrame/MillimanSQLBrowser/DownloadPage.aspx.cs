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
    public partial class DownloadPage : System.Web.UI.Page
    {
        protected DbUtilConfig conf = (DbUtilConfig)ConfigurationSettings.GetConfig("DbUtilConfig");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Downloader();
            }
        }

        protected void Downloader()
        {
            DataSet ds = new DataSet();
            int count = 1;
            try
            {
                string query = (string)Session["SQLCommand"];  //TxtQuery.Text.Trim();  //"SELECT CustomerID, CompanyName, ContactName, ContactTitle, Address, PostalCode FROM Customers";
                String ConnString = conf.GetConnection("SQL Server").ConnectionString; //ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConnString);

                conn.Open();

                //if (conn is OleDbConnection)
                //{
                //    OleDbDataAdapter adap = new OleDbDataAdapter(query, (OleDbConnection)conn);
                //    adap.Fill(ds, "Table" + count);
                //}
                if (conn is SqlConnection)
                {
                    SqlDataAdapter adap = new SqlDataAdapter(query, (SqlConnection)conn);
                    adap.Fill(ds, "Table" + count);
                }
                //else if (conn is OdbcConnection)
                //{
                //    OdbcDataAdapter adap = new OdbcDataAdapter(query, (OdbcConnection)conn);
                //    adap.Fill(ds, "Table" + count);
                //}

                conn.Close();
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                MillimanCommon.Alert.Show("Results could not be generated because:\\n\\n " + Error);
                Response.AppendCookie(new HttpCookie("downloadstatus", "completed"));
                return;
            }

            Response.AppendCookie(new HttpCookie("downloadstatus", "completed"));
            string ButtonID = Request["buttonid"];
            if (ButtonID == "BtnDownloadXml")
            {
                Response.Clear();
                Response.ContentType = "text/xml";
                Response.AddHeader("Content-Disposition", "attachment; filename=DataSet.xml");
                ds.WriteXml(Response.OutputStream, XmlWriteMode.WriteSchema);
                Response.End();
            }
            else if ((ButtonID == "BtnDownloadCSV") || (ButtonID == "BtnDownloadExcel"))
            {
                // create CSV

                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= count; i++)
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
                        for (int j = 0; j < arr.Count; j++)
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
                if (ButtonID == "BtnDownloadExcel")
                    Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=data.csv");
                Response.Write(sb.ToString());

                Response.End();

            }
        }
    }
}