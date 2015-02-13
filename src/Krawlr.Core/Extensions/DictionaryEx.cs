using System;
using System.Collections.Generic;

namespace Krawlr.Core.Extensions
{
    public static class DictionaryEx
    {
        /// <summary>
        /// <para>(IS) finds value in the dictionary which corresponds to the given key. Calls the defaultGetter function and returns its result if the key isnt in the dictionary.</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <typeparam name="TKey">dictionary key type</typeparam>
        /// <typeparam name="TValue">dictionary value type</typeparam>
        /// <param name="dictionary">dictionary to search for the given key</param>
        /// <param name="key">the key to find in the dictionary</param>
        /// <param name="defaultGetter">factory function to call to get a value if its is not found in the dictionary</param>
        /// <returns>either value found in the dictionary, or value returned by the defaultGetter function if not found.</returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultGetter)
        {
            return dictionary.FindValue(key).GetValue(() => defaultGetter(key));
        }

        /// <summary>
        /// (IS) Returns the value for the given key; if not found, creates it using the value factory function, and adds it to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="dictionary">dictionary to search for the value</param>
        /// <param name="key">key value to search by</param>
        /// <param name="valueFactory">value factory function - called when the value isnt found in the dictionary</param>
        /// <returns>value for the given key - existing if found, or created through the value factory function and added to the dictionary otherwise</returns>
        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            {
                var res = dictionary.FindValue(key);
                if (res.IsSome)
                    return res.Value;
            }

            {
                var res = valueFactory();
                dictionary[key] = res;
                return res;
            }
        }

        /// <summary>
        /// (IS) returns value in the dictionary which corresponds to the given key, or TValue default value if not found.
        /// </summary>
        /// <typeparam name="TKey">dictionary key type</typeparam>
        /// <typeparam name="TValue">dictionary value type</typeparam>
        /// <param name="dictionary">dictionary to search for the given key</param>
        /// <param name="key">the key to find in the dictionary</param>
        /// <returns>either value found in the dictionary, or default value of TValue type.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.FindValue(key).GetValueOrDefault();
        }

        /// <summary>
        /// (IS) returns Some(value) if the given key is found in the dictionary, or None otherwise
        /// </summary>
        /// <typeparam name="TKey">dictionary key type</typeparam>
        /// <typeparam name="TValue">dictionary value type</typeparam>
        /// <param name="dictionary">dictionary to search for the given key</param>
        /// <param name="key">the key to find in the dictionary</param>
        /// <returns>either Some(value) found in the dictionary, or None.</returns>
        public static Option<TValue> FindValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                return Option<TValue>.None;

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                return Option<TValue>.None;
            else
                return Option.Some(value);
        }

        /// <summary>
        /// Determines whether the System.Collections.Generic.IDictionary[TKey,TValue] contains an element with the specified key.
        /// </summary>
        /// <typeparam name="TKey">dictionary key type</typeparam>
        /// <typeparam name="TValue">dictionary value type</typeparam>
        /// <param name="dict">this dictionary</param>
        /// <param name="key">key value (can be null)</param>
        /// <returns>true if the dictionary contains an element with the key; otherwise, false.</returns>
        public static bool ContainsKeyEx<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return key == null ? false : dict.ContainsKey(key);
        }

        /// <summary>
        /// (IS) adds the given key/value if the given ket isnt present in the dictionary; returns true if the key/value were added
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="dict">dictionary to add the given key/value to</param>
        /// <param name="key">the key to add</param>
        /// <param name="value">the value to add</param>
        /// <returns>returns true if the key/value didnt exist in the dictionary and were added; otherwise - false</returns>
        public static bool AddIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }
    }
}
