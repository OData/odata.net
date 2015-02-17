//---------------------------------------------------------------------
// <copyright file="IPayloadElementToJsonConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting a reach payload element representation into Json representation.
    /// </summary>
    [ImplementationSelector("PayloadElementToJsonConverter", DefaultImplementation = "Default", HelpText = "The converter from a reach payload element representation to an Json representation.")]    
    public interface IPayloadElementToJsonConverter
    {
        /// <summary>
        /// Converts the given payload element into an Json representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>  
        /// <returns>The the payload string representation</returns>
        string ConvertToJson(ODataPayloadElement rootElement);
    }
}
