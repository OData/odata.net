//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData collection reader for the Verbose JSON format.
    /// </summary>
    internal sealed class ODataVerboseJsonCollectionReader : ODataCollectionReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataVerboseJsonInputContext verboseJsonInputContext;

        /// <summary>The collection deserializer to use to read from the input.</summary>
        private readonly ODataVerboseJsonCollectionDeserializer verboseJsonCollectionDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The input to read the payload from.</param>
        /// <param name="expectedItemTypeReference">The expected type for the items in the collection.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        internal ODataVerboseJsonCollectionReader(
            ODataVerboseJsonInputContext verboseJsonInputContext,
            IEdmTypeReference expectedItemTypeReference, 
            IODataReaderWriterListener listener)
            : base(verboseJsonInputContext, expectedItemTypeReference, listener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonInputContext != null, "verboseJsonInputContext != null");

            this.verboseJsonInputContext = verboseJsonInputContext;
            this.verboseJsonCollectionDeserializer = new ODataVerboseJsonCollectionDeserializer(verboseJsonInputContext);

            if (!verboseJsonInputContext.Model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionReader_ParsingWithoutMetadata);
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
                return this.verboseJsonInputContext.Version >= ODataVersion.V2 && this.verboseJsonInputContext.ReadingResponse;
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
            Debug.Assert(this.IsReadingNestedPayload || this.verboseJsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            // read the data wrapper depending on whether we are reading a request or response
            this.verboseJsonCollectionDeserializer.ReadPayloadStart(this.IsReadingNestedPayload);

            if (this.IsResultsWrapperExpected && this.verboseJsonCollectionDeserializer.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionReader_CannotReadWrappedCollectionStart(this.verboseJsonCollectionDeserializer.JsonReader.NodeType));
            }

            if (!this.IsResultsWrapperExpected && this.verboseJsonCollectionDeserializer.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionReader_CannotReadCollectionStart(this.verboseJsonCollectionDeserializer.JsonReader.NodeType));
            }

            // read the start of the collection until we find the content array
            ODataCollectionStart collectionStart = this.verboseJsonCollectionDeserializer.ReadCollectionStart(this.IsResultsWrapperExpected);

            this.verboseJsonCollectionDeserializer.JsonReader.ReadStartArray();

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

            if (this.verboseJsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // empty collection
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.verboseJsonCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
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

            if (this.verboseJsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
            {
                // end of collection reached
                this.PopScope(ODataCollectionReaderState.Value);
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.verboseJsonCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
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
            Debug.Assert(this.verboseJsonCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-Condition: expected JsonNodeType.EndArray");

            this.PopScope(ODataCollectionReaderState.CollectionEnd);
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

            // read the end-array node and the (potential) end-object node of the 'results' wrapper
            this.verboseJsonCollectionDeserializer.ReadCollectionEnd(this.IsResultsWrapperExpected);

            // read the end-of-payload suffix (with the potential data wrapper end-object node)
            this.verboseJsonCollectionDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
            Debug.Assert(this.IsReadingNestedPayload || this.verboseJsonInputContext.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input when not reading a nested payload.");

            // replace the 'Start' scope with the 'Completed' scope
            this.ReplaceScope(ODataCollectionReaderState.Completed, null);

            return false;
        }
    }
}
