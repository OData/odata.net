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
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using o = Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// OData reader for the JSON format.
    /// </summary>
    internal sealed class ODataJsonParameterReader : ODataParameterReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>The property and value deserializer to read input with.</summary>
        private readonly ODataJsonPropertyAndValueDeserializer jsonPropertyAndValueDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The input to read the payload from.</param>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        internal ODataJsonParameterReader(ODataJsonInputContext jsonInputContext, IEdmFunctionImport functionImport)
            : base(jsonInputContext, functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");
            Debug.Assert(jsonInputContext.ReadingResponse == false, "jsonInputContext.ReadingResponse == false");

            this.jsonInputContext = jsonInputContext;
            this.jsonPropertyAndValueDeserializer = new ODataJsonPropertyAndValueDeserializer(jsonInputContext);
            Debug.Assert(this.jsonInputContext.Model.IsUserModel(), "this.jsonInputContext.Model.IsUserModel()");
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
            Debug.Assert(this.jsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // The parameter payload looks like "{ param1 : value1, ..., paramN : valueN }", where each value can be primitive, complex, collection, entity, feed or collection.
            // Since the parameter payload must come from a request message, ReadPayloadStart() simply positions the reader on the first node.
            this.jsonPropertyAndValueDeserializer.ReadPayloadStart(false /*isReadingNestedPayload*/);
            if (this.jsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput)
            {
                this.PopScope(ODataParameterReaderState.Start);
                this.EnterScope(ODataParameterReaderState.Completed, null, null);
                return false;
            }

            // Read the starting '{'.
            this.jsonPropertyAndValueDeserializer.JsonReader.ReadStartObject();

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
            return new ODataJsonCollectionReader(this.jsonInputContext, expectedItemTypeReference, this /*IODataReaderListener*/);
        }

        /// <summary>
        /// Checks to see if we are at the end of the parameters payload.
        /// </summary>
        /// <returns>Returns true if we are at the ending '}' of the parameters payload.</returns>
        private bool EndOfParameters()
        {
            return this.jsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndObject;
        }

        /// <summary>
        /// Reads the end '}' of the parameters payload.
        /// </summary>
        private void ReadParametersEnd()
        {
            Debug.Assert(this.State == ODataParameterReaderState.Start, "this.State == ODataParameterReaderState.Start");

            // Read the ending '}'.
            this.jsonPropertyAndValueDeserializer.JsonReader.ReadEndObject();
            this.jsonPropertyAndValueDeserializer.ReadPayloadEnd(false /*isReadingNestedPayload*/);
            Debug.Assert(this.jsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.PopScope(ODataParameterReaderState.Start);
            this.EnterScope(ODataParameterReaderState.Completed, null, null);
        }

        /// <summary>
        /// Reads the next parameter from the parameters payload.
        /// </summary>
        private void ReadNextParameter()
        {
            ODataParameterReaderState state;
            string parameterName = this.jsonPropertyAndValueDeserializer.JsonReader.ReadPropertyName();
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
                        throw new ODataException(o.Strings.ODataJsonParameterReader_UnsupportedPrimitiveParameterType(parameterName, primitiveTypeReference.PrimitiveKind()));
                    }

                    parameterValue = this.jsonPropertyAndValueDeserializer.ReadNonEntityValue(
                        primitiveTypeReference,
                        /*duplicatePropertyNamesChecker*/ null,
                        /*collectionValidator*/ null,
                        /*validateNullValue*/ true);
                    state = ODataParameterReaderState.Value;
                    break;

                case EdmTypeKind.Complex:
                    parameterValue = this.jsonPropertyAndValueDeserializer.ReadNonEntityValue(
                        parameterTypeReference,
                        /*duplicatePropertyNamesChecker*/ null,
                        /*collectionValidator*/ null,
                        /*validateNullValue*/ true);
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
                    if (this.jsonPropertyAndValueDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                    {
                        parameterValue = this.jsonPropertyAndValueDeserializer.JsonReader.ReadPrimitiveValue();
                        if (parameterValue != null)
                        {
                            throw new ODataException(o.Strings.ODataJsonParameterReader_NullCollectionExpected(JsonNodeType.PrimitiveValue, parameterValue));
                        }

                        state = ODataParameterReaderState.Value;
                    }
                    else if (((IEdmCollectionType)parameterTypeReference.Definition).ElementType.TypeKind() == EdmTypeKind.Entity)
                    {
#if SUPPORT_ENTITY_PARAMETER
                        state = ODataParameterReaderState.Feed;
#else
                        throw new ODataException(o.Strings.ODataJsonParameterReader_UnsupportedParameterTypeKind(parameterName, "Entity Collection"));
#endif
                    }
                    else
                    {
                        state = ODataParameterReaderState.Collection;
                    }

                    break;
                default:
                    throw new ODataException(o.Strings.ODataJsonParameterReader_UnsupportedParameterTypeKind(parameterName, parameterTypeReference.TypeKind()));
            }

            this.EnterScope(state, parameterName, parameterValue);
        }
    }
}
