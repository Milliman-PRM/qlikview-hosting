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
        #region UserAdmin
        #region UserName Enrty Validation

        public static List<string> ValidateUserNameInput(string inputValue)
        {
            var errorsList = new List<string>();
            if (!string.IsNullOrEmpty(inputValue))
            {
                errorsList = CheckUserNameInput(inputValue);
            }

            return errorsList;

        }

        private static List<string> CheckUserNameInput(string inputValue)
        {
            var errorsList = new List<string>();
            var errorMsg = string.Empty;
            //get the occurance of @ - if there is no @ then ignore
            if (inputValue.IndexOf('@') > -1)
            {
                //do nothing, this condition is true
            }
            else
            {
                errorMsg = "String does not contain @ sign.";
                errorsList.Add(errorMsg);
            }

            //check how many '@' exist
            var countofAtSign = inputValue.Count(x => x == '@');
            if (countofAtSign > 1)
            {
                errorMsg = "String contains more than one @ sign.";
                errorsList.Add(errorMsg);
            }

            if (errorsList.Count > 0)
            {
                return errorsList;
            }

            //first part of email before @ 
            var localString = inputValue.Split('@')[0];

            //there is empty string before @
            if (localString.Length == 0)
            {
                errorMsg = "No characters before @ sign.";
                errorsList.Add(errorMsg);
            }

            //second part of email
            var serverString = inputValue.Split('@')[1];

            //serverString must have few chars
            if (serverString.Length == 0)
            {
                errorMsg = "No characters after @ sign.";
                errorsList.Add(errorMsg);
            }

            //chcek for number
            var HasNumbers = RegexUtility.CheckInputForNumbers(inputValue);
            //check for alphabets
            var HasAlphabets = RegexUtility.CheckInputForAlphabets(inputValue);
            //check to see at least one character exist or one number is in string. either 1 number or 1 alphabet must exist
            if (!HasNumbers && !HasAlphabets)
            {
                errorMsg = "There must be at least few characters before and after @ sign.";
                errorsList.Add(errorMsg);
            }

            if (errorsList.Count>0)
            {
                return errorsList;
            }

            var hasSpecialCharacters = StringProcessingUtility.StringProcessing.CheckStringForAnySpecailChars(inputValue, SpecialChars);

            //check to see if there are any specail chars in string then check if those special chars are allowed
            if (hasSpecialCharacters) 
            {
                //****************** Local String Check **********************************/
                var localStringHasSpecialChars = StringProcessingUtility.StringProcessing.CheckStringForAnySpecailChars(localString,SpecialCharsInLocalPart);
                var nonMatchingCharsLocal = new List<string>();
                var nonMatchingCharsServer = new List<string>();
                if (localStringHasSpecialChars)
                {
                    nonMatchingCharsLocal = StringProcessingUtility.StringProcessing.CheckListforAllowedChars(RegexUtility.SplitAllWords(localString.Trim()).ToList(), string.Join("",SpecialCharsInLocalPart));
                }
                //****************** Server String Check **********************************/
                var ServerStringHasSpecialChars = StringProcessingUtility.StringProcessing.CheckStringForAnySpecailChars(serverString,SpecialCharsInServerPart);
                if (localStringHasSpecialChars)
                {
                    nonMatchingCharsServer = StringProcessingUtility.StringProcessing.CheckListforAllowedChars(RegexUtility.SplitAllWords(serverString.Trim()).ToList(), string.Join("", SpecialCharsInServerPart));
                }

                if (nonMatchingCharsLocal.Count > 0 || nonMatchingCharsServer.Count>0)
                {
                    var combinedList = nonMatchingCharsLocal.Concat(nonMatchingCharsServer);
                    errorsList.Add("There are few unsupported specail characters in the string like " + (String.Join(",", combinedList)) + "");
                }
            }
            return errorsList;

        }

        private static readonly char[] SpecialChars = GetAllSpecialChars().ToCharArray();
        private static readonly char[] SpecialCharsInServerPart = GetAllowedCharsServerStringUserName().ToCharArray();
        private static readonly char[] SpecialCharsInLocalPart = GetAllowedCharsInLocalStringUserName().ToCharArray();
              
        #endregion
        
        #region Config Keys Methods        
        private static string GetAllSpecialChars()
        {
            //if there are values for allwoed character then display it
            var AllSpecialChars = ConfigurationManager.AppSettings["AllSpecialChars"].ToString();
            var BadCharactersInFriendlyNameDoubleQuote = ConfigurationManager.AppSettings["BadCharactersInFriendlyNameDoubleQuote"].ToString();
            //remove comma from the characters than at the end add double quotes
            var allChars = (AllSpecialChars) + (BadCharactersInFriendlyNameDoubleQuote);
            if (!string.IsNullOrEmpty(allChars))
                return allChars;

            return string.Empty;
        }

        private static string GetAllowedCharsInLocalStringUserName()
        {
            //if there are values for allwoed character then display it
            var AllowedCharsInLocalStringUserName = ConfigurationManager.AppSettings["AllowedCharsLocalPartUserName"].ToString();
            if (!string.IsNullOrEmpty(AllowedCharsInLocalStringUserName))
                return AllowedCharsInLocalStringUserName;

            return string.Empty;
        }

        private static string GetAllowedCharsServerStringUserName()
        {
            //if there are values for allwoed character then display it
            var AllowedCharsServerStringUserName = ConfigurationManager.AppSettings["AllowedCharsServerPartUserName"].ToString();
            if (!string.IsNullOrEmpty(AllowedCharsServerStringUserName))
                return AllowedCharsServerStringUserName;

            return string.Empty;
        }

        #endregion
        #endregion
    }
}