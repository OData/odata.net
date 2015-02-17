//---------------------------------------------------------------------
// <copyright file="StatusCodeVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A response verifier that compares the HTTP status code of the response to an expected value
    /// </summary>
    public class StatusCodeVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        /// <summary>
        /// Initializes a new instance of the StatusCodeVerifier class
        /// </summary>
        /// <param name="expected">The expected status code</param>
        public StatusCodeVerifier(HttpStatusCode expected)
        {
            this.ExpectedStatusCode = expected;
        }

        /// <summary>
        /// Gets the expected status code
        /// </summary>
        public HttpStatusCode ExpectedStatusCode { get; private set; }

        /// <summary>
        /// Returns true if this is not action
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            // ActionResponseVerifier verifies action status code for assumed working scenario, other wise should expect error
            if (request.Uri.IsAction())
            {
                return ((int)this.ExpectedStatusCode) > 399; 
            }
            
            return true; 
        }

        /// <summary>
        /// Returns true for all responses
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return true;
        }

        /// <summary>
        /// Checks that the response's status code matches the expected value
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            this.Assert(response.StatusCode == this.ExpectedStatusCode, string.Format(CultureInfo.InvariantCulture, "Expected status code '{0}', observed '{1}'", this.ExpectedStatusCode, response.StatusCode), request, response);
            
            this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Status code was '{0}'", this.ExpectedStatusCode);
        }
    }
}
