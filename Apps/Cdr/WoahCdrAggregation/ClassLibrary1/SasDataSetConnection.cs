using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasDataSetLib
{
    public class SasDataSetConnection
    {
        string ConnectionString = "Provider=sas.IOMProvider; Data Source =_LOCAL_";
        string Directory = @"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files";
        string libname = "saslib";
        string TableName = "member";

        public OleDbDataReader makeConnection()
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = ConnectionString;
            cn.Open();
            Console.WriteLine("SAS Server Version " + cn.ServerVersion);


            var getDirectory = new DirectoryInfo(Directory);
            var mostRecentFile = getDirectory.GetDirectories().OrderByDescending(f => f.Name).First();

            OleDbCommand sascmd = cn.CreateCommand();
            sascmd.CommandType = CommandType.Text;
            sascmd.CommandText = "libname " + libname + "  'K:\\PHI\\0273WOH\\3.005-0273WOH06\\5-Support_files\\" + mostRecentFile + "\\035_Staging_Membership\\'";
            sascmd.ExecuteNonQuery();


            sascmd.CommandType = CommandType.TableDirect;
            sascmd.CommandText = libname + "." + TableName;


            OleDbDataReader reader = sascmd.ExecuteReader();

            return reader;

        }
    }
}
