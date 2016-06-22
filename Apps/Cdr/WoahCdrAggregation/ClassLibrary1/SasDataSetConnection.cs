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
        //ConnectionString = "Provider=sas.IOMProvider; Data Source =_LOCAL_"

        public enum CustomerEnum { WOAH };

        CustomerEnum Customer;
        public string TableName;
        OleDbConnection Connection;
        OleDbDataReader Reader;
        OleDbCommand SasCommand;


        public void ConnectToSasDataSet()
        {
            switch (Customer)
            {
                case CustomerEnum.WOAH:
                        Connection = new OleDbConnection();
                        Connection.ConnectionString = "Provider=sas.IOMProvider; Data Source =_LOCAL_";
                        var SasRootDirectoryInfo = new DirectoryInfo(@"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files");
                        var MostRecentDirectory = SasRootDirectoryInfo.GetDirectories().OrderByDescending(f => f.Name).First();

                        SasCommand = Connection.CreateCommand();
                        SasCommand.CommandType = CommandType.Text;
                        SasCommand.CommandText = "libname saslib  'K:\\PHI\\0273WOH\\3.005-0273WOH06\\5-Support_files\\" + MostRecentDirectory + "\\035_Staging_Membership\\'";

                    break;
                default:
                    break;
            }

            SasCommand.ExecuteNonQuery();
            SasCommand.CommandType = CommandType.TableDirect;
            SasCommand.CommandText = "saslib." + TableName;
            Reader = SasCommand.ExecuteReader();
            Connection.Open();
        }

        public Boolean Connect(CustomerEnum Customer, string TableName)
        {
            this.Customer = Customer;
            this.TableName = TableName;

            try
            {
                ConnectToSasDataSet();

            } catch(Exception E) { 
                Console.WriteLine(E);
                return false;
            }

            return true;
        }

    }
}
