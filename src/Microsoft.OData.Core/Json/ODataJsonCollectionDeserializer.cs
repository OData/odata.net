//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for collections.
    /// </summary>
    internal sealed class ODataJsonCollectionDeserializer : ODataJsonPropertyAndValueDeserializer
    {
        /// <summary>Cached duplicate property names checker to use if the items are complex values.</summary>
        private readonly PropertyAndAnnotationCollector propertyAndAnnotationCollector;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonCollectionDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            this.propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
        }

        /// <summary>
        /// Reads the start of a collection; this includes collection-level properties (e.g., the 'results' property) if the version permits it.
        /// </summary>
        /// <param name="collectionStartPropertyAndAnnotationCollector">The duplicate property names checker used to keep track of the properties and annotations
        /// in the collection wrapper object.</param>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a parameter payload; otherwise false.</param>
        /// <param name="expectedItemTypeReference">The expected item type reference or null if none is expected.</param>
        /// <param name="actualItemTypeReference">The validated actual item type reference (if specified in the payload) or the expected item type reference.</param>
        /// <returns>An <see cref="ODataCollectionStart"/> representing the collection-level information.</returns>
        /// <remarks>
        /// Pre-Condition:  Any:                      the start of a nested collection value; if this is not a 'StartArray' node this method will fail.
        ///                 JsonNodeType.Property:    the first property of the collection wrapper object after the context URI.
        ///                 JsonNodeType.EndObject:   when the collection wrapper object has no properties (other than the context URI).
        /// Post-Condition: JsonNodeType.StartArray:  the start of the array of the collection items.
        /// </remarks>
        internal ODataCollectionStart ReadCollectionStart(
            PropertyAndAnnotationCollector collectionStartPropertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            IEdmTypeReference expectedItemTypeReference,
            out IEdmTypeReference actualItemTypeReference)
        {
            this.JsonReader.AssertNotBuffering();

            actualItemTypeReference = expectedItemTypeReference;

            ODataCollectionStart collectionStart = null;
            if (isReadingNestedPayload)
            {
                Debug.Assert(!this.JsonInputContext.ReadingResponse, "Nested collections are only supported in parameter payloads in requests.");
                collectionStart = new ODataCollectionStart
                {
                    Name = null
                };
            }
            else
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    IEdmTypeReference actualItemTypeRef = expectedItemTypeReference;
                    this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                    this.ProcessProperty(
                        collectionStartPropertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValue,
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
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                    }

                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(propertyName));

                                case PropertyParsingResult.PropertyWithValue:
                                    if (!string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                    {
                                        throw new ODataException(
                                            ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName(propertyName, ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    string payloadTypeName = ValidateDataPropertyTypeNameAnnotation(collectionStartPropertyAndAnnotationCollector, propertyName);
                                    if (payloadTypeName != null)
                                    {
                                        string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(payloadTypeName);
                                        if (itemTypeName == null)
                                        {
                                            throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_InvalidCollectionTypeName(payloadTypeName));
                                        }

                                        EdmTypeKind targetTypeKind;
                                        ODataTypeAnnotation typeAnnotation;
                                        Func<EdmTypeKind> typeKindFromPayloadFunc = () => { throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionStart_TypeKindFromPayloadFunc)); };
                                        actualItemTypeRef = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                                            EdmTypeKind.None,
                                            /*expectStructuredType*/ null,
                                            /*defaultPrimitivePayloadType*/ null,
                                            expectedItemTypeReference,
                                            itemTypeName,
                                            this.Model,
                                            typeKindFromPayloadFunc,
                                            out targetTypeKind,
                                            out typeAnnotation);
                                    }

                                    collectionStart = new ODataCollectionStart
                                    {
                                        Name = null
                                    };

                                    break;

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionStart));
                            }
                        });

                    actualItemTypeReference = actualItemTypeRef;
                }

                if (collectionStart == null)
                {
                    // No collection property found; there should be exactly one property in the collection wrapper that does not have a reserved name.
                    throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_ExpectedCollectionPropertyNotFound(ODataJsonConstants.ODataValuePropertyName));
                }
            }

            // at this point the reader is positioned on the start array node for the collection contents
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();

            return collectionStart;
        }

        /// <summary>
        /// Reads an item in the collection.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type of the item to read.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <returns>The value of the collection item that was read; this can be a primitive value or 'null'.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the item in the collection
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no more items in the collection
        /// </remarks>
        internal object ReadCollectionItem(IEdmTypeReference expectedItemTypeReference, CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            Debug.Assert(
                expectedItemTypeReference == null ||
                expectedItemTypeReference.IsODataPrimitiveTypeKind() ||
                expectedItemTypeReference.IsODataEnumTypeKind() ||
                expectedItemTypeReference.IsODataTypeDefinitionTypeKind(),
                "If an expected type is specified, it must be a primitive, enum type or type definition.");
            this.JsonReader.AssertNotBuffering();

            object item = this.ReadNonEntityValue(
                /*payloadTypeName*/ null,
                expectedItemTypeReference,
                this.propertyAndAnnotationCollector,
                collectionValidator,
                /*validateNullValue*/ true,
                /*isTopLevelPropertyValue*/ false,
                /*insideResourceValue*/ false,
                /*propertyName*/ null);

            this.JsonReader.AssertNotBuffering();

            return item;
        }

        /// <summary>
        /// Reads the end of a collection; this includes collection-level instance annotations.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a parameter payload; otherwise false.</param>
        /// <remarks>
        /// Pre-Condition:  EndArray node:      End of the collection content array
        /// Post-Condition: EndOfInput:         All of the collection payload has been consumed.
        /// </remarks>
        internal void ReadCollectionEnd(bool isReadingNestedPayload)
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-condition: JsonNodeType.EndArray");
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.ReadEndArray();

            if (!isReadingNestedPayload)
            {
                // Create a new duplicate property names checker object here; we don't have to use the one from reading the
                // collection start since we don't allow any annotations/properties after the collection property.
                PropertyAndAnnotationCollector collectionEndPropertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                // Fail on anything after the collection that is not a custom instance annotation
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                    this.ProcessProperty(
                        collectionEndPropertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValue,
                        (propertyParsingResult, propertyName) =>
                        {
                            if (this.JsonReader.NodeType == JsonNodeType.Property)
                            {
                                // Read over property name
                                this.JsonReader.Read();
                            }

                            // This method will allow and skip over any custom annotations, but will not report them as enum values, so any result we get other than EndOfObject indicates a malformed payload.
                            switch (propertyParsingResult)
                            {
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.ODataInstanceAnnotation:
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionEnd(propertyName));
                                    }

                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:        // fall through
                                case PropertyParsingResult.PropertyWithValue:           // fall through
                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionEnd(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionEnd));
                            }
                        });
                }

                // read the end-object node of the value containing the 'value' property
                this.JsonReader.ReadEndObject();
            }
        }

        /// <summary>
        /// Asynchronously reads the start of a collection; this includes collection-level properties (e.g., the 'results' property) if the version permits it.
        /// </summary>
        /// <param name="collectionStartPropertyAndAnnotationCollector">The duplicate property names checker used to keep track of the properties and annotations
        /// in the collection wrapper object.</param>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a parameter payload; otherwise false.</param>
        /// <param name="expectedItemTypeReference">The expected item type reference or null if none is expected.</param>
        /// <param name="actualItemTypeReference">The validated actual item type reference (if specified in the payload) or the expected item type reference.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1. An <see cref="ODataCollectionStart"/> representing the collection-level information.
        /// 2. The validated actual item type reference (if specified in the payload) or the expected item type reference.
        /// Currently this is only the name of the collection.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Any:                      the start of a nested collection value; if this is not a 'StartArray' node this method will fail.
        ///                 JsonNodeType.Property:    the first property of the collection wrapper object after the context URI.
        ///                 JsonNodeType.EndObject:   when the collection wrapper object has no properties (other than the context URI).
        /// Post-Condition: JsonNodeType.StartArray:  the start of the array of the collection items.
        /// </remarks>
        internal async Task<Tuple<ODataCollectionStart, IEdmTypeReference>> ReadCollectionStartAsync(
            PropertyAndAnnotationCollector collectionStartPropertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            IEdmTypeReference expectedItemTypeReference)
        {
            this.JsonReader.AssertNotBuffering();

            IEdmTypeReference actualItemTypeReference = expectedItemTypeReference;

            ODataCollectionStart collectionStart = null;

            if (isReadingNestedPayload)
            {
                Debug.Assert(!this.JsonInputContext.ReadingResponse, "Nested collections are only supported in parameter payloads in requests.");
                collectionStart = new ODataCollectionStart
                {
                    Name = null
                };
            }
            else
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    IEdmTypeReference actualItemTypeRef = expectedItemTypeReference;
                    this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                    await this.ProcessPropertyAsync(
                        collectionStartPropertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValueAsync,
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
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                    }

                                    await this.JsonReader.SkipValueAsync()
                                        .ConfigureAwait(false);
                                    break;

                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    await this.JsonReader.SkipValueAsync()
                                        .ConfigureAwait(false);
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(propertyName));

                                case PropertyParsingResult.PropertyWithValue:
                                    if (!string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                    {
                                        throw new ODataException(
                                            ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName(propertyName, ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    string payloadTypeName = ValidateDataPropertyTypeNameAnnotation(collectionStartPropertyAndAnnotationCollector, propertyName);
                                    if (payloadTypeName != null)
                                    {
                                        string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(payloadTypeName);
                                        if (itemTypeName == null)
                                        {
                                            throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_InvalidCollectionTypeName(payloadTypeName));
                                        }

                                        EdmTypeKind targetTypeKind;
                                        ODataTypeAnnotation typeAnnotation;
                                        Func<EdmTypeKind> typeKindFromPayloadFunc = () =>
                                        {
                                            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionStart_TypeKindFromPayloadFunc));
                                        };

                                        actualItemTypeRef = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                                            expectedTypeKind: EdmTypeKind.None,
                                            expectStructuredType: null,
                                            defaultPrimitivePayloadType: null,
                                            expectedTypeReference: expectedItemTypeReference,
                                            payloadTypeName: itemTypeName,
                                            model: this.Model,
                                            typeKindFromPayloadFunc: typeKindFromPayloadFunc,
                                            targetTypeKind: out targetTypeKind,
                                            typeAnnotation: out typeAnnotation);
                                    }

                                    collectionStart = new ODataCollectionStart
                                    {
                                        Name = null
                                    };

                                    break;

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                                default:
                                    throw new ODataException(
                                        ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionStart));
                            }
                        }).ConfigureAwait(false);

                    actualItemTypeReference = actualItemTypeRef;
                }

                if (collectionStart == null)
                {
                    // No collection property found; there should be exactly one property in the collection wrapper that does not have a reserved name.
                    throw new ODataException(
                        ODataErrorStrings.ODataJsonCollectionDeserializer_ExpectedCollectionPropertyNotFound(ODataJsonConstants.ODataValuePropertyName));
                }
            }

            // at this point the reader is positioned on the start array node for the collection contents
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();

            return Tuple.Create(collectionStart, actualItemTypeReference);
        }

        /// <summary>
        /// Asynchronously reads an item in the collection.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type of the item to read.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the collection item that was read; this can be a primitive value or 'null'.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the item in the collection
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no more items in the collection
        /// </remarks>
        internal async Task<object> ReadCollectionItemAsync(
            IEdmTypeReference expectedItemTypeReference,
            CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            Debug.Assert(
                expectedItemTypeReference == null ||
                expectedItemTypeReference.IsODataPrimitiveTypeKind() ||
                expectedItemTypeReference.IsODataEnumTypeKind() ||
                expectedItemTypeReference.IsODataTypeDefinitionTypeKind(),
                "If an expected type is specified, it must be a primitive, enum type or type definition.");
            this.JsonReader.AssertNotBuffering();

            object item = await this.ReadNonEntityValueAsync(
                payloadTypeName: null,
                expectedValueTypeReference: expectedItemTypeReference,
                propertyAndAnnotationCollector: this.propertyAndAnnotationCollector,
                collectionValidator: collectionValidator,
                validateNullValue: true,
                isTopLevelPropertyValue: false,
                insideResourceValue: false,
                propertyName: null).ConfigureAwait(false);

            this.JsonReader.AssertNotBuffering();

            return item;
        }

        /// <summary>
        /// Asynchronously reads the end of a collection; this includes collection-level instance annotations.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a parameter payload; otherwise false.</param>
        /// <remarks>
        /// Pre-Condition:  EndArray node:      End of the collection content array
        /// Post-Condition: EndOfInput:         All of the collection payload has been consumed.
        /// </remarks>
        internal async Task ReadCollectionEndAsync(bool isReadingNestedPayload)
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "Pre-condition: JsonNodeType.EndArray");
            this.JsonReader.AssertNotBuffering();

            await this.JsonReader.ReadEndArrayAsync()
                .ConfigureAwait(false);

            if (!isReadingNestedPayload)
            {
                // Create a new duplicate property names checker object here; we don't have to use the one from reading the
                // collection start since we don't allow any annotations/properties after the collection property.
                PropertyAndAnnotationCollector collectionEndPropertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                // Fail on anything after the collection that is not a custom instance annotation
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                    await this.ProcessPropertyAsync(
                        collectionEndPropertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValueAsync,
                        async (propertyParsingResult, propertyName) =>
                        {
                            if (this.JsonReader.NodeType == JsonNodeType.Property)
                            {
                                // Read over property name
                                await this.JsonReader.ReadAsync()
                                    .ConfigureAwait(false);
                            }

                            // This method will allow and skip over any custom annotations, but will not report them as enum values, so any result we get other than EndOfObject indicates a malformed payload.
                            switch (propertyParsingResult)
                            {
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    await this.JsonReader.SkipValueAsync()
                                        .ConfigureAwait(false);
                                    break;

                                case PropertyParsingResult.ODataInstanceAnnotation:
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionEnd(propertyName));
                                    }

                                    await this.JsonReader.SkipValueAsync()
                                        .ConfigureAwait(false);
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:        // fall through
                                case PropertyParsingResult.PropertyWithValue:           // fall through
                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionEnd(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                default:
                                    throw new ODataException(
                                        ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonCollectionDeserializer_ReadCollectionEnd));
                            }
                        }).ConfigureAwait(false);
                }

                // Read the end-object node of the value containing the 'value' property
                await this.JsonReader.ReadEndObjectAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns if a property is a valid OData annotation of a collection.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>If the property is a valid OData annotation of a collection.</returns>
        private static bool IsValidODataAnnotationOfCollection(string propertyName)
        {
            return string.Equals(ODataAnnotationNames.ODataCount, propertyName, StringComparison.Ordinal)
                || string.Equals(ODataAnnotationNames.ODataNextLink, propertyName, StringComparison.Ordinal);
        }
    }
}
