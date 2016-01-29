using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities
{
    public static class Constants
    {
        public const string DEFAULT_MICROSOFT_TIME_ZONE_ID = "Eastern Standard Time";

        #region DB Namespace

        public const string DB_POSTGRESQL_NAMESPACE = "public";
        public const string DB_SQL_NAMESPACE = "dbo";

        #endregion

        #region Areas, Controllers, Actions, Views, Defaults 

        #endregion

        #region Time Zones

        public const string EASTERN_TZ = "Eastern Standard Time";
        public const string MOUNTAIN_TZ = "Mountain Standard Time";
        public const string GREENWICH_MERIDIAN_ABBREVIATION = "GM";
        public const string UNIVERSAL_TIME_ABBREVIATION = "UT";
        public const string MOUNTAIN_TZ_ABBREVIATION = "MT";
        public const string ATLANTIC_TZ_ABBREVIATION = "AST";
        public const string ATLANTIC_TZ_EDIABBREVIATION = "TT";

        #endregion

        #region Validators

        public const string ALPHA_NUMERIC = "AlphaNumeric";
        public const string ALPHA_NUMERICS_WITH_SPACES = "AlphaNumericWithSpace";
        public const string ALPHA_CHAR = "AlaphaChar";
        public const string NUMERIC = "Numeric";
        public const string DECIMAL = "Decimal";
        public const string ALPHA_NUMERIC_SPECIAL_CHAR = "AlphaNumericSpecialChar";
        public const string ALPHA_NUMERICS_WITH_ALL_SPECIAL_CHARS = "AlphaNumericWithAllSpecialChars";
        public const string DATE_TIME_DECIMAL = "DateTimeDecimal";

        #endregion


    }
}
