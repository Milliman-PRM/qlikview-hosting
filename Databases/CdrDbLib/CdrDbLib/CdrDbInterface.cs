using System;
using System.Diagnostics;
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
            try
            {
                Context = new CdrDataContext(ConnectionString);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception while constructing CdrDataContext, Exception content:\n" + e.Message + "\n" + e.StackTrace);
            }
        }

       ~CdrDbInterface()
        {
            Context.Dispose();
            Context = null;
        }

        public Organization EnsureOrganizationRecord(String OrganizationName)
        {
            List<Organization> MatchingOrganizations = Context.Organizations.Where(x => x.OrgName == OrganizationName).ToList();

            if (MatchingOrganizations.Count == 1)  // normal case, the entity exists
            {
                return MatchingOrganizations.First();
            }
            else if (MatchingOrganizations.Count == 0)  // need to create the entity, probably the first time through
            {
                Organization NewOrganization = new Organization
                {
                    OrgName = OrganizationName,
                    EmrIdentifier = ""  // TODO get this value from somewhere
                };

                Context.Organizations.InsertOnSubmit(NewOrganization);
                Context.SubmitChanges();

                return NewOrganization;
            }
            else  // something is wrong, this should never happen
            {
                return null;
            }

        }

        public DataFeed EnsureFeedRecord(String FeedName, Organization LinkedOrganization)
        {
            List<DataFeed> MatchingFeeds = Context.DataFeeds.Where(x => x.FeedName == FeedName && x.Organizationdbid == LinkedOrganization.dbid).ToList();

            if (MatchingFeeds.Count == 1)  // normal case, the feed entity exists
            {
                return MatchingFeeds.First();
            }
            else if (MatchingFeeds.Count == 0)  // need to create the feed entity, probably the first time through
            {
                DataFeed NewFeed = new DataFeed
                {
                    Organizationdbid = LinkedOrganization.dbid,
                    FeedName = FeedName
                };

                Context.DataFeeds.InsertOnSubmit(NewFeed);
                Context.SubmitChanges();

                return NewFeed;
            }
            else  // something is wrong, this should never happen
            {
                return null;
            }
        }
    }
}
