//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
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
