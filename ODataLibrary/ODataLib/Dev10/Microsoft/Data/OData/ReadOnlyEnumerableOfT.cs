//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
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
        /// The list of values to expose through IEnumerable.
        /// </summary>
        private List<T> sourceList;

        /// <summary>
        /// Constructor which initializes the enumerable with an empty list storage.
        /// </summary>
        internal ReadOnlyEnumerable()
            : this(new List<T>())
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceList">The list of values to wrap.</param>
        internal ReadOnlyEnumerable(List<T> sourceList) : base(sourceList)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(sourceList != null, "sourceList != null");

            this.sourceList = sourceList;
        }

        /// <summary>
        /// The source list which holds the values in the enumeration.
        /// </summary>
        internal List<T> SourceList
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.sourceList; 
            }
        }

        /// <summary>
        /// Returns the enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.sourceList.GetEnumerator();
        }
    }
}
