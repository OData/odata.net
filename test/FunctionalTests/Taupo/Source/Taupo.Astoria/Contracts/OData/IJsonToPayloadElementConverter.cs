//---------------------------------------------------------------------
// <copyright file="IJsonToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting an Json to a reach payload element representation.
    /// </summary>
    [ImplementationSelector("JsonToPayloadElementConverter", DefaultImplementation = "Default", HelpText = "The converter from an Json to a reach payload element representation.")] 
    public interface IJsonToPayloadElementConverter
    {
        /// <summary>
        /// Converts the given Json element into a rich payload element representation.
        /// </summary>
        /// <param name="jsonValue">Json element to convert.</param>
        /// <returns>A payload element representing the given element.</returns>
        ODataPayloadElement ConvertToPayloadElement(JsonValue jsonValue);
    }
}
