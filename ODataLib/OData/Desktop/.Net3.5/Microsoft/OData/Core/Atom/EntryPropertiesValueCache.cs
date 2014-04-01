//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Caches values of properties enumerations on an entry.
    /// </summary>
    internal sealed class EntryPropertiesValueCache
    {
        /// <summary>
        /// Caches the ODataEntry.Properties enumeration.
        /// </summary>
        private readonly List<ODataProperty> entryPropertiesCache;

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="entry">The entry for which to create the properties cache.</param>
        internal EntryPropertiesValueCache(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // Cache the entry properties right here as there's no point in delaying since we will enumerate those anyway.
            if (entry.Properties != null)
            {
                this.entryPropertiesCache = new List<ODataProperty>(entry.Properties);
            }
        }

        /// <summary>
        /// Returns enumeration of properties (excluding stream properties) for the entry.
        /// </summary>
        internal IEnumerable<ODataProperty> EntryProperties
        {
            get
            {
                return this.entryPropertiesCache != null ? this.entryPropertiesCache.Where(p => p == null || !(p.Value is ODataStreamReferenceValue)) : null;
            }
        }

        /// <summary>
        /// Returns enumeration of stream properties for the entry.
        /// </summary>
        internal IEnumerable<ODataProperty> EntryStreamProperties
        {
            get
            {
                return this.entryPropertiesCache != null ? this.entryPropertiesCache.Where(p => p != null && p.Value is ODataStreamReferenceValue) : null;
            }
        }
    }
}
