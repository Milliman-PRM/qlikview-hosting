using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MillimanCommon
{
    public class RegexUtility
    {
        /// <summary>
        /// Method to check if the input contains any numbers
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool CheckInputForNumbers(string inputString)
        {
            //create pattren for looking up any numbers
            var pattren = @".*?[d].*?";
            var counter = Regex.Matches(inputString, pattren).Count;
            if (counter > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Method to check if input contains any alphabets
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool CheckInputForAlphabets(string inputString)
        {
            //create pattren for looking up any alphabets
            var pattren = @".*?[a-zA-Z].*?";
            var counter = Regex.Matches(inputString, pattren).Count;
            if (counter > 0)
                return true;

            return false;
        }        

        /// <summary>
        /// Function to convert a string into list (Take all the words in the input string and separate them into list.)
        /// </summary>
       public static string[] SplitAllWords(string inputString)
        {
            //
            // Split all word on characters.
            // ... Returns an array of all the words.
            //
            string[] substrings = Regex.Split(inputString, "",RegexOptions.IgnoreCase);
            return substrings;
        }


    }
}
