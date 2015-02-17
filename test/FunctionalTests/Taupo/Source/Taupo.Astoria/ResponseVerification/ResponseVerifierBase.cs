//---------------------------------------------------------------------
// <copyright file="ResponseVerifierBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A base class for response verifiers that provides useful helper methods and a logger
    /// </summary>
    public abstract class ResponseVerifierBase : IResponseVerifier
    {
        /// <summary>
        /// Initializes a new instance of the ResponseVerifierBase class
        /// </summary>
        protected ResponseVerifierBase()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Internal event raised when reporting failure. Primarily used for unit-testing.
        /// </summary>
        internal event Action<ODataRequest, ODataResponse> OnReportingFailure;

        /// <summary>
        /// Gets or sets the logger for this verifier
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }
        
        /// <summary>
        /// Verifies the given OData request/response pair
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public virtual void Verify(ODataRequest request, ODataResponse response)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            ExceptionUtilities.CheckObjectNotNull(this.Logger, "Cannot run verifier without logger");
        }

        /// <summary>
        /// Helper method for verifiers to report failure
        /// </summary>
        /// <param name="request">The request that failed verification</param>
        /// <param name="response">The response that failed verification</param>
        protected void ReportFailure(ODataRequest request, ODataResponse response)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");

            if (this.OnReportingFailure != null)
            {
                this.OnReportingFailure(request, response);
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Response verification failure");
            request.WriteToLog(this.Logger, LogLevel.Verbose);
            response.WriteToLog(this.Logger, LogLevel.Verbose);
        }

        /// <summary>
        /// Helper method for verifiers to make assertions
        /// </summary>
        /// <param name="condition">The condition to assert is true</param>
        /// <param name="message">The message to write to the log if the assertion fails</param>
        /// <param name="request">The request being verified</param>
        /// <param name="response">The response being verified</param>
        protected void Assert(bool condition, string message, ODataRequest request, ODataResponse response)
        {
            if (!condition)
            {
                ExceptionUtilities.CheckArgumentNotNull(message, "message");
                ExceptionUtilities.CheckArgumentNotNull(request, "request");
                ExceptionUtilities.CheckArgumentNotNull(response, "response");

                this.Logger.WriteLine(LogLevel.Error, message);
                this.ReportFailure(request, response);
                throw new ResponseVerificationException();
            }
        }
    }
}
