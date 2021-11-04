//---------------------------------------------------------------------
// <copyright file="HttpDrivenSaveChangesVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Verifier for the DataServiceContext.SaveChanges method.
    /// </summary>
    [ImplementationName(typeof(ISaveChangesVerifier), "HttpDriven")]
    public class HttpDrivenSaveChangesVerifier : SaveChangesVerifierBase
    {
        private IList<KeyValuePair<HttpRequestData, HttpResponseData>> httpLog;

        /// <summary>
        /// Gets or sets the http tracker to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextHttpTracker HttpTracker { get; set; }

        /// <summary>
        /// Gets or sets the http validating SaveChanges-emulator to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISaveChangesHttpValidatingEmulator Emulator { get; set; }

        /// <summary>
        /// Initializes the verifier's state based on its inputs
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void InitializeState(IAsyncContinuation continuation)
        {
            this.httpLog = new List<KeyValuePair<HttpRequestData, HttpResponseData>>();
            base.InitializeState(continuation);
        }

        /// <summary>
        /// Called immediatlely before the product API call
        /// </summary>
        protected override void SetupBeforeProductCall()
        {
            base.SetupBeforeProductCall();
            this.HttpTracker.RegisterHandler(this.Input.Context, this.HandleRequestResponsePair);
            this.DescriptorDataChangeTracker.IgnoreAllUpdates = true;
        }

        /// <summary>
        /// Called immediately after the product API call on both success and failure
        /// </summary>
        protected override void CleanupAfterProductCall()
        {
            base.CleanupAfterProductCall();
            this.HttpTracker.UnregisterHandler(this.Input.Context, this.HandleRequestResponsePair, this.State.Response != null);
            this.DescriptorDataChangeTracker.IgnoreAllUpdates = false;
        }

        /// <summary>
        /// Verifies the requests sent by the context
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void VerifyRequests(IAsyncContinuation continuation)
        {
            this.State.ExpectedResponse = this.Emulator.ValidateAndTrackChanges(this.Input.ContextData, this.State.CachedPropertyValuesBeforeSave, this.State.UsedOptions, this.httpLog);
            continuation.Continue();
        }

        private void HandleRequestResponsePair(DataServiceContext context, HttpRequestData request, HttpResponseData response)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");

            this.Assert.AreSame(this.Input.Context, context, "Handle request/response pair called with unexpected context");

            this.httpLog.Add(new KeyValuePair<HttpRequestData, HttpResponseData>(request, response));
        }
    }
}