//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    /// OData JSON deserializer for collections.
    /// </summary>
    internal sealed class ODataVerboseJsonCollectionDeserializer : ODataVerboseJsonPropertyAndValueDeserializer
    {
        /// <summary>Cached duplicate property names checker to use if the items are complex values.</summary>
        private readonly DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataVerboseJsonCollectionDeserializer(ODataVerboseJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            this.duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
        }

        /// <summary>
        /// Reads the start of a collection; this includes collection-level properties (e.g., the 'results' property) if the version permits it.
        /// </summary>
        /// <param name="isResultsWrapperExpected">true if the results wrapper should be in the payload being read; false otherwise.</param>
        /// <returns>An <see cref="ODataCollectionStart"/> representing the collection-level information. Currently this is only the name of the collection in ATOM.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:    for a V1 collection
        ///                 JsonNodeType.StartObject:   for a >=V2 collection
        /// Post-Condition: JsonNodeType.StartArray:    the start of the array of the collection items
        /// </remarks>
        internal ODataCollectionStart ReadCollectionStart(bool isResultsWrapperExpected)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                isResultsWrapperExpected && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                !isResultsWrapperExpected && this.JsonReader.NodeType == JsonNodeType.StartArray,
                "Pre-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray");
            this.JsonReader.AssertNotBuffering();

            if (isResultsWrapperExpected)
            {
                this.JsonReader.ReadStartObject();

                // read all the properties until we get to the 'results' property
                bool resultsPropertyFound = false;
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    // read the property name and move the reader onto the start of the property value
                    string propertyName = this.JsonReader.ReadPropertyName();

                    if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                    {
                        resultsPropertyFound = true;
                        break;
                    }

                    // skip over all properties we don't recognize
                    this.JsonReader.SkipValue();
                }

                if (!resultsPropertyFound)
                {
                    // we did not find the expected 'results' property; fail
                    throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_MissingResultsPropertyForCollection);
                }
            }

            // at this point the reader is positioned on the start array node for the collection contents
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.StartArray, "Post-Condition: expected JsonNodeType.StartArray");

            // NOTE: JSON does not have the name of the collection in the payload, so we report a 'null' name
            return new ODataCollectionStart
            {
                Name = null
            };
        }

        /// <summary>
        /// Reads an item in the collection.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type of the item to read.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <returns>The value of the collection item that was read; this can be an ODataComplexValue, a primitive value or 'null'.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the item in the collection
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no more items in the collection
        /// </remarks>
        internal object ReadCollectionItem(IEdmTypeReference expectedItemTypeReference, CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                expectedItemTypeReference == null ||
                expectedItemTypeReference.IsODataPrimitiveTypeKind() ||
                expectedItemTypeReference.IsODataComplexTypeKind(),
                "If an expected type is specified, it must be a primitive or complex type.");
            this.JsonReader.AssertNotBuffering();

            object item = this.ReadNonEntityValue(
                expectedItemTypeReference, 
                this.duplicatePropertyNamesChecker, 
                collectionValidator,
                /*validateNullValue*/ true, 
                /*propertyName*/ null);

            this.JsonReader.AssertNotBuffering();

            return item;
        }

        /// <summary>
        /// Reads the end of a collection; this includes collection-level properties if the version permits it.
        /// </summary>
        /// <param name="isResultsWrapperExpected">true if the results wrapper should be in the payload being read; false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  EndArray node:      End of the collection content array
        /// Post-Condition: EndOfInput:         V1 collection
        ///                 EndObject           V1 collection in response
        ///                 EndObject           wrapped collection with no extra properties after the 'results' property
        ///                 Property            wrapped collection with extra properties after the 'results' property
        /// </remarks>
        internal void ReadCollectionEnd(bool isResultsWrapperExpected)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-condition: JsonNodeType.EndArray");
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.ReadEndArray();

            if (!isResultsWrapperExpected)
            {
                return;
            }

            // skip over all properties after the 'results' property
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                {
                    // two 'results' properties are not allowed; we found the first one when parsing the start
                    // of the collection (or would have failed then)
                    throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_MultipleResultsPropertiesForCollection);
                }

                this.JsonReader.SkipValue();
            }

            // read the end-object node of the value containing the 'results' property
            this.JsonReader.ReadEndObject();
        }
    }
}
