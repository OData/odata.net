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
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for entries and feeds.
    /// </summary>
    internal sealed class ODataJsonEntryAndFeedDeserializer : ODataJsonPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataJsonEntryAndFeedDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Reads the start of a feed; this includes feed-level properties if the version permits it.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="isResultsWrapperExpected">A flag indicating whether we expect the results wrapper for feeds to be present.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:    for a V1 feed
        ///                 JsonNodeType.StartObject:   for a >=V2 feed
        /// Post-Condition: JsonNodeType.StartArray:    the start of the array of the feed elements
        /// </remarks>
        internal void ReadFeedStart(ODataFeed feed, bool isResultsWrapperExpected)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(
                isResultsWrapperExpected && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                !isResultsWrapperExpected && this.JsonReader.NodeType == JsonNodeType.StartArray,
                "Pre-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray");
            this.JsonReader.AssertNotBuffering();

            if (isResultsWrapperExpected)
            {
                this.JsonReader.ReadNext();  // skip over the StartObject node

                // read all the properties until we get to the 'results' property
                string propertyName = null;
                while (true)
                {
                    if (this.JsonReader.NodeType != JsonNodeType.Property)
                    {
                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "Only properties and end-of-object should be returned here.");

                        // we only expect properties until we find the 'results' property
                        throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound);
                    }

                    // read the property name and move the reader onto the start of the property value
                    propertyName = this.JsonReader.ReadPropertyName();

                    if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                    {
                        break;
                    }

                    this.ReadFeedProperty(feed, propertyName);
                }

                // At this point the reader is guaranteed to be positioned over the value of the 'results' property
            }

            // at this point the reader is positioned on the start array node for the feed contents;
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.StartArray, "Post-Condition: expected JsonNodeType.StartArray");
        }

        /// <summary>
        /// Reads the end of a feed; this includes feed-level properties if the version permits them.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="readResultsWrapper">A flag indicating whether we expect the results wrapper for feeds to be present.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndArray
        /// Post-Condition: JsonNodeType.EndArray   if the feed is not wrapped
        ///                 JsonNodeType.EndObject  if the feed is wrapped
        /// </remarks>
        internal void ReadFeedEnd(ODataFeed feed, bool readResultsWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-Condition: JsonNodeType.EndArray");
            this.JsonReader.AssertNotBuffering();

            if (readResultsWrapper)
            {
                this.JsonReader.ReadEndArray();

                // read all the feed-level properties until we reach the end object node
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = this.JsonReader.ReadPropertyName();
                    this.ReadFeedProperty(feed, propertyName);
                }

                Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "A wrapped feed must end with an EndObject node.");
            }

            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the content of an entry until a navigation link is detected.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationProperty">If a navigation link was found this parameter will hold the navigation property for that link, otherwise it's null.</param>
        /// <returns>A <see cref="ODataNavigationLink"/> instance representing the navigation link detected while reading the entry contents; null if no navigation link was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        ///                 JsonNodeType.StartObject            The first node of the navigation link property value to read next 
        ///                                                     (deferred link or entry inside expanded link or wrapped feed inside expanded link)
        ///                 JsonNodeType.StartArray             V1 feed inside of expanded link
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry
        /// </remarks>
        internal ODataNavigationLink ReadEntryContent(
            IODataJsonReaderEntryState entryState,
            out IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.EntityType != null && this.Model.IsUserModel(), "A non-null entity type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataNavigationLink navigationLink = null;
            navigationProperty = null;

            ODataEntry entry = entryState.Entry;
            IEdmEntityType entityType = entryState.EntityType;

            // figure out whether we have more properties for this entry
            // read all the properties until we hit a link
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                // check the well-known property names first
                if (string.CompareOrdinal(JsonConstants.ODataMetadataName, propertyName) == 0)
                {
                    // __metadata property
                    if (entryState.MetadataPropertyFound)
                    {
                        throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue);
                    }

                    entryState.MetadataPropertyFound = true;

                    this.ReadEntryMetadata(entry);

                    // once we read all the metadata let's validate that the metadata information is consistent
                    // By default validate the media resource.
                    // In WCF DS Server mode, do not validate in JSON (here).
                    // In WCF DS Client mode, do not validate.
                    bool validateMediaResource = this.MessageReaderSettings.ReaderBehavior.BehaviorKind == ODataBehaviorKind.Default;
                    ValidationUtils.ValidateEntryMetadata(entry, entityType, validateMediaResource);

                    // check the names of the association links
                    if (entry.AssociationLinks != null)
                    {
                        foreach (ODataAssociationLink associationLink in entry.AssociationLinks)
                        {
                            Debug.Assert(associationLink != null, "associationLink != null");
                            ValidationUtils.ValidateAssociationLink(associationLink);
                            ValidationUtils.ValidateNavigationPropertyDefined(associationLink.Name, entityType);
                            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(associationLink);
                        }
                    }
                }
                else
                {
                    IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(propertyName, entityType);
                    Debug.Assert(edmProperty != null || entityType.IsOpenType(), "If we didn't find a property it must be an open property.");

                    if (edmProperty != null)
                    {
                        navigationProperty = edmProperty as IEdmNavigationProperty;
                        if (navigationProperty != null)
                        {
                            // this can be a deferred link or an expanded link (with entry or feed payload)
                            bool isCollection = navigationProperty.To.Multiplicity == EdmAssociationMultiplicity.Many;
                            navigationLink = new ODataNavigationLink
                            {
                                Name = propertyName, 
                                IsCollection = isCollection
                            };

                            this.ValidateNavigationLinkPropertyValue(isCollection);
                            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLink);
                        }
                        else
                        {
                            this.ReadEntryProperty(entryState, edmProperty);
                        }
                    }
                    else
                    {
                        this.ReadOpenProperty(entryState, propertyName);
                    }
                }

                if (navigationLink != null)
                {
                    // we found a navigation link
                    // stop parsing the entry content and return to the caller
                    break;
                }

                Debug.Assert(
                    this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "After reading a property the reader should be positioned on another property or have hit the end of the object.");
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                navigationLink != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                navigationLink != null && this.Version == ODataVersion.V1 && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                navigationLink != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return navigationLink;
        }

        /// <summary>
        /// Reads the Url of a non-expanded link and moves the reader forward to the position after the link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to set the Url on.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:    the start of the object representing the deferred link
        /// Post-Condition: JsonNodeType.Property:       the next property after the deferred link
        ///                 JsonNodeType.EndObject       the end of the owning entry if the deferred link is the last property
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "deferredPropertyName", Justification = "Used in debug assertions.")]
        internal void ReadDeferredNavigationLinkEnd(ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "!string.IsNullOrEmpty(navigationLink.Name)");
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.StartObject, "Pre-Condition: expected JsonNodeType.StartObject");
            this.JsonReader.AssertNotBuffering();

            // read the start object node of the object with the __deferred property
            this.JsonReader.ReadStartObject();

            // read the __deferred property; the presence of the property is guaranteed by the link detection code
            // move the reader to the start of the property value of the __deferred property
            string deferredPropertyName = this.JsonReader.ReadPropertyName();
            Debug.Assert(deferredPropertyName == JsonConstants.ODataDeferredName, "Expected __deferred property here.");

            // start of the deferred link object; position the reader on the node following the start object node
            this.JsonReader.ReadStartObject();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // read the 'uri' property
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataNavigationLinkUriName, propertyName) == 0)
                {
                    if (navigationLink.Url != null)
                    {
                        throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleUriPropertiesInDeferredLink);
                    }

                    // read the value of the 'uri' property
                    string deferredLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataNavigationLinkUriName);
                    if (deferredLinkString == null)
                    {
                        throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_DeferredLinkUriCannotBeNull);
                    }

                    navigationLink.Url = this.ProcessUriFromPayload(deferredLinkString);
                }
                else
                {
                    // Skip unrecognized properties
                    this.JsonReader.SkipValue();
                }
            }

            if (navigationLink.Url == null)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_DeferredLinkMissingUri);
            }

            // end of the deferred link object
            this.JsonReader.ReadEndObject();

            // end of the deferred object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.EndObject || this.JsonReader.NodeType == JsonNodeType.Property,
                "Post-Condition: expected JsonNodeType.EndObject or JsonNodeType.Property");
        }

        /// <summary>
        /// Analyzes the current navigation property node to determine whether it represents an expanded or deferred link.
        /// </summary>
        /// <returns>true if the current navigation property represents an expanded link; false for a deferred link.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  The first node of the property value
        /// Post-Condition: The first node of the property value
        /// </remarks>
        internal bool IsExpandedLink()
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();

            // The reader is already positioned on the first node of the value
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.PrimitiveValue)
            {
                if (this.JsonReader.Value != null)
                {
                    // a navigation property value cannot be a primitive value (unless it's null)
                    throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
                }

                // the value of the property is an expanded null entry; this is an expanded link
                return true;
            }

            if (nodeType == JsonNodeType.StartArray)
            {
                // the value of the property is a V1 feed; this is an expanded link
                return true;
            }

            Debug.Assert(nodeType == JsonNodeType.StartObject, "Only PrimitiveValue, StartArray and StartObject nodes are expected after as the first node of a property value.");

            // At this point we can either have a non-expanded link, an expanded link with entry payload or an expanded link with a (>=V2) feed payload.
            // We read another node but do so in buffering mode so we can continue to read where we left off after figuring out the entry kind
            this.JsonReader.StartBuffering();

            try
            {
                this.JsonReader.ReadStartObject();

                nodeType = this.JsonReader.NodeType;
                if (nodeType == JsonNodeType.EndObject)
                {
                    // expanded link with empty entry payload
                    return true;
                }
                else
                {
                    Debug.Assert(nodeType == JsonNodeType.Property, "Only EndObject and Property nodes are expected after a StartObject node.");
                    string innerPropertyName = this.JsonReader.GetPropertyName();

                    // we found a non-expanded link
                    return string.CompareOrdinal(JsonConstants.ODataDeferredName, innerPropertyName) != 0;
                }
            }
            finally
            {
                this.JsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Read a feed-level property (e.g., __count, __next, etc.) and check its version compliance. 
        /// This method fails on properties that are not recognized as feed-level properties.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="propertyName">The name of the property being read.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is on the first node of the feed-level property's value.
        /// Post-Condition: JsonNodeType.Property:          the next feed property to read or
        ///                 JsonNodeType.EndObject:         the end of the results wrapper
        /// </remarks>
        private void ReadFeedProperty(ODataFeed feed, string propertyName)
        {
            Debug.Assert(feed != null, "feed != null");
            this.JsonReader.AssertNotBuffering();

            ODataJsonReaderUtils.FeedPropertyKind propertyKind = ODataJsonReaderUtils.DetermineFeedPropertyKind(propertyName);

            switch (propertyKind)
            {
                case ODataJsonReaderUtils.FeedPropertyKind.InlineCount:
                    {
                        // "__count": "number"
                        string inlineCountString = this.JsonReader.ReadStringValue(JsonConstants.ODataCountName);
                        ODataJsonReaderUtils.ValidateFeedProperty(inlineCountString, JsonConstants.ODataCountName);
                        long inlineCount = (long)ODataJsonReaderUtils.ConvertValue(inlineCountString, EdmCoreModel.Instance.GetInt64(false), /*usesV1ProviderBehavior*/ false);
                        feed.Count = inlineCount;
                    }

                    break;
                case ODataJsonReaderUtils.FeedPropertyKind.NextPageLink:
                    {
                        // "__next": "next-link"
                        string nextLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataNextLinkName);
                        ODataJsonReaderUtils.ValidateFeedProperty(nextLinkString, JsonConstants.ODataNextLinkName);
                        feed.NextPageLink = this.ProcessUriFromPayload(nextLinkString);
                    }

                    break;
                case ODataJsonReaderUtils.FeedPropertyKind.Unsupported:
                    // Ignore any properties we don't recognize
                    this.JsonReader.SkipValue();
                    break;
                case ODataJsonReaderUtils.FeedPropertyKind.Results:
                    // If we find results property here, it means we didn't expect it and we've already seen one.
                    throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound);
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataJsonEntryAndFeedDeserializer_ReadFeedProperty));
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an entry-level property and check its version compliance. 
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="edmProperty">The EDM property of the property being read, or null if the property is an open property.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        private void ReadEntryProperty(IODataJsonReaderEntryState entryState, IEdmProperty edmProperty)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");
            this.JsonReader.AssertNotBuffering();

            object propertyValue = null;
            IEdmTypeReference propertyTypeReference = edmProperty.Type;
            EdmTypeKind propertyTypeKind = propertyTypeReference.TypeKind();
            switch (propertyTypeKind)
            {
                case EdmTypeKind.Primitive:
                    propertyValue = propertyTypeReference.IsStream() ? this.ReadStreamReferenceValue() : this.ReadPrimitiveValue(propertyTypeReference.AsPrimitiveOrNull());
                    break;
                case EdmTypeKind.Complex:
                    propertyValue = this.ReadComplexValue(propertyTypeReference.AsComplexOrNull(), null);
                    break;
                default:
                    if (propertyTypeReference.IsODataMultiValueTypeKind())
                    {
                        propertyValue = this.ReadMultiValue(propertyTypeReference.AsMultiValueOrNull());
                    }
                    else
                    {
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataJsonEntryAndFeedDeserializer_ReadEntryProperty));
                    }

                    break;
            }

            ODataProperty property = new ODataProperty { Name = edmProperty.Name, Value = propertyValue };
            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            ReaderUtils.AddPropertyToPropertiesList(entryState.Entry.Properties, property);

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an open property.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        private void ReadOpenProperty(IODataJsonReaderEntryState entryState, string propertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            object propertyValue = this.ReadNonEntityValue(null, null);

            ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue);
            
            ODataProperty property = new ODataProperty { Name = propertyName, Value = propertyValue };
            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            ReaderUtils.AddPropertyToPropertiesList(entryState.Entry.Properties, property);

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Reads a named stream (stream reference value).
        /// </summary>
        /// <returns>The value of the stream reference.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: almost anything - the node after the stream reference (after the EndObject)
        /// </remarks>
        private ODataStreamReferenceValue ReadStreamReferenceValue()
        {
            this.JsonReader.AssertNotBuffering();

            // Read over the start object of the stream reference
            this.JsonReader.ReadStartObject();

            // Skip all the properties except for the __mediaresource one
            ODataStreamReferenceValue streamReference = null;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataMetadataMediaResourceName, propertyName) == 0)
                {
                    if (streamReference != null)
                    {
                        // we found another __mediaresource property; this is not allowed
                        throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(JsonConstants.ODataMetadataMediaResourceName));
                    }

                    streamReference = this.ReadStreamReferenceMetadata();
                }
                else
                {
                    // ignore all other properties
                    this.JsonReader.SkipValue();
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "Only Property or EndObject can appear at the object scope.");
            this.JsonReader.ReadEndObject();

            if (streamReference == null)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }

            this.JsonReader.AssertNotBuffering();

            return streamReference;
        }

        /// <summary>
        /// Reads the __metadata property of an object representing an entry.
        /// </summary>
        /// <param name="entry">The <see cref="ODataEntry"/> the metadata is read for.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the '__metadata' property's value (we will throw if this is not a start object node)
        /// Post-Condition: JsonNodeType.Property:      the next property of the entry
        ///                 JsonNodeType.EndObject:     the end-object node of the entry
        /// </remarks>
        private void ReadEntryMetadata(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");
            this.JsonReader.AssertNotBuffering();

            // make sure the '__metadata' property value is an object
            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MetadataPropertyMustHaveObjectValue(this.JsonReader.NodeType));
            }

            // read over the start-object node
            this.JsonReader.ReadStartObject();

            ODataStreamReferenceValue mediaResource = null;
            ODataJsonReaderUtils.MetadataPropertyBitMask propertiesFoundBitField = 0;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                switch (propertyName)
                {
                    case JsonConstants.ODataMetadataUriName:    // 'uri'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.Uri,
                            JsonConstants.ODataMetadataUriName);
                        string editLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataUriName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(editLinkString, JsonConstants.ODataMetadataUriName);
                        entry.EditLink = this.ProcessUriFromPayload(editLinkString);
                        break;

                    case JsonConstants.ODataEntryIdName:   // 'id'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.Id,
                            JsonConstants.ODataEntryIdName);
                        string idString = this.JsonReader.ReadStringValue(JsonConstants.ODataEntryIdName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(idString, JsonConstants.ODataEntryIdName);
                        entry.Id = idString;
                        break;

                    case JsonConstants.ODataMetadataETagName:   // 'etag'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.ETag,
                            JsonConstants.ODataMetadataETagName);
                        string etagString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataETagName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(etagString, JsonConstants.ODataMetadataETagName);
                        entry.ETag = etagString;
                        break;

                    case JsonConstants.ODataMetadataTypeName:   // 'type'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.Type,
                            JsonConstants.ODataMetadataTypeName);

                        // Type is validated outside of this function.
                        // We also don't want to store the value anywhere, we've already read it once.
                        this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataTypeName);
                        break;

                    case JsonConstants.ODataMetadataMediaUriName:   // 'media_src'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.MediaUri,
                            JsonConstants.ODataMetadataMediaUriName);
                        ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                        string mediaReadLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaUriName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(mediaReadLinkString, JsonConstants.ODataMetadataMediaUriName);
                        mediaResource.ReadLink = this.ProcessUriFromPayload(mediaReadLinkString);
                        break;

                    case JsonConstants.ODataMetadataEditMediaName:  // 'edit_media'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.EditMedia,
                            JsonConstants.ODataMetadataEditMediaName);
                        ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                        string mediaEditLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataEditMediaName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(mediaEditLinkString, JsonConstants.ODataMetadataEditMediaName);
                        mediaResource.EditLink = this.ProcessUriFromPayload(mediaEditLinkString);
                        break;

                    case JsonConstants.ODataMetadataContentTypeName:  // 'content_type'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.ContentType,
                            JsonConstants.ODataMetadataContentTypeName);
                        ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                        string mediaContentType = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataContentTypeName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(mediaContentType, JsonConstants.ODataMetadataContentTypeName);
                        mediaResource.ContentType = mediaContentType;
                        break;

                    case JsonConstants.ODataMetadataMediaETagName:  // 'media_etag'
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.MediaETag,
                            JsonConstants.ODataMetadataMediaETagName);
                        ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                        string mediaETag = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaETagName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(mediaETag, JsonConstants.ODataMetadataMediaETagName);
                        mediaResource.ETag = mediaETag;
                        break;

                    case JsonConstants.ODataMetadataPropertiesName:
                        ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.MetadataPropertyBitMask.Properties,
                            JsonConstants.ODataMetadataPropertiesName);
                        this.ReadPropertiesMetadata(entry);
                        break;

                    default:
                        // ignore all extra properties in __metadata that we don't understand
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            entry.MediaResource = mediaResource;

            // read over the end-object node of the metadata object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read the property metadata for the properties of an entry being read.
        /// </summary>
        /// <param name="entry">The <see cref="ODataEntry"/> the properties metadata is read for.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'properties' property's value (we will throw if this is not a start object node)
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadPropertiesMetadata(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");
            this.JsonReader.AssertNotBuffering();

            // make sure the 'properties' property value is an object
            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue(JsonConstants.ODataMetadataPropertiesName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for 'properties'
            this.JsonReader.ReadStartObject();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                // read the start-object node of the metadata for the current propertyName
                this.JsonReader.ReadStartObject();

                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string innerPropertyName = this.JsonReader.ReadPropertyName();

                    // ignore all properties we don't understand
                    if (string.CompareOrdinal(innerPropertyName, JsonConstants.ODataMetadataPropertiesAssociationUriName) == 0)
                    {
                        string associationUrlString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataPropertiesAssociationUriName);
                        ODataJsonReaderUtils.ValidateMetadataStringProperty(associationUrlString, JsonConstants.ODataMetadataPropertiesAssociationUriName);
                        ODataAssociationLink associationLink = new ODataAssociationLink
                        {
                            Name = propertyName,
                            Url = this.ProcessUriFromPayload(associationUrlString)
                        };

                        ReaderUtils.AddAssociationLinkToEntry(entry, associationLink);
                    }
                    else
                    {
                        // skip over all unknown properties and read the next property or 
                        // the end of the metadata for the current propertyName
                        this.JsonReader.SkipValue();
                    }
                }

                // read the end-object node of the metadata for the current propertyName
                this.JsonReader.ReadEndObject();
            }

            // read over the end-object node of the metadata object for 'properties'
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Reads the stream reference metadata from the value of the __mediaresource property.
        /// </summary>
        /// <returns>The value of the stream reference with the metadata properties filled.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: Either a property node or an EndObject node.
        /// </remarks>
        private ODataStreamReferenceValue ReadStreamReferenceMetadata()
        {
            this.JsonReader.AssertNotBuffering();

            // read the start of the __mediaresource object
            this.JsonReader.ReadStartObject();

            ODataStreamReferenceValue streamReference = new ODataStreamReferenceValue();
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                switch (propertyName)
                {
                    case JsonConstants.ODataMetadataEditMediaName:      // "edit_media": "url"
                        if (streamReference.EditLink != null)
                        {
                            // we found another edit_media property; this is not allowed
                            throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(JsonConstants.ODataMetadataEditMediaName));
                        }

                        string editLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataEditMediaName);
                        ODataJsonReaderUtils.ValidateMediaResourceStringProperty(editLinkString, JsonConstants.ODataMetadataEditMediaName);
                        streamReference.EditLink = this.ProcessUriFromPayload(editLinkString);
                        break;

                    case JsonConstants.ODataMetadataMediaUriName:       // "media_src" : "url"
                        if (streamReference.ReadLink != null)
                        {
                            // we found another media_src property; this is not allowed
                            throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(JsonConstants.ODataMetadataMediaUriName));
                        }

                        string readLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaUriName);
                        ODataJsonReaderUtils.ValidateMediaResourceStringProperty(readLinkString, JsonConstants.ODataMetadataMediaUriName);
                        streamReference.ReadLink = this.ProcessUriFromPayload(readLinkString);
                        break;

                    case JsonConstants.ODataMetadataContentTypeName:    // "content_type" : "type"
                        if (streamReference.ContentType != null)
                        {
                            // we found another content_type property; this is not allowed
                            throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(JsonConstants.ODataMetadataContentTypeName));
                        }

                        string contentTypeString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataContentTypeName);
                        ODataJsonReaderUtils.ValidateMediaResourceStringProperty(contentTypeString, JsonConstants.ODataMetadataContentTypeName);
                        streamReference.ContentType = contentTypeString;
                        break;

                    case JsonConstants.ODataMetadataMediaETagName:      // "media_etag" : "etag"
                        if (streamReference.ETag != null)
                        {
                            // we found another content_type property; this is not allowed
                            throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(JsonConstants.ODataMetadataMediaETagName));
                        }

                        string etag = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaETagName);
                        ODataJsonReaderUtils.ValidateMediaResourceStringProperty(etag, JsonConstants.ODataMetadataMediaETagName);
                        streamReference.ETag = etag;
                        break;

                    default:
                        // ignore all properties that we don't recognize
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "Only Property or EndObject can appear at the object scope.");
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: JsonNodeType.Property or JsonNodeType.EndObject");

            return streamReference;
        }

        /// <summary>
        /// Validates that the value of a JSON property can represent navigation link.
        /// </summary>
        /// <param name="isCollection">true if the property is entity set reference property; false for a resource reference property.</param>
        private void ValidateNavigationLinkPropertyValue(bool isCollection)
        {
            // a deferred link, expanded link with entry or >=V2 expanded link with feed require a StartObject node here;
            // a V1 expanded link with feed requires a StartArray node here;
            // an expanded link with null entry requires a primitive null node here;
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                if (!isCollection)
                {
                    throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(nodeType));
                }

                if (this.Version >= ODataVersion.V2)
                {
                    // if we are reading a payload with version >=V2 all expanded feeds must be wrapped.
                    throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadWrappedFeedStart(nodeType));
                }
            }
            else if (nodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null)
            {
                // Expanded null entry
                if (isCollection)
                {
                    throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue(nodeType));
                }
            }
            else if (nodeType != JsonNodeType.StartObject)
            {
                Debug.Assert(nodeType == JsonNodeType.PrimitiveValue, "nodeType == JsonNodeType.PrimitiveValue");
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
            }
        }
    }
}
