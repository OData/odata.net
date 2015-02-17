//---------------------------------------------------------------------
// <copyright file="IODataRequestVersionResourceErrorCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Component that Calculates the version based on the ODataUri provided
    /// </summary>
    [ImplementationSelector("ODataRequestVersionResourceErrorCalculator", DefaultImplementation = "Default")]
    public interface IODataRequestVersionResourceErrorCalculator
    {
        /// <summary>
        /// Calculates the ResourceStringInformation based on the ODataRequest and maxProtocol version to determine if this i
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <param name="expectedErrorMessage">Calculated Version Error information</param>
        /// <returns>boolean value of if a Error ResourceString Information was calculated or not</returns>
        bool TryCalculateError(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion, out ExpectedErrorMessage expectedErrorMessage);
    }
}