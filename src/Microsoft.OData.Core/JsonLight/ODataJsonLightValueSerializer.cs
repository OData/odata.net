//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for value types.
    /// </summary>
    internal class ODataJsonLightValueSerializer : ODataJsonLightSerializer, IODataJsonLightValueSerializer
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
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
        /// Both ODataJsonLightSerializer and IODataJsonLightValueSerializer define this, so we pass through to our base class.
        /// </summary>
        IJsonWriter IODataJsonLightValueSerializer.JsonWriter
        {
            get { return this.JsonWriter; }
        }

        /// <summary>
        /// The model to use.
        /// </summary>
        IEdmModel IODataJsonLightValueSerializer.Model
        {
            get { return this.Model; }
        }

        /// <summary>
        /// The message writer settings to use when writing the message payload.
        /// </summary>
        ODataMessageWriterSettings IODataJsonLightValueSerializer.Settings
        {
            get { return this.JsonLightOutputContext.MessageWriterSettings; }
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
        /// Writes a null value to the wire.
        /// </summary>
        public void WriteNullValue()
        {
            this.JsonWriter.WriteValue((string)null);
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="metadataTypeReference">The metadata type for the complex value.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <remarks>The current recursion depth should be a value, measured by the number of complex and collection values between
        /// this complex value and the top-level payload, not including this one.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        public void WriteComplexValue(
            ODataComplexValue complexValue,
            IEdmTypeReference metadataTypeReference,
            bool isTopLevel,
            bool isOpenPropertyType,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(complexValue != null, "complexValue != null");

            this.IncreaseRecursionDepth();

            // Start the object scope which will represent the entire complex instance;
            // for top-level complex properties we already wrote the object scope (and the context URI when needed).
            if (!isTopLevel)
            {
                this.JsonWriter.StartObjectScope();
            }

            string typeName = complexValue.TypeName;

            if (isTopLevel)
            {
                Debug.Assert(metadataTypeReference == null, "Never expect a metadata type for top-level properties.");
                if (typeName == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightValueSerializer_MissingTypeNameOnComplex);
                }
            }
            else
            {
                // In requests, we allow the property type reference to be null if the type name is specified in the OM
                if (metadataTypeReference == null && !this.WritingResponse && typeName == null && this.Model.IsUserModel())
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForComplexValueRequest);
                }
            }

            // Resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata.
            IEdmComplexTypeReference complexValueTypeReference = (IEdmComplexTypeReference)TypeNameOracle.ResolveAndValidateTypeForComplexValue(this.Model, metadataTypeReference, complexValue, isOpenPropertyType, this.WriterValidator);
            Debug.Assert(
                metadataTypeReference == null || complexValueTypeReference == null || EdmLibraryExtensions.IsAssignableFrom(metadataTypeReference, complexValueTypeReference),
                "Complex property types must be the same as or inherit from the ones from metadata (unless open).");

            typeName = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(complexValue, metadataTypeReference, complexValueTypeReference, isOpenPropertyType);
            if (typeName != null)
            {
                this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
            }

            // Write custom instance annotations
            this.InstanceAnnotationWriter.WriteInstanceAnnotations(complexValue.InstanceAnnotations);

            // Write the properties of the complex value as usual. Note we do not allow complex types to contain named stream properties.
            this.PropertySerializer.WriteProperties(
                complexValueTypeReference == null ? null : complexValueTypeReference.ComplexDefinition(),
                complexValue.Properties,
                true /* isComplexValue */,
                duplicatePropertyNamesChecker,
                null /*projectedProperties */);

            // End the object scope which represents the complex instance;
            // for top-level complex properties we already wrote the end object scope.
            if (!isTopLevel)
            {
                this.JsonWriter.EndObjectScope();
            }

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        public void WriteEnumValue(
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
        /// Writes out the value of a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="metadataTypeReference">The metadata type reference for the collection.</param>
        /// <param name="valueTypeReference">The value type reference for the collection.</param>
        /// <param name="isTopLevelProperty">Whether or not a top-level property is being written.</param>
        /// <param name="isInUri">Whether or not the value is being written for a URI.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <remarks>The current recursion depth is measured by the number of complex and collection values between 
        /// this one and the top-level payload, not including this one.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        public void WriteCollectionValue(ODataCollectionValue collectionValue, IEdmTypeReference metadataTypeReference, IEdmTypeReference valueTypeReference, bool isTopLevelProperty, bool isInUri, bool isOpenPropertyType)
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

                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    this.WriterValidator.ValidateCollectionItem(item, expectedItemTypeReference.IsNullable());

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
                            false /*isTopLevel*/,
                            false /*isOpenPropertyType*/,
                            duplicatePropertyNamesChecker);

                        duplicatePropertyNamesChecker.Clear();
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
        public void WritePrimitiveValue(
            object value,
            IEdmTypeReference expectedTypeReference)
        {
            Debug.Assert(value != null, "value != null");

            // Try convert primitive values from their actual CLR types to their underlying CLR types.
            value = this.Model.ConvertToUnderlyingTypeIfUIntValue(value, expectedTypeReference);

            IEdmPrimitiveTypeReference actualTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            ODataPayloadValueConverter converter = this.JsonLightOutputContext.PayloadValueConverter;

            // Skip validation if user has set custom PayloadValueConverter
            if (expectedTypeReference != null && converter.GetType() == typeof(ODataPayloadValueConverter))
            {
                this.WriterValidator.ValidateIsExpectedPrimitiveType(value, actualTypeReference, expectedTypeReference);
            }

            value = converter.ConvertToPayloadValue(value, expectedTypeReference);

            if (actualTypeReference != null && actualTypeReference.IsSpatial())
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
        public void WriteUntypedValue(
            ODataUntypedValue value)
        {
            Debug.Assert(value != null, "value != null");

            if (string.IsNullOrEmpty(value.RawValue))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightValueSerializer_MissingRawValueOnUntyped);
            }

            this.JsonWriter.WriteRawValue(value.RawValue);
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// Both ODataJsonLightSerializer and IODataJsonLightValueSerializer define this, so we pass through to our base class.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        DuplicatePropertyNamesChecker IODataJsonLightValueSerializer.CreateDuplicatePropertyNamesChecker()
        {
            return this.CreateDuplicatePropertyNamesChecker();
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