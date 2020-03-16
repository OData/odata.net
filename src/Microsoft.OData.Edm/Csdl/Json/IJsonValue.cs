//---------------------------------------------------------------------
// <copyright file="IJsonValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Defines JSON Value types.
    /// </summary>
    public enum JsonValueKind
    {
        /// <summary>
        /// Represents a Primitive JSON value kind. Primitive value could be:
        /// null, boolean, string, DateTimeOffset, Int32, Double, etc.
        /// </summary>
        JPrimitive,

        /// <summary>
        /// Represents an Object JSON value kind.
        /// An object JSON is a wrapper of a key/value pair.
        /// </summary>
        JObject,

        /// <summary>
        /// Represents an Array JSON value kind.
        /// An array JSON is a collection of other JSON value.
        /// </summary>
        JArray
    }

    /// <summary>
    /// Represents the definition of a JSON value.
    /// </summary>
    public interface IJsonValue
    {
        /// <summary>
        /// Gets the kind of this JSON value.
        /// </summary>
        JsonValueKind ValueKind { get; }
    }
}
