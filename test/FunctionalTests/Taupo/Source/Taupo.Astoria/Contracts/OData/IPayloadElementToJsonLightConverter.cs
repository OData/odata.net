//---------------------------------------------------------------------
// <copyright file="IPayloadElementToJsonLightConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting a reach payload element representation into Json representation.
    /// </summary>
    [ImplementationSelector("PayloadElementToJsonLightConverter", DefaultImplementation = "Default", HelpText = "The converter from a reach payload element representation to a Json Light representation.")]    
    public interface IPayloadElementToJsonLightConverter
    {
        /// <summary>
        /// Converts the given payload element into a Json Light representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>  
        /// <returns>The the payload string representation</returns>
        string ConvertToJsonLight(ODataPayloadElement rootElement);

        /// <summary>
        /// Converts the given payload element into a Json Light object model representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json Light object model representation of the payload.</returns>
        JsonValue ConvertToJsonLightValue(ODataPayloadElement rootElement);
    }
}
