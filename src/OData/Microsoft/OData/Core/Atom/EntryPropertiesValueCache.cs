//   OData .NET Libraries ver. 6.9
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
