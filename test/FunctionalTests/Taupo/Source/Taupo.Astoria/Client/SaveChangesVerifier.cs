//---------------------------------------------------------------------
// <copyright file="SaveChangesVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Verifier for the DataServiceContext.SaveChanges method.
    /// </summary>
    [ImplementationName(typeof(ISaveChangesVerifier), "WithoutTestHooks")]
    public class SaveChangesVerifier : SaveChangesVerifierBase
    {
        private EventLogger<DSClient.SendingRequest2EventArgs> sendingRequestLog;

        /// <summary>
        /// Gets or sets the request calculator.
        /// </summary>
        /// <value>The request calculator.</value>
        [InjectDependency(IsRequired = true)]
        public ISaveChangesRequestCalculator RequestCalculator { get; set; }

        /// <summary>
        /// Gets or sets the requests verifier.
        /// </summary>
        /// <value>The requests verifier.</value>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextRequestsVerifier RequestsVerifier { get; set; }

        /// <summary>
        /// Gets or sets the response calculator.
        /// </summary>
        /// <value>The response calculator.</value>
        [InjectDependency(IsRequired = true)]
        public ISaveChangesResponseCalculator ResponseCalculator { get; set; }

        /// <summary>
        /// Gets or sets the XML converter.
        /// </summary>
        /// <value>The XML converter.</value>
        [InjectDependency]
        public IXmlToPayloadElementConverter XmlConverter { get; set; }

        /// <summary>
        /// Gets or sets the calculator to use for entity descriptor values
        /// </summary>
        [InjectDependency]
        public IEntityDescriptorValueCalculator EntityDescriptorCalculator { get; set; }

        /// <summary>
        /// Initializes the verifier's state based on its inputs
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void InitializeState(IAsyncContinuation continuation)
        {
            this.sendingRequestLog = new EventLogger<DSClient.SendingRequest2EventArgs>();

            base.InitializeState(continuation);
        }

        /// <summary>
        /// Called immediatlely before the product API call
        /// </summary>
        protected override void SetupBeforeProductCall()
        {
            base.SetupBeforeProductCall();

            this.DescriptorDataChangeTracker.ApplyUpdatesImmediately = false;

            // Attach event handlers
            this.Input.Context.SendingRequest2 += this.sendingRequestLog.LogEvent;
            this.State.ExpectedResponse = this.ResponseCalculator.CalculateSaveChangesResponseData(this.Input.ContextData, this.State.UsedOptions, this.Input.Context);
        }

        /// <summary>
        /// Called immediately after the product API call on both success and failure
        /// </summary>
        protected override void CleanupAfterProductCall()
        {
            base.CleanupAfterProductCall();

            this.DescriptorDataChangeTracker.ApplyPendingUpdates();
            this.DescriptorDataChangeTracker.ApplyUpdatesImmediately = true;

            // remove event handlers
            this.Input.Context.SendingRequest2 -= this.sendingRequestLog.LogEvent;
        }

        /// <summary>
        /// Called after the product API call if it succeeded
        /// </summary>
        protected override void OnProductCallSuccess()
        {
            base.OnProductCallSuccess();
            this.Input.ContextData.TrackSaveChanges(this.State.UsedOptions, this.State.Response, this.State.EnumeratedOperationResponses, this.DescriptorDataChangeTracker);
        }

        /// <summary>
        /// Verifies the requests sent by the context
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void VerifyRequests(IAsyncContinuation continuation)
        {
            if (this.State.UsedOptions != SaveChangesOptions.Batch)
            {
                var expectedRequests = this.RequestCalculator.CalculateSaveChangesRequestData(this.State.ContextDataBeforeChanges, this.Input.Context, this.State.UsedOptions, this.State.EnumeratedOperationResponses);
                var requestsSent = this.sendingRequestLog.Events.Select(e => e.Value).ToList();
                this.RequestsVerifier.VerifyRequests(expectedRequests, requestsSent);
            }

            continuation.Continue();
        }
    }
}
