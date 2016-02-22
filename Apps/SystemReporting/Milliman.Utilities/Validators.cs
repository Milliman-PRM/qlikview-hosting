using SystemReporting.Utilities.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C = SystemReporting.Utilities.Constants;


namespace SystemReporting.Utilities
{
    public static class Validators
    {
        #region variables and properties
        static string[] dateTimeFormats = {"M/d/yyyy", "MM/dd/yyyy", "M/d/yyyy", "M/dd/yyyy", "M/d/yy", "MM/dd/yy",
                                        "MMddyy","MMddyyyy","Mddyy","Mddyyyy","Mdyy","MMdyyyy",
                                        "yy/MM/dd","yyyy/MM/dd","yy/M/dd","yyyy/M/dd","yyyyMMdd","yyMdd","yyMd","yyyyMd", "yyMMdd"};
        #endregion
        #region Validate Rules

        public static bool CheckValidate(string type, int length, string text, int min, double max, bool compare)
        {
            bool isValid = false;
            Regex regex;
            switch (type)
            {
                case C.ALPHA_CHAR:
                    regex = new Regex(@"^[a-zA-Z]*$");
                    break;
                case C.ALPHA_NUMERIC:
                    regex = new Regex(@"^[a-zA-Z0-9]*$");
                    break;
                case C.NUMERIC:
                    regex = new Regex(@"^\d+$");
                    break;
                case C.DECIMAL:
                    regex = new Regex(@"^(?!0\d)\d*(\.\d{1,5})?$");
                    break;
                case C.ALPHA_NUMERIC_SPECIAL_CHAR:
                    regex = new Regex(@"^[A-Za-z0-9-\\\/\s]*$");
                    break;
                case C.ALPHA_NUMERICS_WITH_SPACES:
                    regex = new Regex(@"^[a-zA-Z0-9-\s]*$");
                    break;
                case C.ALPHA_NUMERICS_WITH_ALL_SPECIAL_CHARS:
                    regex = new Regex(@"^[A-Za-z0-9-\\\/\.\@\#\!\-\&\%\(\)\+\-\=\*\$\s_]*$");
                    break;
                case C.DATE_TIME_DECIMAL:
                    regex = new Regex(@"^-?(([1-9]\d*)|0)(.0*[1-9](0*[1-9])*)?$");
                    break;
                default:
                    regex = new Regex(@"^[a-zA-Z0-9]*$");
                    break;
            }
            if (regex.IsMatch(text))
            {
                isValid = true;
                if (length != 0)
                    isValid = ValidateLength(length, text);
                if (compare)
                {
                    double number = Convert.ToDouble(text);
                    if (number <= min && number >= max)
                        isValid = false;
                }
            }
            return isValid;
        }
        private static bool ValidateLength(int length, string text)
        {
            bool isValidLength = false;
            if (text.Length <= length)
                isValidLength = true;
            return isValidLength;
        }
        public static bool ValidateDate(string date)
        {
            try
            {
                // for US, alter to suit if splitting on hyphen, comma, etc.
                string[] dateParts = date.Split('/');

                // create new date from the parts; if this does not fail
                // the method will return true and the date is valid
                DateTime dateTime = new
                    DateTime(Convert.ToInt32(dateParts[2]),
                    Convert.ToInt32(dateParts[0]),
                    Convert.ToInt32(dateParts[1]));

                return true;
            }
            catch (Exception ex)
            {
                SystemExceptionHandling.WriteException(ex);
                return false;
            }
        }
        public static bool CheckOrderDate(string value, string[] formats, int fromDays, int toDays, bool compare, bool isDateTime, bool isASN = false)
        {
            bool isValidDate = false;
            DateTime dateValue;

            if (DateTime.TryParseExact(value, formats, new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                if (dateValue != null)
                {
                    isValidDate = true;
                    DateTime fromDate = DateTime.Now.AddDays(fromDays);
                    DateTime toDate = DateTime.Now.AddDays(toDays);
                    if (!isDateTime)
                    {
                        fromDate = DateTime.Parse(fromDate.ToShortDateString());
                        toDate = DateTime.Parse(toDate.ToShortDateString());
                    }
                    if (compare)
                    {
                        if (dateValue <= fromDate && dateValue >= toDate)
                            isValidDate = false;
                    }
                    if (isASN)
                    {
                        if ((toDate.Date <= dateValue.Date && dateValue.Date <= fromDate.Date))
                            isValidDate = true;
                        else
                            isValidDate = false;
                    }

                }
            }
            return isValidDate;
        }
        public static bool CheckDateTime(string value, string[] formats)
        {
            bool isValidDate = false;
            DateTime dateValue;

            if (DateTime.TryParseExact(value, formats, new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                if (dateValue != null)
                    isValidDate = true;
            }
            return isValidDate;
        }

        public static string Truncate(string value, int length)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > length)
                return value = value.Remove(length);

            return value;
        }

        public static string CheckDate(string value)
        {
            DateTime dateValue;

            if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                value = dateValue.ToString("MM/dd/yyyy");

            return value;
        }
        public static bool IsValidDate(string value)
        {
            bool validDate = false;
            DateTime dateValue;

            if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                validDate = true;

            return validDate;
        }

        public static bool IsValidTime(string value)
        {
            bool validTime = false;
            DateTime dateValue;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Length == 4)
                    DateTime.TryParse(value.Substring(0, 2) + ":" + value.Substring(2, 2), out dateValue);
                else if (value.Contains(":"))
                    DateTime.TryParse(value, out dateValue);

                validTime = true;
            }

            return validTime;
        }

        public static DateTime ConvertToDate(string value)
        {
            DateTime date = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(value))
                DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            return date;
        }

        public static DateTime ConvertToTime(string value)
        {
            DateTime time = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(value) && value.Length == 4)
                DateTime.TryParse(value.Substring(0, 2) + ":" + value.Substring(2, 2), out time);
            else if (value.Contains(":"))
                DateTime.TryParse(value, out time);

            return time;
        }

        #endregion
    }
}
