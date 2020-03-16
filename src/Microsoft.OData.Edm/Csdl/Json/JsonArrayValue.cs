//---------------------------------------------------------------------
// <copyright file="JsonArrayValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Represents a JSON array value.
    /// A json object value likes:
    /// [
    ///    ...
    /// ]
    /// The JSON array includes list of other IJsonValue.
    /// </summary>
    internal class JsonArrayValue : List<IJsonValue>, IJsonValue
    {
        /// <summary>
        /// Gets the kind of this JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.JArray;
    }
}
