//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for service documents.
    /// </summary>
    internal sealed class ODataJsonLightServiceDocumentDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightServiceDocumentDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:        assumes that the JSON reader has not been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal ODataServiceDocument ReadServiceDocument()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node
            this.ReadPayloadStart(
                ODataPayloadKind.ServiceDocument,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataServiceDocument serviceDocument = this.ReadServiceDocumentImplementation(duplicatePropertyNamesChecker);

            // Read the end of the response.
            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return serviceDocument;
        }

#if PORTABLELIB
        /// <summary>
        /// Read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:        assumes that the JSON reader has not been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal Task<ODataServiceDocument> ReadServiceDocumentAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node
            return this.ReadPayloadStartAsync(
                ODataPayloadKind.ServiceDocument,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                    {
                        ODataServiceDocument serviceDocument = this.ReadServiceDocumentImplementation(duplicatePropertyNamesChecker);

                        // Read the end of the response.
                        this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
                        this.JsonReader.AssertNotBuffering();

                        return serviceDocument;
                    });
        }
#endif

        /// <summary>
        /// Read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property   The property right after the context URI property.
        ///                 JsonNodeType.EndObject  The EndObject of the service document.
        /// Post-Condition: Any                     The node after the EndObject of the service document.
        /// </remarks>
        private ODataServiceDocument ReadServiceDocumentImplementation(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            List<ODataServiceDocumentElement>[] serviceDocumentElements = { null };

            // Read all the properties in the service document object; we ignore all except 'value'.
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // Property annotations are not allowed on the 'value' property, so fail if we see one.
                Func<string, object> readPropertyAnnotationInServiceDoc = annotationName => { throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(annotationName, JsonLightConstants.ODataValuePropertyName)); };

                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    readPropertyAnnotationInServiceDoc,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(propertyName, JsonLightConstants.ODataValuePropertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) == 0)
                                {
                                    // Fail if we've already processed a 'value' property.
                                    if (serviceDocumentElements[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(JsonLightConstants.ODataValuePropertyName));
                                    }

                                    serviceDocumentElements[0] = new List<ODataServiceDocumentElement>();

                                    // Read the value of the 'value' property.
                                    this.JsonReader.ReadStartArray();
                                    DuplicatePropertyNamesChecker resourceCollectionDuplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

                                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                                    {
                                        ODataServiceDocumentElement serviceDocumentElement = this.ReadServiceDocumentElement(resourceCollectionDuplicatePropertyNamesChecker);

                                        if (serviceDocumentElement != null)
                                        {
                                            serviceDocumentElements[0].Add(serviceDocumentElement);
                                        }

                                        resourceCollectionDuplicatePropertyNamesChecker.Clear();
                                    }

                                    this.JsonReader.ReadEndArray();
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(propertyName, JsonLightConstants.ODataValuePropertyName));
                                }

                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    });
            }

            if (serviceDocumentElements[0] == null)
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(JsonLightConstants.ODataValuePropertyName));
            }

            // Read over the end object (nothing else can happen after all properties have been read)
            this.JsonReader.ReadEndObject();

            return new ODataServiceDocument
            {
                EntitySets = new ReadOnlyEnumerable<ODataEntitySetInfo>(serviceDocumentElements[0].OfType<ODataEntitySetInfo>().ToList()),
                FunctionImports = new ReadOnlyEnumerable<ODataFunctionImportInfo>(serviceDocumentElements[0].OfType<ODataFunctionImportInfo>().ToList()),
                Singletons = new ReadOnlyEnumerable<ODataSingletonInfo>(serviceDocumentElements[0].OfType<ODataSingletonInfo>().ToList())
            };
        }

        /// <summary>
        /// Reads a resource collection within a service document.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The <see cref="DuplicatePropertyNamesChecker"/> to use for parsing annotations within the service document element object.</param>
        /// <returns>A <see cref="ODataEntitySetInfo"/> representing the read resource collection.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:     The beginning of the JSON object representing the service document element.
        ///                 other:                        Will throw with an appropriate message on any other node type encountered.
        /// Post-Condition: JsonNodeType.StartObject:     The beginning of the next resource collection in the array.
        ///                 JsonNodeType.EndArray:        The end of the array.
        ///                 other:                        Any other node type occuring after the end object of the current service document element. (Would be invalid).
        /// </remarks>
        private ODataServiceDocumentElement ReadServiceDocumentElement(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            this.JsonReader.ReadStartObject();
            string[] name = { null };
            string[] url = { null };
            string[] kind = { null };
            string[] title = { null };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported in service document element objects.
                Func<string, object> propertyAnnotationValueReader = annotationName => { throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement(annotationName)); };

                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    propertyAnnotationValueReader,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement(propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataServiceDocumentElementName, propertyName) == 0)
                                {
                                    if (name[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementName));
                                    }

                                    name[0] = this.JsonReader.ReadStringValue();
                                }
                                else if (string.CompareOrdinal(JsonLightConstants.ODataServiceDocumentElementUrlName, propertyName) == 0)
                                {
                                    if (url[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementUrlName));
                                    }

                                    url[0] = this.JsonReader.ReadStringValue();
                                    ValidationUtils.ValidateServiceDocumentElementUrl(url[0]);
                                }
                                else if (string.CompareOrdinal(JsonLightConstants.ODataServiceDocumentElementKind, propertyName) == 0)
                                {
                                    if (kind[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementKind));
                                    }

                                    kind[0] = this.JsonReader.ReadStringValue();
                                }
                                else if (string.CompareOrdinal(JsonLightConstants.ODataServiceDocumentElementTitle, propertyName) == 0)
                                {
                                    if (title[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementTitle));
                                    }

                                    title[0] = this.JsonReader.ReadStringValue();
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement(propertyName, JsonLightConstants.ODataServiceDocumentElementName, JsonLightConstants.ODataServiceDocumentElementUrlName));
                                }

                                break;
                        }
                    });
            }

            // URL and Name are mandatory
            if (string.IsNullOrEmpty(name[0]))
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementName));
            }

            if (string.IsNullOrEmpty(url[0]))
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementUrlName));
            }

            ODataServiceDocumentElement serviceDocumentElement = null;
            if (kind[0] != null)
            {
                if (kind[0].Equals(JsonLightConstants.ServiceDocumentEntitySetKindName, StringComparison.Ordinal))
                {
                    serviceDocumentElement = new ODataEntitySetInfo();
                }
                else if (kind[0].Equals(JsonLightConstants.ServiceDocumentFunctionImportKindName, StringComparison.Ordinal))
                {
                    serviceDocumentElement = new ODataFunctionImportInfo();
                }
                else if (kind[0].Equals(JsonLightConstants.ServiceDocumentSingletonKindName, StringComparison.Ordinal))
                {
                    serviceDocumentElement = new ODataSingletonInfo();
                }
            }
            else
            {
                // if not specified its an entity set.
                serviceDocumentElement = new ODataEntitySetInfo();
            }

            if (serviceDocumentElement != null)
            {
                serviceDocumentElement.Url = this.ProcessUriFromPayload(url[0]);
                serviceDocumentElement.Name = name[0];
                serviceDocumentElement.Title = title[0];
            }

            this.JsonReader.ReadEndObject();

            return serviceDocumentElement;
        }
    }
}
