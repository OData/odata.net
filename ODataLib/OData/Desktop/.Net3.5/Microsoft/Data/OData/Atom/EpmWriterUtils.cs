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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for EPM writers.
    /// </summary>
    internal static class EpmWriterUtils
    {
        /// <summary>
        /// Given a property value returns the text value to be used in EPM.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The text representation of the value, or the method throws if the text representation was not possible to obtain.</returns>
        internal static string GetPropertyValueAsText(object propertyValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (propertyValue == null)
            {
                return null;
            }

            return AtomValueUtils.ConvertPrimitiveToString(propertyValue);
        }

        /// <summary>
        /// Gets the <see cref="EntityPropertyMappingAttribute"/> for the specified <paramref name="propertyName"/>
        /// from the <paramref name="epmParentSourcePathSegment"/>.
        /// </summary>
        /// <param name="epmParentSourcePathSegment">The EPM source path segment for the parent of the property being written.</param>
        /// <param name="propertyName">The name of the property to get the <see cref="EntityPropertyMappingAttribute"/> for.</param>
        /// <returns>The <see cref="EntityPropertyMappingAttribute"/> for the specified <paramref name="propertyName"/> or null if none exists.</returns>
        internal static EntityPropertyMappingAttribute GetEntityPropertyMapping(EpmSourcePathSegment epmParentSourcePathSegment, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(propertyName, "propertyName");

            EpmSourcePathSegment epmSourcePathSegment = GetPropertySourcePathSegment(epmParentSourcePathSegment, propertyName);
            return GetEntityPropertyMapping(epmSourcePathSegment);
        }

        /// <summary>
        /// Gets the <see cref="EntityPropertyMappingAttribute"/> for the specified <paramref name="epmSourcePathSegment"/>.
        /// </summary>
        /// <param name="epmSourcePathSegment">The EPM source path segment to get the <see cref="EntityPropertyMappingAttribute"/> from.</param>
        /// <returns>The <see cref="EntityPropertyMappingAttribute"/> for the specified <paramref name="epmSourcePathSegment"/> or null if none exists.</returns>
        internal static EntityPropertyMappingAttribute GetEntityPropertyMapping(EpmSourcePathSegment epmSourcePathSegment)
        {
            DebugUtils.CheckNoExternalCallers();

            if (epmSourcePathSegment == null)
            {
                return null;
            }

            EntityPropertyMappingInfo epmInfo = epmSourcePathSegment.EpmInfo;
            if (epmInfo == null)
            {
                return null;
            }

            EntityPropertyMappingAttribute epmAttribute = epmInfo.Attribute;
            Debug.Assert(epmAttribute != null, "Attribute should always be initialized for EpmInfo.");
            return epmAttribute;
        }

        /// <summary>
        /// Returns an <see cref="EpmSourcePathSegment"/> for a given property provided the parent <see cref="EpmSourcePathSegment"/>.
        /// </summary>
        /// <param name="epmParentSourcePathSegment">The parent <see cref="EpmSourcePathSegment"/> to get the property segment from.</param>
        /// <param name="propertyName">The name of the property to get the <see cref="EpmSourcePathSegment"/> for.</param>
        /// <returns>An <see cref="EpmSourcePathSegment"/> for a given property provided the parent <see cref="EpmSourcePathSegment"/>.</returns>
        internal static EpmSourcePathSegment GetPropertySourcePathSegment(EpmSourcePathSegment epmParentSourcePathSegment, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            EpmSourcePathSegment epmSourcePathSegment = null;
            if (epmParentSourcePathSegment != null)
            {
                epmSourcePathSegment = epmParentSourcePathSegment.SubProperties.FirstOrDefault(subProperty => subProperty.PropertyName == propertyName);
            }

            return epmSourcePathSegment;
        }

        /// <summary>
        /// Cache all the properties and collection item enumerations needed for EPM processing.
        /// </summary>
        /// <param name="propertyValueCache">The property value cache to cache the EPM related properties in.</param>
        /// <param name="sourceTree">The source tree describing all properties taking part in entity property mappings.</param>
        internal static void CacheEpmProperties(EntryPropertiesValueCache propertyValueCache, EpmSourceTree sourceTree)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(propertyValueCache != null, "propertyValueCache != null");
            Debug.Assert(sourceTree != null, "sourceTree != null");

            EpmSourcePathSegment rootSegment = sourceTree.Root;
            Debug.Assert(rootSegment.PropertyName == null, "Root segment should have 'null' property name.");
            CacheEpmSourcePathSegments(propertyValueCache, rootSegment.SubProperties, propertyValueCache.EntryProperties);
        }

        /// <summary>
        /// Cache the property and collection item enumerations needed in EPM mappings. We do this for syndication and custom
        /// mappings.
        /// </summary>
        /// <param name="valueCache">The property value cache to use for caching.</param>
        /// <param name="segments">The source path segments to cache.</param>
        /// <param name="properties">The <see cref="ODataProperty"/> values to compute the segments against.</param>
        private static void CacheEpmSourcePathSegments(EpmValueCache valueCache, List<EpmSourcePathSegment> segments, IEnumerable<ODataProperty> properties)
        {
            Debug.Assert(valueCache != null, "valueCache != null");
            Debug.Assert(segments != null, "segments != null");

            if (properties == null)
            {
                return;
            }

            foreach (EpmSourcePathSegment segment in segments)
            {
                if (segment.EpmInfo == null)
                {
                    // No EPM info means that this is a complex value (cannot be mapped directly); cache it.
                    ODataComplexValue complexValue;
                    if (TryGetPropertyValue<ODataComplexValue>(properties, segment.PropertyName, out complexValue))
                    {
                        // Cache the properties of the complex property
                        IEnumerable<ODataProperty> subProperties = valueCache.CacheComplexValueProperties(complexValue);
                        CacheEpmSourcePathSegments(valueCache, segment.SubProperties, subProperties);
                    }
                }
                else
                {
                    // Primitive collection item inside a complex value; nothing to do - we cached the property values already.
                    Debug.Assert(segment.SubProperties.Count == 0, "No sub-segments expected.");
                }
            }
        }

        /// <summary>
        /// Gets the property value as the requested type.
        /// </summary>
        /// <typeparam name="T">The expected type of the property value.</typeparam>
        /// <param name="properties">The properties to search.</param>
        /// <param name="propertyName">The name of the property to get the value for.</param>
        /// <param name="propertyValue">The property value as <typeparamref name="T"/> or null if no property 
        /// with name <paramref name="propertyName"/> or with the expected type exists.</param>
        /// <returns>true if a property of the expected type was found; otherwise false.</returns>
        private static bool TryGetPropertyValue<T>(IEnumerable<ODataProperty> properties, string propertyName, out T propertyValue) where T : class
        {
            Debug.Assert(properties != null, "properties != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            propertyValue = null;

            ODataProperty property = properties.Where(p => string.CompareOrdinal(p.Name, propertyName) == 0).FirstOrDefault();
            if (property != null)
            {
                // Ignore cases where we don't get a property of the expected type;
                // we will fail later on with a better error message.
                propertyValue = property.Value as T;
                return propertyValue != null || property.Value == null;
            }

            return false;
        }
    }
}
