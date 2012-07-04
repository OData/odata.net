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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Caches values of properties enumerations on an entry and then EPM values for the rest of property values.
    /// </summary>
    internal sealed class EntryPropertiesValueCache : EpmValueCache
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
            DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.entryPropertiesCache != null ? this.entryPropertiesCache.Where(p => p != null && p.Value is ODataStreamReferenceValue) : null;
            }
        }
    }
}
