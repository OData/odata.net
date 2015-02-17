//---------------------------------------------------------------------
// <copyright file="IProtocolImplementationDetails.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for describing the protocol implementation details of an odata service
    /// </summary>
    [ImplementationSelector("ProtocolImplementationDetails", DefaultImplementation = "DataServices")]
    public interface IProtocolImplementationDetails
    {
        /// <summary>
        /// Gets the expected options for the given content type and version
        /// </summary>
        /// <param name="contentType">The content type of the payload</param>
        /// <param name="version">The current version</param>
        /// <param name="payloadUri">The payload URI</param>
        /// <returns>The payload options for the given content type and version</returns>
        ODataPayloadOptions GetExpectedPayloadOptions(string contentType, DataServiceProtocolVersion version, ODataUri payloadUri);
    }
}
