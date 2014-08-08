//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Spatial;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Base class for EPM writers.
    /// </summary>
    internal abstract class EpmWriter
    {
        /// <summary>The output context currently in use.</summary>
        private readonly ODataAtomOutputContext atomOutputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context currently in use.</param>
        protected EpmWriter(ODataAtomOutputContext atomOutputContext)
        {
            Debug.Assert(atomOutputContext != null, "atomOutputContext != null");

            this.atomOutputContext = atomOutputContext;
        }

        /// <summary>The version of OData protocol to use.</summary>
        protected ODataVersion Version
        {
            get
            {
                return this.atomOutputContext.Version;
            }
        }

        /// <summary>The settings to control the behavior of the writer.</summary>
        protected ODataWriterBehavior WriterBehavior
        {
            get { return this.atomOutputContext.MessageWriterSettings.WriterBehavior; }
        }

        /// <summary>
        /// Reads a property value starting on an entry.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="epmValueCache">The EPM value cache for the entry to read from.</param>
        /// <param name="entityType">The type of the entry.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        protected object ReadEntryPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference entityType)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(epmInfo.PropertyValuePath != null, "The PropertyValuePath should have been initialized by now.");
            Debug.Assert(epmInfo.PropertyValuePath.Length > 0, "The PropertyValuePath must not be empty for an entry property.");
            Debug.Assert(entityType != null, "entityType != null");

            // TODO: It might be possible to avoid the "value" type checks below if we do property value validation based on the type
            return this.ReadPropertyValue(
                epmInfo,
                epmValueCache.EntryProperties,
                0,
                entityType,
                epmValueCache);
        }

        /// <summary>
        /// Reads a property value starting on a complex value.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="complexValue">The complex value to start with.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="complexType">The type of the complex value.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadComplexPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            ODataComplexValue complexValue,
            EpmValueCache epmValueCache,
            int sourceSegmentIndex,
            IEdmComplexTypeReference complexType)
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
                epmValueCache);
        }

        /// <summary>
        /// Reads a property value starting with the specified index to the property value path.
        /// </summary>
        /// <param name="epmInfo">The EPM info which describes the mapping for which to read the property value.</param>
        /// <param name="cachedProperties">The enumeration of properties to search for the first property in the property value path.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="structuredTypeReference">The type of the entry or complex value the <paramref name="cachedProperties"/> enumeration belongs to.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadPropertyValue(
            EntityPropertyMappingInfo epmInfo,
            IEnumerable<ODataProperty> cachedProperties,
            int sourceSegmentIndex,
            IEdmStructuredTypeReference structuredTypeReference,
            EpmValueCache epmValueCache)
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
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, structuredType);
            IEdmTypeReference propertyTypeReference = null;
            if (edmProperty != null)
            {
                propertyTypeReference = edmProperty.Type;

                // If this is the last part of the path, then it has to be a primitive or atomic collection type otherwise should be a complex type
                if (lastSegment)
                {
                    if (!propertyTypeReference.IsODataPrimitiveTypeKind() && !propertyTypeReference.IsNonEntityCollectionType())
                    {
                        throw new ODataException(ODataErrorStrings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                }
                else
                {
                    if (propertyTypeReference.TypeKind() != EdmTypeKind.Complex)
                    {
                        throw new ODataException(ODataErrorStrings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                    }
                }
            }
            else
            {
                Debug.Assert(
                    structuredType.IsOpen,
                    "Only open types can have undeclared properties, otherwise we should have failed in the ValidatePropertyDefined method.");
            }

            ODataProperty property = cachedProperties == null ? null : cachedProperties.FirstOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new ODataException(ODataErrorStrings.EpmSourceTree_MissingPropertyOnInstance(propertyName, structuredTypeReference.ODataFullName()));
            }

            object propertyValue = property.Value;
            ODataComplexValue propertyComplexValue = propertyValue as ODataComplexValue;
            if (lastSegment)
            {
                if (propertyValue == null)
                {
                    WriterValidationUtils.ValidateNullPropertyValue(propertyTypeReference, propertyName, this.WriterBehavior, this.atomOutputContext.Model);
                }
                else
                {
                    // If this property is the last one it has to be either a primitive or collection
                    if (propertyComplexValue != null)
                    {
                        throw new ODataException(ODataErrorStrings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                    else
                    {
                        ODataCollectionValue propertyCollectionValue = propertyValue as ODataCollectionValue;
                        if (propertyCollectionValue != null)
                        {
                            // Validate the type name for the collection
                            TypeNameOracle.ResolveAndValidateTypeNameForValue(
                                this.atomOutputContext.Model,
                                propertyTypeReference,
                                propertyCollectionValue,
                                /*isOpenProperty*/ edmProperty == null);
                        }
                        else
                        {
                            if (propertyValue is ODataStreamReferenceValue)
                            {
                                // Stream properties should not come here, if it were an ODataEntry property it would have been 
                                // filtered in ReadEntryPropertyValue() by "epmValueCache.EntryProperties" call.
                                throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
                            }
                            else if (propertyValue is ISpatial)
                            {
                                throw new ODataException(ODataErrorStrings.EpmSourceTree_OpenPropertySpatialTypeCannotBeMapped(propertyName, epmInfo.DefiningType.FullName()));
                            }
                            else if (propertyTypeReference != null)
                            {
                                ValidationUtils.ValidateIsExpectedPrimitiveType(propertyValue, propertyTypeReference);
                            }
                        }
                    }
                }

                return propertyValue;
            }

            // Otherwise it's in the middle and thus it must be a complex value
            if (propertyComplexValue == null)
            {
                if (propertyValue != null)
                {
                    // It's not a complex value - fail.
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                }
                else
                {
                    // The value of the property is null, which can be a null complex value
                    // Note that we must not attempt to resolve the type as if the type name was null here, because
                    //  1) We don't need the type for anything anyway (the value is null, this is the end)
                    //  2) If the property is open, trying to resolve a null type name would throw
                    //     but we don't have a null type name, we have a null entire value.
                    return null;
                }
            }

            IEdmComplexTypeReference complexValueType = TypeNameOracle.ResolveAndValidateTypeNameForValue(
                this.atomOutputContext.Model,
                edmProperty == null ? null : edmProperty.Type,
                propertyComplexValue,
                /*isOpenProperty*/ edmProperty == null).AsComplexOrNull();

            return this.ReadComplexPropertyValue(
                epmInfo,
                propertyComplexValue,
                epmValueCache,
                sourceSegmentIndex + 1,
                complexValueType);
        }
    }
}
