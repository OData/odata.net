//---------------------------------------------------------------------
// <copyright file="IODataLiteralConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for serializing/deserializing primitive values according to the rules for literals in uris, etags, and skip-tokens
    /// </summary>
    [ImplementationSelector("ODataLiteralConverter", DefaultImplementation = "Default")]
    public interface IODataLiteralConverter
    {
        /// <summary>
        /// Converts the given value to a string according to the OData literal formatting rules
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="capitalizeIdentifiers">if set to <c>true</c> then identifiers for decimal, double, etc should be capitalized.</param>
        /// <returns>
        /// The serialized value
        /// </returns>
        string SerializePrimitive(object value, bool capitalizeIdentifiers);

        /// <summary>
        /// Converts the given string to a value according to the OData literal formatting rules for the given type
        /// </summary>
        /// <param name="value">The serialized value</param>
        /// <param name="targetType">The expected type</param>
        /// <returns>The deserialized value</returns>
        object DeserializePrimitive(string value, Type targetType);

        /// <summary>
        /// Returns whether the given type is a literal type
        /// </summary>
        /// <param name="targetType">The type to check</param>
        /// <returns>True if it is a literal type, false otherwise</returns>
        bool IsLiteralType(Type targetType);
    }
}