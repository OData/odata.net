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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for properties and value types.
    /// </summary>
    internal class ODataJsonPropertyAndValueDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataJsonPropertyAndValueDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal ODataProperty ReadTopLevelProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            this.JsonReader.AssertNotBuffering();

            if (!this.Model.IsUserModel())
            {
                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata);
            }

            // Read the response wrapper "d" if expected.
            this.ReadDataWrapperStart();

            string propertyName;
            object propertyValue;

            // We have to support reading top-level complex values without the { "property": ... } wrapper for open properties
            // in WCF DS Server backward compat mode. (Open property == property without expected type for us).
            if (this.ShouldReadTopLevelPropertyValueWithoutPropertyWrapper(expectedPropertyTypeReference))
            {
                // We will report the value without property wrapper as a property with empty name (this is technically invalid, but it will only happen in WCF DS Server mode).
                propertyName = string.Empty;

                // Read the value directly
                propertyValue = this.ReadNonEntityValue(expectedPropertyTypeReference, null);
            }
            else
            {
                // Read the start of the object container around the property { "property": ... }
                this.JsonReader.ReadStartObject();

                // Read once - this should be the property
                propertyName = this.JsonReader.ReadPropertyName();

                // Now read the property value
                propertyValue = this.ReadNonEntityValue(expectedPropertyTypeReference, null);

                if (this.JsonReader.NodeType != JsonNodeType.EndObject)
                {
                    // There should be only one property in the top-level property wrapper.
                    throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
                }

                // Read over the end object - note that this might be the last node in the input (in case there's no response wrapper)
                this.JsonReader.Read();
            }

            // Read the end of the response wrapper "d" if expected.
            this.ReadDataWrapperEnd();

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return new ODataProperty
            {
                Name = propertyName,
                Value = propertyValue
            };
        }

        /// <summary>
        /// Reads an entry, complex or multivalue content in buffering mode until it finds the type name in the __metadata object
        /// or hits the end of the object.
        /// </summary>
        /// <returns>The type name as read from the __metadata object; null if none was found.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// Pre-Condition:  JsonNodeType.Property           The first property of the entry to be read
        ///                 JsonNodeType.EndObject          If no properties exist in the entry's content
        /// Post-Condition: JsonNodeType.Property           The first property of the entry to be read
        ///                 JsonNodeType.EndObject          If no properties exist in the entry's content
        /// </remarks>
        internal string FindTypeNameInPayload()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            // start buffering so we don't move the reader
            this.JsonReader.StartBuffering();

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
                this.JsonReader.NodeType == JsonNodeType.Property ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: JsonNodeType.Property or JsonNodeType.EndObject");

            return typeName;
        }

        /// <summary>
        /// Reads a primitive value, complex value or multivalue.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the property value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
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
        /// - <see cref="ODataMultiValue"/>
        /// </remarks>
        internal object ReadNonEntityValue(IEdmTypeReference expectedValueTypeReference, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject | this.JsonReader.NodeType == JsonNodeType.StartArray,
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject or JsonNodeType.StartArray");
            Debug.Assert(
                expectedValueTypeReference == null || !expectedValueTypeReference.IsODataEntityTypeKind(),
                "Only primitive, complex or multivalue types can be read by this method.");
            this.JsonReader.AssertNotBuffering();

            // Property values can be only primitives or objects. No property can have a JSON array value.
            JsonNodeType nodeType = this.JsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue(nodeType));
            }

            object result;
            switch (this.GetNonEntityValueKind(expectedValueTypeReference))
            {
                case EdmTypeKind.Primitive:
                    Debug.Assert(expectedValueTypeReference == null || expectedValueTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                    result = this.ReadPrimitiveValue(expectedValueTypeReference.AsPrimitiveOrNull());
                    break;
                case EdmTypeKind.Complex:
                    result = this.ReadComplexValue(expectedValueTypeReference.AsComplexOrNull(), duplicatePropertyNamesChecker);
                    break;
                case EdmTypeKind.Collection:
                    result = this.ReadMultiValue(expectedValueTypeReference.ValidateMultiValueType());
                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataJsonPropertyAndValueDeserializer_ReadPropertyValue));
            }

            this.JsonReader.AssertNotBuffering();

            return result;
        }

        /// <summary>
        /// Reads a primitive value.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <returns>The value of the primitive value.</returns>
        /// <remarks>
        /// Pre-Condition:  none - Fails if the current node is not a JsonNodeType.PrimitiveValue
        /// Post-Condition: almost anything - the node after the primitive value.
        /// </remarks>
        internal object ReadPrimitiveValue(IEdmPrimitiveTypeReference expectedValueTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();

            object result = this.JsonReader.ReadPrimitiveValue();

            if (expectedValueTypeReference != null && !this.MessageReaderSettings.DisablePrimitiveTypeConversion)
            {
                result = ODataJsonReaderUtils.ConvertValue(result, expectedValueTypeReference, this.MessageReaderSettings.ReaderBehavior.UseV1ProviderBehavior);
            }

            this.JsonReader.AssertNotBuffering();

            return result;
        }

        /// <summary>
        /// Reads a complex value.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <returns>The value of the complex value.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject or JsonNodeType.PrimitiveValue (with null value)
        /// Post-Condition: almost anything - the node after the complex value (after the EndObject)
        /// </remarks>
        internal ODataComplexValue ReadComplexValue(IEdmComplexTypeReference expectedValueTypeReference, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();

            // Complex value can be either null constant or a JSON object
            // If it's a null primitive value, report a null value.
            if (this.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null)
            {
                this.JsonReader.ReadNext();
                return null;
            }

            // Read over the start object
            this.JsonReader.ReadStartObject();

            // Find the type name (if there's any) in the payload
            string payloadTypeName = this.FindTypeNameInPayload();

            SerializationTypeNameAnnotation serializationTypeNameAnnotation;
            IEdmComplexTypeReference targetComplexTypeReference = (IEdmComplexTypeReference)ReaderValidationUtils.ResolveAndValidateTargetType(
                EdmTypeKind.Complex,
                expectedValueTypeReference,
                payloadTypeName,
                this.Model,
                this.MessageReaderSettings,
                out serializationTypeNameAnnotation);

            Debug.Assert(targetComplexTypeReference != null, "targetComplexTypeReference != null");
            ODataComplexValue complexValue = new ODataComplexValue();

            complexValue.TypeName = targetComplexTypeReference.ODataFullName();
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
                        throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue);
                    }

                    metadataPropertyFound = true;

                    this.JsonReader.SkipValue();
                }
                else
                {
                    // Any other property is data
                    ODataProperty property = new ODataProperty();
                    property.Name = propertyName;

                    // Lookup the property in metadata
                    IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(propertyName, targetComplexTypeReference.ComplexDefinition());

                    // Read the property value
                    property.Value = this.ReadNonEntityValue(edmProperty == null ? null : edmProperty.Type, null);

                    duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
                    properties.Add(property);
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all the properties of a complex value are read the EndObject node is expected.");
            this.JsonReader.ReadEndObject();

            complexValue.Properties = new ReadOnlyEnumerable<ODataProperty>(properties);

            this.JsonReader.AssertNotBuffering();

            return complexValue;
        }

        /// <summary>
        /// Reads a MultiValue.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <returns>The value of the MultiValue.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartObject
        /// Post-Condition: almost anything - the node after the MultiValue (after the EndObject)
        /// </remarks>
        internal ODataMultiValue ReadMultiValue(IEdmCollectionTypeReference expectedValueTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                expectedValueTypeReference == null || expectedValueTypeReference.IsODataMultiValueTypeKind(),
                "If the metadata is specified it must denote a MultiValue for this method to work.");
            this.JsonReader.AssertNotBuffering();

            // Read over the start object
            this.JsonReader.ReadStartObject();

            // Find the type name (if there's any) in the payload
            string payloadTypeName = this.FindTypeNameInPayload();
            SerializationTypeNameAnnotation serializationTypeNameAnnotation;
            IEdmTypeReference targetTypeReference = ReaderValidationUtils.ResolveAndValidateTargetType(
                EdmTypeKind.Collection,
                expectedValueTypeReference,
                payloadTypeName,
                this.Model,
                this.MessageReaderSettings,
                out serializationTypeNameAnnotation);
            IEdmCollectionTypeReference multiValueTypeReference = targetTypeReference.ValidateMultiValueType();

            Debug.Assert(multiValueTypeReference != null, "multiValueTypeReference != null");

            ODataMultiValue multiValue = new ODataMultiValue();
            multiValue.TypeName = multiValueTypeReference.ODataFullName();
            if (serializationTypeNameAnnotation != null)
            {
                multiValue.SetAnnotation(serializationTypeNameAnnotation);
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
                        throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInMultiValueWrapper(JsonConstants.ODataMetadataName));
                    }

                    metadataPropertyFound = true;

                    // Note that we don't need to read the type name again, as we've already read it above in FindTypeNameInPayload.
                    // There's nothing else of interest in the __metadata for multivalues.
                    this.JsonReader.SkipValue();
                }
                else if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
                {
                    // results property
                    if (items != null)
                    {
                        throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInMultiValueWrapper(JsonConstants.ODataResultsName));
                    }

                    items = new List<object>();

                    this.JsonReader.ReadStartArray();

                    DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                    IEdmTypeReference itemType = multiValueTypeReference.CollectionDefinition().ElementType;
                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        object itemValue = this.ReadNonEntityValue(itemType, duplicatePropertyNamesChecker);

                        // Validate the item (for example that it's not null)
                        ValidationUtils.ValidateMultiValueItem(itemValue);

                        // Note that the ReadNonEntityValue already validated that the actual type of the value matches
                        // the expected type (the itemType).
                        items.Add(itemValue);
                    }

                    Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "The results value must end with an end array.");
                    this.JsonReader.ReadEndArray();
                }
                else
                {
                    // Skip over any other property in the multivalue object
                    this.JsonReader.SkipValue();
                }
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all properties of MultiValue wrapper are read the EndObject node is expected.");
            this.JsonReader.ReadEndObject();

            if (items == null)
            {
                // We didn't find any results property. All multivalues must have the results property.
                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_MultiValueWithoutResults);
            }

            multiValue.Items = new ReadOnlyEnumerable(items);

            this.JsonReader.AssertNotBuffering();

            return multiValue;
        }

        /// <summary>
        /// Determines the value kind for a non-entity value (that is top-level property value, property value on a complex type, item in a multivalue)
        /// </summary>
        /// <param name="expectedPropertyValueTypeReference">The expected type reference of the property value.</param>
        /// <returns>The type kind of the property value.</returns>
        /// <remarks>
        /// Doesn't move the JSON reader.
        /// Pre-Condition:  JsonNodeType.PrimitiveValue
        ///                 JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue
        ///                 JsonNodeType.StartObject
        /// </remarks>
        private EdmTypeKind GetNonEntityValueKind(IEdmTypeReference expectedPropertyValueTypeReference)
        {
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject,
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                expectedPropertyValueTypeReference == null || !expectedPropertyValueTypeReference.IsODataEntityTypeKind(),
                "Only primitive, complex or multivalue types can be read by this method.");

            EdmTypeKind? result = null;

            if (expectedPropertyValueTypeReference != null)
            {
                // If we have metadata, use that.
                // Note that WCF DS always uses the expected type to decide what kind if payload to parse
                // so this is compatible. (In fact it goes further and verifies that the payload matches that type as well).
                result = expectedPropertyValueTypeReference.TypeKind();
            }
            else
            {
                JsonNodeType nodeType = this.JsonReader.NodeType;
                if (nodeType == JsonNodeType.PrimitiveValue)
                {
                    result = EdmTypeKind.Primitive;
                }
                else
                {
                    Debug.Assert(nodeType == JsonNodeType.StartObject, "StartObject expected.");

                    // it can be either a complex value or a multivalue
                    // complex value might have a __metadata property or any other property.
                    // In the __metadata the type property (if present) would differentiate the two.
                    // Also multivalue has a results property which value is an array.
                    // Complex value cannot have any property with value of an array, so that's an easy thing to recognize.
                    // So we're looking for either __metadata or results, nothing else can exist on a multivalue.
                    // We need to buffer since once detected we need to go back and reread the real value.
                    this.JsonReader.StartBuffering();

                    // Read over the start object - the start of the complex or multivalue
                    this.JsonReader.ReadNext();

                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        string propertyName = this.JsonReader.ReadPropertyName();
                        if (string.Equals(JsonConstants.ODataResultsName, propertyName, StringComparison.Ordinal))
                        {
                            // "results" property, it might be a normal property of a complex type
                            // so we need to check it's value, if it's an array, it's a multivalue.
                            if (this.JsonReader.NodeType == JsonNodeType.StartArray)
                            {
                                result = EdmTypeKind.Collection;
                                break;
                            }

                            // If it's not an array, it's a normal property on a complex type
                            result = EdmTypeKind.Complex;
                            break;
                        }

                        if (string.Equals(JsonConstants.ODataMetadataName, propertyName, StringComparison.Ordinal))
                        {
                            // No point in trying to figure out the kind from the typename although it's possible
                            // it's easier to just rely on the results property alone. The caching cost is negligible
                            // because the metadata property is small as compared to the possible size of the results property.
                            // So nothing to do here, the __metadata property is expected in both complex and multivalue.
                        }
                        else
                        {
                            // Any other property means it's not a multivalue
                            result = EdmTypeKind.Complex;
                            break;
                        }

                        // Skip over the property value
                        this.JsonReader.SkipValue();
                    }

                    if (!result.HasValue)
                    {
                        // If we end here, it means we didn't find enough properties to tell if we're looking at multivalue or complex type
                        // In which case it must be a complex type, since multivalue requires the results property.
                        result = EdmTypeKind.Complex;
                    }

                    // Stop the buffering, since we're done deciding the type of the value.
                    this.JsonReader.StopBuffering();
                }
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(result.HasValue, "We should have decided on the value type by now.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject,
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject");

            return result.Value;
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
        private string ReadTypeNameFromMetadataPropertyValue()
        {
            string typeName = null;

            // The value of the __metadata property must be an object
            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_MetadataPropertyMustHaveObjectValue(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartObject();

            // Go over the properties and look for the type property.
            ODataJsonReaderUtils.MetadataPropertyBitMask propertiesFoundBitField = 0;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataMetadataTypeName, propertyName) == 0)
                {
                    ODataJsonReaderUtils.VerifyMetadataPropertyNotFound(
                        ref propertiesFoundBitField,
                        ODataJsonReaderUtils.MetadataPropertyBitMask.Type,
                        JsonConstants.ODataMetadataTypeName);
                    object typeNameValue = this.JsonReader.ReadPrimitiveValue();
                    typeName = typeNameValue as string;
                    if (string.IsNullOrEmpty(typeName))
                    {
                        throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName(typeNameValue));
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
        /// Determines if the top-level property payload should be read as usual, or without the property wrapper.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference for the property value to read.</param>
        /// <returns>true if the property payload should be read without the property wrapper, false if it should be read as usual with the property wrapper.</returns>
        /// <remarks>This method is to support backward compat behavior for WCF DS Server, which can read open property values without property wrapper.</remarks>
        private bool ShouldReadTopLevelPropertyValueWithoutPropertyWrapper(IEdmTypeReference expectedPropertyTypeReference)
        {
            // We have to support reading top-level complex values without the { "property": ... } wrapper for open properties
            // in WCF DS Server backward compat mode. (Open property == property without expected type for us).
            if (this.MessageReaderSettings.ReaderBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesServer && expectedPropertyTypeReference == null)
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
    }
}
