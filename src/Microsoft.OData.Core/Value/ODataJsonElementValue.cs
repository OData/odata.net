﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonElementValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents a <see cref="JsonElement"/> value.
    /// Useful for writingparsed  JSON inputs directly without reading them first, which would result in double allocations.
    /// </summary>
    public sealed class ODataJsonElementValue : ODataValue
    {
        /// <summary>
        /// Creates an intsance of <see cref="ODataJsonElementValue"/> based
        /// on the specified <see cref="JsonElement"/> value.
        /// </summary>
        /// <param name="value">The <see cref="JsonElement"/> to wrap.</param>
        /// <remarks>
        /// The caller is responsible for ensuring the contents of the <see cref="JsonElement"/> are valid.
        /// </remarks>
        public ODataJsonElementValue(JsonElement value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the underlying <see cref="JsonElement"/> wrapped by this
        /// <see cref="ODataJsonElementValue"/>.
        /// </summary>
        public JsonElement Value { get; private set; }
    }
}
#endif