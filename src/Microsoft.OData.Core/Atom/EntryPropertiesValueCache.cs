//---------------------------------------------------------------------
// <copyright file="EntryPropertiesValueCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
