using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Simple Extension Method for IDictionary
        /// If you have an IDicitonary whose value implements
        /// IColleciton, you can use this function to add a new item
        /// to a key's value.
        /// </summary>
        /// <typeparam name="T1">The key's type</typeparam>
        /// <typeparam name="T2">The value's type; must implement ICollection&lt;T3&gt;</typeparam>
        /// <typeparam name="T3">The type of the ICollection</typeparam>
        /// <param name="iDictionary">The IDictionary upon which to add the values</param>
        /// <param name="key">The key, whose ICollection value is what the item will be added to</param>
        /// <param name="item">The item to add to the key's ICollection</param>
        /// <returns>True if the IDictionary had to create a new instance of the ICollection, false otherwise</returns>
        public static bool TryAdd<T1, T2, T3>(this IDictionary<T1, T2> iDictionary, T1 key, T3 item) where T2 : ICollection<T3>, new()
        {
            bool retval = false;

            T2 collection;
            if (!iDictionary.TryGetValue(key, out collection) || collection == null)
            {
                collection = new T2();
                iDictionary[key] = collection;
                retval = true;
            }
            collection.Add(item);

            return retval;
        }

        /// <summary>
        /// Merge two Dictionaries with the same signiture. **WARNING** Duplicate Keys will take the Value from the Merge Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="me"></param>
        /// <param name="merge"></param>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> me, IDictionary<TKey, TValue> merge)
        {
            foreach (var item in merge)
            {
                me[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// If the given key is present in the dictionary, returns the associated item.  Otherwise, returns the default for the item's class.
        /// </summary>
        /// <param name="key">The key to search for in the dictionary.</param>
        public static TValue GetItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            return value;
        }
        public static TValue GetItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            TValue value = default(TValue);
            if (dictionary.TryGetValue(key, out value) == false)
            {
                value = new TValue();
                dictionary[key] = value;
            }
            return value;
        }

        public static TValue GetItemOrDefault<TValue>(this IDictionary<Guid, TValue> dictionary, Guid? key)
        {
            TValue value = default(TValue);
            if (key.HasValue)
                dictionary.TryGetValue(key.Value, out value);

            return value;
        }

        public static TValue GetItemOrDefault<TValue>(this IDictionary<Guid?, TValue> dictionary, Guid? key)
        {
            TValue value = default(TValue);
            if (key.HasValue)
                dictionary.TryGetValue(key.Value, out value);

            return value;
        }

        public static TValue GetItem<TValue>(this IDictionary<Guid, TValue> dictionary, Guid? key)
            where TValue : new()
        {
            TValue value = default(TValue);
            if (key.HasValue)
            {
                if (dictionary.TryGetValue(key.Value, out value) == false)
                {
                    value = new TValue();
                    dictionary[key.Value] = value;
                }
            }
            return value;
        }

        /// <summary>
        /// If the given key is present in the dictionary, returns the associated item.  Otherwise, returns the default item provided.
        /// </summary>
        /// <param name="key">The key to search for in the dictionary.</param>
        /// <param name="defaultItem">The item to be returned if the key is not present.</param>
        public static TValue GetItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultItem)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value)) value = defaultItem;
            return value;
        }

        /// <summary>
        /// Adds the key-value pair to the dictionary if the key is not already present.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public static bool AddIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key)) return false;
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Removes the key-value pair associated with the given key from the dictionary, if present.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public static bool RemoveIfPresent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key)) return false;
            dictionary.Remove(key);
            return true;
        }

        /// <summary>
        /// Inverts a dictionary such that the values become the keys, and vice-versa
        /// </summary>
        /// <typeparam name="TKey">The data type of the input dictionary's keys</typeparam>
        /// <typeparam name="TValue">The data type of the input dictionary's values</typeparam>
        /// <param name="dictionary">The input dictionary</param>
        /// <param name="keyComparer">An IEqualityComparer for the input dictionary's values, and
        ///                           the output dictionary's keys. If omitted or null, the default
        ///                           IEqualityComparer for the type is used.</param>
        /// <returns>The inverted dictionary</returns>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                                    IEqualityComparer<TValue> keyComparer = null)
        {
            return dictionary.ToFirstDictionary(kvp => kvp.Value, kvp => kvp.Key, keyComparer);
        }

        /// <summary>
        /// Inverts a dictionary such that the values become the keys, and vice-versa
        /// </summary>
        /// <typeparam name="TKey">The data type of the input dictionary's keys</typeparam>
        /// <typeparam name="TValue">The data type of the input dictionary's values</typeparam>
        /// <param name="dictionary">The input dictionary</param>
        /// <param name="keyComparer">An IEqualityComparer for the input dictionary's values, and
        ///                           the output dictionary's keys. If omitted or null, the default
        ///                           IEqualityComparer for the type is used.</param>
        /// <returns>The inverted dictionary</returns>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary,
                                                                    IEqualityComparer<TValue> keyComparer = null)
        {
            return dictionary.ToFirstDictionary(kvp => kvp.Value, kvp => kvp.Key, keyComparer);
        }

        /// <summary>
        /// Inverts a dictionary such that the values become the keys, and vice-versa. 
        /// Since more than one keys can associate with a same value, when value become new key, the new value will be a list of keys.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="keyComparer"></param>
        /// <returns></returns>
        public static IDictionary<TValue, IEnumerable<TKey>> GroupInvert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue> keyComparer = null)
        {
            return dictionary.GroupBy(kvp => kvp.Value).ToDictionary(grp => grp.Key, grp => grp.Select(x => x.Key), keyComparer);
        }


        public static void AddOrCreate<TKey>(this IDictionary<TKey, double> dictionary, TKey key, double value)
        {
            double val;
            if (!dictionary.TryGetValue(key, out val))
            {
                val = default(double);
            }

            dictionary[key] = val + value;
        }
    }
}
