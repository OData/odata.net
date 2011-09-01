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

            string textPropertyValue;
            bool preserveWhitespace;
            if (!AtomValueUtils.TryConvertPrimitiveToString(propertyValue, out textPropertyValue, out preserveWhitespace))
            {
                throw new ODataException(Strings.AtomValueUtils_CannotConvertValueToAtomPrimitive(propertyValue.GetType().FullName));
            }

            return textPropertyValue;
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
    }
}
