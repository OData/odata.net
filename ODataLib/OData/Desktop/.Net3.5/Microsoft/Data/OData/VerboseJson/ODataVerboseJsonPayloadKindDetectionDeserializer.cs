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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON deserializer for detecting the payload kind of a JSON payload.
    /// </summary>
    internal sealed class ODataVerboseJsonPayloadKindDetectionDeserializer : ODataVerboseJsonPropertyAndValueDeserializer
    {
        /// <summary>The set of detected payload kinds.</summary>
        private readonly HashSet<ODataPayloadKind> detectedPayloadKinds = new HashSet<ODataPayloadKind>(EqualityComparer<ODataPayloadKind>.Default);
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The Verbose JSON input context to read from.</param>
        internal ODataVerboseJsonPayloadKindDetectionDeserializer(ODataVerboseJsonInputContext verboseJsonInputContext)
            : base(verboseJsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Detects the payload kind(s).
        /// </summary>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind()
        {
            DebugUtils.CheckNoExternalCallers();
            this.detectedPayloadKinds.Clear();

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            this.JsonReader.DisableInStreamErrorDetection = true;

            try
            {
                // Read the payload start including the 'd' wrapper in responses
                this.ReadPayloadStart(/*isReadingNestedPayload*/ false);

                JsonNodeType nodeType = this.JsonReader.NodeType;
                if (nodeType == JsonNodeType.StartObject)
                {
                    // Read the start of the object container
                    this.JsonReader.ReadStartObject();

                    // read all the properties in the object and detect well-known ones
                    int propertyCount = 0;
                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        string propertyName = this.JsonReader.ReadPropertyName();
                        propertyCount++;

                        // If we find a __metadata property we know that the payload is not a top-level property anymore.
                        // Check the type name to rule out collection and primitive types; otherwise report an entry.
                        // NOTE: once we find __metadata we stop scanning to avoid having to read the whole object
                        //       for every payload.
                        if (string.CompareOrdinal(JsonConstants.ODataMetadataName, propertyName) == 0)
                        {
                            this.ProcessMetadataPropertyValue();
                            break;
                        }

                        if (propertyCount == 1)
                        {
                            // If this is the first property, initialize the set of possible payload kinds with all the kinds
                            // the property could be if it remained the only property and independent of the property name.
                            //
                            // A single, top-level JSON property can always be a property payload, an entry payload or a parameter payload.
                            // If the property value is a primitive value, report a top-level primitive property, an entry or a parameter
                            // If the property value is an array, report a top-level collection property, an entry or a parameter
                            // If the property value is an object, report a complex property, an entry or a parameter
                            this.AddPayloadKinds(ODataPayloadKind.Property, ODataPayloadKind.Entry, ODataPayloadKind.Parameter);

                            // Then deal with the well-known properties that only have special meaning if they are the only
                            // property in the payload
                            ODataError error;
                            if (string.CompareOrdinal(JsonConstants.ODataUriName, propertyName) == 0 &&
                                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                            {
                                // First property with name 'uri' and a primitive value; if this remains the only
                                // property also treat this payload as entity reference link
                                this.AddPayloadKinds(ODataPayloadKind.EntityReferenceLink);
                            }
                            else if (string.CompareOrdinal(JsonConstants.ODataErrorName, propertyName) == 0 &&
                                this.JsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error))
                            {
                                // First property with name 'error' and matching error payload; if this remains the
                                // only property also treat this payload as top-level error
                                this.AddPayloadKinds(ODataPayloadKind.Error);
                            }
                        }
                        else if (propertyCount == 2)
                        {
                            // If this is the second property, adjust the previously assumed payload kinds since some of them
                            // don't apply anymore.
                            //
                            // Payload cannot be a property payload if there are more than one properties in the top-level object.
                            // Entity reference links require the 'uri' property to be the only one
                            // Top-level errors require the 'error' property to be the only one
                            this.RemovePayloadKinds(ODataPayloadKind.Property, ODataPayloadKind.EntityReferenceLink, ODataPayloadKind.Error);

                            // If we have at least 2 properties, the payload can always be an entry or a parameter.
                            // These payload kinds should have been added already.
                            Debug.Assert(
                                this.detectedPayloadKinds.Contains(ODataPayloadKind.Entry),
                                "Entry payload kind should have already been added.");
                            Debug.Assert(
                                this.ReadingResponse || this.detectedPayloadKinds.Contains(ODataPayloadKind.Parameter),
                                "Parameter payload kind should have already been added for requests.");
                        }

                        // After establishing the possible payload kinds without looking at the property name, 
                        // handle the well-known properties that can appear anywhere in the object (not only as
                        // the first/only property).
                        if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0 &&
                            this.JsonReader.NodeType == JsonNodeType.StartArray)
                        {
                            // Property with name 'results' and array value: it is either a feed, a set of entity reference links,
                            // a top-level collection or a collection property with name 'results' that is missing the results wrapper object.
                            this.DetectStartArrayPayloadKind(/*isTopLevel*/ false);
                        }
                        else if (this.ReadingResponse &&
                            string.CompareOrdinal(JsonConstants.ODataServiceDocumentEntitySetsName, propertyName) == 0 &&
                            this.JsonReader.NodeType == JsonNodeType.StartArray)
                        {
                            // Property with name 'EntitySets' and array value: the payload might also be a service document.
                            this.ProcessEntitySetsArray();
                        }

                        this.JsonReader.SkipValue();
                    }

                    // If we did not find any properties, the payload is an entry or an empty parameter
                    if (propertyCount == 0)
                    {
                        this.AddPayloadKinds(ODataPayloadKind.Entry, ODataPayloadKind.Parameter);
                    }
                }
                else if (nodeType == JsonNodeType.StartArray)
                {
                    // A top-level array is either a V1 feed, a top-level collection or an array of entity reference links
                    this.DetectStartArrayPayloadKind(/*isTopLevel*/ true);
                }

                return this.detectedPayloadKinds;
            }
            catch (ODataException)
            {
                // If we are not able to read the payload in the expected JSON format/structure
                // return no detected payload kind below.
                return Enumerable.Empty<ODataPayloadKind>();
            }
            finally
            {
                this.JsonReader.DisableInStreamErrorDetection = false;
            }
        }

        /// <summary>
        /// Detects whether a JSON payload where the current node is a start array node represents
        /// a feed, a set of entity reference links, a collection or a combination of them.
        /// </summary>
        /// <param name="isTopLevel">true if the array is a top-level array; if it is a property value false.</param>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  JsonNodeType.StartArray         The StartArray node of the feed or entity reference links array (if at the top-level)
        /// Post-Condition: JsonNodeType.StartArray         The StartArray node of the feed or entity reference links array (if at the top-level)
        /// </remarks>
        private void DetectStartArrayPayloadKind(bool isTopLevel)
        {
            this.AssertJsonCondition(JsonNodeType.StartArray);
            this.JsonReader.AssertNotBuffering();

            // an array as property value can always be a collection property (with the missing results wrapper).
            if (!isTopLevel)
            {
                this.AddPayloadKinds(ODataPayloadKind.Property);
            }

            this.JsonReader.StartBuffering();
            try
            {
                this.JsonReader.ReadStartArray();

                // Treat an empty array as either a feed or entity reference links or a collection
                switch (this.JsonReader.NodeType)
                {
                    case JsonNodeType.StartObject:
                        // Further processing needed
                        break;

                    case JsonNodeType.EndArray:
                        this.AddPayloadKinds(ODataPayloadKind.Feed, ODataPayloadKind.Collection, ODataPayloadKind.EntityReferenceLinks);
                        return;

                    case JsonNodeType.PrimitiveValue:
                        // A primitive value in the array means this is a collection
                        this.AddPayloadKinds(ODataPayloadKind.Collection);
                        return;

                    default:
                        // Any node type other than primitive, end array and start object are invalid payloads
                        return;
                }

                // Check whether the first object in the array has a single 'uri' property with a primitive value; 
                // if so, report 'entity reference links' as well; otherwise only 'feed' and 'collection'
                this.JsonReader.ReadStartObject();

                bool singleUriFound = false;
                int propertyCount = 0;

                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = this.JsonReader.ReadPropertyName();
                    propertyCount++;

                    if (propertyCount > 1)
                    {
                        break;
                    }

                    if (string.CompareOrdinal(JsonConstants.ODataUriName, propertyName) == 0 &&
                        this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                    {
                        singleUriFound = true;
                    }

                    this.JsonReader.SkipValue();
                }

                this.AddPayloadKinds(ODataPayloadKind.Feed, ODataPayloadKind.Collection);

                if (singleUriFound && propertyCount == 1)
                {
                    this.AddPayloadKinds(ODataPayloadKind.EntityReferenceLinks);
                }
            }
            finally
            {
                this.JsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Read the value of the __metadata property and compute the payload kind based on the type name.
        /// </summary>
        /// <remarks>This method checks whether it can determine the type kind from the type name; if we
        /// find a primitive or collection type we set the result to empty since such payloads are not supported.
        /// Otherwise we'll treat the payload as an entry since top-level complex values are not supported either.
        /// Pre-Condition:  Any                             The first node of the __metadata property value
        /// Post-Condition: Property or EndObject           This method reads the entire value of the __metadata object and positions
        ///                                                 the reader on the next property or on the EndObject node if this is the last property.
        /// </remarks>
        private void ProcessMetadataPropertyValue()
        {
            // Clear all previously detected payload kinds since when we find a top-level __metadata
            // we base our decision solely on it.
            this.detectedPayloadKinds.Clear();

            string typeName = this.ReadTypeNameFromMetadataPropertyValue();

            // NOTE: we intentionally do not use the model passed to the message reader because we want to
            //       keep the paylaod detection code independent of any model (at least for now).
            EdmTypeKind typeKind = EdmTypeKind.None;
            if (typeName != null)
            {
                MetadataUtils.ResolveTypeNameForRead(
                    EdmCoreModel.Instance,
                    /*expectedType*/ null,
                    typeName,
                    this.MessageReaderSettings.ReaderBehavior,
                    this.Version,
                    out typeKind);
            }

            // A valid top-level object with a __metadata property must not specify a primitive or collection type
            if (typeKind == EdmTypeKind.Primitive || typeKind == EdmTypeKind.Collection)
            {
                return;
            }

            Debug.Assert(typeKind == EdmTypeKind.None, "In the core model we should only be able to detect primitive and collection types.");

            // We don't support top-level complex value so the payload has to be an entry
            this.detectedPayloadKinds.Add(ODataPayloadKind.Entry);
        }

        /// <summary>
        /// Process the array value of an 'EntitySets' property to determine whether it should be treated as service document payload.
        /// </summary>
        /// <remarks>
        /// This method does not move the Json reader.
        /// Pre-Condition:  StartArray                      The start of the array value of the 'EntitySets' property
        /// Post-Condition: StartArray                      The start of the array value of the 'EntitySets' property
        /// </remarks>
        private void ProcessEntitySetsArray()
        {
            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.StartArray);

            this.JsonReader.StartBuffering();
            try
            {
                this.JsonReader.ReadStartArray();

                if (this.JsonReader.NodeType == JsonNodeType.EndArray ||
                    this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                {
                    this.AddPayloadKinds(ODataPayloadKind.ServiceDocument);
                }
            }
            finally
            {
                this.JsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Adds the set of specified payload kinds to the detected payload kinds (if the specified 
        /// payload kinds are valid for the current request/response).
        /// </summary>
        /// <param name="payloadKinds">The payload kinds to add.</param>
        private void AddPayloadKinds(params ODataPayloadKind[] payloadKinds)
        {
            Debug.Assert(payloadKinds != null, "payloadKinds != null");

            this.AddOrRemovePayloadKinds(this.detectedPayloadKinds.Add, payloadKinds);
        }

        /// <summary>
        /// Removes the set of specified payload kinds from the detected payload kinds (if the specified 
        /// payload kinds are valid for the current request/response).
        /// </summary>
        /// <param name="payloadKinds">The payload kinds to remove.</param>
        private void RemovePayloadKinds(params ODataPayloadKind[] payloadKinds)
        {
            Debug.Assert(payloadKinds != null, "payloadKinds != null");

            this.AddOrRemovePayloadKinds(this.detectedPayloadKinds.Remove, payloadKinds);
        }

        /// <summary>
        /// Adds or removes the set of specified payload kinds to/from the detected payload kinds (if the specified 
        /// payload kinds are valid for the current request/response).
        /// </summary>
        /// <param name="addOrRemoveAction">The function that implements the 'Add' or 'Remove' action.</param>
        /// <param name="payloadKinds">The payload kinds to add/remove.</param>
        private void AddOrRemovePayloadKinds(Func<ODataPayloadKind, bool> addOrRemoveAction, params ODataPayloadKind[] payloadKinds)
        {
            Debug.Assert(addOrRemoveAction != null, "addOrRemoveAction != null");
            Debug.Assert(payloadKinds != null, "payloadKinds != null");

            for (int i = 0; i < payloadKinds.Length; ++i)
            {
                ODataPayloadKind payloadKind = payloadKinds[i];
                if (ODataUtilsInternal.IsPayloadKindSupported(payloadKind, !this.ReadingResponse))
                {
                    addOrRemoveAction(payloadKind);
                }
            }
        }
    }
}
