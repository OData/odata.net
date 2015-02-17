//---------------------------------------------------------------------
// <copyright file="IJsonPrimitiveConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An json-specific contract for converting primitives
    /// </summary>
    [ImplementationSelector("OData/JsonPrimitiveConverter", DefaultImplementation = "Default")]
    public interface IJsonPrimitiveConverter
    {
        /// <summary>
        /// Converts a primitive value represented as a string in json back into the clr representation
        /// </summary>
        /// <param name="valueToConvert">The value to convert</param>
        /// <param name="expectedType">The expected type. Must be guid, datetime, or binary</param>
        /// <param name="convertedValue">The converted value</param>
        /// <returns>True if conversion was possible, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Cannot know the type at compile time")]
        bool TryConvertFromString(string valueToConvert, Type expectedType, out object convertedValue);

        /// <summary>
        /// Serializes the given primitive into a string
        /// </summary>
        /// <param name="value">The primitive</param>
        /// <returns>The primitive serialized as a string</returns>
        string SerializePrimitive(object value);
        
        /// <summary>
        /// Normalize DateTime and DateTimeOffset values
        /// </summary>
        /// <param name="value">Value to normalize</param>
        /// <returns>Normalized DateTime values</returns>
        object Normalize(object value);
    }
}