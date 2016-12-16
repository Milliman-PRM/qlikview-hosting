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
            List<string> EmailSplit = inputValue.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (EmailSplit.Count() != 2)
            {
                errorsList.Add("Valid email addresses must contain two parts seperated by a '@' character.");
                return errorsList;
            }
            
            //check for invalid characters in LOCAL part of email
            string EmailLocalAllowedChars = System.Configuration.ConfigurationManager.AppSettings["EmailLocalAllowedChars"];
            System.Text.RegularExpressions.Regex LocalCharsRegEx = new System.Text.RegularExpressions.Regex("[^" + EmailLocalAllowedChars + "]");
            if (LocalCharsRegEx.IsMatch(EmailSplit[0]) == true)
            {
                errorsList.Add("The part of email address before the '@' character contained invalid characters. It should only contain characters from the set '" + EmailLocalAllowedChars + "'");
                return errorsList;
            }
            
            //check for invalid chars in SERVER part of email
            string EmailServerAllowedChars = System.Configuration.ConfigurationManager.AppSettings["EmailServerAllowedChars"];
            System.Text.RegularExpressions.Regex ServerCharsRegEx = new System.Text.RegularExpressions.Regex("[^" + EmailServerAllowedChars + "]");
            if (ServerCharsRegEx.IsMatch(EmailSplit[1]) == true)
            {
                errorsList.Add("The part of email address after the '@' character contained invalid characters. It should only contain characters from the set '" + EmailServerAllowedChars + "'");
                return errorsList;
            }

            return errorsList;
        }
                
    }
}