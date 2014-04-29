//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to deal with EDM values over OData OM instances.
    /// </summary>
    internal static class ODataEdmValueUtils
    {
        /// <summary>
        /// Converts an <see cref="ODataProperty"/> into the corresponding <see cref="IEdmPropertyValue"/>.
        /// </summary>
        /// <param name="property">The non-null <see cref="ODataProperty"/> to convert.</param>
        /// <param name="declaringType">The declaring type of the property.</param>
        /// <returns>An <see cref="IEdmPropertyValue"/> implementation of the <paramref name="property"/> value.</returns>
        internal static IEdmPropertyValue GetEdmPropertyValue(this ODataProperty property, IEdmStructuredTypeReference declaringType)
        {
            Debug.Assert(property != null, "property != null");

            IEdmTypeReference propertyType = null;
            if (declaringType != null)
            {
                IEdmProperty edmProperty = declaringType.FindProperty(property.Name);
                if (edmProperty == null && !declaringType.IsOpen())
                {
                    throw new ODataException(ODataErrorStrings.ODataEdmStructuredValue_UndeclaredProperty(property.Name, declaringType.FullName()));
                }

                propertyType = edmProperty == null ? null : edmProperty.Type;
            }

            return new EdmPropertyValue(property.Name, ConvertValue(property.Value, propertyType).Value);
        }

        /// <summary>
        /// Converts an OData value into the corresponding <see cref="IEdmDelayedValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="type">The <see cref="IEdmTypeReference"/> of the value or null if no type reference is available.</param>
        /// <returns>An <see cref="IEdmDelayedValue"/> implementation of the <paramref name="value"/>.</returns>
        internal static IEdmDelayedValue ConvertValue(object value, IEdmTypeReference type)
        {
            if (value == null)
            {
                return type == null ? ODataEdmNullValue.UntypedInstance : new ODataEdmNullValue(type);
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                return new ODataEdmStructuredValue(complexValue);
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return new ODataEdmCollectionValue(collectionValue);
            }

            // If the property value is not null, a complex value or a collection value, 
            // it has to be a primitive value
            return EdmValueUtils.ConvertPrimitiveValue(value, type == null ? null : type.AsPrimitive());
        }
    }
}
