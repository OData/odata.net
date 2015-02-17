//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a component that calculates the protocol version based on the payload provided
    /// </summary>
    [ImplementationSelector("ODataPayloadElementVersionCalculator", DefaultImplementation = "Default")]
    public interface IODataPayloadElementVersionCalculator
    {
        /// <summary>
        /// Calculates the ProtocolVersion based on the payload that is provided
        /// </summary>
        /// <param name="payloadElement">Payload element</param>
        /// <param name="contentType">Content Type of the payload</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Data Service Protocol Version</returns>
        DataServiceProtocolVersion CalculateProtocolVersion(ODataPayloadElement payloadElement, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion);
    }
}