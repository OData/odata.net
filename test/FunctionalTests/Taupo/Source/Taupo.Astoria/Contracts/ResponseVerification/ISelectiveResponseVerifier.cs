//---------------------------------------------------------------------
// <copyright file="ISelectiveResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// A contract for response verifiers that only apply to certain types of requests or responses
    /// </summary>
    public interface ISelectiveResponseVerifier : IResponseVerifier
    {
        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given request
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>True if it applies, false otherwise</returns>
        bool Applies(ODataRequest request);

        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>True if it applies, false otherwise</returns>
        bool Applies(ODataResponse response);
    }
}
