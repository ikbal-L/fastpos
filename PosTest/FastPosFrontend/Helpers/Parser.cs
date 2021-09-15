using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class Parser
    {
        private static Parser _instance ;

        public static readonly string DECIMAL_REGEX = @"((\+|\-)?\d+\.?\d*)|(\d*\.\d+)";

        public static readonly string INTEGER_REGEX = @"((\+|\-)?\d+)";

        public static readonly string TIMESPAN_HH_MM_REGEX = @"(\d?\d:\d{2})";

        public static readonly string TIMESPAN_HH_MM_SS_REGEX = @"(\d?\d:\d{2}:\d{2})";

        private Parser()
        {

        }

        public static Parser Instance =>_instance?? new Parser();

        public (decimal? min, decimal? max, decimal? exact) ParseDecimalValues(string text)
        {
            var exactValue = Regex.Match(text, @$"={DECIMAL_REGEX}").Value.Replace("=", "");
            var lesserValue = Regex.Match(text, @$">{DECIMAL_REGEX}").Value.Replace(">", "");
            var GreaterValue = Regex.Match(text, @$"<{DECIMAL_REGEX}").Value.Replace("<", "");

            decimal? min = null;
            decimal? max = null;
            decimal? equivalent = null;

            if (!string.IsNullOrEmpty(exactValue)) equivalent = decimal.Parse(exactValue);
            if (!string.IsNullOrEmpty(lesserValue)) min = decimal.Parse(lesserValue);
            if (!string.IsNullOrEmpty(GreaterValue)) max = decimal.Parse(GreaterValue);
            return (min, max, equivalent);
        }

        public (int? min, int? max, int? exact) ParseIntegerValues(string FilterText)
        {
            var exactValue = Regex.Match(FilterText, @$"={INTEGER_REGEX}").Value.Replace("=", "");
            var lesserValue = Regex.Match(FilterText, @$">{INTEGER_REGEX}").Value.Replace(">", "");
            var GreaterValue = Regex.Match(FilterText, @$"<{INTEGER_REGEX}").Value.Replace("<", "");

            int? min = null;
            int? max = null;
            int? equivalent = null;

            if (!string.IsNullOrEmpty(exactValue)) equivalent = int.Parse(exactValue);
            if (!string.IsNullOrEmpty(lesserValue)) min = int.Parse(lesserValue);
            if (!string.IsNullOrEmpty(GreaterValue)) max = int.Parse(GreaterValue);
            return (min, max, equivalent);
        }

        public (TimeSpan? min, TimeSpan? max, TimeSpan? exact) ParseTimeSpanValues(string FilterText)
        {
            var equivalentValue = Regex.Match(FilterText, @$"={TIMESPAN_HH_MM_REGEX}").Value.Replace("=", "");
            var lesserValue = Regex.Match(FilterText, @$">{TIMESPAN_HH_MM_REGEX}").Value.Replace(">", "");
            var GreaterValue = Regex.Match(FilterText, @$"<{TIMESPAN_HH_MM_REGEX}").Value.Replace("<", "");

            TimeSpan? min = null;
            TimeSpan? max = null;
            TimeSpan? equivalent = null;

            if (!string.IsNullOrEmpty(equivalentValue)) equivalent = TimeSpan.Parse(equivalentValue);
            if (!string.IsNullOrEmpty(lesserValue)) min = TimeSpan.Parse(lesserValue);
            if (!string.IsNullOrEmpty(GreaterValue)) max = TimeSpan.Parse(GreaterValue);
            return (min, max, equivalent);
        }
    }
}
