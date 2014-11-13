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

namespace Microsoft.Data.OData.Evaluation
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Spatial;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Values;
    using Microsoft.Data.Edm.Values;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();

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
