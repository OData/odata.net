//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for ReadOnlyEnumerable and ReadOnlyEnumerableOfT
    /// </summary>
    internal static class ReadOnlyEnumerableExtensions
    {
        /// <summary>
        /// true if <paramref name="source"/> is the same instance as ReadOnlyEnumerableOfT.Empty(). false otherwise.
        /// </summary>
        /// <typeparam name="T">The element type of the enumerable.</typeparam>
        /// <param name="source">The enumerable in question.</param>
        /// <returns>Returns true if <paramref name="source"/> is the empty ReadOnlyEnumerableOfT. false otherwise.</returns>
        internal static bool IsEmptyReadOnlyEnumerable<T>(this IEnumerable<T> source)
        {
            return ReferenceEquals(source, ReadOnlyEnumerable<T>.Empty());
        }

        /// <summary>
        /// Casts an IEnumerableOfT to ReadOnlyEnumerableOfT.
        /// </summary>
        /// <typeparam name="T">The element type of the enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="collectionName">The name of the collection to report in case there's an error.</param>
        /// <returns>The casted ReadOnlyEnumerableOfT.</returns>
        internal static ReadOnlyEnumerable<T> ToReadOnlyEnumerable<T>(this IEnumerable<T> source, string collectionName)
        {
            Debug.Assert(!String.IsNullOrEmpty(collectionName), "!string.IsNullOrEmpty(collectionName)");

            ReadOnlyEnumerable<T> readonlyCollection = source as ReadOnlyEnumerable<T>;
            if (readonlyCollection == null)
            {
                throw new ODataException(Strings.ReaderUtils_EnumerableModified(collectionName));
            }

            return readonlyCollection;
        }

        /// <summary>
        /// Returns the <paramref name="source"/> as ReadOnlyEnumerableOfT or
        /// a new instance of ReadOnlyEnumerableOfT if <paramref name="source"/> is the same instance as ReadOnlyEnumerableOfT.Empty().
        /// </summary>
        /// <typeparam name="T">The element type of the enumerable.</typeparam>
        /// <param name="source">The source enumerable in question.</param>
        /// <param name="collectionName">The name of the collection to report in case there's an error.</param>
        /// <returns>Returns the <paramref name="source"/> as ReadOnlyEnumerableOfT or
        /// a new instance of ReadOnlyEnumerableOfT if <paramref name="source"/> is the same instance as ReadOnlyEnumerableOfT.Empty().</returns>
        internal static ReadOnlyEnumerable<T> GetOrCreateReadOnlyEnumerable<T>(this IEnumerable<T> source, string collectionName)
        {
            Debug.Assert(!String.IsNullOrEmpty(collectionName), "!string.IsNullOrEmpty(collectionName)");

            if (source.IsEmptyReadOnlyEnumerable())
            {
                return new ReadOnlyEnumerable<T>();
            }

            return source.ToReadOnlyEnumerable(collectionName);
        }

        /// <summary>
        /// Returns a ReadOnlyEnumerableOfT that is the result of <paramref name="source"/> plus <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the enumerable.</typeparam>
        /// <param name="source">The source enumerable to concat.</param>
        /// <param name="collectionName">The name of the collection to report in case there's an error.</param>
        /// <param name="item">Item to concat to the source enumerable.</param>
        /// <returns>Returns a ReadOnlyEnumerableOfT that is the result of <paramref name="source"/> plus <paramref name="item"/>.</returns>
        internal static ReadOnlyEnumerable<T> ConcatToReadOnlyEnumerable<T>(this IEnumerable<T> source, string collectionName, T item)
        {
            ReadOnlyEnumerable<T> readOnlyEnumerable = source.GetOrCreateReadOnlyEnumerable(collectionName);
            readOnlyEnumerable.AddToSourceList(item);
            return readOnlyEnumerable;
        }
    }
}
