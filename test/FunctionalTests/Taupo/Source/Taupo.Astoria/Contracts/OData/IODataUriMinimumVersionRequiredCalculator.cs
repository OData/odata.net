//---------------------------------------------------------------------
// <copyright file="IODataUriMinimumVersionRequiredCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Component that Calculates the version based on the ODataUri provided
    /// </summary>
    [ImplementationSelector("ODataUriMinimumVersionRequiredCalculator", DefaultImplementation = "Default")]
    public interface IODataUriMinimumVersionRequiredCalculator
    {
        /// <summary>
        /// Calculates the version based on the ODataUri provided
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns>Data Service Protocol Version</returns>
        DataServiceProtocolVersion CalculateMinRequestVersion(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion);

        /// <summary>
        /// Calculates the version based on the ODataUri provided
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns>Data Service Protocol Version</returns>
        DataServiceProtocolVersion CalculateMinResponseVersion(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion);
    }
}