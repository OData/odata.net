//---------------------------------------------------------------------
// <copyright file="IResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Contract for verifying OData request/response pairs
    /// </summary>
    public interface IResponseVerifier
    {
        /// <summary>
        /// Verifies the given OData request/response pair
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        void Verify(ODataRequest request, ODataResponse response);
    }
}
