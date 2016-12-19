using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MillimanCommon
{
    public class MillimanHelper
    {
        public static List<string> ValidateUserNameInput(string inputValue)
        {
            var errorsList = new List<string>();

            //valid email address can only contain 1 AT sign
            if (inputValue.Count(element => element == '@') != 1)
            {
                errorsList.Add("Valid email addresses must contain a single '@' character.");
                return errorsList;
            }
            //email address must fit the form LOCAL@SERVER format
            var EmailSplit = inputValue.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (EmailSplit.Count() != 2)
            {
                errorsList.Add("Valid email addresses must contain two parts seperated by a '@' character.");
                return errorsList;
            }

            //check for invalid characters in LOCAL part of email
            var EmailLocalAllowedChars = ConfigurationManager.AppSettings["EmailLocalAllowedChars"];
            //filter out the non matching chars.
            var filter =
                (from w in EmailSplit[0]
                 select new {
                     Test=w,
                     Count = w.ToString().ToLower().Count(c => EmailLocalAllowedChars.Contains(c))
                 }).Where(item => item.Count == 0);//find every char that has 0 match

            //if there are non-matching chars then invalidate email
            if (filter.Count()> 0)
            {
                errorsList.Add("The part of email address before the '@' character contained invalid characters. It should only contain characters from the set '" + EmailLocalAllowedChars + "'");
                return errorsList;
            }

            var EmailServerAllowedChars = ConfigurationManager.AppSettings["EmailServerAllowedChars"];
            filter =
                (from w in EmailSplit[1]
                 select new
                 {
                     Test = w,
                     Count = w.ToString().ToLower().Count(c => EmailServerAllowedChars.Contains(c))
                 }).Where(item => item.Count == 0);//find every char that has 0 match

            if (filter.Count() > 0)
            {
                errorsList.Add("The part of email address after the '@' character contained invalid characters. It should only contain characters from the set '" + EmailServerAllowedChars + "'");
                return errorsList;
            }

            return errorsList;
        }                      
    }
}