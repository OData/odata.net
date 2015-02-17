//---------------------------------------------------------------------
// <copyright file="PreferHeaderVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The response verifier for the 'prefer' and 'preference-applied' HTTP headers
    /// </summary>
    public class PreferHeaderVerifier : ResponseVerifierBase
    {
        private static readonly string[] validPreferValues = new[] { HttpHeaders.ReturnContent, HttpHeaders.ReturnNoContent };
        private DataServiceProtocolVersion maxProtocolVersion;

        /// <summary>
        /// Initializes a new instance of the PreferHeaderVerifier class
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        internal PreferHeaderVerifier(DataServiceProtocolVersion maxProtocolVersion)
            : base()
        {
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Verifies response to request with prefer header 
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            string preferAppliedHeaderValue;
            string preferHeaderValue = null;
            bool preferIsValid = request.Headers.TryGetValue(HttpHeaders.Prefer, out preferHeaderValue) && validPreferValues.Contains(preferHeaderValue);

            // if the prefer header is present, valid, and applies to the request...
            if (preferIsValid && request.PreferHeaderApplies(this.maxProtocolVersion))
            {
                // ensure the preference-applied header is there
                this.Assert(
                    response.Headers.TryGetValue(HttpHeaders.PreferenceApplied, out preferAppliedHeaderValue),
                    "Missing Preference-Applied header in the response",
                    request,
                    response);

                // ensure the value of preference-applied matches the prefer header sent
                this.Assert(
                    preferHeaderValue == preferAppliedHeaderValue,
                    "The Preference-Applied Header value on response is different from request",
                    request,
                    response);

                // verify whether the content matches the preference
                if (preferAppliedHeaderValue.Equals(HttpHeaders.ReturnContent))
                {
                    // if the URI refers to a property value, it may be empty if the value is empty
                    // we will verify elsewhere that the value matches the expected property value
                    if (!request.Uri.IsPropertyValue())
                    {
                        this.AssertResponseIsNonEmpty(request, response);
                    }
                }
                else if (preferAppliedHeaderValue.Equals(HttpHeaders.ReturnNoContent))
                {
                    this.AssertResponseIsEmpty(request, response);
                }
            }
            else
            {
                // no preference-applied header should be present
                this.Assert(
                    !response.Headers.TryGetValue(HttpHeaders.PreferenceApplied, out preferAppliedHeaderValue),
                    "Unexpected Preference-Applied header in the response",
                    request,
                    response);

                // the content should match the default behavior
                if (request.PreferHeaderApplies(this.maxProtocolVersion))
                {
                    if (request.GetEffectiveVerb() == HttpVerb.Post)
                    {
                        this.AssertResponseIsNonEmpty(request, response);
                    }
                    else if (request.Uri.IsEntity())
                    {
                        this.AssertResponseIsEmpty(request, response);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the value of Content-Length Header in the response
        /// </summary>
        /// <param name="response">The response to verify</param>
        /// <returns>Returns true if the content-length header is 0 or it doesnt exist</returns>
        internal static bool ResponseIsEmpty(ODataResponse response)
        {
            string contentLength;
            if (response.Headers.TryGetValue(HttpHeaders.ContentLength, out contentLength) && contentLength != "0")
            {
                return false;
            }

            if (response.Body != null && response.Body.Length != 0)
            {
                return false;
            }

            return true;
        }

        private void AssertResponseIsEmpty(ODataRequest request, ODataResponse response)
        {
            this.Assert(ResponseIsEmpty(response), "Response unexpectedly non-empty", request, response);
        }

        private void AssertResponseIsNonEmpty(ODataRequest request, ODataResponse response)
        {
            this.Assert(!ResponseIsEmpty(response), "Response unexpectedly empty", request, response);
        }
    }
}
