//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Represents a JSON primitive value.
    /// A json primitive value likes: boolean, string, double, null, or others
    /// </summary>
    internal class JsonPrimitiveValue : IJsonValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPrimitiveValue"/> class.
        /// </summary>
        /// <param name="value">The wrapper value.</param>
        public JsonPrimitiveValue(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the wrapper value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the kind of this JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.JPrimitive;
    }
}
