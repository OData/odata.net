//---------------------------------------------------------------------
// <copyright file="IDictionaryToJsonObjectConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting between dictionaries and json object representations
    /// </summary>
    [ImplementationSelector("DictionaryToJsonObjectConverter", DefaultImplementation = "Default")]
    public interface IDictionaryToJsonObjectConverter
    {
        /// <summary>
        /// Converts the specified dictionary into a json object.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>The converted object</returns>
        JsonObject Convert(IDictionary<string, object> dictionary);

        /// <summary>
        /// Converts the specified json object into a dictionary.
        /// </summary>
        /// <param name="jsonObject">The json object to convert.</param>
        /// <returns>The converted dictionary</returns>
        IDictionary<string, object> Convert(JsonObject jsonObject);
    }
}
