using System.Text.RegularExpressions;

namespace PBS.Blazor.Framework.Extensions
{
    public static partial class Extensions
    {
        #region String
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
        #endregion

        #region DateTime
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
        #endregion

        #region Enumerable
        public static bool HasItems<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static bool IsInSet<T>(this T value, params T[] args)
        {
            return args.Any(x => x != null && x.Equals(value));
        }
        #endregion

        #region Byte
        public static bool HasValue(this byte[] b)
        {
            return b != null && b.Length > 0;
        }
        #endregion

        #region regex
        [GeneratedRegex(@"^\d+$")]
        private static partial Regex IntegerRegex();

        [GeneratedRegex(@"^\d+(\.\d+)?$")]
        private static partial Regex NumberRegex();
        #endregion
    }
}
