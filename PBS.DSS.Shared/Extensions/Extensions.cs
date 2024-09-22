using System.Text.RegularExpressions;
using PBS.DSS.Shared.Models.WorkItems;

namespace PBS.DSS.Shared
{
    public static partial class Extensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhitespace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsInteger(this string value)
        {
            return IntegerRegex().IsMatch(value);
        }

        public static bool IsNumeric(this string value)
        {
            return NumberRegex().IsMatch(value);
        }

        public static string Left(this string vaule, int length)
        {
            return string.Concat(vaule.Take(length));
        }

        public static string Right(this string vaule, int length)
        {
            return string.Concat(vaule.TakeLast(length));
        }

        public static DateTime StartOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        public static DateTime EndOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static DateTime StartOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1, 0, 0, 0);
        }

        public static DateTime EndOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month), 23, 59, 59);
        }

        public static string ToStandardDateString(this DateTime value)
        {
            return value.ToString("MM/dd/yyyy");
        }

        public static bool HasItems<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static bool IsInSet<T>(this T value, params T[] args)
        {
            return args.Any(x => x != null && x.Equals(value));
        }

        public static bool IsApproved(this RequestLine req)
        {
            return req.AWRStatus == AWRStatuses.Approved;
        }

        public static string FriendlyText(this RecommendedPriority p)
        {
            switch (p)
            {
                case RecommendedPriority.Medium:
                    return Resources.Resources.AttentionRequired;
                case RecommendedPriority.High:
                    return Resources.Resources.ImmediateAttentionRequired;
                default:
                    return string.Empty;
            }
        }

        public static string LabelStyle(this RecommendedPriority p)
        {
            var baseStyle = "color:white; width:fit-content; border-radius:8px;";

            switch (p)
            {
                case RecommendedPriority.Medium:
                    return $"{baseStyle} background-color:darkorange;";
                case RecommendedPriority.High:
                    return $"{baseStyle} background-color:darkred;";
                default:
                    return string.Empty;
            }
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex IntegerRegex();

        [GeneratedRegex(@"^\d+(\.\d+)?$")]
        private static partial Regex NumberRegex();
    }
}
