//---------------------------------------------------------------------
// <copyright file="ODataReaderCore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData readers that verifies a proper sequence of read calls on the reader.
    /// </summary>
    internal abstract class ODataReaderCore : ODataReader, IODataStreamListener
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>true if the reader is created for reading a resource set; false when it is created for reading a resource.</summary>
        private readonly bool readingResourceSet;

        /// <summary>true if the reader is created for reading expanded navigation property in delta response; false otherwise.</summary>
        private readonly bool readingDelta;

        /// <summary>Stack of reader scopes to keep track of the current context of the reader.</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</summary>
        private readonly IODataReaderWriterListener listener;

        /// <summary>The number of entries which have been started but not yet ended.</summary>
        private int currentResourceDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read the payload from.</param>
        /// <param name="readingResourceSet">true if the reader is created for reading a resource set; false when it is created for reading a resource.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataReaderCore(
            ODataInputContext inputContext,
            bool readingResourceSet,
            bool readingDelta,
            IODataReaderWriterListener listener)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.inputContext = inputContext;
            this.readingResourceSet = readingResourceSet;
            this.readingDelta = readingDelta;
            this.listener = listener;
            this.currentResourceDepth = 0;
            this.Version = inputContext.MessageReaderSettings.Version;
        }

        /// <summary>
        /// Enum used to describe the current state of the stream.
        /// </summary>
        internal enum StreamingState
        {
            None = 0,
            Streaming,
            Completed
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override sealed ODataReaderState State
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().State;
            }
        }

        /// <summary>
        /// The most recent <see cref="ODataItem"/> that has been read.
        /// </summary>
        public override sealed ODataItem Item
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().Item;
            }
        }

        /// <summary>
        /// OData Version being read.
        /// </summary>
        internal ODataVersion? Version { get; }

        /// <summary>
        /// Returns the current item as <see cref="ODataResourceSet"/>. Must only be called if the item actually is a resource set.
        /// </summary>
        protected ODataResourceSet CurrentResourceSet
        {
            get
            {
                Debug.Assert(this.Item is ODataResourceSet, "this.Item is ODataResourceSet");
                return (ODataResourceSet)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataDeltaResourceSet"/>. Must only be called if the item actually is a delta resource set.
        /// </summary>
        protected ODataDeltaResourceSet CurrentDeltaResourceSet
        {
            get
            {
                Debug.Assert(this.Item is ODataDeltaResourceSet, "this.Item is ODataDeltaResourceSet");
                return (ODataDeltaResourceSet)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as ODataDeltaLink
        /// </summary>
        protected ODataDeltaLink CurrentDeltaLink
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataDeltaLink, "this.Item is ODataDeltaLink");
                return (ODataDeltaLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as ODataDeltaDeletedLink.
        /// </summary>
        protected ODataDeltaDeletedLink CurrentDeltaDeletedLink
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataDeltaDeletedLink, "this.Item is ODataDeltaDeletedLink");
                return (ODataDeltaDeletedLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the current resource depth.
        /// </summary>
        protected int CurrentResourceDepth
        {
            get
            {
                return this.currentResourceDepth;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataNestedResourceInfo"/>. Must only be called if the item actually is a nested resource info.
        /// </summary>
        protected ODataNestedResourceInfo CurrentNestedResourceInfo
        {
            get
            {
                Debug.Assert(this.Item is ODataNestedResourceInfo, "this.Item is ODataNestedResourceInfo");
                return (ODataNestedResourceInfo)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataEntityReferenceLink"/>. Must only be called if the item actually is an entity reference link.
        /// </summary>
        protected ODataEntityReferenceLink CurrentEntityReferenceLink
        {
            get
            {
                Debug.Assert(this.Item is ODataEntityReferenceLink, "this.Item is ODataEntityReferenceLink");
                return (ODataEntityReferenceLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the expected resource type for the current scope.
        /// </summary>
        protected IEdmType CurrentResourceType
        {
            get
            {
                if (CurrentResourceTypeReference != null)
                {
                    return CurrentResourceTypeReference.Definition;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets and Sets the expected resource type reference for the current scope.
        /// </summary>
        protected IEdmTypeReference CurrentResourceTypeReference
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmTypeReference resourceTypeReference = this.scopes.Peek().ResourceTypeReference;
                Debug.Assert(resourceTypeReference == null || this.inputContext.Model.IsUserModel(), "We can only have structured type if we also have metadata.");

                return resourceTypeReference;
            }

            set
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                this.scopes.Peek().ResourceTypeReference = value;
            }
        }

        /// <summary>
        /// Returns the navigation source for the current scope.
        /// </summary>
        protected IEdmNavigationSource CurrentNavigationSource
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmNavigationSource navigationSource = this.scopes.Peek().NavigationSource;
                Debug.Assert(navigationSource == null || this.inputContext.Model.IsUserModel(), "We can only have navigation source if we also have metadata.");
                return navigationSource;
            }
        }

        /// <summary>
        /// Returns the current scope.
        /// </summary>
        protected Scope CurrentScope
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek();
            }
        }

        protected Stack<Scope> Scopes
        {
            get { return this.scopes; }
        }

        /// <summary>
        /// Returns the parent scope.
        /// </summary>
        protected Scope ParentScope
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 1, "We must have at least two scopes in the stack.");
                return this.scopes.Skip(1).First();
            }
        }

        /// <summary>
        /// A flag indicating whether the reader is at the top level.
        /// </summary>
        protected bool IsTopLevel
        {
            get
            {
                Debug.Assert(this.scopes != null, "Scopes must exist.");

                // there is the root scope at the top (when the writer has not started or has completed)
                // and then the top-level scope (the top-level resource/resource set item) as the second scope on the stack
                return this.scopes.Count <= 2;
            }
        }

        /// <summary>
        /// If the current scope is a content of an expanded link, this returns the parent nested resource info scope, otherwise null.
        /// </summary>
        protected Scope ExpandedLinkContentParentScope
        {
            get
            {
                Debug.Assert(this.scopes != null, "this.scopes != null");
                if (this.scopes.Count > 1)
                {
                    Scope parentScope = this.scopes.Skip(1).First();
                    if (parentScope.State == ODataReaderState.NestedResourceInfoStart)
                    {
                        return parentScope;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// True if we are reading a resource or resource set that is the direct content of an expanded link. Otherwise false.
        /// </summary>
        protected bool IsExpandedLinkContent
        {
            get
            {
                return this.ExpandedLinkContentParentScope != null;
            }
        }

        /// <summary>
        /// Set to true if a resource set is being read.
        /// </summary>
        protected bool ReadingResourceSet
        {
            get
            {
                return this.readingResourceSet;
            }
        }

        /// <summary>
        /// Set to true if a delta response is being read.
        /// </summary>
        protected bool ReadingDelta
        {
            get
            {
                return this.readingDelta;
            }
        }

        /// <summary>
        /// Returns true if we are reading a nested payload,
        /// e.g. an expanded resource or resource set within a delta payload,
        /// or a resource or a resource set within a parameters payload.
        /// </summary>
        protected bool IsReadingNestedPayload
        {
            get
            {
                return this.listener != null;
            }
        }

        /// <summary>
        /// Validator to validate consistency of entries in top-level resource sets.
        /// </summary>
        /// <remarks>We only use this for top-level resource sets since we support collection validation for
        /// resource sets only when metadata is available and in these cases we already validate the
        /// types of the entries in nested resource sets.</remarks>
        protected ResourceSetWithoutExpectedTypeValidator CurrentResourceSetValidator
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.DeletedResourceStart, "CurrentResourceSetValidator should only be called while reading a resource.");
                return this.ParentScope == null ? null : this.ParentScope.ResourceTypeValidator;
            }
        }

        /// <summary>
        /// Validator to validate the derived type constraint.
        /// </summary>
        protected DerivedTypeValidator CurrentDerivedTypeValidator
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.DeletedResourceStart, "CurrentDerivedTypeValidator should only be called while reading a resource.");
                return this.ParentScope == null ? null : this.ParentScope.DerivedTypeValidator;
            }
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override sealed bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException(this.ReadSynchronously);
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override sealed Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.EnterScope(new Scope(ODataReaderState.Exception, null, null)));
        }

        /// <summary>
        /// Creates a stream for reading an inline stream property.
        /// </summary>
        /// <returns>A stream for reading the stream property.</returns>
        public override sealed Stream CreateReadStream()
        {
            if (this.State != ODataReaderState.Stream)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
            }

            StreamScope scope = this.CurrentScope as StreamScope;
            Debug.Assert(scope != null, "ODataReaderState.Stream when Scope is not a StreamScope");
            if (scope.StreamingState != StreamingState.None)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
            }

            scope.StreamingState = StreamingState.Streaming;
            return new ODataNotificationStream(this.InterceptException(this.CreateReadStreamImplementation), this);
        }

        /// <summary>
        /// Creates a TextWriter for reading an inline stream property.
        /// </summary>
        /// <returns>A TextWriter for reading the stream property.</returns>
        public override sealed TextReader CreateTextReader()
        {
            if (this.State == ODataReaderState.Stream)
            {
                StreamScope scope = this.CurrentScope as StreamScope;
                Debug.Assert(scope != null, "ODataReaderState.Stream when Scope is not a StreamScope");
                if (scope.StreamingState != StreamingState.None)
                {
                    throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
                }

                scope.StreamingState = StreamingState.Streaming;
                return new ODataNotificationReader(this.InterceptException(this.CreateTextReaderImplementation), this);
            }
            else
            {
                throw new ODataException(Strings.ODataReaderCore_CreateTextReaderCalledInInvalidState);
            }
        }

        /// <summary>
        /// This method is called when a stream is requested. It is a no-op.
        /// </summary>
        void IODataStreamListener.StreamRequested()
        {
        }

        /// <summary>
        /// This method is called when an async stream is requested. It is a no-op.
        /// </summary>
        /// <returns>A task for method called when a stream is requested.</returns>
        Task IODataStreamListener.StreamRequestedAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => ((IODataStreamListener)this).StreamRequested());
        }

        /// <summary>
        /// This method is called when a stream is disposed.
        /// </summary>
        void IODataStreamListener.StreamDisposed()
        {
            Debug.Assert(this.State == ODataReaderState.Stream, "Stream was disposed when not in ReaderState.Stream state.");
            StreamScope scope = this.CurrentScope as StreamScope;
            Debug.Assert(scope != null, "Stream disposed when not in stream scope");
            Debug.Assert(scope.StreamingState == StreamingState.Streaming, "StreamDisposed called when reader was not streaming");
            scope.StreamingState = StreamingState.Completed;
        }

        /// <summary>
        /// This method is called asynchronously when a stream is disposed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task IODataStreamListener.StreamDisposedAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Seek scope in the stack which is type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of scope to seek.</typeparam>
        /// <param name="maxDepth">The max depth to seek.</param>
        /// <returns>The scope with type of <typeparamref name="T"/></returns>
        internal Scope SeekScope<T>(int maxDepth) where T : Scope
        {
            int count = 1;

            foreach (Scope scope in this.scopes)
            {
                if (count > maxDepth)
                {
                    return null;
                }

                if (scope is T)
                {
                    return scope;
                }

                count++;
            }

            return null;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtResourceSetStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtResourceSetEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtResourceStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtResourceEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtPrimitiveImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtNestedPropertyInfoImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtStreamImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Stream for reading a stream property when in state 'Stream'.
        /// </summary>
        /// <returns>A stream for reading the stream property.</returns>
        protected virtual Stream CreateReadStreamImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a TextReader for reading a string property when in state 'Text'.
        /// </summary>
        /// <returns>A TextReader for reading the string property.</returns>
        protected virtual TextReader CreateTextReaderImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtNestedResourceInfoStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtNestedResourceInfoEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtEntityReferenceLink();

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeltaResourceSetStartImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeltaResourceSetEndImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeletedResourceStartImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeletedResourceEndImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeltaLinkImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected virtual bool ReadAtDeltaDeletedLinkImplementation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pushes the <paramref name="scope"/> on the stack of scopes.
        /// </summary>
        /// <param name="scope">The scope to enter.</param>
        protected void EnterScope(Scope scope)
        {
            Debug.Assert(scope != null, "scope != null");

            if ((scope.State == ODataReaderState.ResourceSetStart || scope.State == ODataReaderState.DeltaResourceSetStart)
                && this.inputContext.Model.IsUserModel())
            {
                scope.ResourceTypeValidator = new ResourceSetWithoutExpectedTypeValidator(scope.ResourceType);
            }

            if (scope.State == ODataReaderState.ResourceSetStart || scope.State == ODataReaderState.DeltaResourceSetStart)
            {
                scope.DerivedTypeValidator = this.CurrentScope.DerivedTypeValidator;
            }

            // TODO: implement some basic validation that the transitions are ok
            this.scopes.Push(scope);
            if (this.listener != null)
            {
                if (scope.State == ODataReaderState.Exception)
                {
                    this.listener.OnException();
                }
                else if (scope.State == ODataReaderState.Completed)
                {
                    this.listener.OnCompleted();
                }
            }
        }

        /// <summary>
        /// Replaces the current scope with the specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope">The scope to replace the current scope with.</param>
        protected void ReplaceScope(Scope scope)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");
            Debug.Assert(scope != null, "scope != null");
            Debug.Assert(scope.State != ODataReaderState.ResourceEnd, "Call EndEntry instead.");

            // TODO: implement some basic validation that the transitions are ok
            this.scopes.Pop();
            this.EnterScope(scope);
        }

        /// <summary>
        /// Removes the current scope from the stack of all scopes.
        /// </summary>
        /// <param name="state">The expected state of the current scope (to be popped).</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
        protected void PopScope(ODataReaderState state)
        {
            Debug.Assert(this.scopes.Count > 1, "Stack must have more than 1 items in order to pop an item.");

            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.State == state, "scope.State == state");
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        /// <param name="scope">The scope for the EntryEnd state.</param>
        protected void EndEntry(Scope scope)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");
            Debug.Assert(scope != null, "scope != null");
            Debug.Assert(scope.State == ODataReaderState.ResourceEnd | scope.State == ODataReaderState.DeletedResourceEnd, "Called EndEntry when not in ResourceEnd or DeletedResourceEnd state");

            this.scopes.Pop();
            this.EnterScope(scope);
        }

        /// <summary>
        /// If an entity type name is found in the payload this method is called to apply it to the current scope.
        /// This method should be called even if the type name was not found in which case a null should be passed in.
        /// The method validates that some type will be available as the current entity type after it returns (if we are parsing using metadata).
        /// </summary>
        /// <param name="resourceTypeNameFromPayload">The entity type name found in the payload or null if no type was specified in the payload.</param>
        protected void ApplyResourceTypeNameFromPayload(string resourceTypeNameFromPayload)
        {
            Debug.Assert(
                this.scopes.Count > 0 && this.scopes.Peek().Item is ODataResourceBase,
                "Resource type can be applied only when in resource scope.");

            ODataTypeAnnotation typeAnnotation;
            EdmTypeKind targetTypeKind;
            IEdmStructuredTypeReference targetResourceTypeReference =
                this.inputContext.MessageReaderSettings.Validator.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*expectStructuredType*/ true,
                    /*defaultPrimitivePayloadType*/ null,
                    this.CurrentResourceTypeReference,
                    resourceTypeNameFromPayload,
                    this.inputContext.Model,
                    () => EdmTypeKind.Entity,
                    out targetTypeKind,
                    out typeAnnotation) as IEdmStructuredTypeReference;

            IEdmStructuredType targetResourceType = null;
            ODataResourceBase resource = this.Item as ODataResourceBase;
            if (targetResourceTypeReference != null)
            {
                targetResourceType = targetResourceTypeReference.StructuredDefinition();
                resource.TypeName = targetResourceType.FullTypeName();

                if (typeAnnotation != null)
                {
                    resource.TypeAnnotation = typeAnnotation;
                }
            }
            else if (resourceTypeNameFromPayload != null)
            {
                resource.TypeName = resourceTypeNameFromPayload;
            }
            else if (this.CurrentResourceTypeReference.IsUntyped())
            {
                targetResourceTypeReference = this.CurrentResourceTypeReference.IsNullable ?
                    EdmUntypedStructuredTypeReference.NullableTypeReference : 
                    EdmUntypedStructuredTypeReference.NonNullableTypeReference;
            }

            // Set the current resource type since the type might be derived from the expected one.
            this.CurrentResourceTypeReference = targetResourceTypeReference;
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected bool ReadSynchronously()
        {
            return this.ReadImplementation();
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        protected virtual Task<bool> ReadAsynchronously()
        {
            // We are reading from the fully buffered read stream here; thus it is ok
            // to use synchronous reads and then return a completed task
            // NOTE: once we switch to fully async reading this will have to change
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadImplementation);
        }

        /// <summary>
        /// Increments the nested resource count by one and fails if the new value exceeds the maxium nested resource depth limit.
        /// </summary>
        protected void IncreaseResourceDepth()
        {
            this.currentResourceDepth++;

            if (this.currentResourceDepth > this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth)
            {
                throw new ODataException(Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth));
            }
        }

        /// <summary>
        /// Decrements the nested resource count by one.
        /// </summary>
        protected void DecreaseResourceDepth()
        {
            Debug.Assert(this.currentResourceDepth > 0, "Resource depth should never become negative.");

            this.currentResourceDepth--;
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        private bool ReadImplementation()
        {
            bool result;
            switch (this.State)
            {
                case ODataReaderState.Start:
                    result = this.ReadAtStartImplementation();
                    break;

                case ODataReaderState.ResourceSetStart:
                    result = this.ReadAtResourceSetStartImplementation();
                    break;

                case ODataReaderState.ResourceSetEnd:
                    result = this.ReadAtResourceSetEndImplementation();
                    break;

                case ODataReaderState.ResourceStart:
                    this.IncreaseResourceDepth();
                    result = this.ReadAtResourceStartImplementation();
                    break;

                case ODataReaderState.ResourceEnd:
                    this.DecreaseResourceDepth();
                    result = this.ReadAtResourceEndImplementation();
                    break;

                case ODataReaderState.Primitive:
                    result = this.ReadAtPrimitiveImplementation();
                    break;

                case ODataReaderState.Stream:
                    result = this.ReadAtStreamImplementation();
                    break;

                case ODataReaderState.NestedProperty:
                    result = this.ReadAtNestedPropertyInfoImplementation();
                    break;

                case ODataReaderState.NestedResourceInfoStart:
                    result = this.ReadAtNestedResourceInfoStartImplementation();
                    break;

                case ODataReaderState.NestedResourceInfoEnd:
                    result = this.ReadAtNestedResourceInfoEndImplementation();
                    break;

                case ODataReaderState.EntityReferenceLink:
                    result = this.ReadAtEntityReferenceLink();
                    break;

                case ODataReaderState.DeltaResourceSetStart:
                    result = this.ReadAtDeltaResourceSetStartImplementation();
                    break;

                case ODataReaderState.DeltaResourceSetEnd:
                    result = this.ReadAtDeltaResourceSetEndImplementation();
                    break;

                case ODataReaderState.DeletedResourceStart:
                    result = this.ReadAtDeletedResourceStartImplementation();
                    break;

                case ODataReaderState.DeletedResourceEnd:
                    result = this.ReadAtDeletedResourceEndImplementation();
                    break;

                case ODataReaderState.DeltaLink:
                    result = this.ReadAtDeltaLinkImplementation();
                    break;

                case ODataReaderState.DeltaDeletedLink:
                    result = this.ReadAtDeltaDeletedLinkImplementation();
                    break;

                case ODataReaderState.Exception:    // fall through
                case ODataReaderState.Completed:
                    throw new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCore_ReadImplementation));
            }

            return result;
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the reader into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <typeparam name="T">The type returned from the <paramref name="action"/> to execute.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of executing the <paramref name="action"/>.</returns>
        private T InterceptException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.EnterScope(new Scope(ODataReaderState.Exception, null, null));
                }

                throw;
            }
        }

        /// <summary>
        /// Verifies that calling Read is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanRead(bool synchronousCall)
        {
            this.inputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State == ODataReaderState.Exception || this.State == ODataReaderState.Completed)
            {
                throw new ODataException(Strings.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(this.State));
            }

            if (this.State == ODataReaderState.Stream)
            {
                StreamScope scope = this.CurrentScope as StreamScope;
                Debug.Assert(scope != null, "In stream state without a stream scope");
                if (scope.StreamingState != StreamingState.Completed)
                {
                    throw new ODataException(Strings.ODataReaderCore_ReadCalledWithOpenStream);
                }
            }
        }

        /// <summary>
        /// Verifies that a call is allowed to the reader.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.inputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_SyncCallOnAsyncReader);
                }
            }
            else
            {
                if (this.inputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_AsyncCallOnSyncReader);
                }
            }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        protected internal class Scope
        {
            /// <summary>The reader state of this scope.</summary>
            private readonly ODataReaderState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>The odataUri parsed based on the context uri attached to this scope.</summary>
            private readonly ODataUri odataUri;

            /// <summary>
            /// The <see cref="ResourceSetWithoutExpectedTypeValidator"/> to use for entries in this resourceSet.
            /// </summary>
            private ResourceSetWithoutExpectedTypeValidator resourceTypeValidator;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            internal Scope(ODataReaderState state, ODataItem item, ODataUri odataUri)
            {
                Debug.Assert(
                    state == ODataReaderState.Exception && item == null ||
                    state == ODataReaderState.ResourceStart && (item == null || item is ODataResource) ||
                    state == ODataReaderState.ResourceEnd && (item is ODataResource || item == null) ||
                    state == ODataReaderState.Primitive && (item == null || item is ODataPrimitiveValue || item is ODataNullValue) ||
                    state == ODataReaderState.Stream && (item == null || item is ODataStreamItem) ||
                    state == ODataReaderState.NestedProperty && (item == null || item is ODataPropertyInfo) ||
                    state == ODataReaderState.ResourceSetStart && item is ODataResourceSet ||
                    state == ODataReaderState.ResourceSetEnd && item is ODataResourceSet ||
                    state == ODataReaderState.NestedResourceInfoStart && item is ODataNestedResourceInfo ||
                    state == ODataReaderState.NestedResourceInfoEnd && item is ODataNestedResourceInfo ||
                    state == ODataReaderState.EntityReferenceLink && item is ODataEntityReferenceLink ||
                    state == ODataReaderState.DeletedResourceStart && (item == null || item is ODataDeletedResource) ||
                    state == ODataReaderState.DeletedResourceEnd && (item is ODataDeletedResource || item == null) ||
                    state == ODataReaderState.DeltaResourceSetStart && item is ODataDeltaResourceSet ||
                    state == ODataReaderState.DeltaResourceSetEnd && item is ODataDeltaResourceSet ||
                    state == ODataReaderState.DeltaLink && (item == null || item is ODataDeltaLink) ||
                    state == ODataReaderState.DeltaDeletedLink && (item == null || item is ODataDeltaDeletedLink) ||
                    state == ODataReaderState.Start && item == null ||
                    state == ODataReaderState.Completed && item == null,
                    "Reader state and associated item do not match.");

                this.state = state;
                this.item = item;
                this.odataUri = odataUri;
            }

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedResourceTypeReference">The expected resource type reference for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedResourceTypeReference"/> has the following meanings for given state:
            /// Start -               it's the expected base type reference of the top-level resource or resources in the top-level resource set.
            /// ResourceSetStart -           it's the expected base type reference of the resources in the resource set.
            ///                       note that it might be a more derived type than the base type of the entity set for the resource set.
            /// EntryStart -          it's the expected base type reference of the resource. If the resource has no type name specified
            ///                       this type will be assumed. Otherwise the specified type name must be
            ///                       the expected type or a more derived type.
            /// NestedResourceInfoStart - it's the expected base type reference the entries in the expanded link (either the single resource
            ///                       or entries in the expanded resource set).
            /// EntityReferenceLink - it's null, no need for types on entity reference links.
            /// In all cases the specified type must be an structured type.</remarks>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            internal Scope(ODataReaderState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmTypeReference expectedResourceTypeReference, ODataUri odataUri)
                : this(state, item, odataUri)
            {
                this.NavigationSource = navigationSource;
                this.ResourceTypeReference = expectedResourceTypeReference;
            }

            /// <summary>
            /// The reader state of this scope.
            /// </summary>
            internal ODataReaderState State
            {
                get
                {
                    return this.state;
                }
            }

            /// <summary>
            /// The item attached to this scope.
            /// </summary>
            internal ODataItem Item
            {
                get
                {
                    return this.item;
                }
            }

            /// <summary>
            /// The odataUri parsed based on the context url to this scope.
            /// </summary>
            internal ODataUri ODataUri
            {
                get
                {
                    return this.odataUri;
                }
            }

            /// <summary>
            /// The navigation source we are reading entries from (possibly null).
            /// </summary>
            internal IEdmNavigationSource NavigationSource { get; set; }

            /// <summary>
            /// The resource type for this scope. Can be either the expected one if the real one
            /// was not found yet, or the one specified in the payload itself (the real one).
            /// </summary>
            internal IEdmType ResourceType
            {
                get
                {
                    if (this.ResourceTypeReference != null)
                    {
                        return ResourceTypeReference.Definition;
                    }

                    return null;
                }
            }

            /// <summary>
            /// The resource type reference for this scope. Can be either the expected one if the real one
            /// was not found yet, or the one specified in the payload itself (the real one).
            /// </summary>
            internal IEdmTypeReference ResourceTypeReference { get; set; }

            /// <summary>
            /// Validator for resource type.
            /// </summary>
            internal ResourceSetWithoutExpectedTypeValidator ResourceTypeValidator
            {
                get
                {
                    return this.resourceTypeValidator;
                }

                set
                {
                    this.resourceTypeValidator = value;
                }
            }

            /// <summary>
            /// Gets or sets the derived type constraint validator.
            /// </summary>
            internal DerivedTypeValidator DerivedTypeValidator { get; set; }
        }

        protected internal class StreamScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedResourceType">The expected resource type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            internal StreamScope(ODataReaderState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmTypeReference expectedResourceType, ODataUri odataUri)
                : base(state, item, navigationSource, expectedResourceType, odataUri)
            {
                this.StreamingState = StreamingState.None;
            }

            /// <summary>
            /// Current state of the stream.
            /// </summary>
            internal StreamingState StreamingState { get; set; }
        }
    }
}
