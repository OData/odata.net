//---------------------------------------------------------------------
// <copyright file="IODataUriVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Component that Calculates the version based on the ODataUri provided
    /// </summary>
    [ImplementationSelector("ODataUriVersionCalculator", DefaultImplementation = "Default")]
    public interface IODataUriVersionCalculator
    {
        /// <summary>
        /// Calculates the version based on the ODataUri provided
        /// </summary>
        /// <param name="uri">ODataUri to use</param>
        /// <param name="contentType">Content type of the results of the uri</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="dataServiceVersion">The data service version of the request</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Data Service Protocol Version</returns>
        DataServiceProtocolVersion CalculateProtocolVersion(ODataUri uri, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion dataServiceVersion, DataServiceProtocolVersion maxDataServiceVersion);
    }
}