//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
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
        /// Serialization info for current property.
        /// </summary>
        private PropertySerializationInfo currentPropertyInfo;

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

            if (property.ODataValue == null || property.ODataValue.IsNullValue)
            {
                // TODO: Enable updating top-level properties to null #645
                throw new ODataException("A null top-level property is not allowed to be serialized.");
            }

            this.WriteTopLevelPayload(
                () =>
                {
                    this.JsonWriter.StartObjectScope();
                    ODataPayloadKind kind = this.JsonLightOutputContext.MessageWriterSettings.IsIndividualProperty ? ODataPayloadKind.IndividualProperty : ODataPayloadKind.Property;

                    ODataContextUrlInfo contextInfo = ODataContextUrlInfo.Create(property.ODataValue, this.JsonLightOutputContext.MessageWriterSettings.ODataUri, this.Model);
                    this.WriteContextUriProperty(kind, () => contextInfo);

                    // Note we do not allow named stream properties to be written as top level property.
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();
                    this.WriteProperty(
                        property,
                        null /*owningType*/,
                        true /* isTopLevel */,
                        false /* allowStreamProperty */,
                        this.CreateDuplicatePropertyNameChecker());
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();

                    this.JsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes property names and value pairs.
        /// </summary>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the resource (or null if not metadata is available).</param>
        /// <param name="properties">The enumeration of properties to write out.</param>
        /// <param name="isComplexValue">
        /// Whether the properties are being written for complex value. Also used for detecting whether stream properties
        /// are allowed as named stream properties should only be defined on ODataResource instances
        /// </param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        internal void WriteProperties(
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> properties,
            bool isComplexValue,
            IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
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
                    duplicatePropertyNameChecker);
            }
        }

        /// <summary>
        /// Test to see if <paramref name="property"/> is an open property or not.
        /// </summary>
        /// <param name="property">The property in question.</param>
        /// <returns>true if the property is an open property; false if it is not, or if openness cannot be determined</returns>
        private bool IsOpenProperty(ODataProperty property)
        {
            Debug.Assert(property != null, "property != null");

            bool isOpenProperty;

            if (property.SerializationInfo != null)
            {
                isOpenProperty = property.SerializationInfo.PropertyKind == ODataPropertyKind.Open;
            }
            else
            {
                // TODO: (issue #888) this logic results in type annotations not being written for dynamic properties on types that are not
                // marked as open. Type annotations should always be written for dynamic properties whose type cannot be hueristically
                // determined. Need to change this.currentPropertyInfo.MetadataType.IsOpenProperty to this.currentPropertyInfo.MetadataType.IsDynamic,
                // and fix related tests and other logic (this change alone results in writing type even if it's already implied by context).
                isOpenProperty = (!this.WritingResponse && this.currentPropertyInfo.MetadataType.OwningType == null) // Treat property as dynamic property when writing request and owning type is null
                || this.currentPropertyInfo.MetadataType.IsOpenProperty;
            }

            if (isOpenProperty)
            {
                this.WriterValidator.ValidateOpenPropertyValue(property.Name, property.ODataValue);
            }

            return isOpenProperty;
        }

        /// <summary>
        /// Writes a name/value pair for a property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The owning type for the <paramref name="property"/> or null if no metadata is available.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        /// <param name="allowStreamProperty">Should pass in true if we are writing a property of an ODataResource instance, false otherwise.
        /// Named stream properties should only be defined on ODataResource instances.</param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Splitting the code would make the logic harder to understand; class coupling is only slightly above threshold.")]
        private void WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool isTopLevel,
            bool allowStreamProperty,
            IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
        {
            WriterValidationUtils.ValidatePropertyNotNull(property);

            string propertyName = property.Name;

            if (!this.JsonLightOutputContext.PropertyCacheHandler.InResourceSetScope())
            {
                WriterValidationUtils.ValidatePropertyName(propertyName);
                this.currentPropertyInfo = new PropertySerializationInfo(propertyName, owningType) { IsTopLevel = isTopLevel };
            }
            else
            {
                this.currentPropertyInfo = this.JsonLightOutputContext.PropertyCacheHandler.GetProperty(propertyName, owningType);
            }

            WriterValidationUtils.ValidatePropertyDefined(this.currentPropertyInfo, this.MessageWriterSettings.ThrowOnUndeclaredPropertyForNonOpenType);

            duplicatePropertyNameChecker.ValidatePropertyUniqueness(property);

            if (currentPropertyInfo.MetadataType.IsUndeclaredProperty)
            {
                WriteODataTypeAnnotation(property, isTopLevel);
            }

            WriteInstanceAnnotation(property, isTopLevel, currentPropertyInfo.MetadataType.IsUndeclaredProperty);

            ODataValue value = property.ODataValue;

            // handle ODataUntypedValue
            ODataUntypedValue untypedValue = value as ODataUntypedValue;
            if (untypedValue != null)
            {
                WriteUntypedValue(untypedValue);
                return;
            }

            ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                if (!allowStreamProperty)
                {
                    throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataResource(propertyName));
                }

                Debug.Assert(owningType == null || owningType.IsODataEntityTypeKind(), "The metadata should not allow named stream properties to be defined on a non-entity type.");
                Debug.Assert(!isTopLevel, "Stream properties are not allowed at the top level.");
                WriterValidationUtils.ValidateStreamReferenceProperty(property, currentPropertyInfo.MetadataType.EdmProperty, this.WritingResponse);
                this.WriteStreamReferenceProperty(propertyName, streamReferenceValue);
                return;
            }

            if (value is ODataNullValue || value == null)
            {
                this.WriteNullProperty(property);
                return;
            }

            bool isOpenPropertyType = this.IsOpenProperty(property);

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                this.WritePrimitiveProperty(primitiveValue, isOpenPropertyType);
                return;
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                this.WriteEnumProperty(enumValue, isOpenPropertyType);
                return;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                this.WriteCollectionProperty(collectionValue, isOpenPropertyType);
                return;
            }
        }

        private void WriteUntypedValue(ODataUntypedValue untypedValue)
        {
            if (!this.MessageWriterSettings.ThrowOnUndeclaredPropertyForNonOpenType)
            {
                this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
                this.jsonLightValueSerializer.WriteUntypedValue(untypedValue);
                return;
            }

            Debug.Assert(
                this.MessageWriterSettings.ThrowOnUndeclaredPropertyForNonOpenType,
                "this.MessageWriterSettings.ThrowOnUndeclaredPropertyForNonOpenType");
            throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(this.currentPropertyInfo.PropertyName, this.currentPropertyInfo.MetadataType.OwningType.FullTypeName()));
        }

        /// <summary>
        /// Writes instance annotation for property
        /// </summary>
        /// <param name="property">The property to handle.</param>
        /// <param name="isTopLevel">If writing top level property.</param>
        /// <param name="isUndeclaredProperty">If writing an undeclared property.</param>
        private void WriteInstanceAnnotation(ODataProperty property, bool isTopLevel, bool isUndeclaredProperty)
        {
            if (property.InstanceAnnotations.Count != 0)
            {
                if (isTopLevel)
                {
                    this.InstanceAnnotationWriter.WriteInstanceAnnotations(property.InstanceAnnotations);
                }
                else
                {
                    this.InstanceAnnotationWriter.WriteInstanceAnnotations(property.InstanceAnnotations, property.Name, isUndeclaredProperty);
                }
            }
        }

        /// <summary>
        /// Writes odata type annotation for property
        /// </summary>
        /// <param name="property">The property to handle.</param>
        /// <param name="isTopLevel">If writing top level property.</param>
        private void WriteODataTypeAnnotation(ODataProperty property, bool isTopLevel)
        {
            if (property.TypeAnnotation != null && property.TypeAnnotation.TypeName != null)
            {
                string typeName = property.TypeAnnotation.TypeName;
                IEdmPrimitiveType primitiveType = EdmCoreModel.Instance.FindType(typeName) as IEdmPrimitiveType;
                if (primitiveType == null ||
                    (primitiveType.PrimitiveKind != EdmPrimitiveTypeKind.String &&
                    primitiveType.PrimitiveKind != EdmPrimitiveTypeKind.Decimal &&
                    primitiveType.PrimitiveKind != EdmPrimitiveTypeKind.Boolean))
                {
                    if (isTopLevel)
                    {
                        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
                    }
                    else
                    {
                        this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(property.Name, typeName);
                    }
                }
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
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaEditLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaEditLink));
            }

            Uri mediaReadLink = streamReferenceValue.ReadLink;
            if (mediaReadLink != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaReadLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaReadLink));
            }

            string mediaContentType = streamReferenceValue.ContentType;
            if (mediaContentType != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaContentType);
                this.JsonWriter.WriteValue(mediaContentType);
            }

            string mediaETag = streamReferenceValue.ETag;
            if (mediaETag != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaETag);
                this.JsonWriter.WriteValue(mediaETag);
            }
        }

        /// <summary>
        /// Writes a Null property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        private void WriteNullProperty(
            ODataProperty property)
        {
            this.WriterValidator.ValidateNullPropertyValue(
                this.currentPropertyInfo.MetadataType.TypeReference, property.Name, this.Model);

            if (this.currentPropertyInfo.IsTopLevel)
            {
                // TODO: Enable updating top-level properties to null #645
                throw new ODataException("A null top-level property is not allowed to be serialized.");
            }
            else
            {
                this.JsonWriter.WriteName(property.Name);
                this.JsonLightValueSerializer.WriteNullValue();
            }
        }

        /// <summary>
        /// Writes a enum property.
        /// </summary>
        /// <param name="enumValue">The enum value to be written.</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WriteEnumProperty(
            ODataEnumValue enumValue,
            bool isOpenPropertyType)
        {
            ResolveEnumValueTypeName(enumValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
            this.JsonLightValueSerializer.WriteEnumValue(enumValue, this.currentPropertyInfo.MetadataType.TypeReference);
        }

        private void ResolveEnumValueTypeName(ODataEnumValue enumValue, bool isOpenPropertyType)
        {
            if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != enumValue.TypeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForEnumValue(
                    this.Model,
                    enumValue,
                    isOpenPropertyType);

                // This is a work around, needTypeOnWire always = true for client side:
                // ClientEdmModel's reflection can't know a property is open type even if it is, so here
                // make client side always write 'odata.type' for enum.
                bool needTypeOnWire = string.Equals(this.JsonLightOutputContext.Model.GetType().Name, "ClientEdmModel",
                    StringComparison.OrdinalIgnoreCase);
                string typeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(
                    enumValue, this.currentPropertyInfo.MetadataType.TypeReference, typeFromValue, needTypeOnWire || isOpenPropertyType);

                this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(enumValue.TypeName, typeFromValue);
                this.currentPropertyInfo.TypeNameToWrite = typeNameToWrite;
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(enumValue, out typeNameToWrite))
                {
                    this.currentPropertyInfo.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to be written</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WriteCollectionProperty(
            ODataCollectionValue collectionValue,
            bool isOpenPropertyType)
        {
            ResolveCollectionValueTypeName(collectionValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);

            // passing false for 'isTopLevel' because the outer wrapping object has already been written.
            this.JsonLightValueSerializer.WriteCollectionValue(
                collectionValue,
                this.currentPropertyInfo.MetadataType.TypeReference,
                this.currentPropertyInfo.ValueType.TypeReference,
                this.currentPropertyInfo.IsTopLevel,
                false /*isInUri*/,
                isOpenPropertyType);
        }

        private void ResolveCollectionValueTypeName(ODataCollectionValue collectionValue, bool isOpenPropertyType)
        {
            if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != collectionValue.TypeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(
                    this.Model,
                    this.currentPropertyInfo.MetadataType.TypeReference,
                    collectionValue,
                    isOpenPropertyType,
                    this.WriterValidator);

                this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(collectionValue.TypeName, typeFromValue);
                this.currentPropertyInfo.TypeNameToWrite =
                    this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue,
                        this.currentPropertyInfo, isOpenPropertyType);
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(collectionValue, out typeNameToWrite))
                {
                    this.currentPropertyInfo.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes a primitive property.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to be written</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WritePrimitiveProperty(
            ODataPrimitiveValue primitiveValue,
            bool isOpenPropertyType)
        {
            ResolvePrimitiveValueTypeName(primitiveValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
            this.JsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, this.currentPropertyInfo.ValueType.TypeReference, this.currentPropertyInfo.MetadataType.TypeReference);
        }

        private void ResolvePrimitiveValueTypeName(
            ODataPrimitiveValue primitiveValue,
            bool isOpenPropertyType)
        {
            string typeName = primitiveValue.Value.GetType().Name;
            if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != typeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);

                this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(typeName, typeFromValue);
                this.currentPropertyInfo.TypeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(primitiveValue,
                        this.currentPropertyInfo, isOpenPropertyType);
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(primitiveValue, out typeNameToWrite))
                {
                    this.currentPropertyInfo.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes the type name on the wire.
        /// </summary>
        private void WritePropertyTypeName()
        {
            string typeNameToWrite = this.currentPropertyInfo.TypeNameToWrite;
            if (typeNameToWrite != null)
            {
                // We write the type name as an instance annotation (named "odata.type") for top-level properties, but as a property annotation (e.g., "...@odata.type") if not top level.
                if (this.currentPropertyInfo.IsTopLevel)
                {
                    this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameToWrite);
                }
                else
                {
                    this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(this.currentPropertyInfo.PropertyName, typeNameToWrite);
                }
            }
        }
    }
}
