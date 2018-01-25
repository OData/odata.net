//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataDeltaWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightDeltaWriter : ODataDeltaWriter, IODataOutputInStreamErrorListener
    {
        #region Private Fields

        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// The JsonLight resource and resource set serializer to use.
        /// </summary>
        private readonly ODataJsonLightResourceSerializer jsonLightResourceSerializer;

        /// <summary>
        /// Stack of writer scopes to keep track of the current context of the writer.
        /// </summary>
        private readonly ScopeStack scopes = new ScopeStack();

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        public ODataJsonLightDeltaWriter(ODataJsonLightOutputContext jsonLightOutputContext, IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            // TODO: Replace the assertion with ODataException.
            Debug.Assert(jsonLightOutputContext.WritingResponse, "jsonLightOutputContext.WritingResponse is true");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightResourceSerializer = new ODataJsonLightResourceSerializer(this.jsonLightOutputContext);

            this.NavigationSource = navigationSource;
            this.EntityType = entityType;

            if (navigationSource != null && entityType == null)
            {
                entityType = this.jsonLightOutputContext.EdmTypeResolver.GetElementType(navigationSource);
            }

            ODataUri odataUri = this.jsonLightOutputContext.MessageWriterSettings.ODataUri.Clone();

            this.scopes.Push(new Scope(WriterState.Start, /*item*/null, navigationSource, entityType,
                this.jsonLightOutputContext.MessageWriterSettings.SelectedProperties, odataUri));
            this.jsonWriter = jsonLightOutputContext.JsonWriter;
            this.odataAnnotationWriter = new JsonLightODataAnnotationWriter(this.jsonWriter,
                this.jsonLightOutputContext.ODataSimplifiedOptions.EnableWritingODataAnnotationWithoutPrefix);
        }

        #endregion

        #region Private Enums

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        private enum WriterState
        {
            /// <summary>The writer is at the start; nothing has been written yet.</summary>
            Start,

            /// <summary>The writer is currently writing a delta resource.</summary>
            DeltaResource,

            /// <summary>The writer is currently writing a delta deleted resource.</summary>
            DeltaDeletedEntry,

            /// <summary>The writer is currently writing a delta resource set.</summary>
            DeltaResourceSet,

            /// <summary>The writer is currently writing a delta link.</summary>
            DeltaLink,

            /// <summary>The writer is currently writing a delta deleted link.</summary>
            DeltaDeletedLink,

            /// <summary>The writer is currently writing an expanded navigation property, complex property or complex collection property.</summary>
            NestedResource,

            /// <summary>The writer has completed; nothing can be written anymore.</summary>
            Completed,

            /// <summary>The writer is in error state; nothing can be written anymore.</summary>
            Error
        }

        /// <summary>
        /// Enumeration of JSON Light metadata property flags, used to keep track of which properties were already written.
        /// </summary>
        [Flags]
        private enum JsonLightEntryMetadataProperty
        {
            /// <summary>The odata.editLink property.</summary>
            EditLink = 0x1,

            /// <summary>The odata.readLink property.</summary>
            ReadLink = 0x2,

            /// <summary>The odata.mediaEditLink property.</summary>
            MediaEditLink = 0x4,

            /// <summary>The odata.mediaReadLink property.</summary>
            MediaReadLink = 0x8,

            /// <summary>The odata.mediaContentType property.</summary>
            MediaContentType = 0x10,

            /// <summary>The odata.mediaEtag property.</summary>
            MediaETag = 0x20,
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The navigation source we are going to write entities for.
        /// </summary>
        public IEdmNavigationSource NavigationSource { get; set; }

        /// <summary>
        /// The entity type we are going to write entities for.
        /// </summary>
        public IEdmEntityType EntityType { get; set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// The current scope for the writer.
        /// </summary>
        private Scope CurrentScope
        {
            get
            {
                Debug.Assert(this.scopes.Count > 0, "We should have at least one active scope all the time.");
                return this.scopes.Peek();
            }
        }

        /// <summary>
        /// The current state of the writer.
        /// </summary>
        private WriterState State
        {
            get
            {
                return this.CurrentScope.State;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeltaResourceSetScope.
        /// </summary>
        private JsonLightDeltaResourceSetScope CurrentDeltaResourceSetScope
        {
            get
            {
                JsonLightDeltaResourceSetScope jsonLightDeltaResourceSetScope = this.CurrentScope as JsonLightDeltaResourceSetScope;
                Debug.Assert(jsonLightDeltaResourceSetScope != null, "Asking for JsonDeltaResourceSetScope when the current scope is not a JsonDeltaResourceSetScope.");
                return jsonLightDeltaResourceSetScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeltaResourceScope.
        /// </summary>
        private JsonLightDeltaResourceScope CurrentDeltaResourceScope
        {
            get
            {
                JsonLightDeltaResourceScope jsonLightDeltaResourceScope = this.CurrentScope as JsonLightDeltaResourceScope;
                Debug.Assert(jsonLightDeltaResourceScope != null, "Asking for JsonLightDeltaResourceScope when the current scope is not an JsonLightDeltaResourceScope.");
                return jsonLightDeltaResourceScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeltaLinkScope.
        /// </summary>
        private JsonLightDeltaLinkScope CurrentDeltaLinkScope
        {
            get
            {
                JsonLightDeltaLinkScope jsonLightDeltaLinkScope = this.CurrentScope as JsonLightDeltaLinkScope;
                Debug.Assert(jsonLightDeltaLinkScope != null, "Asking for JsonLightDeltaLinkScope when the current scope is not an JsonLightDeltaLinkScope.");
                return jsonLightDeltaLinkScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightExpandedNavigationPropertyScope.
        /// </summary>
        private JsonLightNestedResourceInfoScope CurrentExpandedNavigationPropertyScope
        {
            get
            {
                JsonLightNestedResourceInfoScope jsonLightExpandedNavigationPropertyScope = this.CurrentScope as JsonLightNestedResourceInfoScope;
                Debug.Assert(jsonLightExpandedNavigationPropertyScope != null, "Asking for JsonLightExpandedNavigationPropertyScope when the current scope is not an JsonLightExpandedNavigationPropertyScope.");
                return jsonLightExpandedNavigationPropertyScope;
            }
        }

        /// <summary>
        /// A flag indicating whether the writer is at the top level.
        /// </summary>
        private bool IsTopLevel
        {
            get
            {
                Debug.Assert(this.State != WriterState.Start && this.State != WriterState.Completed, "IsTopLevel should only be called while writing the payload.");

                // there is the root scope at the top (when the writer has not started or has completed)
                // and then the top-level scope (the top-level resource/resource set item) as the second scope on the stack
                return this.scopes.Count == 2;
            }
        }

        /// <summary>
        /// The structured type of the current delta resource.
        /// </summary>
        private IEdmStructuredType DeltaResourceType
        {
            get
            {
                return this.CurrentScope.ResourceType;
            }
        }

        /// <summary>
        /// Checker to detect duplicate property names.
        /// </summary>
        private IDuplicatePropertyNameChecker DuplicatePropertyNameChecker
        {
            get
            {
                Debug.Assert(
                    this.State == WriterState.DeltaResource || this.State == WriterState.DeltaDeletedEntry,
                    "PropertyAndAnnotationCollector should only be called while writing a delta (deleted) resource.");

                switch (this.State)
                {
                    case WriterState.DeltaResource:
                    case WriterState.DeltaDeletedEntry:
                        return this.CurrentDeltaResourceScope.DuplicatePropertyNameChecker;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_PropertyAndAnnotationCollector));
                }
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        public override void WriteStart(ODataDeltaResourceSet deltaResourceSet)
        {
            this.VerifyCanWriteStartDeltaResourceSet(true, deltaResourceSet);
            this.WriteStartDeltaResourceSetImplementation(deltaResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            this.VerifyCanWriteStartDeltaResourceSet(false, deltaResourceSet);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartDeltaResourceSetImplementation(deltaResourceSet));
        }
#endif

        /// <summary>
        /// Finish writing a delta resource set.
        /// </summary>
        public override void WriteEnd()
        {
            this.VerifyCanWriteEnd(true);
            this.WriteEndImplementation();
            if (this.CurrentScope.State == WriterState.Completed)
            {
                // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                this.Flush();
            }
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously finish writing a delta resource set.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteEndAsync()
        {
            this.VerifyCanWriteEnd(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndImplementation)
                .FollowOnSuccessWithTask(
                    task =>
                    {
                        if (this.CurrentScope.State == WriterState.Completed)
                        {
                            // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                            return this.FlushAsync();
                        }

                        return TaskUtils.CompletedTask;
                    });
        }
#endif

        /// <summary>
        /// Start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        public override void WriteStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            this.VerifyCanWriteNestedResourceInfo(true, nestedResourceInfo);
            this.WriteStartNestedResourceInfoImplementation(nestedResourceInfo);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            this.VerifyCanWriteNestedResourceInfo(false, nestedResourceInfo);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartNestedResourceInfoImplementation(nestedResourceInfo));
        }
#endif

        /// <summary>
        /// Start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        public override void WriteStart(ODataResourceSet expandedResourceSet)
        {
            this.VerifyCanWriteExpandedResourceSet(true, expandedResourceSet);
            this.WriteStartExpandedResourceSetImplementation(expandedResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResourceSet expandedResourceSet)
        {
            this.VerifyCanWriteExpandedResourceSet(false, expandedResourceSet);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartExpandedResourceSetImplementation(expandedResourceSet));
        }
#endif

        /// <summary>
        /// Start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        public override void WriteStart(ODataResource deltaResource)
        {
            this.VerifyCanWriteResource(true, deltaResource);
            this.WriteStartDeltaResourceImplementation(deltaResource);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResource deltaResource)
        {
            this.VerifyCanWriteResource(false, deltaResource);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartDeltaResourceImplementation(deltaResource));
        }
#endif

        /// <summary>
        /// Writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        public override void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry)
        {
            this.VerifyCanWriteResource(true, deltaDeletedEntry);
            this.WriteStartDeltaResourceImplementation(deltaDeletedEntry);
            this.WriteEnd();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry)
        {
            this.VerifyCanWriteResource(false, deltaDeletedEntry);
            return TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                this.WriteStartDeltaResourceImplementation(deltaDeletedEntry);
                this.WriteEnd();
            });
        }
#endif

        /// <summary>
        /// Writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        public override void WriteDeltaLink(ODataDeltaLink deltaLink)
        {
            this.VerifyCanWriteLink(true, deltaLink);
            this.WriteStartDeltaLinkImplementation(deltaLink);
            this.WriteEnd();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink)
        {
            this.VerifyCanWriteLink(false, deltaLink);
            return TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                this.WriteStartDeltaLinkImplementation(deltaLink);
                this.WriteEnd();
            });
        }
#endif

        /// <summary>
        /// Writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        public override void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink)
        {
            this.VerifyCanWriteLink(true, deltaDeletedLink);
            this.WriteStartDeltaLinkImplementation(deltaDeletedLink);
            this.WriteEnd();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink)
        {
            this.VerifyCanWriteLink(false, deltaDeletedLink);
            return TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                this.WriteStartDeltaLinkImplementation(deltaDeletedLink);
                this.WriteEnd();
            });
        }
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public override void Flush()
        {
            this.jsonLightOutputContext.Flush();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            return this.jsonLightOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        void IODataOutputInStreamErrorListener.OnInStreamError()
        {
            this.VerifyNotDisposed();

            // We're in a completed state trying to write an error (we can't write error after the payload was finished as it might
            // introduce another top-level element in XML)
            if (this.State == WriterState.Completed)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), WriterState.Error.ToString()));
            }

            this.StartPayloadInStartState();
            this.EnterScope(WriterState.Error, this.CurrentScope.Item);
        }

        #endregion

        #region Private Methods

        #region Verifying Methods

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            this.jsonLightOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Verifies that a call is allowed to the writer.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.jsonLightOutputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataWriterCore_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.jsonLightOutputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataWriterCore_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Verifies that calling WriteStart delta resource set is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="deltaResourceSet">Resource Set/collection to write.</param>
        private void VerifyCanWriteStartDeltaResourceSet(bool synchronousCall, ODataDeltaResourceSet deltaResourceSet)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaResourceSet, "deltaResourceSet");

            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
            this.StartPayloadInStartState();
        }

        /// <summary>
        /// Verifies that calling WriteStart nested resource info is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="nestedResourceInfo">Navigation link to write.</param>
        private void VerifyCanWriteNestedResourceInfo(bool synchronousCall, ODataNestedResourceInfo nestedResourceInfo)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            ExceptionUtils.CheckArgumentNotNull(nestedResourceInfo, "nestedResourceInfo");
        }

        /// <summary>
        /// Verifies that calling WriteStart expanded resource set is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="expandedResourceSet">Expanded resource set to write.</param>
        private void VerifyCanWriteExpandedResourceSet(bool synchronousCall, ODataResourceSet expandedResourceSet)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            ExceptionUtils.CheckArgumentNotNull(expandedResourceSet, "expandedResourceSet");
        }

        /// <summary>
        /// Verifies that calling WriteStart delta (deleted) resource is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="resource">Resource/item to write.</param>
        private void VerifyCanWriteResource(bool synchronousCall, ODataItem resource)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            ExceptionUtils.CheckArgumentNotNull(resource, "resource");
        }

        /// <summary>
        /// Verifies that calling WriteStart delta link is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="deltaLink">Delta link to write.</param>
        private void VerifyCanWriteLink(bool synchronousCall, ODataDeltaLink deltaLink)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            ExceptionUtils.CheckArgumentNotNull(deltaLink, "delta link");
        }

        /// <summary>
        /// Verifies that calling WriteStart delta deleted link is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="deltaDeletedLink">Delta deleted link to write.</param>
        private void VerifyCanWriteLink(bool synchronousCall, ODataDeltaDeletedLink deltaDeletedLink)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            ExceptionUtils.CheckArgumentNotNull(deltaDeletedLink, "delta deleted link");
        }

        /// <summary>
        /// Verify that calling WriteEnd is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEnd(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        private void ValidateTransition(WriterState newState)
        {
            if (!IsErrorState(this.State) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
                return;
            }

            if (this.State != WriterState.DeltaResource && newState == WriterState.NestedResource)
            {
                throw new ODataException(Strings.ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource(this.State.ToString(), newState.ToString()));
            }

            switch (this.State)
            {
                case WriterState.Start:
                    if (newState != WriterState.DeltaResourceSet)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromStart(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.DeltaResource:
                case WriterState.DeltaDeletedEntry:
                case WriterState.DeltaLink:
                case WriterState.DeltaDeletedLink:
                    if (this.CurrentScope.Item == null)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromNullResource(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.DeltaResourceSet:
                    if (newState != WriterState.DeltaResource && newState != WriterState.DeltaDeletedEntry &&
                        newState != WriterState.DeltaLink && newState != WriterState.DeltaDeletedLink)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromResourceSet(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.NestedResource:
                    throw new ODataException(Strings.ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource(this.State.ToString(), newState.ToString()));
                case WriterState.Completed:
                    // we should never see a state transition when in state 'Completed'
                    throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), newState.ToString()));
                case WriterState.Error:
                    if (newState != WriterState.Error)
                    {
                        // No more state transitions once we are in error state except for the fatal error
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.State.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Validates the media resource on the delta resource.
        /// </summary>
        /// <param name="resource">The resource to validate.</param>
        /// <param name="entityType">The entity type of the resource.</param>
        private void ValidateEntryMediaResource(ODataResource resource, IEdmEntityType entityType)
        {
            if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
            {
                return;
            }

            this.jsonLightOutputContext.WriterValidator.ValidateMetadataResource(resource, entityType);
        }

        #endregion

        #region WriteStart Implementation Methods

        /// <summary>
        /// Start writing a delta resource set - implementation of the actual functionality.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        private void WriteStartDeltaResourceSetImplementation(ODataDeltaResourceSet resourceSet)
        {
            this.EnterScope(WriterState.DeltaResourceSet, resourceSet);

            this.InterceptException(() => this.StartDeltaResourceSet(resourceSet));
        }

        /// <summary>
        /// Start writing a nested resource info - implementation of the actual functionality.
        /// </summary>
        /// <param name="nestedResourceInfo">Navigation link to write.</param>
        private void WriteStartNestedResourceInfoImplementation(ODataNestedResourceInfo nestedResourceInfo)
        {
            if (!IsNestedResourceState(this.State))
            {
                this.EnterScope(WriterState.NestedResource, nestedResourceInfo);
            }

            this.InterceptException(() => this.CurrentExpandedNavigationPropertyScope
                .JsonLightExpandedNavigationPropertyWriter.WriteStart(nestedResourceInfo));
        }

        /// <summary>
        /// Start writing an expanded resource set - implementation of the actual functionality.
        /// </summary>
        /// <param name="expandedResourceSet">Expanded resource set to write.</param>
        private void WriteStartExpandedResourceSetImplementation(ODataResourceSet expandedResourceSet)
        {
            if (!IsNestedResourceState(this.State))
            {
                throw new ODataException(Strings.ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState(this.State.ToString()));
            }

            this.InterceptException(() => this.CurrentExpandedNavigationPropertyScope
                .JsonLightExpandedNavigationPropertyWriter.WriteStart(expandedResourceSet));
        }

        /// <summary>
        /// Start writing a delta resource - implementation of the actual functionality.
        /// </summary>
        /// <param name="resource">Resource/item to write.</param>
        private void WriteStartDeltaResourceImplementation(ODataResource resource)
        {
            Debug.Assert(resource != null, "resource != null");

            if (IsNestedResourceState(this.State))
            {
                this.InterceptException(() => this.CurrentExpandedNavigationPropertyScope
                    .JsonLightExpandedNavigationPropertyWriter.WriteStart(resource));
                return;
            }

            this.StartPayloadInStartState();
            this.EnterScope(WriterState.DeltaResource, resource);

            this.InterceptException(() =>
            {
                this.PreStartDeltaResource(resource);
                this.StartDeltaResource(resource);
            });
        }

        /// <summary>
        /// Start writing a delta deleted resource - implementation of the actual functionality.
        /// </summary>
        /// <param name="resource">Resource/item to write.</param>
        private void WriteStartDeltaResourceImplementation(ODataDeltaDeletedEntry resource)
        {
            Debug.Assert(resource != null, "resource != null");

            this.StartPayloadInStartState();
            this.EnterScope(WriterState.DeltaDeletedEntry, resource);

            this.InterceptException(() => this.StartDeltaDeletedEntry(resource));
        }

        /// <summary>
        /// Resolve Resource Type before starting the resource.
        /// </summary>
        /// <param name="resource">The resource to start.</param>
        private void ResolveEntityType(ODataResource resource)
        {
            DeltaResourceScope resourceScope = this.CurrentDeltaResourceScope;

            // Try resolving entity type from serialization info.
            IEdmEntityType entityTypeFromInfo = null;
            if (resource.SerializationInfo != null)
            {
                Debug.Assert(!string.IsNullOrEmpty(resource.SerializationInfo.NavigationSourceName), "NavigationSource name should be set in serialization info.");
                if (this.jsonLightOutputContext.Model != null && jsonLightOutputContext.Model != EdmCoreModel.Instance)
                {
                    if (resource.SerializationInfo.NavigationSourceKind == EdmNavigationSourceKind.EntitySet)
                    {
                        IEdmEntitySet entitySet = this.jsonLightOutputContext.Model.FindDeclaredEntitySet(resource.SerializationInfo.NavigationSourceName);
                        if (entitySet != null)
                        {
                            entityTypeFromInfo = entitySet.EntityType();
                        }
                    }
                }
            }

            // Try resolving entity type from delta resource.
            IEdmEntityType entityTypeFromEntry = null;
            if (!string.IsNullOrEmpty(resource.TypeName))
            {
                if (jsonLightOutputContext.Model != null && jsonLightOutputContext.Model != EdmCoreModel.Instance)
                {
                    entityTypeFromEntry = TypeNameOracle.ResolveAndValidateTypeName(
                        this.jsonLightOutputContext.Model,
                        resource.TypeName,
                        EdmTypeKind.Entity,
                        /* expectStructuredType */ true,
                        this.jsonLightOutputContext.WriterValidator) as IEdmEntityType;
                }
            }

            // Get entity type from the parent scope.
            // TODO: Update this one in the code change for writer.
            IEdmEntityType entityTypeFromResourceSet = CurrentDeltaResourceScope.ResourceType as IEdmEntityType;
            resourceScope.ResourceTypeFromMetadata = entityTypeFromResourceSet;

            // For expected entity type, prefer type from resource over resource set.
            if (entityTypeFromEntry != null)
            {
                resourceScope.ResourceType = entityTypeFromEntry;
            }
            else if (entityTypeFromInfo != null)
            {
                resourceScope.ResourceType = entityTypeFromInfo;
            }
            else if (entityTypeFromResourceSet != null)
            {
                resourceScope.ResourceType = entityTypeFromResourceSet;
            }
            else
            {
                resourceScope.ResourceType = null;
            }
        }

        /// <summary>
        /// Handle something before starting a delta resource.
        /// </summary>
        /// <param name="resource">The resource to start.</param>
        private void PreStartDeltaResource(ODataResource resource)
        {
            this.ResolveEntityType(resource);
            this.PrepareResourceForWriteStart(resource);
            this.ValidateEntryMediaResource(resource, CurrentDeltaResourceScope.ResourceType as IEdmEntityType);
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the resource is writen, at the begining of WriteStartDeltaResourceImplementation.
        /// </summary>
        /// <param name="resource">Resource to write.</param>
        private void PrepareResourceForWriteStart(ODataResource resource)
        {
            DeltaResourceScope resourceScope = this.CurrentDeltaResourceScope;
            Debug.Assert(resourceScope != null, "resourceScope != null");

            ODataResourceMetadataBuilder builder = this.jsonLightOutputContext.MetadataLevel.CreateResourceMetadataBuilder(
                resource,
                resourceScope.GetOrCreateTypeContext(),
                resourceScope.SerializationInfo,
                resourceScope.ResourceType,
                resourceScope.SelectedProperties,
                /*writingResponse*/ true,
                this.jsonLightOutputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment,
                this.jsonLightOutputContext.MessageWriterSettings.ODataUri);

            this.jsonLightOutputContext.MetadataLevel.InjectMetadataBuilder(resource, builder);
        }

        /// <summary>
        /// Start writing a delta link - implementation of the actual functionality.
        /// </summary>
        /// <param name="deltaLink">Delta link to write.</param>
        private void WriteStartDeltaLinkImplementation(ODataDeltaLink deltaLink)
        {
            this.EnterScope(WriterState.DeltaLink, deltaLink);
            this.StartDeltaLink(deltaLink);
        }

        /// <summary>
        /// Start writing a delta deleted link - implementation of the actual functionality.
        /// </summary>
        /// <param name="deltaDeletedLink">Delta deleted link to write.</param>
        private void WriteStartDeltaLinkImplementation(ODataDeltaDeletedLink deltaDeletedLink)
        {
            this.EnterScope(WriterState.DeltaDeletedLink, deltaDeletedLink);
            this.StartDeltaLink(deltaDeletedLink);
        }

        #endregion

        #region WriteEnd Implementation Methods

        /// <summary>
        /// Finish writing a delta resource set/resource.
        /// </summary>
        private void WriteEndImplementation()
        {
            if (this.State == WriterState.NestedResource)
            {
                if (this.CurrentExpandedNavigationPropertyScope.JsonLightExpandedNavigationPropertyWriter.WriteEnd())
                {
                    this.LeaveScope();
                }

                return;
            }

            this.InterceptException(() =>
            {
                Scope currentScope = this.CurrentScope;

                switch (currentScope.State)
                {
                    case WriterState.DeltaResource:
                        {
                            Debug.Assert((ODataResource)currentScope.Item != null, "resource == null");

                            this.EndDeltaResource();
                        }

                        break;
                    case WriterState.DeltaDeletedEntry:
                        {
                            Debug.Assert((ODataDeltaDeletedEntry)currentScope.Item != null, "resource == null");

                            this.EndDeltaResource();
                        }

                        break;
                    case WriterState.DeltaResourceSet:
                        {
                            ODataDeltaResourceSet resourceSet = (ODataDeltaResourceSet)currentScope.Item;
                            WriterValidationUtils.ValidateResourceSetAtEnd(DeltaConverter.ToODataResourceSet(resourceSet), /*writingResponse*/ false);
                            this.EndDeltaResourceSet(resourceSet);
                        }

                        break;
                    case WriterState.DeltaLink:
                    case WriterState.DeltaDeletedLink:
                        this.EndDeltaLink();

                        break;
                    case WriterState.Start:                 // fall through
                    case WriterState.Completed:             // fall through
                    case WriterState.Error:                 // fall through
                        throw new ODataException(Strings.ODataWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_WriteEnd_UnreachableCodePath));
                }

                this.LeaveScope();
            });
        }

        #endregion

        #region WriteDeltaResourceSet<...> Methods

        /// <summary>
        /// Writes the odata.count annotation for a resource set if it has not been written yet (and the count is specified on the resource set).
        /// </summary>
        /// <param name="resourceSet">The resource set to write the count for.</param>
        private void WriteDeltaResourceSetCount(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // If we haven't written the count yet and it's available, write it.
            long? count = resourceSet.Count;
            if (count.HasValue)
            {
                this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataCount);
                this.jsonWriter.WriteValue(count.Value);
            }
        }

        /// <summary>
        /// Writes the odata.nextLink annotation for a delta resource set if the next link is specified on the resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write the next link for.</param>
        private void WriteDeltaResourceSetNextLink(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // If we haven't written the next link yet and it's available, write it.
            Uri nextPageLink = resourceSet.NextPageLink;
            if (nextPageLink != null && !this.CurrentDeltaResourceSetScope.NextPageLinkWritten)
            {
                this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNextLink);
                this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(nextPageLink));
                this.CurrentDeltaResourceSetScope.NextPageLinkWritten = true;
            }
        }

        /// <summary>
        /// Writes the odata.deltaLink annotation for a delta resource set if the delta link is specified on the resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write the delta link for.</param>
        private void WriteDeltaResourceSetDeltaLink(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // If we haven't written the delta link yet and it's available, write it.
            Uri deltaLink = resourceSet.DeltaLink;
            if (deltaLink != null && !this.CurrentDeltaResourceSetScope.DeltaLinkWritten)
            {
                this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataDeltaLink);
                this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(deltaLink));
                this.CurrentDeltaResourceSetScope.DeltaLinkWritten = true;
            }
        }

        /// <summary>
        /// Writes the context uri for a delta resource set.
        /// </summary>
        private void WriteDeltaResourceSetContextUri()
        {
            this.CurrentDeltaResourceSetScope.ContextUriInfo = this.jsonLightResourceSerializer.WriteDeltaContextUri(
                this.CurrentDeltaResourceSetScope.GetOrCreateTypeContext(),
                ODataDeltaKind.ResourceSet);
        }

        /// <summary>
        /// Writes the instance annotions for a delta resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write instance annotations for.</param>
        private void WriteDeltaResourceSetInstanceAnnotations(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(resourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);
        }

        /// <summary>
        /// Writes the value start for a delta resource set.
        /// </summary>
        private void WriteDeltaResourceSetValueStart()
        {
            this.jsonWriter.WriteValuePropertyName();
            this.jsonWriter.StartArrayScope();
        }

        #endregion

        #region WriteDeltaResource<...> Methods

        /// <summary>
        /// Writes the odata.id annotation for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the id for.</param>
        private void WriteDeltaResourceId(ODataDeltaDeletedEntry resource)
        {
            Debug.Assert(resource != null, "resource != null");

            this.jsonWriter.WriteName(JsonLightConstants.ODataIdPropertyName);
            this.jsonWriter.WriteValue(resource.Id);
        }

        /// <summary>
        /// Writes the reason annotation for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the reason for.</param>
        private void WriteDeltaResourceReason(ODataDeltaDeletedEntry resource)
        {
            Debug.Assert(resource != null, "resource != null");

            if (!resource.Reason.HasValue)
            {
                return;
            }

            this.jsonWriter.WriteName(JsonLightConstants.ODataReasonPropertyName);

            switch (resource.Reason.Value)
            {
                case DeltaDeletedEntryReason.Deleted:
                    this.jsonWriter.WriteValue(JsonLightConstants.ODataReasonDeletedValue);
                    break;
                case DeltaDeletedEntryReason.Changed:
                    this.jsonWriter.WriteValue(JsonLightConstants.ODataReasonChangedValue);
                    break;
                default:
                    Debug.Assert(false, "Unknown reason.");
                    break;
            }
        }

        /// <summary>
        /// Writes the context uri for a delta (deleted) resource.
        /// </summary>
        /// <param name="kind">The kind of delta resource.</param>
        private void WriteDeltaResourceContextUri(ODataDeltaKind kind)
        {
            Debug.Assert(kind == ODataDeltaKind.Resource || kind == ODataDeltaKind.DeletedEntry, "kind must be either DeltaResource or DeltaDeletedEntry.");
            this.jsonLightResourceSerializer.WriteDeltaContextUri(
                this.CurrentDeltaResourceScope.GetOrCreateTypeContext(),
                kind,
                this.GetParentDeltaResourceSetScope().ContextUriInfo);
        }

        /// <summary>
        /// Start writing the metadata for a delta (deleted) resource.
        /// </summary>
        private void WriteDeltaResourceStartMetadata()
        {
            this.jsonLightResourceSerializer.WriteResourceStartMetadataProperties(this.CurrentDeltaResourceScope);
            this.jsonLightResourceSerializer.WriteResourceMetadataProperties(this.CurrentDeltaResourceScope);
        }

        /// <summary>
        /// End writing the metadata for a delta (deleted) resource.
        /// </summary>
        private void WriteDeltaResourceEndMetadata()
        {
            this.jsonLightResourceSerializer.WriteResourceEndMetadataProperties(this.CurrentDeltaResourceScope, this.CurrentDeltaResourceScope.DuplicatePropertyNameChecker);
        }

        /// <summary>
        /// Writes the instance annotions for a delta resource.
        /// </summary>
        /// <param name="resource">The resource to write instance annotations for.</param>
        private void WriteDeltaResourceInstanceAnnotations(ODataResource resource)
        {
            this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, this.CurrentDeltaResourceScope.InstanceAnnotationWriteTracker);
        }

        /// <summary>
        /// Writes the properties for a delta resource.
        /// </summary>
        /// <param name="resource">The resource to write properties for.</param>
        private void WriteDeltaResourceProperties(ODataResource resource)
        {
            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            this.jsonLightResourceSerializer.WriteProperties(
                this.DeltaResourceType,
                resource.Properties,
                false /* isComplexValue */,
                this.DuplicatePropertyNameChecker);
            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
        }

        #endregion

        #region WriteDeltaLink<...> Methods

        /// <summary>
        /// Writes the context uri for a delta (deleted) link.
        /// </summary>
        /// <param name="kind">The delta kind of link.</param>
        private void WriteDeltaLinkContextUri(ODataDeltaKind kind)
        {
            Debug.Assert(kind == ODataDeltaKind.Link || kind == ODataDeltaKind.DeletedLink, "kind must be either DeltaLink or DeltaDeletedLink.");
            this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeltaLinkScope.GetOrCreateTypeContext(), kind);
        }

        /// <summary>
        /// Writes the source for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write source for.</param>
        private void WriteDeltaLinkSource(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataSourcePropertyName);
            this.jsonWriter.WriteValue(UriUtils.UriToString(link.Source));
        }

        /// <summary>
        /// Writes the relationship for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write relationship for.</param>
        private void WriteDeltaLinkRelationship(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataRelationshipPropertyName);
            this.jsonWriter.WriteValue(link.Relationship);
        }

        /// <summary>
        /// Writes the target for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write target for.</param>
        private void WriteDeltaLinkTarget(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataTargetPropertyName);
            this.jsonWriter.WriteValue(UriUtils.UriToString(link.Target));
        }

        #endregion

        #region Start<...> Methods

        /// <summary>
        /// Start writing a delta resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        private void StartDeltaResourceSet(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(this.IsTopLevel, "Delta resource set must be on top level.");

            this.jsonWriter.StartObjectScope();

            this.WriteDeltaResourceSetContextUri();
            this.WriteDeltaResourceSetCount(resourceSet);
            this.WriteDeltaResourceSetNextLink(resourceSet);
            this.WriteDeltaResourceSetDeltaLink(resourceSet);
            this.WriteDeltaResourceSetInstanceAnnotations(resourceSet);
            this.WriteDeltaResourceSetValueStart();
        }

        /// <summary>
        /// Start writing a delta resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        private void StartDeltaResource(ODataResource resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(!this.IsTopLevel, "Delta resource cannot be on top level.");

            this.jsonWriter.StartObjectScope();

            this.WriteDeltaResourceContextUri(ODataDeltaKind.Resource);
            this.WriteDeltaResourceStartMetadata();
            this.WriteDeltaResourceInstanceAnnotations(resource);
            this.WriteDeltaResourceProperties(resource);
        }

        /// <summary>
        /// Start writing a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        private void StartDeltaDeletedEntry(ODataDeltaDeletedEntry resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(!this.IsTopLevel, "Delta resource cannot be on top level.");

            this.jsonWriter.StartObjectScope();

            this.WriteDeltaResourceContextUri(ODataDeltaKind.DeletedEntry);
            this.WriteDeltaResourceId(resource);
            this.WriteDeltaResourceReason(resource);
        }

        /// <summary>
        /// Start writing a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        private void StartDeltaLink(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.StartObjectScope();

            if (link is ODataDeltaLink)
            {
                this.WriteDeltaLinkContextUri(ODataDeltaKind.Link);
            }
            else
            {
                this.WriteDeltaLinkContextUri(ODataDeltaKind.DeletedLink);
            }

            this.WriteDeltaLinkSource(link);
            this.WriteDeltaLinkRelationship(link);
            this.WriteDeltaLinkTarget(link);
        }

        #endregion

        #region End<...> Methods

        /// <summary>
        /// Finish writing a delta resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        private void EndDeltaResourceSet(ODataDeltaResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // End the array which holds the entries in the resource set.
            this.jsonWriter.EndArrayScope();

            // Write custom instance annotations
            this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(resourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);

            // Write the next link if it's available.
            this.WriteDeltaResourceSetNextLink(resourceSet);

            // Write the delta link if it's available.
            this.WriteDeltaResourceSetDeltaLink(resourceSet);

            // Close the object wrapper.
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Finish writing a delta (deleted) resource.
        /// </summary>
        private void EndDeltaResource()
        {
            Debug.Assert(CurrentScope.State == WriterState.DeltaResource || CurrentScope.State == WriterState.DeltaDeletedEntry, "state must be either DeltaResource or DeltaDeletedEntry.");

            if (CurrentScope.State == WriterState.DeltaResource)
            {
                this.WriteDeltaResourceEndMetadata();
            }

            // Close the object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Finish writing a delta (deleted) link.
        /// </summary>
        private void EndDeltaLink()
        {
            this.jsonWriter.EndObjectScope();
        }

        #endregion

        #region Scope Methods

        /// <summary>
        /// Enter a new writer scope; verifies that the transition from the current state into new state is valid
        /// and attaches the item to the new scope.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        /// <param name="item">The item to associate with the new scope.</param>
        private void EnterScope(WriterState newState, ODataItem item)
        {
            Debug.Assert(item != null, "item != null");

            this.InterceptException(() => this.ValidateTransition(newState));

            Scope currentScope = this.CurrentScope;

            IEdmNavigationSource navigationSource = null;
            IEdmEntityType entityType = null;
            SelectedPropertiesNode selectedProperties = currentScope.SelectedProperties;
            ODataUri odataUri = currentScope.ODataUri.Clone();

            if (newState == WriterState.DeltaResource || newState == WriterState.DeltaDeletedEntry ||
                newState == WriterState.DeltaLink || newState == WriterState.DeltaDeletedLink ||
                newState == WriterState.DeltaResourceSet || newState == WriterState.NestedResource)
            {
                navigationSource = currentScope.NavigationSource;

                // TODO: Update this one in the code change for writer.
                entityType = currentScope.ResourceType as IEdmEntityType;
            }

            this.PushScope(newState, item, navigationSource, entityType, selectedProperties, odataUri);
        }

        /// <summary>
        /// Leave the current writer scope and return to the previous scope.
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        private void LeaveScope()
        {
            Debug.Assert(this.State != WriterState.Error, "this.State != WriterState.Error");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                Scope startScope = this.scopes.Pop();
                Debug.Assert(startScope.State == WriterState.Start, "startScope.State == WriterState.Start");

                // TODO: Update this one in the code change for writer.
                this.PushScope(WriterState.Completed, /*item*/null, startScope.NavigationSource, startScope.ResourceType, startScope.SelectedProperties, startScope.ODataUri);
                this.InterceptException(this.EndPayload);
            }
        }

        /// <summary>
        /// Create a new writer scope.
        /// </summary>
        /// <param name="state">The writer state of the scope to create.</param>
        /// <param name="item">The item attached to the scope to create.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set  for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The OdataUri info of this scope.</param>
        private void PushScope(WriterState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            Debug.Assert(
                state == WriterState.Error ||
                state == WriterState.DeltaResource && item is ODataResource ||
                state == WriterState.DeltaDeletedEntry && item is ODataDeltaDeletedEntry ||
                state == WriterState.DeltaResourceSet && item is ODataDeltaResourceSet ||
                state == WriterState.DeltaLink && item is ODataDeltaLink ||
                state == WriterState.DeltaDeletedLink && item is ODataDeltaDeletedLink ||
                state == WriterState.NestedResource && item is ODataNestedResourceInfo ||
                state == WriterState.Start && item == null ||
                state == WriterState.Completed && item == null,
                "Writer state and associated item do not match.");

            var entityType = resourceType as IEdmEntityType;
            Scope scope;
            switch (state)
            {
                case WriterState.DeltaResource:
                    scope = this.CreateDeltaResourceScope(WriterState.DeltaResource, item, navigationSource, entityType, selectedProperties, odataUri);
                    break;
                case WriterState.DeltaDeletedEntry:
                    scope = this.CreateDeltaResourceScope(WriterState.DeltaDeletedEntry, item, navigationSource, entityType, selectedProperties, odataUri);
                    break;
                case WriterState.DeltaResourceSet:
                    scope = CreateDeltaResourceSetScope(item, navigationSource, entityType, selectedProperties, odataUri);
                    break;
                case WriterState.DeltaLink:
                    scope = this.CreateDeltaLinkScope(WriterState.DeltaLink, item, navigationSource, entityType, selectedProperties, odataUri);
                    break;
                case WriterState.DeltaDeletedLink:
                    scope = this.CreateDeltaLinkScope(WriterState.DeltaDeletedLink, item, navigationSource, entityType, selectedProperties, odataUri);
                    break;
                case WriterState.NestedResource:
                    scope = this.CreateNestedResourceScope(item, navigationSource, resourceType, selectedProperties, odataUri);
                    break;
                case WriterState.Start:                     // fall through
                case WriterState.Completed:                 // fall through
                case WriterState.Error:
                    scope = new Scope(state, item, navigationSource, resourceType, selectedProperties, odataUri);
                    break;
                default:
                    string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_Scope_Create_UnreachableCodePath);
                    Debug.Assert(false, errorMessage);
                    throw new ODataException(errorMessage);
            }

            this.scopes.Push(scope);
        }

        /// <summary>
        /// Get instance of the parent delta resource set scope
        /// </summary>
        /// <returns>
        /// The parent delta resource set scope
        /// Or null if there is no parent delta resource set scope
        /// </returns>
        private DeltaResourceSetScope GetParentDeltaResourceSetScope()
        {
            ScopeStack scopeStack = new ScopeStack();
            Scope parentResourceSetScope = null;

            if (this.scopes.Count > 0)
            {
                // pop current scope and push into scope stack
                scopeStack.Push(this.scopes.Pop());
            }

            while (this.scopes.Count > 0)
            {
                Scope scope = this.scopes.Pop();
                scopeStack.Push(scope);

                if (scope is DeltaResourceSetScope)
                {
                    parentResourceSetScope = scope;
                    break;
                }
            }

            while (scopeStack.Count > 0)
            {
                Scope scope = scopeStack.Pop();
                this.scopes.Push(scope);
            }

            return parentResourceSetScope as DeltaResourceSetScope;
        }

        #endregion

        #endregion

        #region Create Scope Methods

        /// <summary>
        /// Create a new delta resource set scope.
        /// </summary>
        /// <param name="resourceSet">The resource set for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        private static DeltaResourceSetScope CreateDeltaResourceSetScope(ODataItem resourceSet, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightDeltaResourceSetScope(resourceSet, navigationSource, resourceType, selectedProperties, odataUri);
        }

        /// <summary>
        /// Create a new delta resource scope.
        /// </summary>
        /// <param name="state">The writer state of the scope to create.</param>
        /// <param name="resource">The resource for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type of the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        private DeltaResourceScope CreateDeltaResourceScope(WriterState state, ODataItem resource, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightDeltaResourceScope(
                state,
                resource,
                this.GetResourceSerializationInfo(resource),
                navigationSource,
                resourceType,
                this.jsonLightOutputContext.MessageWriterSettings,
                selectedProperties,
                odataUri);
        }

        /// <summary>
        /// Create a new delta link scope.
        /// </summary>
        /// <param name="state">The writer state of the scope to create.</param>
        /// <param name="link">The link for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        private DeltaLinkScope CreateDeltaLinkScope(WriterState state, ODataItem link, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightDeltaLinkScope(
                state,
                link,
                this.GetLinkSerializationInfo(link),
                navigationSource,
                entityType,
                selectedProperties,
                odataUri);
        }

        /// <summary>
        /// Create a new expanded navigation property scope.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info for the resource set.</param>
        /// <param name="navigationSource">The navigation source of the parent delta resource.</param>
        /// <param name="entityType">The entity type of the parent delta resource.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly created scope.</returns>
        private NestedResourceInfoScope CreateNestedResourceScope(ODataItem nestedResourceInfo, IEdmNavigationSource navigationSource, IEdmStructuredType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightNestedResourceInfoScope(
                nestedResourceInfo,
                navigationSource,
                entityType,
                selectedProperties,
                odataUri,
                this.CurrentDeltaResourceScope.Resource,
                this.jsonLightOutputContext);
        }

        #endregion

        #region Payload Methods

        /// <summary>
        /// Checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        private void StartPayloadInStartState()
        {
            if (this.State == WriterState.Start)
            {
                this.InterceptException(this.StartPayload);
            }
        }

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        private void StartPayload()
        {
            this.jsonLightResourceSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        private void EndPayload()
        {
            this.jsonLightResourceSerializer.WritePayloadEnd();
        }

        #endregion

        #region Exception Methods

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void InterceptException(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(WriterState.Error, this.CurrentScope.Item);
                }

                throw;
            }
        }

        #endregion

        #region SerializationInfo Methods

        /// <summary>
        /// Gets the serialization info from the parent resource set.
        /// </summary>
        /// <returns>The serialization info from the parent resource set.</returns>
        private ODataDeltaSerializationInfo GetParentResourceSetSerializationInfo()
        {
            DeltaResourceSetScope parentDeltaResourceSetScope = this.CurrentScope as DeltaResourceSetScope;
            if (parentDeltaResourceSetScope != null)
            {
                ODataDeltaResourceSet resourceSet = (ODataDeltaResourceSet)parentDeltaResourceSetScope.Item;
                Debug.Assert(resourceSet != null, "resourceSet != null");

                return DeltaConverter.ToDeltaSerializationInfo(resourceSet.SerializationInfo);
            }

            return null;
        }

        /// <summary>
        /// Gets the serialization info for the given delta resource.
        /// </summary>
        /// <param name="item">The resource to get the serialization info for.</param>
        /// <returns>The serialization info for the given resource.</returns>
        private ODataResourceSerializationInfo GetResourceSerializationInfo(ODataItem item)
        {
            Debug.Assert(item != null, "item != null");

            ODataResourceSerializationInfo serializationInfo = null;

            var resource = item as ODataResource;
            if (resource != null)
            {
                serializationInfo = resource.SerializationInfo;
            }

            var deltaDeletedEntry = item as ODataDeltaDeletedEntry;
            if (deltaDeletedEntry != null)
            {
                serializationInfo = DeltaConverter.ToResourceSerializationInfo(deltaDeletedEntry.SerializationInfo);
            }

            if (serializationInfo == null)
            {
                serializationInfo = DeltaConverter.ToResourceSerializationInfo(this.GetParentResourceSetSerializationInfo());
            }

            return serializationInfo;
        }

        /// <summary>
        /// Gets the serialization info for the given delta link.
        /// </summary>
        /// <param name="item">The resource to get the serialization info for.</param>
        /// <returns>The serialization info for the given resource.</returns>
        private ODataDeltaSerializationInfo GetLinkSerializationInfo(ODataItem item)
        {
            Debug.Assert(item != null, "item != null");

            ODataDeltaSerializationInfo serializationInfo = null;

            var deltaLink = item as ODataDeltaLink;
            if (deltaLink != null)
            {
                serializationInfo = deltaLink.SerializationInfo;
            }

            var deltaDeletedLink = item as ODataDeltaDeletedLink;
            if (deltaDeletedLink != null)
            {
                serializationInfo = deltaDeletedLink.SerializationInfo;
            }

            return serializationInfo ?? this.GetParentResourceSetSerializationInfo();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        private static bool IsErrorState(WriterState state)
        {
            return state == WriterState.Error;
        }

        /// <summary>
        /// Determines whether a given writer is writing expanded navigation property, complex property or complex collection property.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer is writing expanded navigation property, complex property or complex collection property; otherwise false.</returns>
        private static bool IsNestedResourceState(WriterState state)
        {
            return state == WriterState.NestedResource;
        }

        #endregion

        #region Private Classes

        #region Scope Classes

        /// <summary>
        /// A writer scope; keeping track of the current writer state and an item associated with this state.
        /// </summary>
        private class Scope
        {
            /// <summary>The writer state of this scope.</summary>
            private readonly WriterState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>The selected properties for the current scope.</summary>
            private readonly SelectedPropertiesNode selectedProperties;

            /// <summary>The odata uri info for current scope.</summary>
            private readonly ODataUri odataUri;

            /// <summary>
            /// Constructor creating a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
            /// <param name="resourceType">The structured type of the items in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public Scope(WriterState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
            {
                this.state = state;
                this.item = item;
                this.ResourceType = resourceType;
                this.NavigationSource = navigationSource;
                this.selectedProperties = selectedProperties;
                this.odataUri = odataUri;
            }

            /// <summary>
            /// The structured type of the items in the resource set to be written (or null if the entity set base type should be used).
            /// </summary>
            public IEdmStructuredType ResourceType { get; set; }

            /// <summary>
            /// The writer state of this scope.
            /// </summary>
            public WriterState State
            {
                get
                {
                    return this.state;
                }
            }

            /// <summary>
            /// The item attached to this scope.
            /// </summary>
            public ODataItem Item
            {
                get
                {
                    return this.item;
                }
            }

            /// <summary>
            /// The navigation source we are going to write entities for.
            /// </summary>
            public IEdmNavigationSource NavigationSource { get; private set; }

            /// <summary>
            /// The selected properties for the current scope.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties
            {
                get
                {
                    Debug.Assert(this.selectedProperties != null, "this.selectedProperties != null");
                    return this.selectedProperties;
                }
            }

            /// <summary>
            /// The odata Uri for the current scope.
            /// </summary>
            public ODataUri ODataUri
            {
                get
                {
                    Debug.Assert(this.odataUri != null, "this.odataUri != null");
                    return this.odataUri;
                }
            }
        }

        /// <summary>
        /// Base class for DeltaResourceScope and DeltaDeletedResourceScope.
        /// </summary>
        private abstract class DeltaResourceScope : Scope
        {
            /// <summary>Checker to detect duplicate property names.</summary>
            private readonly IDuplicatePropertyNameChecker duplicatePropertyNameChecker;

            /// <summary>The serialization info for the current resource.</summary>
            private readonly ODataResourceSerializationInfo serializationInfo;

            /// <summary>The type context to answer basic questions regarding the type info of the resource.</summary>
            private ODataResourceTypeContext typeContext;

            /// <summary>Maintains the write status for each annotation using its key.</summary>
            private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;

            /// <summary>
            /// Constructor to create a new resource scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="resource">The resource for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="writerSettings">The <see cref="ODataMessageWriterSettings"/> The settings of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            protected DeltaResourceScope(WriterState state, ODataItem resource, ODataResourceSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, ODataMessageWriterSettings writerSettings, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(state, resource, navigationSource, entityType, selectedProperties, odataUri)
            {
                Debug.Assert(resource != null, "resource != null");
                Debug.Assert(
                    state == WriterState.DeltaResource && resource is ODataResource ||
                    state == WriterState.DeltaDeletedEntry && resource is ODataDeltaDeletedEntry,
                    "resource must be either DeltaResource or DeltaDeletedEntry.");
                Debug.Assert(writerSettings != null, "writerSettings != null");

                duplicatePropertyNameChecker = writerSettings.Validator.CreateDuplicatePropertyNameChecker();
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The entity type which was derived from the model (may be either the same as entity type or its base type.
            /// </summary>
            public IEdmStructuredType ResourceTypeFromMetadata { get; set; }

            /// <summary>
            /// The serialization info for the current resource.
            /// </summary>
            public ODataResourceSerializationInfo SerializationInfo
            {
                get { return serializationInfo; }
            }

            /// <summary>
            /// Checker to detect duplicate property names.
            /// </summary>
            public IDuplicatePropertyNameChecker DuplicatePropertyNameChecker
            {
                get
                {
                    return duplicatePropertyNameChecker;
                }
            }

            /// <summary>
            /// Tracks the write status of the annotations.
            /// </summary>
            public InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
            {
                get
                {
                    if (this.instanceAnnotationWriteTracker == null)
                    {
                        this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
                    }

                    return this.instanceAnnotationWriteTracker;
                }
            }

            /// <summary>
            /// Gets or creates the type context to answer basic questions regarding the type info of the resource.
            /// </summary>
            /// <param name="writingResponse">Whether writing Json payload. Should always be true.</param>
            /// <returns>The type context to answer basic questions regarding the type info of the resource.</returns>
            public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse = true)
            {
                if (this.typeContext == null)
                {
                    this.typeContext = ODataResourceTypeContext.Create(
                        this.serializationInfo,
                        this.NavigationSource,
                        EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource),
                        this.ResourceTypeFromMetadata ?? this.ResourceType,
                        writingResponse);
                }

                return this.typeContext;
            }
        }

        /// <summary>
        /// A scope for a delta resource set.
        /// </summary>
        private abstract class DeltaResourceSetScope : Scope
        {
            /// <summary>The serialization info for the current resource set.</summary>
            private readonly ODataDeltaResourceSetSerializationInfo serializationInfo;

            /// <summary>Maintains the write status for each annotation using its key.</summary>
            private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;

            /// <summary>The type context to answer basic questions regarding the type info of the resource set.</summary>
            private ODataResourceTypeContext typeContext;

            /// <summary>
            /// Constructor to create a new resource set scope.
            /// </summary>
            /// <param name="item">The resource set for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
            /// <param name="resourceType">The structured type of the items in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            protected DeltaResourceSetScope(ODataItem item, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(WriterState.DeltaResourceSet, item, navigationSource, resourceType, selectedProperties, odataUri)
            {
                Debug.Assert(item != null, "item != null");

                var resourceSet = item as ODataDeltaResourceSet;
                Debug.Assert(resourceSet != null, "resource set must be DeltaResourceSet.");

                this.serializationInfo = resourceSet.SerializationInfo;
            }

            /// <summary>
            /// Tracks the write status of the annotations.
            /// </summary>
            public InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
            {
                get
                {
                    if (this.instanceAnnotationWriteTracker == null)
                    {
                        this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
                    }

                    return this.instanceAnnotationWriteTracker;
                }
            }

            /// <summary>
            /// The context uri info created for this scope.
            /// </summary>
            public ODataContextUrlInfo ContextUriInfo { get; set; }

            /// <summary>
            /// Gets or creates the type context to answer basic questions regarding the type info of the resource.
            /// </summary>
            /// <param name="writingResponse">Whether writing Json payload. Should always be true.</param>
            /// <returns>The type context to answer basic questions regarding the type info of the resource.</returns>
            public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse = true)
            {
                if (this.typeContext == null)
                {
                    this.typeContext = ODataResourceTypeContext.Create(
                        DeltaConverter.ToResourceSerializationInfo(this.serializationInfo),
                        this.NavigationSource,
                        EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource),
                        this.ResourceType,
                        writingResponse);
                }

                return this.typeContext;
            }
        }

        /// <summary>
        /// A scope for a delta link.
        /// </summary>
        private abstract class DeltaLinkScope : Scope
        {
            /// <summary>The serialization info for the current link.</summary>
            private readonly ODataResourceSerializationInfo serializationInfo;

            /// <summary>
            /// Fake entity type to be passed to context.
            /// </summary>
            private readonly EdmEntityType fakeEntityType = new EdmEntityType("MyNS", "Fake");

            /// <summary>The type context to answer basic questions regarding the type info of the link.</summary>
            private ODataResourceTypeContext typeContext;

            /// <summary>
            /// Constructor to create a new delta link scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="link">The link for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            protected DeltaLinkScope(WriterState state, ODataItem link, ODataDeltaSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(state, link, navigationSource, entityType, selectedProperties, odataUri)
            {
                Debug.Assert(link != null, "link != null");
                Debug.Assert(
                    state == WriterState.DeltaLink && link is ODataDeltaLink ||
                    state == WriterState.DeltaDeletedLink && link is ODataDeltaDeletedLink,
                    "link must be either DeltaLink or DeltaDeletedLink.");

                this.serializationInfo = DeltaConverter.ToResourceSerializationInfo(serializationInfo);
            }

            /// <summary>
            /// Gets or creates the type context to answer basic questions regarding the type info of the resource.
            /// </summary>
            /// <param name="writingResponse">Whether writing Json payload. Should always be true.</param>
            /// <returns>The type context to answer basic questions regarding the type info of the resource.</returns>
            public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse = true)
            {
                if (this.typeContext == null)
                {
                    this.typeContext = ODataResourceTypeContext.Create(
                        this.serializationInfo,
                        this.NavigationSource,
                        EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource),
                        this.fakeEntityType,
                        writingResponse);
                }

                return this.typeContext;
            }
        }

        /// <summary>
        /// A scope for a nested resource info.
        /// </summary>
        private abstract class NestedResourceInfoScope : Scope
        {
            /// <summary>
            /// Constructor to create a new nested resource info scope.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info for the resource set.</param>
            /// <param name="navigationSource">The navigation source of the parent delta resource.</param>
            /// <param name="resourceType">The structured type of the parent delta resource.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            protected NestedResourceInfoScope(ODataItem nestedResourceInfo, IEdmNavigationSource navigationSource,
                IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(WriterState.NestedResource, nestedResourceInfo, navigationSource, resourceType, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A scope for a delta resource in JSON Light writer.
        /// </summary>
        private sealed class JsonLightDeltaResourceScope : DeltaResourceScope, IODataJsonLightWriterResourceState
        {
            /// <summary>Bit field of the JSON Light metadata properties written so far.</summary>
            private int alreadyWrittenMetadataProperties;

            /// <summary>
            /// Constructor to create a new resource scope.
            /// </summary>
            /// <param name="state">The writer state of the scope to create.</param>
            /// <param name="resource">The resource for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="writerSettings">The <see cref="ODataMessageWriterSettings"/> The settings of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public JsonLightDeltaResourceScope(WriterState state, ODataItem resource, ODataResourceSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, ODataMessageWriterSettings writerSettings, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(state, resource, serializationInfo, navigationSource, entityType, writerSettings, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// The resource being written.
            /// </summary>
            public ODataResource Resource
            {
                get { return (ODataResource)this.Item; }
            }

            /// <summary>
            /// This is always the top level resource from some entity set.
            /// </summary>
            public bool IsUndeclared
            {
                get { return false; }
            }

            /// <summary>
            /// Flag which indicates that the odata.editLink metadata property has been written.
            /// </summary>
            public bool EditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonLightMetadataProperty"/> as written in this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadta property which was written.</param>
            private void SetWrittenMetadataProperty(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonLightMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonLightMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonLightMetadataProperty) == (int)jsonLightMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a nested resource info in JSON Light writer.
        /// </summary>
        private sealed class JsonLightNestedResourceInfoScope : NestedResourceInfoScope
        {
            /// <summary>
            /// The writer for writing nested resource info in delta response.
            /// </summary>
            private JsonLightExpandedNavigationPropertyWriter jsonLightExpandedNavigationPropertyWriter;

            /// <summary>
            /// Constructor to create a new nested resource info scope.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info for the resource set.</param>
            /// <param name="navigationSource">The navigation source of the parent delta resource.</param>
            /// <param name="resourceType">The structured type of this nested resource.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="parentDeltaResource">The parent delta resource.</param>
            /// <param name="jsonLightOutputContext">The output context for Json.</param>
            public JsonLightNestedResourceInfoScope(ODataItem nestedResourceInfo, IEdmNavigationSource navigationSource,
                IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri,
                ODataResource parentDeltaResource, ODataJsonLightOutputContext jsonLightOutputContext)
                : base(nestedResourceInfo, navigationSource, resourceType, selectedProperties, odataUri)
            {
                this.jsonLightExpandedNavigationPropertyWriter = new JsonLightExpandedNavigationPropertyWriter(navigationSource, resourceType, parentDeltaResource, jsonLightOutputContext);
            }

            /// <summary>
            /// The writer for writing expanded navigation property in delta response.
            /// </summary>
            public JsonLightExpandedNavigationPropertyWriter JsonLightExpandedNavigationPropertyWriter
            {
                get { return this.jsonLightExpandedNavigationPropertyWriter; }
            }
        }

        /// <summary>
        /// A scope for a JSON lite resource set.
        /// </summary>
        private sealed class JsonLightDeltaResourceSetScope : DeltaResourceSetScope
        {
            /// <summary>
            /// Constructor to create a new resource set scope.
            /// </summary>
            /// <param name="resourceSet">The resource set for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="resourceType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public JsonLightDeltaResourceSetScope(ODataItem resourceSet, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(resourceSet, navigationSource, resourceType, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// true if the odata.nextLink annotation was already written, false otherwise.
            /// </summary>
            public bool NextPageLinkWritten { get; set; }

            /// <summary>
            /// true if the odata.deltaLink annotation was already written, false otherwise.
            /// </summary>
            public bool DeltaLinkWritten { get; set; }
        }

        /// <summary>
        /// A scope for a delta link in JSON Light writer.
        /// </summary>
        private sealed class JsonLightDeltaLinkScope : DeltaLinkScope
        {
            /// <summary>
            /// Constructor to create a new delta link scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="link">The link for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public JsonLightDeltaLinkScope(WriterState state, ODataItem link, ODataDeltaSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(state, link, serializationInfo, navigationSource, entityType, selectedProperties, odataUri)
            {
            }
        }

        #endregion

        #region ScopeStack Class

        /// <summary>
        /// Lightweight wrapper for the stack of scopes which exposes a few helper properties for getting parent scopes.
        /// </summary>
        private sealed class ScopeStack
        {
            /// <summary>
            /// Use a list to store the scopes instead of a true stack so that parent/grandparent lookups will be fast.
            /// </summary>
            private readonly Stack<Scope> scopes = new Stack<Scope>();

            /// <summary>
            /// Gets the count of items in the stack.
            /// </summary>
            public int Count
            {
                get
                {
                    return this.scopes.Count;
                }
            }

            /// <summary>
            /// Pushes the specified scope onto the stack.
            /// </summary>
            /// <param name="scope">The scope.</param>
            public void Push(Scope scope)
            {
                Debug.Assert(scope != null, "scope != null");
                this.scopes.Push(scope);
            }

            /// <summary>
            /// Pops the current scope off the stack.
            /// </summary>
            /// <returns>The popped scope.</returns>
            public Scope Pop()
            {
                Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
                return this.scopes.Pop();
            }

            /// <summary>
            /// Peeks at the current scope on the top of the stack.
            /// </summary>
            /// <returns>The current scope at the top of the stack.</returns>
            public Scope Peek()
            {
                Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
                return this.scopes.Peek();
            }
        }

        #endregion

        #region DeltaConverter Class

        /// <summary>
        /// This converter is to provide various conversion methods for resource, resource set, serialization info, etc.
        /// TODO: Review the conversion logic in this class.
        /// </summary>
        private static class DeltaConverter
        {
            /// <summary>
            /// Convert DeltaSerializationInfo to ResourceSerializationInfo.
            /// </summary>
            /// <param name="serializationInfo">The DeltaSerializationInfo to convert.</param>
            /// <returns>The converted ResourceSerializationInfo.</returns>
            public static ODataResourceSerializationInfo ToResourceSerializationInfo(ODataDeltaSerializationInfo serializationInfo)
            {
                if (serializationInfo == null)
                {
                    return null;
                }

                return new ODataResourceSerializationInfo
                {
                    NavigationSourceName = serializationInfo.NavigationSourceName,
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceEntityTypeName = JsonConstants.JsonNullLiteral, // Won't write out in delta (deleted) entries.
                    ExpectedTypeName = JsonConstants.JsonNullLiteral // Same as above.
                };
            }

            /// <summary>
            /// Convert DeltaResourceSetSerializationInfo to ODataResourceSerializationInfo.
            /// </summary>
            /// <param name="serializationInfo">The DeltaResourceSetSerializationInfo to convert.</param>
            /// <returns>The converted ODataResourceSerializationInfo.</returns>
            public static ODataResourceSerializationInfo ToResourceSerializationInfo(ODataDeltaResourceSetSerializationInfo serializationInfo)
            {
                if (serializationInfo == null)
                {
                    return null;
                }

                return new ODataResourceSerializationInfo
                {
                    NavigationSourceName = serializationInfo.EntitySetName,
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceEntityTypeName = serializationInfo.EntityTypeName,
                    ExpectedTypeName = serializationInfo.ExpectedTypeName
                };
            }

            /// <summary>
            /// Convert DeltaResourceSetSerializationInfo to DeltaSerializationInfo.
            /// </summary>
            /// <param name="serializationInfo">The DeltaResourceSetSerializationInfo to convert.</param>
            /// <returns>The converted DeltaSerializationInfo.</returns>
            public static ODataDeltaSerializationInfo ToDeltaSerializationInfo(ODataDeltaResourceSetSerializationInfo serializationInfo)
            {
                if (serializationInfo == null)
                {
                    return null;
                }

                return new ODataDeltaSerializationInfo { NavigationSourceName = serializationInfo.EntitySetName };
            }

            /// <summary>
            /// Convert DeltaResourceSet to ODataResourceSet.
            /// </summary>
            /// <param name="deltaResourceSet">The DeltaResourceSet to convert.</param>
            /// <returns>The converted Resource Set.</returns>
            public static ODataResourceSet ToODataResourceSet(ODataDeltaResourceSet deltaResourceSet)
            {
                Debug.Assert(deltaResourceSet != null, "deltaResourceSet != null");

                var result = Clone(deltaResourceSet);

                result.SetSerializationInfo(ToResourceSerializationInfo(deltaResourceSet.SerializationInfo));

                return result;
            }

            /// <summary>
            /// Create an ODataResourceSet cloning from an ODataResourceSetBase.
            /// </summary>
            /// <param name="resourceSetBase">The resource set to be cloned.</param>
            /// <returns>The created resource set.</returns>
            private static ODataResourceSet Clone(ODataResourceSetBase resourceSetBase)
            {
                Debug.Assert(resourceSetBase != null, "resourceSetBase != null");

                return new ODataResourceSet
                {
                    Count = resourceSetBase.Count,
                    DeltaLink = resourceSetBase.DeltaLink,
                    Id = resourceSetBase.Id,
                    InstanceAnnotations = resourceSetBase.InstanceAnnotations,
                    NextPageLink = resourceSetBase.NextPageLink
                };
            }
        }

        #endregion

        #region JsonLightExpandedNavigationPropertyWriter Class

        /// <summary>
        /// The writer for writing expanded navigation property in delta response.
        /// </summary>
        private sealed class JsonLightExpandedNavigationPropertyWriter
        {
            /// <summary>
            /// The resource writer to write entries, resource sets and navigation links.
            /// </summary>
            private readonly ODataWriter resourceWriter;

            /// <summary>
            /// The parent delta resource.
            /// </summary>
            private readonly ODataResource parentDeltaResource;

            /// <summary>
            /// Current depth of the resource writer.
            /// </summary>
            private int currentResourceDepth;

            /// <summary>
            /// Constructor to create an <see cref="JsonLightExpandedNavigationPropertyWriter"/>.
            /// </summary>
            /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
            /// <param name="resourceType">The structured type of the nested resource.</param>
            /// <param name="parentDeltaResource">The parent delta resource.</param>
            /// <param name="jsonLightOutputContext">The output context for Json.</param>
            public JsonLightExpandedNavigationPropertyWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType,
                ODataResource parentDeltaResource, ODataJsonLightOutputContext jsonLightOutputContext)
            {
                this.parentDeltaResource = parentDeltaResource;
                this.resourceWriter = new ODataJsonLightWriter(jsonLightOutputContext, navigationSource, resourceType, /*writingResourceSet*/ false, writingDelta: true);
            }

            /// <summary>
            /// Starts the writing of a resource.
            /// </summary>
            /// <param name="resource">The resource to write.</param>
            public void WriteStart(ODataResource resource)
            {
                this.IncreaseResourceDepth();
                this.resourceWriter.WriteStart(resource);
            }

            /// <summary>
            /// Starts the writing of a resource set.
            /// </summary>
            /// <param name="resourceSet">The resource set to write.</param>
            public void WriteStart(ODataResourceSet resourceSet)
            {
                this.IncreaseResourceDepth();
                this.resourceWriter.WriteStart(resourceSet);
            }

            /// <summary>
            /// Starts the writing of a nested resource info.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info to write.</param>
            public void WriteStart(ODataNestedResourceInfo nestedResourceInfo)
            {
                this.IncreaseResourceDepth();
                this.resourceWriter.WriteStart(nestedResourceInfo);
            }

            /// <summary>
            /// Finishes the writing of a resource set, a resource, or a nested resource info.
            /// </summary>
            /// <returns>True if the status of the resource writer is completed; false otherwise.</returns>
            public bool WriteEnd()
            {
                // This is to match WriteStart(parentDeltaResource).
                this.resourceWriter.WriteEnd();
                return this.DecreaseResourceDepth();
            }

            /// <summary>
            /// Increase the depth of the resource writer.
            /// </summary>
            private void IncreaseResourceDepth()
            {
                if (this.currentResourceDepth == 0)
                {
                    // This is to ensure the underlying writer has the correct state and scope to
                    // write a nested resource info. The structural properties and instance annotations
                    // of the delta resource will NOT actually be written to the payload. Only the
                    // navigation links, expanded resource sets and expanded entries will be written.
                    this.resourceWriter.WriteStart(this.parentDeltaResource);
                }

                this.currentResourceDepth++;
            }

            /// <summary>
            /// Decrease the depth of the resource writer.
            /// </summary>
            /// <returns>True if the status of the resource writer is completed; false otherwise.</returns>
            private bool DecreaseResourceDepth()
            {
                this.currentResourceDepth--;

                if (this.currentResourceDepth == 0)
                {
                    // Match WriteStart(this.parentDeltaResource).
                    this.resourceWriter.WriteEnd();
                    return true;
                }

                return false;
            }
        }

        #endregion

        #endregion
    }
}
