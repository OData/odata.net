//---------------------------------------------------------------------
// <copyright file="ODataAtomPropertyAndValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for properties and values.
    /// </summary>
    internal class ODataAtomPropertyAndValueSerializer : ODataAtomSerializer
    {
        /// <summary>
        /// The current recursion depth of values written by this serializer.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomPropertyAndValueSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        internal void WriteTopLevelProperty(ODataProperty property)
        {
            this.WritePayloadStart();
            this.AssertRecursionDepthIsZero();
            this.WriteProperty(
                property,
                null /*owningType*/,
                true  /* isTopLevel */,
                false /* isWritingCollection */,
                null  /* beforePropertyAction */,
                this.CreateDuplicatePropertyNamesChecker(),
                null  /* projectedProperties */);
            this.AssertRecursionDepthIsZero();
            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a collection of instance annotations in ATOM format.
        /// </summary>
        /// <param name="instanceAnnotations">Instance annotation collection to write.</param>
        /// <param name="tracker">The tracker to track which instance annotations have been written.</param>
        internal void WriteInstanceAnnotations(IEnumerable<AtomInstanceAnnotation> instanceAnnotations, InstanceAnnotationWriteTracker tracker)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            Debug.Assert(tracker != null, "tracker should not be null if we called this");

            HashSet<string> instanceAnnotationNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var annotation in instanceAnnotations)
            {
                if (!instanceAnnotationNames.Add(annotation.TermName))
                {
                    throw new ODataException(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection(annotation.TermName));
                }

                if (!tracker.IsAnnotationWritten(annotation.TermName))
                {
                    this.WriteInstanceAnnotation(annotation);
                    tracker.MarkAnnotationWritten(annotation.TermName);
                }
            }
        }

        /// <summary>
        /// Writes a single instance annotation in ATOM format.
        /// </summary>
        /// <param name="instanceAnnotation">The instance annotation to write.</param>
        internal void WriteInstanceAnnotation(AtomInstanceAnnotation instanceAnnotation)
        {
            Debug.Assert(instanceAnnotation != null, "instanceAnnotation != null");
            Debug.Assert(!string.IsNullOrEmpty(instanceAnnotation.TermName), "!string.IsNullOrEmpty(instanceAnnotation.TermName)");

            if (this.MessageWriterSettings.ShouldSkipAnnotation(instanceAnnotation.TermName))
            {
                return;
            }

            IEdmTypeReference expectedType = MetadataUtils.LookupTypeOfValueTerm(instanceAnnotation.TermName, this.Model);
            this.WriteInstanceAnnotationStart(instanceAnnotation);

            ODataPrimitiveValue primitiveValue = instanceAnnotation.Value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                this.WritePrimitiveInstanceAnnotationValue(primitiveValue, expectedType);
            }
            else
            {
                ODataComplexValue complexValue = instanceAnnotation.Value as ODataComplexValue;
                if (complexValue != null)
                {
                    this.WriteComplexValue(
                        complexValue,
                        expectedType,
                        /*isOpenPropertyType*/ false,
                        /*isWritingCollection*/ false,
                        /*beforeValueAction*/ null,
                        /*afterValueAction*/ null,
                        this.CreateDuplicatePropertyNamesChecker(),
                        /*collectionValidator*/ null,
                        /*projectedProperties*/ null);
                }
                else
                {
                    ODataCollectionValue collectionValue = instanceAnnotation.Value as ODataCollectionValue;
                    if (collectionValue != null)
                    {
                        this.WriteCollectionValue(
                            collectionValue,
                            expectedType,
                            /*isOpenPropertyType*/ false,
                            /*isWritingCollection*/ false);
                    }
                    else
                    {
                        // Note that the ODataInstanceAnnotation constructor validates that the value is never an ODataStreamReferenceValue
                        Debug.Assert(instanceAnnotation.Value is ODataNullValue, "instanceAnnotation.Value is ODataNullValue");
                        if (expectedType != null && !expectedType.IsNullable)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(instanceAnnotation.TermName, expectedType.FullName()));
                        }

                        this.WriteNullAttribute();
                    }
                }
            }

            this.WriteInstanceAnnotationEnd();
        }

        /// <summary>
        /// Write the given collection of properties.
        /// </summary>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="cachedProperties">Collection of cached properties for the entry.</param>
        /// <param name="isWritingCollection">true if we are writing a top level collection instead of an entry.</param>
        /// <param name="beforePropertiesAction">Action which is called before the properties are written, if there are any property.</param>
        /// <param name="afterPropertiesAction">Action which is called after the properties are written, if there are any property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        internal bool WriteProperties(
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> cachedProperties,
            bool isWritingCollection,
            Action beforePropertiesAction,
            Action afterPropertiesAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            if (cachedProperties == null)
            {
                return false;
            }

            bool propertyWritten = false;
            foreach (ODataProperty property in cachedProperties)
            {
                propertyWritten |= this.WriteProperty(
                    property,
                    owningType,
                    /*isTopLevel*/false,
                    isWritingCollection,
                    propertyWritten ? null : beforePropertiesAction,
                    duplicatePropertyNamesChecker,
                    projectedProperties);
            }

            if (afterPropertiesAction != null && propertyWritten)
            {
                afterPropertiesAction();
            }

            return propertyWritten;
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="collectionValidator">The collection validator instance.</param>
        /// <param name="expectedTypeReference">The expected type of the primitive value.</param>
        /// <param name="typeNameAnnotation">The optional type name annotation provided by the user on the OM for this primitive value. The annotation value will override whatever type name is being written.</param>
        internal void WritePrimitiveValue(
            object value,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            IEdmTypeReference expectedTypeReference,
            SerializationTypeNameAnnotation typeNameAnnotation)
        {
            Debug.Assert(value != null, "value != null");

            IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            if (primitiveTypeReference == null)
            {
                throw new ODataException(ODataErrorStrings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            if (collectionValidator != null)
            {
                collectionValidator.ValidateCollectionItem(primitiveTypeReference.FullName(), EdmTypeKind.Primitive);
            }

            if (expectedTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, primitiveTypeReference, expectedTypeReference);
            }

            string collectionItemTypeName;
            string typeName = this.AtomOutputContext.TypeNameOracle.GetValueTypeNameForWriting(value, primitiveTypeReference, typeNameAnnotation, collectionValidator, out collectionItemTypeName);
            Debug.Assert(collectionItemTypeName == null, "collectionItemTypeName == null");

            if (typeName != null && typeName != EdmConstants.EdmStringTypeName)
            {
                this.WritePropertyTypeAttribute(typeName);
            }

            AtomValueUtils.WritePrimitiveValue(this.XmlWriter, value);
        }

        /// <summary>
        /// Writes an enumeration value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="collectionValidator">The collection validator instance.</param>
        /// <param name="expectedTypeReference">The expected type of the enumeration value.</param>
        /// <param name="typeNameAnnotation">The optional type name annotation provided by the user on the OM for this enumeration value. The annotation value will override whatever type name is being written.</param>
        internal void WriteEnumValue(
            ODataEnumValue value,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            IEdmTypeReference expectedTypeReference,
            SerializationTypeNameAnnotation typeNameAnnotation)
        {
            Debug.Assert(value != null, "value != null");

            // write type name without validation:
            // TODO: consider adding a overloaded GetValueTypeNameForWriting() without out parameter.
            string collectionItemTypeName;
            string typeName = this.AtomOutputContext.TypeNameOracle.GetValueTypeNameForWriting(value, expectedTypeReference, typeNameAnnotation, collectionValidator, out collectionItemTypeName);
            Debug.Assert(collectionItemTypeName == null, "collectionItemTypeName == null");
            if (typeName != null)
            {
                Debug.Assert(typeName != EdmConstants.EdmStringTypeName, "Enum typeName != EdmConstants.StringTypeName");
                this.WritePropertyTypeAttribute(typeName);
            }

            // write string value without validation:
            AtomValueUtils.WritePrimitiveValue(this.XmlWriter, value.Value);
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="metadataTypeReference">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="beforeValueAction">Action called before the complex value is written, if it's actually written.</param>
        /// <param name="afterValueAction">Action called after the copmlex value is written, if it's actually written.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="collectionValidator">The collection validator instance to validate the type names and type kinds of collection items; null if no validation is needed.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
        internal bool WriteComplexValue(
            ODataComplexValue complexValue,
            IEdmTypeReference metadataTypeReference,
            bool isOpenPropertyType,
            bool isWritingCollection,
            Action beforeValueAction,
            Action afterValueAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            Debug.Assert(complexValue != null, "complexValue != null");

            string typeName = complexValue.TypeName;

            if (collectionValidator != null)
            {
                collectionValidator.ValidateCollectionItem(typeName, EdmTypeKind.Complex);
            }

            this.IncreaseRecursionDepth();

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmComplexTypeReference complexTypeReference = TypeNameOracle.ResolveAndValidateTypeForComplexValue(this.Model, metadataTypeReference, complexValue, isOpenPropertyType, this.WriterValidator).AsComplexOrNull();

            string collectionItemTypeName;
            typeName = this.AtomOutputContext.TypeNameOracle.GetValueTypeNameForWriting(complexValue, complexTypeReference, complexValue.GetAnnotation<SerializationTypeNameAnnotation>(), collectionValidator, out collectionItemTypeName);
            Debug.Assert(collectionItemTypeName == null, "collectionItemTypeName == null");

            Action beforeValueCallbackWithTypeName = beforeValueAction;
            if (typeName != null)
            {
                // The beforeValueAction (if specified) will write the actual property element start.
                // So if we are to write the type attribute, we must postpone that after the start element was written.
                // And so we chain the existing action with our type attribute writing and use that
                // as the before action instead.
                if (beforeValueAction != null)
                {
                    beforeValueCallbackWithTypeName = () =>
                    {
                        beforeValueAction();
                        this.WritePropertyTypeAttribute(typeName);
                    };
                }
                else
                {
                    this.WritePropertyTypeAttribute(typeName);
                }
            }

            bool propertyWritten = this.WriteProperties(
                complexTypeReference == null ? null : complexTypeReference.ComplexDefinition(),
                complexValue.Properties,
                isWritingCollection,
                beforeValueCallbackWithTypeName,
                afterValueAction,
                duplicatePropertyNamesChecker,
                projectedProperties);

            this.DecreaseRecursionDepth();
            return propertyWritten;
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
        /// Write the items of a collection in ATOM format.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="propertyTypeReference">The type reference of the collection value (or null if not metadata is available).</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">true if we are writing a top-level collection instead of an entry.</param>
        private void WriteCollectionValue(
            ODataCollectionValue collectionValue,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            bool isWritingCollection)
        {
            Debug.Assert(collectionValue != null, "collectionValue != null");

            this.IncreaseRecursionDepth();

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)TypeNameOracle.ResolveAndValidateTypeForCollectionValue(this.Model, propertyTypeReference, collectionValue, isOpenPropertyType, this.WriterValidator);

            string collectionItemTypeName;
            string typeName = this.AtomOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue, collectionTypeReference, collectionValue.GetAnnotation<SerializationTypeNameAnnotation>(), /*collectionValidator*/ null, out collectionItemTypeName);

            if (typeName != null)
            {
                this.WritePropertyTypeAttribute(typeName);
            }

            // COMPAT 4: ATOM and JSON format for collections - The collection format is not formally specified yet, since it's a new feature
            // If the official format deviates from the current WCFDS behavior we would have to introduce a back compat mode here as well.
            IEdmTypeReference expectedItemTypeReference = collectionTypeReference == null ? null : collectionTypeReference.ElementType();

            CollectionWithoutExpectedTypeValidator collectionValidator = new CollectionWithoutExpectedTypeValidator(collectionItemTypeName);

            IEnumerable items = collectionValue.Items;
            if (items != null)
            {
                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    ValidationUtils.ValidateCollectionItem(item, expectedItemTypeReference.IsNullable());

                    this.XmlWriter.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataCollectionItemElementName, AtomConstants.ODataMetadataNamespace);
                    ODataComplexValue complexValue = item as ODataComplexValue;
                    if (complexValue != null)
                    {
                        if (duplicatePropertyNamesChecker == null)
                        {
                            duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                        }

                        this.WriteComplexValue(
                            complexValue,
                            expectedItemTypeReference,
                            false,
                            isWritingCollection,
                            null /* beforeValueAction */,
                            null /* afterValueAction */,
                            duplicatePropertyNamesChecker,
                            collectionValidator,
                            null /* projectedProperties */);

                        duplicatePropertyNamesChecker.Clear();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");

                        ODataEnumValue enumValue = item as ODataEnumValue;
                        if (enumValue != null)
                        {
                            // Note: Currently there is no way for a user to control enum type's serializationTypeNameAnnotation information when the enum values are part of a collection.
                            this.WriteEnumValue(enumValue, collectionValidator, expectedItemTypeReference, /*serializationTypeNameAnnotation*/ null);
                        }
                        else
                        {
                            // Note: Currently there is no way for a user to control primitive type information when the primitive values are part of a collection.
                            if (item != null)
                            {
                                this.WritePrimitiveValue(item, collectionValidator, expectedItemTypeReference, /*serializationTypeNameAnnotation*/ null);
                            }
                            else
                            {
                                this.WriteNullCollectionElementValue(expectedItemTypeReference);
                            }
                        }
                    }

                    this.XmlWriter.WriteEndElement();
                }
            }

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes the value of a primitive instance annotation.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to write.</param>
        /// <param name="expectedTypeReference">The expected type of the annotation from the metadata.</param>
        private void WritePrimitiveInstanceAnnotationValue(ODataPrimitiveValue primitiveValue, IEdmTypeReference expectedTypeReference)
        {
            object clrValue = primitiveValue.Value;
            IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(clrValue.GetType());
            string attributeValueNotationName = AtomInstanceAnnotation.LookupAttributeValueNotationNameByEdmTypeKind(primitiveTypeReference.PrimitiveKind());

            // Some primitive values can be specified more concisely via an attribute rather than in the content of the xml element. This is called "attribute value notation".
            // If we're writing a type that supports this, then we always prefer attribute value notation over writing the value in the element content.
            if (attributeValueNotationName != null)
            {
                if (expectedTypeReference != null)
                {
                    ValidationUtils.ValidateIsExpectedPrimitiveType(primitiveValue.Value, primitiveTypeReference, expectedTypeReference);
                }

                this.XmlWriter.WriteAttributeString(attributeValueNotationName, AtomValueUtils.ConvertPrimitiveToString(clrValue));
            }
            else
            {
                this.WritePrimitiveValue(clrValue, /*collectionValidator*/ null, expectedTypeReference, primitiveValue.GetAnnotation<SerializationTypeNameAnnotation>());
            }
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The owning type for the <paramref name="property"/> or null if no metadata is available.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a top-level collection instead of an entry.</param>
        /// <param name="beforePropertyAction">Action which is called before the property is written, if it's going to be written.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if the property was actually written, false otherwise.</returns>
        private bool WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool isTopLevel,
            bool isWritingCollection,
            Action beforePropertyAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            WriterValidationUtils.ValidatePropertyNotNull(property);

            object value = property.Value;
            string propertyName = property.Name;
            //// TODO: If we implement type conversions the value needs to be converted here
            ////       since the next method call needs to know if the value is a string or not in some cases.

            ODataComplexValue complexValue = value as ODataComplexValue;
            ProjectedPropertiesAnnotation complexValueProjectedProperties = null;
            if (!ShouldWritePropertyInContent(projectedProperties, propertyName))
            {
                return false;
            }

            WriterValidationUtils.ValidatePropertyName(propertyName);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType);
            IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;

            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
            }

            // Null property value.
            if (value == null)
            {
                this.WriteNullPropertyValue(propertyTypeReference, propertyName, isTopLevel, isWritingCollection, beforePropertyAction);
                return true;
            }

            bool isOpenPropertyType = owningType != null && owningType.IsOpen && propertyTypeReference == null;
            if (isOpenPropertyType)
            {
                ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
            }

            if (complexValue != null)
            {
                return this.WriteComplexValueProperty(
                    complexValue,
                    propertyName,
                    isTopLevel,
                    isWritingCollection,
                    beforePropertyAction,
                    propertyTypeReference,
                    isOpenPropertyType,
                    complexValueProjectedProperties);
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                this.WriteCollectionValueProperty(
                    collectionValue,
                    propertyName,
                    isTopLevel,
                    isWritingCollection,
                    beforePropertyAction,
                    propertyTypeReference,
                    isOpenPropertyType);

                return true;
            }

            // If the value isn't one of the value types tested for already, it must be a non-null primitive or enum type.
            this.WritePropertyStart(beforePropertyAction, property, isWritingCollection, isTopLevel);
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = property.ODataValue.GetAnnotation<SerializationTypeNameAnnotation>();
            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                this.WriteEnumValue(enumValue, /*collectionValidator*/ null, propertyTypeReference, serializationTypeNameAnnotation);
            }
            else
            {
                this.WritePrimitiveValue(value, /*collectionValidator*/ null, propertyTypeReference, serializationTypeNameAnnotation);
            }

            this.WritePropertyEnd();
            return true;
        }

        /// <summary>
        /// Writes a property with a complex value in ATOM format.
        /// </summary>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="propertyName">The name of the property being written.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a top-level collection instead of an entry.</param>
        /// <param name="beforeValueAction">Action called before the complex value is written, if it's actually written.</param>
        /// <param name="propertyTypeReference">The type information for the property being written.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="complexValueProjectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        private bool WriteComplexValueProperty(
            ODataComplexValue complexValue,
            string propertyName,
            bool isTopLevel,
            bool isWritingCollection,
            Action beforeValueAction,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            ProjectedPropertiesAnnotation complexValueProjectedProperties)
        {
            // Complex properties are written recursively.
            DuplicatePropertyNamesChecker complexValuePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
            if (isTopLevel)
            {
                // Top-level property must always write the property element
                Debug.Assert(complexValueProjectedProperties == null, "complexValueProjectedProperties == null");
                this.WritePropertyStart(beforeValueAction, propertyName, complexValue, isWritingCollection, /*isTopLevel*/ true);

                this.AssertRecursionDepthIsZero();
                this.WriteComplexValue(
                    complexValue,
                    propertyTypeReference,
                    isOpenPropertyType,
                    isWritingCollection,
                    null /* beforeValueAction */,
                    null /* afterValueAction */,
                    complexValuePropertyNamesChecker,
                    null /* collectionValidator */,
                    null /* projectedProperties */);
                this.AssertRecursionDepthIsZero();
                this.WritePropertyEnd();
                return true;
            }

            return this.WriteComplexValue(
                complexValue,
                propertyTypeReference,
                isOpenPropertyType,
                isWritingCollection,
                () => this.WritePropertyStart(beforeValueAction, propertyName, complexValue, isWritingCollection, /*isTopLevel*/ false),
                this.WritePropertyEnd,
                complexValuePropertyNamesChecker,
                null /* collectionValidator */,
                complexValueProjectedProperties);
        }

        /// <summary>
        /// Writes a property with a collection value in ATOM format.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="propertyName">The name of the property being written.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingTopLevelCollection">true if writing a top-level collection payload instead of an entry.</param>
        /// <param name="beforePropertyAction">Action which is called before the property is written, if it's going to be written.</param>
        /// <param name="propertyTypeReference">The type reference of the collection value (or null if no metadata is available).</param>
        /// <param name="isOpenPropertyType">true if this property is undeclared and the owning type is open.</param>
        private void WriteCollectionValueProperty(
            ODataCollectionValue collectionValue,
            string propertyName,
            bool isTopLevel,
            bool isWritingTopLevelCollection,
            Action beforePropertyAction,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType)
        {
            this.WritePropertyStart(beforePropertyAction, propertyName, collectionValue, isWritingTopLevelCollection, isTopLevel);
            this.WriteCollectionValue(collectionValue, propertyTypeReference, isOpenPropertyType, isWritingTopLevelCollection);
            this.WritePropertyEnd();
        }

        /// <summary>
        /// Determines if the property with the specified value should be written into content or not.
        /// </summary>
        /// <param name="projectedProperties">The set of projected properties for the owning type</param>
        /// <param name="propertyName">The name of the property to be checked.</param>
        /// <returns>true if the property should be written into content, or false otherwise</returns>
        private static bool ShouldWritePropertyInContent(
            ProjectedPropertiesAnnotation projectedProperties,
            string propertyName)
        {
            // check whether the property is projected;
            bool propertyProjected = !projectedProperties.ShouldSkipProperty(propertyName);
            return propertyProjected;
        }

        /// <summary>
        /// Writes a null property value in Atom format.
        /// </summary>
        /// <param name="propertyTypeReference">The property type or null if we don't have any.</param>
        /// <param name="propertyName">The name of the property to write out.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="beforePropertyAction">Action which is called before the property is written, if it's going to be written.</param>
        private void WriteNullPropertyValue(
            IEdmTypeReference propertyTypeReference,
            string propertyName,
            bool isTopLevel,
            bool isWritingCollection,
            Action beforePropertyAction)
        {
            WriterValidationUtils.ValidateNullPropertyValue(propertyTypeReference, propertyName, this.MessageWriterSettings.WriterBehavior, this.Model);

            // <d:PropertyName
            this.WritePropertyStart(beforePropertyAction, propertyName, new ODataNullValue(), isWritingCollection, isTopLevel);

            // The default behavior is to not write type name for null values.
            if (propertyTypeReference != null && !this.UseDefaultFormatBehavior)
            {
                string typeName = propertyTypeReference.FullName();

                if (typeName != Metadata.EdmConstants.EdmStringTypeName)
                {
                    // For WCF DS Client we write the type name on null values only for primitive types
                    // For WCF DS Server we write the type name on null values always
                    if (propertyTypeReference.IsODataPrimitiveTypeKind() || this.UseServerFormatBehavior)
                    {
                        // m:type = 'type name'
                        this.WritePropertyTypeAttribute(typeName);
                    }
                }
            }

            // m:null = 'true'
            this.WriteNullAttribute();

            // />
            this.WritePropertyEnd();
        }

        /// <summary>
        /// Write null collection element value.
        /// </summary>
        /// <param name="propertyTypeReference">The property type reference.</param>
        private void WriteNullCollectionElementValue(IEdmTypeReference propertyTypeReference)
        {
            ValidationUtils.ValidateNullCollectionItem(
                propertyTypeReference,
                this.AtomOutputContext.MessageWriterSettings.WriterBehavior);

            // NOTE can't use ODataAtomWriterUtils.WriteNullAttribute because that method assumes the
            //      default 'm' prefix for the metadata namespace.
            this.AtomOutputContext.XmlWriter.WriteAttributeString(
                AtomConstants.ODataNullAttributeName,
                AtomConstants.ODataMetadataNamespace,
                AtomConstants.AtomTrueLiteral);
        }

        /// <summary>
        /// Writes the property start element.
        /// </summary>
        /// <param name="beforePropertyCallback">Action called before anything else is written (if it's not null).</param>
        /// <param name="propertyName">The name of the property to write.</param>
        /// <param name="value">The odata value to write.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        private void WritePropertyStart(Action beforePropertyCallback, string propertyName, ODataValue value, bool isWritingCollection, bool isTopLevel)
        {
            if (beforePropertyCallback != null)
            {
                beforePropertyCallback();
            }

            if (!isTopLevel)
            {
                // <d:propertyname>
                this.XmlWriter.WriteStartElement(
                    isWritingCollection ? string.Empty : AtomConstants.ODataNamespacePrefix,
                    propertyName,
                    AtomConstants.ODataNamespace);
            }
            else
            {
                // <m:value>
                this.XmlWriter.WriteStartElement(
                    AtomConstants.ODataMetadataNamespacePrefix,
                    AtomConstants.ODataValueElementName,
                    AtomConstants.ODataMetadataNamespace);

                // COMPAT 24: Use standard ('d' and 'm') namespace prefixes for top-level property payloads
                // Top-level collection payloads don't write namespace declarations on the root element (except for the d namespace)
                //             We decided to use the same set of default namespaces on entries, feeds, collections and top-level properties
                DefaultNamespaceFlags namespaces = DefaultNamespaceFlags.Gml | DefaultNamespaceFlags.GeoRss;
                if (!this.MessageWriterSettings.AlwaysUseDefaultXmlNamespaceForRootElement)
                {
                    // DEVNOTE: no need to include the OData namespace here, because we already defined it above.
                    // However, the order will change if we leave it to XmlWriter to add it. So, if the knob hasn't been flipped,
                    // manually add it.
                    namespaces |= DefaultNamespaceFlags.OData;
                }

                this.WriteDefaultNamespaceAttributes(namespaces);

                ODataContextUriBuilder contextUriBuilder = this.AtomOutputContext.CreateContextUriBuilder();

                ODataPayloadKind kind = this.AtomOutputContext.MessageWriterSettings.IsIndividualProperty ? ODataPayloadKind.IndividualProperty : ODataPayloadKind.Property;

                ODataContextUrlInfo contextInfo = ODataContextUrlInfo.Create(value, this.AtomOutputContext.MessageWriterSettings.ODataUri);
                this.WriteContextUriProperty(contextUriBuilder.BuildContextUri(kind, contextInfo));
            }
        }

        /// <summary>
        /// Writes the property start element.
        /// </summary>
        /// <param name="beforePropertyCallback">Action called before anything else is written (if it's not null).</param>
        /// <param name="property">The odata property to write.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        private void WritePropertyStart(Action beforePropertyCallback, ODataProperty property, bool isWritingCollection, bool isTopLevel)
        {
            this.WritePropertyStart(beforePropertyCallback, property.Name, property.ODataValue, isWritingCollection, isTopLevel);
        }

        /// <summary>
        /// Writes the property end element.
        /// </summary>
        private void WritePropertyEnd()
        {
            // </d:propertyname>
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes the instance annotation start element.
        /// </summary>
        /// <param name="instanceAnnotation">The the instance annotation to write.</param>
        private void WriteInstanceAnnotationStart(AtomInstanceAnnotation instanceAnnotation)
        {
            // <m:annotation>
            this.XmlWriter.WriteStartElement(AtomConstants.ODataAnnotationElementName, AtomConstants.ODataMetadataNamespace);

            // <m:annotation term="instanceAnnotation.TermName">
            this.XmlWriter.WriteAttributeString(AtomConstants.ODataAnnotationTermAttribute, string.Empty, instanceAnnotation.TermName);

            if (!string.IsNullOrEmpty(instanceAnnotation.Target))
            {
                // <m:annotation term="termName" target="instanceAnnotation.Target">
                this.XmlWriter.WriteAttributeString(AtomConstants.ODataAnnotationTargetAttribute, string.Empty, instanceAnnotation.Target);
            }
        }

        /// <summary>
        /// Writes the instance annotation end element.
        /// </summary>
        private void WriteInstanceAnnotationEnd()
        {
            // </m:annotation>
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes the m:type attribute for a property given the name of the type.
        /// </summary>
        /// <param name="typeName">The type name to write.</param>
        private void WritePropertyTypeAttribute(string typeName)
        {
            // m:type attribute
            this.XmlWriter.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.AtomTypeAttributeName,
                AtomConstants.ODataMetadataNamespace,
                ODataAtomWriterUtils.PrefixTypeName(WriterUtils.RemoveEdmPrefixFromTypeName(typeName)));
        }

        /// <summary>
        /// Write the m:null attribute with a value of 'true'
        /// </summary>
        private void WriteNullAttribute()
        {
            // m:null="true"
            this.XmlWriter.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataNullAttributeName,
                AtomConstants.ODataMetadataNamespace,
                AtomConstants.AtomTrueLiteral);
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
