//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// OData collection reader for the Json format.
    /// </summary>
    internal sealed class ODataJsonCollectionReader : ODataCollectionReaderCoreAsync
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>The collection deserializer to use to read from the input.</summary>
        private readonly ODataJsonCollectionDeserializer jsonCollectionDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The input to read the payload from.</param>
        /// <param name="expectedItemTypeReference">The expected type for the items in the collection.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        internal ODataJsonCollectionReader(
            ODataJsonInputContext jsonInputContext,
            IEdmTypeReference expectedItemTypeReference,
            IODataReaderWriterListener listener)
            : base(jsonInputContext, expectedItemTypeReference, listener)
        {
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");

            this.jsonInputContext = jsonInputContext;
            this.jsonCollectionDeserializer = new ODataJsonCollectionDeserializer(jsonInputContext);
        }

        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the first item or the EndArray node of an empty item array
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");
            Debug.Assert(this.IsReadingNestedPayload || this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.jsonInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or not
            this.jsonCollectionDeserializer.ReadPayloadStart(
                ODataPayloadKind.Collection,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                /*allowEmptyPayload*/false);

            return this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector);
        }

        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <returns>Task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the first item or the EndArray node of an empty item array
        /// </remarks>
        protected override async Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");
            Debug.Assert(this.IsReadingNestedPayload || this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.jsonInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or not
            await this.jsonCollectionDeserializer.ReadPayloadStartAsync(
                ODataPayloadKind.Collection,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                allowEmptyPayload: false).ConfigureAwait(false);

            return await this.ReadAtStartImplementationAsynchronously(propertyAndAnnotationCollector)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the first item in the collection or the EndArray node of the (empty) item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for an empty item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex value as first item
        ///                 JsonNodeType.PrimitiveValue: for a primitive value as first item
        /// Post-Condition: The reader is positioned on the first node of the second item or an EndArray node if there are no items in the collection
        /// </remarks>
        protected override bool ReadAtCollectionStartImplementation()
        {
            return this.ReadAtCollectionStartImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>Task which returns true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the first item in the collection or the EndArray node of the (empty) item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for an empty item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex value as first item
        ///                 JsonNodeType.PrimitiveValue: for a primitive value as first item
        /// Post-Condition: The reader is positioned on the first node of the second item or an EndArray node if there are no items in the collection
        /// </remarks>
        protected override Task<bool> ReadAtCollectionStartImplementationAsync()
        {
            return this.ReadAtCollectionStartImplementationAsynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the next item in the collection or the EndArray node of the item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for the end of the item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no items in the collection
        /// </remarks>
        protected override bool ReadAtValueImplementation()
        {
            return this.ReadAtValueImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>Task which returns true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the next item in the collection or the EndArray node of the item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for the end of the item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no items in the collection
        /// </remarks>
        protected override Task<bool> ReadAtValueImplementationAsync()
        {
            return this.ReadAtValueImplementationAsynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>false since no more nodes can be read from the reader after the collection ended.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.EndArray        the end of the item array of the collection
        /// Post-Condition: JsonNodeType.EndOfInput     nothing else to read when not reading a nested payload
        /// </remarks>
        protected override bool ReadAtCollectionEndImplementation()
        {
            return this.ReadAtCollectionEndImplementationSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>Task which should return false since no more nodes can be read from the reader after the collection ends.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.EndArray        the end of the item array of the collection
        /// Post-Condition: JsonNodeType.EndOfInput     nothing else to read when not reading a nested payload
        /// </remarks>
        protected override Task<bool> ReadAtCollectionEndImplementationAsync()
        {
            return this.ReadAtCollectionEndImplementationAsynchronously();
        }

        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the top-level scope.</param>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the first item or the EndArray node of an empty item array
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            IEdmTypeReference actualItemTypeReference;
            this.ExpectedItemTypeReference = ReaderValidationUtils.ValidateCollectionContextUriAndGetPayloadItemTypeReference(
                this.jsonCollectionDeserializer.ContextUriParseResult,
                this.ExpectedItemTypeReference);

            // read the start of the collection until we find the content array for top-level collections
            ODataCollectionStart collectionStart = this.jsonCollectionDeserializer.ReadCollectionStart(
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                this.ExpectedItemTypeReference,
                out actualItemTypeReference);

            if (actualItemTypeReference != null)
            {
                this.ExpectedItemTypeReference = actualItemTypeReference;
            }

            this.jsonCollectionDeserializer.JsonReader.ReadStartArray();

            this.EnterScope(ODataCollectionReaderState.CollectionStart, collectionStart);

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the first item in the collection or the EndArray node of the (empty) item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for an empty item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex value as first item
        ///                 JsonNodeType.PrimitiveValue: for a primitive value as first item
        /// Post-Condition: The reader is positioned on the first node of the second item or an EndArray node if there are no items in the collection
        /// </remarks>
        private bool ReadAtCollectionStartImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionStart, "this.State == ODataCollectionReaderState.CollectionStart");

            if (this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // empty collection
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.jsonCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
                this.EnterScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the next item in the collection or the EndArray node of the item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for the end of the item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no items in the collection
        /// </remarks>
        private bool ReadAtValueImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Value, "this.State == ODataCollectionReaderState.Value");

            if (this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // end of collection reached
                this.PopScope(ODataCollectionReaderState.Value);
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.jsonCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
                this.ReplaceScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>false since no more nodes can be read from the reader after the collection ended.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.EndArray        the end of the item array of the collection
        /// Post-Condition: JsonNodeType.EndOfInput     nothing else to read when not reading a nested payload
        /// </remarks>
        private bool ReadAtCollectionEndImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionEnd, "this.State == ODataCollectionReaderState.CollectionEnd");
            Debug.Assert(this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-Condition: expected JsonNodeType.EndArray");

            this.PopScope(ODataCollectionReaderState.CollectionEnd);
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

            this.jsonCollectionDeserializer.ReadCollectionEnd(this.IsReadingNestedPayload);
            this.jsonCollectionDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);

            // replace the 'Start' scope with the 'Completed' scope
            this.ReplaceScope(ODataCollectionReaderState.Completed, null);

            return false;
        }

        /// <summary>
        /// Asynchronous implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the top-level scope.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the first item or the EndArray node of an empty item array
        /// </remarks>
        private async Task<bool> ReadAtStartImplementationAsynchronously(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            IEdmTypeReference actualItemTypeReference;
            this.ExpectedItemTypeReference = ReaderValidationUtils.ValidateCollectionContextUriAndGetPayloadItemTypeReference(
                this.jsonCollectionDeserializer.ContextUriParseResult,
                this.ExpectedItemTypeReference);

            // Read the start of the collection until we find the content array for top-level collections
            Tuple<ODataCollectionStart, IEdmTypeReference> readCollectionStartResult = await this.jsonCollectionDeserializer.ReadCollectionStartAsync(
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                this.ExpectedItemTypeReference).ConfigureAwait(false);
            ODataCollectionStart collectionStart = readCollectionStartResult.Item1;
            actualItemTypeReference = readCollectionStartResult.Item2;

            if (actualItemTypeReference != null)
            {
                this.ExpectedItemTypeReference = actualItemTypeReference;
            }

            await this.jsonCollectionDeserializer.JsonReader.ReadStartArrayAsync()
                .ConfigureAwait(false);

            this.EnterScope(ODataCollectionReaderState.CollectionStart, collectionStart);

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more nodes can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the first item in the collection or the EndArray node of the (empty) item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for an empty item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex value as first item
        ///                 JsonNodeType.PrimitiveValue: for a primitive value as first item
        /// Post-Condition: The reader is positioned on the first node of the second item or an EndArray node if there are no items in the collection
        /// </remarks>
        private async Task<bool> ReadAtCollectionStartImplementationAsynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionStart, "this.State == ODataCollectionReaderState.CollectionStart");

            if (this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // Empty collection
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = await this.jsonCollectionDeserializer.ReadCollectionItemAsync(
                    this.ExpectedItemTypeReference,
                    this.CollectionValidator).ConfigureAwait(false);
                this.EnterScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more nodes can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the next item in the collection or the EndArray node of the item array
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.EndArray:       for the end of the item array of the collection
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no items in the collection
        /// </remarks>
        private async Task<bool> ReadAtValueImplementationAsynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Value, "this.State == ODataCollectionReaderState.Value");

            if (this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // End of collection reached
                this.PopScope(ODataCollectionReaderState.Value);
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = await this.jsonCollectionDeserializer.ReadCollectionItemAsync(
                    this.ExpectedItemTypeReference,
                    this.CollectionValidator).ConfigureAwait(false);
                this.ReplaceScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains false since no more nodes can be read from the reader after the collection ended.
        /// </returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.EndArray        the end of the item array of the collection
        /// Post-Condition: JsonNodeType.EndOfInput     nothing else to read when not reading a nested payload
        /// </remarks>
        private async Task<bool> ReadAtCollectionEndImplementationAsynchronously()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionEnd, "this.State == ODataCollectionReaderState.CollectionEnd");
            Debug.Assert(this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-Condition: expected JsonNodeType.EndArray");

            this.PopScope(ODataCollectionReaderState.CollectionEnd);
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

            await this.jsonCollectionDeserializer.ReadCollectionEndAsync(this.IsReadingNestedPayload)
                .ConfigureAwait(false);
            await this.jsonCollectionDeserializer.ReadPayloadEndAsync(this.IsReadingNestedPayload)
                .ConfigureAwait(false);

            // Replace the 'Start' scope with the 'Completed' scope
            this.ReplaceScope(ODataCollectionReaderState.Completed, null);

            return false;
        }
    }
}
