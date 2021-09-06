//---------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Useful extension methods for IDictionary and ConcurrentDictionary
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Convenience function that wraps ConcurrentDictionary.TryAdd() to allow same signature as IDictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="self">The ConcurrentDictionary to which to add the item.</param>
        /// <param name="key">The key to use in adding the item to the ConcurrentDictionary.</param>
        /// <param name="value">The value to add to the ConcurrentDictionary.</param>
        public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (!self.TryAdd(key, value))
            {
                throw new ArgumentException("Argument_AddingDuplicate");
            }
        }

        /// <summary>
        /// Convenience function for ConcurrentDictionary to allow same signature as IDictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="self">The concurrent dictionary from which to remove the item.</param>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>
        /// True, if the item is removed, otherwise False.
        /// </returns>
        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            return ((IDictionary<TKey, TValue>)self).Remove(key);
        }

        /// <summary>
        /// If the key exists in the dictionary, returns it. Otherwise creates a new value, adds it to the dictionary, and returns it.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to look in.</param>
        /// <param name="key">The key to find/add.</param>
        /// <param name="createValue">A callback to create a new value if one is not found.</param>
        /// <returns>The new or found value.</returns>
        internal static TValue FindOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createValue)
        {
            Debug.Assert(dictionary != null, "dictionary != null");
            Debug.Assert(createValue != null, "createValue != null");

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                dictionary[key] = value = createValue();
            }

            return value;
        }

        /// <summary>
        /// Sets a range of values in the dictionary. A set operation is performed on each value in <paramref name="valuesToCopy"/>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to set the values in.</param>
        /// <param name="valuesToCopy">Enumerable of key-value pairs to set in <paramref name="dictionary"/>.</param>
        internal static void SetRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> valuesToCopy)
        {
            foreach (var item in valuesToCopy)
            {
                dictionary[item.Key] = item.Value;
            }
        }
    }
}