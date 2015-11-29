﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for entries and feeds.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
    internal sealed class ODataJsonLightEntryAndFeedDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightEntryAndFeedDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Reads the start of the JSON array for the content of the feed.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:    The start of the feed property array; this method will fail if the node is anything else.
        /// Post-Condition: JsonNodeType.StartObject:   The first item in the feed
        ///                 JsonNodeType.EndArray:      The end of the feed
        /// </remarks>
        internal void ReadFeedContentStart()
        {
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_CannotReadFeedContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartArray();

            if (this.JsonReader.NodeType != JsonNodeType.EndArray && this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_InvalidNodeTypeForItemsInFeed(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the end of the array containing the feed content.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndArray
        /// Post-Condition: JsonNodeType.Property   if the feed is part of an expanded navigation link and there are more properties in the object
        ///                 JsonNodeType.EndObject  if the feed is a top-level feed or the expanded navigation link is the last property of the payload
        /// </remarks>
        internal void ReadFeedContentEnd()
        {
            this.AssertJsonCondition(JsonNodeType.EndArray);
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.ReadEndArray();

            this.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);
        }

        /// <summary>
        /// Reads the entry type name annotation (odata.type)
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the entry object.
        ///                 JsonNodeType.EndObject         End of the entry object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the entry object.
        ///                 
        /// This method fills the ODataEntry.TypeName property if the type name is found in the payload.
        /// </remarks>
        internal void ReadEntryTypeName(IODataJsonLightReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.type property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType, propertyName) == 0
                    || this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataTypePropertyName, propertyName))
                {
                    Debug.Assert(entryState.Entry.TypeName == null, "type name should not have already been set");

                    // Read over the property to move to its value.
                    this.JsonReader.Read();

                    // Read the annotation value.
                    entryState.Entry.TypeName = this.ReadODataTypeAnnotationValue();
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the content of an entry until a navigation link is detected.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <returns>A reader navigation link info representing the navigation link detected while reading the entry contents; null if no navigation link was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        ///                 JsonNodeType.Property               If we've read a deferred link (this is the property after the deferred link)
        ///                 JsonNodeType.StartObject            Expanded entry
        ///                 JsonNodeType.StartArray             Expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null
        /// </remarks>
        internal ODataJsonLightReaderNavigationLinkInfo ReadEntryContent(IODataJsonLightReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.EntityType != null && this.Model.IsUserModel(), "A non-null entity type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = null;
            Debug.Assert(entryState.EntityType != null, "In JSON we must always have an entity type when reading entity.");

            // Figure out whether we have more properties for this entry
            // read all the properties until we hit a link
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    entryState.DuplicatePropertyNamesChecker,
                    this.ReadEntryPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                object value = this.ReadEntryInstanceAnnotation(propertyName, entryState.AnyPropertyFound, /*typeAnnotationFound*/ true, entryState.DuplicatePropertyNamesChecker);
                                this.ApplyEntryInstanceAnnotation(entryState, propertyName, value);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                entryState.AnyPropertyFound = true;
                                navigationLinkInfo = this.ReadEntryPropertyWithoutValue(entryState, propertyName);
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                entryState.AnyPropertyFound = true;
                                navigationLinkInfo = this.ReadEntryPropertyWithValue(entryState, propertyName);
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                this.ReadMetadataReferencePropertyValue(entryState, propertyName);
                                break;

                            case PropertyParsingResult.EndOfObject:
                                break;
                        }
                    });

                if (navigationLinkInfo != null)
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
            //  - StartObject - if it's an expanded entry
            //  - StartArray - if it's an expanded feed
            //  - Property - if it's a deferred link
            //  - PrimitiveValue (null) - if it's an expanded null entry
            //  - EndObject - end of the entry
            Debug.Assert(
                navigationLinkInfo != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                navigationLinkInfo != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                navigationLinkInfo != null && this.JsonReader.NodeType == JsonNodeType.Property ||
                navigationLinkInfo != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Property or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return navigationLinkInfo;
        }

        /// <summary>
        /// Validates entry metadata.
        /// </summary>
        /// <param name="entryState">The entry state to use.</param>
        internal void ValidateEntryMetadata(IODataJsonLightReaderEntryState entryState)
        {
            ODataEntry entry = entryState.Entry;
            if (entry != null)
            {
                IEdmEntityType entityType = entryState.EntityType;

                // If the entry in the model has a default stream and if no MR related metadata exists in the entry payload, create an empty MediaResource.
                // Note that for responses the metadata builder will compute the default stream.  For requests we really don't need to add the default stream since the service knows its metadata.
                // We leave this here for now so we don't introduce a breaking change.
                if (!this.ReadingResponse && entityType.HasStream && entry.MediaResource == null)
                {
                    ODataStreamReferenceValue mediaResource = entry.MediaResource;
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    this.SetEntryMediaResource(entryState, mediaResource);
                }

                // By default validate media resource
                // In WCF DS Server mode, validate media resource in JSON Light (here)
                // In WCF DS Client mode, do not validate media resource.

                // Client doesn't validate default media resource against metadata.
                // Server doesn't validate default media resource when reading Verbose JSON against the model
                // So it doesn't fail if the model says the entity should have an MR, but the payload doesn't and vice versa.
                bool validateMediaResource = this.UseDefaultFormatBehavior || this.UseServerFormatBehavior;
                ValidationUtils.ValidateEntryMetadataResource(entry, entityType, this.Model, validateMediaResource);
            }
        }

        /// <summary>
        /// Reads the feed instance annotations for a top-level feed.
        /// </summary>
        /// <param name="feed">The <see cref="ODataFeed"/> to read the instance annotations for.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the top-level scope.</param>
        /// <param name="forFeedStart">true when parsing the instance annotations before the feed property; 
        /// false when parsing the instance annotations after the feed property.</param>
        /// <param name="readAllFeedProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        internal void ReadTopLevelFeedAnnotations(ODataFeedBase feed, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool forFeedStart, bool readAllFeedProperties)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.JsonReader.AssertNotBuffering();

            bool buffering = false;
            try
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    bool foundValueProperty = false;

                    if (!forFeedStart && readAllFeedProperties)
                    {
                        // If this is not called for reading FeedStart and we already scanned ahead and processed all feed properties, we already checked for duplicate property names.
                        // Use an empty duplicate property name checker since this.ParseProperty() read through the same property annotation of instance annotations again. 
                        duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(/*allowDuplicateProperties*/ true, this.JsonLightInputContext.ReadingResponse, !this.JsonLightInputContext.MessageReaderSettings.EnableFullValidation);
                    }

                    this.ProcessProperty(
                        duplicatePropertyNamesChecker,
                        this.ReadTypePropertyAnnotationValue,
                        (propertyParseResult, propertyName) =>
                        {
                            switch (propertyParseResult)
                            {
                                case PropertyParsingResult.ODataInstanceAnnotation:
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    // When we are reading the start of a feed (in scan-ahead mode or not) or when
                                    // we read the end of a feed and not in scan-ahead mode, read the value;
                                    // otherwise skip it.
                                    if (forFeedStart || !readAllFeedProperties)
                                    {
                                        this.ReadAndApplyFeedInstanceAnnotationValue(propertyName, feed, duplicatePropertyNamesChecker);
                                    }
                                    else
                                    {
                                        this.JsonReader.SkipValue();
                                    }

                                    break;

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) == 0)
                                    {
                                        // We found the feed property and are done parsing property annotations;
                                        // When we are in the mode where we scan ahead and read all feed properties
                                        // (for the reordering scenario), we have to start buffering and continue 
                                        // reading. Otherwise we found the feed's data property and are done.
                                        if (readAllFeedProperties)
                                        {
                                            this.JsonReader.StartBuffering();
                                            buffering = true;

                                            this.JsonReader.SkipValue();
                                        }
                                        else
                                        {
                                            foundValueProperty = true;
                                        }
                                    }
                                    else
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyInTopLevelFeed(propertyName, JsonLightConstants.ODataValuePropertyName));
                                    }

                                    break;
                                case PropertyParsingResult.PropertyWithoutValue:
                                    // If we find a property without a value it means that we did not find the feed property (yet)
                                    // but an invalid property annotation
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyAnnotationInTopLevelFeed(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    if (!(feed is ODataFeed))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                                    }

                                    this.ReadMetadataReferencePropertyValue((ODataFeed)feed, propertyName);
                                    break;

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightEntryAndFeedDeserializer_ReadTopLevelFeedAnnotations));
                            }
                        });

                    if (foundValueProperty)
                    {
                        return;
                    }
                }
            }
            finally
            {
                if (buffering)
                {
                    Debug.Assert(readAllFeedProperties, "Expect the reader to be in buffering mode only when scanning to the end.");
                    this.JsonReader.StopBuffering();
                }
            }

            if (forFeedStart && !readAllFeedProperties)
            {
                // We did not find any properties or only instance annotations.
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_ExpectedFeedPropertyNotFound(JsonLightConstants.ODataValuePropertyName));
            }
        }

        /// <summary>
        /// Reads a value of property annotation on the entry level.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>The value of the property annotation.</returns>
        /// <remarks>
        /// This method should read the property annotation value and return a representation of the value which will be later
        /// consumed by the entry reading code.
        /// 
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the entry object
        ///                 JsonNodeType.Property               The next property after the property annotation
        /// </remarks>
        internal object ReadEntryPropertyAnnotationValue(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            string typeName;
            if (this.TryReadODataTypeAnnotationValue(propertyAnnotationName, out typeName))
            {
                return typeName;
            }

            switch (propertyAnnotationName)
            {
                case ODataAnnotationNames.ODataNavigationLinkUrl:  // odata.navigationLinkUrl
                case ODataAnnotationNames.ODataAssociationLinkUrl: // odata.associationLinkUrl
                case ODataAnnotationNames.ODataNextLink:           // odata.nextLink
                case ODataAnnotationNames.ODataMediaEditLink:      // odata.mediaEditLink
                case ODataAnnotationNames.ODataMediaReadLink:      // odata.mediaReadLink
                case ODataAnnotationNames.ODataContext:            // odata.context
                    return this.ReadAndValidateAnnotationStringValueAsUri(propertyAnnotationName);

                case ODataAnnotationNames.ODataCount:              // odata.count
                    return this.ReadAndValidateAnnotationAsLongForIeee754Compatible(propertyAnnotationName);

                case ODataAnnotationNames.ODataMediaETag:          // odata.mediaEtag
                case ODataAnnotationNames.ODataMediaContentType:   // odata.mediaContentType
                    return this.ReadAndValidateAnnotationStringValue(propertyAnnotationName);

                // odata.bind
                case ODataAnnotationNames.ODataBind:
                    // The value of the odata.bind annotation can be either an array of strings or a string (collection or singleton navigation link).
                    // Note that we don't validate that the cardinality of the navigation property matches the payload here, since we don't want to lookup the property twice.
                    // We will validate that later when we consume the value of the property annotation.
                    if (this.JsonReader.NodeType != JsonNodeType.StartArray)
                    {
                        return new ODataEntityReferenceLink
                        {
                            Url = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataBind)
                        };
                    }

                    LinkedList<ODataEntityReferenceLink> entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();

                    // Read over the start array
                    this.JsonReader.Read();
                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        entityReferenceLinks.AddLast(
                            new ODataEntityReferenceLink
                            {
                                Url = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataBind)
                            });
                    }

                    // Read over the end array
                    this.JsonReader.Read();
                    if (entityReferenceLinks.Count == 0)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_EmptyBindArray(ODataAnnotationNames.ODataBind));
                    }

                    return entityReferenceLinks;

                case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded feeds.
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyAnnotationName));
            }
        }

        /// <summary>
        /// Reads instance annotation in the entry object.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="anyPropertyFound">true if a non-annotation property has already been encountered.</param>
        /// <param name="typeAnnotationFound">true if the 'odata.type' annotation has already been encountered, or should have been by now.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the entry being read.</param>
        /// <returns>The value of the annotation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the entry object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        internal object ReadEntryInstanceAnnotation(string annotationName, bool anyPropertyFound, bool typeAnnotationFound, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    if (!typeAnnotationFound)
                    {
                        return this.ReadODataTypeAnnotationValue();
                    }

                    // We already read the odata.type if it was the first property in ReadEntryStart, so any other occurrence means
                    // that it was not the first property.
                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_EntryTypeAnnotationNotFirst);

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty(annotationName));
                    }

                    return this.ReadAnnotationStringValueAsUri(annotationName);

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty(annotationName));
                    }

                    return this.ReadAndValidateAnnotationStringValue(annotationName);

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                case ODataAnnotationNames.ODataMediaEditLink:   // 'odata.mediaEditLink'
                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    return this.ReadAndValidateAnnotationStringValueAsUri(annotationName);

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    return this.ReadAndValidateAnnotationStringValue(annotationName);

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    return this.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, annotationName);
            }
        }

        /// <summary>
        /// Reads instance annotation in the entry object.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="annotationValue">The value of the annotation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the entry object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "The casts aren't actually being done multiple times, since they occur in different cases of the switch statement.")]
        internal void ApplyEntryInstanceAnnotation(IODataJsonLightReaderEntryState entryState, string annotationName, object annotationValue)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            ODataEntry entry = entryState.Entry;
            ODataStreamReferenceValue mediaResource = entry.MediaResource;
            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    entry.TypeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)annotationValue));
                    break;

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (annotationValue == null)
                    {
                        entry.IsTransient = true;
                    }
                    else
                    {
                        entry.Id = (Uri)annotationValue;
                    }

                    break;

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    entry.ETag = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                    entry.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                    entry.ReadLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaEditLink:   // 'odata.mediaEditLink'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ReadLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ContentType = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ETag = (string)annotationValue;
                    break;

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, annotationValue.ToODataValue()));
                    break;
            }

            if (mediaResource != null && entry.MediaResource == null)
            {
                this.SetEntryMediaResource(entryState, mediaResource);
            }
        }

        /// <summary>
        /// Reads the value of a feed annotation (count or next link).
        /// </summary>
        /// <param name="annotationName">The name of the annotation found.</param>
        /// <param name="feed">The feed to read the annotation for; if non-null, the annotation value will be assigned to the feed.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker instance.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the feed object
        ///                 JsonNodeType.Property               The next annotation after the current annotation
        /// </remarks>
        internal void ReadAndApplyFeedInstanceAnnotationValue(string annotationName, ODataFeedBase feed, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(feed != null, "feed != null");

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataCount:
                    feed.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                    break;

                case ODataAnnotationNames.ODataNextLink:
                    feed.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                    break;

                case ODataAnnotationNames.ODataDeltaLink:
                    feed.DeltaLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataDeltaLink);
                    break;

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    object instanceAnnotationValue = this.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, annotationName);
                    feed.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, instanceAnnotationValue.ToODataValue()));
                    break;
            }
        }

        /// <summary>
        /// Reads entry property which doesn't have value, just annotations.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>A reader navigation link info representing the navigation link detected while reading the entry contents; null if no navigation link was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              The end of the entry object.
        ///                 JsonNodeType.Property               The property after the one we're to read.
        /// Post-Condition: JsonNodeType.EndObject              This method doesn't move the reader.
        ///                 JsonNodeType.Property               
        /// </remarks>
        internal ODataJsonLightReaderNavigationLinkInfo ReadEntryPropertyWithoutValue(IODataJsonLightReaderEntryState entryState, string propertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = null;
            IEdmEntityType entityType = entryState.EntityType;
            IEdmProperty edmProperty = ReaderValidationUtils.FindDefinedProperty(propertyName, entityType);
            if (edmProperty != null)
            {
                // Declared property - read it.
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (navigationProperty != null)
                {
                    if (this.ReadingResponse)
                    {
                        // Deferred link
                        navigationLinkInfo = ReadDeferredNavigationLink(entryState, propertyName, navigationProperty);
                    }
                    else
                    {
                        // Entity reference link or links
                        navigationLinkInfo = navigationProperty.Type.IsCollection()
                            ? ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(entryState, navigationProperty, /*isExpanded*/ false)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(entryState, navigationProperty, /*isExpanded*/ false);

                        if (!navigationLinkInfo.HasEntityReferenceLink)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(propertyName, ODataAnnotationNames.ODataBind));
                        }
                    }

                    entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLinkInfo.NavigationLink);
                }
                else
                {
                    IEdmTypeReference propertyTypeReference = edmProperty.Type;
                    if (propertyTypeReference.IsStream())
                    {
                        Debug.Assert(propertyName == edmProperty.Name, "propertyName == edmProperty.Name");
                        ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(entryState, propertyName);
                        AddEntryProperty(entryState, edmProperty.Name, streamPropertyValue);
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithWrongType(propertyName, propertyTypeReference.FullName()));
                    }
                }
            }
            else
            {
                // Undeclared property - we need to run detection alogorithm here.
                navigationLinkInfo = this.ReadUndeclaredProperty(entryState, propertyName, /*propertyWithValue*/ false);
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return navigationLinkInfo;
        }

        /// <summary>
        /// Reads any next link annotation immediately after the end of a feed.
        /// </summary>
        /// <param name="feed">The feed being read.</param>
        /// <param name="expandedNavigationLinkInfo">The information about the expanded link. This must be non-null if we're reading an expanded feed, and must be null if we're reading a top-level feed.</param>
        /// <param name="duplicatePropertyNamesChecker">The top-level duplicate property names checker, if we're reading a top-level feed.</param>
        internal void ReadNextLinkAnnotationAtFeedEnd(
            ODataFeedBase feed,
            ODataJsonLightReaderNavigationLinkInfo expandedNavigationLinkInfo,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(feed != null, "feed != null");

            // Check for annotations on the feed that occur after the feed itself. (Note: the only allowed one is odata.nextLink, and we fail for anything else.)
            // We do this slightly differently depending on whether the feed was an expanded navigation or a top-level feed.
            if (expandedNavigationLinkInfo != null)
            {
                this.ReadExpandedFeedAnnotationsAtFeedEnd(feed, expandedNavigationLinkInfo);
            }
            else
            {
                Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

                // Check for feed instance annotations that appear after the feed.
                bool isReordering = this.JsonReader is ReorderingJsonReader;
                this.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, /*forFeedStart*/false, /*readAllFeedProperties*/isReordering);
            }
        }

        /// <summary>
        /// Reads the information of a deferred link.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationPropertyName">The name of the navigation property for which to read the deferred link.</param>
        /// <param name="navigationProperty">The navigation property for which to read the deferred link. This can be null.</param>
        /// <returns>Returns the navigation link info for the deferred navigation link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNavigationLinkInfo ReadDeferredNavigationLink(IODataJsonLightReaderEntryState entryState, string navigationPropertyName, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(navigationProperty == null || navigationPropertyName == navigationProperty.Name, "navigationProperty == null || navigationPropertyName == navigationProperty.Name");

            ODataNavigationLink navigationLink = new ODataNavigationLink()
            {
                Name = navigationPropertyName,
                IsCollection = navigationProperty == null ? null : (bool?)navigationProperty.Type.IsCollection()
            };

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(navigationLink.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation(navigationLink.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNavigationLinkInfo.CreateDeferredLinkInfo(navigationLink, navigationProperty);
        }

        /// <summary>
        /// Reads expanded entry navigation link.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link.</param>
        /// <returns>The navigation link info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNavigationLinkInfo ReadExpandedEntryNavigationLink(IODataJsonLightReaderEntryState entryState, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNavigationLink navigationLink = new ODataNavigationLink()
            {
                Name = navigationProperty.Name,
                IsCollection = false
            };

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(navigationLink.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataContext:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                            navigationLink.ContextUrl = (Uri)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation(navigationLink.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNavigationLinkInfo.CreateExpandedEntryLinkInfo(navigationLink, navigationProperty);
        }

        /// <summary>
        /// Reads expanded feed navigation link.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link.</param>
        /// <returns>The navigation link info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNavigationLinkInfo ReadExpandedFeedNavigationLink(IODataJsonLightReaderEntryState entryState, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNavigationLink navigationLink = new ODataNavigationLink()
            {
                Name = navigationProperty.Name,
                IsCollection = true
            };

            ODataFeed expandedFeed = new ODataFeed();

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(navigationLink.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            navigationLink.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataNextLink:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.nextLink annotation should have been parsed as a non-null Uri.");
                            expandedFeed.NextPageLink = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataCount:
                            Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                            expandedFeed.Count = (long?)propertyAnnotation.Value;
                            break;
                        case ODataAnnotationNames.ODataContext:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                            navigationLink.ContextUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded feeds.
                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(navigationLink.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNavigationLinkInfo.CreateExpandedFeedLinkInfo(navigationLink, navigationProperty, expandedFeed);
        }

        /// <summary>
        /// Reads entity reference link for a singleton navigation link in request.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNavigationLinkInfo ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(
            IODataJsonLightReaderEntryState entryState,
            IEdmNavigationProperty navigationProperty,
            bool isExpanded)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNavigationLink navigationLink = new ODataNavigationLink()
            {
                Name = navigationProperty.Name,
                IsCollection = false
            };

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(navigationLink.Name);
            ODataEntityReferenceLink entityReferenceLink = null;
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataBind:
                            LinkedList<ODataEntityReferenceLink> entityReferenceLinksList = propertyAnnotation.Value as LinkedList<ODataEntityReferenceLink>;
                            if (entityReferenceLinksList != null)
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_ArrayValueForSingletonBindPropertyAnnotation(navigationLink.Name, ODataAnnotationNames.ODataBind));
                            }

                            if (isExpanded)
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_SingletonNavigationPropertyWithBindingAndValue(navigationLink.Name, ODataAnnotationNames.ODataBind));
                            }

                            Debug.Assert(
                                propertyAnnotation.Value is ODataEntityReferenceLink && propertyAnnotation.Value != null,
                                "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                            entityReferenceLink = (ODataEntityReferenceLink)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(
                                navigationLink.Name,
                                propertyAnnotation.Key,
                                ODataAnnotationNames.ODataBind));
                    }
                }
            }

            return ODataJsonLightReaderNavigationLinkInfo.CreateSingletonEntityReferenceLinkInfo(navigationLink, navigationProperty, entityReferenceLink, isExpanded);
        }

        /// <summary>
        /// Reads entity reference links for a collection navigation link in request.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference links read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNavigationLinkInfo ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(
            IODataJsonLightReaderEntryState entryState,
            IEdmNavigationProperty navigationProperty,
            bool isExpanded)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNavigationLink navigationLink = new ODataNavigationLink()
            {
                Name = navigationProperty.Name,
                IsCollection = true
            };

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(navigationLink.Name);
            LinkedList<ODataEntityReferenceLink> entityReferenceLinksList = null;
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataBind:
                            ODataEntityReferenceLink entityReferenceLink = propertyAnnotation.Value as ODataEntityReferenceLink;
                            if (entityReferenceLink != null)
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StringValueForCollectionBindPropertyAnnotation(navigationLink.Name, ODataAnnotationNames.ODataBind));
                            }

                            Debug.Assert(
                                propertyAnnotation.Value is LinkedList<ODataEntityReferenceLink> && propertyAnnotation.Value != null,
                                "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                            entityReferenceLinksList = (LinkedList<ODataEntityReferenceLink>)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(
                                navigationLink.Name,
                                propertyAnnotation.Key,
                                ODataAnnotationNames.ODataBind));
                    }
                }
            }

            return ODataJsonLightReaderNavigationLinkInfo.CreateCollectionEntityReferenceLinksInfo(navigationLink, navigationProperty, entityReferenceLinksList, isExpanded);
        }

        /// <summary>
        /// Adds a new property to an entry.
        /// </summary>
        /// <param name="entryState">The entry state for the entry to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property to add.</param>
        private static void AddEntryProperty(IODataJsonLightReaderEntryState entryState, string propertyName, object propertyValue)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            ODataProperty property = new ODataProperty { Name = propertyName, Value = propertyValue };
            var propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetCustomPropertyAnnotations(propertyName);
            if (propertyAnnotations != null)
            {
                foreach (var annotation in propertyAnnotations)
                {
                    if (annotation.Value != null)
                    {
                        // annotation.Value == null indicates that this annotation should be skipped.
                        property.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Key, annotation.Value.ToODataValue()));
                    }
                }
            }

            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            ODataEntry entry = entryState.Entry;
            Debug.Assert(entry != null, "entry != null");
            entry.Properties = entry.Properties.ConcatToReadOnlyEnumerable("Properties", property);
        }

        /// <summary>
        /// Checks if there is a next link annotation immediately after an expanded feed, and reads and stores it if there is one. 
        /// We fail here if we encounter any other property annotation for the expanded navigation (since these should come before the property itself).
        /// </summary>
        /// <param name="feed">The feed that was just read.</param>
        /// <param name="expandedNavigationLinkInfo">The information for the current expanded navigation link being read.</param>
        private void ReadExpandedFeedAnnotationsAtFeedEnd(ODataFeedBase feed, ODataJsonLightReaderNavigationLinkInfo expandedNavigationLinkInfo)
        {
            Debug.Assert(expandedNavigationLinkInfo != null, "expandedNavigationLinkInfo != null");
            Debug.Assert(expandedNavigationLinkInfo.NavigationLink.IsCollection == true, "Only collection navigation properties can have feed content.");

            // Look at the next property in the owning entry, if it's a property annotation for the expanded navigation link info property, read it.
            string propertyName, annotationName;
            while (this.JsonReader.NodeType == JsonNodeType.Property &&
                   TryParsePropertyAnnotation(this.JsonReader.GetPropertyName(), out propertyName, out annotationName) &&
                   string.CompareOrdinal(propertyName, expandedNavigationLinkInfo.NavigationLink.Name) == 0)
            {
                if (!this.ReadingResponse)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation(propertyName, annotationName));
                }

                // Read over the property name.
                this.JsonReader.Read();

                switch (this.CompleteSimplifiedODataAnnotation(annotationName))
                {
                    case ODataAnnotationNames.ODataNextLink:
                        if (feed.NextPageLink != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation(ODataAnnotationNames.ODataNextLink, expandedNavigationLinkInfo.NavigationLink.Name));
                        }

                        // Read the property value.
                        feed.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        if (feed.Count != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation(ODataAnnotationNames.ODataCount, expandedNavigationLinkInfo.NavigationLink.Name));
                        }

                        // Read the property value.
                        feed.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded feeds.
                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed(annotationName, expandedNavigationLinkInfo.NavigationLink.Name));
                }
            }
        }

        /// <summary>
        /// Sets specified media resource on an entry and hooks up metadata builder.
        /// </summary>
        /// <param name="entryState">The entry state to use.</param>
        /// <param name="mediaResource">The media resource to set.</param>
        private void SetEntryMediaResource(IODataJsonLightReaderEntryState entryState, ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(mediaResource != null, "mediaResource != null");
            ODataEntry entry = entryState.Entry;
            Debug.Assert(entry != null, "entry != null");

            ODataEntityMetadataBuilder builder = this.MetadataContext.GetEntityMetadataBuilderForReader(entryState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
            mediaResource.SetMetadataBuilder(builder, /*propertyName*/ null);
            entry.MediaResource = mediaResource;
        }

        /// <summary>
        /// Reads entry property (which is neither instance nor property annotation) which has a value.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>A reader navigation link info representing the navigation link detected while reading the entry contents; null if no navigation link was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the entry object
        ///                 JsonNodeType.Property               The next property after the property
        ///                 JsonNodeType.StartObject            Expanded entry
        ///                 JsonNodeType.StartArray             Expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry
        /// </remarks>
        private ODataJsonLightReaderNavigationLinkInfo ReadEntryPropertyWithValue(IODataJsonLightReaderEntryState entryState, string propertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = null;
            IEdmEntityType entityType = entryState.EntityType;
            IEdmProperty edmProperty = ReaderValidationUtils.FindDefinedProperty(propertyName, entityType);
            if (edmProperty != null)
            {
                // Declared property - read it.
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (navigationProperty != null)
                {
                    // Expanded link
                    bool isCollection = navigationProperty.Type.IsCollection();
                    this.ValidateExpandedNavigationLinkPropertyValue(isCollection, navigationProperty.Name);
                    if (isCollection)
                    {
                        navigationLinkInfo = this.ReadingResponse
                            ? ReadExpandedFeedNavigationLink(entryState, navigationProperty)
                            : ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(entryState, navigationProperty, /*isExpanded*/ true);
                    }
                    else
                    {
                        navigationLinkInfo = this.ReadingResponse
                            ? ReadExpandedEntryNavigationLink(entryState, navigationProperty)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(entryState, navigationProperty, /*isExpanded*/ true);
                    }

                    entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLinkInfo.NavigationLink);
                }
                else
                {
                    IEdmTypeReference propertyTypeReference = edmProperty.Type;
                    if (propertyTypeReference.IsStream())
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue(propertyName));
                    }

                    // NOTE: we currently do not check whether the property should be skipped
                    //       here because this can only happen for navigation properties and open properties.
                    this.ReadEntryDataProperty(entryState, edmProperty, ValidateDataPropertyTypeNameAnnotation(entryState.DuplicatePropertyNamesChecker, propertyName));
                }
            }
            else
            {
                // Undeclared property - we need to run detection alogorithm here.
                navigationLinkInfo = this.ReadUndeclaredProperty(entryState, propertyName, /*propertyWithValue*/ true);

                // Note that if navigation link is returned it's already validated, so we just report it here.
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue);
            return navigationLinkInfo;
        }

        /// <summary>
        /// Read an entry-level data property and check its version compliance. 
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="edmProperty">The EDM property of the property being read, or null if the property is an open property.</param>
        /// <param name="propertyTypeName">The type name specified for the property in property annotation, or null if no such type name is available.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        private void ReadEntryDataProperty(IODataJsonLightReaderEntryState entryState, IEdmProperty edmProperty, string propertyTypeName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");
            this.JsonReader.AssertNotBuffering();

            // EdmLib bridge marks all key properties as non-nullable, but Astoria allows them to be nullable.
            // If the property has an annotation to ignore null values, we need to omit the property in requests.
            ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse
                ? ODataNullValueBehaviorKind.Default
                : this.Model.NullValueReadBehaviorKind(edmProperty);
            object propertyValue = this.ReadNonEntityValue(
                propertyTypeName,
                edmProperty.Type,
                /*duplicatePropertyNamesChecker*/ null,
                /*collectionValidator*/ null,
                nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                /*isTopLevelPropertyValue*/ false,
                /*insideComplexValue*/ false,
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
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        private void ReadOpenProperty(IODataJsonLightReaderEntryState entryState, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_OpenPropertyWithoutValue(propertyName));
            }

            string propertyTypeName = ValidateDataPropertyTypeNameAnnotation(entryState.DuplicatePropertyNamesChecker, propertyName);

            object propertyValue = this.ReadNonEntityValue(
                propertyTypeName,
                /*expectedValueTypeReference*/ null,
                /*duplicatePropertyNamesChecker*/ null,
                /*collectionValidator*/ null,
                /*validateNullValue*/ true,
                /*isTopLevelPropertyValue*/ false,
                /*insideComplexValue*/ false,
                propertyName,
                true);

            ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue);
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
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue:  propertyWithValue is true and the reader is positioned on the first node of the property value.
        ///                 JsonNodeType.StartObject:
        ///                 JsonNodeType.StartArray:
        ///                 JsonNodeType.Property:        propertyWithValue is false and the reader is positioned on the node after the property.
        ///                 JsonNodeType.EndObject:
        /// Post-Condition: JsonNodeType.Property:    the next property of the entry
        ///                 JsonNodeType.EndObject:   the end-object node of the entry
        /// </remarks>
        /// <returns>A navigation link info instance if the propery read is a navigation link which should be reported to the caller.
        /// Otherwise null if the property was either ignored or read and added to the list of properties on the entry.</returns>
        private ODataJsonLightReaderNavigationLinkInfo ReadUndeclaredProperty(IODataJsonLightReaderEntryState entryState, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
#if DEBUG
            if (propertyWithValue)
            {
                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
            }
            else
            {
                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }
#endif
            // to preserve error messages, if we're not going to report undeclared links, then read open properties first.
            if (!this.MessageReaderSettings.ReportUndeclaredLinkProperties)
            {
                if (entryState.EntityType.IsOpen)
                {
                    // Open property - read it as such.
                    this.ReadOpenProperty(entryState, propertyName, propertyWithValue);
                    return null;
                }
            }

            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            Dictionary<string, object> odataPropertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(propertyName);
            if (odataPropertyAnnotations != null)
            {
                object propertyAnnotationValue;

                // If the property has 'odata.navigationLink' or 'odata.associationLink' annotation, read it as a navigation property
                if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataNavigationLinkUrl, out propertyAnnotationValue) ||
                    odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataAssociationLinkUrl, out propertyAnnotationValue))
                {
                    // Undeclared link properties are reported if the right flag is used, otherwise we need to fail.
                    if (!this.MessageReaderSettings.ReportUndeclaredLinkProperties)
                    {
                        throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.FullTypeName()));
                    }

                    // Read it as a deferred link - we never read the expanded content.
                    ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = ReadDeferredNavigationLink(entryState, propertyName, /*navigationProperty*/ null);
                    entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLinkInfo.NavigationLink);

                    // If the property is expanded, ignore the content if we're asked to do so.
                    if (propertyWithValue)
                    {
                        if (!this.MessageReaderSettings.IgnoreUndeclaredValueProperties)
                        {
                            throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.FullTypeName()));
                        }

                        this.ValidateExpandedNavigationLinkPropertyValue(null, propertyName);

                        // Since we marked the navigation link as deffered the reader will not try to read its content
                        // instead it will behave as if it was a real deferred link (without a property value).
                        // So skip the value here to move to the next property in the payload, which will look exactly the same
                        // as if the navigation link was deferred.
                        this.JsonReader.SkipValue();
                    }

                    return navigationLinkInfo;
                }

                // If the property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' annotation, read it as a stream property
                if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaEditLink, out propertyAnnotationValue) ||
                    odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaReadLink, out propertyAnnotationValue) ||
                    odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaContentType, out propertyAnnotationValue) ||
                    odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaETag, out propertyAnnotationValue))
                {
                    // Undeclared link properties are reported if the right flag is used, otherwise we need to fail.
                    if (!this.MessageReaderSettings.ReportUndeclaredLinkProperties)
                    {
                        throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.FullTypeName()));
                    }

                    // Stream properties can't have a value
                    if (propertyWithValue)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue(propertyName));
                    }

                    ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(entryState, propertyName);
                    AddEntryProperty(entryState, propertyName, streamPropertyValue);
                    return null;
                }
            }

            if (entryState.EntityType.IsOpen)
            {
                // Open property - read it as such.
                this.ReadOpenProperty(entryState, propertyName, propertyWithValue);
                return null;
            }

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType(propertyName));
            }

            // Property with value can only be ignored if we're asked to do so.
            if (!this.MessageReaderSettings.IgnoreUndeclaredValueProperties)
            {
                throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, entryState.EntityType.FullTypeName()));
            }

            // Validate that the property doesn't have unrecognized annotations
            // We ignore the type name since we might not have the full model and thus might not be able to resolve it correctly.
            ValidateDataPropertyTypeNameAnnotation(entryState.DuplicatePropertyNamesChecker, propertyName);

            // Read it as such.
            this.ReadOpenProperty(entryState, propertyName, propertyWithValue);
            return null;
        }

        /// <summary>
        /// Reads a stream property value from the property annotations.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="streamPropertyName">The name of the stream property to read the value for.</param>
        /// <returns>The newly created stream reference value.</returns>
        private ODataStreamReferenceValue ReadStreamPropertyValue(IODataJsonLightReaderEntryState entryState, string streamPropertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(!string.IsNullOrEmpty(streamPropertyName), "!string.IsNullOrEmpty(streamPropertyName)");

            // Fail on stream properties in requests as they cannot appear there.
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyInRequest);
            }

            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();

            Dictionary<string, object> propertyAnnotations = entryState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(streamPropertyName);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataMediaEditLink:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaEditLink annotation should have been parsed as a non-null Uri.");
                            streamReferenceValue.EditLink = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataMediaReadLink:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaReadLink annotation should have been parsed as a non-null Uri.");
                            streamReferenceValue.ReadLink = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataMediaETag:
                            Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaEtag annotation should have been parsed as a non-null string.");
                            streamReferenceValue.ETag = (string)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataMediaContentType:
                            Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaContentType annotation should have been parsed as a non-null string.");
                            streamReferenceValue.ContentType = (string)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation(streamPropertyName, propertyAnnotation.Key));
                    }
                }
            }

            ODataEntityMetadataBuilder builder = this.MetadataContext.GetEntityMetadataBuilderForReader(entryState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);

            // Note that we set the metadata builder even when streamProperty is null, which is the case when the stream property is undeclared.
            // For undeclared stream properties, we will apply conventional metadata evaluation just as declared stream properties.
            streamReferenceValue.SetMetadataBuilder(builder, streamPropertyName);

            return streamReferenceValue;
        }

        /// <summary>
        /// Reads one operation for the entry being read.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:   first node of the operation value.
        /// Post-Condition: JsonNodeType.Property:      the property after the current operation being read when there is one target for the operation.
        ///                 JsonNodeType.StartObject:   the first node of the next operation value when there are multiple targets for the operation.
        ///                 JsonNodeType.EndArray:      the end-array of the operation values when there are multiple target for the operation.
        /// </remarks>
        private void ReadSingleOperationValue(IODataJsonOperationsDeserializerContext readerContext, IODataJsonLightReaderEntryState entryState, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(readerContext != null, "readerContext != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonLightReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(metadataReferencePropertyName, readerContext.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            readerContext.JsonReader.ReadStartObject();

            var operation = this.CreateODataOperationAndAddToEntry(readerContext, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
                {
                    readerContext.JsonReader.ReadPropertyName();
                    readerContext.JsonReader.SkipValue();
                }

                readerContext.JsonReader.ReadEndObject();
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(readerContext.JsonReader.ReadPropertyName());
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = readerContext.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // skip over all unknown properties and read the next property or 
                        // the end of the metadata for the current propertyName
                        readerContext.JsonReader.SkipValue();
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_OperationMissingTargetProperty(metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            readerContext.JsonReader.ReadEndObject();

            // Sets the metadata builder to evaluate by convention any operation property that's not on the wire.
            // Note we must only set this after the operation is read from the wire since we lose the ability to tell
            // what was on the wire and what is being dynamically computed.
            this.SetMetadataBuilder(entryState, operation);
        }

        /// <summary>
        /// Reads one operation for the feed being read.
        /// </summary>
        /// <param name="feed">The feed to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        private void ReadSingleOperationValue(ODataFeed feed, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonLightReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(metadataReferencePropertyName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            this.JsonReader.ReadStartObject();

            var operation = this.CreateODataOperationAndAddToFeed(feed, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    this.JsonReader.ReadPropertyName();
                    this.JsonReader.SkipValue();
                }

                this.JsonReader.ReadEndObject();
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(this.JsonReader.ReadPropertyName());
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = this.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // skip over all unknown properties and read the next property or 
                        // the end of the metadata for the current propertyName
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_OperationMissingTargetProperty(metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            this.JsonReader.ReadEndObject();
        }

        /// <summary>
        /// Sets the metadata builder for the operation.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="operation">The operation to set the metadata builder on.</param>
        private void SetMetadataBuilder(IODataJsonLightReaderEntryState entryState, ODataOperation operation)
        {
            ODataEntityMetadataBuilder builder = this.MetadataContext.GetEntityMetadataBuilderForReader(entryState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
            operation.SetMetadataBuilder(builder, this.ContextUriParseResult.MetadataDocumentUri);
        }

        /// <summary>
        /// Creates a new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.</returns>
        private ODataOperation CreateODataOperationAndAddToEntry(IODataJsonOperationsDeserializerContext readerContext, string metadataReferencePropertyName)
        {
            string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonLightInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            var operation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

            if (isAction)
            {
                readerContext.AddActionToEntry((ODataAction)operation);
            }
            else
            {
                readerContext.AddFunctionToEntry((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Creates a new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.
        /// </summary>
        /// <param name="feed">The feed to add the action or function .</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.</returns>
        private ODataOperation CreateODataOperationAndAddToFeed(ODataFeed feed, string metadataReferencePropertyName)
        {
            string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonLightInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            var operation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

            if (isAction)
            {
                feed.AddAction((ODataAction)operation);
            }
            else
            {
                feed.AddFunction((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Read the metadata reference property value for the entry being read.
        /// </summary>
        /// <param name="entryState">The state of the reader for entry to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:      first node of the metadata reference property's value. Currently 
        ///                                             actions and functions are the only supported metadata reference property,
        ///                                             we will throw if this is not a start object or start array node.
        /// Post-Condition: JsonNodeType.Property:      the property after the annotation value
        ///                 JsonNodeType.EndObject:     the end-object of the entry
        /// </remarks>
        private void ReadMetadataReferencePropertyValue(IODataJsonLightReaderEntryState entryState, string metadataReferencePropertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.Entry != null, "entryState.Entry != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) > -1, "metadataReferencePropertyName.IndexOf(JsonLightConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            IODataJsonOperationsDeserializerContext readerContext = new OperationsDeserializerContext(entryState.Entry, this);

            bool insideArray = false;
            if (readerContext.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                readerContext.JsonReader.ReadStartArray();
                insideArray = true;
            }

            do
            {
                this.ReadSingleOperationValue(readerContext, entryState, metadataReferencePropertyName, insideArray);
            }
            while (insideArray && readerContext.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                readerContext.JsonReader.ReadEndArray();
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Read the metadata reference property value for the feed being read.
        /// </summary>
        /// <param name="feed">The feed to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        private void ReadMetadataReferencePropertyValue(ODataFeed feed, string metadataReferencePropertyName)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) > -1, "metadataReferencePropertyName.IndexOf(JsonLightConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            bool insideArray = false;
            if (this.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                this.JsonReader.ReadStartArray();
                insideArray = true;
            }

            do
            {
                this.ReadSingleOperationValue(feed, metadataReferencePropertyName, insideArray);
            }
            while (insideArray && this.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                this.JsonReader.ReadEndArray();
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Validates that we can read metadata reference property.
        /// </summary>
        private void ValidateCanReadMetadataReferenceProperty()
        {
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_MetadataReferencePropertyInRequest);
            }
        }

        /// <summary>
        /// Validates that the value of a JSON property can represent expanded navigation link.
        /// </summary>
        /// <param name="isCollection">true if the property is entity set reference property; false for a resource reference property, null if unknown.</param>
        /// <param name="propertyName">Name for the navigation property, used in error message only.</param>
        private void ValidateExpandedNavigationLinkPropertyValue(bool? isCollection, string propertyName)
        {
            // an expanded link with entry requires a StartObject node here;
            // an expanded link with feed requires a StartArray node here;
            // an expanded link with null entry requires a primitive null node here;
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                if (isCollection == false)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(nodeType, propertyName));
                }
            }
            else if ((nodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null) || nodeType == JsonNodeType.StartObject)
            {
                // Expanded entry (null or non-null)
                if (isCollection == true)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue(nodeType, propertyName));
                }
            }
            else
            {
                Debug.Assert(nodeType == JsonNodeType.PrimitiveValue, "nodeType == JsonNodeType.PrimitiveValue");
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue(propertyName));
            }
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
            private ODataJsonLightEntryAndFeedDeserializer jsonLightEntryAndFeedDeserializer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="entry">The entry to add operations to.</param>
            /// <param name="jsonLightEntryAndFeedDeserializer">The deserializer to use.</param>
            public OperationsDeserializerContext(ODataEntry entry, ODataJsonLightEntryAndFeedDeserializer jsonLightEntryAndFeedDeserializer)
            {
                Debug.Assert(entry != null, "entry != null");
                Debug.Assert(jsonLightEntryAndFeedDeserializer != null, "jsonLightEntryAndFeedDeserializer != null");

                this.entry = entry;
                this.jsonLightEntryAndFeedDeserializer = jsonLightEntryAndFeedDeserializer;
            }

            /// <summary>
            /// The JSON reader to read the operations value from.
            /// </summary>
            public JsonReader JsonReader
            {
                get
                {
                    return this.jsonLightEntryAndFeedDeserializer.JsonReader;
                }
            }

            /// <summary>
            /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
            /// </summary>
            /// <param name="uriFromPayload">The URI string from the payload to process.</param>
            /// <returns>An absolute URI to report.</returns>
            public Uri ProcessUriFromPayload(string uriFromPayload)
            {
                return this.jsonLightEntryAndFeedDeserializer.ProcessUriFromPayload(uriFromPayload);
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
