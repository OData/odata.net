//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for value types.
    /// </summary>
    internal class ODataJsonLightValueSerializer : ODataJsonLightSerializer
    {
        /// <summary>
        /// The current recursion depth of values written by this serializer.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Property serializer.
        /// </summary>
        private ODataJsonLightPropertySerializer propertySerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonLightValueSerializer"/> class.
        /// </summary>
        /// <param name="propertySerializer">The property serializer to use when writing complex values.</param>
        /// <param name="initContextUriBuilder">Whether contextUriBuilder should be initialized.</param>
        internal ODataJsonLightValueSerializer(ODataJsonLightPropertySerializer propertySerializer, bool initContextUriBuilder = false)
            : base(propertySerializer.JsonLightOutputContext, initContextUriBuilder)
        {
            this.propertySerializer = propertySerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonLightValueSerializer"/> class.
        /// </summary>
        /// <param name="outputContext">The output context to use.</param>
        /// <param name="initContextUriBuilder">Whether contextUriBuilder should be initialized.</param>
        internal ODataJsonLightValueSerializer(ODataJsonLightOutputContext outputContext, bool initContextUriBuilder = false)
            : base(outputContext, initContextUriBuilder)
        {
        }

        /// <summary>
        /// Gets the property serializer.
        /// </summary>
        private ODataJsonLightPropertySerializer PropertySerializer
        {
            get
            {
                if (this.propertySerializer == null)
                {
                    this.propertySerializer = new ODataJsonLightPropertySerializer(this.JsonLightOutputContext);
                }

                return this.propertySerializer;
            }
        }

        /// <summary>
        /// Writes a null value to the writer.
        /// </summary>
        public virtual void WriteNullValue()
        {
            this.JsonWriter.WriteValue((string)null);
        }

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        public virtual void WriteEnumValue(
            ODataEnumValue value,
            IEdmTypeReference expectedTypeReference)
        {
            if (value.Value == null)
            {
                this.WriteNullValue();
            }
            else
            {
                this.JsonWriter.WritePrimitiveValue(value.Value);
            }
        }

        /// <summary>
        /// Writes out the value of a resource (complex or entity).
        /// </summary>
        /// <param name="resourceValue">The resource (complex or entity) value to write.</param>
        /// <param name="metadataTypeReference">The metadata type for the resource value.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open (dynamic) property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <remarks>The current recursion depth should be a value, measured by the number of resource and collection values between
        /// this resource value and the top-level payload, not including this one.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        public virtual void WriteResourceValue(
            ODataResourceValue resourceValue,
            IEdmTypeReference metadataTypeReference,
            bool isOpenPropertyType,
            IDuplicatePropertyNameChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(resourceValue != null, "resourceValue != null");

            this.IncreaseRecursionDepth();

            // Start the object scope which will represent the entire resource instance;
            this.JsonWriter.StartObjectScope();

            string typeName = resourceValue.TypeName;

            // In requests, we allow the property type reference to be null if the type name is specified in the OM
            if (metadataTypeReference == null && !this.WritingResponse && typeName == null && this.Model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForResourceValueRequest);
            }

            // Resolve the type name to the type; if no type name is specified we will use the type inferred from metadata.
            IEdmStructuredTypeReference resourceValueTypeReference =
                (IEdmStructuredTypeReference)TypeNameOracle.ResolveAndValidateTypeForResourceValue(this.Model, metadataTypeReference, resourceValue, isOpenPropertyType, this.WriterValidator);
            Debug.Assert(
                metadataTypeReference == null || resourceValueTypeReference == null || EdmLibraryExtensions.IsAssignableFrom(metadataTypeReference, resourceValueTypeReference),
                "Complex property types must be the same as or inherit from the ones from metadata (unless open).");

            typeName = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(resourceValue, metadataTypeReference, resourceValueTypeReference, isOpenPropertyType);
            if (typeName != null)
            {
                this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
            }

            // Write custom instance annotations
            this.InstanceAnnotationWriter.WriteInstanceAnnotations(resourceValue.InstanceAnnotations);

            // Write the properties of the resource value as usual. Note we do not allow resource types to contain named stream properties.
            this.PropertySerializer.WriteProperties(
                resourceValueTypeReference == null ? null : resourceValueTypeReference.StructuredDefinition(),
                resourceValue.Properties,
                true /* isComplexValue */,
                duplicatePropertyNamesChecker,
                null);

            // End the object scope which represents the resource instance;
            this.JsonWriter.EndObjectScope();

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes out the value of a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="metadataTypeReference">The metadata type reference for the collection.</param>
        /// <param name="valueTypeReference">The value type reference for the collection.</param>
        /// <param name="isTopLevelProperty">Whether or not a top-level property is being written.</param>
        /// <param name="isInUri">Whether or not the value is being written for a URI.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <remarks>The current recursion depth is measured by the number of resource and collection values between
        /// this one and the top-level payload, not including this one.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        public virtual void WriteCollectionValue(ODataCollectionValue collectionValue, IEdmTypeReference metadataTypeReference, IEdmTypeReference valueTypeReference, bool isTopLevelProperty, bool isInUri, bool isOpenPropertyType)
        {
            Debug.Assert(collectionValue != null, "collectionValue != null");
            Debug.Assert(!isTopLevelProperty || !isInUri, "Cannot be a top level property and in a uri");

            this.IncreaseRecursionDepth();

            // If the CollectionValue has type information write out the metadata and the type in it.
            string typeName = collectionValue.TypeName;

            if (isTopLevelProperty)
            {
                Debug.Assert(metadataTypeReference == null, "Never expect a metadata type for top-level properties.");
                if (typeName == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightValueSerializer_MissingTypeNameOnCollection);
                }
            }
            else
            {
                // In requests, we allow the metadata type reference to be null if the type name is specified in the OM
                if (metadataTypeReference == null && !this.WritingResponse && typeName == null && this.Model.IsUserModel())
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
                }
            }

            if (valueTypeReference == null)
            {
                valueTypeReference = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(this.Model, metadataTypeReference, collectionValue, isOpenPropertyType, this.WriterValidator);
            }

            bool useValueProperty = false;
            if (isInUri)
            {
                // resolve the type name to the type; if no type name is specified we will use the
                // type inferred from metadata
                typeName = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue, metadataTypeReference, valueTypeReference, isOpenPropertyType);
                if (!string.IsNullOrEmpty(typeName))
                {
                    useValueProperty = true;

                    // "{"
                    this.JsonWriter.StartObjectScope();
                    this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
                    this.JsonWriter.WriteValuePropertyName();
                }
            }

            // [
            // This represents the array of items in the CollectionValue
            this.JsonWriter.StartArrayScope();

            // Iterate through the CollectionValue items and write them out (treat null Items as an empty enumeration)
            IEnumerable items = collectionValue.Items;
            if (items != null)
            {
                IEdmTypeReference expectedItemTypeReference = valueTypeReference == null ? null : ((IEdmCollectionTypeReference)valueTypeReference).ElementType();

                IDuplicatePropertyNameChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    ValidationUtils.ValidateCollectionItem(item, expectedItemTypeReference.IsNullable());

                    ODataResourceValue itemAsResourceValue = item as ODataResourceValue;
                    if (itemAsResourceValue != null)
                    {
                        if (duplicatePropertyNamesChecker == null)
                        {
                            duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNameChecker();
                        }

                        this.WriteResourceValue(
                            itemAsResourceValue,
                            expectedItemTypeReference,
                            false /*isOpenPropertyType*/,
                            duplicatePropertyNamesChecker);

                        duplicatePropertyNamesChecker.Reset();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");

                        // by design: collection element's type name is not written for enum or non-spatial primitive value even in case of full metadata.
                        // because enum and non-spatial primitive types don't have inheritance, the type of each element is the same as the item type of the collection, whose type name for spatial types in full metadata mode.
                        ODataEnumValue enumValue = item as ODataEnumValue;
                        if (enumValue != null)
                        {
                            this.WriteEnumValue(enumValue, expectedItemTypeReference);
                        }
                        else
                        {
                            ODataUntypedValue untypedValue = item as ODataUntypedValue;
                            if (untypedValue != null)
                            {
                                this.WriteUntypedValue(untypedValue);
                            }
                            else if (item != null)
                            {
                                this.WritePrimitiveValue(item, expectedItemTypeReference);
                            }
                            else
                            {
                                this.WriteNullValue();
                            }
                        }
                    }
                }
            }

            // End the array scope which holds the items
            this.JsonWriter.EndArrayScope();

            if (useValueProperty)
            {
                this.JsonWriter.EndObjectScope();
            }

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes a primitive value.
        /// Uses a registered primitive type converter to write the value if one is registered for the type, otherwise directly writes the value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedTypeReference">The expected type reference of the primitive value.</param>
        public virtual void WritePrimitiveValue(
            object value,
            IEdmTypeReference expectedTypeReference)
        {
            this.WritePrimitiveValue(value, null, expectedTypeReference);
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="actualTypeReference">The actual type reference of the primitive value.</param>
        /// <param name="expectedTypeReference">The expected type reference of the primitive value.</param>
        public virtual void WritePrimitiveValue(
            object value,
            IEdmTypeReference actualTypeReference,
            IEdmTypeReference expectedTypeReference)
        {
            Debug.Assert(value != null, "value != null");

            if (actualTypeReference == null)
            {
                // Try convert primitive values from their actual CLR types to their underlying CLR types.
                value = this.Model.ConvertToUnderlyingTypeIfUIntValue(value, expectedTypeReference);
                actualTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            }

            ODataPayloadValueConverter converter = this.JsonLightOutputContext.PayloadValueConverter;

            // Skip validation if user has set custom PayloadValueConverter
            if (expectedTypeReference != null && converter.GetType() == typeof(ODataPayloadValueConverter))
            {
                this.WriterValidator.ValidateIsExpectedPrimitiveType(value, (IEdmPrimitiveTypeReference)actualTypeReference, expectedTypeReference);
            }

            value = converter.ConvertToPayloadValue(value, expectedTypeReference);

            if (actualTypeReference != null && actualTypeReference.IsSpatial() || actualTypeReference.IsDictionaryOfStringObject() || actualTypeReference.IsDictionaryOfStringString())
            {
                PrimitiveConverter.Instance.WriteJsonLight(value, this.JsonWriter);
            }
            else
            {
                this.JsonWriter.WritePrimitiveValue(value);
            }
        }

        /// <summary>
        /// Writes an untyped value.
        /// </summary>
        /// <param name="value">The untyped value to write.</param>
        public virtual void WriteUntypedValue(
            ODataUntypedValue value)
        {
            Debug.Assert(value != null, "value != null");

            if (string.IsNullOrEmpty(value.RawValue))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightValueSerializer_MissingRawValueOnUntyped);
            }

            this.JsonWriter.WriteRawValue(value.RawValue);
        }

        public virtual void WriteStreamValue(ODataBinaryStreamValue streamValue)
        {
            IJsonStreamWriter streamWriter = this.JsonWriter as IJsonStreamWriter;
            if (streamWriter == null)
            {
                // write as a string
                this.JsonWriter.WritePrimitiveValue(new StreamReader(streamValue.Stream).ReadToEnd());
            }
            else
            {
                Stream stream = streamWriter.StartStreamValueScope();
                streamValue.Stream.CopyTo(stream);
                stream.Flush();
                stream.Dispose();
                streamWriter.EndStreamValueScope();
            }
        }

        /// <summary>
        /// Asserts that the current recursion depth of values is zero. This should be true on all calls into this class from outside of this class.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is needed in DEBUG build.")]
        internal void AssertRecursionDepthIsZero()
        {
            Debug.Assert(this.recursionDepth == 0, "The current recursion depth must be 0.");
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