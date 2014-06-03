//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for properties.
    /// </summary>
    internal class ODataJsonLightPropertySerializer : ODataJsonLightSerializer
    {
        /// <summary>
        /// Serializer to use to write property values.
        /// </summary>
        private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="initContextUriBuilder">Whether contextUriBuilder should be initialized.</param>
        internal ODataJsonLightPropertySerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool initContextUriBuilder = false)
            : base(jsonLightOutputContext, initContextUriBuilder)
        {
            this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this, initContextUriBuilder);
        }

        /// <summary>
        /// Gets the json light value writer.
        /// </summary>
        internal ODataJsonLightValueSerializer JsonLightValueSerializer
        {
            get
            {
                return this.jsonLightValueSerializer;
            }
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream and writes the property to it.
        /// </summary>
        /// <param name="property">The property to write.</param>
        internal void WriteTopLevelProperty(ODataProperty property)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(!(property.Value is ODataStreamReferenceValue), "!(property.Value is ODataStreamReferenceValue)");

            this.WriteTopLevelPayload(
                () =>
                {
                    this.JsonWriter.StartObjectScope();
                    ODataPayloadKind kind = this.JsonLightOutputContext.MessageWriterSettings.IsIndividualProperty ? ODataPayloadKind.IndividualProperty : ODataPayloadKind.Property;

                    ODataContextUrlInfo contextInfo = ODataContextUrlInfo.Create(property.ODataValue, this.JsonLightOutputContext.MessageWriterSettings.ODataUri);
                    this.WriteContextUriProperty(kind, () => contextInfo);

                    // Note we do not allow named stream properties to be written as top level property.
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();
                    this.WriteProperty(
                        property,
                        null /*owningType*/,
                        true /* isTopLevel */,
                        false /* allowStreamProperty */,
                        this.CreateDuplicatePropertyNamesChecker(),
                        null /* projectedProperties */);
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();

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
            if (properties == null)
            {
                return;
            }

            foreach (ODataProperty property in properties)
            {
                this.WriteProperty(
                    property,
                    owningType,
                    false /* isTopLevel */,
                    !isComplexValue,
                    duplicatePropertyNamesChecker,
                    projectedProperties);
            }
        }

        /// <summary>
        /// Test to see if <paramref name="property"/> is an open property or not.
        /// </summary>
        /// <param name="property">The property in question.</param>
        /// <param name="owningType">The owning type of the property.</param>
        /// <param name="edmProperty">The metadata of the property.</param>
        /// <returns>true if the property is an open property; false if it is not, or if openness cannot be determined</returns>
        private bool IsOpenProperty(ODataProperty property, IEdmStructuredType owningType, IEdmProperty edmProperty)
        {
            Debug.Assert(property != null, "property != null");
            if (property.SerializationInfo != null)
            {
                return property.SerializationInfo.PropertyKind == ODataPropertyKind.Open;
            }

            return (!this.WritingResponse && owningType == null) // Treat property as dynamic property when writing request and owning type is null
                || (owningType != null && owningType.IsOpen && edmProperty == null);
        }

        /// <summary>
        /// Writes a name/value pair for a property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The owning type for the <paramref name="property"/> or null if no metadata is available.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        /// <param name="allowStreamProperty">Should pass in true if we are writing a property of an ODataEntry instance, false otherwise.
        /// Named stream properties should only be defined on ODataEntry instances.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Splitting the code would make the logic harder to understand; class coupling is only slightly above threshold.")]
        private void WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool isTopLevel,
            bool allowStreamProperty,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            WriterValidationUtils.ValidatePropertyNotNull(property);

            string propertyName = property.Name;
            if (projectedProperties.ShouldSkipProperty(propertyName))
            {
                return;
            }

            WriterValidationUtils.ValidatePropertyName(propertyName);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = null;
            IEdmTypeReference propertyTypeReference = null;
            if (this.JsonLightOutputContext.MessageWriterSettings.EnableFullValidation)
            {
                edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType);
                propertyTypeReference = edmProperty == null ? null : edmProperty.Type;
            }
            
            ODataValue value = property.ODataValue;
            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;

            ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                if (!allowStreamProperty)
                {
                    throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
                }

                Debug.Assert(owningType == null || owningType.IsODataEntityTypeKind(), "The metadata should not allow named stream properties to be defined on a non-entity type.");
                Debug.Assert(!isTopLevel, "Stream properties are not allowed at the top level.");
                WriterValidationUtils.ValidateStreamReferenceProperty(property, edmProperty, this.Version, this.WritingResponse);
                this.WriteStreamReferenceProperty(propertyName, streamReferenceValue);
                return;
            }

            string wirePropertyName = isTopLevel ? JsonLightConstants.ODataValuePropertyName : propertyName;

            if (value is ODataNullValue || value == null)
            {
                WriterValidationUtils.ValidateNullPropertyValue(propertyTypeReference, propertyName, this.MessageWriterSettings.WriterBehavior, this.Model);

                if (isTopLevel)
                {
                    // Write the special null marker for top-level null properties.
                    this.JsonWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNull);
                    this.JsonWriter.WriteValue(true);
                }
                else
                {
                    this.JsonWriter.WriteName(wirePropertyName);
                    this.JsonLightValueSerializer.WriteNullValue();
                }

                return;
            }

            bool isOpenPropertyType = this.IsOpenProperty(property, owningType, edmProperty);
            if (isOpenPropertyType && this.JsonLightOutputContext.MessageWriterSettings.EnableFullValidation)
            {
                ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                if (!isTopLevel)
                {
                    this.JsonWriter.WriteName(wirePropertyName);
                }

                this.JsonLightValueSerializer.WriteComplexValue(complexValue, propertyTypeReference, isTopLevel, isOpenPropertyType, this.CreateDuplicatePropertyNamesChecker());
                return;
            }

            IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeNameForValue(this.Model, propertyTypeReference, value, isOpenPropertyType);
            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                string typeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(enumValue, propertyTypeReference, typeFromValue, isOpenPropertyType);
                this.WritePropertyTypeName(wirePropertyName, typeNameToWrite, isTopLevel);
                this.JsonWriter.WriteName(wirePropertyName);
                this.JsonLightValueSerializer.WriteEnumValue(enumValue, propertyTypeReference);
                return;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                string collectionTypeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue, propertyTypeReference, typeFromValue, isOpenPropertyType);
                this.WritePropertyTypeName(wirePropertyName, collectionTypeNameToWrite, isTopLevel);
                this.JsonWriter.WriteName(wirePropertyName);

                // passing false for 'isTopLevel' because the outer wrapping object has already been written.
                this.JsonLightValueSerializer.WriteCollectionValue(collectionValue, propertyTypeReference, isTopLevel, false /*isInUri*/, isOpenPropertyType);
            }
            else
            {
                Debug.Assert(primitiveValue != null, "primitiveValue != null");

                string typeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(primitiveValue, propertyTypeReference, typeFromValue, isOpenPropertyType);
                this.WritePropertyTypeName(wirePropertyName, typeNameToWrite, isTopLevel);

                this.JsonWriter.WriteName(wirePropertyName);
                this.JsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, propertyTypeReference);
            }
        }

        /// <summary>
        /// Writes a stream property.
        /// </summary>
        /// <param name="propertyName">The name of the property to write.</param>
        /// <param name="streamReferenceValue">The stream reference value to be written</param>
        private void WriteStreamReferenceProperty(string propertyName, ODataStreamReferenceValue streamReferenceValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            Uri mediaEditLink = streamReferenceValue.EditLink;
            if (mediaEditLink != null)
            {
                this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaEditLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaEditLink));
            }

            Uri mediaReadLink = streamReferenceValue.ReadLink;
            if (mediaReadLink != null)
            {
                this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaReadLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaReadLink));
            }

            string mediaContentType = streamReferenceValue.ContentType;
            if (mediaContentType != null)
            {
                this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaContentType);
                this.JsonWriter.WriteValue(mediaContentType);
            }

            string mediaETag = streamReferenceValue.ETag;
            if (mediaETag != null)
            {
                this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaETag);
                this.JsonWriter.WriteValue(mediaETag);
            }
        }

        /// <summary>
        /// Writes the type name on the wire.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeNameToWrite">Type name of the property.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        private void WritePropertyTypeName(string propertyName, string typeNameToWrite, bool isTopLevel)
        {
            if (typeNameToWrite != null)
            {
                // We write the type name as an instance annotation (named "odata.type") for top-level properties, but as a property annotation (e.g., "...@odata.type") if not top level.
                if (isTopLevel)
                {
                    ODataJsonLightWriterUtils.WriteODataTypeInstanceAnnotation(this.JsonWriter, typeNameToWrite);
                }
                else
                {
                    ODataJsonLightWriterUtils.WriteODataTypePropertyAnnotation(this.JsonWriter, propertyName, typeNameToWrite);
                }
            }
        }
    }
}
