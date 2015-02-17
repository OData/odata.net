//---------------------------------------------------------------------
// <copyright file="DelegateBasedResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Selective response verifier implementation based on delegates provided at construction time
    /// </summary>
    public class DelegateBasedResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private Func<ODataRequest, bool> appliesToRequest;
        private Func<ODataResponse, bool> appliesToResponse;
        private Action<ODataRequest, ODataResponse> verify;

        /// <summary>
        /// Initializes a new instance of the DelegateBasedResponseVerifier class
        /// </summary>
        /// <param name="verify">The delegate to use for verifying requests</param>
        public DelegateBasedResponseVerifier(Action<ODataRequest, ODataResponse> verify)
            : this(verify, r => true, r => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DelegateBasedResponseVerifier class
        /// </summary>
        /// <param name="verify">The delegate to use for verifying requests</param>
        /// <param name="appliesToRequest">The delegate to use for deciding whether to verify a request</param>
        /// <param name="appliesToResponse">The delegate to use for deciding whether to verify a response</param>
        public DelegateBasedResponseVerifier(Action<ODataRequest, ODataResponse> verify, Func<ODataRequest, bool> appliesToRequest, Func<ODataResponse, bool> appliesToResponse)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(verify, "verify");
            ExceptionUtilities.CheckArgumentNotNull(appliesToRequest, "appliesToRequest");
            ExceptionUtilities.CheckArgumentNotNull(appliesToResponse, "appliesToResponse");

            this.verify = verify;
            this.appliesToRequest = appliesToRequest;
            this.appliesToResponse = appliesToResponse;
        }

        /// <summary>
        /// Verifies the given OData request/response pair using the delegate given at construction time.
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            try
            {
                this.verify(request, response);
            }
            catch (ResponseVerificationException e)
            {
                // wrap and re-throw to preserve original call-stack. Ideally we would just let these through.
                throw new ResponseVerificationException(e);
            }
            catch (Exception e)
            {
                this.ReportFailure(request, response);
                throw new ResponseVerificationException(e);
            }
        }

        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given request using the delegate given at construction.
        /// If no delegate was given, the verifier applies to all requests.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>True if it applies, false otherwise</returns>
        public bool Applies(ODataRequest request)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            return this.appliesToRequest(request);
        }

        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given response using the delegate given at construction.
        /// If no delegate was given, the verifier applies to all responses.
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>True if it applies, false otherwise</returns>
        public bool Applies(ODataResponse response)
        {
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            return this.appliesToResponse(response);
        }
    }
}
