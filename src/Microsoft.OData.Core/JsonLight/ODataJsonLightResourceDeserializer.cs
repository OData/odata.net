//---------------------------------------------------------------------
// <copyright file="ODataJsonLightResourceDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for entries and resource sets.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
    internal sealed class ODataJsonLightResourceDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightResourceDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Reads the start of the JSON array for the content of the resource set.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:    The start of the resource set property array; this method will fail if the node is anything else.
        /// Post-Condition: JsonNodeType.StartObject:   The first item in the resource set
        ///                 JsonNodeType.EndArray:      The end of the resource set
        /// </remarks>
        internal void ReadResourceSetContentStart()
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
        /// Reads the end of the array containing the resource set content.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndArray
        /// Post-Condition: JsonNodeType.Property   if the resource set is part of an expanded nested resource info and there are more properties in the object
        ///                 JsonNodeType.EndObject  if the resource set is a top-level resource set or the expanded nested resource info is the last property of the payload
        /// </remarks>
        internal void ReadResourceSetContentEnd()
        {
            this.AssertJsonCondition(JsonNodeType.EndArray);
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.ReadEndArray();

            this.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);
        }

        /// <summary>
        /// Reads the resource type name annotation (odata.type)
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///                 
        /// This method fills the ODataResource.TypeName property if the type name is found in the payload.
        /// </remarks>
        internal void ReadResourceTypeName(IODataJsonLightReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.type property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType, propertyName) == 0
                    || this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataTypePropertyName, propertyName))
                {
                    Debug.Assert(resourceState.Resource.TypeName == null, "type name should not have already been set");

                    // Read over the property to move to its value.
                    this.JsonReader.Read();

                    // Read the annotation value.
                    resourceState.Resource.TypeName = this.ReadODataTypeAnnotationValue();
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the content of a resource until a nested resource info is detected.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        ///                 JsonNodeType.Property               If we've read a deferred link (this is the property after the deferred link)
        ///                 JsonNodeType.StartObject            Expanded resource
        ///                 JsonNodeType.StartArray             Expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null
        /// </remarks>
        internal ODataJsonLightReaderNestedResourceInfo ReadResourceContent(IODataJsonLightReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.ResourceType != null && this.Model.IsUserModel(), "A non-null resource type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            Debug.Assert(resourceState.ResourceType != null, "In JSON we must always have an structured type when reading resource.");

            // Figure out whether we have more properties for this resource
            // read all the properties until we hit a link
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    resourceState.DuplicatePropertyNamesChecker,
                    this.ReadEntryPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                object value = this.ReadEntryInstanceAnnotation(propertyName, resourceState.AnyPropertyFound, /*typeAnnotationFound*/ true, resourceState.DuplicatePropertyNamesChecker);
                                this.ApplyEntryInstanceAnnotation(resourceState, propertyName, value);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithoutValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                this.ReadMetadataReferencePropertyValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.EndOfObject:
                                break;
                        }
                    });

                if (readerNestedResourceInfo != null)
                {
                    // we found a nested resource info
                    // stop parsing the resource content and return to the caller
                    break;
                }

                Debug.Assert(
                    this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "After reading a property the reader should be positioned on another property or have hit the end of the object.");
            }

            this.JsonReader.AssertNotBuffering();

            // The reader can be either on
            //  - StartObject - if it's an expanded resource
            //  - StartArray - if it's an expanded resource set
            //  - Property - if it's a deferred link
            //  - PrimitiveValue (null) - if it's an expanded null resource
            //  - EndObject - end of the resource
            Debug.Assert(
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.Property ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Property or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Validates resource metadata.
        /// </summary>
        /// <param name="resourceState">The resource state to use.</param>
        internal void ValidateMediaEntity(IODataJsonLightReaderResourceState resourceState)
        {
            ODataResource resource = resourceState.Resource;
            if (resource != null)
            {
                IEdmEntityType entityType = resourceState.ResourceType as IEdmEntityType;
                if (entityType != null)
                {
                    // If the entity in the model has a default stream and if no MR related metadata exists in the resource payload, create an empty MediaResource.
                    // Note that for responses the metadata builder will compute the default stream.  For requests we really don't need to add the default stream since the service knows its metadata.
                    // We leave this here for now so we don't introduce a breaking change.
                    if (!this.ReadingResponse && entityType.HasStream && resource.MediaResource == null)
                    {
                        ODataStreamReferenceValue mediaResource = resource.MediaResource;
                        ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                        this.SetEntryMediaResource(resourceState, mediaResource);
                    }

                    ValidationUtils.ValidateMediaResource(resource, entityType, this.Model, this.MessageReaderSettings.EnableFullValidation);
                }
            }
        }

        /// <summary>
        /// Reads the resource set instance annotations for a top-level resource set.
        /// </summary>
        /// <param name="resourceSet">The <see cref="ODataResourceSet"/> to read the instance annotations for.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the top-level scope.</param>
        /// <param name="forResourceSetStart">true when parsing the instance annotations before the resource set property; 
        /// false when parsing the instance annotations after the resource set property.</param>
        /// <param name="readAllResourceSetProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        internal void ReadTopLevelResourceSetAnnotations(ODataResourceSetBase resourceSet, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool forResourceSetStart, bool readAllResourceSetProperties)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.JsonReader.AssertNotBuffering();

            bool buffering = false;
            try
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    bool foundValueProperty = false;

                    if (!forResourceSetStart && readAllResourceSetProperties)
                    {
                        // If this is not called for reading ResourceSetStart and we already scanned ahead and processed all resource set properties, we already checked for duplicate property names.
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
                                    // When we are reading the start of a resource set (in scan-ahead mode or not) or when
                                    // we read the end of a resource set and not in scan-ahead mode, read the value;
                                    // otherwise skip it.
                                    if (forResourceSetStart || !readAllResourceSetProperties)
                                    {
                                        this.ReadAndApplyResourceSetInstanceAnnotationValue(propertyName, resourceSet, duplicatePropertyNamesChecker);
                                    }
                                    else
                                    {
                                        this.JsonReader.SkipValue();
                                    }

                                    break;

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) == 0)
                                    {
                                        // We found the resource set property and are done parsing property annotations;
                                        // When we are in the mode where we scan ahead and read all resource set properties
                                        // (for the reordering scenario), we have to start buffering and continue 
                                        // reading. Otherwise we found the resourceSet's data property and are done.
                                        if (readAllResourceSetProperties)
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
                                    // If we find a property without a value it means that we did not find the resource set property (yet)
                                    // but an invalid property annotation
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyAnnotationInTopLevelFeed(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    if (!(resourceSet is ODataResourceSet))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                                    }

                                    this.ReadMetadataReferencePropertyValue((ODataResourceSet)resourceSet, propertyName);
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
                    Debug.Assert(readAllResourceSetProperties, "Expect the reader to be in buffering mode only when scanning to the end.");
                    this.JsonReader.StopBuffering();
                }
            }

            if (forResourceSetStart && !readAllResourceSetProperties)
            {
                // We did not find any properties or only instance annotations.
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_ExpectedFeedPropertyNotFound(JsonLightConstants.ODataValuePropertyName));
            }
        }

        /// <summary>
        /// Reads a value of property annotation on the resource level.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>The value of the property annotation.</returns>
        /// <remarks>
        /// This method should read the property annotation value and return a representation of the value which will be later
        /// consumed by the resource reading code.
        /// 
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
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
                    // The value of the odata.bind annotation can be either an array of strings or a string (collection or singleton nested resource info).
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

                case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyAnnotationName));
            }
        }

        /// <summary>
        /// Reads instance annotation in the resource object.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="anyPropertyFound">true if a non-annotation property has already been encountered.</param>
        /// <param name="typeAnnotationFound">true if the 'odata.type' annotation has already been encountered, or should have been by now.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the resource being read.</param>
        /// <returns>The value of the annotation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
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
        /// Reads instance annotation in the resource object.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="annotationValue">The value of the annotation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "The casts aren't actually being done multiple times, since they occur in different cases of the switch statement.")]
        internal void ApplyEntryInstanceAnnotation(IODataJsonLightReaderResourceState resourceState, string annotationName, object annotationValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            ODataResource resource = resourceState.Resource;
            ODataStreamReferenceValue mediaResource = resource.MediaResource;
            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    resource.TypeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)annotationValue));
                    break;

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (annotationValue == null)
                    {
                        resource.IsTransient = true;
                    }
                    else
                    {
                        resource.Id = (Uri)annotationValue;
                    }

                    break;

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    resource.ETag = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                    resource.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                    resource.ReadLink = (Uri)annotationValue;
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
                    resource.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, annotationValue.ToODataValue()));
                    break;
            }

            if (mediaResource != null && resource.MediaResource == null)
            {
                this.SetEntryMediaResource(resourceState, mediaResource);
            }
        }

        /// <summary>
        /// Reads the value of a resource set annotation (count or next link).
        /// </summary>
        /// <param name="annotationName">The name of the annotation found.</param>
        /// <param name="resourceSet">The resource set to read the annotation for; if non-null, the annotation value will be assigned to the resource set.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker instance.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource set object
        ///                 JsonNodeType.Property               The next annotation after the current annotation
        /// </remarks>
        internal void ReadAndApplyResourceSetInstanceAnnotationValue(string annotationName, ODataResourceSetBase resourceSet, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataCount:
                    resourceSet.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                    break;

                case ODataAnnotationNames.ODataNextLink:
                    resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                    break;

                case ODataAnnotationNames.ODataDeltaLink:
                    resourceSet.DeltaLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataDeltaLink);
                    break;
                case ODataAnnotationNames.ODataType:

                    // TODO: skip the odata.type;
                    this.ReadAndValidateAnnotationStringValue(ODataAnnotationNames.ODataType);
                    break;
                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    object instanceAnnotationValue = this.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, annotationName);
                    resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, instanceAnnotationValue.ToODataValue()));
                    break;
            }
        }

        /// <summary>
        /// Reads resource property which doesn't have value, just annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              The end of the resource object.
        ///                 JsonNodeType.Property               The property after the one we're to read.
        /// Post-Condition: JsonNodeType.EndObject              This method doesn't move the reader.
        ///                 JsonNodeType.Property               
        /// </remarks>
        internal ODataJsonLightReaderNestedResourceInfo ReadPropertyWithoutValue(IODataJsonLightReaderResourceState resourceState, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = ReaderValidationUtils.FindDefinedProperty(propertyName, resourceType);
            if (edmProperty != null)
            {
                // Declared property - read it.
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (navigationProperty != null)
                {
                    if (this.ReadingResponse)
                    {
                        // Deferred link
                        readerNestedResourceInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, navigationProperty);
                    }
                    else
                    {
                        // Entity reference link or links
                        readerNestedResourceInfo = navigationProperty.Type.IsCollection()
                            ? ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, /*isExpanded*/ false)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, /*isExpanded*/ false);

                        if (!readerNestedResourceInfo.HasEntityReferenceLink)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(propertyName, ODataAnnotationNames.ODataBind));
                        }
                    }

                    resourceState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                }
                else
                {
                    IEdmTypeReference propertyTypeReference = edmProperty.Type;
                    if (propertyTypeReference.IsStream())
                    {
                        Debug.Assert(propertyName == edmProperty.Name, "propertyName == edmProperty.Name");
                        ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                        AddResourceProperty(resourceState, edmProperty.Name, streamPropertyValue);
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
                readerNestedResourceInfo = this.ReadUndeclaredProperty(resourceState, propertyName, /*propertyWithValue*/ false);
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Reads any next link annotation immediately after the end of a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set being read.</param>
        /// <param name="expandedNestedResourceInfo">The information about the expanded link. This must be non-null if we're reading an expanded resource set, and must be null if we're reading a top-level resource set.</param>
        /// <param name="duplicatePropertyNamesChecker">The top-level duplicate property names checker, if we're reading a top-level resource set.</param>
        internal void ReadNextLinkAnnotationAtResourceSetEnd(
            ODataResourceSetBase resourceSet,
            ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // Check for annotations on the resource set that occur after the resource set itself. (Note: the only allowed one is odata.nextLink, and we fail for anything else.)
            // We do this slightly differently depending on whether the resource set was an expanded navigation or a top-level resource set.
            if (expandedNestedResourceInfo != null)
            {
                this.ReadExpandedResourceSetAnnotationsAtResourceSetEnd(resourceSet, expandedNestedResourceInfo);
            }
            else
            {
                Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

                // Check for resource set instance annotations that appear after the resource set.
                bool isReordering = this.JsonReader is ReorderingJsonReader;
                this.ReadTopLevelResourceSetAnnotations(resourceSet, duplicatePropertyNamesChecker, /*forResourceSetStart*/false, /*readAllResourceSetProperties*/isReordering);
            }
        }

        /// <summary>
        /// Reads the information of a deferred link.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationPropertyName">The name of the navigation property for which to read the deferred link.</param>
        /// <param name="navigationProperty">The navigation property for which to read the deferred link. This can be null.</param>
        /// <returns>Returns the nested resource info for the deferred nested resource info read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadDeferredNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, string navigationPropertyName, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(navigationProperty == null || navigationPropertyName == navigationProperty.Name, "navigationProperty == null || navigationPropertyName == navigationProperty.Name");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationPropertyName,
                IsCollection = navigationProperty == null ? null : (bool?)navigationProperty.Type.IsCollection()
            };

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation(nestedResourceInfo.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateDeferredLinkInfo(nestedResourceInfo, navigationProperty);
        }

        /// <summary>
        /// Reads expanded resource nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadExpandedResourceNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationProperty.Name,
                IsCollection = false
            };

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataContext:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.ContextUrl = (Uri)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation(nestedResourceInfo.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, navigationProperty);
        }

        /// <summary>
        /// Reads expanded resource set nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadExpandedResourceSetNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationProperty.Name,
                IsCollection = true
            };

            ODataResourceSet expandedResourceSet = new ODataResourceSet();

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNavigationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataAssociationLinkUrl:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataNextLink:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.nextLink annotation should have been parsed as a non-null Uri.");
                            expandedResourceSet.NextPageLink = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataCount:
                            Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                            expandedResourceSet.Count = (long?)propertyAnnotation.Value;
                            break;
                        case ODataAnnotationNames.ODataContext:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                            nestedResourceInfo.ContextUrl = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(nestedResourceInfo.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, expandedResourceSet);
        }

        /// <summary>
        /// Reads non-expanded resource nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="complexProperty">The complex property for which to read the nested resource info.</param>
        /// <returns>The nested resource info for the complex property to read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadNonExpandedResourceNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, IEdmStructuralProperty complexProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(complexProperty != null, "complexProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = complexProperty.Name,
                IsCollection = false
            };

            // Check the odata.type annotation for the complex property, it should show inside the complex object.
            if (ValidateDataPropertyTypeNameAnnotation(resourceState.DuplicatePropertyNamesChecker, nestedResourceInfo.Name) != null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation(ODataAnnotationNames.ODataType));
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, complexProperty);
        }

        /// <summary>
        /// Reads non-expanded nested resource set.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="collectionProperty">The collection of complex property for which to read the nested resource info.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadNonExpandedResourceSetNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, IEdmStructuralProperty collectionProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(collectionProperty != null, "collectionProperty != null");
            Debug.Assert(collectionProperty.ToStructuredType().IsODataComplexTypeKind(), "The item in the collection property should be complex instance");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = collectionProperty.Name,
                IsCollection = true
            };

            ODataResourceSet expandedResourceSet = new ODataResourceSet();

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
            if (propertyAnnotations != null)
            {
                foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                {
                    switch (propertyAnnotation.Key)
                    {
                        case ODataAnnotationNames.ODataNextLink:
                            Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.nextLink annotation should have been parsed as a non-null Uri.");
                            expandedResourceSet.NextPageLink = (Uri)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataCount:
                            Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                            expandedResourceSet.Count = (long?)propertyAnnotation.Value;
                            break;

                        case ODataAnnotationNames.ODataType:
                            // Just ignore the type info on resource set.
                            Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a non-null string.");
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(nestedResourceInfo.Name, propertyAnnotation.Key));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, collectionProperty, expandedResourceSet);
        }

        /// <summary>
        /// Reads entity reference link for a singleton navigation link in request.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(
            IODataJsonLightReaderResourceState resourceState,
            IEdmNavigationProperty navigationProperty,
            bool isExpanded)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationProperty.Name,
                IsCollection = false
            };

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
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
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_ArrayValueForSingletonBindPropertyAnnotation(nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                            }

                            if (isExpanded)
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_SingletonNavigationPropertyWithBindingAndValue(nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                            }

                            Debug.Assert(
                                propertyAnnotation.Value is ODataEntityReferenceLink && propertyAnnotation.Value != null,
                                "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                            entityReferenceLink = (ODataEntityReferenceLink)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(
                                nestedResourceInfo.Name,
                                propertyAnnotation.Key,
                                ODataAnnotationNames.ODataBind));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateSingletonEntityReferenceLinkInfo(nestedResourceInfo, navigationProperty, entityReferenceLink, isExpanded);
        }

        /// <summary>
        /// Reads entity reference links for a collection navigation link in request.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference links read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(
            IODataJsonLightReaderResourceState resourceState,
            IEdmNavigationProperty navigationProperty,
            bool isExpanded)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationProperty.Name,
                IsCollection = true
            };

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(nestedResourceInfo.Name);
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
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StringValueForCollectionBindPropertyAnnotation(nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                            }

                            Debug.Assert(
                                propertyAnnotation.Value is LinkedList<ODataEntityReferenceLink> && propertyAnnotation.Value != null,
                                "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                            entityReferenceLinksList = (LinkedList<ODataEntityReferenceLink>)propertyAnnotation.Value;
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(
                                nestedResourceInfo.Name,
                                propertyAnnotation.Key,
                                ODataAnnotationNames.ODataBind));
                    }
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateCollectionEntityReferenceLinksInfo(nestedResourceInfo, navigationProperty, entityReferenceLinksList, isExpanded);
        }

        /// <summary>
        /// Adds a new property to a resource.
        /// </summary>
        /// <param name="resourceState">The resource state for the resource to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property to add.</param>
        private static void AddResourceProperty(IODataJsonLightReaderResourceState resourceState, string propertyName, object propertyValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            ODataProperty property = new ODataProperty { Name = propertyName, Value = propertyValue };
            var propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetCustomPropertyAnnotations(propertyName);
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

            resourceState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            ODataResource resource = resourceState.Resource;
            Debug.Assert(resource != null, "resource != null");
            resource.Properties = resource.Properties.ConcatToReadOnlyEnumerable("Properties", property);
        }

        /// <summary>
        /// Checks if there is a next link annotation immediately after an expanded resource set, and reads and stores it if there is one. 
        /// We fail here if we encounter any other property annotation for the expanded navigation (since these should come before the property itself).
        /// </summary>
        /// <param name="resourceSet">The resource set that was just read.</param>
        /// <param name="expandedNestedResourceInfo">The information for the current expanded nested resource info being read.</param>
        private void ReadExpandedResourceSetAnnotationsAtResourceSetEnd(ODataResourceSetBase resourceSet, ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo)
        {
            Debug.Assert(expandedNestedResourceInfo != null, "expandedNestedResourceInfo != null");
            Debug.Assert(expandedNestedResourceInfo.NestedResourceInfo.IsCollection == true, "Only collection navigation properties can have resourceSet content.");

            // Look at the next property in the owning resource, if it's a property annotation for the expanded nested resource info property, read it.
            string propertyName, annotationName;
            while (this.JsonReader.NodeType == JsonNodeType.Property &&
                   TryParsePropertyAnnotation(this.JsonReader.GetPropertyName(), out propertyName, out annotationName) &&
                   string.CompareOrdinal(propertyName, expandedNestedResourceInfo.NestedResourceInfo.Name) == 0)
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
                        if (resourceSet.NextPageLink != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation(ODataAnnotationNames.ODataNextLink, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        if (resourceSet.Count != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation(ODataAnnotationNames.ODataCount, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed(annotationName, expandedNestedResourceInfo.NestedResourceInfo.Name));
                }
            }
        }

        /// <summary>
        /// Sets specified media resource on a resource and hooks up metadata builder.
        /// </summary>
        /// <param name="resourceState">The resource state to use.</param>
        /// <param name="mediaResource">The media resource to set.</param>
        private void SetEntryMediaResource(IODataJsonLightReaderResourceState resourceState, ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(mediaResource != null, "mediaResource != null");
            ODataResource resource = resourceState.Resource;
            Debug.Assert(resource != null, "resource != null");

            ODataResourceMetadataBuilder builder = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
            mediaResource.SetMetadataBuilder(builder, /*propertyName*/ null);
            resource.MediaResource = mediaResource;
        }

        /// <summary>
        /// Reads resource property (which is neither instance nor property annotation) which has a value.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the property
        ///                 JsonNodeType.StartObject            Expanded resource
        ///                 JsonNodeType.StartArray             Expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource
        /// </remarks>
        private ODataJsonLightReaderNestedResourceInfo ReadPropertyWithValue(IODataJsonLightReaderResourceState resourceState, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            IEdmStructuredType resouceType = resourceState.ResourceType;
            IEdmProperty edmProperty = ReaderValidationUtils.FindDefinedProperty(propertyName, resouceType);
            if (edmProperty != null)
            {
                IEdmStructuralProperty structuredProperty = edmProperty as IEdmStructuralProperty;
                if (structuredProperty != null && structuredProperty.ToStructuredType() != null)
                {
                    // Complex property or collection of complex property.
                    bool isCollection = structuredProperty.Type.IsCollection();
                    this.ValidateExpandedNestedResourceInfoPropertyValue(isCollection, structuredProperty.Name);

                    if (isCollection)
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceSetNestedResourceInfo(resourceState, structuredProperty);
                    }
                    else
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceNestedResourceInfo(resourceState, structuredProperty);
                    }

                    resourceState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                }
                else
                {
                    // Declared property - read it.
                    IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                    if (navigationProperty != null)
                    {
                        // Expanded link
                        bool isCollection = navigationProperty.Type.IsCollection();
                        this.ValidateExpandedNestedResourceInfoPropertyValue(isCollection, navigationProperty.Name);
                        if (isCollection)
                        {
                            readerNestedResourceInfo = this.ReadingResponse
                                ? ReadExpandedResourceSetNestedResourceInfo(resourceState, navigationProperty)
                                : ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, /*isExpanded*/ true);
                        }
                        else
                        {
                            readerNestedResourceInfo = this.ReadingResponse
                                ? ReadExpandedResourceNestedResourceInfo(resourceState, navigationProperty)
                                : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, /*isExpanded*/ true);
                        }

                        resourceState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
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
                        this.ReadEntryDataProperty(resourceState, edmProperty, ValidateDataPropertyTypeNameAnnotation(resourceState.DuplicatePropertyNamesChecker, propertyName));
                    }
                }
            }
            else
            {
                // Undeclared property - we need to run detection alogorithm here.
                readerNestedResourceInfo = this.ReadUndeclaredProperty(resourceState, propertyName, /*propertyWithValue*/ true);

                // Note that if nested resource info is returned it's already validated, so we just report it here.
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Read a resource-level data property and check its version compliance. 
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="edmProperty">The EDM property of the property being read, or null if the property is an open property.</param>
        /// <param name="propertyTypeName">The type name specified for the property in property annotation, or null if no such type name is available.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private void ReadEntryDataProperty(IODataJsonLightReaderResourceState resourceState, IEdmProperty edmProperty, string propertyTypeName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
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
                AddResourceProperty(resourceState, edmProperty.Name, propertyValue);
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an open property.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private void InnerReadOpenUndeclaredProperty(IODataJsonLightReaderResourceState resourceState, IEdmStructuredType owningStructuredType, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_OpenPropertyWithoutValue(propertyName));
            }

            object propertyValue = null;
            bool insideComplexValue = false;
            string outterPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(resourceState.DuplicatePropertyNamesChecker, propertyName);
            string payloadTypeName = TryReadOrPeekPayloadType(resourceState.DuplicatePropertyNamesChecker, propertyName, insideComplexValue);
            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ReaderValidationUtils.ResolvePayloadTypeName(
                this.Model,
                null, // expectedTypeReference
                payloadTypeName,
                EdmTypeKind.Complex,
                this.MessageReaderSettings.ClientCustomTypeResolver,
                out payloadTypeKind);
            IEdmTypeReference payloadTypeReference = null;
            if (!string.IsNullOrEmpty(payloadTypeName) && payloadType != null)
            {
                // only try resolving for known type (the below will throw on unknown type name) :
                SerializationTypeNameAnnotation serializationTypeNameAnnotation;
                EdmTypeKind targetTypeKind;
                payloadTypeReference = ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*defaultPrimitivePayloadType*/ null,
                    null, // expectedTypeReference 
                    payloadTypeName,
                    this.Model,
                    this.MessageReaderSettings,
                    this.GetNonEntityValueKind,
                    out targetTypeKind,
                    out serializationTypeNameAnnotation);
            }

            bool isKnownValueType = IsKnownValueTypeForOpenEntityOrComplex(this.JsonReader.NodeType, this.JsonReader.Value, payloadTypeName, payloadTypeReference);
            if (isKnownValueType)
            {
                this.JsonReader.AssertNotBuffering();
                propertyValue = this.ReadNonEntityValue(
                    outterPayloadTypeName,
                    /*expectedValueTypeReference*/ null,
                    /*duplicatePropertyNamesChecker*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*isTopLevelPropertyValue*/ false,
                    /*insideComplexValue*/ false,
                    propertyName,
                    /*isDynamicProperty*/true);
            }
            else if (this.MessageReaderSettings.ShouldThrowOnUndeclaredProperty())
            {
                // throw specific exceptions for backward compatibility
                if (!string.IsNullOrEmpty(payloadTypeName) && payloadTypeReference == null)
                {
                    throw new ODataException(ODataErrorStrings.ValidationUtils_UnrecognizedTypeName(payloadTypeName));
                }

                if (string.IsNullOrEmpty(payloadTypeName)
                    && (this.JsonReader.NodeType == JsonNodeType.StartObject
                        || this.JsonReader.NodeType == JsonNodeType.StartArray))
                {
                    throw new ODataException(ODataErrorStrings.ReaderValidationUtils_ValueWithoutType);
                }

                throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, owningStructuredType.FullTypeName()));
            }
            else
            {
                Debug.Assert(
                    this.MessageReaderSettings.ShouldSupportUndeclaredProperty(),
                    "this.MessageReaderSettings.ShouldSupportUndeclaredProperty()");
                propertyValue = this.JsonReader.ReadAsUntypedOrNullValue();
            }

            ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue);
            AddResourceProperty(resourceState, propertyName, propertyValue);
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                        this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                        "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an undeclared property. That is a property which is not declared by the model, but the owning type is not an open type.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue:  propertyWithValue is true and the reader is positioned on the first node of the property value.
        ///                 JsonNodeType.StartObject:
        ///                 JsonNodeType.StartArray:
        ///                 JsonNodeType.Property:        propertyWithValue is false and the reader is positioned on the node after the property.
        ///                 JsonNodeType.EndObject:
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        /// <returns>A nested resource info instance if the propery read is a nested resource info which should be reported to the caller.
        /// Otherwise null if the property was either ignored or read and added to the list of properties on the resource.</returns>
        private ODataJsonLightReaderNestedResourceInfo ReadUndeclaredProperty(IODataJsonLightReaderResourceState resourceState, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
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
                if (resourceState.ResourceType.IsOpen)
                {
                    // Open property - read it as such.
                    this.InnerReadOpenUndeclaredProperty(resourceState, resourceState.ResourceType, propertyName, propertyWithValue);
                    return null;
                }
            }

            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            Dictionary<string, object> odataPropertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(propertyName);
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
                        throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, resourceState.ResourceType.FullTypeName()));
                    }

                    // Read it as a deferred link - we never read the expanded content.
                    ODataJsonLightReaderNestedResourceInfo navigationLinkInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, /*navigationProperty*/ null);
                    resourceState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNestedResourceInfoStart(navigationLinkInfo.NestedResourceInfo);

                    // If the property is expanded, ignore the content if we're asked to do so.
                    if (propertyWithValue)
                    {
                        if (this.MessageReaderSettings.ShouldThrowOnUndeclaredProperty())
                        {
                            throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, resourceState.ResourceType.FullTypeName()));
                        }

                        this.ValidateExpandedNestedResourceInfoPropertyValue(null, propertyName);

                        // Since we marked the nested resource info as deffered the reader will not try to read its content
                        // instead it will behave as if it was a real deferred link (without a property value).
                        // So skip the value here to move to the next property in the payload, which will look exactly the same
                        // as if the nested resource info was deferred.
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
                        throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, resourceState.ResourceType.FullTypeName()));
                    }

                    // Stream properties can't have a value
                    if (propertyWithValue)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue(propertyName));
                    }

                    ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                    AddResourceProperty(resourceState, propertyName, streamPropertyValue);
                    return null;
                }
            }

            if (resourceState.ResourceType.IsOpen)
            {
                // Open property - read it as such.
                this.InnerReadOpenUndeclaredProperty(resourceState, resourceState.ResourceType, propertyName, propertyWithValue);
                return null;
            }

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType(propertyName));
            }

            // Property with value can only be ignored if we're asked to do so.
            if (this.MessageReaderSettings.ShouldThrowOnUndeclaredProperty())
            {
                throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, resourceState.ResourceType.FullTypeName()));
            }

            // Validate that the property doesn't have unrecognized annotations
            // We ignore the type name since we might not have the full model and thus might not be able to resolve it correctly.
            ValidateDataPropertyTypeNameAnnotation(resourceState.DuplicatePropertyNamesChecker, propertyName);

            if (this.MessageReaderSettings.ShouldSupportUndeclaredProperty())
            {
                bool isTopLevelPropertyValue = false;
                object propertyValue = this.InnerReadNonOpenUndeclaredProperty(resourceState.DuplicatePropertyNamesChecker, propertyName, isTopLevelPropertyValue);
                AddResourceProperty(resourceState, propertyName, propertyValue);
            }
            else
            {
                Debug.Assert(
                    this.MessageReaderSettings.ShouldThrowOnUndeclaredProperty(),
                    "this.MessageReaderSettings.ShouldThrowOnUndeclaredProperty()");
            }

            return null;
        }

        /// <summary>
        /// Reads a stream property value from the property annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="streamPropertyName">The name of the stream property to read the value for.</param>
        /// <returns>The newly created stream reference value.</returns>
        private ODataStreamReferenceValue ReadStreamPropertyValue(IODataJsonLightReaderResourceState resourceState, string streamPropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(streamPropertyName), "!string.IsNullOrEmpty(streamPropertyName)");

            // Fail on stream properties in requests as they cannot appear there.
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyInRequest);
            }

            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();

            Dictionary<string, object> propertyAnnotations = resourceState.DuplicatePropertyNamesChecker.GetODataPropertyAnnotations(streamPropertyName);
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

            ODataResourceMetadataBuilder builder = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);

            // Note that we set the metadata builder even when streamProperty is null, which is the case when the stream property is undeclared.
            // For undeclared stream properties, we will apply conventional metadata evaluation just as declared stream properties.
            streamReferenceValue.SetMetadataBuilder(builder, streamPropertyName);

            return streamReferenceValue;
        }

        /// <summary>
        /// Reads one operation for the resource being read.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:   first node of the operation value.
        /// Post-Condition: JsonNodeType.Property:      the property after the current operation being read when there is one target for the operation.
        ///                 JsonNodeType.StartObject:   the first node of the next operation value when there are multiple targets for the operation.
        ///                 JsonNodeType.EndArray:      the end-array of the operation values when there are multiple target for the operation.
        /// </remarks>
        private void ReadSingleOperationValue(IODataJsonOperationsDeserializerContext readerContext, IODataJsonLightReaderResourceState resourceState, string metadataReferencePropertyName, bool insideArray)
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
            this.SetMetadataBuilder(resourceState, operation);
        }

        /// <summary>
        /// Reads one operation for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        private void ReadSingleOperationValue(ODataResourceSet resourceSet, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonLightReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(metadataReferencePropertyName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            this.JsonReader.ReadStartObject();

            var operation = this.CreateODataOperationAndAddToResourceSet(resourceSet, metadataReferencePropertyName);

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
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="operation">The operation to set the metadata builder on.</param>
        private void SetMetadataBuilder(IODataJsonLightReaderResourceState resourceState, ODataOperation operation)
        {
            ODataResourceMetadataBuilder builder = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
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
                readerContext.AddActionToResource((ODataAction)operation);
            }
            else
            {
                readerContext.AddFunctionToResource((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Creates a new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.
        /// </summary>
        /// <param name="resourceSet">The resource set to add the action or function .</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.</returns>
        private ODataOperation CreateODataOperationAndAddToResourceSet(ODataResourceSet resourceSet, string metadataReferencePropertyName)
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
                resourceSet.AddAction((ODataAction)operation);
            }
            else
            {
                resourceSet.AddFunction((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Read the metadata reference property value for the resource being read.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:      first node of the metadata reference property's value. Currently 
        ///                                             actions and functions are the only supported metadata reference property,
        ///                                             we will throw if this is not a start object or start array node.
        /// Post-Condition: JsonNodeType.Property:      the property after the annotation value
        ///                 JsonNodeType.EndObject:     the end-object of the resource
        /// </remarks>
        private void ReadMetadataReferencePropertyValue(IODataJsonLightReaderResourceState resourceState, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.Resource != null, "resourceState.Resource != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) > -1, "metadataReferencePropertyName.IndexOf(JsonLightConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            IODataJsonOperationsDeserializerContext readerContext = new OperationsDeserializerContext(resourceState.Resource, this);

            bool insideArray = false;
            if (readerContext.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                readerContext.JsonReader.ReadStartArray();
                insideArray = true;
            }

            do
            {
                this.ReadSingleOperationValue(readerContext, resourceState, metadataReferencePropertyName, insideArray);
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
        /// Read the metadata reference property value for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        private void ReadMetadataReferencePropertyValue(ODataResourceSet resourceSet, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
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
                this.ReadSingleOperationValue(resourceSet, metadataReferencePropertyName, insideArray);
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
        /// Validates that the value of a JSON property can represent expanded nested resource info.
        /// </summary>
        /// <param name="isCollection">true if the property is entity set reference property; false for a resource reference property, null if unknown.</param>
        /// <param name="propertyName">Name for the navigation property, used in error message only.</param>
        private void ValidateExpandedNestedResourceInfoPropertyValue(bool? isCollection, string propertyName)
        {
            // an expanded link with resource requires a StartObject node here;
            // an expanded link with resource set requires a StartArray node here;
            // an expanded link with null resource requires a primitive null node here;
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
                // Expanded resource (null or non-null)
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
            /// The resource to add operations to.
            /// </summary>
            private ODataResource resource;

            /// <summary>
            /// The deserializer to use.
            /// </summary>
            private ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="resource">The resource to add operations to.</param>
            /// <param name="jsonLightResourceDeserializer">The deserializer to use.</param>
            public OperationsDeserializerContext(ODataResource resource, ODataJsonLightResourceDeserializer jsonLightResourceDeserializer)
            {
                Debug.Assert(resource != null, "resource != null");
                Debug.Assert(jsonLightResourceDeserializer != null, "jsonLightResourceDeserializer != null");

                this.resource = resource;
                this.jsonLightResourceDeserializer = jsonLightResourceDeserializer;
            }

            /// <summary>
            /// The JSON reader to read the operations value from.
            /// </summary>
            public IJsonReader JsonReader
            {
                get
                {
                    return this.jsonLightResourceDeserializer.JsonReader;
                }
            }

            /// <summary>
            /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
            /// </summary>
            /// <param name="uriFromPayload">The URI string from the payload to process.</param>
            /// <returns>An absolute URI to report.</returns>
            public Uri ProcessUriFromPayload(string uriFromPayload)
            {
                return this.jsonLightResourceDeserializer.ProcessUriFromPayload(uriFromPayload);
            }

            /// <summary>
            /// Adds the specified action to the current resource.
            /// </summary>
            /// <param name="action">The action whcih is fully populated with the data from the payload.</param>
            public void AddActionToResource(ODataAction action)
            {
                Debug.Assert(action != null, "action != null");
                this.resource.AddAction(action);
            }

            /// <summary>
            /// Adds the specified function to the current resource.
            /// </summary>
            /// <param name="function">The function whcih is fully populated with the data from the payload.</param>
            public void AddFunctionToResource(ODataFunction function)
            {
                Debug.Assert(function != null, "function != null");
                this.resource.AddFunction(function);
            }
        }
    }
}
