//---------------------------------------------------------------------
// <copyright file="IResponseVerificationServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for performing common verification logic across verifiers
    /// </summary>
    [ImplementationSelector("ResponseVerificationServices", DefaultImplementation = "Default")]
    public interface IResponseVerificationServices
    {
        /// <summary>
        /// Validates the data in the response payload based on the expected query value
        /// </summary>
        /// <param name="requestUri">The request uri</param>
        /// <param name="response">The response</param>
        /// <param name="expected">The expected query value</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        void ValidateResponsePayload(ODataUri requestUri, ODataResponse response, QueryValue expected, DataServiceProtocolVersion maxProtocolVersion);
    }
}
