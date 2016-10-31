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
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData parameter reader for the Verbose JSON format.
    /// </summary>
    internal sealed class ODataVerboseJsonParameterReader : ODataParameterReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataVerboseJsonInputContext verboseJsonInputContext;

        /// <summary>The property and value deserializer to read input with.</summary>
        private readonly ODataVerboseJsonPropertyAndValueDeserializer verboseJsonPropertyAndValueDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The input to read the payload from.</param>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        internal ODataVerboseJsonParameterReader(ODataVerboseJsonInputContext verboseJsonInputContext, IEdmFunctionImport functionImport)
            : base(verboseJsonInputContext, functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonInputContext != null, "verboseJsonInputContext != null");
            Debug.Assert(verboseJsonInputContext.ReadingResponse == false, "verboseJsonInputContext.ReadingResponse == false");

            this.verboseJsonInputContext = verboseJsonInputContext;
            this.verboseJsonPropertyAndValueDeserializer = new ODataVerboseJsonPropertyAndValueDeserializer(verboseJsonInputContext);
            Debug.Assert(this.verboseJsonInputContext.Model.IsUserModel(), "this.verboseJsonInputContext.Model.IsUserModel()");
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
            Debug.Assert(this.verboseJsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, feed or collection.
            // Since the parameter payload must come from a request message, ReadPayloadStart() simply positions the reader on the first node.
            this.verboseJsonPropertyAndValueDeserializer.ReadPayloadStart(false /*isReadingNestedPayload*/);
            if (this.verboseJsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput)
            {
                this.PopScope(ODataParameterReaderState.Start);
                this.EnterScope(ODataParameterReaderState.Completed, null, null);
                return false;
            }

            // Read the starting '{'.
            this.verboseJsonPropertyAndValueDeserializer.JsonReader.ReadStartObject();

            if (this.EndOfParameters())
            {
                this.ReadParametersEnd();
                return false;
            }
            else
            {
                // Read the first parameter.
                this.ReadNextParameter();
                return true;
            }
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
        protected override bool ReadNextParameterImplementation()
        {
            Debug.Assert(
                this.State != ODataParameterReaderState.Start &&
                this.State != ODataParameterReaderState.Exception &&
                this.State != ODataParameterReaderState.Completed,
                "The current state must not be Start, Exception or Completed.");

            this.PopScope(this.State);
            if (this.EndOfParameters())
            {
                this.ReadParametersEnd();
                return false;
            }
            else
            {
                this.ReadNextParameter();
                return true;
            }
        }

#if SUPPORT_ENTITY_PARAMETER
        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.</returns>
        protected override ODataReader CreateEntryReader(IEdmEntityType expectedEntityType)
        {
            Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
            return new ODataJsonReader(this.jsonInputContext, expectedEntityType, false /*readingFeed*/, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Cretes an <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected feed element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.</returns>
        protected override ODataReader CreateFeedReader(IEdmEntityType expectedEntityType)
        {
            Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
            return new ODataJsonReader(this.jsonInputContext, expectedEntityType, true /*readingFeed*/, this /*IODataReaderListener*/);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        protected override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            Debug.Assert(expectedItemTypeReference != null, "expectedItemTypeReference != null");
            return new ODataVerboseJsonCollectionReader(this.verboseJsonInputContext, expectedItemTypeReference, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Checks to see if we are at the end of the parameters payload.
        /// </summary>
        /// <returns>Returns true if we are at the ending '}' of the parameters payload.</returns>
        private bool EndOfParameters()
        {
            return this.verboseJsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndObject;
        }

        /// <summary>
        /// Reads the end '}' of the parameters payload.
        /// </summary>
        private void ReadParametersEnd()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");

            // Read the ending '}'.
            this.verboseJsonPropertyAndValueDeserializer.JsonReader.ReadEndObject();
            this.verboseJsonPropertyAndValueDeserializer.ReadPayloadEnd(false /*isReadingNestedPayload*/);
            Debug.Assert(this.verboseJsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.PopScope(ODataParameterReaderState.Start);
            this.EnterScope(ODataParameterReaderState.Completed, null, null);
        }

        /// <summary>
        /// Reads the next parameter from the parameters payload.
        /// </summary>
        private void ReadNextParameter()
        {
            ODataParameterReaderState state;
            string parameterName = this.verboseJsonPropertyAndValueDeserializer.JsonReader.ReadPropertyName();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            IEdmTypeReference parameterTypeReference = this.GetParameterTypeReference(parameterName);
            Debug.Assert(parameterTypeReference != null, "parameterTypeReference != null");

            object parameterValue;
            switch (parameterTypeReference.TypeKind())
            {
                case EdmTypeKind.Primitive:
                    IEdmPrimitiveTypeReference primitiveTypeReference = parameterTypeReference.AsPrimitive();
                    if (primitiveTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Stream)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonParameterReader_UnsupportedPrimitiveParameterType(parameterName, primitiveTypeReference.PrimitiveKind()));
                    }

                    parameterValue = this.verboseJsonPropertyAndValueDeserializer.ReadNonEntityValue(
                        primitiveTypeReference,
                        /*duplicatePropertyNamesChecker*/ null,
                        /*collectionValidator*/ null,
                        /*validateNullValue*/ true,
                        parameterName);
                    state = ODataParameterReaderState.Value;
                    break;

                case EdmTypeKind.Complex:
                    parameterValue = this.verboseJsonPropertyAndValueDeserializer.ReadNonEntityValue(
                        parameterTypeReference,
                        /*duplicatePropertyNamesChecker*/ null,
                        /*collectionValidator*/ null,
                        /*validateNullValue*/ true,
                        parameterName);
                    state = ODataParameterReaderState.Value;
                    break;

#if SUPPORT_ENTITY_PARAMETER
                case EdmTypeKind.Entity:
                    parameterValue = null;
                    state = ODataParameterReaderState.Entry;
                    break;
#endif

                case EdmTypeKind.Collection:
                    parameterValue = null;
                    if (this.verboseJsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                    {
                        parameterValue = this.verboseJsonPropertyAndValueDeserializer.JsonReader.ReadPrimitiveValue();
                        if (parameterValue != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonParameterReader_NullCollectionExpected(JsonNodeType.PrimitiveValue, parameterValue));
                        }

                        state = ODataParameterReaderState.Value;
                    }
                    else if (((IEdmCollectionType)parameterTypeReference.Definition).ElementType.TypeKind() == EdmTypeKind.Entity)
                    {
#if SUPPORT_ENTITY_PARAMETER
                        state = ODataParameterReaderState.Feed;
#else
                        throw new ODataException(ODataErrorStrings.ODataJsonParameterReader_UnsupportedParameterTypeKind(parameterName, "Entity Collection"));
#endif
                    }
                    else
                    {
                        state = ODataParameterReaderState.Collection;
                    }

                    break;
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonParameterReader_UnsupportedParameterTypeKind(parameterName, parameterTypeReference.TypeKind()));
            }

            this.EnterScope(state, parameterName, parameterValue);
        }
    }
}
