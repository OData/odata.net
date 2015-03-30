//---------------------------------------------------------------------
// <copyright file="IPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.PrimitivePayloadValueConverters
{
    /// <summary>
    /// Class for defining a primitive payload value conversion for a type definition.
    /// Suppose a type definition defines a primitive type X (underlying type) for a read Json payload type Y,
    /// the ConvertToPayloadValue method converts value from X to Y
    /// and the ConvertFromUnderlyingType method converts value from Y to X.
    /// </summary>
    public interface IPrimitivePayloadValueConverter
    {
        /// <summary>
        /// Converts the given primitive value defined in a type definition from the payload object.
        /// </summary>
        /// <param name="value">The given CLR value.</param>
        /// <param name="primitiveTypeReference">The given Primitive Type Reference.</param>
        /// <returns>The converted payload value of the underlying type.</returns>
        object ConvertToPayloadValue(object value, IEdmPrimitiveTypeReference primitiveTypeReference, ODataMessageWriterSettings messageWriterSettings);

        /// <summary>
        /// Converts the given payload value to the type defined in a type definition.
        /// </summary>
        /// <param name="value">The given payload value.</param>
        /// <param name="primitiveTypeReference">The given Primitive Type Reference.</param>
        /// <returns>The converted value of the type.</returns>
         object ConvertFromPayloadValue(object value, IEdmPrimitiveTypeReference primitiveTypeReference, ODataMessageReaderSettings messageReaderSettings);
    }
}
