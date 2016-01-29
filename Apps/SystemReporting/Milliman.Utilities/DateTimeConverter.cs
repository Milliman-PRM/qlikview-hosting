using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities
{
    public static class DateTimeConverter
    {
        private static TimeZoneInfo defaultTimeZoneInfo = null;

        public static TimeZoneInfo DefaultTimeZoneInfo
        {
            get
            {
                if (defaultTimeZoneInfo == null)
                    defaultTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Constants.DEFAULT_MICROSOFT_TIME_ZONE_ID);

                return defaultTimeZoneInfo;
            }
        }

        public static DateTime FromUTC(DateTime utcDateTime)
        {
            DateTime convertedDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            TimeZoneInfo timezoneInfo = DefaultTimeZoneInfo;
            convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(convertedDateTime, timezoneInfo);
            return convertedDateTime;
        }

        public static DateTime FromUTC(DateTime utcDateTime, TimeZoneInfo toTimeZone)
        {
            DateTime convertedDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(convertedDateTime, toTimeZone);
            return convertedDateTime;
        }

        public static DateTime ToUTC(DateTime dateTimeToConvert, TimeZoneInfo currentTimeZone)
        {
            bool isInvalid = currentTimeZone.IsInvalidTime(dateTimeToConvert);
            if (isInvalid == true)
            {
                dateTimeToConvert = (dateTimeToConvert.AddHours(1));
            }
            if (dateTimeToConvert.Kind == DateTimeKind.Local)
                dateTimeToConvert = DateTime.SpecifyKind(dateTimeToConvert, DateTimeKind.Unspecified);

            DateTime returnUtcDateTime = dateTimeToConvert;

            if (dateTimeToConvert.Kind != DateTimeKind.Utc)
                returnUtcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeToConvert, currentTimeZone);

            return returnUtcDateTime;
        }

        public static DateTime ToUTC(string localTime, string localDate, TimeZoneInfo supportedTimezone)
        {
            var localTimeValue = TimeSpan.Parse(localTime);
            var localDateValue = DateTime.Parse(localDate);
            var combinedDateTime = localDateValue.Date + localTimeValue;

            return DateTimeConverter.ToUTC(combinedDateTime, supportedTimezone);
        }

        public static DateTime ToUTC(TimeSpan localTime, DateTime localDate, TimeZoneInfo supportedTimezone)
        {
            return DateTimeConverter.ToUTC(localDate.Date + localTime, supportedTimezone);
        }

        public static DateTime ToUTC(DateTime dateTimeToConvert)
        {
            return ToUTC(dateTimeToConvert, DefaultTimeZoneInfo);
        }

        public static DateTime ToNewTimeZone(DateTime dateTimeToConvert, TimeZoneInfo originalTZ, TimeZoneInfo newTZ)
        {
            var returnDate = ToUTC(dateTimeToConvert, originalTZ);
            returnDate = FromUTC(dateTimeToConvert, newTZ);

            return returnDate;
        }
    }
}
