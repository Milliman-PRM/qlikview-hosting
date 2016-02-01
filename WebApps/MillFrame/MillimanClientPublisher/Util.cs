using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;

namespace ClientPublisher
{
    public class Util
    {
        /// <summary>
        /// The query string variable that indicates the IdentityProvider to ServiceProvider binding.
        /// </summary>
        public const string BindingVarName = "binding";

        /// <summary>
        /// The query string parameter that contains error description for the login failure. 
        /// </summary>
        public const string ErrorVarName = "error";

        public static string GetAbsoluteUrl(Page page, string relativeUrl)
        {
            return new Uri(page.Request.Url, page.ResolveUrl(relativeUrl)).ToString();
        }

        public static Dictionary<string, string> ParseQueryString(String query)
        {
            return Regex.Matches(query, "([^?=&]+)(=([^&]*))?").Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => HttpUtility.UrlDecode(x.Groups[3].Value));
        }
    }
}