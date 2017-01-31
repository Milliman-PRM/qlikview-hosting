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
        OneConfiguration Config1;
        OneConfiguration Config2;

        public ComparisonResult(string Path1, string Path2, bool DoWebConfig, bool DoAppConfig)
        {
            Config1 = new OneConfiguration(Path1, DoWebConfig, DoAppConfig);
            Config2 = new OneConfiguration(Path2, DoWebConfig, DoAppConfig);

            DataTable TableOfKeysInBothPaths = new DataTable("KeysInBothPaths");
            TableOfKeysInBothPaths.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInBothPaths.Columns.Add("Path 1 Value", typeof(string));
            TableOfKeysInBothPaths.Columns.Add("Path 2 Value", typeof(string));

            var KeysInBothPaths = Config1.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Intersect(
                                    Config2.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInBothPaths)
            {
                DataRow NewRow = TableOfKeysInBothPaths.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 1 Value"] = Config1.ThisWebConfig.AppSettings.Settings[Key].Value;
                NewRow["Path 2 Value"] = Config2.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInBothPaths.Rows.Add(NewRow);
            }

            DataTable TableOfKeysInPath1Only = new DataTable("KeysInPath1Only");
            TableOfKeysInPath1Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath1Only.Columns.Add("Path 1 Value", typeof(string));
            var KeysInPath1Only = Config1.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Except(
                                    Config2.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath1Only)
            {
                DataRow NewRow = TableOfKeysInPath1Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 1 Value"] = Config1.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInPath1Only.Rows.Add(NewRow);
            }

            DataTable TableOfKeysInPath2Only = new DataTable("KeysInPath2Only");
            TableOfKeysInPath2Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath2Only.Columns.Add("Path 2 Value", typeof(string));
            var KeysInPath2Only = Config2.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Except(
                                    Config1.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath2Only)
            {
                DataRow NewRow = TableOfKeysInPath2Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path 2 Value"] = Config2.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInPath2Only.Rows.Add(NewRow);
            }

            DataTable TableOfConnectionStrings = new DataTable("ConnectionStrings");
            TableOfConnectionStrings.Columns.Add("Connection String Name", typeof(string));
            TableOfConnectionStrings.Columns.Add("Path 1 Value", typeof(string));
            TableOfConnectionStrings.Columns.Add("Path 2 Value", typeof(string));
            HashSet<string> Path1ConnectionStringNames = new HashSet<string>();
            HashSet<string> Path2ConnectionStringNames = new HashSet<string>();
            foreach (ConnectionStringSettings ConnectionSettings in Config1.ThisWebConfig.ConnectionStrings.ConnectionStrings)
            {
                Path1ConnectionStringNames.Add(ConnectionSettings.Name);
            }
            foreach (ConnectionStringSettings ConnectionSettings in Config2.ThisWebConfig.ConnectionStrings.ConnectionStrings)
            {
                Path2ConnectionStringNames.Add(ConnectionSettings.Name);
            }

            // Connectionstring in both paths
            foreach (string ConnectionName in Path1ConnectionStringNames.Intersect(Path2ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = Config1.ThisWebConfig.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                NewRow["Path 2 Value"] = Config2.ThisWebConfig.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }
            // Connectionstring in Path1 only
            foreach (string ConnectionName in Path1ConnectionStringNames.Except(Path2ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = Config1.ThisWebConfig.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                NewRow["Path 2 Value"] = null;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }
            // Connectionstring in Path2 only
            foreach (string ConnectionName in Path2ConnectionStringNames.Except(Path1ConnectionStringNames))
            {
                DataRow NewRow = TableOfConnectionStrings.NewRow();
                NewRow["Connection String Name"] = ConnectionName;
                NewRow["Path 1 Value"] = null;
                NewRow["Path 2 Value"] = Config2.ThisWebConfig.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
                TableOfConnectionStrings.Rows.Add(NewRow);
            }

            ComparisonResults.Tables.AddRange(new DataTable[]
                {
                    TableOfKeysInBothPaths,
                    TableOfKeysInPath1Only,
                    TableOfKeysInPath2Only,
                    TableOfConnectionStrings
                });
        }
    }
}
