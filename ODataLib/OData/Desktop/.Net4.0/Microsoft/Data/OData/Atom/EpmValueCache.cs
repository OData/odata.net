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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Caches values of properties and items enumerations so that we only ever enumerate these once even if they were use in EPM.
    /// </summary>
    internal class EpmValueCache
    {
        /// <summary>
        /// Caches either ComplexValue properties enumeration or Collection items enumeration.
        /// </summary>
        /// <remarks>The key is the complex value, or collection for the property in question.
        /// For complex property, the value is a List of ODataProperty which stores the enumeration ODataComplexValue.Properties cache.
        /// For collection property, the value is a List of object which stores the enumeration ODataCollectionValue.Items cache.
        /// The items are either EpmCollectionItemCache instances in which case the value of the item is cached inside that instance,
        /// or it's any other type in which case the value of the item is that instance.</remarks>
        private Dictionary<object, object> epmValuesCache;

        /// <summary>
        /// Creates a new empty cache.
        /// </summary>
        internal EpmValueCache()
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Returns the properties for the specified complex value.
        /// </summary>
        /// <param name="epmValueCache">The EPM value cache to use (can be null).</param>
        /// <param name="complexValue">The complex value to get the properties for.</param>
        /// <param name="writingContent">If we're writing content of an entry or not.</param>
        /// <returns>The properties enumeration for the complex value.</returns>
        internal static IEnumerable<ODataProperty> GetComplexValueProperties(
            EpmValueCache epmValueCache,
            ODataComplexValue complexValue,
            bool writingContent)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");
            Debug.Assert(writingContent || epmValueCache != null, "If we're not writing content, then the EPM value cache must exist.");

            if (epmValueCache == null)
            {
                return complexValue.Properties;
            }
            else
            {
                return epmValueCache.GetComplexValueProperties(complexValue, writingContent);
            }
        }

        /// <summary>
        /// Caches and returns the properties for the specified complex value.
        /// </summary>
        /// <param name="complexValue">The complex value to cache the properties for.</param>
        /// <returns>The cached properties enumeration for the complex value.</returns>
        /// <remarks>This method assumes that the complex value's properties are not cached yet.</remarks>
        internal IEnumerable<ODataProperty> CacheComplexValueProperties(ODataComplexValue complexValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.epmValuesCache == null || complexValue == null || !this.epmValuesCache.ContainsKey(complexValue), 
                "Complex value must not be cached.");

            if (complexValue == null)
            {
                return null;
            }

            IEnumerable<ODataProperty> properties = complexValue.Properties;
            List<ODataProperty> cachedProperties = null;
            if (properties != null)
            {
                cachedProperties = new List<ODataProperty>(properties);
            }

            if (this.epmValuesCache == null)
            {
                this.epmValuesCache = new Dictionary<object, object>(ReferenceEqualityComparer<object>.Instance);
            }

            this.epmValuesCache.Add(complexValue, cachedProperties);
            return cachedProperties;
        }

        /// <summary>
        /// Returns the properties for the specified complex value.
        /// </summary>
        /// <param name="complexValue">The complex value to get the properties for.</param>
        /// <param name="writingContent">true if we're writing entry content or false when writing out-of-content EPM.</param>
        /// <returns>The properties enumeration for the complex value.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writingContent", Justification = "Parameter used in debug.")]
        private IEnumerable<ODataProperty> GetComplexValueProperties(ODataComplexValue complexValue, bool writingContent)
        {
            Debug.Assert(complexValue != null, "complexValue != null");

            object cachedPropertiesValue;
            if (this.epmValuesCache != null && this.epmValuesCache.TryGetValue(complexValue, out cachedPropertiesValue))
            {
                Debug.Assert(cachedPropertiesValue is List<ODataProperty>, "The cached value for complex type must be a List of ODataProperty");
                return (IEnumerable<ODataProperty>)cachedPropertiesValue;
            }

            // We should only ever get here when writing the content; for EPM purposes all values should have been pre-cached.
            Debug.Assert(writingContent, "Only when writing content should we find non-cached values.");
            return complexValue.Properties;
        }
    }
}
