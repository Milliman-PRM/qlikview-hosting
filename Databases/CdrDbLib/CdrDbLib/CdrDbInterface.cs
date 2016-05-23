using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CdrContext;

namespace CdrDbLib
{
    public enum ConnectionArgumentType
    {
        ConnectionString,
        ConnectionStringName
    }

    /// <summary>
    /// Class supports generated context entities plus additional database features like stored procedures
    /// </summary>
    public class CdrDbInterface
    {
        public CdrDataContext Context;

        public CdrDbInterface(String ConnectionArgument, ConnectionArgumentType ArgType)
        {
            String ConnectionString = null;

            switch (ArgType)
            {
                case ConnectionArgumentType.ConnectionString:
                    ConnectionString = ConnectionArgument;
                    break;

                case ConnectionArgumentType.ConnectionStringName:
                    if (ConfigurationManager.ConnectionStrings[ConnectionArgument] == null)
                    {
                        throw new ArgumentOutOfRangeException("CdrDbInterface ctor: Failed to find configured connection string named: " + ConnectionArgument);
                    }

                    ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionArgument].ConnectionString;
                    break;

            }
            Context = new CdrDataContext(ConnectionString);
        }

        public DataFeed EnsureFeed(String FeedName)
        {
            List<DataFeed> MatchingFeeds = Context.DataFeeds.Where(x => x.FeedName == FeedName).ToList();

            if (MatchingFeeds.Count == 1)
            {
                return MatchingFeeds.First();
            }
            else if (MatchingFeeds.Count > 1)
            {
                // something is wrong
                return null;
            }
            else
            {
                Organization BayClinicOrg = Context.Organizations.Where(x => x.OrgName == "Bay Clinic").FirstOrDefault();
                DataFeed NewFeed = new DataFeed
                {
                    Organizationdbid = BayClinicOrg.dbid,
                    FeedName = FeedName
                };
                Context.DataFeeds.InsertOnSubmit(NewFeed);
                Context.SubmitChanges();

                return NewFeed;
            }
        }
    }
}
