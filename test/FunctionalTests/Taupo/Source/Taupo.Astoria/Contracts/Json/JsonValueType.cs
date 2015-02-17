//---------------------------------------------------------------------
// <copyright file="JsonValueType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    /// <summary>
    /// Specifies the type of JsonValue.
    /// </summary>
    public enum JsonValueType
    {
        /// <summary>
        /// Json Primitive
        /// </summary>
        JsonPrimitiveValue,

        /// <summary>
        /// Json Array
        /// </summary>
        JsonArray,

        /// <summary>
        /// Json Object
        /// </summary>
        JsonObject,

        /// <summary>
        /// Json Property
        /// </summary>
        JsonProperty,
    }
}
