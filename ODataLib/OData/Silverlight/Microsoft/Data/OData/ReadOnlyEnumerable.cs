//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IEnumerable which is based on another IEnumerable
    /// but only exposes readonly access to that collection. This class doesn't implement
    /// any other public interfaces or public API unlike most other IEnumerable implementations
    /// which also implement other public interfaces.
    /// </summary>
    internal class ReadOnlyEnumerable : IEnumerable
    {
        /// <summary>
        /// The IEnumerable to wrap.
        /// </summary>
        private readonly IEnumerable sourceEnumerable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceEnumerable">The enumerable to wrap.</param>
        internal ReadOnlyEnumerable(IEnumerable sourceEnumerable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(sourceEnumerable != null, "sourceEnumerable != null");

            this.sourceEnumerable = sourceEnumerable;
        }

        /// <summary>
        /// Returns the enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sourceEnumerable.GetEnumerator();
        }
    }
}
