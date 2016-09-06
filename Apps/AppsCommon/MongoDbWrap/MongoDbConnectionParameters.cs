using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbWrap
{
    public class MongoDbConnectionParameters
    {
        public MongoDbConnectionParameters(string TheSource, string SectionName)
        {
            ReadFromSource(TheSource, SectionName);
        }

        public MongoDbConnectionParameters(string TheSource)
        {
            ReadFromSource(TheSource, @"MongoCredentials");
        }

        public string _UserDomain = "admin";
        public string _User = null;
        public string _Password = null;
        public string _Host = null;
        public int _Port = 0;  // zero means use the default 27017
        public string _Db = null;

        /// <summary>
        /// Populates member variables by reading settings from an .ini file.  
        /// The specified section is expected to contain settings: domain, user, password, server, database
        /// </summary>
        /// <param name="TheSource">Full path to the .ini file to be read</param>
        /// <param name="SectionName">Section name in the .ini file from which to read settings </param>
        /// <exception cref="Exception">Can rethrow any exception thrown by File.ReadAllText(string), usually related to the ini file not being readable.</exception>
        /// <returns></returns>
        private bool ReadFromSource(string TheSource, string SectionName)
        {
            // This will throw if the file is not found or can not be read.  
            IniProcessor IniProc = new IniProcessor(TheSource);

            _UserDomain = IniProc.GetValue("domain", SectionName);
            _User = IniProc.GetValue("user", SectionName);
            _Password = IniProc.GetValue("password", SectionName);
            _Host = IniProc.GetValue("server", SectionName);
            _Db = IniProc.GetValue("database", SectionName);

            if (_User == "" || _Password == "" || _Host == "" || _Db == "")  // _UserDomain has a default
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return
            " _UserDomain: " + _UserDomain +
            " _User: " + _User +
            " _Password: " + _Password +
            " _Host: " + _Host +
            " _Port: " + _Port.ToString() +
            " _Db: " + _Db;
    }
}
}
