//---------------------------------------------------------------------
// <copyright file="SaveChangesVerifierBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Base implementation of verifier for the DataServiceContext.SaveChanges method.
    /// </summary>
    public abstract class SaveChangesVerifierBase : ISaveChangesVerifier
    {
        /// <summary>
        /// Initializes a new instance of the SaveChangesVerifierBase class.
        /// </summary>
        protected SaveChangesVerifierBase()
        {
            this.Asynchronous = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the verifier should call the asynchronous or synchronous product API
        /// </summary>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "False", HelpText = "Whether to use asynchronous API")]
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the response verifier.
        /// </summary>
        /// <value>The response verifier.</value>
        [InjectDependency(IsRequired = true)]
        public IDataServiceResponseVerifier ResponseVerifier { get; set; }

        /// <summary>
        /// Gets or sets the data service context verifier.
        /// </summary>
        /// <value>The data service context verifier.</value>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextVerifier DataServiceContextVerifier { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceResponsePreferenceVerifier
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceResponsePreferenceVerifier ResponsePreferenceVerifier { get; set; }

        /// <summary>
        /// Gets or sets the sending request event verifier to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISendingRequestEventVerifier SendingRequestEventVerifier { get; set; }

        /// <summary>
        /// Gets or sets the server state verifier to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISaveChangesServerStateVerifier ServerStateVerifier { get; set; }

        /// <summary>
        /// Gets or sets the descriptor data change tracker to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityDescriptorDataChangeTracker DescriptorDataChangeTracker { get; set; }

        /// <summary>
        /// Gets or sets the object services.
        /// </summary>
        /// <value>The object services.</value>
        [InjectDependency(IsRequired = true)]
        public IEntityModelObjectServices ObjectServices { get; set; }

        /// <summary>
        /// Gets or sets the entity model schema.
        /// </summary>
        /// <value>The entity model schema.</value>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema ModelSchema { get; set; }

        /// <summary>
        /// Gets the current set of input values for the verifier
        /// </summary>
        internal VerifierInput Input { get; private set; }

        /// <summary>
        /// Gets the current set of state values for the verifier
        /// </summary>
        internal VerifierState State { get; private set; }

        /// <summary>
        /// Executes SaveChanges method on the specified context and verifies the results.
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        /// <param name="contextData">The context data.</param>
        /// <param name="context">The context to verify SaveChanges.</param>
        /// <param name="options">The options for saving changes. Passing null will use the context's default.</param>
        /// <param name="onCompletion">callback for when save changes verification completes</param>
        public virtual void VerifySaveChanges(IAsyncContinuation continuation, DataServiceContextData contextData, DSClient.DataServiceContext context, SaveChangesOptions? options, Action<IAsyncContinuation, DSClient.DataServiceResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // ensure that two calls are not being processed in parallel
            ExceptionUtilities.Assert(this.Input == null, "Input was not null, possibly due to simultaneous calls or using the wrong async continuation");
            ExceptionUtilities.Assert(this.State == null, "State was not null, possibly due to simultaneous calls or using the wrong async continuation");
            continuation = continuation.OnContinueOrFail(
                () =>
                {
                    this.Input = null;
                    this.State = null;
                });

            this.Input = new VerifierInput()
            {
                ContextData = contextData,
                Context = context,
                Options = options,
                OnCompletion = onCompletion,
            };

            AsyncHelpers.RunActionSequence(
                continuation,
                this.InitializeState,
                this.VerifyContextState,
                this.CallProductApi,
                this.VerifyRequests,
                this.VerifyResponse,
                this.VerifyContextState,
                this.VerifyServerState,
                this.Complete);
        }

        /// <summary>
        /// Initializes the verifier's state based on its inputs
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void InitializeState(IAsyncContinuation continuation)
        {
            this.State = new VerifierState();

            this.State.UsedOptions = this.Input.Options.HasValue ? this.Input.Options.Value : this.Input.Context.SaveChangesDefaultOptions.ToTestEnum();

            this.State.ContextDataBeforeChanges = this.Input.ContextData.Clone();

            this.State.Response = null;
            this.State.EnumeratedOperationResponses.Clear();

            this.State.CachedPropertyValuesBeforeSave.Clear();
            foreach (var entityDescriptorData in this.State.ContextDataBeforeChanges.EntityDescriptorsData.Where(e => e.Entity != null))
            {
                var entityType = this.ModelSchema.EntityTypes.Single(t => t.FullName == entityDescriptorData.EntityClrType.FullName);
                this.State.CachedPropertyValuesBeforeSave[entityDescriptorData.Entity] = this.ObjectServices.GetPropertiesValues(entityDescriptorData.Entity, entityType);
            }

            this.ServerStateVerifier.InitializeExpectedChanges(this.Input.ContextData, this.State.CachedPropertyValuesBeforeSave);

            continuation.Continue();
        }

        /// <summary>
        /// Called immediatlely before the product API call
        /// </summary>
        protected virtual void SetupBeforeProductCall()
        {
            this.ResponsePreferenceVerifier.RegisterEventHandler(this.Input.Context);
            this.SendingRequestEventVerifier.RegisterEventHandler(this.Input.Context);
        }

        /// <summary>
        /// Called immediately after the product API call on both success and failure
        /// </summary>
        protected virtual void CleanupAfterProductCall()
        {
            this.ResponsePreferenceVerifier.UnregisterEventHandler(this.Input.Context, this.State.Response == null);
            this.SendingRequestEventVerifier.UnregisterEventHandler(this.Input.Context, this.State.Response == null);
        }

        /// <summary>
        /// Called after the product API call if it succeeded
        /// </summary>
        protected virtual void OnProductCallSuccess()
        {
            this.State.EnumeratedOperationResponses.AddRange(this.State.Response);
        }

        /// <summary>
        /// Call's the product API for save changes
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void CallProductApi(IAsyncContinuation continuation)
        {
            this.SetupBeforeProductCall();

            if (this.Asynchronous)
            {
                continuation = continuation.OnContinueOrFail(this.CleanupAfterProductCall);

                // the product API continuation
                AsyncCallback productApiCallback = result =>
                {
                    AsyncHelpers.CatchErrors(
                        continuation,
                        () =>
                        {
                            this.Assert.AreSame(this.State, result.AsyncState, "AsyncState was incorrect");
                            this.State.Response = this.Input.Context.EndSaveChanges(result);
                            this.OnProductCallSuccess();
                            continuation.Continue();
                        });
                };

                // call one of the two BeginSaveChanges overloads based on whether there was a SaveChangesOptions value given
                if (this.Input.Options.HasValue)
                {
                    this.Input.Context.BeginSaveChanges(this.Input.Options.Value.ToProductEnum(), productApiCallback, this.State);
                }
                else
                {
                    this.Input.Context.BeginSaveChanges(productApiCallback, this.State);
                }
            }
            else
            {
                // in the sync case we can use a simple try-finally block rather than wrapping the continuation
                try
                {
                    // call one of the two SaveChanges overloads based on whether there was a SaveChangesOptions value given
                    if (this.Input.Options.HasValue)
                    {
                        this.State.Response = this.Input.Context.SaveChanges(this.Input.Options.Value.ToProductEnum());
                    }
                    else
                    {
                        this.State.Response = this.Input.Context.SaveChanges();
                    }
                }
                finally
                {
                    this.CleanupAfterProductCall();
                }
                
                this.OnProductCallSuccess();
                continuation.Continue();
            }
        }
        
        /// <summary>
        /// Verifies the requests sent by the context
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected abstract void VerifyRequests(IAsyncContinuation continuation);

        /// <summary>
        /// Verifies the response returned by the context
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void VerifyResponse(IAsyncContinuation continuation)
        {
            this.ResponseVerifier.VerifyDataServiceResponse(this.State.ExpectedResponse, this.State.Response, this.State.EnumeratedOperationResponses);
            continuation.Continue();
        }

        /// <summary>
        /// Verifies the context and descriptor states
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void VerifyContextState(IAsyncContinuation continuation)
        {
            this.DataServiceContextVerifier.VerifyDataServiceContext(this.Input.ContextData, this.Input.Context);
            continuation.Continue();
        }

        /// <summary>
        /// Verifies that the values on the server are correct
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void VerifyServerState(IAsyncContinuation continuation)
        {
            this.ServerStateVerifier.VerifyChangesOnServer(continuation, this.Input.ContextData);
        }

        /// <summary>
        /// Calls the completion callback given as input
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected virtual void Complete(IAsyncContinuation continuation)
        {
            this.Input.OnCompletion(continuation, this.State.Response);
        }

        /// <summary>
        /// Helper class for representing the input to the save changes verifier
        /// </summary>
        protected internal class VerifierInput
        {
            /// <summary>
            /// Gets or sets the context data
            /// </summary>
            public DataServiceContextData ContextData { get; set; }

            /// <summary>
            /// Gets or sets the context
            /// </summary>
            public DSClient.DataServiceContext Context { get; set; }

            /// <summary>
            /// Gets or sets the save changes options
            /// </summary>
            public SaveChangesOptions? Options { get; set; }

            /// <summary>
            /// Gets or sets the continuation for when the verifier is done
            /// </summary>
            public Action<IAsyncContinuation, DSClient.DataServiceResponse> OnCompletion { get; set; }
        }

         /// <summary>
        /// Helper class for representing the current state of the save changes verifier
        /// </summary>
        protected internal class VerifierState
        {
            /// <summary>
            /// Initializes a new instance of the VerifierState class
            /// </summary>
            public VerifierState()
            {
                this.EnumeratedOperationResponses = new List<DSClient.OperationResponse>();
                this.CachedPropertyValuesBeforeSave = new Dictionary<object, IEnumerable<NamedValue>>(ReferenceEqualityComparer.Create<object>());
            }

            /// <summary>
            /// Gets or sets the effective options of the save changes call
            /// </summary>
            public SaveChangesOptions UsedOptions { get; set; }

            /// <summary>
            /// Gets or sets the context data as it was before any changes were made
            /// </summary>
            public DataServiceContextData ContextDataBeforeChanges { get; set; }

            /// <summary>
            /// Gets or sets the expected response data
            /// </summary>
            public DataServiceResponseData ExpectedResponse { get; set; }

            /// <summary>
            /// Gets or sets the actual response
            /// </summary>
            public DSClient.DataServiceResponse Response { get; set; }

            /// <summary>
            /// Gets collection of enumerated/cached operation responses from the data service response
            /// </summary>
            public IList<DSClient.OperationResponse> EnumeratedOperationResponses { get; private set; }

            /// <summary>
            /// Gets a cache for the property values of tracked client objects from before the call to SaveChanges
            /// </summary>
            public IDictionary<object, IEnumerable<NamedValue>> CachedPropertyValuesBeforeSave { get; private set; }
        }
    }
}
