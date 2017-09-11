//---------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Useful extension methods for IDictionary
    /// </summary>
    internal static class DictionaryExtensions
    {
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

#if PORTABLELIB
        /// <summary>
        /// Try to add a key/value pair to the dictionary and returns true if the key was added, false if it was already present.
        /// The difference with Add is that it will not throw <see cref="ArgumentException"/> if the key is already present.
        /// </summary>
        /// <remarks>
        /// This provides a shim for ConcurrentDictionary.TryAdd that isn't supported in Windows Phone 8.0.
        /// </remarks>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to try to add the value to.</param>
        /// <param name="key">The key to add if not already present.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>True if the key was added or false if the key is already present.</returns>
        internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            try
            {
                TValue val;
                if (!dict.TryGetValue(key, out val))
                {
                    dict.Add(key, value);
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }

            return false;
        }
#endif
    }
}
