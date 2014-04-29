//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IEnumerable&gt;T&lt; which is based on a List&gt;T&lt;
    /// but only exposes readonly access to that collection. This class doesn't implement
    /// any other public interfaces or public API unlike most other IEnumerable implementations
    /// which also implement other public interfaces.
    /// </summary>
    /// <typeparam name="T">The type of a single item in the enumeration.</typeparam>
    internal sealed class ReadOnlyEnumerable<T> : ReadOnlyEnumerable, IEnumerable<T>
    {
        /// <summary>
        /// The IEnumerable to wrap.
        /// </summary>
        private readonly IList<T> sourceList;

        /// <summary>
        /// The empty instance of ReadOnlyEnumerableOfT.
        /// </summary>
        private static readonly SimpleLazy<ReadOnlyEnumerable<T>> EmptyInstance = new SimpleLazy<ReadOnlyEnumerable<T>>(() => new ReadOnlyEnumerable<T>(new ReadOnlyCollection<T>(new List<T>(0))), true /*isThreadSafe*/);

        /// <summary>
        /// Constructor which initializes the enumerable with an empty list storage.
        /// </summary>
        internal ReadOnlyEnumerable()
            : this(new List<T>())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceList">The list of values to wrap.</param>
        internal ReadOnlyEnumerable(IList<T> sourceList)
            : base(sourceList)
        {
            Debug.Assert(sourceList != null, "sourceList != null");

            this.sourceList = sourceList;
        }

        /// <summary>
        /// Returns the enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.sourceList.GetEnumerator();
        }

        /// <summary>
        /// Gets the empty instance of ReadOnlyEnumerableOfT.
        /// </summary>
        /// <returns>Returns the empty instance of ReadOnlyEnumerableOfT.</returns>
        internal static ReadOnlyEnumerable<T> Empty()
        {
            return EmptyInstance.Value;
        }
        
        /// <summary>
        /// This internal method adds <paramref name="itemToAdd"/> to the wrapped source list. From the public's perspective, this enumerable is still readonly.
        /// </summary>
        /// <param name="itemToAdd">Item to add to the source list.</param>
        internal void AddToSourceList(T itemToAdd)
        {
            Debug.Assert(this.sourceList != null, "this.sourceList != null");

            this.sourceList.Add(itemToAdd);
        }
    }
}
