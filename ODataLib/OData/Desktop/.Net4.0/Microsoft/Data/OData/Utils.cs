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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Generic  utility methods.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Calls IDisposable.Dispose() on the argument if it is not null 
        /// and is an IDisposable.
        /// </summary>
        /// <param name="o">The instance to dispose.</param>
        /// <returns>'True' if IDisposable.Dispose() was called; 'false' otherwise.</returns>
        internal static bool TryDispose(object o)
        {
            DebugUtils.CheckNoExternalCallers();

            IDisposable disposable = o as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                return true;
            }

            return false;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes a stream.
        /// </summary>
        /// <param name="stream">The stream to flush.</param>
        /// <returns>Task which represents the pending Flush operation.</returns>
        internal static Task FlushAsync(this Stream stream)
        {
            DebugUtils.CheckNoExternalCallers();
            return Task.Factory.StartNew(stream.Flush);
        }
#endif

        /// <summary>
        /// Perform a stable sort of the <paramref name="array"/> using the specified <paramref name="comparison"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array to sort.</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparison">The comparison to use to compare items in the array</param>
        /// <returns>Array of KeyValuePairs where the sequence of Values is the sorted representation of <paramref name="array"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Array.Sort is causing this, but it is needed in order to sort the KeyValuePairs using our own comparer.")]
        internal static KeyValuePair<int, T>[] StableSort<T>(this T[] array, Comparison<T> comparison)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(array, "array");
            ExceptionUtils.CheckArgumentNotNull(comparison, "comparison");

            KeyValuePair<int, T>[] keys = new KeyValuePair<int, T>[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                keys[i] = new KeyValuePair<int, T>(i, array[i]);
            }

            Array.Sort(keys, new StableComparer<T>(comparison));
            return keys;
        }

        /// <summary>
        /// Stable comparer of a sequence of key/value pairs where each pair 
        /// knows its position in the sequence and its value.
        /// </summary>
        /// <typeparam name="T">The type of the values in the sequence.</typeparam>
        private sealed class StableComparer<T> : IComparer<KeyValuePair<int, T>>
        {
            /// <summary>
            /// The <see cref="Comparison&lt;T&gt;"/> to compare the values.
            /// </summary>
            private readonly Comparison<T> innerComparer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="innerComparer">The <see cref="Comparison&lt;T&gt;"/> to compare the values.</param>
            public StableComparer(Comparison<T> innerComparer)
            {
                this.innerComparer = innerComparer;
            }

            /// <summary>
            /// Compares two key/value pairs by first comparing their value. If the values are equal,
            /// the position in the array determines the relative order (and preserves the original relative order).
            /// </summary>
            /// <param name="x">First key/value pair.</param>
            /// <param name="y">Second key/value pair.</param>
            /// <returns>
            /// A value &lt; 0 if <paramref name="x"/> is less than <paramref name="y"/>.
            /// The value 0 if <paramref name="x"/> is equal to <paramref name="y"/>. Note this only happens when comparing the same items when used in StableSort.
            /// A value &gt; 0 if <paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <remarks>This method will never return the value 0 since the input sequence is constructed in a way
            /// that all key/value pairs have unique indeces.</remarks>
            public int Compare(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
            {
                int result = this.innerComparer(x.Value, y.Value);

                if (result == 0)
                {
                    // use the position of the value in the array to preserve the relative order
                    result = x.Key - y.Key;
                }

                return result;
            }
        }
    }
}
