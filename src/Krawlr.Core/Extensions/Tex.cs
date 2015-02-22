namespace Krawlr.Core.Extensions
{
    using System.Linq;

    public static class Tex
    {
        public static bool In<T>(this T item, params T[] that)
        {
            var result = that.Any(t => t.Equals(item));
            return result;
        }
    }
}
