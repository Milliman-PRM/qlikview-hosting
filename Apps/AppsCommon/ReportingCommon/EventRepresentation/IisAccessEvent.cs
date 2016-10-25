using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportingCommon
{
    public class IisAccessEvent : UserEventBase
    {
        public enum IisEventType
        {
            IIS_LOGIN,
            IIS_LAST_ACCESS,
            IIS_UNKNOWN_EVENT
        }

        public IisEventType EventType = IisEventType.IIS_UNKNOWN_EVENT;

        /// <summary>
        /// Constructor, infers member data values from a provided instance of IisLogFileEntry. 
        /// </summary>
        /// <param name="Entry">An IisLogFileEntry instance from which properties of this instance should be inferred</param>
        /// <param name="QvDocRoot">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        public IisAccessEvent(IisLogFileEntry Entry, string QvDocRoot)
        {
            this.EventType = GetEventType(Entry);
            this.User = Entry.UserName;
            this.Group = DecodeUriQuery(Entry.UriQuery, QvDocRoot);
            this.Browser = GetBrowserFromAgentString(Entry.UserAgent);
            this.TimeStamp = Entry.TimeStamp;
        }

        /// <summary>
        /// Evaluates the event type, inferred from the provided instance of IisLogFileEntry
        /// </summary>
        /// <param name="Entry"></param>
        /// <returns></returns>
        public static IisEventType GetEventType(IisLogFileEntry Entry)
        {
            if (Entry.IsLogin())
            {
                return IisEventType.IIS_LOGIN;
            }
            else if (!string.IsNullOrEmpty(Entry.UserName)){
                return IisEventType.IIS_LAST_ACCESS;
            }
            else
            {
                return IisEventType.IIS_UNKNOWN_EVENT;
            }
        }

        /// <summary>
        ///     Decodes the uriQuery
        ///     HEX to STRING conversion
        /// </summary>
        /// <param name="uriQuery">uri-query</param>
        /// <param name="QvDocRoot">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        /// <returns>decoded string</returns>
        public static string DecodeUriQuery(string uriQuery, string QvDocRoot)
        {
            string groupName = null;
            string reduced;
            string query;
            try
            {
                if (!string.IsNullOrWhiteSpace(uriQuery) && uriQuery != "-")
                {
                    Regex regex = new Regex(@"\w+\W");
                    reduced = regex.Replace(uriQuery, string.Empty);
                    regex = new Regex(@"^0123456789ABCDEFabcdef"); // regex matches non-hexadecimal character set

                    if (regex.IsMatch(reduced))  // hopefully this prevents exceptions during ConvertHexToString(..)
                    {
                        // query is not entirely hex characters
                        return "";
                    }
                    query = ConvertHexToString(reduced);
                    query = query.ToLower().Replace(QvDocRoot, "");

                    string[] splitGroupNames = query.Split('\\');
                    splitGroupNames = splitGroupNames.Take(splitGroupNames.Count() - 1).ToArray();
                    groupName = string.Join("_", splitGroupNames);
                    groupName = groupName.ToUpper();
                    groupName = groupName.Replace(@"_REDUCEDCACHEDQVWS", "");
                }
            }
            catch (Exception)
            {
                //int i = 1;  // for convenient breakpoint
            }

            return groupName;
        }

        // TODO: remove when integrated to main code base, this comes from MillimanCommon.Utilities
        public static string ConvertHexToString(string HexValue)
        {
            string StrValue = "";
            while (HexValue.Length > 0)
            {
                StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                HexValue = HexValue.Substring(2, HexValue.Length - 2);
            }
            return StrValue;
        }

        /// <summary>
        ///     Splits the user-agent, check if it IE, Firefox, Chrome or Other
        /// </summary>
        /// <param name="UserAgent">User-agent string as read from IIS log file</param>
        /// <returns>string indicating browser type and its version </returns>
        public static string GetBrowserFromAgentString(string UserAgent)
        {
            string BrowserType = null;
            var tokens = UserAgent.Split('+');

            try
            {
                string BrowserVersion = "";

                if (UserAgent.ToUpper().Contains("MSIE"))
                {
                    int MsieIndex = Array.IndexOf(tokens, tokens.First(a => a.StartsWith("MSIE")));
                    if (tokens.Length > (MsieIndex+1) && 
                        !string.IsNullOrWhiteSpace(tokens[MsieIndex + 1]) && 
                        Char.IsDigit(tokens[MsieIndex + 1][0]))
                    {
                        BrowserVersion = TrimEndNonDigit(tokens[MsieIndex + 1]);
                    }
                    BrowserType = "MSIE " + BrowserVersion;
                }
                else if (UserAgent.ToUpper().Contains("FIREFOX"))
                {
                    int FirefoxIndex = Array.IndexOf(tokens, tokens.First(a => a.StartsWith("Firefox")));
                    string[] FirefoxTokens = tokens[FirefoxIndex].Split('/');
                    if (FirefoxTokens.Length == 2 && 
                        !string.IsNullOrWhiteSpace(FirefoxTokens[1]) && 
                        Char.IsDigit(FirefoxTokens[1][0]))
                    {
                        BrowserVersion = FirefoxTokens[1];
                    }
                    BrowserType = "Firefox " + BrowserVersion;
                }
                else if (UserAgent.ToUpper().Contains("CHROME"))
                {
                    int ChromeIndex = Array.IndexOf(tokens, tokens.First(a => a.StartsWith("Chrome")));
                    string[] ChromeTokens = tokens[ChromeIndex].Split('/');
                    if (ChromeTokens.Length == 2 &&
                        !string.IsNullOrWhiteSpace(ChromeTokens[1]) &&
                        Char.IsDigit(ChromeTokens[1][0]))
                    {
                        BrowserVersion = ChromeTokens[1];
                    }
                    BrowserType = "Chrome " + BrowserVersion;
                }
                else if (UserAgent.ToUpper().Contains("SAFARI") && !UserAgent.ToUpper().Contains("CHROME") && !UserAgent.ToUpper().Contains("ANDROID"))
                {
                    int SafariIndex = Array.IndexOf(tokens, tokens.First(a => a.StartsWith("Safari")));
                    string[] SafariTokens = tokens[SafariIndex].Split('/');
                    if (SafariTokens.Length == 2 &&
                        !string.IsNullOrWhiteSpace(SafariTokens[1]) &&
                        Char.IsDigit(SafariTokens[1][0]))
                    {
                        BrowserVersion = SafariTokens[1];
                    }
                    BrowserType = "Safari " + BrowserVersion;
                }
                else if (UserAgent.ToUpper().Contains("ANDROID") && !UserAgent.ToUpper().Contains("CHROME")   )
                {
                    if (UserAgent.Contains("Version"))
                    {
                        int BrowserIndex = Array.IndexOf(tokens, tokens.First(a => a.StartsWith("Version")));

                        string[] AndroidTokens = tokens[BrowserIndex].Split('/');
                        if (AndroidTokens.Length == 2 &&
                            !string.IsNullOrWhiteSpace(AndroidTokens[1]) &&
                            Char.IsDigit(AndroidTokens[1][0]))
                        {
                            BrowserVersion = AndroidTokens[1];
                        }
                    }                    

                    BrowserType = "Android " + BrowserVersion;
                }
                else if (UserAgent.ToUpper().Contains("RV:"))
                {
                    string MsRevision = tokens.First(a => a.StartsWith("rv:"));
                    MsRevision = MsRevision.Substring(3);
                    if (!string.IsNullOrWhiteSpace(MsRevision) &&
                        Char.IsDigit(MsRevision[0]))
                    {
                        BrowserVersion = TrimEndNonDigit(MsRevision);
                    }
                    BrowserType = "MSIE" + " " + BrowserVersion;
                }
                else
                {
                    BrowserType = "";
                }
            }
            catch (Exception e)
            {
                return "Exception while parsing client agent: " + e.Message;
            }

            return BrowserType;
        }

        public static string TrimEndNonDigit(string Value)
        {
            char FirstNonDigit = Value.ToArray().FirstOrDefault(a => !Char.IsDigit(a) && a != '.');
            int i = Value.IndexOf(FirstNonDigit); // ok if FirstNonDigit is null
            if (i >= 0)
            {
                Value = Value.Substring(0, i);
            }

            return Value;
        }
    }
}
