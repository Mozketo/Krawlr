using System;

namespace Krawlr.Core.Infrastructure.Extensions
{
    public static class StringEx
    {
        public static bool ContainsEx(this string value, string subString)
        {
            return value.ContainsEx(subString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// (IS) Returns a value indicating whether the specified System.String object occurs within this string.
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="subString">subString to find in this string</param>
        /// <param name="comparison">comparison to use when looking for the sub-string</param>
        /// <returns>true if the specified subString occurs within this string.</returns>
        public static bool ContainsEx(this string value, string subString, StringComparison comparison)
        {
            if (!subString.HasValue())
                return false;

            return value.EmptyIfNull().IndexOf(subString, comparison) >= 0;
        }

        /// <summary>
        /// (IS) Returns string.Empty if this string is null
        /// </summary>
        /// <param name="value">this string</param>
        /// <returns>given value if not null, otherwise ""</returns>
        public static string EmptyIfNull(this string value)
        {
            return value ?? String.Empty;
        }

        public static bool HasValue(this string s)
        {
            return !String.IsNullOrEmpty(s);
        }
    }
}
