//---------------------------------------------------------------------
// <copyright file="ExpectedPayloadTypeResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Response verifier which ensures the payload type of the response is correct
    /// </summary>
    public class ExpectedPayloadTypeResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        /// <summary>
        /// Returns true for all requests
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the observed status code is not 204 (No content) and is not an error status code
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return response.StatusCode != HttpStatusCode.NoContent && !response.StatusCode.IsError();
        }

        /// <summary>
        /// Verifies the response payload type is consistent with the request uri
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            var expectedPayloadType = request.Uri.GetExpectedPayloadType();

            // Posting to entity set expects entity instance in response, but posting to service operation response should be consistent with its definition
            if (request.Verb == HttpVerb.Post && request.Uri.IsEntitySet() && (!request.Uri.IsWebInvokeServiceOperation()) && (!request.Uri.IsAction()))
            {
                expectedPayloadType = ODataPayloadElementType.EntityInstance;
            }

            string message = null;
            if (response.RootElement.ElementType != expectedPayloadType)
            {
                message = @"Response payload type did not match. 
Expected: '{0}'
Actual:   '{1}'";
                message = string.Format(CultureInfo.InvariantCulture, message, expectedPayloadType, response.RootElement.ElementType);
            }

            this.Assert(message == null, message, request, response);
        }
    }
}
