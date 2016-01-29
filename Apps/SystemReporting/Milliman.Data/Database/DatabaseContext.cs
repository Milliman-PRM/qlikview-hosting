using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Data.Database
{
    public class DatabaseContext : DbContext
    {
        static String ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
        
        public DatabaseContext() : base(new NpgsqlConnection(ConnectionString), true)
        {
            //Database.SetInitializer<DatabaseContext>(new ApplicationDbContext());
        }
    }
}
