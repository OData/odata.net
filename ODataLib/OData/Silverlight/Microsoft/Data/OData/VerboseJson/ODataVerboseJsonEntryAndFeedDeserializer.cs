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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON deserializer for entries and feeds.
    /// </summary>
    internal sealed class ODataVerboseJsonEntryAndFeedDeserializer : ODataVerboseJsonPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The Verbose JSON input context to read from.</param>
        internal ODataVerboseJsonEntryAndFeedDeserializer(ODataVerboseJsonInputContext verboseJsonInputContext)
            : base(verboseJsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Reads the start of a feed; this includes feed-level properties if the version permits it.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="isResultsWrapperExpected">A flag indicating whether we expect the results wrapper for feeds to be present.</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:    for a feed without 'results' wrapper
        ///                 JsonNodeType.StartObject:   for a feed wrapped with 'results' wrapper
        /// Post-Condition: Any start node              The first item in the feed
        ///                 JsonNodeType.EndArray:      The end of the feed
        /// </remarks>
        internal void ReadFeedStart(ODataFeed feed, bool isResultsWrapperExpected, bool isExpandedLinkContent)
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
                while (true)
                {
                    if (this.JsonReader.NodeType != JsonNodeType.Property)
                    {
                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "Only properties and end-of-object should be returned here.");

                        // we only expect properties until we find the 'results' property
                        throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound);
                    }

                    // read the property name and move the reader onto the start of the property value
                    string propertyName = this.JsonReader.ReadPropertyName();

                    if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                    {
                        break;
                    }

                    this.ReadFeedProperty(feed, propertyName, isExpandedLinkContent);
                }

                // At this point the reader is guaranteed to be positioned over the value of the 'results' property
            }

            // at this point the reader is positioned on the start array node for the feed contents;
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartArray();

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
        }

        /// <summary>
        /// Reads the end of a feed; this includes feed-level properties if the version permits them.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="readResultsWrapper">A flag indicating whether we expect the results wrapper for feeds to be present.</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndArray
        /// Post-Condition: JsonNodeType.EndArray   if the feed is not wrapped
        ///                 JsonNodeType.EndObject  if the feed is wrapped
        /// </remarks>
        internal void ReadFeedEnd(ODataFeed feed, bool readResultsWrapper, bool isExpandedLinkContent)
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
                    this.ReadFeedProperty(feed, propertyName, isExpandedLinkContent);
                }

                Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "A wrapped feed must end with an EndObject node.");
            }

            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the start of an entry (non-null)
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Will fail if it's anything else
        /// Post-Condition: JsonNodeType.Property               The first property of the entry
        ///                 JsonNodeType.EndObject              The end of the property object
        /// </remarks>
        internal void ReadEntryStart()
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntryStart(this.JsonReader.NodeType));
            }

            // read over the StartObject node and position the reader on the first node of the entry's content
            this.JsonReader.ReadNext();

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the entry metadata property.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  JsonNodeType.Object             The start object of the __metadata property value.
        /// Post-Condition: JsonNodeType.EndObject          The end object of the __metadtaa property value.
        /// </remarks>
        internal void ReadEntryMetadataPropertyValue(IODataVerboseJsonReaderEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertJsonCondition(JsonNodeType.StartObject);
            this.JsonReader.AssertBuffering();

            ODataEntry entry = entryState.Entry;

            // read over the start-object node
            this.JsonReader.ReadStartObject();

            ODataStreamReferenceValue mediaResource = null;
            ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField = 0;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                switch (propertyName)
                {
                    case JsonConstants.ODataMetadataUriName:    // 'uri'
                        this.ReadUriMetadataProperty(entry, ref metadataPropertiesFoundBitField);
                        break;

                    case JsonConstants.ODataEntryIdName:   // 'id'
                        this.ReadIdMetadataProperty(entry, ref metadataPropertiesFoundBitField);
                        break;

                    case JsonConstants.ODataMetadataETagName:   // 'etag'
                        this.ReadETagMetadataProperty(entry, ref metadataPropertiesFoundBitField);
                        break;

                    case JsonConstants.ODataMetadataTypeName:   // 'type'
                        // The type was already read and validated.
                        this.JsonReader.SkipValue();
                        break;

                    case JsonConstants.ODataMetadataMediaUriName:   // 'media_src'
                        this.ReadMediaSourceMetadataProperty(ref metadataPropertiesFoundBitField, ref mediaResource);
                        break;

                    case JsonConstants.ODataMetadataEditMediaName:  // 'edit_media'
                        this.ReadEditMediaMetadataProperty(ref metadataPropertiesFoundBitField, ref mediaResource);
                        break;

                    case JsonConstants.ODataMetadataContentTypeName:  // 'content_type'
                        this.ReadContentTypeMetadataProperty(ref metadataPropertiesFoundBitField, ref mediaResource);
                        break;

                    case JsonConstants.ODataMetadataMediaETagName:  // 'media_etag'
                        this.ReadMediaETagMetadataProperty(ref metadataPropertiesFoundBitField, ref mediaResource);
                        break;

                    case JsonConstants.ODataActionsMetadataName:  // 'actions'
                        this.ReadActionsMetadataProperty(entry, ref metadataPropertiesFoundBitField);
                        break;

                    case JsonConstants.ODataFunctionsMetadataName:  // 'functions'
                        this.ReadFunctionsMetadataProperty(entry, ref metadataPropertiesFoundBitField);
                        break;

                    case JsonConstants.ODataMetadataPropertiesName:  // 'properties'
                        this.ReadPropertiesMetadataProperty(entryState, ref metadataPropertiesFoundBitField);
                        break;

                    default:
                        // ignore all extra properties in __metadata that we don't understand
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            entry.MediaResource = mediaResource;

            this.AssertJsonCondition(JsonNodeType.EndObject);
        }

        /// <summary>
        /// Validates entry metadata properties against the model.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <remarks>
        /// This method must be called only after the ReadEntryMetadata was already called.
        /// It should be called always, regardless of whether the __metadata property was found ot not.
        /// </remarks>
        internal void ValidateEntryMetadata(IODataVerboseJsonReaderEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");

            ODataEntry entry = entryState.Entry; 
            IEdmEntityType entityType = entryState.EntityType;
            Debug.Assert(entityType != null, "We should have resolved the entity type by now.");

            if (this.Model.HasDefaultStream(entityType) && entry.MediaResource == null)
            {
                ODataStreamReferenceValue mediaResource = null; 
                ODataVerboseJsonReaderUtils.EnsureInstance(ref mediaResource);
                entry.MediaResource = mediaResource;
            }

            // By default validate the media resource.
            // In WCF DS Server mode, do not validate in JSON (here).
            // In WCF DS Client mode, do not validate.
            bool validateMediaResource = this.UseDefaultFormatBehavior;
            ValidationUtils.ValidateEntryMetadataResource(entry, entityType, this.Model, validateMediaResource);
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
        ///                                                     (deferred link or entry inside expanded link or feed with 'results' wrapper inside expanded link)
        ///                 JsonNodeType.StartArray             feed without 'results' wrapper inside of expanded link
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry
        /// </remarks>
        internal ODataNavigationLink ReadEntryContent(
            IODataVerboseJsonReaderEntryState entryState,
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
            IEdmEntityType entityType = entryState.EntityType;
            Debug.Assert(entityType != null, "In JSON we must always have an entity type when reading entity.");

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
                        throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue);
                    }

                    entryState.MetadataPropertyFound = true;

                    // Just skip over the value since we should have already scanned ahead for it by now.
                    this.JsonReader.SkipValue();
                }
                else
                {
                    if (!ValidationUtils.IsValidPropertyName(propertyName))
                    {
                        this.JsonReader.SkipValue();
                        continue;
                    }

                    IEdmProperty edmProperty = ReaderValidationUtils.FindDefinedProperty(propertyName, entityType);
                    if (edmProperty != null)
                    {
                        // Declared property - read it.
                        navigationProperty = edmProperty as IEdmNavigationProperty;
                        if (navigationProperty != null)
                        {
                            if (this.ShouldEntryPropertyBeSkipped())
                            {
                                this.JsonReader.SkipValue();
                            }
                            else
                            {
                                // this can be a deferred link or an expanded link (with entry or feed payload)
                                bool isCollection = navigationProperty.Type.IsCollection();
                                navigationLink = new ODataNavigationLink
                                {
                                    Name = propertyName,
                                    IsCollection = isCollection
                                };

                                this.ValidateNavigationLinkPropertyValue(isCollection);
                                entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLink);
                            }
                        }
                        else
                        {
                            // NOTE: we currently do not check whether the property should be skipped
                            //       here because this can only happen for navigation properties and open properties.
                            this.ReadEntryProperty(entryState, edmProperty);
                        }
                    }
                    else
                    {
                        // Undeclared property - we need to run detection alogorithm here.
                        navigationLink = this.ReadUndeclaredProperty(entryState, propertyName);

                        // Note that if navigation link is returned it's already validated, so we just report it here.
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

            // The reader can be either on
            //  - StartObject - if it's an expanded entry, or expanded feed wrapped with 'results' wrapper
            //  - StartArray - if it's an expanded feed that is not wrapped with 'results' wrapper
            //  - PrimitiveValue (null) - if it's an expanded null entry
            //  - EndObject - end of the entry
            Debug.Assert(
                navigationLink != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                navigationLink != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
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
        internal void ReadDeferredNavigationLink(ODataNavigationLink navigationLink)
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
                        throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleUriPropertiesInDeferredLink);
                    }

                    // read the value of the 'uri' property
                    string deferredLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataNavigationLinkUriName);
                    if (deferredLinkString == null)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_DeferredLinkUriCannotBeNull);
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
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_DeferredLinkMissingUri);
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
        /// Reads the entity reference link and moves the reader forward to the position after the link.
        /// </summary>
        /// <returns>The entity reference link read from the payload.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:    the start of the object representing the entity reference link
        /// Post-Condition: JsonNodeType.Property:       the next property after the entity reference link
        ///                 JsonNodeType.EndObject:      the end of the owning entry if the entity reference link is the last property
        ///                 JsonNodeType.EndArray:       the end of the owning array (if the entity reference link is part of expanded feed)
        ///                 Any:                         the next item in the owning array (if the entity reference link is part of expanded feed)
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "metadataPropertyName", Justification = "Used in debug assertions.")]
        internal ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertJsonCondition(JsonNodeType.StartObject);
            this.JsonReader.AssertNotBuffering();

            // read the start object node of the object with the __metadata property
            this.JsonReader.ReadStartObject();

            // read the __metadata property; the presence of the property is guaranteed by the link detection code
            // move the reader to the start of the property value of the __metadata property
            string metadataPropertyName = this.JsonReader.ReadPropertyName();
            Debug.Assert(metadataPropertyName == JsonConstants.ODataMetadataName, "Expected __metadata property here.");

            // start of the metadata object; position the reader on the node following the start object node
            this.JsonReader.ReadStartObject();

            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink();
            ODataVerboseJsonReaderUtils.MetadataPropertyBitMask propertiesFoundBitField = 0;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // read the 'uri' property
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataMetadataUriName, propertyName) == 0)
                {
                    // read the value of the 'uri' property
                    ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                        ref propertiesFoundBitField,
                        ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Uri,
                        JsonConstants.ODataMetadataUriName);
                    string entityReferenceLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataUriName);
                    ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(entityReferenceLinkString, JsonConstants.ODataMetadataUriName);

                    entityReferenceLink.Url = this.ProcessUriFromPayload(entityReferenceLinkString);
                }
                else
                {
                    // Skip unrecognized properties
                    this.JsonReader.SkipValue();
                }
            }

            // end of the __metadata object
            this.JsonReader.ReadEndObject();

            // end of the entity reference link object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

        /// <summary>
        /// Analyzes the current navigation property node to determine whether it represents a deferred link.
        /// </summary>
        /// <param name="navigationLinkFound">true if we already detected a navigation link and now determine its kind; false if we are detecting a deferred navigation link.</param>
        /// <returns>true if the current navigation property represents a deferred link; false for an expanded navigation link.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  The first node of the property value
        /// Post-Condition: The first node of the property value
        /// </remarks>
        internal bool IsDeferredLink(bool navigationLinkFound)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                !navigationLinkFound || this.ReadingResponse, 
                "Either we are scanning for a deferred link or ensure that deferred links can only appear in responses.");

            // The reader is already positioned on the first node of the value
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.PrimitiveValue)
            {
                if (this.JsonReader.Value != null)
                {
                    // a navigation property value cannot be a primitive value (unless it's null)
                    if (navigationLinkFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
                    }

                    return false;
                }

                // the value of the property is an expanded null entry; this is an expanded link
                return false;
            }

            if (nodeType == JsonNodeType.StartArray)
            {
                // the value of the property is a V1 feed; this is an expanded link
                return false;
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
                    return false;
                }

                Debug.Assert(nodeType == JsonNodeType.Property, "Only EndObject and Property nodes are expected after a StartObject node.");
                string innerPropertyName = this.JsonReader.ReadPropertyName();

                // We recognize deferred links by ensuring that there is a single property whose name must be '__deferred'.
                if (string.CompareOrdinal(JsonConstants.ODataDeferredName, innerPropertyName) != 0)
                {
                    return false;
                }

                this.JsonReader.SkipValue();
                Debug.Assert(
                    this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "Expected to either find another property or the end of the link object.");

                return this.JsonReader.NodeType == JsonNodeType.EndObject;
            }
            finally
            {
                this.JsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Analyzes the current node to determine whether it represents an entity reference link.
        /// </summary>
        /// <returns>true if the current node represents an entity reference link; false is it's an entry or something else.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  The first node of the property value
        /// Post-Condition: The first node of the property value
        /// </remarks>
        internal bool IsEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(!this.ReadingResponse, "Entity reference links can only appear in requests.");

            // The reader is already positioned on the first node of the value
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType != JsonNodeType.StartObject)
            {
                // The value must be an object for it to be recognized as entity reference link.
                return false;
            }

            // We read another node but do so in buffering mode so we can continue to read where we left off after figuring out if the value is an entity reference link.
            this.JsonReader.StartBuffering();

            try
            {
                this.JsonReader.ReadStartObject();

                nodeType = this.JsonReader.NodeType;
                if (nodeType == JsonNodeType.EndObject)
                {
                    // Empty object, it's not the entity reference link.
                    return false;
                }

                Debug.Assert(nodeType == JsonNodeType.Property, "Only EndObject and Property nodes are expected after a StartObject node.");

                // Entity reference link must have a single property called __metadata
                // and its value must have a property called uri (and can have any number of other properties).
                bool uriPropertyFound = false;
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string metadataPropertyName = this.JsonReader.ReadPropertyName();
                    if (string.CompareOrdinal(JsonConstants.ODataMetadataName, metadataPropertyName) != 0)
                    {
                        // If we find any other property than __metadata, assume it's an expanded entry instance.
                        return false;
                    }
                    else
                    {
                        // Found __metadata, it must be an object with a uri property in it
                        if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                        {
                            return false;
                        }

                        this.JsonReader.ReadStartObject();

                        while (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            string innerPropertyName = this.JsonReader.ReadPropertyName();
                            if (string.CompareOrdinal(JsonConstants.ODataMetadataUriName, innerPropertyName) == 0)
                            {
                                uriPropertyFound = true;
                            }

                            this.JsonReader.SkipValue();
                        }

                        this.JsonReader.ReadEndObject();
                    }
                }

                // We didn't find anything which would not match entity reference link,
                // thus assume this is an entity reference link if we did find the uri property.
                return uriPropertyFound;
            }
            finally
            {
                this.JsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Adds a new property to an entry.
        /// </summary>
        /// <param name="entryState">The entry state for the entry to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property to add.</param>
        private static void AddEntryProperty(IODataVerboseJsonReaderEntryState entryState, string propertyName, object propertyValue)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            ODataProperty property = new ODataProperty { Name = propertyName, Value = propertyValue };
            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            ODataEntry entry = entryState.Entry;
            Debug.Assert(entry != null, "entry != null");
            entry.Properties = entry.Properties.ConcatToReadOnlyEnumerable("Properties", property);
        }

        /// <summary>
        /// Read a feed-level property (e.g., __count, __next, etc.) and check its version compliance. 
        /// This method fails on properties that are not recognized as feed-level properties.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> instance to fill with the data read.</param>
        /// <param name="propertyName">The name of the property being read.</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is on the first node of the feed-level property's value.
        /// Post-Condition: JsonNodeType.Property:          the next feed property to read or
        ///                 JsonNodeType.EndObject:         the end of the results wrapper
        /// </remarks>
        private void ReadFeedProperty(ODataFeed feed, string propertyName, bool isExpandedLinkContent)
        {
            Debug.Assert(feed != null, "feed != null");
            this.JsonReader.AssertNotBuffering();

            ODataVerboseJsonReaderUtils.FeedPropertyKind propertyKind = ODataVerboseJsonReaderUtils.DetermineFeedPropertyKind(propertyName);

            switch (propertyKind)
            {
                case ODataVerboseJsonReaderUtils.FeedPropertyKind.Count:
                    {
                        // "__count": "number"
                        if (this.ReadingResponse && this.Version >= ODataVersion.V2 && !isExpandedLinkContent)
                        {
                            string countString = this.JsonReader.ReadStringValue(JsonConstants.ODataCountName);
                            ODataVerboseJsonReaderUtils.ValidateFeedProperty(countString, JsonConstants.ODataCountName);
                            long count = (long)ODataVerboseJsonReaderUtils.ConvertValue(
                                countString, 
                                EdmCoreModel.Instance.GetInt64(false), 
                                this.MessageReaderSettings,
                                this.Version, 
                                /*validateNullValue*/ true,
                                propertyName);
                            feed.Count = count;
                        }
                        else
                        {
                            // "__count" property is not expected in request or V1 payloads so ignore it
                            this.JsonReader.SkipValue();
                        }
                    }

                    break;
                case ODataVerboseJsonReaderUtils.FeedPropertyKind.NextPageLink:
                    {
                        // "__next": "next-link"
                        if (this.ReadingResponse && this.Version >= ODataVersion.V2)
                        {
                            string nextLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataNextLinkName);
                            ODataVerboseJsonReaderUtils.ValidateFeedProperty(nextLinkString, JsonConstants.ODataNextLinkName);
                            feed.NextPageLink = this.ProcessUriFromPayload(nextLinkString);
                        }
                        else
                        {
                            // "__next" property is not expected in request and V1 payloads so ignore it
                            this.JsonReader.SkipValue();
                        }
                    }

                    break;
                case ODataVerboseJsonReaderUtils.FeedPropertyKind.Unsupported:
                    // Ignore any properties we don't recognize
                    this.JsonReader.SkipValue();
                    break;
                case ODataVerboseJsonReaderUtils.FeedPropertyKind.Results:
                    // If we find results property here, it means we didn't expect it and we've already seen one.
                    throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound);
                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataVerboseJsonEntryAndFeedDeserializer_ReadFeedProperty));
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
        private void ReadEntryProperty(IODataVerboseJsonReaderEntryState entryState, IEdmProperty edmProperty)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");
            this.JsonReader.AssertNotBuffering();

            ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse
                ? ODataNullValueBehaviorKind.Default
                : this.Model.NullValueReadBehaviorKind(edmProperty);
            IEdmTypeReference propertyTypeReference = edmProperty.Type;
            object propertyValue = propertyTypeReference.IsStream()
                ? this.ReadStreamPropertyValue()
                : this.ReadNonEntityValue(
                    propertyTypeReference, 
                    /*duplicatePropertyNamesChecker*/ null, 
                    /*collectionValidator*/ null, 
                    nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default, 
                    edmProperty.Name);

            if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.IgnoreValue || propertyValue != null)
            {
                AddEntryProperty(entryState, edmProperty.Name, propertyValue);
            }

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
        private void ReadOpenProperty(IODataVerboseJsonReaderEntryState entryState, string propertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            this.JsonReader.AssertNotBuffering();

            object propertyValue = this.ReadNonEntityValue(
                /*expectedValueTypeReference*/ null, 
                /*duplicatePropertyNamesChecker*/ null, 
                /*collectionValidator*/ null,
                /*validateNullValue*/ true,
                propertyName);

            ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue, this.MessageReaderSettings.UndeclaredPropertyBehaviorKinds);
            AddEntryProperty(entryState, propertyName, propertyValue);

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an undeclared property. That is a property which is not declared by the model, but the owning type is not an open type.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        /// <returns>A navigation link instance if the propery read is a navigation link which should be reported to the caller.
        /// Otherwise null if the property was either ignored or read and added to the list of properties on the entry.</returns>
        private ODataNavigationLink ReadUndeclaredProperty(IODataVerboseJsonReaderEntryState entryState, string propertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (entryState.EntityType.IsOpen && this.ShouldEntryPropertyBeSkipped())
            {
                this.JsonReader.SkipValue();
                return null;
            }

            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            bool streamPropertyFound = false;
            bool deferredLinkFound = false;
            if (this.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.JsonReader.StartBuffering();

                // Both stream property and deferred link must be an object with exactly one property in it, with the right name.
                this.JsonReader.Read();
                if (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string innerPropertyName = this.JsonReader.ReadPropertyName();
                    if (string.CompareOrdinal(innerPropertyName, JsonConstants.ODataDeferredName) == 0)
                    {
                        deferredLinkFound = true;
                    }
                    else if (string.CompareOrdinal(innerPropertyName, JsonConstants.ODataMetadataMediaResourceName) == 0)
                    {
                        streamPropertyFound = true;
                    }

                    this.JsonReader.SkipValue();

                    // The __mediaresource or __deferred is not the only property, so the value is not a stream property or deferred link.
                    if (this.JsonReader.NodeType != JsonNodeType.EndObject)
                    {
                        streamPropertyFound = false;
                        deferredLinkFound = false;
                    }
                }

                this.JsonReader.StopBuffering();
            }

            if (streamPropertyFound || deferredLinkFound)
            {
                // Undeclared link properties (stream property or deferred links) are reported if the right flag is used, otherwise we need to fail
                // reporting the undeclared property.
                if (!this.MessageReaderSettings.ContainUndeclaredPropertyBehavior(ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty))
                {
                    throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.ODataFullName()));
                }
            }
            else
            {
                if (entryState.EntityType.IsOpen)
                {
                    // Open property - read it as such.
                    this.ReadOpenProperty(entryState, propertyName);
                    return null;
                }

                // Undeclared value properties (anything which doesn't look like stream property or deferred link) are ignored if the right flag is used,
                // otherwise we need to fail reporting the undeclared property.
                if (!this.MessageReaderSettings.ContainUndeclaredPropertyBehavior(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty))
                {
                    throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.ODataFullName()));
                }
            }

            if (deferredLinkFound)
            {
                // We've found a deferred link - so report it (we've already checked the flags which allow this).
                ODataNavigationLink navigationLink = new ODataNavigationLink
                {
                    Name = propertyName,
                    IsCollection = null
                };

                // Note - no need to validate the navigation link since we know it's a valid deferred link at least from the first object content.
                entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLink);
                return navigationLink;
            }

            if (streamPropertyFound)
            {
                // We've found a stream property, so read it and report it (we've already checked the flags which allow this).
                object propertyValue = this.ReadStreamPropertyValue();
                AddEntryProperty(entryState, propertyName, propertyValue);
                return null;
            }

            // Primitive, Complex, Collection or expanded link property, or property we don't recognize - treat all of these as value properties
            // and thus ignore them.
            // Note that in JSON expanded navigation links don't have the URL of the navigation link (the "href")
            // and thus we would have nothing to report even if the ReportUndeclaredLinkProperty would be set to true.
            // As such it's OK to treat them as value properties all up.
            this.JsonReader.SkipValue();
            return null;
        }

        /// <summary>
        /// Reads a stream property value.
        /// </summary>
        /// <returns>The value of the stream property.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: almost anything - the node after the stream reference (after the EndObject)
        /// </remarks>
        private ODataStreamReferenceValue ReadStreamPropertyValue()
        {
            this.JsonReader.AssertNotBuffering();

            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_StreamPropertyInRequest);
            }

            ODataVersionChecker.CheckStreamReferenceProperty(this.Version);

            // Read over the start object of the stream reference
            this.JsonReader.ReadStartObject();

            // Stream reference value must have a single property called __metadata
            ODataStreamReferenceValue streamReference = null;
            if (this.JsonReader.NodeType != JsonNodeType.Property)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }

            string propertyName = this.JsonReader.ReadPropertyName();
            if (string.CompareOrdinal(JsonConstants.ODataMetadataMediaResourceName, propertyName) == 0)
            {
                streamReference = this.ReadStreamReferenceValue();
            }
            else
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }

            if (this.JsonReader.NodeType != JsonNodeType.EndObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }

            this.JsonReader.Read();
            this.JsonReader.AssertNotBuffering();

            return streamReference;
        }

        /// <summary>
        /// Reads the uri property in metadata value.
        /// </summary>
        /// <param name="entry">The entry being read.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'uri' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadUriMetadataProperty(ODataEntry entry, ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entry != null, "entry != null");

            ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                ref metadataPropertiesFoundBitField,
                ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Uri,
                JsonConstants.ODataMetadataUriName);
            string editLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataUriName);

            if (editLinkString != null)
            {
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(editLinkString, JsonConstants.ODataMetadataUriName);
                entry.EditLink = this.ProcessUriFromPayload(editLinkString);
            }
        }

        /// <summary>
        /// Reads the id property in metadata value.
        /// </summary>
        /// <param name="entry">The entry being read.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'id' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadIdMetadataProperty(ODataEntry entry, ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entry != null, "entry != null");

            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Id,
                    JsonConstants.ODataEntryIdName);
                string idString = this.JsonReader.ReadStringValue(JsonConstants.ODataEntryIdName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(idString, JsonConstants.ODataEntryIdName);
                entry.Id = idString;
            }
        }

        /// <summary>
        /// Reads the etag property in metadata value.
        /// </summary>
        /// <param name="entry">The entry being read.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'etag' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadETagMetadataProperty(ODataEntry entry, ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entry != null, "entry != null");

            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.ETag,
                    JsonConstants.ODataMetadataETagName);
                string etagString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataETagName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(etagString, JsonConstants.ODataMetadataETagName);
                entry.ETag = etagString;
            }
        }

        /// <summary>
        /// Reads the media_src property in metadata value.
        /// </summary>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <param name="mediaResource">The media resource value for the entry.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'media_src' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadMediaSourceMetadataProperty(
            ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField,
            ref ODataStreamReferenceValue mediaResource)
        {
            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.MediaUri,
                    JsonConstants.ODataMetadataMediaUriName);
                ODataVerboseJsonReaderUtils.EnsureInstance(ref mediaResource);
                string mediaReadLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaUriName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(mediaReadLinkString, JsonConstants.ODataMetadataMediaUriName);
                mediaResource.ReadLink = this.ProcessUriFromPayload(mediaReadLinkString);
            }
        }

        /// <summary>
        /// Reads the edit_media property in metadata value.
        /// </summary>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <param name="mediaResource">The media resource value for the entry.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'edit_media' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadEditMediaMetadataProperty(
            ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField,
            ref ODataStreamReferenceValue mediaResource)
        {
            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.EditMedia,
                    JsonConstants.ODataMetadataEditMediaName);
                ODataVerboseJsonReaderUtils.EnsureInstance(ref mediaResource);
                string mediaEditLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataEditMediaName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(mediaEditLinkString, JsonConstants.ODataMetadataEditMediaName);
                mediaResource.EditLink = this.ProcessUriFromPayload(mediaEditLinkString);
            }
        }

        /// <summary>
        /// Reads the content_type property in metadata value.
        /// </summary>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <param name="mediaResource">The media resource value for the entry.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'content_type' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadContentTypeMetadataProperty(
            ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField,
            ref ODataStreamReferenceValue mediaResource)
        {
            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.ContentType,
                    JsonConstants.ODataMetadataContentTypeName);
                ODataVerboseJsonReaderUtils.EnsureInstance(ref mediaResource);
                string mediaContentType = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataContentTypeName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(mediaContentType, JsonConstants.ODataMetadataContentTypeName);
                mediaResource.ContentType = mediaContentType;
            }
        }

        /// <summary>
        /// Reads the media_etag property in metadata value.
        /// </summary>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <param name="mediaResource">The media resource value for the entry.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'media_etag' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadMediaETagMetadataProperty(
            ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField,
            ref ODataStreamReferenceValue mediaResource)
        {
            if (this.UseServerFormatBehavior)
            {
                this.JsonReader.SkipValue();
            }
            else
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.MediaETag,
                    JsonConstants.ODataMetadataMediaETagName);
                ODataVerboseJsonReaderUtils.EnsureInstance(ref mediaResource);
                string mediaETag = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaETagName);
                ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(mediaETag, JsonConstants.ODataMetadataMediaETagName);
                mediaResource.ETag = mediaETag;
            }
        }

        /// <summary>
        /// Reads the actions property in metadata value.
        /// </summary>
        /// <param name="entry">The entry being read.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'actions' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadActionsMetadataProperty(ODataEntry entry, ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entry != null, "entry != null");

            if (this.MessageReaderSettings.MaxProtocolVersion >= ODataVersion.V3 && this.ReadingResponse)
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Actions,
                    JsonConstants.ODataActionsMetadataName);
                this.ReadOperationsMetadata(entry, true /* isActions */);
            }
            else
            {
                this.JsonReader.SkipValue();
            }
        }

        /// <summary>
        /// Reads the functions property in metadata value.
        /// </summary>
        /// <param name="entry">The entry being read.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'functions' property's value
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadFunctionsMetadataProperty(ODataEntry entry, ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entry != null, "entry != null");

            if (this.MessageReaderSettings.MaxProtocolVersion >= ODataVersion.V3 && this.ReadingResponse)
            {
                ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                    ref metadataPropertiesFoundBitField,
                    ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Functions,
                    JsonConstants.ODataFunctionsMetadataName);
                this.ReadOperationsMetadata(entry, false /* isActions */);
            }
            else
            {
                this.JsonReader.SkipValue();
            }
        }

        /// <summary>
        /// Read the property metadata for the properties of an entry being read.
        /// </summary>
        /// <param name="entryState">The entry state for the current reader.</param>
        /// <param name="metadataPropertiesFoundBitField">The bit fields with all the properties found in metadata value so far.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'properties' property's value (we will throw if this is not a start object node)
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        ///                 
        /// This method will not validate anything against the model because it will read the type name and thus it can't rely
        /// on knowing the actual type of the entry being read.
        /// </remarks>
        private void ReadPropertiesMetadataProperty(
            IODataVerboseJsonReaderEntryState entryState,
            ref ODataVerboseJsonReaderUtils.MetadataPropertyBitMask metadataPropertiesFoundBitField)
        {
            Debug.Assert(entryState != null, "entryState != null");

            if (!this.ReadingResponse || this.MessageReaderSettings.MaxProtocolVersion < ODataVersion.V3)
            {
                this.JsonReader.SkipValue();
                return;
            }

            ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                ref metadataPropertiesFoundBitField,
                ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Properties,
                JsonConstants.ODataMetadataPropertiesName);

            this.JsonReader.AssertBuffering();

            // make sure the 'properties' property value is an object
            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue(JsonConstants.ODataMetadataPropertiesName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for 'properties'
            this.JsonReader.ReadStartObject();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                ValidationUtils.ValidateAssociationLinkName(propertyName);
                ReaderValidationUtils.ValidateNavigationPropertyDefined(propertyName, entryState.EntityType, this.MessageReaderSettings);

                // read the start-object node of the metadata for the current propertyName
                this.JsonReader.ReadStartObject();

                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string innerPropertyName = this.JsonReader.ReadPropertyName();

                    // ignore all properties we don't understand
                    if (string.CompareOrdinal(innerPropertyName, JsonConstants.ODataMetadataPropertiesAssociationUriName) == 0)
                    {
                        string associationUrlString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataPropertiesAssociationUriName);
                        ODataVerboseJsonReaderUtils.ValidateMetadataStringProperty(associationUrlString, JsonConstants.ODataMetadataPropertiesAssociationUriName);
                        ODataAssociationLink associationLink = new ODataAssociationLink
                        {
                            Name = propertyName,
                            Url = this.ProcessUriFromPayload(associationUrlString)
                        };

                        ValidationUtils.ValidateAssociationLink(associationLink);
                        ReaderUtils.CheckForDuplicateAssociationLinkAndUpdateNavigationLink(entryState.DuplicatePropertyNamesChecker, associationLink);
                        entryState.Entry.AddAssociationLink(associationLink);
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

            this.JsonReader.AssertBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read the 'actions' or 'functions' metadata for the entry being read.
        /// </summary>
        /// <param name="entry">The <see cref="ODataEntry"/> the 'actions' or 'functions' metadata is read for.</param>
        /// <param name="isActions">When True the 'actions' metadata is being read, otherwise 'functions' metadata is being read.</param>
        /// <remarks>
        /// Pre-Condition:  first node of the 'actions' or 'functions' property's value (we will throw if this is not a start object node)
        /// Post-Condition: JsonNodeType.Property:      the next metadata property
        ///                 JsonNodeType.EndObject:     the end-object node of the metadata object
        /// </remarks>
        private void ReadOperationsMetadata(ODataEntry entry, bool isActions)
        {
            Debug.Assert(entry != null, "entry != null");
            this.JsonReader.AssertBuffering();

            IODataJsonOperationsDeserializerContext readerContext = new OperationsDeserializerContext(entry, this);
            string operationsHeaderName = isActions ? JsonConstants.ODataActionsMetadataName : JsonConstants.ODataFunctionsMetadataName;

            // make sure the operations value is an object
            if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(operationsHeaderName, readerContext.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            readerContext.JsonReader.ReadStartObject();

            // iterate through each property of the metadata object
            HashSet<string> metadataValues = new HashSet<string>(StringComparer.Ordinal);
            while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
            {
                string metadataValue = readerContext.JsonReader.ReadPropertyName();

                // JSON reader should have already validated that the property name is a string and is not empty or null.
                Debug.Assert(!string.IsNullOrEmpty(metadataValue), "!string.IsNullOrEmpty(metadataValue)");

                if (metadataValues.Contains(metadataValue))
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_RepeatMetadataValue(operationsHeaderName, metadataValue));
                }

                metadataValues.Add(metadataValue);

                if (readerContext.JsonReader.NodeType != JsonNodeType.StartArray)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_MetadataMustHaveArrayValue(metadataValue, readerContext.JsonReader.NodeType, operationsHeaderName));
                }

                // read the start-array node of the current metadata
                readerContext.JsonReader.ReadStartArray();

                if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationMetadataArrayExpectedAnObject(metadataValue, readerContext.JsonReader.NodeType, operationsHeaderName));
                }

                Uri metadataUri = this.ResolveUri(metadataValue);
                while (readerContext.JsonReader.NodeType == JsonNodeType.StartObject)
                {
                    readerContext.JsonReader.ReadStartObject();

                    ODataOperation operation;
                    if (isActions)
                    {
                        operation = new ODataAction();
                    }
                    else
                    {
                        operation = new ODataFunction();
                    }

                    operation.Metadata = metadataUri;
                    while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        string operationPropertyName = readerContext.JsonReader.ReadPropertyName();

                        switch (operationPropertyName)
                        {
                            case JsonConstants.ODataOperationTitleName:
                                if (operation.Title != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_MultipleOptionalPropertiesInOperation(
                                        operationPropertyName,
                                        metadataValue,
                                        operationsHeaderName));
                                }

                                string titleString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                                ReaderValidationUtils.ValidateOperationProperty(titleString, operationPropertyName, metadataValue, operationsHeaderName);
                                operation.Title = titleString;
                                break;

                            case JsonConstants.ODataOperationTargetName:
                                if (operation.Target != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_MultipleTargetPropertiesInOperation(
                                        metadataValue,
                                        operationsHeaderName));
                                }

                                string targetString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                                ReaderValidationUtils.ValidateOperationProperty(targetString, operationPropertyName, metadataValue, operationsHeaderName);
                                operation.Target = readerContext.ProcessUriFromPayload(targetString);
                                break;

                            default:
                                // skip over all unknown properties and read the next property or 
                                // the end of the metadata for the current propertyName
                                readerContext.JsonReader.SkipValue();
                                break;
                        }
                    }

                    if (operation.Target == null)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationMissingTargetProperty(metadataValue, operationsHeaderName));
                    }

                    // read the end-object node of the target / title pair
                    readerContext.JsonReader.ReadEndObject();

                    if (isActions)
                    {
                        readerContext.AddActionToEntry((ODataAction)operation);
                    }
                    else
                    {
                        readerContext.AddFunctionToEntry((ODataFunction)operation);
                    }
                }

                // read the end-array node of the metadata array
                readerContext.JsonReader.ReadEndArray();
            }

            // read over the end-object node of the metadata object for the operations
            readerContext.JsonReader.ReadEndObject();

            this.JsonReader.AssertBuffering();
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
        private ODataStreamReferenceValue ReadStreamReferenceValue()
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
                            throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty(JsonConstants.ODataMetadataEditMediaName));
                        }

                        string editLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataEditMediaName);
                        ODataVerboseJsonReaderUtils.ValidateMediaResourceStringProperty(editLinkString, JsonConstants.ODataMetadataEditMediaName);
                        streamReference.EditLink = this.ProcessUriFromPayload(editLinkString);
                        break;

                    case JsonConstants.ODataMetadataMediaUriName:       // "media_src" : "url"
                        if (streamReference.ReadLink != null)
                        {
                            // we found another media_src property; this is not allowed
                            throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty(JsonConstants.ODataMetadataMediaUriName));
                        }

                        string readLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaUriName);
                        ODataVerboseJsonReaderUtils.ValidateMediaResourceStringProperty(readLinkString, JsonConstants.ODataMetadataMediaUriName);
                        streamReference.ReadLink = this.ProcessUriFromPayload(readLinkString);
                        break;

                    case JsonConstants.ODataMetadataContentTypeName:    // "content_type" : "type"
                        if (streamReference.ContentType != null)
                        {
                            // we found another content_type property; this is not allowed
                            throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty(JsonConstants.ODataMetadataContentTypeName));
                        }

                        string contentTypeString = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataContentTypeName);
                        ODataVerboseJsonReaderUtils.ValidateMediaResourceStringProperty(contentTypeString, JsonConstants.ODataMetadataContentTypeName);
                        streamReference.ContentType = contentTypeString;
                        break;

                    case JsonConstants.ODataMetadataMediaETagName:      // "media_etag" : "etag"
                        if (streamReference.ETag != null)
                        {
                            // we found another content_type property; this is not allowed
                            throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty(JsonConstants.ODataMetadataMediaETagName));
                        }

                        string etag = this.JsonReader.ReadStringValue(JsonConstants.ODataMetadataMediaETagName);
                        ODataVerboseJsonReaderUtils.ValidateMediaResourceStringProperty(etag, JsonConstants.ODataMetadataMediaETagName);
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
        /// Tries to resolve the payload URI using the custom URI resolver. If the custom resolver fails, it returns the actual URI from the payload.
        /// </summary>
        /// <param name="uriFromPayload">the URI to be resolved.</param>
        /// <returns>A resolved URI or the URI from the payload, if the custom resolver fails.</returns>
        private Uri ResolveUri(string uriFromPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uriFromPayload != null, "uriFromPayload != null");
            Uri uri = new Uri(uriFromPayload, UriKind.RelativeOrAbsolute);

            // Try to resolve the URI using a custom URL resolver first.
            Uri resolvedUri = this.VerboseJsonInputContext.ResolveUri(this.MessageReaderSettings.BaseUri, uri);
            if (resolvedUri != null)
            {
                return resolvedUri;
            }

            return uri;
        }

        /// <summary>
        /// Validates that the value of a JSON property can represent navigation link.
        /// </summary>
        /// <param name="isCollection">true if the property is entity set reference property; false for a resource reference property.</param>
        private void ValidateNavigationLinkPropertyValue(bool isCollection)
        {
            // a deferred link, expanded link with entry or expanded link with feed wrapeed in 'results' wrapper require a StartObject node here;
            // an expanded link with feed without 'results' wrapper requires a StartArray node here;
            // an expanded link with null entry requires a primitive null node here;
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                if (!isCollection)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(nodeType));
                }
            }
            else if (nodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null)
            {
                // Expanded null entry
                if (isCollection)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue(nodeType));
                }
            }
            else if (nodeType != JsonNodeType.StartObject)
            {
                Debug.Assert(nodeType == JsonNodeType.PrimitiveValue, "nodeType == JsonNodeType.PrimitiveValue");
                throw new ODataException(ODataErrorStrings.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
            }
        }

        /// <summary>
        /// Determines whether a property of an entry should be skipped during reading.
        /// </summary>
        /// <returns>true if the current property should be skipped; otherwise false.</returns>
        private bool ShouldEntryPropertyBeSkipped()
        {
            return !this.ReadingResponse &&
                this.UseServerFormatBehavior &&
                this.IsDeferredLink(/*navigationLinkFound*/ false);
        }

        /// <summary>
        /// Operations deserializer context to pass to JSON operations reader.
        /// </summary>
        private sealed class OperationsDeserializerContext : IODataJsonOperationsDeserializerContext
        {
            /// <summary>
            /// The entry to add operations to.
            /// </summary>
            private ODataEntry entry;

            /// <summary>
            /// The deserializer to use.
            /// </summary>
            private ODataVerboseJsonEntryAndFeedDeserializer verboseJsonEntryAndFeedDeserializer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="entry">The entry to add operations to.</param>
            /// <param name="verboseJsonEntryAndFeedDeserializer">The deserializer to use.</param>
            public OperationsDeserializerContext(ODataEntry entry, ODataVerboseJsonEntryAndFeedDeserializer verboseJsonEntryAndFeedDeserializer)
            {
                Debug.Assert(entry != null, "entry != null");
                Debug.Assert(verboseJsonEntryAndFeedDeserializer != null, "verboseJsonEntryAndFeedDeserializer != null");

                this.entry = entry;
                this.verboseJsonEntryAndFeedDeserializer = verboseJsonEntryAndFeedDeserializer;
            }

            /// <summary>
            /// The JSON reader to read the operations value from.
            /// </summary>
            public JsonReader JsonReader
            {
                get
                {
                    return this.verboseJsonEntryAndFeedDeserializer.JsonReader;
                }
            }

            /// <summary>
            /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
            /// </summary>
            /// <param name="uriFromPayload">The URI string from the payload to process.</param>
            /// <returns>An absolute URI to report.</returns>
            public Uri ProcessUriFromPayload(string uriFromPayload)
            {
                return this.verboseJsonEntryAndFeedDeserializer.ProcessUriFromPayload(uriFromPayload);
            }

            /// <summary>
            /// Adds the specified action to the current entry.
            /// </summary>
            /// <param name="action">The action whcih is fully populated with the data from the payload.</param>
            public void AddActionToEntry(ODataAction action)
            {
                Debug.Assert(action != null, "action != null");
                this.entry.AddAction(action);
            }

            /// <summary>
            /// Adds the specified function to the current entry.
            /// </summary>
            /// <param name="function">The function whcih is fully populated with the data from the payload.</param>
            public void AddFunctionToEntry(ODataFunction function)
            {
                Debug.Assert(function != null, "function != null");
                this.entry.AddFunction(function);
            }
        }
    }
}
