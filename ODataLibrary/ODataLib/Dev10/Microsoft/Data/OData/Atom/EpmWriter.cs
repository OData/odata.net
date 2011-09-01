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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for EPM writers.
    /// </summary>
    internal abstract class EpmWriter
    {
        /// <summary>The metadata provider to use.</summary>
        private readonly IEdmModel model;

        /// <summary>The version of OData protocol to use.</summary>
        private readonly ODataVersion version;

        /// <summary>The settings to control the behavior of the writer.</summary>
        private readonly ODataWriterBehavior writerBehavior;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        protected EpmWriter(
            IEdmModel model,
            ODataVersion version,
            ODataWriterBehavior writerBehavior)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");
            
            this.model = model;
            this.version = version;
            this.writerBehavior = writerBehavior;
        }

        /// <summary>The version of OData protocol to use.</summary>
        protected ODataVersion Version
        {
            get
            {
                return this.version;
            }
        }

        /// <summary>
        /// Reads a property value starting on an entry.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="epmValueCache">The EPM value cache for the entry to read from.</param>
        /// <param name="entityType">The type of the entry.</param>
        /// <param name="valueTypeReference">The type of the value read. This may be null if the property in question was open.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        protected object ReadEntryPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference entityType,
            out IEdmTypeReference valueTypeReference,
            out bool nullOnParentProperty)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(epmInfo.PropertyValuePath != null, "The PropertyValuePath should have been initialized by now.");
            Debug.Assert(epmInfo.PropertyValuePath.Length > 0, "The PropertyValuePath must not be empty for an entry property.");
            Debug.Assert(entityType != null, "entityType != null");

            // TODO - It might be possible to avoid the "value" type checks below if we do property value validation based on the type
            return this.ReadPropertyValue(
                epmInfo,
                epmValueCache.EntryProperties,
                0,
                entityType,
                epmValueCache,
                out valueTypeReference,
                out nullOnParentProperty);
        }

        /// <summary>
        /// Reads a property value starting on a MultiValue item which is a complex type.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="epmValueCache">The EPM value cache for the item to read from.</param>
        /// <param name="itemTypeReference">The type of the MultiValue item.</param>
        /// <param name="valueTypeReference">The type of the value read. This may be null if the property in question was open.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        protected object ReadMultiValueItemPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            EpmMultiValueItemCache epmValueCache,
            IEdmTypeReference itemTypeReference,
            out IEdmTypeReference valueTypeReference)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(epmInfo.PropertyValuePath != null, "The PropertyValuePath should have been initialized by now.");
            Debug.Assert(itemTypeReference != null, "itemTypeReference != null");

            // Currently only complex values are cached in EpmMultiValueItemCache
            ODataComplexValue complexValue = (ODataComplexValue)epmValueCache.ItemValue;

            // Resolve the type name to the type reference; if no type name is specified we will use the 
            // type inferred from metadata
            string typeName = complexValue.TypeName;
            IEdmComplexTypeReference complexTypeReference = WriterValidationUtils.ResolveTypeNameForWriting(
                this.model,
                itemTypeReference,
                ref typeName,
                EdmTypeKind.Complex,
                false /* isOpenPropertyType */).AsComplexOrNull();

            bool nullOnParentProperty;
            return this.ReadPropertyValue(
                epmInfo,
                EpmValueCache.GetComplexValueProperties(epmValueCache, complexValue, false),
                0,
                complexTypeReference,
                epmValueCache,
                out valueTypeReference,
                out nullOnParentProperty);
        }

        /// <summary>
        /// Reads a property value starting on a complex value.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="complexValue">The complex value to start with.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="complexType">The type of the complex value.</param>
        /// <param name="valueTypeReference">The type of the value read. This may be null if the property in question was open.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadComplexPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            ODataComplexValue complexValue,
            EpmValueCache epmValueCache,
            int sourceSegmentIndex,
            IEdmComplexTypeReference complexType,
            out IEdmTypeReference valueTypeReference,
            out bool nullOnParentProperty)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(epmInfo.PropertyValuePath != null, "The PropertyValuePath should have been initialized by now.");
            Debug.Assert(epmInfo.PropertyValuePath.Length > sourceSegmentIndex, "The PropertyValuePath must be at least as long as the source segment index.");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(sourceSegmentIndex >= 0, "sourceSegmentIndex >= 0");
            Debug.Assert(complexType != null, "complexType != null");
            Debug.Assert(complexValue != null, "complexValue != null");

            return this.ReadPropertyValue(
                epmInfo,
                EpmValueCache.GetComplexValueProperties(epmValueCache, complexValue, false),
                sourceSegmentIndex,
                complexType,
                epmValueCache,
                out valueTypeReference,
                out nullOnParentProperty);
        }

        /// <summary>
        /// Reads a property value starting with the specified index to the property value path.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="cachedProperties">The enumeration of properties to search for the first property in the property value path.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="structuredTypeReference">The type of the entry or complex value the <paramref name="cachedProperties"/> enumeration belongs to.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <param name="valueTypeReference">The type of the value read. This may be null if the property in question was open.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            IEnumerable<ODataProperty> cachedProperties,
            int sourceSegmentIndex,
            IEdmStructuredTypeReference structuredTypeReference,
            EpmValueCache epmValueCache,
            out IEdmTypeReference valueTypeReference,
            out bool nullOnParentProperty)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(epmInfo.PropertyValuePath != null, "The PropertyValuePath should have been initialized by now.");
            Debug.Assert(epmInfo.PropertyValuePath.Length > sourceSegmentIndex, "The PropertyValuePath must be at least as long as the source segment index.");
            Debug.Assert(structuredTypeReference != null, "structuredTypeReference != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");

            EpmSourcePathSegment sourceSegment = epmInfo.PropertyValuePath[sourceSegmentIndex];
            string propertyName = sourceSegment.PropertyName;
            bool lastSegment = epmInfo.PropertyValuePath.Length == sourceSegmentIndex + 1;

            IEdmStructuredType structuredType = structuredTypeReference.StructuredDefinition();
            IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(propertyName, structuredType);
            if (edmProperty != null)
            {
                // If this is the last part of the path, then it has to be a primitive or multiValue type otherwise should be a complex type
                if (lastSegment)
                {
                    if (!edmProperty.Type.IsODataPrimitiveTypeKind() && !edmProperty.Type.IsODataMultiValueTypeKind())
                    {
                        throw new ODataException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                }
                else
                {
                    if (edmProperty.Type.TypeKind() != EdmTypeKind.Complex)
                    {
                        throw new ODataException(Strings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                    }
                }

                valueTypeReference = edmProperty.Type;
            }
            else
            {
                Debug.Assert(
                    structuredType.IsOpen,
                    "Only open types can have undeclared properties, otherwise we should have failed in the ValidatePropertyDefined method.");

                valueTypeReference = null;
            }

            ODataProperty property = cachedProperties == null ? null : cachedProperties.FirstOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new ODataException(Strings.EpmSourceTree_MissingPropertyOnInstance(propertyName, structuredTypeReference.ODataFullName()));
            }

            object propertyValue = property.Value;
            ODataComplexValue propertyComplexValue = propertyValue as ODataComplexValue;
            if (lastSegment)
            {
                if (propertyValue == null)
                {
                    ValidationUtils.ValidateNullPropertyValue(edmProperty, this.writerBehavior);
                }
                else
                {
                    // If this property is the last one it has to be either a primitive or multivalue
                    if (propertyComplexValue != null)
                    {
                        throw new ODataException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                    else
                    {
                        ODataMultiValue propertyMultiValue = propertyValue as ODataMultiValue;
                        if (propertyMultiValue != null)
                        {
                            // Validate the type name for the multivalue
                            string typeName = propertyMultiValue.TypeName;
                            valueTypeReference = WriterValidationUtils.ResolveTypeNameForWriting(
                                this.model,
                                edmProperty == null ? null : edmProperty.Type,
                                ref typeName,
                                EdmTypeKind.Collection,
                                edmProperty == null);
                        }
                        else
                        {
                            if (propertyValue is ODataStreamReferenceValue)
                            {
                                // NamedStream properties should not come here, if it were an ODataEntry property it would have been 
                                // filtered in ReadEntryPropertyValue() by "epmValueCache.EntryProperties" call.
                                throw new ODataException(Strings.ODataWriter_NamedStreamPropertiesMustBePropertiesOfODataEntry(propertyName));
                            }
                            else if (edmProperty != null)
                            {
                                ValidationUtils.ValidateIsExpectedPrimitiveType(propertyValue, edmProperty.Type);
                            }
                        }
                    }
                }

                nullOnParentProperty = false;
                return propertyValue;
            }

            // Otherwise it's in the middle and thus it must be a complex value
            if (propertyComplexValue == null)
            {
                if (propertyValue != null)
                {
                    // It's not a complex value - fail.
                    throw new ODataException(Strings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                }
                else
                {
                    // The value of the property is null, which can be a null complex value
                    // Note that we must not attempt to resolve the type as if the type name was null here, because
                    //  1) We don't need the type for anything anyway (the value is null, this is the end)
                    //  2) If the property is open, trying to resolve a null type name would throw
                    //     but we don't have a null type name, we have a null entire value.
                    nullOnParentProperty = true;
                    return null;
                }
            }

            string localTypeName = propertyComplexValue.TypeName;
            IEdmComplexTypeReference complexValueType = WriterValidationUtils.ResolveTypeNameForWriting(
                this.model,
                edmProperty == null ? null : edmProperty.Type,
                ref localTypeName,
                EdmTypeKind.Complex,
                edmProperty == null).AsComplexOrNull();

            return this.ReadComplexPropertyValue(
                epmInfo,
                propertyComplexValue,
                epmValueCache,
                sourceSegmentIndex + 1,
                complexValueType,
                out valueTypeReference,
                out nullOnParentProperty);
        }
    }
}
