//---------------------------------------------------------------------
// <copyright file="QueryResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A response verifier that compares the results of a GET request against the expected values
    /// </summary>
    public class QueryResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private DataServiceProtocolVersion maxProtocolVersion;
        
        /// <summary>
        /// Initializes a new instance of the QueryResponseVerifier class
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        internal QueryResponseVerifier(DataServiceProtocolVersion maxProtocolVersion)
            : base()
        {
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Gets or sets the uri evaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the response verification services
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationServices VerificationServices { get; set; }

        /// <summary>
        /// Returns true if this is a query-like GET request
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            if (request.Verb != HttpVerb.Get)
            {
                return false;
            }

            if (request.Uri.IsBatch() || request.Uri.IsMetadata() || request.Uri.IsServiceDocument() || request.Uri.IsMediaResource())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the observed status code is 200 (OK)
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Processes the request's uri, executes the query against the evaluator, and validates the results
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Must catch all exceptions to wrap them in response verification exceptions")]
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            try
            {
                var expected = this.Evaluator.Evaluate(request.Uri);
                this.VerificationServices.ValidateResponsePayload(request.Uri, response, expected, this.maxProtocolVersion);
            }
            catch (Exception e)
            {
                this.ReportFailure(request, response);
                throw new ResponseVerificationException(e);
            }
        }
    }
}
