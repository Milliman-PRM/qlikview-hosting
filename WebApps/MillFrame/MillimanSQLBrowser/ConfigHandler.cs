using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;


namespace WebSqlUtility
{

	public class DbUtilConfig
	{
		public string[] Names = new string[0];
		public IDbConnection[] Connections = new IDbConnection[0];
		public string FavouritesFile;
		public string HistoryFolder;
		public string QuerySeparator = ";";
		public string[] ResultGridColors = new string[] { "#777", "#ffc", "#000", "#fff", "#eee" };

		public IDbConnection GetConnection(string name)
		{
			int index = Array.IndexOf(Names, name);
			if (index >= 0)
				return Connections[index];
			else
				return null;
		}
	}


	public class ConfigHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode sectionNode)
		{
			DbUtilConfig conf = new DbUtilConfig();

			ArrayList arrConnections = new ArrayList(), arrNames = new ArrayList();

			XmlNodeList nodes = sectionNode.SelectNodes("Connections/Connection");
			foreach (XmlNode node in nodes)
			{
				string name = node.Attributes["name"].Value;
				string type = node.Attributes["type"].Value;
				string cs = null;
				if (node.Attributes["csAppKey"] != null)
					cs = (string)HttpContext.Current.Application[node.Attributes["csAppKey"].Value];
				else if (node.Attributes["csConfKey"] != null)
					cs = System.Configuration.ConfigurationManager.AppSettings[node.Attributes["csConfKey"].Value];
				else
					cs = node.Attributes["cs"].Value;

				if (type == "oledb")
					arrConnections.Add(new OleDbConnection(cs));
				else if (type == "mssql")
					arrConnections.Add(new SqlConnection(cs));
				else if (type == "odbc")
					arrConnections.Add(new OdbcConnection(cs));

				arrNames.Add(name);
			}

			conf.Connections = (IDbConnection[])arrConnections.ToArray(typeof(IDbConnection));
			conf.Names = (string[])arrNames.ToArray(typeof(string));


			if (sectionNode.SelectSingleNode("FavouritesFile") != null)
				conf.FavouritesFile = sectionNode.SelectSingleNode("FavouritesFile").InnerText;

			if (sectionNode.SelectSingleNode("HistoryFolder") != null)
				conf.HistoryFolder = sectionNode.SelectSingleNode("HistoryFolder").InnerText;

			if (sectionNode.SelectSingleNode("QuerySeparator") != null)
				conf.QuerySeparator = sectionNode.SelectSingleNode("QuerySeparator").InnerText;

			if (sectionNode.SelectSingleNode("ResultGridColors") != null)
			{
				if (sectionNode.SelectSingleNode("ResultGridColors").Attributes["Border"] != null)
					conf.ResultGridColors[0] = sectionNode.SelectSingleNode("ResultGridColors").Attributes["Border"].InnerText;
				if (sectionNode.SelectSingleNode("ResultGridColors").Attributes["HeaderBg"] != null)
					conf.ResultGridColors[1] = sectionNode.SelectSingleNode("ResultGridColors").Attributes["HeaderBg"].InnerText;
				if (sectionNode.SelectSingleNode("ResultGridColors").Attributes["Header"] != null)
					conf.ResultGridColors[2] = sectionNode.SelectSingleNode("ResultGridColors").Attributes["Header"].InnerText;
				if (sectionNode.SelectSingleNode("ResultGridColors").Attributes["ItemBg"] != null)
					conf.ResultGridColors[3] = sectionNode.SelectSingleNode("ResultGridColors").Attributes["ItemBg"].InnerText;
				if (sectionNode.SelectSingleNode("ResultGridColors").Attributes["AltItemBg"] != null)
					conf.ResultGridColors[4] = sectionNode.SelectSingleNode("ResultGridColors").Attributes["AltItemBg"].InnerText;
			}

			return conf;
		}
	}

}
