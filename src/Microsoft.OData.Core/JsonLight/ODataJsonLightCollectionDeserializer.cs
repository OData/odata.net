//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for collections.
    /// </summary>
    internal sealed class ODataJsonLightCollectionDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>Cached duplicate property names checker to use if the items are complex values.</summary>
        private readonly DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightCollectionDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            this.duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
        }

        /// <summary>
        /// Reads the start of a collection; this includes collection-level properties (e.g., the 'results' property) if the version permits it.
        /// </summary>
        /// <param name="collectionStartDuplicatePropertyNamesChecker">The duplicate property names checker used to keep track of the properties and annotations
        /// in the collection wrapper object.</param>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a paramter payload; otherwise false.</param>
        /// <param name="expectedItemTypeReference">The expected item type reference or null if none is expected.</param>
        /// <param name="actualItemTypeReference">The validated actual item type reference (if specified in the payload) or the expected item type reference.</param>
        /// <returns>An <see cref="ODataCollectionStart"/> representing the collection-level information. Currently this is only the name of the collection in ATOM.</returns>
        /// <remarks>
        /// Pre-Condition:  Any:                      the start of a nested collection value; if this is not a 'StartArray' node this method will fail.
        ///                 JsonNodeType.Property:    the first property of the collection wrapper object after the context URI.
        ///                 JsonNodeType.EndObject:   when the collection wrapper object has no properties (other than the context URI).
        /// Post-Condition: JsonNodeType.StartArray:  the start of the array of the collection items.
        /// </remarks>
        internal ODataCollectionStart ReadCollectionStart(
            DuplicatePropertyNamesChecker collectionStartDuplicatePropertyNamesChecker, 
            bool isReadingNestedPayload,
            IEdmTypeReference expectedItemTypeReference,
            out IEdmTypeReference actualItemTypeReference)
        {
            this.JsonReader.AssertNotBuffering();

            actualItemTypeReference = expectedItemTypeReference;

            ODataCollectionStart collectionStart = null;
            if (isReadingNestedPayload)
            {
                Debug.Assert(!this.JsonLightInputContext.ReadingResponse, "Nested collections are only supported in parameter payloads in requests.");
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
                    this.ProcessProperty(
                        collectionStartDuplicatePropertyNamesChecker,
                        this.ReadTypePropertyAnnotationValue,
                        (propertyParsingResult, propertyName) =>
                        {
                            switch (propertyParsingResult)
                            {
                                case PropertyParsingResult.ODataInstanceAnnotation:
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyName));
                                    }

                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(propertyName));

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) != 0)
                                    {
                                        throw new ODataException(
                                            ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName(propertyName, JsonLightConstants.ODataValuePropertyName));
                                    }

                                    string payloadTypeName = ValidateDataPropertyTypeNameAnnotation(collectionStartDuplicatePropertyNamesChecker, propertyName);
                                    if (payloadTypeName != null)
                                    {
                                        string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(payloadTypeName);
                                        if (itemTypeName == null)
                                        {
                                            throw new ODataException(ODataErrorStrings.ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName(payloadTypeName));
                                        }

                                        EdmTypeKind targetTypeKind;
                                        SerializationTypeNameAnnotation serializationTypeNameAnnotation;
                                        Func<EdmTypeKind> typeKindFromPayloadFunc = () => { throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionStart_TypeKindFromPayloadFunc)); };
                                        actualItemTypeRef = ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(
                                            EdmTypeKind.None,
                                            /*defaultPrimitivePayloadType*/ null,
                                            expectedItemTypeReference,
                                            itemTypeName,
                                            this.Model,
                                            this.MessageReaderSettings,
                                            typeKindFromPayloadFunc,
                                            out targetTypeKind,
                                            out serializationTypeNameAnnotation);
                                    }

                                    collectionStart = new ODataCollectionStart
                                    {
                                        Name = null
                                    };

                                    break;

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionStart));
                            }
                        });

                    actualItemTypeReference = actualItemTypeRef;
                }

                if (collectionStart == null)
                {
                    // No collection property found; there should be exactly one property in the collection wrapper that does not have a reserved name.
                    throw new ODataException(ODataErrorStrings.ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound(JsonLightConstants.ODataValuePropertyName));
                }
            }

            // at this point the reader is positioned on the start array node for the collection contents
            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.AssertNotBuffering();

            return collectionStart;
        }

        /// <summary>
        /// Reads an item in the collection.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type of the item to read.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <returns>The value of the collection item that was read; this can be an ODataComplexValue, a primitive value or 'null'.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the item in the collection
        ///                 NOTE: this method will throw if the node is not
        ///                 JsonNodeType.StartObject:    for a complex item
        ///                 JsonNodeType.PrimitiveValue: for a primitive item
        /// Post-Condition: The reader is positioned on the first node of the next item or an EndArray node if there are no more items in the collection
        /// </remarks>
        internal object ReadCollectionItem(IEdmTypeReference expectedItemTypeReference, CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            Debug.Assert(
                expectedItemTypeReference == null ||
                expectedItemTypeReference.IsODataPrimitiveTypeKind() ||
                expectedItemTypeReference.IsODataComplexTypeKind() ||
                expectedItemTypeReference.IsODataEnumTypeKind() ||
                expectedItemTypeReference.IsODataTypeDefinitionTypeKind(),
                "If an expected type is specified, it must be a primitive, complex type, enum type or type definition.");
            this.JsonReader.AssertNotBuffering();

            object item = this.ReadNonEntityValue(
                /*payloadTypeName*/ null,
                expectedItemTypeReference, 
                this.duplicatePropertyNamesChecker, 
                collectionValidator,
                /*validateNullValue*/ true,
                /*isTopLevelPropertyValue*/ false,
                /*insideComplexValue*/ false,
                /*propertyName*/ null);

            this.JsonReader.AssertNotBuffering();

            return item;
        }

        /// <summary>
        /// Reads the end of a collection; this includes collection-level instance annotations.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are reading a nested collection inside a paramter payload; otherwise false.</param>
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
                DuplicatePropertyNamesChecker collectionEndDuplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

                // Fail on anything after the collection that is not a custom instance annotation
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    this.ProcessProperty(
                        collectionEndDuplicatePropertyNamesChecker,
                        this.ReadTypePropertyAnnotationValue,
                        (propertyParsingResult, propertyName) =>
                        {
                            // This method will allow and skip over any custom annotations, but will not report them as enum values, so any result we get other than EndOfObject indicates a malformed payload.
                            switch (propertyParsingResult)
                            {
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.ODataInstanceAnnotation:
                                    if (!IsValidODataAnnotationOfCollection(propertyName))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd(propertyName));
                                    }

                                    this.JsonReader.SkipValue();
                                    break;

                                case PropertyParsingResult.PropertyWithoutValue:        // fall through
                                case PropertyParsingResult.PropertyWithValue:           // fall through
                                case PropertyParsingResult.MetadataReferenceProperty:
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionEnd));
                            }
                        });
                }

                // read the end-object node of the value containing the 'value' property
                this.JsonReader.ReadEndObject();
            }
        }

        /// <summary>
        /// Returns if a property is a valid OData annotation of a collection.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>If the property is a valid OData annotation of a collection.</returns>
        private static bool IsValidODataAnnotationOfCollection(string propertyName)
        {
            return string.CompareOrdinal(ODataAnnotationNames.ODataCount, propertyName) == 0
                || string.CompareOrdinal(ODataAnnotationNames.ODataNextLink, propertyName) == 0;
        }
    }
}
