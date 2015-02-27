namespace Krawlr.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Tex
    {
        public static bool In<T>(this T item, params T[] that)
        {
            var result = that.Any(t => t.Equals(item));
            return result;
        }

        public static string ToCsv<T>(this T item, string separator)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            return String.Join(separator, fields.Select(f => f.Name).Union(properties.Select(p => p.Name)).ToArray());
        }

        public static IEnumerable<string> ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            yield return String.Join(separator, fields.Select(f => f.Name).Union(properties.Select(p => p.Name)).ToArray());
            foreach (var o in objectlist)
            {
                yield return string.Join(separator, fields.Select(f => (f.GetValue(o) ?? "").ToString())
                    .Union(properties.Select(p => (p.GetValue(o, null) ?? "").ToString())).ToArray());
            }
        }
    }
}
