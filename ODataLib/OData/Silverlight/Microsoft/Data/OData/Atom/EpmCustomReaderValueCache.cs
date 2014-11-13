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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Cache for values read during deserialization of custom EPM.
    /// </summary>
    internal sealed class EpmCustomReaderValueCache
    {
        /// <summary>
        /// List of custom EPM values read.
        /// This is a map from EPM info to the value read from the payload.
        /// </summary>
        /// <remarks>
        /// The list order is the order in which the values were read from the payload.
        /// They will be applied to the entry properties in that order (this needs to be maintained).
        /// The key is the EPM info for the mapping according to which the value was read.
        /// The value is the string value read from the content (not converted in any way).
        /// null value means true null value should be used.
        /// If the value was missing from the payload there will be no record of it in this list.
        /// </remarks>
        private readonly List<KeyValuePair<EntityPropertyMappingInfo, string>> customEpmValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal EpmCustomReaderValueCache()
        {
            DebugUtils.CheckNoExternalCallers();
            this.customEpmValues = new List<KeyValuePair<EntityPropertyMappingInfo, string>>();
        }

        /// <summary>
        /// The list of stored custom EPM values (key is the EPM info, value is the string value read for it).
        /// The list is in the order in which the values were read from the payload.
        /// </summary>
        internal IEnumerable<KeyValuePair<EntityPropertyMappingInfo, string>> CustomEpmValues
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.customEpmValues;
            }
        }

        /// <summary>
        /// Determines if the cache already contains a value for the specified EPM mapping.
        /// </summary>
        /// <param name="epmInfo">The EPM info for the EPM mapping to look for.</param>
        /// <returns>true if the cache already contains a value for this mapping, false otherwise.</returns>
        internal bool Contains(EntityPropertyMappingInfo epmInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmInfo != null, "epmInfo != null");

            return this.customEpmValues.Any(epmValue => object.ReferenceEquals(epmValue.Key, epmInfo));
        }

        /// <summary>
        /// Adds a value to cache.
        /// </summary>
        /// <param name="epmInfo">The EPM info for the mapping according to which the value was read.</param>
        /// <param name="value">The value to cache.</param>
        /// <remarks>
        /// The method will only store the first value for any given EPM info, since in custom EPM
        /// only the first occurrence of the element/attribute is used, the others are ignored.
        /// </remarks>
        internal void Add(EntityPropertyMappingInfo epmInfo, string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(!this.Contains(epmInfo), "We should not be trying to add more than one value for any given mapping.");

            this.customEpmValues.Add(new KeyValuePair<EntityPropertyMappingInfo, string>(epmInfo, value));
        }
    }
}
