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

        public static string ToCsv<T>(this T item, string separator = ",", bool includeHeader = false)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            //if (includeHeader)
            //{
            //    yield return String.Join(separator, fields.Select(f => f.Name).Union(properties.Select(p => p.Name)).ToArray());
            //}
            //yield 
            return string.Join(separator, fields.Select(f => (f.GetValue(item) ?? "").ToString())
                .Union(properties.Select(p => (ToString(p.GetValue(item, null)) ?? "").ToString())));
        }

        public static IEnumerable<string> ToCsv<T>(this IEnumerable<T> objectlist, string separator = ",", bool includeHeader = false)
        {
            foreach (var o in objectlist)
                yield return o.ToCsv(separator, includeHeader);
        }

        static string ToString<T>(T type)
        {
            var primitiveTypes = new[] { typeof(int), typeof(string), typeof(DateTime) };
            if (primitiveTypes.Contains(type.GetType()))
                return type.ToString();

            var items = type as IEnumerable<string>;
            if (items != null)
                return String.Join(",", items);

            return type.ToString();
        }
    }
}
