//---------------------------------------------------------------------
// <copyright file="ODataJsonServiceDocumentDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for service documents.
    /// </summary>
    internal sealed class ODataJsonServiceDocumentDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonServiceDocumentDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
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
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node
            this.ReadPayloadStart(
                ODataPayloadKind.ServiceDocument,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataServiceDocument serviceDocument = this.ReadServiceDocumentImplementation(propertyAndAnnotationCollector);

            // Read the end of the response.
            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return serviceDocument;
        }

        /// <summary>
        /// Asynchronously read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:        assumes that the JSON reader has not been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal async Task<ODataServiceDocument> ReadServiceDocumentAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            await this.ReadPayloadStartAsync(
                ODataPayloadKind.ServiceDocument,
                propertyAndAnnotationCollector,
                isReadingNestedPayload: false,
                allowEmptyPayload: false).ConfigureAwait(false);

            ODataServiceDocument serviceDocument = await this.ReadServiceDocumentImplementationAsync(propertyAndAnnotationCollector)
                .ConfigureAwait(false);

            // Read the end of the response.
            await this.ReadPayloadEndAsync(isReadingNestedPayload: false)
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return serviceDocument;
        }

        /// <summary>
        /// Read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property   The property right after the context URI property.
        ///                 JsonNodeType.EndObject  The EndObject of the service document.
        /// Post-Condition: Any                     The node after the EndObject of the service document.
        /// </remarks>
        private ODataServiceDocument ReadServiceDocumentImplementation(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            List<ODataServiceDocumentElement>[] serviceDocumentElements = { null };

            // Read all the properties in the service document object; we ignore all except 'value'.
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // Property annotations are not allowed on the 'value' property, so fail if we see one.
                Func<string, object> readPropertyAnnotationInServiceDoc = annotationName => { throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(annotationName, ODataJsonConstants.ODataValuePropertyName)); };

                this.ProcessProperty(
                    propertyAndAnnotationCollector,
                    readPropertyAnnotationInServiceDoc,
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
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(propertyName, ODataJsonConstants.ODataValuePropertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                {
                                    // Fail if we've already processed a 'value' property.
                                    if (serviceDocumentElements[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    serviceDocumentElements[0] = new List<ODataServiceDocumentElement>();

                                    // Read the value of the 'value' property.
                                    this.JsonReader.ReadStartArray();
                                    PropertyAndAnnotationCollector resourceCollectionPropertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                                    {
                                        ODataServiceDocumentElement serviceDocumentElement = this.ReadServiceDocumentElement(resourceCollectionPropertyAndAnnotationCollector);

                                        if (serviceDocumentElement != null)
                                        {
                                            serviceDocumentElements[0].Add(serviceDocumentElement);
                                        }

                                        resourceCollectionPropertyAndAnnotationCollector.Reset();
                                    }

                                    this.JsonReader.ReadEndArray();
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(propertyName, ODataJsonConstants.ODataValuePropertyName));
                                }

                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    });
            }

            if (serviceDocumentElements[0] == null)
            {
                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(ODataJsonConstants.ODataValuePropertyName));
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
        /// <param name="propertyAndAnnotationCollector">The <see cref="PropertyAndAnnotationCollector"/> to use for parsing annotations within the service document element object.</param>
        /// <returns>A <see cref="ODataEntitySetInfo"/> representing the read resource collection.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:     The beginning of the JSON object representing the service document element.
        ///                 other:                        Will throw with an appropriate message on any other node type encountered.
        /// Post-Condition: JsonNodeType.StartObject:     The beginning of the next resource collection in the array.
        ///                 JsonNodeType.EndArray:        The end of the array.
        ///                 other:                        Any other node type occuring after the end object of the current service document element. (Would be invalid).
        /// </remarks>
        private ODataServiceDocumentElement ReadServiceDocumentElement(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            this.JsonReader.ReadStartObject();
            string[] name = { null };
            string[] url = { null };
            string[] kind = { null };
            string[] title = { null };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported in service document element objects.
                Func<string, object> propertyAnnotationValueReader = annotationName =>
                {
                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement(annotationName));
                };

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
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement(propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementName, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementName, name);

                                    name[0] = this.JsonReader.ReadStringValue();
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementUrlName, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementUrlName, url);

                                    url[0] = this.JsonReader.ReadStringValue();
                                    ValidationUtils.ValidateServiceDocumentElementUrl(url[0]);
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementKind, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementKind, kind);

                                    kind[0] = this.JsonReader.ReadStringValue();
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementTitle, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementTitle, title);

                                    title[0] = this.JsonReader.ReadStringValue();
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement(
                                        propertyName,
                                        ODataJsonConstants.ODataServiceDocumentElementName,
                                        ODataJsonConstants.ODataServiceDocumentElementUrlName));
                                }

                                break;
                        }
                    });
            }

            // URL and Name are mandatory
            ValidateServiceDocumentElementHasRequiredProperty(ODataJsonConstants.ODataServiceDocumentElementName, name);
            ValidateServiceDocumentElementHasRequiredProperty(ODataJsonConstants.ODataServiceDocumentElementUrlName, url);

            ODataServiceDocumentElement serviceDocumentElement = CreateServiceDocumentElement(kind);

            if (serviceDocumentElement != null)
            {
                serviceDocumentElement.Url = this.ProcessUriFromPayload(url[0]);
                serviceDocumentElement.Name = name[0];
                serviceDocumentElement.Title = title[0];
            }

            this.JsonReader.ReadEndObject();

            return serviceDocumentElement;
        }

        /// <summary>
        /// Asynchronously read a service document.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataServiceDocument"/> representing the read service document.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property   The property right after the context URI property.
        ///                 JsonNodeType.EndObject  The EndObject of the service document.
        /// Post-Condition: Any                     The node after the EndObject of the service document.
        /// </remarks>
        private async Task<ODataServiceDocument> ReadServiceDocumentImplementationAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            List<ODataServiceDocumentElement>[] serviceDocumentElements = { null };

            // Read all the properties in the service document object; we ignore all except 'value'.
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // Property annotations are not allowed on the 'value' property, so fail if we see one.
                Func<string, Task<object>> readPropertyAnnotationInServiceDocumentAsync = annotationName =>
                {
                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(
                        annotationName,
                        ODataJsonConstants.ODataValuePropertyName));
                };

                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    readPropertyAnnotationInServiceDocumentAsync,
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
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(
                                    propertyName,
                                    ODataJsonConstants.ODataValuePropertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                await this.JsonReader.SkipValueAsync()
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                {
                                    // Fail if we've already processed a 'value' property.
                                    if (serviceDocumentElements[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(
                                            ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    serviceDocumentElements[0] = new List<ODataServiceDocumentElement>();

                                    // Read the value of the 'value' property.
                                    await this.JsonReader.ReadStartArrayAsync()
                                        .ConfigureAwait(false);
                                    PropertyAndAnnotationCollector resourceCollectionPropertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                                    {
                                        ODataServiceDocumentElement serviceDocumentElement = await this.ReadServiceDocumentElementAsync(
                                            resourceCollectionPropertyAndAnnotationCollector).ConfigureAwait(false);

                                        if (serviceDocumentElement != null)
                                        {
                                            serviceDocumentElements[0].Add(serviceDocumentElement);
                                        }

                                        resourceCollectionPropertyAndAnnotationCollector.Reset();
                                    }

                                    await this.JsonReader.ReadEndArrayAsync()
                                        .ConfigureAwait(false);
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(
                                        propertyName,
                                        ODataJsonConstants.ODataValuePropertyName));
                                }

                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    }).ConfigureAwait(false);
            }

            if (serviceDocumentElements[0] == null)
            {
                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(
                    ODataJsonConstants.ODataValuePropertyName));
            }

            // Read over the end object (nothing else can happen after all properties have been read)
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            return new ODataServiceDocument
            {
                EntitySets = new ReadOnlyEnumerable<ODataEntitySetInfo>(serviceDocumentElements[0].OfType<ODataEntitySetInfo>().ToList()),
                FunctionImports = new ReadOnlyEnumerable<ODataFunctionImportInfo>(serviceDocumentElements[0].OfType<ODataFunctionImportInfo>().ToList()),
                Singletons = new ReadOnlyEnumerable<ODataSingletonInfo>(serviceDocumentElements[0].OfType<ODataSingletonInfo>().ToList())
            };
        }

        /// <summary>
        /// Asynchronously reads a resource collection within a service document.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">
        /// The <see cref="PropertyAndAnnotationCollector"/> to use for parsing annotations within the service document element object.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataEntitySetInfo"/> representing the read resource collection.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:     The beginning of the JSON object representing the service document element.
        ///                 other:                        Will throw with an appropriate message on any other node type encountered.
        /// Post-Condition: JsonNodeType.StartObject:     The beginning of the next resource collection in the array.
        ///                 JsonNodeType.EndArray:        The end of the array.
        ///                 other:                        Any other node type occuring after the end object of the current service document element. (Would be invalid).
        /// </remarks>
        private async Task<ODataServiceDocumentElement> ReadServiceDocumentElementAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            await this.JsonReader.ReadStartObjectAsync()
                .ConfigureAwait(false);

            string[] name = { null };
            string[] url = { null };
            string[] kind = { null };
            string[] title = { null };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported in service document element objects.
                Func<string, Task<object>> propertyAnnotationValueReaderAsync = annotationName =>
                {
                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement(annotationName));
                };

                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    propertyAnnotationValueReaderAsync,
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
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement(propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                await this.JsonReader.SkipValueAsync()
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementName, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementName, name);

                                    name[0] = await this.JsonReader.ReadStringValueAsync()
                                        .ConfigureAwait(false);
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementUrlName, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementUrlName, url);

                                    url[0] = await this.JsonReader.ReadStringValueAsync()
                                        .ConfigureAwait(false);
                                    ValidationUtils.ValidateServiceDocumentElementUrl(url[0]);
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementKind, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementKind, kind);

                                    kind[0] = await this.JsonReader.ReadStringValueAsync()
                                        .ConfigureAwait(false);
                                }
                                else if (string.Equals(ODataJsonConstants.ODataServiceDocumentElementTitle, propertyName, StringComparison.Ordinal))
                                {
                                    ValidateServiceDocumentElementHasNoRepeatedProperty(ODataJsonConstants.ODataServiceDocumentElementTitle, title);

                                    title[0] = await this.JsonReader.ReadStringValueAsync()
                                        .ConfigureAwait(false);
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement(
                                        propertyName,
                                        ODataJsonConstants.ODataServiceDocumentElementName,
                                        ODataJsonConstants.ODataServiceDocumentElementUrlName));
                                }

                                break;
                        }
                    }).ConfigureAwait(false);
            }

            // URL and Name are mandatory
            ValidateServiceDocumentElementHasRequiredProperty(ODataJsonConstants.ODataServiceDocumentElementName, name);
            ValidateServiceDocumentElementHasRequiredProperty(ODataJsonConstants.ODataServiceDocumentElementUrlName, url);

            ODataServiceDocumentElement serviceDocumentElement = CreateServiceDocumentElement(kind);

            if (serviceDocumentElement != null)
            {
                serviceDocumentElement.Url = this.ProcessUriFromPayload(url[0]);
                serviceDocumentElement.Name = name[0];
                serviceDocumentElement.Title = title[0];
            }

            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            return serviceDocumentElement;
        }

        /// <summary>
        /// Creates a <see cref="ODataServiceDocumentElement"/>.
        /// </summary>
        /// <param name="kind">Property values for the "kind" property.</param>
        /// <returns>A <see cref="ODataServiceDocumentElement"/> instance.</returns>
        private static ODataServiceDocumentElement CreateServiceDocumentElement(string[] kind)
        {
            // If not specified its an entity set.
            if (kind[0] == null)
            {
                return new ODataEntitySetInfo();
            }

            ODataServiceDocumentElement serviceDocumentElement = null;

            if (kind[0].Equals(ODataJsonConstants.ServiceDocumentEntitySetKindName, StringComparison.Ordinal))
            {
                serviceDocumentElement = new ODataEntitySetInfo();
            }
            else if (kind[0].Equals(ODataJsonConstants.ServiceDocumentFunctionImportKindName, StringComparison.Ordinal))
            {
                serviceDocumentElement = new ODataFunctionImportInfo();
            }
            else if (kind[0].Equals(ODataJsonConstants.ServiceDocumentSingletonKindName, StringComparison.Ordinal))
            {
                serviceDocumentElement = new ODataSingletonInfo();
            }

            return serviceDocumentElement;
        }

        /// <summary>
        /// Validates that the specified property in the service document element is not repeated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="propertyValues">Property values for the specified property.</param>
        private static void ValidateServiceDocumentElementHasNoRepeatedProperty(string property, string[] propertyValues)
        {
            Debug.Assert(propertyValues != null && propertyValues.Length > 0, $"{nameof(propertyValues)} != null && {nameof(propertyValues)}.Length > 0");

            if (propertyValues[0] != null)
            {
                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(property));
            }
        }

        /// <summary>
        /// Validates that the specified required property in the service document element has been read and value is not null.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="propertyValues">Property values for the specified property.</param>
        private static void ValidateServiceDocumentElementHasRequiredProperty(string property, string[] propertyValues)
        {
            Debug.Assert(propertyValues != null && propertyValues.Length > 0, $"{nameof(propertyValues)} != null && {nameof(propertyValues)}.Length > 0");

            if (string.IsNullOrEmpty(propertyValues[0]))
            {
                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(property));
            }
        }
    }
}
