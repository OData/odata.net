//---------------------------------------------------------------------
// <copyright file="IXmlPrimitiveConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An xml-specific contract for converting primitives
    /// </summary>
    [ImplementationSelector("OData/XmlPrimitiveConverter", DefaultImplementation = "Default")]
    public interface IXmlPrimitiveConverter
    {
        /// <summary>
        /// Serializes the given primitive into a string
        /// </summary>
        /// <param name="value">The primitive</param>
        /// <returns>The primitive serialized as a string</returns>
        string SerializePrimitive(object value);

        /// <summary>
        /// Deserializes the given string into a primitive
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="expectedType">The expected type</param>
        /// <returns>The deserialized primitive</returns>
        object DeserializePrimitive(string value, Type expectedType);

        /// <summary>
        /// Convert a datetime primitive value to atom datetime offset format
        /// </summary>
        /// <param name="value">DateTime clr value</param>
        /// <returns>string format of atom datetime offset</returns>
        string ConvertToAtomDateTimeOffset(DateTime value);

        /// <summary>
        /// Convert a datetimeoffse primitive value to atom datetime offset format
        /// </summary>
        /// <param name="value">DateTimeOffset clr value</param>
        /// <returns>string format of atom datetime offset</returns>
        string ConvertToAtomDateTimeOffset(DateTimeOffset value);
    }
}