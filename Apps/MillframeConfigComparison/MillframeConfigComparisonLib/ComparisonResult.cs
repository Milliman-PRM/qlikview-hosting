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
            TableOfKeysInBothPaths.Columns.Add("Path1 Value", typeof(string));
            TableOfKeysInBothPaths.Columns.Add("Path2 Value", typeof(string));

            var KeysInBothPaths = Config1.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Intersect(
                                    Config2.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInBothPaths)
            {
                DataRow NewRow = TableOfKeysInBothPaths.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path1 Value"] = Config1.ThisWebConfig.AppSettings.Settings[Key].Value;
                NewRow["Path2 Value"] = Config2.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInBothPaths.Rows.Add(NewRow);
            }

            DataTable TableOfKeysInPath1Only = new DataTable("KeysInPath1Only");
            TableOfKeysInPath1Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath1Only.Columns.Add("Path1 Value", typeof(string));
            var KeysInPath1Only = Config1.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Except(
                                    Config2.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath1Only)
            {
                DataRow NewRow = TableOfKeysInPath1Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path1 Value"] = Config1.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInPath1Only.Rows.Add(NewRow);
            }

            DataTable TableOfKeysInPath2Only = new DataTable("KeysInPath2Only");
            TableOfKeysInPath2Only.Columns.Add("Configuration Key", typeof(string));
            TableOfKeysInPath2Only.Columns.Add("Path2 Value", typeof(string));
            var KeysInPath2Only = Config2.ThisWebConfig.AppSettings.Settings.AllKeys.AsQueryable().Except(
                                    Config1.ThisWebConfig.AppSettings.Settings.AllKeys);
            foreach (string Key in KeysInPath2Only)
            {
                DataRow NewRow = TableOfKeysInPath2Only.NewRow();
                NewRow["Configuration Key"] = Key;
                NewRow["Path2 Value"] = Config2.ThisWebConfig.AppSettings.Settings[Key].Value;
                TableOfKeysInPath2Only.Rows.Add(NewRow);
            }

            ComparisonResults.Tables.AddRange(new DataTable[]
                {
                    TableOfKeysInBothPaths,
                    TableOfKeysInPath1Only,
                    TableOfKeysInPath2Only
                });
        }
    }
}
