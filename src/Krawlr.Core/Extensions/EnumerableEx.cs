using System;
using System.Collections.Generic;
using System.Linq;

namespace Krawlr.Core.Extensions
{
    public static class EnumerableEx
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iter<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            foreach (var item in items.EmptyIfNull())
                action(item);
        }

        /// <summary>
        /// (IS) Iterates the sequence and calls the given action with the item and its index in the sequence
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iteri<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                action(item, index++);
        }

        /// <summary>
        /// (IS) Returns given items seq or empty seq if the items argument is null
        /// </summary>
        /// <typeparam name="T">seq item type</typeparam>
        /// <param name="items">seq to check for null</param>
        /// <returns>given items seq or empty seq if items is null</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }
    }
}
