//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using o = Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// OData collection reader for the JSON format.
    /// </summary>
    internal sealed class ODataJsonCollectionReader : ODataCollectionReaderCore
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
        internal ODataJsonCollectionReader(ODataJsonInputContext jsonInputContext, IEdmTypeReference expectedItemTypeReference, IODataReaderWriterListener listener)
            : base(jsonInputContext, expectedItemTypeReference, listener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");

            this.jsonInputContext = jsonInputContext;
            this.jsonCollectionDeserializer = new ODataJsonCollectionDeserializer(jsonInputContext);

            if (!jsonInputContext.Model.IsUserModel())
            {
                throw new ODataException(o.Strings.ODataJsonCollectionReader_ParsingWithoutMetadata);
            }
        }

        /// <summary>
        /// Set to true if collections are expected to have the 'results' wrapper.
        /// Collections are only expected to have a results wrapper if
        /// (a) the protocol version is >= 2 AND
        /// (b) we are reading a response
        /// NOTE: OIPI does not specify a format for >= v2 collections in requests; we thus use the v1 format and consequently do not expect a result wrapper.
        /// </summary>
        private bool IsResultsWrapperExpected
        {
            get
            {
                return this.jsonInputContext.Version >= ODataVersion.V2 && this.jsonInputContext.ReadingResponse;
            }
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

            // read the data wrapper depending on whether we are reading a request or response
            this.jsonCollectionDeserializer.ReadPayloadStart(this.IsReadingNestedPayload);

            if (this.IsResultsWrapperExpected && this.jsonCollectionDeserializer.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(o.Strings.ODataJsonCollectionReader_CannotReadWrappedCollectionStart(this.jsonCollectionDeserializer.JsonReader.NodeType));
            }

            if (!this.IsResultsWrapperExpected && this.jsonCollectionDeserializer.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(o.Strings.ODataJsonCollectionReader_CannotReadCollectionStart(this.jsonCollectionDeserializer.JsonReader.NodeType));
            }

            // read the start of the collection until we find the content array
            ODataCollectionStart collectionStart = this.jsonCollectionDeserializer.ReadCollectionStart(this.IsResultsWrapperExpected);

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
        protected override bool ReadAtCollectionStartImplementation()
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
        protected override bool ReadAtValueImplementation()
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
        protected override bool ReadAtCollectionEndImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionEnd, "this.State == ODataCollectionReaderState.CollectionEnd");
            Debug.Assert(this.jsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-Condition: expected JsonNodeType.EndArray");

            this.PopScope(ODataCollectionReaderState.CollectionEnd);
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

            // read the end-array node and the (potential) end-object node of the 'results' wrapper
            this.jsonCollectionDeserializer.ReadCollectionEnd(this.IsResultsWrapperExpected);

            // read the end-of-payload suffix (with the potential data wrapper end-object node)
            this.jsonCollectionDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
            Debug.Assert(this.IsReadingNestedPayload || this.jsonInputContext.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input when not reading a nested payload.");

            // replace the 'Start' scope with the 'Completed' scope
            this.ReplaceScope(ODataCollectionReaderState.Completed, null);

            return false;
        }
    }
}
