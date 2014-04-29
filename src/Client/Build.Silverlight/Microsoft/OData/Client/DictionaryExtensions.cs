//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
        [SuppressMessage("Microsoft.Performance", "CA1811:'DictionaryExtensions.FindOrAdd<TKey, TValue>(this IDictionary<TKey, TValue>, TKey, Func<TValue>)' appears to have no upstream public or protected callers.", Justification = "Callers removed, but a useful helper method to have.")]
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
