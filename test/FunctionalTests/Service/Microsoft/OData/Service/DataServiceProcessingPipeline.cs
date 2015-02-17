//---------------------------------------------------------------------
// <copyright file="DataServiceProcessingPipeline.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class declaring the events for the data service processing pipeline
    /// </summary>
    public sealed class DataServiceProcessingPipeline
    {
#if DEBUG
        // ***************************************************************************************
        // The debug code here is for basic assertions on call orders if the request is successful.
        // ***************************************************************************************

        /// <summary>
        /// Set to true if we have seen an exception in the current request or we are in the $metadata path
        /// </summary>
        /// <remarks>
        /// If the current request encounters an exception, there is no guarantee that all the
        /// events will be fired, we may skip the debug asserts when that happens.
        /// </remarks>
        internal bool SkipDebugAssert;

        /// <summary>
        /// Set to true if any of the service is disposed from WebUtil.Dispose()
        /// </summary>
        private bool HasDisposedProviderInterfaces;

        /// <summary>
        /// Number of times DataServiceOfT.OnStartProcessingRequest() is called.
        /// </summary>
        /// <remarks>OnStartProcessingRequest is called once per operation in a batch.
        /// This is different from ProcessingRequest, which is called once at the begining of the batch.</remarks>
        private int OnStartProcessingRequestInvokeCount;

        /// <summary>
        /// Number of times InvokeProcessingRequest() is called.
        /// </summary>
        /// <remarks>Note that in the batch scenario, ProcessingRequest is called once at the begining of the batch and not once per operation.</remarks>
        private int ProcessingRequestInvokeCount;

        /// <summary>
        /// Number of times InvokeProcessedRequest() is called.
        /// </summary>
        /// <remarks>Note that in the batch scenario, ProcessedRequest is called once at the end of the batch and not once per operation.</remarks>
        private int ProcessedRequestInvokeCount;

        /// <summary>
        /// Number of times InvokeProcessingChangeset() is called.
        /// </summary>
        /// <remarks>Note that in the non-batch scenario, PrcoessingChangeset/ProcessedChangeset is called at the begining/end of a CUD operation.</remarks>
        private int ProcessingChangesetInvokeCount;

        /// <summary>
        /// Number of times IUpdatable.SaveChanges() is called.
        /// </summary>
        private int SaveChangesInvokeCount;

        /// <summary>
        /// Number of times IDataServiceUpdateProvider2.InvokeServiceAction() is called.
        /// </summary>
        private int ServiceActionInvokeCount;
#endif

        /// <summary>
        /// Request start event
        /// </summary>
        public event EventHandler<DataServiceProcessingPipelineEventArgs> ProcessingRequest;

        /// <summary>
        /// Request end event
        /// </summary>
        public event EventHandler<DataServiceProcessingPipelineEventArgs> ProcessedRequest;

        /// <summary>
        /// Change set start event
        /// </summary>
        public event EventHandler<EventArgs> ProcessingChangeset;

        /// <summary>
        /// Change set end event
        /// </summary>
        public event EventHandler<EventArgs> ProcessedChangeset;

        /// <summary>
        /// Assert ProcessingPipeline state before any event has been fired
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertInitialDebugState()
        {
#if DEBUG
            Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
            Debug.Assert(this.OnStartProcessingRequestInvokeCount == 0, "this.OnStartProcessingRequestInvokeCount == 0");
            Debug.Assert(this.ProcessingRequestInvokeCount == 0, "this.ProcessingRequestInvokeCount == 0");
            Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
            Debug.Assert(this.ProcessingChangesetInvokeCount == 0, "this.ProcessingChangesetInvokeCount == 0");
            Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
            Debug.Assert(this.ServiceActionInvokeCount == 0, "this.ServiceActionInvokeCount == 0");
#endif
        }

        /// <summary>
        /// Assert ProcessingPipeline state at DataService&lt;T&gt;.OnStartProcessingRequest
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertAndUpdateDebugStateAtOnStartProcessingRequest()
        {
#if DEBUG
            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
                Debug.Assert(this.ProcessingChangesetInvokeCount <= 1, "this.ProcessingChangesetInvokeCount <= 1");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
            }

            this.OnStartProcessingRequestInvokeCount++;
#endif
        }

        /// <summary>
        /// Assert ProcessingPipeline state before disposing provider interfaces
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertAndUpdateDebugStateAtDispose()
        {
#if DEBUG
            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 1, "this.ProcessedRequestInvokeCount == 1");
                Debug.Assert(this.ProcessingChangesetInvokeCount == 0, "this.ProcessingChangesetInvokeCount == 0");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
                Debug.Assert(this.ServiceActionInvokeCount == 0, "this.ServiceActionInvokeCount == 0");
            }

            this.HasDisposedProviderInterfaces = true;
#endif
        }

        /// <summary>
        /// Assert Processing Pipeline state during request processing
        /// </summary>
        /// <param name="operationContext">data service operation context instance</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertDebugStateDuringRequestProcessing(DataServiceOperationContext operationContext)
        {
#if DEBUG
            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
                Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
                Debug.Assert(
                    (this.ProcessingChangesetInvokeCount == 0) ||
                    (this.ProcessingChangesetInvokeCount == 1 && operationContext.RequestMessage.HttpVerb.IsChange()),
                        "ProcessingChangesetInvokeCount must be 1 during a CUD request.");
            }
#endif
        }

        /// <summary>
        /// Assert Processing Pipeline state when IDataServiceExecutionProvider.Execute() is called.
        /// </summary>
        /// <param name="dataService">data service instance</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertDebugStateAtExecuteExpression(IDataService dataService)
        {
#if DEBUG
            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
                Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
                Debug.Assert(
                    (this.ProcessingChangesetInvokeCount == 0) ||
                    (this.ProcessingChangesetInvokeCount == 1 && dataService.OperationContext.RequestMessage.HttpVerb.IsChange()),
                    "ProcessingChangesetInvokeCount must be 1 during a CUD request and it must 0 during a Read-Only request.");
            }
#endif
        }

        /// <summary>
        /// Assert Processing Pipeline state during request processing
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertAndUpdateDebugStateAtGetService()
        {
#if DEBUG
            Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
#endif
        }

        /// <summary>
        /// Assert Processing Pipeline state at SaveChanges
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertAndUpdateDebugStateAtSaveChanges()
        {
#if DEBUG
            this.SaveChangesInvokeCount++;

            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
                Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
                Debug.Assert(this.ProcessingChangesetInvokeCount == 1, "this.ProcessingChangesetInvokeCount == 1");
                Debug.Assert(this.SaveChangesInvokeCount == 1, "this.SaveChangesInvokeCount == 1");
            }
#endif
        }

        /// <summary>
        /// Assert Processing Pipeline state at InvokeServiceAction
        /// </summary>
        /// <param name="dataService">data service instance</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertAndUpdateDebugStateAtInvokeServiceAction(IDataService dataService)
        {
#if DEBUG
            this.ServiceActionInvokeCount++;

            // If the current request encounters an exception, there is no guarantee that all the
            // events will be fired, we skip the debug asserts when that happens.
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
                Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
                Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
                Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
                Debug.Assert(dataService.OperationContext.RequestMessage.HttpVerb.IsChange(), "HTTP verb should be one of the CUD verbs if we are invoking a Service Action");
                Debug.Assert(this.ProcessingChangesetInvokeCount == 1, "this.ProcessingChangesetInvokeCount == 1");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
            }
#endif
        }

        /// <summary>
        /// Need to be able to reset the states since the caller can reuse the same service instance.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void ResetDebugState()
        {
#if DEBUG
            this.HasDisposedProviderInterfaces = false;
            this.OnStartProcessingRequestInvokeCount = 0;
            this.ProcessingRequestInvokeCount = 0;
            this.ProcessedRequestInvokeCount = 0;
            this.ProcessingChangesetInvokeCount = 0;
            this.SaveChangesInvokeCount = 0;
            this.ServiceActionInvokeCount = 0;
#endif
        }

        /// <summary>
        /// Invoke request start event
        /// </summary>
        /// <param name="sender">Sender, i.e. data service instance.</param>
        /// <param name="e">event arg</param>
        internal void InvokeProcessingRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
#if DEBUG
            this.AssertInitialDebugState();
            this.ProcessingRequestInvokeCount++;
#endif
            if (this.ProcessingRequest != null)
            {
                this.ProcessingRequest(sender, e);
            }
        }

        /// <summary>
        /// Invoke request end event
        /// </summary>
        /// <param name="sender">Sender, i.e. data service instance.</param>
        /// <param name="e">event arg</param>
        internal void InvokeProcessedRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
#if DEBUG
            Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
            Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
            Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
            Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
            if (!this.SkipDebugAssert)
            {
                // The following should be 0 for GET requests.
                // For CUD requests, InvokeProcessedChangeset() would have validated the following and reset them back to 0.
                Debug.Assert(this.ProcessingChangesetInvokeCount == 0, "this.ProcessingChangesetInvokeCount == 0");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
                Debug.Assert(this.ServiceActionInvokeCount == 0, "this.ServiceActionInvokeCount == 0");
            }

            this.ProcessedRequestInvokeCount++;
#endif
            if (this.ProcessedRequest != null)
            {
                this.ProcessedRequest(sender, e);
            }
        }

        /// <summary>
        /// Invoke change set start event
        /// </summary>
        /// <param name="sender">Sender, i.e. data service instance.</param>
        /// <param name="e">event arg</param>
        internal void InvokeProcessingChangeset(object sender, EventArgs e)
        {
#if DEBUG
            Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
            Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
            Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
            Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
            if (!this.SkipDebugAssert)
            {
                // The following should be 0 for GET requests.
                // For CUD requests inside a batch, the previous InvokeProcessedChangeset() would have validated the following and reset them back to 0.
                Debug.Assert(this.ProcessingChangesetInvokeCount == 0, "this.ProcessingChangesetInvokeCount == 0");
                Debug.Assert(this.SaveChangesInvokeCount == 0, "this.SaveChangesInvokeCount == 0");
                Debug.Assert(this.ServiceActionInvokeCount == 0, "this.ServiceActionInvokeCount == 0");
            }

            this.ProcessingChangesetInvokeCount++;
#endif
            if (this.ProcessingChangeset != null)
            {
                this.ProcessingChangeset(sender, e);
            }
        }

        /// <summary>
        /// Invoke change set end event
        /// </summary>
        /// <param name="sender">Sender, i.e. data service instance.</param>
        /// <param name="e">event arg</param>
        internal void InvokeProcessedChangeset(object sender, EventArgs e)
        {
#if DEBUG
            Debug.Assert(!this.HasDisposedProviderInterfaces, "!this.HasDisposedProviderInterfaces");
            Debug.Assert(this.OnStartProcessingRequestInvokeCount > 0, "this.OnStartProcessingRequestInvokeCount > 0");
            Debug.Assert(this.ProcessingRequestInvokeCount == 1, "this.ProcessingRequestInvokeCount == 1");
            Debug.Assert(this.ProcessedRequestInvokeCount == 0, "this.ProcessedRequestInvokeCount == 0");
            if (!this.SkipDebugAssert)
            {
                Debug.Assert(this.ProcessingChangesetInvokeCount == 1, "this.ProcessingChangesetInvokeCount == 1");
                if (this.ServiceActionInvokeCount > 0)
                {
                    // Note that for POST ServiceOps, SaveChanges is not called for V1 providers that doesn't implement IUpdatable and for custom providers.
                    // SaveChanges is always called on ServiceActions.
                    Debug.Assert(this.SaveChangesInvokeCount == 1, "this.SaveChangesInvokeCount == 1");
                }
            }

            this.ProcessingChangesetInvokeCount = 0;
            this.ServiceActionInvokeCount = 0;
            this.SaveChangesInvokeCount = 0;
#endif
            if (this.ProcessedChangeset != null)
            {
                this.ProcessedChangeset(sender, e);
            }
        }
    }
}
