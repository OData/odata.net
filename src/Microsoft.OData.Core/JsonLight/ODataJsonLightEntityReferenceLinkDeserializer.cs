//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntityReferenceLinkDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Json;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for entity reference links.
    /// </summary>
    internal sealed class ODataJsonLightEntityReferenceLinkDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightEntityReferenceLinkDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLinks,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(duplicatePropertyNamesChecker);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

#if PORTABLELIB
        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            return this.ReadPayloadStartAsync(
                ODataPayloadKind.EntityReferenceLinks,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                    {
                        ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(duplicatePropertyNamesChecker);

                        this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
                        this.JsonReader.AssertNotBuffering();

                        return entityReferenceLinks;
                    });
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLink,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(duplicatePropertyNamesChecker);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

#if PORTABLELIB
        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            return this.ReadPayloadStartAsync(
                ODataPayloadKind.EntityReferenceLink,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                    {
                        ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(duplicatePropertyNamesChecker);

                        this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
                        this.JsonReader.AssertNotBuffering();

                        return entityReferenceLink;
                    });
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();

            this.ReadEntityReferenceLinksAnnotations(entityReferenceLinks, duplicatePropertyNamesChecker, /*forLinksStart*/true);

            // Read the start of the content array of the links
            this.JsonReader.ReadStartArray();

            List<ODataEntityReferenceLink> links = new List<ODataEntityReferenceLink>();
            DuplicatePropertyNamesChecker linkDuplicatePropertyNamesChecker = this.JsonLightInputContext.CreateDuplicatePropertyNamesChecker();

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                // read another link
                ODataEntityReferenceLink entityReferenceLink = this.ReadSingleEntityReferenceLink(linkDuplicatePropertyNamesChecker, /*topLevel*/false);
                links.Add(entityReferenceLink);
                linkDuplicatePropertyNamesChecker.Clear();
            }

            this.JsonReader.ReadEndArray();

            this.ReadEntityReferenceLinksAnnotations(entityReferenceLinks, duplicatePropertyNamesChecker, /*forLinksStart*/false);

            this.JsonReader.ReadEndObject();

            entityReferenceLinks.Links = new ReadOnlyEnumerable<ODataEntityReferenceLink>(links);
            return entityReferenceLinks;
        }

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            return this.ReadSingleEntityReferenceLink(duplicatePropertyNamesChecker, /*topLevel*/true);
        }

        /// <summary>
        /// Reads the entity reference link instance annotations.
        /// </summary>
        /// <param name="links">The <see cref="ODataEntityReferenceLinks"/> to read the annotations for.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the entity reference links scope.</param>
        /// <param name="forLinksStart">true when parsing the instance annotations before the 'value' property;
        /// false when parsing the instance annotations after the 'value' property.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The first property in the payload (or the first property after the context URI in responses)
        ///                 JsonNodeType.EndObject              The end of the entity reference links object
        /// Post-Condition: JsonNodeType.EndObject              When the end of the entity reference links object is reached
        ///                 Any                                 The first node of the value of the 'url' property (if found)
        /// </remarks>
        private void ReadEntityReferenceLinksAnnotations(ODataEntityReferenceLinks links, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool forLinksStart)
        {
            Debug.Assert(links != null, "links != null");
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            this.JsonReader.AssertNotBuffering();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported on entity reference links.
                Func<string, object> propertyAnnotationValueReader =
                    annotationName => { throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks); };

                bool foundValueProperty = false;
                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    propertyAnnotationValueReader,
                    (propertyParseResult, propertyName) =>
                    {
                        switch (propertyParseResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (string.CompareOrdinal(ODataAnnotationNames.ODataNextLink, propertyName) == 0)
                                {
                                    this.ReadEntityReferenceLinksNextLinkAnnotationValue(links);
                                }
                                else if (string.CompareOrdinal(ODataAnnotationNames.ODataCount, propertyName) == 0)
                                {
                                    this.ReadEntityReferenceCountAnnotationValue(links);
                                }
                                else
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                }

                                break;

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = this.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, propertyName);
                                links.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) != 0)
                                {
                                    // We did not find a supported link collection property; fail.
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(propertyName, JsonLightConstants.ODataValuePropertyName));
                                }

                                // We found the link collection property and are done parsing property annotations;
                                foundValueProperty = true;
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                // If we find a property without a value it means that we did not find the entity reference links property (yet)
                                // but an invalid property annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightEntityReferenceLinkDeserializer_ReadEntityReferenceLinksAnnotations));
                        }
                    });

                if (foundValueProperty)
                {
                    return;
                }
            }

            if (forLinksStart)
            {
                // We did not find the 'value' property.
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(JsonLightConstants.ODataValuePropertyName));
            }

            this.AssertJsonCondition(JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the odata.nextlink value of an entity reference links nextlink annotation.
        /// </summary>
        /// <param name="links">The entity reference links to read the next link value for; the value of the nextlink will be assigned to this instance.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the entity reference links object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        private void ReadEntityReferenceLinksNextLinkAnnotationValue(ODataEntityReferenceLinks links)
        {
            Debug.Assert(links != null, "links != null");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue);

            Uri nextPageUri = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
            Debug.Assert(links.NextPageLink == null, "We should have checked for duplicates already.");
            links.NextPageLink = nextPageUri;
        }

        /// <summary>
        /// Reads the value of an entity reference links count annotation.
        /// </summary>
        /// <param name="links">The entity reference links to read the count value for; the value of the count will be assigned to this instance.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the entity reference links object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        private void ReadEntityReferenceCountAnnotationValue(ODataEntityReferenceLinks links)
        {
            Debug.Assert(links != null, "links != null");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue);
            Debug.Assert(!links.Count.HasValue, "We should have checked for duplicates already.");
            links.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
        }

        /// <summary>
        /// Read an entity reference link.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to check for duplicate properties and
        /// duplicate annotations; this is a separate instance per entity reference link.</param>
        /// <param name="topLevel">true if we are reading a singleton entity reference link at the top level; false if we are reading
        /// an entity reference link as part of a collection of entity reference links.</param>
        /// <returns>An instance of <see cref="ODataEntityReferenceLink"/> which was read.</returns>
        /// <remarks>
        /// Pre-Condition:  StartObject     when the entity reference link is part of a collection
        ///                 Property        the first property in the entity reference link (for a top-level link)
        ///                 EndObject       the end object node of an entity reference link (for a top-level link)
        /// Post-Condition: EndInput        for a top-level object
        ///                 EndArray        for the last link in a collection of links
        ///                 Any             for the first node of the next link in a collection of links
        /// </remarks>
        private ODataEntityReferenceLink ReadSingleEntityReferenceLink(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool topLevel)
        {
            this.JsonReader.AssertNotBuffering();

            if (!topLevel)
            {
                if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    // entity reference link has to be an object
                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(this.JsonReader.NodeType));
                }

                this.JsonReader.ReadStartObject();
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataEntityReferenceLink[] entityReferenceLink = { null };

            // Entity  reference links use instance annotations. Fail if we find a  property annotation.
            Func<string, object> propertyAnnotationValueReader =
                annotationName => { throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(annotationName)); };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    propertyAnnotationValueReader,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (string.CompareOrdinal(ODataAnnotationNames.ODataId, propertyName) != 0)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(propertyName, ODataAnnotationNames.ODataId));
                                }
                                else if (entityReferenceLink[0] != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(ODataAnnotationNames.ODataId));
                                }

                                // read the value of the 'odata.id' annotation
                                string urlString = this.JsonReader.ReadStringValue(ODataAnnotationNames.ODataId);
                                if (urlString == null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(ODataAnnotationNames.ODataId));
                                }

                                entityReferenceLink[0] = new ODataEntityReferenceLink
                                {
                                    Url = this.ProcessUriFromPayload(urlString)
                                };

                                ReaderValidationUtils.ValidateEntityReferenceLink(entityReferenceLink[0]);

                                break;

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                if (entityReferenceLink[0] == null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
                                }

                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = this.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, propertyName);
                                entityReferenceLink[0].InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                            case PropertyParsingResult.PropertyWithoutValue:
                                // entity reference link  is denoted by odata.id annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink));
                        }
                    });
            }

            if (entityReferenceLink[0] == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
            }

            // end of the entity reference link object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            return entityReferenceLink[0];
        }
    }
}
