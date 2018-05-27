//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;

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
        private PropertyAndAnnotationCollector propertyAndAnnotationCollector;

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
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");
            Debug.Assert(this.jsonLightParameterDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            this.propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, resource set or collection.
            // Position the reader on the first node
            this.jsonLightParameterDeserializer.ReadPayloadStart(
                ODataPayloadKind.Parameter,
                this.propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/true);

            return this.ReadAtStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the parameter reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        protected override Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");
            Debug.Assert(this.jsonLightParameterDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            this.propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, resource set or collection.
            // Position the reader on the first node
            return this.jsonLightParameterDeserializer.ReadPayloadStartAsync(
                ODataPayloadKind.Parameter,
                this.propertyAndAnnotationCollector,
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
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        protected override bool ReadNextParameterImplementation()
        {
            return this.ReadNextParameterImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state Value, Resource, Resource Set or Collection state.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property or JsonNodeType.EndObject:     assumes the last read puts the reader at the begining of the next parameter or at the end of the payload.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        protected override Task<bool> ReadNextParameterImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadNextParameterImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected resource type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.</returns>
        protected override ODataReader CreateResourceReader(IEdmStructuredType expectedResourceType)
        {
            return this.CreateResourceReaderSynchronously(expectedResourceType);
        }

#if PORTABLELIB
        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.</returns>
        protected override Task<ODataReader> CreateResourceReaderAsync(IEdmStructuredType expectedResourceType)
        {
            return TaskUtils.GetTaskForSynchronousOperation<ODataReader>(() => this.CreateResourceReaderSynchronously(expectedResourceType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected resource set element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.</returns>
        protected override ODataReader CreateResourceSetReader(IEdmStructuredType expectedResourceType)
        {
            return this.CreateResourceSetReaderSynchronously(expectedResourceType);
        }

#if PORTABLELIB
        /// <summary>
        /// Cretes an <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected resource set element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.</returns>
        protected override Task<ODataReader> CreateResourceSetReaderAsync(IEdmStructuredType expectedResourceType)
        {
            return TaskUtils.GetTaskForSynchronousOperation<ODataReader>(() => this.CreateResourceSetReaderSynchronously(expectedResourceType));
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

#if PORTABLELIB
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
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously()
        {
            if (this.jsonLightInputContext.JsonReader.NodeType == JsonNodeType.EndOfInput)
            {
                this.PopScope(ODataParameterReaderState.Start);
                this.EnterScope(ODataParameterReaderState.Completed, null, null);
                return false;
            }

            return this.jsonLightParameterDeserializer.ReadNextParameter(this.propertyAndAnnotationCollector);
        }

        /// <summary>
        /// Implementation of the reader logic on the subsequent reads after the first parameter is read.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property or JsonNodeType.EndObject:     assumes the last read puts the reader at the begining of the next parameter or at the end of the payload.
        /// Post-Condition: When the new state is Value, the reader is positioned at the closing '}' or at the name of the next parameter.
        ///                 When the new state is Resource, the reader is positioned at the starting '{' of the resource payload.
        ///                 When the new state is Resource Set or Collection, the reader is positioned at the starting '[' of the resource set or collection payload.
        /// </remarks>
        private bool ReadNextParameterImplementationSynchronously()
        {
            Debug.Assert(
                this.State != ODataParameterReaderState.Start &&
                this.State != ODataParameterReaderState.Exception &&
                this.State != ODataParameterReaderState.Completed,
                "The current state must not be Start, Exception or Completed.");

            this.PopScope(this.State);
            return this.jsonLightParameterDeserializer.ReadNextParameter(this.propertyAndAnnotationCollector);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.</returns>
        private ODataReader CreateResourceReaderSynchronously(IEdmStructuredType expectedResourceType)
        {
            Debug.Assert(expectedResourceType != null, "expectedResourceType != null");
            return new ODataJsonLightReader(this.jsonLightInputContext, null, expectedResourceType, false /*readingResourceSet*/, true /*readingParameter*/, false /*readingDelta*/, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected resource set element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.</returns>
        private ODataReader CreateResourceSetReaderSynchronously(IEdmStructuredType expectedResourceType)
        {
            Debug.Assert(expectedResourceType != null, "expectedResourceType != null");
            return new ODataJsonLightReader(this.jsonLightInputContext, null, expectedResourceType, true /*readingResourceSet*/, true /*readingParameter*/, false /*readingDelta*/, this /*IODataReaderListener*/);
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
