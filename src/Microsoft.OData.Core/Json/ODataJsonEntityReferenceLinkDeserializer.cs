//---------------------------------------------------------------------
// <copyright file="ODataJsonEntityReferenceLinkDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for entity reference links.
    /// </summary>
    internal sealed class ODataJsonEntityReferenceLinkDeserializer : ODataJsonPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonEntityReferenceLinkDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
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
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLinks,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(propertyAndAnnotationCollector);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal async Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            await this.ReadPayloadStartAsync(
                ODataPayloadKind.EntityReferenceLinks,
                propertyAndAnnotationCollector,
                isReadingNestedPayload: false,
                allowEmptyPayload: false).ConfigureAwait(false);

            ODataEntityReferenceLinks entityReferenceLinks = await this.ReadEntityReferenceLinksImplementationAsync(
                propertyAndAnnotationCollector).ConfigureAwait(false);

            await this.ReadPayloadEndAsync(isReadingNestedPayload: false)
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            this.ReadPayloadStart(
                ODataPayloadKind.EntityReferenceLink,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(propertyAndAnnotationCollector);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal async Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            await this.ReadPayloadStartAsync(
                ODataPayloadKind.EntityReferenceLink,
                propertyAndAnnotationCollector,
                isReadingNestedPayload: false,
                allowEmptyPayload: false).ConfigureAwait(false);

            ODataEntityReferenceLink entityReferenceLink = await this.ReadEntityReferenceLinkImplementationAsync(
                propertyAndAnnotationCollector).ConfigureAwait(false);

            await this.ReadPayloadEndAsync(isReadingNestedPayload: false)
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();

            this.ReadEntityReferenceLinksAnnotations(entityReferenceLinks, propertyAndAnnotationCollector, /*forLinksStart*/true);

            // Read the start of the content array of the links
            this.JsonReader.ReadStartArray();

            List<ODataEntityReferenceLink> links = new List<ODataEntityReferenceLink>();
            PropertyAndAnnotationCollector linkPropertyAndAnnotationCollector = this.JsonInputContext.CreatePropertyAndAnnotationCollector();

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                // read another link
                ODataEntityReferenceLink entityReferenceLink = this.ReadSingleEntityReferenceLink(linkPropertyAndAnnotationCollector, /*topLevel*/false);
                links.Add(entityReferenceLink);
                linkPropertyAndAnnotationCollector.Reset();
            }

            this.JsonReader.ReadEndArray();

            this.ReadEntityReferenceLinksAnnotations(entityReferenceLinks, propertyAndAnnotationCollector, /*forLinksStart*/false);

            this.JsonReader.ReadEndObject();

            entityReferenceLinks.Links = new ReadOnlyEnumerable<ODataEntityReferenceLink>(links);
            return entityReferenceLinks;
        }

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            return this.ReadSingleEntityReferenceLink(propertyAndAnnotationCollector, /*topLevel*/true);
        }

        /// <summary>
        /// Reads the entity reference link instance annotations.
        /// </summary>
        /// <param name="links">The <see cref="ODataEntityReferenceLinks"/> to read the annotations for.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the entity reference links scope.</param>
        /// <param name="forLinksStart">true when parsing the instance annotations before the 'value' property;
        /// false when parsing the instance annotations after the 'value' property.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The first property in the payload (or the first property after the context URI in responses)
        ///                 JsonNodeType.EndObject              The end of the entity reference links object
        /// Post-Condition: JsonNodeType.EndObject              When the end of the entity reference links object is reached
        ///                 Any                                 The first node of the value of the 'url' property (if found)
        /// </remarks>
        private void ReadEntityReferenceLinksAnnotations(ODataEntityReferenceLinks links, PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool forLinksStart)
        {
            Debug.Assert(links != null, "links != null");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            this.JsonReader.AssertNotBuffering();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported on entity reference links.
                Func<string, object> propertyAnnotationValueReader =
                    annotationName => { throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks); };

                bool foundValueProperty = false;
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    propertyAndAnnotationCollector,
                    propertyAnnotationValueReader,
                    (propertyParseResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            this.JsonReader.Read();
                        }

                        switch (propertyParseResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (string.Equals(ODataAnnotationNames.ODataNextLink, propertyName, StringComparison.Ordinal))
                                {
                                    this.ReadEntityReferenceLinksNextLinkAnnotationValue(links);
                                }
                                else if (string.Equals(ODataAnnotationNames.ODataCount, propertyName, StringComparison.Ordinal))
                                {
                                    this.ReadEntityReferenceCountAnnotationValue(links);
                                }
                                else
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                }

                                break;

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
                                links.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                if (!string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                {
                                    // We did not find a supported link collection property; fail.
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(propertyName, ODataJsonConstants.ODataValuePropertyName));
                                }

                                // We found the link collection property and are done parsing property annotations;
                                foundValueProperty = true;
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                // If we find a property without a value it means that we did not find the entity reference links property (yet)
                                // but an invalid property annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonEntityReferenceLinkDeserializer_ReadEntityReferenceLinksAnnotations));
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
                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(ODataJsonConstants.ODataValuePropertyName));
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
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to check for duplicate properties and
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
        private ODataEntityReferenceLink ReadSingleEntityReferenceLink(PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool topLevel)
        {
            this.JsonReader.AssertNotBuffering();

            if (!topLevel)
            {
                if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    // entity reference link has to be an object
                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(this.JsonReader.NodeType));
                }

                this.JsonReader.ReadStartObject();
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataEntityReferenceLink[] entityReferenceLink = { null };

            // Entity  reference links use instance annotations. Fail if we find a  property annotation.
            Func<string, object> propertyAnnotationValueReader =
                annotationName => { throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(annotationName)); };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    propertyAndAnnotationCollector,
                    propertyAnnotationValueReader,
                    (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            this.JsonReader.Read();
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (!string.Equals(ODataAnnotationNames.ODataId, propertyName, StringComparison.Ordinal))
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(propertyName, ODataAnnotationNames.ODataId));
                                }
                                else if (entityReferenceLink[0] != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(ODataAnnotationNames.ODataId));
                                }

                                // read the value of the 'odata.id' annotation
                                string urlString = this.JsonReader.ReadStringValue(ODataAnnotationNames.ODataId);
                                if (urlString == null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(ODataAnnotationNames.ODataId));
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
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
                                }

                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
                                entityReferenceLink[0].InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                            case PropertyParsingResult.PropertyWithoutValue:
                                // entity reference link  is denoted by odata.id annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink));
                        }
                    });
            }

            if (entityReferenceLink[0] == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
            }

            // end of the entity reference link object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            return entityReferenceLink[0];
        }

        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataEntityReferenceLinks"/> representing the read links.
        /// </returns>
        private async Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksImplementationAsync(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();

            await this.ReadEntityReferenceLinksAnnotationsAsync(entityReferenceLinks, propertyAndAnnotationCollector, forLinksStart: true)
                .ConfigureAwait(false);
            // Read the start of the content array of the links
            await this.JsonReader.ReadStartArrayAsync()
                .ConfigureAwait(false);

            List<ODataEntityReferenceLink> links = new List<ODataEntityReferenceLink>();
            PropertyAndAnnotationCollector linkPropertyAndAnnotationCollector = this.JsonInputContext.CreatePropertyAndAnnotationCollector();

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                // read another link
                ODataEntityReferenceLink entityReferenceLink = await this.ReadSingleEntityReferenceLinkAsync(
                    linkPropertyAndAnnotationCollector,
                    topLevel: false).ConfigureAwait(false);
                links.Add(entityReferenceLink);
                linkPropertyAndAnnotationCollector.Reset();
            }

            await this.JsonReader.ReadEndArrayAsync()
                .ConfigureAwait(false);
            await this.ReadEntityReferenceLinksAnnotationsAsync(entityReferenceLinks, propertyAndAnnotationCollector, forLinksStart: false)
                .ConfigureAwait(false);
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            entityReferenceLinks.Links = new ReadOnlyEnumerable<ODataEntityReferenceLink>(links);
            return entityReferenceLinks;
        }

        /// <summary>
        /// Asynchronously reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.
        /// </returns>
        private Task<ODataEntityReferenceLink> ReadEntityReferenceLinkImplementationAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            return this.ReadSingleEntityReferenceLinkAsync(propertyAndAnnotationCollector, topLevel: true);
        }

        /// <summary>
        /// Asynchronously reads the entity reference link instance annotations.
        /// </summary>
        /// <param name="links">The <see cref="ODataEntityReferenceLinks"/> to read the annotations for.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the entity reference links scope.</param>
        /// <param name="forLinksStart">true when parsing the instance annotations before the 'value' property;
        /// false when parsing the instance annotations after the 'value' property.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The first property in the payload (or the first property after the context URI in responses)
        ///                 JsonNodeType.EndObject              The end of the entity reference links object
        /// Post-Condition: JsonNodeType.EndObject              When the end of the entity reference links object is reached
        ///                 Any                                 The first node of the value of the 'url' property (if found)
        /// </remarks>
        private async Task ReadEntityReferenceLinksAnnotationsAsync(
            ODataEntityReferenceLinks links,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool forLinksStart)
        {
            Debug.Assert(links != null, "links != null");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            this.JsonReader.AssertNotBuffering();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported on entity reference links.
                Func<string, Task<object>> propertyAnnotationValueReaderDelegate =
                    annotationName =>
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks);
                    };

                bool foundValueProperty = false;
                this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    propertyAnnotationValueReaderDelegate,
                    async (propertyParseResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            await this.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }

                        switch (propertyParseResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (string.Equals(ODataAnnotationNames.ODataNextLink, propertyName, StringComparison.Ordinal))
                                {
                                    this.AssertJsonCondition(JsonNodeType.PrimitiveValue);
                                    Uri nextPageUri = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataNextLink)
                                        .ConfigureAwait(false);
                                    Debug.Assert(links.NextPageLink == null, "We should have checked for duplicates already.");
                                    links.NextPageLink = nextPageUri;
                                }
                                else if (string.Equals(ODataAnnotationNames.ODataCount, propertyName, StringComparison.Ordinal))
                                {
                                    this.AssertJsonCondition(JsonNodeType.PrimitiveValue);
                                    Debug.Assert(!links.Count.HasValue, "We should have checked for duplicates already.");
                                    links.Count = await this.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(ODataAnnotationNames.ODataCount)
                                        .ConfigureAwait(false);
                                }
                                else
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                }

                                break;

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = await this.ReadCustomInstanceAnnotationValueAsync(propertyAndAnnotationCollector, propertyName)
                                    .ConfigureAwait(false);
                                links.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                if (!string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                {
                                    // We did not find a supported link collection property; fail.
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(propertyName, ODataJsonConstants.ODataValuePropertyName));
                                }

                                // We found the link collection property and are done parsing property annotations;
                                foundValueProperty = true;
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                // If we find a property without a value it means that we did not find the entity reference links property (yet)
                                // but an invalid property annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonEntityReferenceLinkDeserializer_ReadEntityReferenceLinksAnnotations));
                        }
                    }).ConfigureAwait(false);

                if (foundValueProperty)
                {
                    return;
                }
            }

            if (forLinksStart)
            {
                // We did not find the 'value' property.
                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(ODataJsonConstants.ODataValuePropertyName));
            }

            this.AssertJsonCondition(JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously read an entity reference link.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to check for duplicate properties and
        /// duplicate annotations; this is a separate instance per entity reference link.</param>
        /// <param name="topLevel">true if we are reading a singleton entity reference link at the top level; false if we are reading
        /// an entity reference link as part of a collection of entity reference links.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an instance of <see cref="ODataEntityReferenceLink"/> which was read.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  StartObject     when the entity reference link is part of a collection
        ///                 Property        the first property in the entity reference link (for a top-level link)
        ///                 EndObject       the end object node of an entity reference link (for a top-level link)
        /// Post-Condition: EndInput        for a top-level object
        ///                 EndArray        for the last link in a collection of links
        ///                 Any             for the first node of the next link in a collection of links
        /// </remarks>
        private async Task<ODataEntityReferenceLink> ReadSingleEntityReferenceLinkAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool topLevel)
        {
            this.JsonReader.AssertNotBuffering();

            if (!topLevel)
            {
                if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    // entity reference link has to be an object
                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(this.JsonReader.NodeType));
                }

                await this.JsonReader.ReadStartObjectAsync()
                    .ConfigureAwait(false);
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataEntityReferenceLink[] entityReferenceLink = { null };

            // Entity  reference links use instance annotations. Fail if we find a  property annotation.
            Func<string, Task<object>> propertyAnnotationValueReaderDelegate =
                annotationName =>
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(annotationName));
                };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    propertyAnnotationValueReaderDelegate,
                    async (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            await this.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                if (!string.Equals(ODataAnnotationNames.ODataId, propertyName, StringComparison.Ordinal))
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(propertyName, ODataAnnotationNames.ODataId));
                                }
                                else if (entityReferenceLink[0] != null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(ODataAnnotationNames.ODataId));
                                }

                                // read the value of the 'odata.id' annotation
                                string urlString = await this.JsonReader.ReadStringValueAsync(ODataAnnotationNames.ODataId)
                                    .ConfigureAwait(false);
                                if (urlString == null)
                                {
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(ODataAnnotationNames.ODataId));
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
                                    throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
                                }

                                Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
                                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(propertyName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object annotationValue = await this.ReadCustomInstanceAnnotationValueAsync(propertyAndAnnotationCollector, propertyName)
                                    .ConfigureAwait(false);
                                entityReferenceLink[0].InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, annotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                            case PropertyParsingResult.PropertyWithoutValue:
                                // entity reference link  is denoted by odata.id annotation
                                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink));
                        }
                    }).ConfigureAwait(false);
            }

            if (entityReferenceLink[0] == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
            }

            // end of the entity reference link object
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            this.JsonReader.AssertNotBuffering();
            return entityReferenceLink[0];
        }
    }
}
