using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EverybodyEdits.Common
{
    internal class SimpleTimeParser
    {
        private Regex Pattern = new Regex(@"^(?=\d)((?<days>\d+)d)?(?=\d)((?<hours>\d+)h)?\s*((?<minutes>\d+)m?)?\s*((?<seconds>\d+)s?)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal TimeSpan ParseSimpleTimeSpan(string input)
        {
            if (!Pattern.IsMatch(input))
                return TimeSpan.Zero;

            var match = Pattern.Match(input);

            var days = int.Parse(match.Groups["days"].Success ? match.Groups["days"].Value : "0");
            var hours = int.Parse(match.Groups["hours"].Success ? match.Groups["hours"].Value : "0");
            var minutes = int.Parse(match.Groups["minutes"].Success ? match.Groups["minutes"].Value : "0");
            var seconds = int.Parse(match.Groups["seconds"].Success ? match.Groups["seconds"].Value : "0");

            var timespan = new TimeSpan(days, hours, minutes, seconds);

            if (timespan.TotalSeconds == 0)
                return TimeSpan.Zero;

            return timespan;
        }

    }

    public static class SimpleTimeParserExtension
    {
        public static string ToPrettyFormat(this TimeSpan span)
        {
            if (span == TimeSpan.Zero)
                return "0 minutes";

            var output = new StringBuilder();

            if (span.Days > 0)
                output.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                output.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                output.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            if (span.Seconds > 0)
                output.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);

            return output.ToString();
        }
    }
}