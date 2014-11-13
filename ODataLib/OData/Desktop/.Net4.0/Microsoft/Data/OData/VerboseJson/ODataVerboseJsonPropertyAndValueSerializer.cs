//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON serializer for properties and value types.
    /// </summary>
    internal class ODataVerboseJsonPropertyAndValueSerializer : ODataVerboseJsonSerializer
    {
        /// <summary>
        /// The current recursion depth of values written by this serializer.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        internal ODataVerboseJsonPropertyAndValueSerializer(ODataVerboseJsonOutputContext verboseJsonOutputContext)
            : base(verboseJsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream and writes the property to it.
        /// </summary>
        /// <param name="property">The property to write.</param>
        internal void WriteTopLevelProperty(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(property != null, "property != null");
            Debug.Assert(!(property.Value is ODataStreamReferenceValue), "!(property.Value is ODataStreamReferenceValue)");

            this.WriteTopLevelPayload(
                () =>
                {
                    this.JsonWriter.StartObjectScope();

                    // Note we do not allow named stream properties to be written as top level property.
                    this.AssertRecursionDepthIsZero();
                    this.WriteProperty(
                        property,
                        null /*owningType*/,
                        false /* allowStreamProperty */,
                        this.CreateDuplicatePropertyNamesChecker(),
                        null /* projectedProperties */);
                    this.AssertRecursionDepthIsZero();

                    this.JsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes property names and value pairs.
        /// </summary>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="properties">The enumeration of properties to write out.</param>
        /// <param name="isComplexValue">
        /// Whether the properties are being written for complex value. Also used for detecting whether stream properties
        /// are allowed as named stream properties should only be defined on ODataEntry instances
        /// </param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        internal void WriteProperties(
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> properties,
            bool isComplexValue,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();

            if (properties == null)
            {
                return;
            }

            foreach (ODataProperty property in properties)
            {
                this.WriteProperty(
                    property,
                    owningType,
                    !isComplexValue,
                    duplicatePropertyNamesChecker,
                    projectedProperties);
            }
        }

        /// <summary>
        /// Writes a primitive value.
        /// Uses a registered primitive type converter to write the value if one is registered for the type, otherwise directly writes the value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="collectionValidator">The collection validator instance.</param>
        /// <param name="expectedTypeReference">The expected type reference of the primitive value.</param>
        internal void WritePrimitiveValue(
            object value,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            IEdmPrimitiveTypeReference actualTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            if (collectionValidator != null)
            {
                if (actualTypeReference == null)
                {
                    throw new ODataException(ODataErrorStrings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
                }

                collectionValidator.ValidateCollectionItem(actualTypeReference.FullName(), EdmTypeKind.Primitive);
            }

            if (expectedTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, actualTypeReference, expectedTypeReference);
            }

            if (actualTypeReference != null && actualTypeReference.IsSpatial())
            {
                // For spatial types, we will always write the type name. This is consistent with complex types.
                string typeName = actualTypeReference.FullName();
                PrimitiveConverter.Instance.WriteVerboseJson(value, this.JsonWriter, typeName, this.Version);
            }
            else
            {
                this.JsonWriter.WritePrimitiveValue(value, this.Version);
            }
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="propertyTypeReference">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="collectionValidator">The collection validator instance to validate the type names and type kinds of collection items; null if no validation is needed.</param>
        /// <remarks>The current recursion depth should be a value, measured by the number of complex and collection values between
        /// this complex value and the top-level payload, not including this one.</remarks>
        internal void WriteComplexValue(
            ODataComplexValue complexValue,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");

            this.IncreaseRecursionDepth();

            // Start the object scope which will represent the entire complex instance
            this.JsonWriter.StartObjectScope();

            string typeName = complexValue.TypeName;

            if (collectionValidator != null)
            {
                collectionValidator.ValidateCollectionItem(typeName, EdmTypeKind.Complex);
            }

            // resolve the type name to the type; if no type name is specified we will use the type inferred from metadata
            IEdmComplexTypeReference complexValueTypeReference = TypeNameOracle.ResolveAndValidateTypeNameForValue(this.Model, propertyTypeReference, complexValue, isOpenPropertyType).AsComplexOrNull();
            
            string collectionItemTypeName;
            typeName = this.VerboseJsonOutputContext.TypeNameOracle.GetValueTypeNameForWriting(complexValue, complexValueTypeReference, complexValue.GetAnnotation<SerializationTypeNameAnnotation>(), collectionValidator, out collectionItemTypeName);
            Debug.Assert(collectionItemTypeName == null, "collectionItemTypeName == null");

            // Write the "__metadata" : { "type": "typename" }
            // But only if we actually have a typename to write, otherwise we need the __metadata to be omitted entirely
            if (typeName != null)
            {
                ODataJsonWriterUtils.WriteMetadataWithTypeName(this.JsonWriter, typeName);
            }

            // Write the properties of the complex value as usual. Note we do not allow complex types to contain named stream properties.
            this.WriteProperties(
                complexValueTypeReference == null ? null : complexValueTypeReference.ComplexDefinition(),
                complexValue.Properties,
                true /* isComplexValue */,
                duplicatePropertyNamesChecker,
                null /*projectedProperties */);

            // End the object scope which represents the complex instance
            this.JsonWriter.EndObjectScope();

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes out the value of a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="metadataTypeReference">The metadata type reference for the collection.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <remarks>The current recursion depth is measured by the number of complex and collection values between 
        /// this one and the top-level payload, not including this one.</remarks>
        internal void WriteCollectionValue(
            ODataCollectionValue collectionValue,
            IEdmTypeReference metadataTypeReference,
            bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(collectionValue != null, "collectionValue != null");

            this.IncreaseRecursionDepth();

            // Start the object scope which will represent the entire CollectionValue instance
            this.JsonWriter.StartObjectScope();

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)TypeNameOracle.ResolveAndValidateTypeNameForValue(this.Model, metadataTypeReference, collectionValue, isOpenPropertyType);

            // "__metadata": { "type": "typename" }
            // If the CollectionValue has type information write out the metadata and the type in it.
            string collectionItemTypeName;
            string typeName = this.VerboseJsonOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue, collectionTypeReference, collectionValue.GetAnnotation<SerializationTypeNameAnnotation>(), /*collectionValidator*/ null, out collectionItemTypeName);

            if (typeName != null)
            {
                ODataJsonWriterUtils.WriteMetadataWithTypeName(this.JsonWriter, typeName);
            }

            // "results": [
            // This represents the array of items in the CollectionValue
            this.JsonWriter.WriteDataArrayName();
            this.JsonWriter.StartArrayScope();

            // Iterate through the CollectionValue items and write them out (treat null Items as an empty enumeration)
            IEnumerable items = collectionValue.Items;
            if (items != null)
            {
                IEdmTypeReference expectedItemTypeReference = collectionTypeReference == null ? null : collectionTypeReference.ElementType();

                CollectionWithoutExpectedTypeValidator collectionValidator = new CollectionWithoutExpectedTypeValidator(collectionItemTypeName);

                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    ValidationUtils.ValidateCollectionItem(item, false /* isStreamable */);

                    ODataComplexValue itemAsComplexValue = item as ODataComplexValue;
                    if (itemAsComplexValue != null)
                    {
                        if (duplicatePropertyNamesChecker == null)
                        {
                            duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                        }

                        this.WriteComplexValue(
                            itemAsComplexValue,
                            expectedItemTypeReference,
                            false,
                            duplicatePropertyNamesChecker,
                            collectionValidator);

                        duplicatePropertyNamesChecker.Clear();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");

                        this.WritePrimitiveValue(item, collectionValidator, expectedItemTypeReference);
                    }
                }
            }

            // End the array scope which holds the items
            this.JsonWriter.EndArrayScope();

            // End the object scope which holds the entire collection
            this.JsonWriter.EndObjectScope();

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes the metadata content for a media resource or a named stream
        /// </summary>
        /// <param name="streamReferenceValue">The stream reference value for which to write the metadata</param>
        internal void WriteStreamReferenceValueContent(ODataStreamReferenceValue streamReferenceValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            // Write the "edit_media": "url"
            Uri mediaEditLink = streamReferenceValue.EditLink;
            if (mediaEditLink != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataMetadataEditMediaName);
                this.JsonWriter.WriteValue(this.UriToAbsoluteUriString(mediaEditLink));
            }

            // Write the "media_src": "url"
            if (streamReferenceValue.ReadLink != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataMetadataMediaUriName);
                this.JsonWriter.WriteValue(this.UriToAbsoluteUriString(streamReferenceValue.ReadLink));
            }

            // Write the "content_type": "type"
            if (streamReferenceValue.ContentType != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataMetadataContentTypeName);
                this.JsonWriter.WriteValue(streamReferenceValue.ContentType);
            }

            // Write the "media_etag": "etag"
            string mediaETag = streamReferenceValue.ETag;
            if (mediaETag != null)
            {
                // Note ValidationUtils.ValidateStreamReferenceValue() should have been called prior to this to make sure mediaEditLink is not null here.
                Debug.Assert(mediaEditLink != null, "The stream edit link cannot be null when the etag value is set.");
                this.WriteETag(JsonConstants.ODataMetadataMediaETagName, mediaETag);
            }
        }

        /// <summary>
        /// Writes the etag property with the given string value.
        /// </summary>
        /// <param name="etagName">The name of the ETag, e.g. media_etag or etag</param>
        /// <param name="etagValue">The value of the ETag</param>
        internal void WriteETag(string etagName, string etagValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(etagName), "!string.IsNullOrEmpty(etagName)");
            Debug.Assert(etagValue != null, "etagValue != null");

            this.JsonWriter.WriteName(etagName);
            this.JsonWriter.WriteValue(etagValue);
        }

        /// <summary>
        /// Asserts that the current recursion depth of values is zero. This should be true on all calls into this class from outside of this class.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is needed in DEBUG build.")]
        internal void AssertRecursionDepthIsZero()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.recursionDepth == 0, "The current recursion depth must be 0.");
        }

        /// <summary>
        /// Writes a name/value pair for a property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry or complex type containing the property (or null if not metadata is available).</param>
        /// <param name="allowStreamProperty">Should pass in true if we are writing a property of an ODataEntry instance, false otherwise.
        /// Named stream properties should only be defined on ODataEntry instances.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        private void WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool allowStreamProperty,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();

            WriterValidationUtils.ValidatePropertyNotNull(property);

            string propertyName = property.Name;
            object value = property.Value;
            if (projectedProperties.ShouldSkipProperty(propertyName))
            {
                return;
            }

            WriterValidationUtils.ValidatePropertyName(propertyName);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType);
            IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;

            // If the property is of Geography or Geometry type or the value is of Geography or Geometry type
            // make sure to check that the version is 3.0 or above.
            if ((propertyTypeReference != null && propertyTypeReference.IsSpatial()) ||
                (propertyTypeReference == null && value is System.Spatial.ISpatial))
            {
                ODataVersionChecker.CheckSpatialValue(this.Version);
            }

            this.JsonWriter.WriteName(propertyName);
            if (value == null)
            {
                WriterValidationUtils.ValidateNullPropertyValue(propertyTypeReference, propertyName, this.MessageWriterSettings.WriterBehavior, this.Model);
                this.JsonWriter.WriteValue(null);
            }
            else
            {
                bool isOpenPropertyType = owningType != null && owningType.IsOpen && propertyTypeReference == null;
                if (isOpenPropertyType)
                {
                    ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
                }

                ODataComplexValue complexValue = value as ODataComplexValue;
                if (complexValue != null)
                {
                    this.WriteComplexValue(
                        complexValue,
                        propertyTypeReference,
                        isOpenPropertyType,
                        this.CreateDuplicatePropertyNamesChecker(),
                        /*collectionValidator*/ null);
                }
                else
                {
                    ODataCollectionValue collectionValue = value as ODataCollectionValue;
                    if (collectionValue != null)
                    {
                        ODataVersionChecker.CheckCollectionValueProperties(this.Version, propertyName);
                        this.WriteCollectionValue(
                            collectionValue,
                            propertyTypeReference,
                            isOpenPropertyType);
                    }
                    else
                    {
                        ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
                        if (streamReferenceValue != null)
                        {
                            if (!allowStreamProperty)
                            {
                                throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
                            }

                            Debug.Assert(owningType == null || owningType.IsODataEntityTypeKind(), "The metadata should not allow named stream properties to be defined on a non-entity type.");
                            WriterValidationUtils.ValidateStreamReferenceProperty(property, edmProperty, this.Version, this.WritingResponse);
                            WriterValidationUtils.ValidateStreamReferenceValue(streamReferenceValue, /*isDefaultStream*/ false);
                            this.WriteStreamReferenceValue(streamReferenceValue);
                        }
                        else
                        {
                            this.WritePrimitiveValue(value, /*collectionValidator*/ null, propertyTypeReference);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a stream property value.
        /// </summary>
        /// <param name="streamReferenceValue">The stream reference value to be written</param>
        private void WriteStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
        {
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            // start of the stream reference value
            this.JsonWriter.StartObjectScope();

            // the __mediaresource property
            this.JsonWriter.WriteName(JsonConstants.ODataMetadataMediaResourceName);

            // start of the __mediaresource property value
            this.JsonWriter.StartObjectScope();

            this.WriteStreamReferenceValueContent(streamReferenceValue);

            // end of the __mediaresource propert value
            this.JsonWriter.EndObjectScope();

            // end of the stream reference value
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Increases the recursion depth of values by 1. This will throw if the recursion depth exceeds the current limit.
        /// </summary>
        private void IncreaseRecursionDepth()
        {
            ValidationUtils.IncreaseAndValidateRecursionDepth(ref this.recursionDepth, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
        }

        /// <summary>
        /// Decreases the recursion depth of values by 1.
        /// </summary>
        private void DecreaseRecursionDepth()
        {
            Debug.Assert(this.recursionDepth > 0, "Can't decrease recursion depth below 0.");

            this.recursionDepth--;
        }
    }
}
