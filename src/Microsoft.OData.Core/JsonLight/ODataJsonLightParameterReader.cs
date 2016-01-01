//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    /// <summary>
    /// OData parameter reader for the Json Light format.
    /// </summary>
    internal sealed class ODataJsonLightParameterReader : ODataParameterReaderCoreAsync
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>The parameter deserializer to read the parameter input with.</summary>
        private readonly ODataJsonLightParameterDeserializer jsonLightParameterDeserializer;

        /// <summary>The duplicate property names checker to use for the parameter payload.</summary>
        private DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The input to read the payload from.</param>
        /// <param name="operation">The operation import whose parameters are being read.</param>
        internal ODataJsonLightParameterReader(ODataJsonLightInputContext jsonLightInputContext, IEdmOperation operation)
            : base(jsonLightInputContext, operation)
        {
            Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");
            Debug.Assert(jsonLightInputContext.ReadingResponse == false, "jsonLightInputContext.ReadingResponse == false");
            Debug.Assert(operation != null, "operationImport != null");

            this.jsonLightInputContext = jsonLightInputContext;
            this.jsonLightParameterDeserializer = new ODataJsonLightParameterDeserializer(this, jsonLightInputContext);
            Debug.Assert(this.jsonLightInputContext.Model.IsUserModel(), "this.jsonLightInputContext.Model.IsUserModel()");
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");
            Debug.Assert(this.jsonLightParameterDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            this.duplicatePropertyNamesChecker = this.jsonLightInputContext.CreateDuplicatePropertyNamesChecker();

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, feed or collection.
            // Position the reader on the first node
            this.jsonLightParameterDeserializer.ReadPayloadStart(
                ODataPayloadKind.Parameter,
                this.duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/true);

            return this.ReadAtStartImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the parameter reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");
            Debug.Assert(this.jsonLightParameterDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            this.duplicatePropertyNamesChecker = this.jsonLightInputContext.CreateDuplicatePropertyNamesChecker();

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, feed or collection.
            // Position the reader on the first node
            return this.jsonLightParameterDeserializer.ReadPayloadStartAsync(
                ODataPayloadKind.Parameter,
                this.duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/true)

                .FollowOnSuccessWith(t =>
                    this.ReadAtStartImplementationSynchronously());
        }
#endif

        /// <summary>
        /// Implementation of the reader logic on the subsequent reads after the first parameter is read.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property or JsonNodeType.EndObject:     assumes the last read puts the reader at the begining of the next parameter or at the end of the payload.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        protected override bool ReadNextParameterImplementation()
        {
            return this.ReadNextParameterImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state Value, Entry, Feed or Collection state.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property or JsonNodeType.EndObject:     assumes the last read puts the reader at the begining of the next parameter or at the end of the payload.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadNextParameterImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadNextParameterImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.</returns>
        protected override ODataReader CreateEntryReader(IEdmEntityType expectedEntityType)
        {
            return this.CreateEntryReaderSynchronously(expectedEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<ODataReader> CreateEntryReaderAsync(IEdmEntityType expectedEntityType)
        {
            return TaskUtils.GetTaskForSynchronousOperation<ODataReader>(() => this.CreateEntryReaderSynchronously(expectedEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected feed element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.</returns>
        protected override ODataReader CreateFeedReader(IEdmEntityType expectedEntityType)
        {
            return this.CreateFeedReaderSynchronously(expectedEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Cretes an <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected feed element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedEntityType)
        {
            return TaskUtils.GetTaskForSynchronousOperation<ODataReader>(() => this.CreateFeedReaderSynchronously(expectedEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        /// <remarks>
        /// Pre-Condition:  Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// Post-Condition: Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// NOTE: this method does not move the reader.
        /// </remarks>
        protected override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            return this.CreateCollectionReaderSynchronously(expectedItemTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        /// <remarks>
        /// Pre-Condition:  Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// Post-Condition: Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// NOTE: this method does not move the reader.
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            return TaskUtils.GetTaskForSynchronousOperation<ODataCollectionReader>(() => this.CreateCollectionReaderSynchronously(expectedItemTypeReference));
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously()
        {
            if (this.jsonLightInputContext.JsonReader.NodeType == JsonNodeType.EndOfInput)
            {
                this.PopScope(ODataParameterReaderState.Start);
                this.EnterScope(ODataParameterReaderState.Completed, null, null);
                return false;
            }

            return this.jsonLightParameterDeserializer.ReadNextParameter(this.duplicatePropertyNamesChecker);
        }

        /// <summary>
        /// Implementation of the reader logic on the subsequent reads after the first parameter is read.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property or JsonNodeType.EndObject:     assumes the last read puts the reader at the begining of the next parameter or at the end of the payload.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Entry, the reader is positioned at the starting '{' of the entry payload.
        ///                 When the new state is Feed or Collection, the reader is positioned at the starting '[' of the feed or collection payload.
        /// </remarks>
        private bool ReadNextParameterImplementationSynchronously()
        {
            Debug.Assert(
                this.State != ODataParameterReaderState.Start &&
                this.State != ODataParameterReaderState.Exception &&
                this.State != ODataParameterReaderState.Completed,
                "The current state must not be Start, Exception or Completed.");

            this.PopScope(this.State);
            return this.jsonLightParameterDeserializer.ReadNextParameter(this.duplicatePropertyNamesChecker);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.</returns>
        private ODataReader CreateEntryReaderSynchronously(IEdmEntityType expectedEntityType)
        {
            Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
            return new ODataJsonLightReader(this.jsonLightInputContext, null, expectedEntityType, false /*readingFeed*/, true /*readingParameter*/, false /*readingDelta*/, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected feed element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.</returns>
        private ODataReader CreateFeedReaderSynchronously(IEdmEntityType expectedEntityType)
        {
            Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
            return new ODataJsonLightReader(this.jsonLightInputContext, null, expectedEntityType, true /*readingFeed*/, true /*readingParameter*/, false /*readingDelta*/, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        /// <remarks>
        /// Pre-Condition:  Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// Post-Condition: Any:    the reader should be on the start array node of the collection value; if it is not we let the collection reader fail.
        /// NOTE: this method does not move the reader.
        /// </remarks>
        private ODataCollectionReader CreateCollectionReaderSynchronously(IEdmTypeReference expectedItemTypeReference)
        {
            Debug.Assert(this.jsonLightInputContext.Model.IsUserModel(), "Should have verified that we created the parameter reader with a user model.");
            Debug.Assert(expectedItemTypeReference != null, "expectedItemTypeReference != null");
            return new ODataJsonLightCollectionReader(this.jsonLightInputContext, expectedItemTypeReference, this /*IODataReaderListener*/);
        }
    }
}
