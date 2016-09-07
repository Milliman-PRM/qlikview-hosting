using System;
using System.Linq;

namespace MongoDbWrap
{
    public class MongoDbConnectionParameters
    {
        public string _UserDomain = "admin";
        public string _User = null;
        public string _Password = null;
        public string _Host = null;
        public int _Port = 0;  // zero means use the default 27017
        public string _Db = null;

        public MongoDbConnectionParameters()
        {}

        /// <summary>
        /// Populates member variables by reading settings from an .ini file.  
        /// The specified section is expected to contain settings: domain, user, password, server, database
        /// </summary>
        /// <param name="TheSource">Full path to the .ini file to be read</param>
        /// <param name="SectionName">Section name in the .ini file from which to read settings </param>
        /// <exception cref="Exception">Can rethrow any exception thrown by File.ReadAllText(string), usually related to the ini file not being readable.</exception>
        /// <returns></returns>
        public bool ReadFromIni(string TheSource, string SectionName)
        {
            // This will throw if the file is not found or can not be read.  
            IniProcessor IniProc = new IniProcessor(TheSource);

            if (!IniProc.GetSections().Contains(SectionName))
            {
                return false;
            }

            _UserDomain = IniProc.GetValue("domain", SectionName);
            _User = IniProc.GetValue("user", SectionName);
            _Password = IniProc.GetValue("password", SectionName);
            _Host = IniProc.GetValue("server", SectionName);
            _Db = IniProc.GetValue("database", SectionName);

            // optional parameter
            String Port = IniProc.GetValue("port", SectionName);
            int.TryParse(Port, out _Port);

            return IsValid();
        }

        public bool ReadFromEnvironment(String DbNameBase = "")
        {
            _User = Environment.GetEnvironmentVariable("ephi_username");
            _Password = Environment.GetEnvironmentVariable("ephi_password");
            _UserDomain = Environment.GetEnvironmentVariable("Mongo.UserDomain");
            _Host = Environment.GetEnvironmentVariable("Mongo.Host");
            _Db = DbNameBase + Environment.GetEnvironmentVariable("Mongo.DbSuffix");

            // optional parameter
            String Port = Environment.GetEnvironmentVariable("Mongo.Port");
            int.TryParse(Port, out _Port);

            return IsValid();
        }

        public bool IsValid()
        {
            return !(String.IsNullOrEmpty(_UserDomain) ||
                     String.IsNullOrEmpty(_User) ||
                     String.IsNullOrEmpty(_Password) ||
                     String.IsNullOrEmpty(_Host) ||
                     String.IsNullOrEmpty(_Db));
        }

        public override string ToString()
        {
            return
                " _User=" + _User +
                " _Password=" + _Password +
                " _UserDomain=" + _UserDomain +
                " _Host=" + _Host +
                " _Port=" + _Port.ToString() +
                " _Db=" + _Db;
        }
    }
}
