using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillframeConfigComparisonLib
{
    public class ComparisonResult
    {
        public DataSet ComparisonResults = new DataSet("ComparisonResults");
        Configuration Config1;
        Configuration Config2;
        public readonly static string KEY_NOT_FOUND = "MillframeConfigComparisonLib:KEY_NOT_FOUND";

        public ComparisonResult(string Path1, string Path2, List<string> Required1 = null, List<string> Required2 = null)
        {
            ExeConfigurationFileMap ConfigFileMap;
            List<DataTable> ResultTableList = new List<DataTable>();

            ConfigFileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path1 };
            Config1 = ConfigurationManager.OpenMappedExeConfiguration(ConfigFileMap, ConfigurationUserLevel.None);

            ConfigFileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path2 };
            Config2 = ConfigurationManager.OpenMappedExeConfiguration(ConfigFileMap, ConfigurationUserLevel.None);

            #region KeysInBothPaths
            DataTable TableOfKeysInBothPaths = new DataTable("KeysInBothPaths");
            TableOfKeysInBothPaths.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInBothPaths.Columns.Add("Path 1 Value", typeof(string));
            TableOfKeysInBothPaths.Columns.Add("Path 2 Value", typeof(string));

            var KeysInBothPaths = Config1.AppSettings.Settings.AllKeys.AsQueryable().Intersect(Config2.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInBothPaths)
            {
                DataRow NewRow = TableOfKeysInBothPaths.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 1 Value"] = Config1.AppSettings.Settings[Key].Value;
                NewRow["Path 2 Value"] = Config2.AppSettings.Settings[Key].Value;
                TableOfKeysInBothPaths.Rows.Add(NewRow);
            }
            ResultTableList.Add(TableOfKeysInBothPaths);
            #endregion

            #region TableOfKeysInPath1Only
            DataTable TableOfKeysInPath1Only = new DataTable("KeysInPath1Only");
            TableOfKeysInPath1Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath1Only.Columns.Add("Path 1 Value", typeof(string));
            var KeysInPath1Only = Config1.AppSettings.Settings.AllKeys.AsQueryable().Except(Config2.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath1Only)
            {
                DataRow NewRow = TableOfKeysInPath1Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 1 Value"] = Config1.AppSettings.Settings[Key].Value;
                TableOfKeysInPath1Only.Rows.Add(NewRow);
            }
            ResultTableList.Add(TableOfKeysInPath1Only);
            #endregion

            #region TableOfKeysInPath2Only
            DataTable TableOfKeysInPath2Only = new DataTable("KeysInPath2Only");
            TableOfKeysInPath2Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath2Only.Columns.Add("Path 2 Value", typeof(string));
            var KeysInPath2Only = Config2.AppSettings.Settings.AllKeys.AsQueryable().Except(Config1.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath2Only)
            {
                DataRow NewRow = TableOfKeysInPath2Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 2 Value"] = Config2.AppSettings.Settings[Key].Value;
                TableOfKeysInPath2Only.Rows.Add(NewRow);
            }
            ResultTableList.Add(TableOfKeysInPath2Only);
            #endregion

            #region TableOfConnectionStrings
            DataTable TableOfConnectionStrings = new DataTable("ConnectionStrings");
            TableOfConnectionStrings.Columns.Add("Connection String Name", typeof(string));
            TableOfConnectionStrings.Columns.Add("Path 1 Value", typeof(string));
            TableOfConnectionStrings.Columns.Add("Path 2 Value", typeof(string));
            HashSet<string> Path1ConnectionStringNames = new HashSet<string>();
            HashSet<string> Path2ConnectionStringNames = new HashSet<string>();
            foreach (ConnectionStringSettings ConnectionSettings in Config1.ConnectionStrings.ConnectionStrings)
            {
                Path1ConnectionStringNames.Add(ConnectionSettings.Name);
            }
            foreach (ConnectionStringSettings ConnectionSettings in Config2.ConnectionStrings.ConnectionStrings)
            {
                Path2ConnectionStringNames.Add(ConnectionSettings.Name);
            }

            // Connectionstring in both paths
            foreach (string ConnectionName in Path1ConnectionStringNames.Intersect(Path2ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = Config1.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                NewRow["Path 2 Value"] = Config2.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }
            // Connectionstring in Path1 only
            foreach (string ConnectionName in Path1ConnectionStringNames.Except(Path2ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = Config1.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                NewRow["Path 2 Value"] = null;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }
            // Connectionstring in Path2 only
            foreach (string ConnectionName in Path2ConnectionStringNames.Except(Path1ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = null;
                NewRow["Path 2 Value"] = Config2.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }
            ResultTableList.Add(TableOfConnectionStrings);
            #endregion

            #region TableOfPath1RequiredKeys
            if (Required1 != null)
            {
                DataTable TableOfPath1RequiredKeys = new DataTable("TableOfPath1RequiredKeys");
                TableOfPath1RequiredKeys.Columns.Add("Required Key", typeof(string));
                TableOfPath1RequiredKeys.Columns.Add("Value", typeof(string));

                foreach (string RequiredKey in Required1)
                {
                    DataRow NewRow = TableOfPath1RequiredKeys.NewRow();
                    NewRow["Required Key"] = RequiredKey;
                    NewRow["Value"] = Config1.AppSettings.Settings.AllKeys.Contains(RequiredKey) ? 
                                        Config1.AppSettings.Settings[RequiredKey].Value :
                                        KEY_NOT_FOUND;
                    TableOfPath1RequiredKeys.Rows.Add(NewRow);
                }
                ResultTableList.Add(TableOfPath1RequiredKeys);
            }
            #endregion

            #region TableOfPath2RequiredKeys
            if (Required2 != null)
            {
                DataTable TableOfPath2RequiredKeys = new DataTable("TableOfPath2RequiredKeys");
                TableOfPath2RequiredKeys.Columns.Add("Required Key", typeof(string));
                TableOfPath2RequiredKeys.Columns.Add("Value", typeof(string));

                foreach (string RequiredKey in Required2)
                {
                    DataRow NewRow = TableOfPath2RequiredKeys.NewRow();
                    NewRow["Required Key"] = RequiredKey;
                    NewRow["Value"] = Config2.AppSettings.Settings.AllKeys.Contains(RequiredKey) ?
                                        Config2.AppSettings.Settings[RequiredKey].Value :
                                        KEY_NOT_FOUND;
                    TableOfPath2RequiredKeys.Rows.Add(NewRow);
                }
                ResultTableList.Add(TableOfPath2RequiredKeys);
            }
            #endregion

            ComparisonResults.Tables.AddRange(ResultTableList.ToArray());
        }
    }
}
