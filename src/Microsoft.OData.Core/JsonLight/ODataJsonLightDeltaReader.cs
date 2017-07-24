//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaReader.cs" company="Microsoft">
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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData delta reader for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightDeltaReader : ODataDeltaReader
    {
        #region Private Fields

        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>The resource and resource set deserializer to read input with.</summary>
        private readonly ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;

        /// <summary>The scope associated with the top level of this payload.</summary>
        private readonly JsonLightTopLevelScope topLevelScope;

        /// <summary>Stack of reader scopes to keep track of the current context of the reader.</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the resource to be read (in case of resource reader) or entries in the resource set to be read (in case of resource set reader).</param>
        public ODataJsonLightDeltaReader(
            ODataJsonLightInputContext jsonLightInputContext,
            IEdmNavigationSource navigationSource,
            IEdmEntityType expectedEntityType)
        {
            Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");
            Debug.Assert(
                expectedEntityType == null || jsonLightInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.jsonLightInputContext = jsonLightInputContext;
            this.jsonLightResourceDeserializer = new ODataJsonLightResourceDeserializer(jsonLightInputContext);
            this.topLevelScope = new JsonLightTopLevelScope(navigationSource, expectedEntityType);
            this.EnterScope(this.topLevelScope);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the current state of the reader. </summary>
        /// <returns>The current state of the reader.</returns>
        public override ODataDeltaReaderState State
        {
            get
            {
                this.jsonLightInputContext.VerifyNotDisposed();
                return this.CurrentScope.State;
            }
        }

        /// <summary>Gets the current sub state of the reader. </summary>
        /// <returns>The current sub state of the reader.</returns>
        /// <remarks>
        /// The sub state is a complement to the current state if the current state itself is not enough to determine
        /// the real state of the reader. The sub state is only meaningful in NestedResourceInfo state.
        /// </remarks>
        public override ODataReaderState SubState
        {
            get
            {
                this.jsonLightInputContext.VerifyNotDisposed();
                return this.State == ODataDeltaReaderState.NestedResource
                    ? this.CurrentJsonLightNestedResourceInfoScope.SubState
                    : ODataReaderState.Start;
            }
        }

        /// <summary>Gets the most recent <see cref="T:Microsoft.OData.ODataItem" /> that has been read. </summary>
        /// <returns>The most recent <see cref="T:Microsoft.OData.ODataItem" /> that has been read.</returns>
        public override ODataItem Item
        {
            get
            {
                this.jsonLightInputContext.VerifyNotDisposed();
                return this.State == ODataDeltaReaderState.NestedResource
                    ? this.CurrentJsonLightNestedResourceInfoScope.Item
                    : this.CurrentScope.Item;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// A flag indicating whether the reader is at the top level.
        /// </summary>
        private bool IsTopLevel
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
        /// Returns the current scope.
        /// </summary>
        private Scope CurrentScope
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek();
            }
        }

        /// <summary>
        /// Returns the expected entity type for the current scope.
        /// </summary>
        private IEdmEntityType CurrentEntityType
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmEntityType entityType = this.scopes.Peek().EntityType;
                return entityType;
            }

            set
            {
                this.scopes.Peek().EntityType = value;
            }
        }

        /// <summary>
        /// Returns the navigation source for the current scope.
        /// </summary>
        private IEdmNavigationSource CurrentNavigationSource
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmNavigationSource navigationSource = this.scopes.Peek().NavigationSource;
                Debug.Assert(navigationSource == null || this.jsonLightInputContext.Model.IsUserModel(), "We can only have navigation source if we also have metadata.");
                return navigationSource;
            }
        }

        /// <summary>
        /// Returns the current delta resource state.
        /// </summary>
        private IODataJsonLightReaderResourceState CurrentDeltaResourceState
        {
            get
            {
                Debug.Assert(
                    this.State == ODataDeltaReaderState.DeltaResourceStart ||
                    this.State == ODataDeltaReaderState.DeltaResourceEnd ||
                    this.State == ODataDeltaReaderState.DeltaDeletedEntry,
                    "This property can only be accessed in the DeltaResource or DeltaDeletedEntry scope.");
                return (IODataJsonLightReaderResourceState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightNestedResourceInfoScope
        /// </summary>
        private JsonLightNestedResourceInfoScope CurrentJsonLightNestedResourceInfoScope
        {
            get
            {
                Debug.Assert(this.State == ODataDeltaReaderState.NestedResource,
                    "This property can only be accessed in NestedResourceInfo state.");
                return (JsonLightNestedResourceInfoScope)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightDeltaResourceSetScope
        /// </summary>
        private JsonLightDeltaResourceSetScope CurrentJsonLightDeltaResourceSetScope
        {
            get
            {
                Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart,
                    "This property can only be accessed in DeltaResourceSetStart state.");
                return (JsonLightDeltaResourceSetScope)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns the current item as ODataResource.
        /// </summary>
        private ODataResource CurrentDeltaResource
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataResource, "this.Item is ODataResource");
                return (ODataResource)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as ODataDeltaDeletedEntry.
        /// </summary>
        private ODataDeltaDeletedEntry CurrentDeltaDeletedEntry
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataDeltaDeletedEntry, "this.Item is ODataDeltaDeletedEntry");
                return (ODataDeltaDeletedEntry)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as ODataDeltaLink
        /// </summary>
        private ODataDeltaLink CurrentDeltaLink
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
        private ODataDeltaDeletedLink CurrentDeltaDeletedLink
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataDeltaDeletedLink, "this.Item is ODataDeltaDeletedLink");
                return (ODataDeltaDeletedLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataDeltaResourceSet"/>. Must only be called if the item actually is a resource set.
        /// </summary>
        private ODataDeltaResourceSet CurrentDeltaResourceSet
        {
            get
            {
                Debug.Assert(this.Item is ODataDeltaResourceSet, "this.Item is ODataDeltaResourceSet");
                return (ODataDeltaResourceSet)this.Item;
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Reads the next <see cref="T:Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException(this.ReadSynchronously);
        }

#if PORTABLELIB
        /// <summary> Asynchronously reads the next <see cref="T:Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.EnterScope(CreateExceptionScope()));
        }
#endif

        #endregion

        #region Private Methods

        #region Scope Methods

        /// <summary>
        /// Pushes the <paramref name="scope"/> on the stack of scopes.
        /// </summary>
        /// <param name="scope">The scope to enter.</param>
        private void EnterScope(Scope scope)
        {
            Debug.Assert(scope != null, "scope != null");

            this.scopes.Push(scope);
        }

        /// <summary>
        /// Replaces the current scope with the specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope">The scope to replace the current scope with.</param>
        private void ReplaceScope(Scope scope)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");
            Debug.Assert(scope != null, "scope != null");

            // TODO: implement some basic validation that the transitions are ok
            this.scopes.Pop();
            this.EnterScope(scope);
        }

        /// <summary>
        /// Replaces the current scope with a new scope with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataDeltaReaderState"/> to use for the new scope.</param>
        private void ReplaceScope(ODataDeltaReaderState state)
        {
            this.ReplaceScope(new Scope(state, this.Item, this.CurrentNavigationSource, this.CurrentEntityType, this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Removes the current scope from the stack of all scopes.
        /// </summary>
        /// <param name="state">The expected state of the current scope (to be popped).</param>
        private void PopScope(ODataDeltaReaderState state)
        {
            Debug.Assert(this.scopes.Count > 1, "Stack must have more than 1 items in order to pop an item.");

#if DEBUG
            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.State == state, "scope.State == state");
#else
            this.scopes.Pop();
#endif
        }

#endregion

#region Verify Methods

        /// <summary>
        /// Verifies that calling Read is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanRead(bool synchronousCall)
        {
            this.jsonLightInputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State == ODataDeltaReaderState.Exception || this.State == ODataDeltaReaderState.Completed)
            {
                throw new ODataException(Strings.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(this.State));
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
                if (!this.jsonLightInputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_SyncCallOnAsyncReader);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.jsonLightInputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_AsyncCallOnSyncReader);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

#endregion

#region Exception Methods

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
                    this.EnterScope(CreateExceptionScope());
                }

                throw;
            }
        }

#endregion

#region Read<...>Value Methods

        /// <summary>
        /// Read value as Uri in Json reader.
        /// </summary>
        /// <returns>The Uri readed.</returns>
        private Uri ReadUriValue()
        {
            return new Uri(this.ReadStringValue(), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Read value as string in Json reader.
        /// </summary>
        /// <returns>The string readed.</returns>
        private string ReadStringValue()
        {
            return this.jsonLightResourceDeserializer.JsonReader.ReadStringValue();
        }

#endregion

#region Read Sync/Async Methods

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        private bool ReadSynchronously()
        {
            return this.ReadImplementation();
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
                case ODataDeltaReaderState.Start:
                    result = this.ReadAtStartImplementation();
                    break;

                case ODataDeltaReaderState.DeltaResourceSetStart:
                    result = this.ReadAtDeltaResourceSetStartImplementation();
                    break;

                case ODataDeltaReaderState.DeltaResourceStart:
                    result = this.ReadAtDeltaResourceStartImplementation();
                    break;

                case ODataDeltaReaderState.DeltaResourceEnd:
                    result = this.ReadAtDeltaResourceEndImplementation();
                    break;

                case ODataDeltaReaderState.DeltaDeletedEntry:
                case ODataDeltaReaderState.DeltaLink:
                case ODataDeltaReaderState.DeltaDeletedLink:
                    this.scopes.Pop();
                    Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart, "We should get back to DeltaResourceSetStart now.");
                    result = this.ReadAtDeltaResourceSetStartImplementation();
                    break;

                case ODataDeltaReaderState.DeltaResourceSetEnd:
                    result = this.ReadAtDeltaResourceSetEndImplementation();
                    break;

                case ODataDeltaReaderState.NestedResource:
                    result = this.ReadAtNestedResourceInfoImplementation();
                    break;

                case ODataDeltaReaderState.Exception:    // fall through
                case ODataDeltaReaderState.Completed:
                    throw new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCore_ReadImplementation));
            }

            return result;
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        private Task<bool> ReadAsynchronously()
        {
            Task<bool> result;
            switch (this.State)
            {
                case ODataDeltaReaderState.Start:
                    result = this.ReadAtStartImplementationAsync();
                    break;

                case ODataDeltaReaderState.DeltaResourceStart:
                    result = this.ReadAtDeltaResourceStartImplementationAsync();
                    break;

                case ODataDeltaReaderState.DeltaResourceEnd:
                    result = this.ReadAtDeltaResourceEndImplementationAsync();
                    break;

                case ODataDeltaReaderState.DeltaResourceSetStart:
                    result = this.ReadAtDeltaResourceSetStartImplementationAsync();
                    break;

                case ODataDeltaReaderState.DeltaDeletedEntry:
                case ODataDeltaReaderState.DeltaLink:
                case ODataDeltaReaderState.DeltaDeletedLink:
                    this.scopes.Pop();
                    Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart, "We should get back to DeltaResourceSetStart now.");
                    result = this.ReadAtDeltaResourceSetStartImplementationAsync();
                    break;

                case ODataDeltaReaderState.DeltaResourceSetEnd:
                    result = this.ReadAtDeltaResourceSetEndImplementationAsync();
                    break;

                case ODataDeltaReaderState.NestedResource:
                    result = this.ReadAtNestedResourceInfoImplementationAsync();
                    break;

                case ODataDeltaReaderState.Exception:    // fall through
                case ODataDeltaReaderState.Completed:
                    result = TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State)));
                    break;

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    result = TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCoreAsync_ReadAsynchronously)));
                    break;
            }

            return result;
        }
#endif

#endregion

#region ReadAt<...>Implementation Methods

        /// <summary>
        /// Handle some thing before reading at Start.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The created duplicate property names checker.</param>
        private void PreReadAtStartImplementation(out PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(this.State == ODataDeltaReaderState.Start, "this.State == ODataDeltaReaderState.Start");
            Debug.Assert(this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None.");

            propertyAndAnnotationCollector =
                this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtStartImplementation()
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector;
            this.PreReadAtStartImplementation(out propertyAndAnnotationCollector);

            this.jsonLightResourceDeserializer.ReadPayloadStart(
                ODataPayloadKind.Delta,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            return this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtDeltaResourceSetStartImplementation()
        {
            return this.ReadAtDeltaResourceSetStartImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtDeltaResourceSetEndImplementation()
        {
            return this.ReadAtDeltaResourceSetEndImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtDeltaResourceStartImplementation()
        {
            return this.ReadAtDeltaResourceStartImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtDeltaResourceEndImplementation()
        {
            return this.ReadAtDeltaResourceEndImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfo'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtNestedResourceInfoImplementation()
        {
            return this.ReadAtNestedResourceInfoImplementationSynchronously();
        }

#endregion

#region ReadAt<...>ImplementationAsync Methods

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        private Task<bool> ReadAtStartImplementationAsync()
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector;
            this.PreReadAtStartImplementation(out propertyAndAnnotationCollector);

            return this.jsonLightResourceDeserializer.ReadPayloadStartAsync(
                ODataPayloadKind.ResourceSet,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)
                .FollowOnSuccessWith(t => this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector));
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        private Task<bool> ReadAtDeltaResourceSetStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceSetStartImplementationSynchronously);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        private Task<bool> ReadAtDeltaResourceSetEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceSetEndImplementationSynchronously);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        private Task<bool> ReadAtDeltaResourceStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceStartImplementationSynchronously);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        private Task<bool> ReadAtDeltaResourceEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceEndImplementationSynchronously);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfo'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false</returns>
        private Task<bool> ReadAtNestedResourceInfoImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtNestedResourceInfoImplementationSynchronously);
        }
#endif

#endregion

#region ReadAt<...>ImplementationSynchronously Methods

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:      the reader is positioned on the context uri annotation.
        /// Post-Condition: JsonNodeType.EndArray:      the reader is positioned on the end array node of an empty resource set.
        ///                 JsonNodeType.StartObject:   the reader is positioned on the first item.
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(this.State == ODataDeltaReaderState.Start, "this.State == ODataDeltaReaderState.Start");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(this.jsonLightResourceDeserializer.ContextUriParseResult != null, "We should have failed by now if we don't have parse results for context URI.");
            Debug.Assert(this.jsonLightResourceDeserializer.ContextUriParseResult.DeltaKind == ODataDeltaKind.ResourceSet, "The context uri should indicate a delta resource set at Start state.");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property);

            // Get the $select query option from the metadata link, if we have one.
            string selectQueryOption = this.jsonLightResourceDeserializer.ContextUriParseResult == null
                ? null
                : this.jsonLightResourceDeserializer.ContextUriParseResult.SelectQueryOption;

            SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(selectQueryOption);

            // Store the duplicate property names checker to use it later when reading the resource set end
            // (since we allow resourceSet-related annotations to appear after the resource set's data).
            this.topLevelScope.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;

            bool isReordering = this.jsonLightInputContext.JsonReader is ReorderingJsonReader;
            ODataDeltaResourceSet resourceSet = new ODataDeltaResourceSet();

            // Read top-level resource set annotations.
            this.jsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations(resourceSet, propertyAndAnnotationCollector, /*forResourceSetStart*/true, /*readAllResourceSetProperties*/isReordering);

            // Enter DeltaResourceSetStart state.
            this.ReadDeltaResourceSetStart(resourceSet, selectedProperties);

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.StartObject);

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        private bool ReadAtDeltaResourceSetStartImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart, "this.State == ODataDeltaReaderState.DeltaResourceSetStart");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            // figure out whether the resource set contains entries or not
            switch (this.jsonLightResourceDeserializer.JsonReader.NodeType)
            {
                // we are at the beginning of an item
                case JsonNodeType.StartObject:
                    // First delta item in the resource set
                    this.ReadDeltaStart(/*propertyAndAnnotationCollector*/ null, this.CurrentJsonLightDeltaResourceSetScope.SelectedProperties);
                    break;
                case JsonNodeType.EndArray:
                    // End of the resource set
                    this.ReadResourceSetEnd();
                    break;
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(this.jsonLightResourceDeserializer.JsonReader.NodeType));
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtDeltaResourceSetEndImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetEnd, "this.State == ODataDeltaReaderState.ResourceSetEnd");
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                !this.IsTopLevel && !this.jsonLightInputContext.ReadingResponse,
                "Pre-Condition: expected JsonNodeType.EndObject or JsonNodeType.Property");

            Debug.Assert(this.IsTopLevel, "Reading resource set must be on top level.");

            this.PopScope(ODataDeltaReaderState.DeltaResourceSetEnd);

            Debug.Assert(this.State == ODataDeltaReaderState.Start, "this.State == ODataReaderState.Start");

            // Read the end-object node of the resource set object and position the reader on the next input node
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();

            // read the end-of-payload
            this.jsonLightResourceDeserializer.ReadPayloadEnd(/*isReadingNestedPayload*/false);
            Debug.Assert(this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input when not reading a nested payload.");

            // replace the 'Start' scope with the 'Completed' scope
            this.ReplaceScope(ODataDeltaReaderState.Completed);
            return false;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the resource.
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node.
        /// </remarks>
        private bool ReadAtDeltaResourceStartImplementationSynchronously()
        {
            Debug.Assert(this.CurrentDeltaResource != null, "this.CurrentDeltaResource != null");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read the odata.type annotation.
            this.jsonLightResourceDeserializer.ReadResourceTypeName(this.CurrentDeltaResourceState);

            // Read the odata.id annotation.
            this.ReadDeltaResourceId();

            // Resolve the type name.
            Debug.Assert(
                this.CurrentNavigationSource != null,
                "We must always have an expected navigation source for each resource (since we can't deduce that from the type name).");
            this.ApplyEntityTypeNameFromPayload(this.CurrentDeltaResource.TypeName);

            // Read other annotations and properties for this resource.
            while (true)
            {
                ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo =
                    this.jsonLightResourceDeserializer.ReadResourceContent(this.CurrentDeltaResourceState);
                if (readerNestedResourceInfo == null)
                {
                    // There is no content left in this resource.
                    break;
                }

                if (!readerNestedResourceInfo.HasValue)
                {
                    // No need to enter NestedResourceInfo state
                    // if there is no actual nested resource set or resource to read.
                    continue;
                }

                this.EnterScope(new JsonLightNestedResourceInfoScope(
                    readerNestedResourceInfo,
                    this.CurrentNavigationSource,
                    readerNestedResourceInfo.NestedResourceType,
                    this.CurrentScope.ODataUri,
                    this.jsonLightInputContext));

                return true;
            }

            // Transit to DeltaResourceEnd state.
            this.EndDeltaResource(
                new JsonLightDeltaResourceScope(
                    ODataDeltaReaderState.DeltaResourceEnd,
                    this.Item,
                    this.CurrentNavigationSource,
                    this.CurrentEntityType,
                    this.CurrentDeltaResourceState.PropertyAndAnnotationCollector,
                    this.CurrentDeltaResourceState.SelectedProperties,
                    this.CurrentScope.ODataUri));

            return true;
        }

        /// <summary>
        /// Called to transition into the DeltaResourceEnd state.
        /// </summary>
        /// <param name="scope">The scope for the DeltaResourceEnd state.</param>
        private void EndDeltaResource(Scope scope)
        {
            this.PopScope(ODataDeltaReaderState.DeltaResourceStart);
            this.EnterScope(scope);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              No more other annotation or property in the resource.
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node or the end of array.
        /// </remarks>
        private bool ReadAtDeltaResourceEndImplementationSynchronously()
        {
            Debug.Assert(this.CurrentDeltaResource != null, "this.CurrentDeltaResource != null");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();

            // Return to DeltaResourceSetStart.
            this.PopScope(ODataDeltaReaderState.DeltaResourceEnd);
            Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart, "We should get back to DeltaResourceSetStart now.");

            return this.ReadAtDeltaResourceSetStartImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfo'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        private bool ReadAtNestedResourceInfoImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataDeltaReaderState.NestedResource,
                "this.State == ODataDeltaReaderState.NestedResourceInfo");

            if (this.SubState == ODataReaderState.Completed)
            {
                // Leave NestedResourceInfo state if the inner reader finished reading.
                this.PopScope(ODataDeltaReaderState.NestedResource);

                // We always have delta payload left to read.
                return true;
            }

            if (this.SubState == ODataReaderState.Exception)
            {
                // Go into Exception state if the inner reader ran into an exception.
                this.EnterScope(CreateExceptionScope());

                // Should not call Read() again if exception happened.
                return false;
            }

            // We place the call to Read() AFTER the two conditions above because we want to
            // enable the user to catch the Completed state and do something he wants.
            this.CurrentJsonLightNestedResourceInfoScope.NestedResourceInfoReader.Read();

            // We always have expanded payload or delta payload left to read.
            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedEntry'.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the resource.
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node.
        /// </remarks>
        private void ReadAtDeltaDeletedEntryImplementationSynchronously()
        {
            Debug.Assert(this.CurrentDeltaDeletedEntry != null, "this.CurrentDeltaDeletedEntry != null");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read id property.
            this.ReadDeltaDeletedEntryId();

            // Read reason property.
            this.ReadDeltaDeletedEntryReason();

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLink'.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private void ReadAtDeltaLinkImplementationSynchronously()
        {
            Debug.Assert(this.CurrentDeltaLink != null, "this.CurrentDeltaLink != null");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read source property.
            this.ReadDeltaLinkSource();

            // Read relationship property.
            this.ReadDeltaLinkRelationship();

            // Read target property.
            this.ReadDeltaLinkTarget();

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLink'.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private void ReadAtDeltaDeletedLinkImplementationSynchronously()
        {
            Debug.Assert(this.CurrentDeltaDeletedLink != null, "this.CurrentDeltaDeletedLink != null");

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read source property.
            this.ReadDeltaDeletedLinkSource();

            // Read relationship property.
            this.ReadDeltaDeletedLinkRelationship();

            // Read target property.
            this.ReadDeltaDeletedLinkTarget();

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();
        }

#endregion

#region ReadDeltaResourceSet<...> Methods

        /// <summary>
        /// Reads the start of the JSON array for the content of the resource set and sets up the reader state correctly.
        /// </summary>
        /// <param name="resourceSet">The resource set to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the resource set property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the resource set, or on the end array of the resource set.
        /// </remarks>
        private void ReadDeltaResourceSetStart(ODataDeltaResourceSet resourceSet, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            this.jsonLightResourceDeserializer.ReadResourceSetContentStart();
            IJsonReader jsonReader = this.jsonLightResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            this.EnterScope(new JsonLightDeltaResourceSetScope(resourceSet, this.CurrentNavigationSource, this.CurrentEntityType, selectedProperties, this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Reads the end of the current resource set.
        /// </summary>
        private void ReadResourceSetEnd()
        {
            Debug.Assert(this.State == ODataDeltaReaderState.DeltaResourceSetStart, "this.State == ODataDeltaReaderState.DeltaResourceSetStart");

            this.jsonLightResourceDeserializer.ReadResourceSetContentEnd();
            this.jsonLightResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEnd(this.CurrentDeltaResourceSet, /*expandedNestedResourceInfo*/null, this.topLevelScope.PropertyAndAnnotationCollector);

            this.ReplaceScope(ODataDeltaReaderState.DeltaResourceSetEnd);
        }

#endregion

#region ReadDeltaResource<...> Methods

        /// <summary>
        /// Reads the start of a delta item and sets up the reader state correctly
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            If the resource is in a resource set - the start of the resource object
        ///                 JsonNodeType.Property               If the resource is a top-level resource and has at least one property
        ///                 JsonNodeType.EndObject              If the resource is a top-level resource and has no properties
        /// Post-Condition: JsonNodeType.StartObject            Start of expanded resource of the nested resource info to read next
        ///                 JsonNodeType.Property               Property after deferred link or expanded entity reference
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        private void ReadDeltaStart(PropertyAndAnnotationCollector propertyAndAnnotationCollector, SelectedPropertiesNode selectedProperties)
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.Property, JsonNodeType.EndObject);

            // If the reader is on StartObject then read over it. This happens for entries in resource set.
            // For top-level entries the reader will be positioned on the first resource property (after odata.context if it was present).
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.jsonLightResourceDeserializer.JsonReader.Read();
            }

            ODataDeltaKind deltaKind = ODataDeltaKind.Resource;
            IEdmEntityType entityTypeFromContextUri = null;

            // Parse context uri.
            string contextUri = this.jsonLightResourceDeserializer.ReadContextUriAnnotation(ODataPayloadKind.Delta, propertyAndAnnotationCollector, false);
            if (!string.IsNullOrEmpty(contextUri))
            {
                ODataJsonLightContextUriParseResult contextUriParseResult = ODataJsonLightContextUriParser.Parse(
                        this.jsonLightInputContext.Model,
                        contextUri,
                        ODataPayloadKind.Delta,
                    /*clientCustomTypeResolver*/null,
                    /*needParseFragment*/true);
                deltaKind = contextUriParseResult.DeltaKind;
                entityTypeFromContextUri = contextUriParseResult.EdmType as IEdmEntityType;
            }

            // Enter different scope according to delta kind.
            switch (deltaKind)
            {
                case ODataDeltaKind.Resource:
                    this.StartDeltaResource(ODataDeltaReaderState.DeltaResourceStart, propertyAndAnnotationCollector, selectedProperties, entityTypeFromContextUri);
                    break;

                case ODataDeltaKind.DeletedEntry:
                    this.StartDeltaResource(ODataDeltaReaderState.DeltaDeletedEntry, propertyAndAnnotationCollector, selectedProperties);
                    this.ReadAtDeltaDeletedEntryImplementationSynchronously();
                    break;

                case ODataDeltaKind.Link:
                    this.StartDeltaLink(ODataDeltaReaderState.DeltaLink, propertyAndAnnotationCollector, selectedProperties);
                    this.ReadAtDeltaLinkImplementationSynchronously();
                    break;

                case ODataDeltaKind.DeletedLink:
                    this.StartDeltaLink(ODataDeltaReaderState.DeltaDeletedLink, propertyAndAnnotationCollector, selectedProperties);
                    this.ReadAtDeltaDeletedLinkImplementationSynchronously();
                    break;

                default:
                    // TODO: Add throw ODataException.
                    Debug.Assert(false, "Invalid delta kind.");
                    break;
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.EndArray);
        }

        /// <summary>
        /// Reads the delta resource id annotation (odata.id)
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method fills the ODataResource.Id property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaResourceId()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.id property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataId, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaResource.Id == null, "id should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the annotation value.
                CurrentDeltaResource.Id = this.jsonLightResourceDeserializer.ReadEntryInstanceAnnotation(ODataAnnotationNames.ODataId, /*anyPropertyFound*/false, /*typeAnnotationFound*/false, CurrentDeltaResourceState.PropertyAndAnnotationCollector) as Uri;
                Debug.Assert(CurrentDeltaResource.Id != null, "value for odata.id must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

#endregion

#region ReadDeltaDeletedEntry<...> Methods

        /// <summary>
        /// Reads the delta deleted resource id.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method fills the ODataDeltaDeletedEntry.Id property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaDeletedEntryId()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the id property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataIdPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaDeletedEntry.Id == null, "id should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the id value.
                CurrentDeltaDeletedEntry.Id = this.ReadStringValue();
                Debug.Assert(CurrentDeltaDeletedEntry.Id != null, "value for id must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta deleted resource reason.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method fills the ODataDeltaDeletedEntry.Reason property if the reason is found in the payload.
        /// </remarks>
        private void ReadDeltaDeletedEntryReason()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the id property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataReasonPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the id value.
                string reason = this.ReadStringValue();
                if (string.CompareOrdinal(reason, JsonLightConstants.ODataReasonChangedValue) == 0)
                {
                    CurrentDeltaDeletedEntry.Reason = DeltaDeletedEntryReason.Changed;
                }
                else if (string.CompareOrdinal(reason, JsonLightConstants.ODataReasonDeletedValue) == 0)
                {
                    CurrentDeltaDeletedEntry.Reason = DeltaDeletedEntryReason.Deleted;
                }
                else
                {
                    // TODO: throw ODataException.
                    Debug.Assert(false, "Unknown reason.");
                    CurrentDeltaDeletedEntry.Reason = null;
                }
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

#endregion

#region ReadDeltaLink<...> Methods

        /// <summary>
        /// Reads the delta link source.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaLink.Source property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaLinkSource()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the source property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataSourcePropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaLink.Source == null, "source should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the source value.
                CurrentDeltaLink.Source = this.ReadUriValue();
                Debug.Assert(CurrentDeltaLink.Source != null, "value for source must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta link relationship.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaLink.Relationship property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaLinkRelationship()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the relationship property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataRelationshipPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaLink.Relationship == null, "relationship should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the relationship value.
                CurrentDeltaLink.Relationship = this.ReadStringValue();
                Debug.Assert(CurrentDeltaLink.Relationship != null, "value for relationship must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta link target.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaLink.Target property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaLinkTarget()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the target property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataTargetPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaLink.Target == null, "target should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the source value.
                CurrentDeltaLink.Target = this.ReadUriValue();
                Debug.Assert(CurrentDeltaLink.Target != null, "value for target must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

#endregion

#region ReadDeltaDeletedLink<...> Methods

        /// <summary>
        /// Reads the delta deleted link source.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaDeletedLink.Source property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaDeletedLinkSource()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the source property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataSourcePropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaDeletedLink.Source == null, "source should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the source value.
                CurrentDeltaDeletedLink.Source = this.ReadUriValue();
                Debug.Assert(CurrentDeltaDeletedLink.Source != null, "value for source must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta deleted link relationship.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaLink.Relationship property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaDeletedLinkRelationship()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the relationship property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataRelationshipPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaDeletedLink.Relationship == null, "relationship should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the relationship value.
                CurrentDeltaDeletedLink.Relationship = this.ReadStringValue();
                Debug.Assert(CurrentDeltaDeletedLink.Relationship != null, "value for relationship must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta deleted link target.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDeltaLink.Target property if the id is found in the payload.
        /// </remarks>
        private void ReadDeltaDeletedLinkTarget()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the target property - read it.
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataTargetPropertyName, this.jsonLightResourceDeserializer.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(CurrentDeltaDeletedLink.Target == null, "target should not have already been set");

                // Read over the property to move to its value.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // Read the source value.
                CurrentDeltaDeletedLink.Target = this.ReadUriValue();
                Debug.Assert(CurrentDeltaDeletedLink.Target != null, "value for target must be provided");
            }

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

#endregion

#region Start<...> Methods

        /// <summary>
        /// Starts the resource, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="state">The reader state to switch to.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <param name="entityTypeFromContextUri">The entity type read from context uri.</param>
        private void StartDeltaResource(ODataDeltaReaderState state, PropertyAndAnnotationCollector propertyAndAnnotationCollector, SelectedPropertiesNode selectedProperties, IEdmEntityType entityTypeFromContextUri = null)
        {
            Debug.Assert(
                state == ODataDeltaReaderState.DeltaResourceStart || state == ODataDeltaReaderState.DeltaDeletedEntry,
                "state must be either DeltaResource or DeltaDeletedEntry or DeltaLink or DeltaDeletedLink.");

            this.EnterScope(new JsonLightDeltaResourceScope(
                state,
                CreateNewDeltaResource(state),
                this.CurrentNavigationSource,
                entityTypeFromContextUri ?? this.CurrentEntityType,
                propertyAndAnnotationCollector ?? this.jsonLightInputContext.CreatePropertyAndAnnotationCollector(),
                selectedProperties,
                this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Starts the link, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="state">The reader state to switch to.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        private void StartDeltaLink(ODataDeltaReaderState state, PropertyAndAnnotationCollector propertyAndAnnotationCollector, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(
                state == ODataDeltaReaderState.DeltaLink || state == ODataDeltaReaderState.DeltaDeletedLink,
                "state must be either DeltaResource or DeltaDeletedEntry or DeltaLink or DeltaDeletedLink.");

            this.EnterScope(new JsonLightDeltaLinkScope(
                state,
                CreateNewDeltaLink(state),
                this.CurrentNavigationSource,
                this.CurrentEntityType,
                this.CurrentScope.ODataUri));
        }

#endregion

#region Others

        /// <summary>
        /// If an entity type name is found in the payload this method is called to apply it to the current scope.
        /// This method should be called even if the type name was not found in which case a null should be passed in.
        /// The method validates that some type will be available as the current entity type after it returns (if we are parsing using metadata).
        /// </summary>
        /// <param name="entityTypeNameFromPayload">The entity type name found in the payload or null if no type was specified in the payload.</param>
        private void ApplyEntityTypeNameFromPayload(string entityTypeNameFromPayload)
        {
            Debug.Assert(
                this.scopes.Count > 0 && this.scopes.Peek().Item is ODataResource,
                "Entity type can be applied only when in delta resource scope.");

            ODataTypeAnnotation typeAnnotation;
            EdmTypeKind targetTypeKind;
            IEdmEntityTypeReference targetEntityTypeReference =
                (IEdmEntityTypeReference)this.jsonLightInputContext.MessageReaderSettings.Validator.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.Entity,
                    /*expectStructuredType*/ true,
                    /*defaultPrimitivePayloadType*/ null,
                    this.CurrentEntityType.ToTypeReference(),
                    entityTypeNameFromPayload,
                    this.jsonLightInputContext.Model,
                    () => EdmTypeKind.Entity,
                    out targetTypeKind,
                    out typeAnnotation);

            IEdmEntityType targetEntityType = null;
            ODataResource resource = this.CurrentDeltaResource;
            if (targetEntityTypeReference != null)
            {
                targetEntityType = targetEntityTypeReference.EntityDefinition();
                resource.TypeName = targetEntityType.FullTypeName();

                if (typeAnnotation != null)
                {
                    resource.TypeAnnotation = typeAnnotation;
                }
            }
            else if (entityTypeNameFromPayload != null)
            {
                resource.TypeName = entityTypeNameFromPayload;
            }

            // Set the current entity type since the type from payload might be more derived than
            // the expected one.
            this.CurrentEntityType = targetEntityType;
        }

#endregion

#endregion

#region Private Static Methods

        /// <summary>
        /// Creates a new delta (deleted) resource to return to the user.
        /// </summary>
        /// <param name="state">The reader state.</param>
        /// <returns>The newly created delta (deleted) resource.</returns>
        /// <remarks>The method populates the Properties property with an empty read only enumeration.</remarks>
        private static ODataItem CreateNewDeltaResource(ODataDeltaReaderState state)
        {
            if (state == ODataDeltaReaderState.DeltaResourceStart)
            {
                return new ODataResource { Properties = new ReadOnlyEnumerable<ODataProperty>() };
            }

            if (state == ODataDeltaReaderState.DeltaDeletedEntry)
            {
                return new ODataDeltaDeletedEntry(null, DeltaDeletedEntryReason.Deleted);
            }

            // TODO: throw ODataException.
            Debug.Assert(false, "state must be either DeltaResourceStart or DeltaDeletedEntry.");
            return null;
        }

        /// <summary>
        /// Creates a new delta (deleted) link to return to the user.
        /// </summary>
        /// <param name="state">The reader state.</param>
        /// <returns>The newly created delta (deleted) link.</returns>
        private static ODataDeltaLinkBase CreateNewDeltaLink(ODataDeltaReaderState state)
        {
            if (state == ODataDeltaReaderState.DeltaLink)
            {
                return new ODataDeltaLink(null, null, null);
            }

            if (state == ODataDeltaReaderState.DeltaDeletedLink)
            {
                return new ODataDeltaDeletedLink(null, null, null);
            }

            // TODO: throw ODataException.
            Debug.Assert(false, "state must be either DeltaLink or DeltaDeletedLink.");
            return null;
        }

        /// <summary>
        /// Creates a new scope with Exception state.
        /// </summary>
        /// <returns>The newly created scope.</returns>
        private static Scope CreateExceptionScope()
        {
            return new Scope(ODataDeltaReaderState.Exception, null, null, null, null);
        }

#endregion

#region Scope Classes

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private class Scope
        {
            /// <summary>The reader state of this scope.</summary>
            private readonly ODataDeltaReaderState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>The odataUri parsed based on the context uri attached to this scope.</summary>
            private readonly ODataUri odataUri;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected entity type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            public Scope(ODataDeltaReaderState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
            {
                Debug.Assert(
                    state == ODataDeltaReaderState.Exception && item == null ||
                    state == ODataDeltaReaderState.DeltaDeletedEntry && (item == null || item is ODataDeltaDeletedEntry) ||
                    state == ODataDeltaReaderState.DeltaDeletedLink && (item == null || item is ODataDeltaDeletedLink) ||
                    (state == ODataDeltaReaderState.DeltaResourceStart || state == ODataDeltaReaderState.DeltaResourceEnd) && (item == null || item is ODataResource) ||
                    (state == ODataDeltaReaderState.DeltaResourceSetStart || state == ODataDeltaReaderState.DeltaResourceSetEnd) && item is ODataDeltaResourceSet ||
                    state == ODataDeltaReaderState.DeltaLink && (item == null || item is ODataDeltaLink) ||
                    state == ODataDeltaReaderState.NestedResource && item is ODataNestedResourceInfo ||
                    state == ODataDeltaReaderState.Start && item == null ||
                    state == ODataDeltaReaderState.Completed && item == null,
                    "Reader state and associated item do not match.");

                this.state = state;
                this.item = item;
                this.EntityType = expectedEntityType;
                this.NavigationSource = navigationSource;
                this.odataUri = odataUri;
            }

            /// <summary>
            /// The reader state of this scope.
            /// </summary>
            public ODataDeltaReaderState State
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
            /// The odataUri parsed based on the context url to this scope.
            /// </summary>
            public ODataUri ODataUri
            {
                get
                {
                    return this.odataUri;
                }
            }

            /// <summary>
            /// The navigation source we are reading entries from (possibly null).
            /// </summary>
            public IEdmNavigationSource NavigationSource { get; private set; }

            /// <summary>
            /// The entity type for this scope. Can be either the expected one if the real one
            /// was not found yet, or the one specified in the payload itself (the real one).
            /// </summary>
            public IEdmEntityType EntityType { get; set; }
        }

        /// <summary>
        /// A reader resource set scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightDeltaResourceSetScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="resourceSet">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the entries in the resource set.
            ///   note that it might be a more derived type than the base type of the entity set for the resource set.
            /// In all cases the specified type must be an entity type.</remarks>
            public JsonLightDeltaResourceSetScope(ODataDeltaResourceSet resourceSet, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(ODataDeltaReaderState.DeltaResourceSetStart, resourceSet, navigationSource, expectedEntityType, odataUri)
            {
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightDeltaLinkScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of the new scope that is being created.</param>
            /// <param name="link">The link info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type the entries in the expanded link (either the single resource
            ///   or entries in the expanded resource set).
            /// In all cases the specified type must be an entity type.</remarks>
            public JsonLightDeltaLinkScope(ODataDeltaReaderState state, ODataDeltaLinkBase link, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
                : base(state, link, navigationSource, expectedEntityType, odataUri)
            {
                Debug.Assert(
                    state == ODataDeltaReaderState.DeltaLink && link is ODataDeltaLink ||
                    state == ODataDeltaReaderState.DeltaDeletedLink && link is ODataDeltaDeletedLink,
                    "link must be either DeltaLink or DeltaDeletedLink.");
            }
        }

        /// <summary>
        /// A reader resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightDeltaResourceScope : Scope, IODataJsonLightReaderResourceState
        {
            /// <summary>The set of names of the navigation properties we have read so far while reading the resource.</summary>
            private List<string> navigationPropertiesRead;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            public JsonLightDeltaResourceScope(
                ODataDeltaReaderState readerState,
                ODataItem resource,
                IEdmNavigationSource navigationSource,
                IEdmEntityType expectedEntityType,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, resource, navigationSource, expectedEntityType, odataUri)
            {
                Debug.Assert(
                    (readerState == ODataDeltaReaderState.DeltaResourceStart || readerState == ODataDeltaReaderState.DeltaResourceEnd) && resource is ODataResource ||
                    readerState == ODataDeltaReaderState.DeltaDeletedEntry && resource is ODataDeltaDeletedEntry,
                    "resource must be either DeltaResource or DeltaDeletedEntry.");

                this.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The metadata builder instance for the resource.
            /// </summary>
            public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

            /// <summary>
            /// Flag which indicates that during parsing of the resource represented by this state,
            /// any property which is not an instance annotation was found. This includes property annotations
            /// for property which is not present in the payload.
            /// </summary>
            /// <remarks>
            /// This is used to detect incorrect ordering of the payload (for example odata.id must not come after the first property).
            /// </remarks>
            public bool AnyPropertyFound { get; set; }

            /// <summary>
            /// If the reader finds a nested resource info to report, but it must first report the parent resource
            /// it will store the nested resource info in this property. So this will only ever store the first nested resource info of a resource.
            /// </summary>
            public ODataJsonLightReaderNestedResourceInfo FirstNestedResourceInfo { get; set; }

            /// <summary>
            /// The duplicate property names checker for the resource represented by the current state.
            /// </summary>
            public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; private set; }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }

            /// <summary>
            /// The set of names of the navigation properties we have read so far while reading the resource.
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public List<string> NavigationPropertiesRead
            {
                get { return this.navigationPropertiesRead ?? (this.navigationPropertiesRead = new List<string>()); }
            }

            /// <summary>
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public bool ProcessingMissingProjectedNestedResourceInfos { get; set; }

            /// <summary>
            /// The resource being read.
            /// </summary>
            ODataResource IODataJsonLightReaderResourceState.Resource
            {
                get { return this.Item as ODataResource; }
            }

            /// <summary>
            /// The structured type for the resource (if available).
            /// </summary>
            IEdmStructuredType IODataJsonLightReaderResourceState.ResourceType
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataDeltaReaderState.DeltaResourceStart ||
                        this.State == ODataDeltaReaderState.DeltaResourceEnd ||
                        this.State == ODataDeltaReaderState.DeltaDeletedEntry,
                        "The IODataJsonReaderResourceState is only supported on DeltaResource or DeltaDeletedEntry scope.");
                    return this.EntityType;
                }
            }

            /// <summary>
            /// The expected type defined in the model for the resource
            /// </summary>
            public IEdmStructuredType ResourceTypeFromMetadata { get; set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightNestedResourceInfoScope : Scope
        {
            /// <summary>
            /// The underlying reader for reading nested resource set or resource.
            /// </summary>
            private readonly ODataReader nestedResourceInfoReader;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info attached to this scope.</param>
            /// <param name="parentNavigationSource">The parent navigation source for the scope.</param>
            /// <param name="expectedResourceType">The resource type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <param name="jsonLightInputContext">The input context for Json.</param>
            /// <remarks>The <paramref name="expectedResourceType"/> is the expected base type the items in the nested resource info.</remarks>
            public JsonLightNestedResourceInfoScope(ODataJsonLightReaderNestedResourceInfo nestedResourceInfo, IEdmNavigationSource parentNavigationSource, IEdmStructuredType expectedResourceType, ODataUri odataUri, ODataJsonLightInputContext jsonLightInputContext)
                : base(ODataDeltaReaderState.NestedResource, nestedResourceInfo.NestedResourceInfo, parentNavigationSource, null, odataUri)
            {
                Debug.Assert(nestedResourceInfo != null, "navigationLinkInfo != null");
                Debug.Assert(nestedResourceInfo.NavigationProperty != null || nestedResourceInfo.StructuralProperty != null || expectedResourceType != null,
                    "nestedResourceInfo.NavigationProperty != null || nestedResourceInfo.StructuralProperty != null || expectedResourceType != null");
                Debug.Assert(parentNavigationSource != null, "parentNavigationSource != null");
                Debug.Assert(expectedResourceType != null, "parentEntityType != null");
                Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");

                bool readingResourceSet = nestedResourceInfo.NestedResourceSet != null;
                IEdmNavigationSource navigationSource = null;
                if (nestedResourceInfo.NavigationProperty != null)
                {
                    navigationSource = parentNavigationSource.FindNavigationTarget(nestedResourceInfo.NavigationProperty);
                }

                this.nestedResourceInfoReader = new ODataJsonLightReader(jsonLightInputContext, navigationSource, expectedResourceType, readingResourceSet, readingDelta: true);
            }

            /// <summary>
            /// The current state of the underlying expanded navigation property reader.
            /// </summary>
            public ODataReaderState SubState
            {
                get { return this.nestedResourceInfoReader.State; }
            }

            /// <summary>
            /// The current item. It will be the nested resource info when the subState is Start or Complected, or the item of the underlying nested resource info reader.
            /// </summary>
            public new ODataItem Item
            {
                get
                {
                    return this.State == ODataDeltaReaderState.NestedResource
                        && (this.SubState == ODataReaderState.Start || this.SubState == ODataReaderState.Completed)
                        ? base.Item : this.nestedResourceInfoReader.Item;
                }
            }

            /// <summary>
            /// The underlying reader for reading nested resource set or resource.
            /// </summary>
            public ODataReader NestedResourceInfoReader
            {
                get { return this.nestedResourceInfoReader; }
            }
        }

        /// <summary>
        /// A reader top-level scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightTopLevelScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the top-level resource or entries in the top-level resource set.
            /// In all cases the specified type must be an entity type.</remarks>
            public JsonLightTopLevelScope(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
                : base(ODataDeltaReaderState.Start, /*item*/ null, navigationSource, expectedEntityType, null)
            {
            }

            /// <summary>
            /// The duplicate property names checker for the top level scope represented by the current state.
            /// </summary>
            public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; set; }
        }

#endregion
    }
}
