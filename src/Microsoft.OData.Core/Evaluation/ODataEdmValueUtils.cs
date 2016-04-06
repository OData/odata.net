//---------------------------------------------------------------------
// <copyright file="ODataEdmValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

<<<<<<< HEAD
namespace Microsoft.OData.Evaluation
=======
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Core.Evaluation
>>>>>>> Move Microsoft.OData.Edm.Values and Microsoft.OData.Edm.Library.Values to
{
    #region Namespaces

    using System.Diagnostics;
    using Microsoft.OData.Edm;
<<<<<<< HEAD
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    using ODataErrorStrings = Microsoft.OData.Strings;
=======
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
>>>>>>> Move Microsoft.OData.Edm.Values and Microsoft.OData.Edm.Library.Values to

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
