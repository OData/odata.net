//---------------------------------------------------------------------
// <copyright file="JsonObjectValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Represents a JSON object value.
    /// A json object value likes:
    /// {
    ///    ...
    /// }
    /// The JSON object includes properties, who is a key/IJsonValue pairs.
    /// </summary>
    internal class JsonObjectValue : Dictionary<string, IJsonValue>, IJsonValue
    {
        /// <summary>
        /// Gets the kind of this JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.JObject;
    }
}
