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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for entity reference links.
    /// </summary>
    internal sealed class ODataJsonLightEntityReferenceLinkDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightEntityReferenceLinkDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal ODataEntityReferenceLinks ReadEntityReferenceLinks(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLinks,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(navigationProperty, duplicatePropertyNamesChecker);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
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
                        ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(navigationProperty, duplicatePropertyNamesChecker);

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
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal ODataEntityReferenceLink ReadEntityReferenceLink(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLink,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(navigationProperty, duplicatePropertyNamesChecker);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
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
                        ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(navigationProperty, duplicatePropertyNamesChecker);

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
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation(IEdmNavigationProperty navigationProperty, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();

            if (this.JsonLightInputContext.ReadingResponse)
            {
                // Validate the metadata URI parsed from the payload against the expected navigation property.
                ReaderValidationUtils.ValidateEntityReferenceLinkMetadataUri(this.MetadataUriParseResult, navigationProperty);
            }

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
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation(IEdmNavigationProperty navigationProperty, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            if (this.JsonLightInputContext.ReadingResponse)
            {
                // Validate the metadata URI parsed from the payload against the expected navigation property.
                ReaderValidationUtils.ValidateEntityReferenceLinkMetadataUri(this.MetadataUriParseResult, navigationProperty);
            }

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
        /// Pre-Condition:  JsonNodeType.Property               The first property in the payload (or the first property after the metadata URI in responses)
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
                                this.JsonReader.SkipValue();
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
            links.Count = this.ReadAndValidateAnnotationStringValueAsLong(ODataAnnotationNames.ODataCount);
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
                                // Instance annotations are not allowed for entity reference links
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                // If we find a property without a value it means that we did not find the 'url' property of the
                                // entity reference link but an invalid property annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataEntityReferenceLinkUrlName, propertyName) != 0)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(propertyName, JsonLightConstants.ODataEntityReferenceLinkUrlName));
                                }
                                else if (entityReferenceLink[0] != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(JsonLightConstants.ODataEntityReferenceLinkUrlName));
                                }

                                // read the value of the 'url' property
                                string urlString = this.JsonReader.ReadStringValue(JsonLightConstants.ODataEntityReferenceLinkUrlName);
                                if (urlString == null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(JsonLightConstants.ODataEntityReferenceLinkUrlName));
                                }

                                entityReferenceLink[0] = new ODataEntityReferenceLink
                                {
                                    Url = this.ProcessUriFromPayload(urlString)
                                };

                                ReaderValidationUtils.ValidateEntityReferenceLink(entityReferenceLink[0]);
                                break;

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink));
                        }
                    });
            }

            if (entityReferenceLink[0] == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(JsonLightConstants.ODataEntityReferenceLinkUrlName));
            }

            // end of the entity reference link object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            return entityReferenceLink[0];
        }
    }
}
