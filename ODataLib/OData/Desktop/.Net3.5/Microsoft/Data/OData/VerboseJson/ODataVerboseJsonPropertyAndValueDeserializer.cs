//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON deserializer for properties and value types.
    /// </summary>
    internal class ODataVerboseJsonPropertyAndValueDeserializer : ODataVerboseJsonDeserializer
    {
        /// <summary>
        /// The current recursion depth of values read by this deserializer, measured by the number of complex, collection, JSON object and JSON array values read so far.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The JSON input context to read from.</param>
        internal ODataVerboseJsonPropertyAndValueDeserializer(ODataVerboseJsonInputContext verboseJsonInputContext)
            : base(verboseJsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedProperty">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal ODataProperty ReadTopLevelProperty(IEdmStructuralProperty expectedProperty, IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            this.JsonReader.AssertNotBuffering();

            if (!this.Model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata);
            }

            // Read the response wrapper "d" if expected.
            this.ReadPayloadStart(false /*isReadingNestedPayload*/);

            string expectedPropertyName = ReaderUtils.GetExpectedPropertyName(expectedProperty);

            string propertyName = null;
            object propertyValue = null;

            // We have to support reading top-level complex values without the { "property": ... } wrapper for open properties
            // in WCF DS Server backward compat mode. (Open property == property without expected type for us).
            if (this.ShouldReadTopLevelPropertyValueWithoutPropertyWrapper(expectedPropertyTypeReference))
            {
                // We will report the value without property wrapper as a property with empty name (this is technically invalid, but it will only happen in WCF DS Server mode).
                propertyName = expectedPropertyName ?? string.Empty;

                // Read the value directly
                propertyValue = this.ReadNonEntityValue(
                    expectedPropertyTypeReference, 
                    /*duplicatePropertyNamesChecker*/ null, 
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    propertyName);
            }
            else
            {
                // Read the start of the object container around the property { "property": ... }
                this.JsonReader.ReadStartObject();

                // Read through all top-level properties, ignore the ones with reserved names (i.e., reserved 
                // characters in their name) and throw if we find none or more than one properties without reserved name.
                bool foundProperty = false;
                string foundPropertyName = null;
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    // Read once - this should be the property
                    propertyName = this.JsonReader.ReadPropertyName();

                    if (!ValidationUtils.IsValidPropertyName(propertyName))
                    {
                        // We ignore properties with an invalid name since these are extension points for the future.
                        this.JsonReader.SkipValue();
                    }
                    else
                    {
                        if (foundProperty)
                        {
                            // There should be only one property in the top-level property wrapper that does not have a reserved name.
                            throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
                        }

                        foundProperty = true;
                        foundPropertyName = propertyName;

                        // Now read the property value
                        propertyValue = this.ReadNonEntityValue(
                            expectedPropertyTypeReference, 
                            /*duplicatePropertyNamesChecker*/ null,
                            /*collectionValidator*/ null,
                            /*validateNullValue*/ true,
                            propertyName);
                    }
                }

                if (!foundProperty)
                {
                    // No property found; there should be exactly one property in the top-level property wrapper that does not have a reserved name.
                    throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
                }

                Debug.Assert(foundPropertyName != null, "foundPropertyName != null");
                ReaderValidationUtils.ValidateExpectedPropertyName(expectedPropertyName, foundPropertyName);
                propertyName = foundPropertyName;

                // Read over the end object - note that this might be the last node in the input (in case there's no response wrapper)
                this.JsonReader.Read();
            }

            // Read the end of the response wrapper "d" if expected.
            this.ReadPayloadEnd(false /*isReadingNestedPayload*/);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            Debug.Assert(propertyName != null, "propertyName != null");

            return new ODataProperty
            {
                Name = propertyName,
                Value = propertyValue
            };
        }

        /// <summary>
        /// Reads an entry, complex or collection content in buffering mode until it finds the type name in the __metadata object
        /// or hits the end of the object. If called for a primitive value, returns 'null' (since primitive types cannot have
        /// type names in JSON)
        /// </summary>
        /// <returns>The type name as read from the __metadata object; null if none was found.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         A primitive value
        ///                 JsonNodeType.StartObject            Any non-primitive value
        /// Post-Condition: JsonNodeType.PrimitiveValue         A primitive value
        ///                 JsonNodeType.StartObject            Any non-primitive value
        /// </remarks>
        internal string FindTypeNameInPayload()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                this.JsonReader.NodeType == JsonNodeType.StartObject,
                "Pre-Condition: JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                // Primitive values don't have type names in JSON
                return null;
            }

            // start buffering so we don't move the reader
            this.JsonReader.StartBuffering();

            // Read the start of the object
            this.JsonReader.ReadStartObject();

            string typeName = null;

            // read through all the properties and find the __metadata one (if it exists)
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                if (string.CompareOrdinal(propertyName, JsonConstants.ODataMetadataName) == 0)
                {
                    // now extract the typename property (if present)
                    //
                    // NOTE we do strictly more work than absolutely necessary by calling this 
                    //      method here since it will read all of the metadata object and not just
                    //      up to the typename; since we will come back and read the metadata object
                    //      later it should not be an issue.
                    typeName = this.ReadTypeNameFromMetadataPropertyValue();
                    break;
                }
                else
                {
                    // skip over the value of this property
                    this.JsonReader.SkipValue();
                }
            }

            // stop buffering and reset the reader to its previous state
            this.JsonReader.StopBuffering();

            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                this.JsonReader.NodeType == JsonNodeType.StartObject,
                "Post-Condition: JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");

            return typeName;
        }

        /// <summary>
        /// Reads a primitive value, complex value or collection.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the property value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The value of the property read.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value
        ///                 JsonNodeType.StartObject      - the value of the property is an object
        ///                 JsonNodeType.StartArray       - the value of the property is an array - method will fail in this case.
        /// Post-Condition: almost anything - the node after the property value.
        ///                 
        /// Returns the value of the property read, which can be one of:
        /// - null
        /// - primitive value
        /// - <see cref="ODataComplexValue"/>
        /// - <see cref="ODataCollectionValue"/>
        /// </remarks>
        internal object ReadNonEntityValue(
            IEdmTypeReference expectedValueTypeReference,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            CollectionWithoutExpectedTypeValidator collectionValidator, 
            bool validateNullValue,
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            this.AssertRecursionDepthIsZero();
            object nonEntityValue = this.ReadNonEntityValueImplementation(
                expectedValueTypeReference, 
                duplicatePropertyNamesChecker, 
                collectionValidator,
                validateNullValue, 
                propertyName);
            this.AssertRecursionDepthIsZero();

            return nonEntityValue;
        }

        /// <summary>
        /// Reads the type name from the value of a __metadata property. All other properties in the __metadata property value are ignored.
        /// </summary>
        /// <returns>The type name found, or null if none was found.</returns>
        /// <remarks>
        /// This method can be used in buffering and non-buffering mode.
        /// 
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.Property - the next property after the __metadata property value.
        ///                 JsonNodeType.EndObject - if the __metadata property was the last property in the object.
        /// </remarks>
        internal string ReadTypeNameFromMetadataPropertyValue()
        {
            DebugUtils.CheckNoExternalCallers();

            string typeName = null;

            // The value of the __metadata property must be an object
            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_MetadataPropertyMustHaveObjectValue(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartObject();

            // Go over the properties and look for the type property.
            ODataVerboseJsonReaderUtils.MetadataPropertyBitMask propertiesFoundBitField = 0;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataMetadataTypeName, propertyName) == 0)
                {
                    ODataVerboseJsonReaderUtils.VerifyMetadataPropertyNotFound(
                        ref propertiesFoundBitField,
                        ODataVerboseJsonReaderUtils.MetadataPropertyBitMask.Type,
                        JsonConstants.ODataMetadataTypeName);
                    object typeNameValue = this.JsonReader.ReadPrimitiveValue();
                    typeName = typeNameValue as string;
                    if (typeName == null)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName(typeNameValue));
                    }
                }
                else
                {
                    // Skip over the property value.
                    this.JsonReader.SkipValue();
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "Only Property or EndObject can occur in Object scope.");
            this.JsonReader.ReadEndObject();

            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");

            return typeName;
        }

        /// <summary>
        /// Reads a primitive value.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The value of the primitive value.</returns>
        /// <remarks>
        /// Pre-Condition:  none - Fails if the current node is not a JsonNodeType.PrimitiveValue
        /// Post-Condition: almost anything - the node after the primitive value.
        /// 
        /// Made internal only for testability.
        /// </remarks>
        internal object ReadPrimitiveValue(IEdmPrimitiveTypeReference expectedValueTypeReference, bool validateNullValue, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            object result;

            if (expectedValueTypeReference != null && expectedValueTypeReference.IsSpatial())
            {
                result = ODataJsonReaderCoreUtils.ReadSpatialValue(
                    this.JsonReader,
                    /*insideJsonObjectValue*/ false,
                    this.VerboseJsonInputContext,
                    expectedValueTypeReference,
                    validateNullValue,
                    this.recursionDepth,
                    propertyName);
            }
            else
            {
                result = this.JsonReader.ReadPrimitiveValue();

                if (expectedValueTypeReference != null && !this.MessageReaderSettings.DisablePrimitiveTypeConversion)
                {
                    result = ODataVerboseJsonReaderUtils.ConvertValue(
                        result, 
                        expectedValueTypeReference, 
                        this.MessageReaderSettings, 
                        this.Version,
                        validateNullValue, 
                        propertyName);
                }
            }

            this.JsonReader.AssertNotBuffering();

            return result;
        }

        /// <summary>
        /// Reads a collection value.
        /// </summary>
        /// <param name="collectionValueTypeReference">The collection type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="serializationTypeNameAnnotation">The serialization type name for the collection value (possibly null).</param>
        /// <returns>The value of the collection.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: almost anything - the node after the collection value (after the EndObject)
        /// </remarks>
        private ODataCollectionValue ReadCollectionValueImplementation(
            IEdmCollectionTypeReference collectionValueTypeReference, 
            string payloadTypeName,
            SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                collectionValueTypeReference == null || collectionValueTypeReference.IsNonEntityCollectionType(),
                "If the metadata is specified it must denote a Collection for this method to work.");
            this.JsonReader.AssertNotBuffering();

            ODataVersionChecker.CheckCollectionValue(this.Version);

            this.IncreaseRecursionDepth();

            // Read over the start object
            this.JsonReader.ReadStartObject();

            ODataCollectionValue collectionValue = new ODataCollectionValue();
            collectionValue.TypeName = collectionValueTypeReference != null ? collectionValueTypeReference.ODataFullName() : payloadTypeName;
            if (serializationTypeNameAnnotation != null)
            {
                collectionValue.SetAnnotation(serializationTypeNameAnnotation);
            }

            List<object> items = null;
            bool metadataPropertyFound = false;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                if (string.CompareOrdinal(JsonConstants.ODataMetadataName, propertyName) == 0)
                {
                    // __metadata property
                    if (metadataPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInCollectionWrapper(JsonConstants.ODataMetadataName));
                    }

                    metadataPropertyFound = true;

                    // Note that we don't need to read the type name again, as we've already read it above in FindTypeNameInPayload.
                    // There's nothing else of interest in the __metadata for collections.
                    this.JsonReader.SkipValue();
                }
                else if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                {
                    // results property
                    if (items != null)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInCollectionWrapper(JsonConstants.ODataResultsName));
                    }

                    items = new List<object>();

                    this.JsonReader.ReadStartArray();

                    DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                    IEdmTypeReference itemType = null;
                    if (collectionValueTypeReference != null)
                    {
                        itemType = collectionValueTypeReference.CollectionDefinition().ElementType;
                    }

                    // NOTE: we do not support reading Verbose JSON without metadata right now so we always have an expected item type;
                    //       The collection validator is always null.
                    CollectionWithoutExpectedTypeValidator collectionValidator = null;

                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        object itemValue = this.ReadNonEntityValueImplementation(
                            itemType,
                            duplicatePropertyNamesChecker,
                            collectionValidator,
                            /*validateNullValue*/ true,
                            /*propertyName*/ null);

                        // Validate the item (for example that it's not null)
                        ValidationUtils.ValidateCollectionItem(itemValue, false /* isStreamable */);

                        // Note that the ReadNonEntityValue already validated that the actual type of the value matches
                        // the expected type (the itemType).
                        items.Add(itemValue);
                    }

                    Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "The results value must end with an end array.");
                    this.JsonReader.ReadEndArray();
                }
                else
                {
                    // Skip over any other property in the collection object
                    this.JsonReader.SkipValue();
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all properties of Collection wrapper are read the EndObject node is expected.");
            this.JsonReader.ReadEndObject();

            if (items == null)
            {
                // We didn't find any results property. All collections must have the results property.
                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_CollectionWithoutResults);
            }

            collectionValue.Items = new ReadOnlyEnumerable(items);

            this.JsonReader.AssertNotBuffering();
            this.DecreaseRecursionDepth();

            return collectionValue;
        }

        /// <summary>
        /// Reads a complex value.
        /// </summary>
        /// <param name="complexValueTypeReference">The expected type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="serializationTypeNameAnnotation">The serialization type name for the collection value (possibly null).</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <returns>The value of the complex value.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject or JsonNodeType.PrimitiveValue (with null value)
        /// Post-Condition: almost anything - the node after the complex value (after the EndObject)
        /// </remarks>
        private ODataComplexValue ReadComplexValueImplementation(
            IEdmComplexTypeReference complexValueTypeReference, 
            string payloadTypeName,
            SerializationTypeNameAnnotation serializationTypeNameAnnotation,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            this.JsonReader.AssertNotBuffering();

            this.IncreaseRecursionDepth();

            // Read over the start object
            this.JsonReader.ReadStartObject();

            ODataComplexValue complexValue = new ODataComplexValue();

            complexValue.TypeName = complexValueTypeReference != null ? complexValueTypeReference.ODataFullName() : payloadTypeName; 
            if (serializationTypeNameAnnotation != null)
            {
                complexValue.SetAnnotation(serializationTypeNameAnnotation);
            }

            if (duplicatePropertyNamesChecker == null)
            {
                duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
            }
            else
            {
                duplicatePropertyNamesChecker.Clear();
            }

            List<ODataProperty> properties = new List<ODataProperty>();
            bool metadataPropertyFound = false;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataMetadataName, propertyName) == 0)
                {
                    // __metadata property.
                    if (metadataPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue);
                    }

                    metadataPropertyFound = true;

                    this.JsonReader.SkipValue();
                }
                else
                {
                    if (!ValidationUtils.IsValidPropertyName(propertyName))
                    {
                        // We ignore properties with an invalid name since these are extension points for the future.
                        this.JsonReader.SkipValue();
                    }
                    else
                    {
                        // Any other property is data
                        ODataProperty property = new ODataProperty();
                        property.Name = propertyName;

                        // Lookup the property in metadata
                        IEdmProperty edmProperty = null;
                        bool ignoreProperty = false;
                        if (complexValueTypeReference != null)
                        {
                            edmProperty = ReaderValidationUtils.ValidateValuePropertyDefined(propertyName, complexValueTypeReference.ComplexDefinition(), this.MessageReaderSettings, out ignoreProperty);
                        }

                        if (ignoreProperty)
                        {
                            this.JsonReader.SkipValue();
                        }
                        else
                        {
                            ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse || edmProperty == null
                                ? ODataNullValueBehaviorKind.Default
                                : this.Model.NullValueReadBehaviorKind(edmProperty);

                            // Read the property value
                            object propertyValue = this.ReadNonEntityValueImplementation(
                                edmProperty == null ? null : edmProperty.Type,
                                /*duplicatePropertyNamesChecker*/ null,
                                /*collectionValidator*/ null,
                                nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default, 
                                propertyName);

                            if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.IgnoreValue || propertyValue != null)
                            {
                                duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
                                property.Value = propertyValue;
                                properties.Add(property);
                            }
                        }
                    }
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all the properties of a complex value are read the EndObject node is expected.");
            this.JsonReader.ReadEndObject();

            complexValue.Properties = new ReadOnlyEnumerable<ODataProperty>(properties);

            this.JsonReader.AssertNotBuffering();
            this.DecreaseRecursionDepth();

            return complexValue;
        }

        /// <summary>
        /// Reads a primitive, complex or collection value.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference of the property value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable.</param>
        /// <returns>The value of the property read.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value
        ///                 JsonNodeType.StartObject      - the value of the property is an object
        ///                 JsonNodeType.StartArray       - the value of the property is an array - method will fail in this case.
        /// Post-Condition: almost anything - the node after the property value.
        ///                 
        /// Returns the value of the property read, which can be one of:
        /// - null
        /// - primitive value
        /// - <see cref="ODataComplexValue"/>
        /// - <see cref="ODataCollectionValue"/>
        /// </remarks>
        private object ReadNonEntityValueImplementation(
            IEdmTypeReference expectedTypeReference,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject || this.JsonReader.NodeType == JsonNodeType.StartArray,
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject or JsonNodeType.StartArray");
            Debug.Assert(
                expectedTypeReference == null || !expectedTypeReference.IsODataEntityTypeKind(),
                "Only primitive, complex or collection types can be read by this method.");
            Debug.Assert(
                expectedTypeReference == null || collectionValidator == null,
                "If an expected value type reference is specified, no collection validator must be provided.");
            this.JsonReader.AssertNotBuffering();

            // Property values can be only primitives or objects. No property can have a JSON array value.
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue(nodeType));
            }

            // Try to read a null value
            object result;
            if (ODataJsonReaderCoreUtils.TryReadNullValue(
                this.JsonReader,
                this.VerboseJsonInputContext,
                expectedTypeReference, 
                validateNullValue,
                propertyName))
            {
                result = null;
            }
            else
            {
                // Read the payload type name
                string payloadTypeName = this.FindTypeNameInPayload();

                SerializationTypeNameAnnotation serializationTypeNameAnnotation;
                EdmTypeKind targetTypeKind;
                IEdmTypeReference targetTypeReference = ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*defaultPrimitivePayloadType*/ null,
                    expectedTypeReference,
                    payloadTypeName,
                    this.Model,
                    this.MessageReaderSettings,
                    this.Version,
                    this.GetNonEntityValueKind,
                    out targetTypeKind,
                    out serializationTypeNameAnnotation);

                switch (targetTypeKind)
                {
                    case EdmTypeKind.Primitive:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                        IEdmPrimitiveTypeReference primitiveTargetTypeReference = targetTypeReference == null ? null : targetTypeReference.AsPrimitive();
                        if (payloadTypeName != null && !primitiveTargetTypeReference.IsSpatial())
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidPrimitiveTypeName(payloadTypeName));
                        }

                        result = this.ReadPrimitiveValue(
                            primitiveTargetTypeReference,
                            validateNullValue,
                            propertyName);
                        break;
                    case EdmTypeKind.Complex:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsComplex(), "Expected a complex type.");
                        result = this.ReadComplexValueImplementation(
                            targetTypeReference == null ? null : targetTypeReference.AsComplex(),
                            payloadTypeName,
                            serializationTypeNameAnnotation,
                            duplicatePropertyNamesChecker);
                        break;
                    case EdmTypeKind.Collection:
                        Debug.Assert(this.Version >= ODataVersion.V3, "Type resolution should already fail if we would decide to read a collection value in V1/V2 payload.");
                        IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(targetTypeReference);
                        result = this.ReadCollectionValueImplementation(
                            collectionTypeReference,
                            payloadTypeName,
                            serializationTypeNameAnnotation);
                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataVerboseJsonPropertyAndValueDeserializer_ReadPropertyValue));
                }

                // If we have no expected type make sure the collection items are of the same kind and specify the same name.
                if (collectionValidator != null)
                {
                    string payloadTypeNameFromResult = ODataVerboseJsonReaderUtils.GetPayloadTypeName(result);
                    Debug.Assert(expectedTypeReference == null, "If a collection validator is specified there must not be an expected value type reference.");
                    collectionValidator.ValidateCollectionItem(payloadTypeNameFromResult, targetTypeKind);
                }
            }

            this.JsonReader.AssertNotBuffering();
            return result;
        }

        /// <summary>
        /// Determines the value kind for a non-entity value (that is top-level property value, property value on a complex type, item in a collection)
        /// </summary>
        /// <returns>The type kind of the property value.</returns>
        /// <remarks>
        /// Doesn't move the JSON reader.
        /// Pre-Condition:  JsonNodeType.PrimitiveValue
        ///                 JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue
        ///                 JsonNodeType.StartObject
        /// </remarks>
        private EdmTypeKind GetNonEntityValueKind()
        {
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject,
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");
            this.JsonReader.AssertNotBuffering();

            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.PrimitiveValue)
            {
                return EdmTypeKind.Primitive;
            }
            else
            {
                Debug.Assert(nodeType == JsonNodeType.StartObject, "StartObject expected.");

                // It can either be a complex value, a collection value or a spatial value.
                // We only get here if we did not find a type name in the payload and we 
                // have to figure out hte payload kind from the shape of the payload.
                // A collection value has a 'results' property which is an array.
                // Spatial and complex values don't have any properties array value, 
                // so that's an easy thing to recognize.
                // Once we established that it is not a collection value, we cannot distinguish
                // a complex value from a spatial value; we report 'Complex' in that case since
                // that will result in an appropriate error message (also for spatial values).
                // We need to buffer since once detected we need to go back and reread the real value.
                this.JsonReader.StartBuffering();

                try
                {
                    // Read over the start object - the start of the complex or collection
                    this.JsonReader.ReadNext();

                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        string propertyName = this.JsonReader.ReadPropertyName();
                        if (string.Equals(JsonConstants.ODataResultsName, propertyName, StringComparison.Ordinal))
                        {
                            // "results" property, it might be a normal property of a complex type
                            // so we need to check it's value, if it's an array, it's a collection.
                            if (this.JsonReader.NodeType == JsonNodeType.StartArray && this.Version >= ODataVersion.V3)
                            {
                                return EdmTypeKind.Collection;
                            }

                            // If it's not an array, it's a normal property on a complex type
                            return EdmTypeKind.Complex;
                        }

                        // Skip over the property value
                        this.JsonReader.SkipValue();
                    }

                    // If we end here, it means we didn't find enough properties to tell if we're looking at collection or complex type
                    // In which case it must be a complex type, since collection requires the results property.
                    return EdmTypeKind.Complex;
                }
                finally
                {
                    // Stop the buffering, since we're done deciding the type of the value.
                    this.JsonReader.StopBuffering();
                    this.JsonReader.AssertNotBuffering();
                    Debug.Assert(
                        this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject,
                        "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");
                }
            }
        }

        /// <summary>
        /// Determines if the top-level property payload should be read as usual, or without the property wrapper.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference for the property value to read.</param>
        /// <returns>true if the property payload should be read without the property wrapper, false if it should be read as usual with the property wrapper.</returns>
        /// <remarks>This method is to support backward compat behavior for WCF DS Server, which can read open property values without property wrapper.</remarks>
        private bool ShouldReadTopLevelPropertyValueWithoutPropertyWrapper(IEdmTypeReference expectedPropertyTypeReference)
        {
            // We have to support reading top-level complex values without the { "property": ... } wrapper for open properties
            // in WCF DS Server backward compat mode. (Open property == property without expected type for us).
            if (this.UseServerFormatBehavior && expectedPropertyTypeReference == null)
            {
                // The behavior we need to emulate is:
                //   - If the JSON object has more than one property in it - read it as a complex value directly (without the property wrapper)
                //   - If the JSON object has exactly one property and it's called __metadata, read the value as a complex value directly
                //     this is because open property values must specify type names and thus if there's __metadata it might be a valid complex value (Without property wrapper)
                //     which has no properties on it.
                //   - In all other cases (single property not called __metadata), 
                // So turn on buffering so that we can read ahead to figure out which of the above cases it is
                this.JsonReader.StartBuffering();

                try
                {
                    // Read the start of the object, as the payload must be an object regardless
                    this.JsonReader.ReadStartObject();

                    // Check for an empty object first
                    if (this.JsonReader.NodeType == JsonNodeType.EndObject)
                    {
                        return false;
                    }

                    // Now read the first property name
                    string firstPropertyName = this.JsonReader.ReadPropertyName();

                    // Skip its value
                    this.JsonReader.SkipValue();

                    // Is there a second property?
                    if (this.JsonReader.NodeType != JsonNodeType.EndObject)
                    {
                        // If there is - read the value in the special mode without the property wrapper
                        return true;
                    }
                    else
                    {
                        // Only one property
                        if (string.CompareOrdinal(firstPropertyName, JsonConstants.ODataMetadataName) == 0)
                        {
                            // It's __metadata, read the value in the special mode without the property wrapper
                            return true;
                        }
                    }
                }
                finally
                {
                    this.JsonReader.StopBuffering();
                }
            }

            return false;
        }

        /// <summary>
        /// Increases the recursion depth of values by 1. This will throw if the recursion depth exceeds the current limit.
        /// </summary>
        private void IncreaseRecursionDepth()
        {
            ValidationUtils.IncreaseAndValidateRecursionDepth(ref this.recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);
        }

        /// <summary>
        /// Decreases the recursion depth of values by 1.
        /// </summary>
        private void DecreaseRecursionDepth()
        {
            Debug.Assert(this.recursionDepth > 0, "Can't decrease recursion depth below 0.");

            this.recursionDepth--;
        }

        /// <summary>
        /// Asserts that the current recursion depth of values is zero. This should be true on all calls into this class from outside of this class.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is needed in DEBUG build.")]
        private void AssertRecursionDepthIsZero()
        {
            Debug.Assert(this.recursionDepth == 0, "The current recursion depth must be 0.");
        }
    }
}
