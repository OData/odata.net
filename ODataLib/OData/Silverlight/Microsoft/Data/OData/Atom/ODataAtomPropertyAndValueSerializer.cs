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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using o = Microsoft.Data.OData;
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
                null  /* owningType */,
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
        /// Write the given collection of properties.
        /// </summary>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="cachedProperties">Collection of cached properties for the entry.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
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
                    false,
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
        internal void WritePrimitiveValue(
            object value,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            if (primitiveTypeReference == null)
            {
                throw new ODataException(o.Strings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            string typeName = primitiveTypeReference.FullName();
            if (collectionValidator != null)
            {
                collectionValidator.ValidateCollectionItem(typeName, EdmTypeKind.Primitive);

                if (string.CompareOrdinal(collectionValidator.ItemTypeNameFromCollection, typeName) == 0)
                {
                    typeName = null;
                }
            }

            if (expectedTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, primitiveTypeReference, expectedTypeReference);
            }

            if (typeName != null && typeName != Metadata.EdmConstants.EdmStringTypeName)
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
            IEdmComplexTypeReference complexTypeReference =
                WriterValidationUtils.ResolveTypeNameForWriting(this.Model, metadataTypeReference, ref typeName, EdmTypeKind.Complex, isOpenPropertyType).AsComplexOrNull();

            // If the type is the same as the one specified by the parent collection, omit the type name, since it's not needed.
            if (typeName != null && collectionValidator != null)
            {
                string expectedItemTypeName = collectionValidator.ItemTypeNameFromCollection;
                if (string.CompareOrdinal(expectedItemTypeName, typeName) == 0)
                {
                    typeName = null;
                }
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = complexValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

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
                !object.ReferenceEquals(projectedProperties, ProjectedPropertiesAnnotation.EmptyProjectedPropertiesMarker))
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
        /// <param name="propertyTypeReference">The type reference of the collection (or null if not metadata is available).</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        private void WriteCollectionValue(
            ODataCollectionValue collectionValue,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            bool isWritingCollection)
        {
            Debug.Assert(collectionValue != null, "collectionValue != null");

            this.IncreaseRecursionDepth();

            string typeName = collectionValue.TypeName;

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmCollectionTypeReference collectionTypeReference =
                (IEdmCollectionTypeReference)WriterValidationUtils.ResolveTypeNameForWriting(this.Model, propertyTypeReference, ref typeName, EdmTypeKind.Collection, isOpenPropertyType);

            string collectionItemTypeName = null;
            if (typeName != null)
            {
                collectionItemTypeName = ValidationUtils.ValidateCollectionTypeName(typeName);
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = collectionValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

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

                        this.WritePrimitiveValue(item, collectionValidator, expectedItemTypeReference);
                    }

                    this.XmlWriter.WriteEndElement();
                }
            }

            this.DecreaseRecursionDepth();
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
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
                    complexValueProjectedProperties = ProjectedPropertiesAnnotation.EmptyProjectedPropertiesMarker;
                }
                else
                {
                    return false;
                }
            }

            WriterValidationUtils.ValidateProperty(property);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType);

            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(o.Strings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
            }

            // If the property is of Geography or Geometry type or the value is of Geography or Geometry type
            // make sure to check that the version is 3.0 or above.
            if ((edmProperty != null && edmProperty.Type.IsSpatial()) ||
                (edmProperty == null && value is System.Spatial.ISpatial))
            {
                ODataVersionChecker.CheckSpatialValue(this.Version);
            }

            // Null property value.
            if (value == null)
            {
                this.WriteNullPropertyValue(edmProperty, propertyName, isTopLevel, isWritingCollection, beforePropertyAction);
                return true;
            }

            bool isOpenPropertyType = owningType != null && owningType.IsOpen && edmProperty == null;
            if (isOpenPropertyType)
            {
                ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
            }

            IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;
            if (complexValue != null)
            {
                // Complex properties are written recursively.
                DuplicatePropertyNamesChecker complexValuePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                if (isTopLevel)
                {
                    // Top-level property must always write the property element
                    Debug.Assert(complexValueProjectedProperties == null, "complexValueProjectedProperties == null");
                    this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
                    this.AssertRecursionDepthIsZero();
                    this.WriteComplexValue(
                        complexValue,
                        propertyTypeReference,
                        isOpenPropertyType,
                        isWritingCollection,
                        null  /* beforeValueAction */,
                        null  /* afterValueAction */,
                        complexValuePropertyNamesChecker,
                        null /* collectionValidator */,
                        epmValueCache,
                        epmSourcePathSegment,
                        null  /* projectedProperties */);
                    this.AssertRecursionDepthIsZero();
                    this.WritePropertyEnd();
                    return true;
                }

                return this.WriteComplexValue(
                    complexValue,
                    propertyTypeReference,
                    isOpenPropertyType,
                    isWritingCollection,
                    () => this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel),
                    () => this.WritePropertyEnd(),
                    complexValuePropertyNamesChecker,
                    null /* collectionValidator */,
                    epmValueCache,
                    epmSourcePathSegment,
                    complexValueProjectedProperties);
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataVersionChecker.CheckCollectionValueProperties(this.Version, propertyName);

                this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
                this.WriteCollectionValue(
                    collectionValue,
                    propertyTypeReference,
                    isOpenPropertyType,
                    isWritingCollection);
                this.WritePropertyEnd();
                return true;
            }

            this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
            this.WritePrimitiveValue(value, /*collectionValidator*/ null, propertyTypeReference);
            this.WritePropertyEnd();
            return true;
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
        /// <param name="edmProperty">The property to write out.</param>
        /// <param name="propertyName">The name of the property to write out.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="beforePropertyAction">Action which is called before the property is written, if it's going to be written.</param>
        private void WriteNullPropertyValue(
            IEdmProperty edmProperty,
            string propertyName,
            bool isTopLevel,
            bool isWritingCollection,
            Action beforePropertyAction)
        {
            WriterValidationUtils.ValidateNullPropertyValue(edmProperty, this.MessageWriterSettings.WriterBehavior, this.Model);

            // <d:PropertyName
            this.WritePropertyStart(beforePropertyAction, propertyName, isWritingCollection, isTopLevel);

            // The default behavior is to not write type name for null values.
            if (edmProperty != null && !this.UseDefaultFormatBehavior)
            {
                string typeName = edmProperty.Type.ODataFullName();

                if (typeName != Metadata.EdmConstants.EdmStringTypeName)
                {
                    // For WCF DS Client we write the type name on null values only for primitive types
                    // For WCF DS Server we write the type name on null values always
                    if (edmProperty.Type.IsODataPrimitiveTypeKind() || this.UseServerFormatBehavior)
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
                this.WriteDefaultNamespaceAttributes(
                    DefaultNamespaceFlags.OData | 
                    DefaultNamespaceFlags.ODataMetadata |
                    DefaultNamespaceFlags.Gml |
                    DefaultNamespaceFlags.GeoRss);
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
