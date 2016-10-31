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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        internal void WriteTopLevelProperty(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();

            this.WritePayloadStart();
            this.AssertRecursionDepthIsZero();
            this.WriteProperty(
                property,
                null /*owningType*/,
                true  /* isTopLevel */,
                false /* isWritingCollection */,
                null  /* beforePropertyAction */,
                null  /* epmValueCache */,
                null  /* epmParentSourcePathSegment */,
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
                        /*epmValueCache*/ null,
                        /*epmSourcePathSegment*/ null,
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
                            throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(instanceAnnotation.TermName, expectedType.ODataFullName()));
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
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property which sub-properites we're writing. (can be null)</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        internal bool WriteProperties(
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> cachedProperties,
            bool isWritingCollection,
            Action beforePropertiesAction,
            Action afterPropertiesAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmSourcePathSegment,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();

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
                    epmValueCache,
                    epmSourcePathSegment,
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
            DebugUtils.CheckNoExternalCallers();
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
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property we're writing. (can be null)</param>
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
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmSourcePathSegment,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");

            string typeName = complexValue.TypeName;

            if (collectionValidator != null)
            {
                collectionValidator.ValidateCollectionItem(typeName, EdmTypeKind.Complex);
            }

            this.IncreaseRecursionDepth();

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmComplexTypeReference complexTypeReference = TypeNameOracle.ResolveAndValidateTypeNameForValue(this.Model, metadataTypeReference, complexValue, isOpenPropertyType).AsComplexOrNull();

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

            // NOTE: see the comments on ODataWriterBehavior.UseV1ProviderBehavior for more information
            // NOTE: We have to check for ProjectedPropertiesAnnotation.Empty here to avoid filling the cache for
            //       complex values we are writing only to ensure we don't have nested EPM-mapped null values 
            //       that will end up in the content eventually.
            if (this.MessageWriterSettings.WriterBehavior != null &&
                this.MessageWriterSettings.WriterBehavior.UseV1ProviderBehavior &&
                !object.ReferenceEquals(projectedProperties, ProjectedPropertiesAnnotation.EmptyProjectedPropertiesInstance))
            {
                IEdmComplexType complexType = (IEdmComplexType)complexTypeReference.Definition;
                CachedPrimitiveKeepInContentAnnotation keepInContentCache = this.Model.EpmCachedKeepPrimitiveInContent(complexType);
                if (keepInContentCache == null)
                {
                    // we are about to write the first value of the given type; compute the keep-in-content information for the primitive properties of this type.
                    List<string> keepInContentPrimitiveProperties = null;

                    // initialize the cache with all primitive properties
                    foreach (IEdmProperty edmProperty in complexType.Properties().Where(p => p.Type.IsODataPrimitiveTypeKind()))
                    {
                        // figure out the keep-in-content value
                        EntityPropertyMappingAttribute entityPropertyMapping = EpmWriterUtils.GetEntityPropertyMapping(epmSourcePathSegment, edmProperty.Name);
                        if (entityPropertyMapping != null && entityPropertyMapping.KeepInContent)
                        {
                            if (keepInContentPrimitiveProperties == null)
                            {
                                keepInContentPrimitiveProperties = new List<string>();
                            }

                            keepInContentPrimitiveProperties.Add(edmProperty.Name);
                        }
                    }

                    this.Model.SetAnnotationValue<CachedPrimitiveKeepInContentAnnotation>(complexType, new CachedPrimitiveKeepInContentAnnotation(keepInContentPrimitiveProperties));
                }
            }

            bool propertyWritten = this.WriteProperties(
                complexTypeReference == null ? null : complexTypeReference.ComplexDefinition(),
                EpmValueCache.GetComplexValueProperties(epmValueCache, complexValue, true),
                isWritingCollection,
                beforeValueCallbackWithTypeName,
                afterValueAction,
                duplicatePropertyNamesChecker,
                epmValueCache,
                epmSourcePathSegment,
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
            DebugUtils.CheckNoExternalCallers();
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
            IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)TypeNameOracle.ResolveAndValidateTypeNameForValue(this.Model, propertyTypeReference, collectionValue, isOpenPropertyType);

            string collectionItemTypeName;
            string typeName = this.AtomOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue, collectionTypeReference, collectionValue.GetAnnotation<SerializationTypeNameAnnotation>(), /*collectionValidator*/ null, out collectionItemTypeName);

            if (typeName != null)
            {
                this.WritePropertyTypeAttribute(typeName);
            }

            IEdmTypeReference expectedItemTypeReference = collectionTypeReference == null ? null : collectionTypeReference.ElementType();

            CollectionWithoutExpectedTypeValidator collectionValidator = new CollectionWithoutExpectedTypeValidator(collectionItemTypeName);

            IEnumerable items = collectionValue.Items;
            if (items != null)
            {
                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    ValidationUtils.ValidateCollectionItem(item, false /* isStreamable */);

                    this.XmlWriter.WriteStartElement(AtomConstants.ODataNamespacePrefix, AtomConstants.ODataCollectionItemElementName, this.MessageWriterSettings.WriterBehavior.ODataNamespace);
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
                            null /* epmValueCache */,
                            null /* epmSourcePathSegment */,
                            null /* projectedProperties */);

                        duplicatePropertyNamesChecker.Clear();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");

                        // Note: Currently there is no way for a user to control primitive type information when the primitive values are part of a collection.
                        this.WritePrimitiveValue(item, collectionValidator, expectedItemTypeReference, /*serializationTypeNameAnnotation*/ null);
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
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmParentSourcePathSegment">The EPM source path segment which points to the property which sub-property we're writing. (can be null)</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if the property was actually written, false otherwise.</returns>
        private bool WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool isTopLevel,
            bool isWritingCollection,
            Action beforePropertyAction,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmParentSourcePathSegment,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();

            WriterValidationUtils.ValidatePropertyNotNull(property);

            object value = property.Value;
            string propertyName = property.Name;
            EpmSourcePathSegment epmSourcePathSegment = EpmWriterUtils.GetPropertySourcePathSegment(epmParentSourcePathSegment, propertyName);

            //// TODO: If we implement type conversions the value needs to be converted here
            ////       since the next method call needs to know if the value is a string or not in some cases.

            ODataComplexValue complexValue = value as ODataComplexValue;
            ProjectedPropertiesAnnotation complexValueProjectedProperties = null;
            if (!this.ShouldWritePropertyInContent(owningType, projectedProperties, propertyName, value, epmSourcePathSegment))
            {
                // If ShouldWritePropertyInContent returns false for a comlex value we have to continue
                // writing the property but set the projectedProperties to an empty array. The reason for this
                // is that we might find EPM on a nested property that has a null value and thus must be written 
                // in content (in which case the parent property also has to be written).
                // This only applies if we have EPM information for the property.
                if (epmSourcePathSegment != null && complexValue != null)
                {
                    Debug.Assert(!projectedProperties.IsPropertyProjected(propertyName), "ShouldWritePropertyInContent must not return false for a projected complex property.");
                    complexValueProjectedProperties = ProjectedPropertiesAnnotation.EmptyProjectedPropertiesInstance;
                }
                else
                {
                    return false;
                }
            }

            WriterValidationUtils.ValidatePropertyName(propertyName);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType, this.MessageWriterSettings.UndeclaredPropertyBehaviorKinds);
            IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;
            
            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
            }

            // If the property is of Geography or Geometry type or the value is of Geography or Geometry type
            // make sure to check that the version is 3.0 or above.
            if ((propertyTypeReference != null && propertyTypeReference.IsSpatial()) ||
                (propertyTypeReference == null && value is System.Spatial.ISpatial))
            {
                ODataVersionChecker.CheckSpatialValue(this.Version);
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
                ValidationUtils.ValidateOpenPropertyValue(propertyName, value, this.MessageWriterSettings.UndeclaredPropertyBehaviorKinds);
            }

            if (complexValue != null)
            {
                return this.WriteComplexValueProperty(
                    complexValue, 
                    propertyName, 
                    isTopLevel, 
                    isWritingCollection, 
                    beforePropertyAction, 
                    epmValueCache, 
                    propertyTypeReference, 
                    isOpenPropertyType, 
                    epmSourcePathSegment, 
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

            // If the value isn't one of the value types tested for already, it must be a non-null primitive.
            this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = property.ODataValue.GetAnnotation<SerializationTypeNameAnnotation>();
            this.WritePrimitiveValue(value, /*collectionValidator*/ null, propertyTypeReference, serializationTypeNameAnnotation);
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
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="propertyTypeReference">The type information for the property being written.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property we're writing. (can be null)</param>
        /// <param name="complexValueProjectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        private bool WriteComplexValueProperty(
            ODataComplexValue complexValue, 
            string propertyName, 
            bool isTopLevel, 
            bool isWritingCollection,
            Action beforeValueAction, 
            EpmValueCache epmValueCache, 
            IEdmTypeReference propertyTypeReference, 
            bool isOpenPropertyType, 
            EpmSourcePathSegment epmSourcePathSegment, 
            ProjectedPropertiesAnnotation complexValueProjectedProperties)
        {
            // Complex properties are written recursively.
            DuplicatePropertyNamesChecker complexValuePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
            if (isTopLevel)
            {
                // Top-level property must always write the property element
                Debug.Assert(complexValueProjectedProperties == null, "complexValueProjectedProperties == null");
                this.WritePropertyStart(beforeValueAction, propertyName, isWritingCollection, /*isTopLevel*/ true);
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
                    epmValueCache, 
                    epmSourcePathSegment, 
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
                () => this.WritePropertyStart(beforeValueAction, propertyName, isWritingCollection, /*isTopLevel*/ false),
                this.WritePropertyEnd, 
                complexValuePropertyNamesChecker, 
                null /* collectionValidator */, 
                epmValueCache, 
                epmSourcePathSegment, 
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
            ODataVersionChecker.CheckCollectionValueProperties(this.Version, propertyName);

            this.WritePropertyStart(beforePropertyAction, propertyName, isWritingTopLevelCollection, isTopLevel);
            this.WriteCollectionValue(collectionValue, propertyTypeReference, isOpenPropertyType, isWritingTopLevelCollection);
            this.WritePropertyEnd();
        }

        /// <summary>
        /// Determines if the property with the specified value should be written into content or not.
        /// </summary>
        /// <param name="owningType">The owning type of the property to be checked.</param>
        /// <param name="projectedProperties">The set of projected properties for the <paramref name="owningType"/></param>
        /// <param name="propertyName">The name of the property to be checked.</param>
        /// <param name="propertyValue">The property value to write.</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment for the property being written.</param>
        /// <returns>true if the property should be written into content, or false otherwise</returns>
        private bool ShouldWritePropertyInContent(
            IEdmStructuredType owningType,
            ProjectedPropertiesAnnotation projectedProperties,
            string propertyName,
            object propertyValue,
            EpmSourcePathSegment epmSourcePathSegment)
        {
            // check whether the property is projected; if no EPM is specified for the property the projection decides 
            bool propertyProjected = !projectedProperties.ShouldSkipProperty(propertyName);

            bool useV1ProviderBehavior = this.MessageWriterSettings.WriterBehavior == null ? false : this.MessageWriterSettings.WriterBehavior.UseV1ProviderBehavior;
            if (useV1ProviderBehavior && owningType != null && owningType.IsODataComplexTypeKind())
            {
                IEdmComplexType owningComplexType = (IEdmComplexType)owningType;
                CachedPrimitiveKeepInContentAnnotation keepInContentAnnotation = this.Model.EpmCachedKeepPrimitiveInContent(owningComplexType);
                if (keepInContentAnnotation != null && keepInContentAnnotation.IsKeptInContent(propertyName))
                {
                    return propertyProjected;
                }
            }

            // We sometimes write properties into content even if asked not to.
            // If the property value is null and the property (or one of its descendant properties) is mapped, 
            // we always write into content, even if the property was not projected.
            if (propertyValue == null && epmSourcePathSegment != null)
            {
                return true;
            }

            EntityPropertyMappingAttribute entityPropertyMapping = EpmWriterUtils.GetEntityPropertyMapping(epmSourcePathSegment);
            if (entityPropertyMapping == null)
            {
                return propertyProjected;
            }

            string stringPropertyValue = propertyValue as string;
            if (stringPropertyValue != null && stringPropertyValue.Length == 0)
            {
                // If the property value is an empty string and we should be writing it into an ATOM element which does not allow empty string
                // we write it into content as well, also even if the property was not projected.
                switch (entityPropertyMapping.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorEmail:
                    case SyndicationItemProperty.ContributorUri:
                        return true;

                    default:
                        break;
                }
            }

            return entityPropertyMapping.KeepInContent && propertyProjected;
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
            this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);

            // The default behavior is to not write type name for null values.
            if (propertyTypeReference != null && !this.UseDefaultFormatBehavior)
            {
                string typeName = propertyTypeReference.ODataFullName();

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
        /// Writes the property start element.
        /// </summary>
        /// <param name="beforePropertyCallback">Action called before anything else is written (if it's not null).</param>
        /// <param name="propertyName">The name of the property to write.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        private void WritePropertyStart(Action beforePropertyCallback, string propertyName, bool isWritingCollection, bool isTopLevel)
        {
            if (beforePropertyCallback != null)
            {
                beforePropertyCallback();
            }

            // <d:propertyname>
            this.XmlWriter.WriteStartElement(
                isWritingCollection ? string.Empty : AtomConstants.ODataNamespacePrefix,
                propertyName,
                this.MessageWriterSettings.WriterBehavior.ODataNamespace);

            if (isTopLevel)
            {
                DefaultNamespaceFlags namespaces = DefaultNamespaceFlags.ODataMetadata | DefaultNamespaceFlags.Gml | DefaultNamespaceFlags.GeoRss;
                if (!this.MessageWriterSettings.AlwaysUseDefaultXmlNamespaceForRootElement)
                {
                    // DEVNOTE: no need to include the OData namespace here, because we already defined it above.
                    // However, the order will change if we leave it to XmlWriter to add it. So, if the knob hasn't been flipped,
                    // manually add it.
                    namespaces |= DefaultNamespaceFlags.OData;
                }

                this.WriteDefaultNamespaceAttributes(namespaces);
            }
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
                typeName);
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
