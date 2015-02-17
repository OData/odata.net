//---------------------------------------------------------------------
// <copyright file="NoSniffHeaderVerifier.cs" company="Microsoft">
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
    /// The response verifier for the 'X-Content-Type-Options' HTTP header
    /// </summary>
    public class NoSniffHeaderVerifier : ResponseVerifierBase
    {
        private const string NoSniffValue = "nosniff";

        /// <summary>
        /// Initializes a new instance of the NoSniffHeaderVerifier class
        /// </summary>
        internal NoSniffHeaderVerifier()
            : base()
        {
        }

        /// <summary>
        /// Verifies response has 'X-Content-Type-Options' HTTP header
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            string noSniffResponseHeaderValue = null;
            bool noSniffExists = response.Headers.TryGetValue(HttpHeaders.XContentTypeOptions, out noSniffResponseHeaderValue);
            this.Assert(noSniffExists, HttpHeaders.XContentTypeOptions + " header was not found in the response", request, response);
            this.Assert(NoSniffValue.Equals(noSniffResponseHeaderValue), HttpHeaders.XContentTypeOptions + " header contained incorrect value " + noSniffResponseHeaderValue, request, response);
        }
    }
}
