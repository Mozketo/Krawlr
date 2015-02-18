using System;

namespace Krawlr.Core.Extensions
{
    public static class StringEx
    {
        public static string RemoveTrailing(this string s, char remove)
        {
            int position = s.LastIndexOf(remove);
            var result = position > -1 && (s.Length - 1) == position
                ? s.Substring(0, s.Length - 1)
                : s;
            return result;
        }
    }
}
