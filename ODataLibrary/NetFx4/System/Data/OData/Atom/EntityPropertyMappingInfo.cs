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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// Holds information needed during content serialization/deserialization for
    /// each EntityPropertyMappingAttribute.
    /// </summary>
    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        /// <summary>
        /// List of criteria values we don't allow for category scheme and link rel.
        /// 1. We should not allow atom defined simple identifiers:
        /// RFC 4287 defines:
        /// - alternate
        /// - related
        /// - self
        /// - enclosure
        /// - via
        /// RFC 5023 defines:
        /// - edit
        /// - edit-media
        /// - service
        /// http://www.w3.org/TR/powder-dr/#assoc-linking defines:
        /// - describedby
        /// 
        /// 2. We should not allow the same atom defined values under the IANA namespace. e.g. http://www.iana.org/assignments/relation/edit
        /// </summary>
        private static readonly string[] IllegalLinkRelCriteriaValues = new string[]
        {
            // self
            AtomConstants.AtomSelfRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomSelfRelationAttributeValue,

            // edit
            AtomConstants.AtomEditRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomEditRelationAttributeValue,

            // edit-media
            AtomConstants.AtomEditMediaRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomEditMediaRelationAttributeValue,

            // alternate
            AtomConstants.AtomAlternateRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomAlternateRelationAttributeValue,

            // related
            AtomConstants.AtomRelatedRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomRelatedRelationAttributeValue,

            // enclosure
            AtomConstants.AtomEnclosureRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomEnclosureRelationAttributeValue,

            // via
            AtomConstants.AtomViaRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomViaRelationAttributeValue,

            // describedby
            AtomConstants.AtomDescribedByRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomDescribedByRelationAttributeValue,

            // service
            AtomConstants.AtomServiceRelationAttributeValue,
            AtomConstants.IanaLinkRelationsNamespace + AtomConstants.AtomServiceRelationAttributeValue,
        };

        /// <summary>
        /// Private field backing Attribute property.
        /// </summary>
        private readonly EntityPropertyMappingAttribute attribute;

        /// <summary>
        /// Private field backing DefiningType property.
        /// </summary>
        private readonly ResourceType definingType;

        /// <summary>
        /// Type whose property is to be read.
        /// </summary>
        private readonly ResourceType actualPropertyType;

        /// <summary>
        /// Path to the property value. Stored as an array of source path segments which describe the path from the entry to the property in question.
        /// If this mapping is for a non-multiValue property or for the multiValue property itself, this path starts at the entity resource (not including the root segment).
        /// If this mapping is for a multiValue item property, this path starts at the multiValue item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        private EpmSourcePathSegment[] propertyValuePath;

        /// <summary>
        /// Used to differentiate between normal properties, MultiValue properties and properties of MultiValue items
        /// </summary>
        private EntityPropertyMappingMultiValueStatus multiValueStatus;

        /// <summary>
        /// Type of a MultiValue item. Used only when the multiValueStatus is MultiValue property.
        /// </summary>
        private ResourceType multiValueItemType;

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping
        /// </summary>
        private bool isSyndicationMapping;

        /// <summary>
        /// Field backing the SyndicationParent property.
        /// </summary>
        private EpmSyndicationParent syndicationParent;

        /// <summary>
        /// Field backing the CriteriaValue property.
        /// </summary>
        private string criteriaValue;

        /// <summary>
        /// Field backing the Criteria property.
        /// </summary>
        private EpmSyndicationCriteria criteria;

        /// <summary>
        /// Creates instance of EntityPropertyMappingInfo class.
        /// </summary>
        /// <param name="attribute">The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object</param>
        /// <param name="definingType">Type the <see cref="EntityPropertyMappingAttribute"/> was defined on.</param>
        /// <param name="actualTypeDeclaringProperty">Type whose property is to be read. This can be different from defining type when inheritance is involved.</param>
        internal EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, ResourceType definingType, ResourceType actualTypeDeclaringProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(definingType != null, "definingType != null");
            Debug.Assert(actualTypeDeclaringProperty != null, "actualTypeDeclaringProperty != null");

            this.attribute = attribute;
            this.definingType = definingType;
            this.actualPropertyType = actualTypeDeclaringProperty;
            this.multiValueStatus = EntityPropertyMappingMultiValueStatus.None;

            this.CriteriaValue = attribute.CriteriaValue;

            // Infer the mapping type from the attribute
            this.isSyndicationMapping = this.attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty;

            switch (this.attribute.TargetSyndicationItem)
            {
                case SyndicationItemProperty.AuthorEmail:
                case SyndicationItemProperty.AuthorName:
                case SyndicationItemProperty.AuthorUri:
                    this.SyndicationParent = EpmSyndicationParent.Author;
                    break;
                case SyndicationItemProperty.CategoryLabel:
                case SyndicationItemProperty.CategoryScheme:
                case SyndicationItemProperty.CategoryTerm:
                    this.SyndicationParent = EpmSyndicationParent.Category;
                    break;
                case SyndicationItemProperty.ContributorEmail:
                case SyndicationItemProperty.ContributorName:
                case SyndicationItemProperty.ContributorUri:
                    this.SyndicationParent = EpmSyndicationParent.Contributor;
                    break;
                case SyndicationItemProperty.LinkHref:
                case SyndicationItemProperty.LinkHrefLang:
                case SyndicationItemProperty.LinkLength:
                case SyndicationItemProperty.LinkRel:
                case SyndicationItemProperty.LinkTitle:
                case SyndicationItemProperty.LinkType:
                    this.SyndicationParent = EpmSyndicationParent.Link;
                    break;
                default:
                    this.SyndicationParent = EpmSyndicationParent.Entry;
                    break;
            }

            if (attribute.CriteriaValue != null)
            {
                if (this.SyndicationParent == EpmSyndicationParent.Category)
                {
                    this.Criteria = EpmSyndicationCriteria.CategoryScheme;
                }
                else if (this.SyndicationParent == EpmSyndicationParent.Link)
                {
                    this.Criteria = EpmSyndicationCriteria.LinkRel;
                }
                else 
                {
                    this.Criteria = EpmSyndicationCriteria.None;
                }
            }
        }

        /// <summary>
        /// The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object.
        /// </summary>
        internal EntityPropertyMappingAttribute Attribute
        { 
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.attribute;
            }
        }

        /// <summary>
        /// Type that has the <see cref="EntityPropertyMappingAttribute"/>.
        /// </summary>
        internal ResourceType DefiningType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.definingType;
            }
        }

        /// <summary>
        /// Type whose property is to be read.
        /// </summary>
        internal ResourceType ActualPropertyType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.actualPropertyType;
            }
        }

#if DEBUG
        /// <summary>
        /// Path to the property value. Stored as an array of source path segments which describe the path from the entry to the property in question.
        /// If this mapping is for a non-multiValue property or for the multiValue property itself, this path starts at the entity resource.
        /// If this mapping is for a multiValue item property, this path starts at the multiValue item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        internal EpmSourcePathSegment[] PropertyValuePath
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.propertyValuePath != null, "The propertyValuePath was not initialized yet.");
                return this.propertyValuePath;
            }
        }
#endif

        /// <summary>
        /// Used to differentiate between normal properties, multiValue properties and properties of multiValue items.
        /// </summary>
        internal EntityPropertyMappingMultiValueStatus MultiValueStatus
        {
            get 
            { 
                DebugUtils.CheckNoExternalCallers();
                return this.multiValueStatus; 
            }

            set 
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.multiValueStatus == EntityPropertyMappingMultiValueStatus.None, "The MultiValue status can only be set once.");
                this.multiValueStatus = value; 
            }
        }

        /// <summary>
        /// Parent SyndicationItem.
        /// </summary>
        internal EpmSyndicationParent SyndicationParent
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.syndicationParent;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.syndicationParent = value;
            }
        }

        /// <summary>
        /// Type of a MultiValue item. Used only when the multiValueStatus is MultiValueProperty.
        /// </summary>
        internal ResourceType MultiValueItemType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.multiValueItemType;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.multiValueItemType == null, "The MultiValue item type can only be set once.");
                this.multiValueItemType = value;
            }
        }

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping.
        /// </summary>
        internal bool IsSyndicationMapping
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.isSyndicationMapping;
            }
        }

        /// <summary>
        /// True when the mapping is an attribute of atom:link.
        /// </summary>
        internal bool IsAtomLinkMapping
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.SyndicationParent == EpmSyndicationParent.Link;
            }
        }

        /// <summary>
        /// True when the mapping is an attribute of atom:category.
        /// </summary>
        internal bool IsAtomCategoryMapping
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.SyndicationParent == EpmSyndicationParent.Category;
            }
        }

        /// <summary>
        /// Criteria value for conditional syndication mapping.
        /// </summary>
        internal String CriteriaValue
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.criteriaValue;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.criteriaValue = value;
            }
        }

        /// <summary>
        /// Criteria syndication attribute to use for conditional syndication mapping.
        /// </summary>
        internal EpmSyndicationCriteria Criteria
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.criteria;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.criteria = value;
            }
        }

        /// <summary>
        /// Checks whether given criteriaValue is a valid conditional link rel criteria value.
        /// </summary>
        /// <param name="criteriaValue">criteria value</param>
        /// <returns>true if it is a valid criteria</returns>
        internal static bool IsValidLinkRelCriteriaValue(string criteriaValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (criteriaValue != null &&
                !EntityPropertyMappingInfo.IllegalLinkRelCriteriaValues.Contains(criteriaValue, StringComparer.OrdinalIgnoreCase) &&
                !criteriaValue.StartsWith(AtomConstants.AtomNamespace, StringComparison.OrdinalIgnoreCase) &&
                !criteriaValue.StartsWith(AtomConstants.ODataNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether given criteriaValue is a valid conditional category scheme criteria value
        /// </summary>
        /// <param name="criteriaValue">criteria value</param>
        /// <returns>true if it is a valid criteria</returns>
        internal static bool IsValidCategorySchemeCriteriaValue(string criteriaValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (criteriaValue != null &&
                criteriaValue.Contains(':') &&
                !criteriaValue.StartsWith(AtomConstants.AtomNamespace, StringComparison.OrdinalIgnoreCase) &&
                !criteriaValue.StartsWith(AtomConstants.ODataNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets path to the source property.
        /// </summary>
        /// <param name="path">The path as an array of source path segments.</param>
        internal void SetPropertyValuePath(EpmSourcePathSegment[] path)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(path != null, "path != null");
            Debug.Assert(
                this.multiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty || path.Length > 0,
                "The path must contain at least one segment unless it's a multivalue item path.");
            Debug.Assert(this.propertyValuePath == null, "The property value path was already initialized.");

            this.propertyValuePath = path;
        }

        /// <summary>
        /// Trims the start of the property value path by removing the path to the multivalue property.
        /// </summary>
        /// <param name="multiValueEpmInfo">Epm info for the multivalue property itself..</param>
        internal void TrimMultiValueItemPropertyPath(EntityPropertyMappingInfo multiValueEpmInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(multiValueEpmInfo != null, "multiValueEpmInfo != null");
            Debug.Assert(multiValueEpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty, "The multiValueEpmInfo must be a multiValue property info.");
#if DEBUG
            Debug.Assert(multiValueEpmInfo.propertyValuePath.Length <= this.propertyValuePath.Length, "The prefix is longer than the actual source path.");
            for (int i = 0; i < multiValueEpmInfo.propertyValuePath.Length; i++)
            {
                Debug.Assert(multiValueEpmInfo.propertyValuePath[i] == this.propertyValuePath[i], "The prefix doesn't match the actual source path.");
            }
#endif

            this.propertyValuePath = this.propertyValuePath.Skip(multiValueEpmInfo.propertyValuePath.Length).ToArray();
        }

        /// <summary>
        /// Sets the type of the mapping used for a multiValue property mapping to syndication mapping.
        /// </summary>
        /// <remarks>This method can only be used for multiValue property mapping.</remarks>
        internal void SetMultiValuePropertySyndicationMapping()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty, "This method can only be used for MultiValue property mapping.");
            Debug.Assert(this.attribute.TargetNamespaceUri == AtomConstants.AtomNamespace, "Syndication mapping is only allowed to the ATOM namespace.");
            this.isSyndicationMapping = true;
        }

        /// <summary>Compares the defining type of this info and other EpmInfo object.</summary>
        /// <param name="other">The other EpmInfo object to compare to.</param>
        /// <returns>true if the defining types are the same</returns>
        internal bool DefiningTypesAreEqual(EntityPropertyMappingInfo other)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(other != null, "other != null");

            return this.DefiningType == other.DefiningType;
        }

        /// <summary>
        /// Reads a property value starting on an entry.
        /// </summary>
        /// <param name="epmValueCache">The EPM value cache for the entry to read from.</param>
        /// <param name="resourceType">The resource type of the entry.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        internal object ReadEntryPropertyValue(
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata,
            out bool nullOnParentProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.propertyValuePath != null, "The propertyValuePath should have been initialized by now.");
            Debug.Assert(this.propertyValuePath.Length > 0, "The propertyValuePath must not be empty for an entry property.");
            Debug.Assert(resourceType != null, "resourceType != null");

            // TODO - verify that we actually need the internal property PropertyValuePath
            // TODO - It might be possible to avoid the "value" type checks below if we do property value validation based on the resource type
            return this.ReadPropertyValue(
                epmValueCache.EntryProperties,
                0,
                resourceType,
                metadata,
                epmValueCache,
                out nullOnParentProperty);
        }

        /// <summary>
        /// Reads a property value starting on a complex value.
        /// </summary>
        /// <param name="complexValue">The complex value to start with.</param>
        /// <param name="complexPropertySegment">The EPM source path segment which points to the <paramref name="complexValue"/>.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="resourceType">The resource type of the complex value.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadComplexPropertyValue(
            ODataComplexValue complexValue, 
            EpmSourcePathSegment complexPropertySegment, 
            EpmValueCache epmValueCache, 
            int sourceSegmentIndex, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata,
            out bool nullOnParentProperty)
        {
            Debug.Assert(this.propertyValuePath != null, "The propertyValuePath should have been initialized by now.");
            Debug.Assert(this.propertyValuePath.Length > sourceSegmentIndex, "The propertyValuePath must be at least as long as the source segment index.");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(sourceSegmentIndex >= 0, "sourceSegmentIndex >= 0");
                Debug.Assert(resourceType != null, "resourceType != null");

            if (complexValue == null)
            {
                nullOnParentProperty = true;
                return null;
            }

            return this.ReadPropertyValue(
                EpmValueCache.GetComplexValueProperties(epmValueCache, complexPropertySegment, complexValue, false),
                sourceSegmentIndex,
                resourceType,
                metadata,
                epmValueCache,
                out nullOnParentProperty);
        }

        /// <summary>
        /// Reads a property value starting with the specified index to the property value path.
        /// </summary>
        /// <param name="cachedProperties">The enumeration of properties to search for the first property in the property value path.</param>
        /// <param name="sourceSegmentIndex">The index in the property value path to start with.</param>
        /// <param name="resourceType">The resource type of the entry or complex value the <paramref name="cachedProperties"/> enumeration belongs to.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="epmValueCache">The EPM value cache to use.</param>
        /// <param name="nullOnParentProperty">true if the value of the property is null because one of its parent properties was null, in this case
        /// the return value of the method is always null. false if the value of the property is the actual property value which may or may not be null.</param>
        /// <returns>The value of the property (may be null), or null if the property itself was not found due to one of its parent properties being null.</returns>
        private object ReadPropertyValue(
            IEnumerable<ODataProperty> cachedProperties,
            int sourceSegmentIndex,
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata,
            EpmValueCache epmValueCache,
            out bool nullOnParentProperty)
        {
            Debug.Assert(this.propertyValuePath != null, "The propertyValuePath should have been initialized by now.");
            Debug.Assert(this.propertyValuePath.Length > sourceSegmentIndex, "The propertyValuePath must be at least as long as the source segment index.");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");

            EpmSourcePathSegment sourceSegment = this.propertyValuePath[sourceSegmentIndex];
            string propertyName = sourceSegment.PropertyName;
            bool lastSegment = this.propertyValuePath.Length == sourceSegmentIndex + 1;

            ResourceProperty resourceProperty = ValidationUtils.ValidatePropertyDefined(propertyName, resourceType);
            if (resourceProperty != null)
            {
                // If this is the last part of the path, then it has to be a primitive or multiValue type otherwise should be a complex type
                if (lastSegment)
                {
                    if (!resourceProperty.IsOfKind(ResourcePropertyKind.Primitive) && !resourceProperty.IsOfKind(ResourcePropertyKind.MultiValue))
                    {
                    throw new ODataException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                }
                else
                {
                    if (!resourceProperty.IsOfKind(ResourcePropertyKind.ComplexType))
                    {
                    throw new ODataException(Strings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                    }
                }
            }
            else
            {
                Debug.Assert(
                    resourceType.IsOpenType, 
                    "Only open types can have undeclared properties, otherwise we should have failed in the ValidatePropertyDefined method.");
            }

            ODataProperty property = null;
            if (cachedProperties != null)
            {
                property = cachedProperties.FirstOrDefault(p => p.Name == propertyName);
            }

            if (property == null)
            {
                throw new ODataException(Strings.EpmSourceTree_MissingPropertyOnInstance(propertyName, resourceType.FullName));
            }

            object propertyValue = property.Value;
            ODataComplexValue propertyComplexValue = propertyValue as ODataComplexValue;
            if (lastSegment)
            {
                if (propertyValue != null && resourceProperty != null)
                {
                    ValidationUtils.ValidateIsExpectedPrimitiveType(propertyValue, resourceProperty.ResourceType);
                }

                // If this property is the last one it has to be either a primitive or multivalue
                // TODO: Check for named streams here as well
                if (propertyComplexValue != null)
                {
                    throw new ODataException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                }

                nullOnParentProperty = false;
                return propertyValue;
            }
            else
            {
                // Otherwise it's in the middle and thus it must be a complex value
                if (propertyComplexValue == null)
                {
                    throw new ODataException(Strings.EpmSourceTree_TraversalOfNonComplexType(propertyName));
                }

                string typeName = propertyComplexValue.TypeName;
                ResourceType complexValueType = MetadataUtils.ResolveTypeName(
                    metadata,
                    resourceProperty == null ? null : resourceProperty.ResourceType,
                    ref typeName,
                    ResourceTypeKind.ComplexType,
                    resourceProperty == null);

                return this.ReadComplexPropertyValue(
                    propertyComplexValue,
                    sourceSegment,
                    epmValueCache,
                    sourceSegmentIndex + 1,
                    complexValueType,
                    metadata,
                    out nullOnParentProperty);
            }
        }
    }
}
