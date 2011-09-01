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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Atom;
    #endregion Namespaces

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
        private readonly IEdmEntityType definingType;

        /// <summary>
        /// Type whose property is to be read.
        /// </summary>
        private readonly IEdmEntityType actualPropertyType;

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
        private IEdmTypeReference multiValueItemTypeReference;

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
        internal EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, IEdmEntityType definingType, IEdmEntityType actualTypeDeclaringProperty)
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
                switch (this.SyndicationParent)
                {
                    case EpmSyndicationParent.Category:
                        this.Criteria = EpmSyndicationCriteria.CategoryScheme;
                        break;
                    case EpmSyndicationParent.Link:
                        this.Criteria = EpmSyndicationCriteria.LinkRel;
                        break;
                    default:
                        this.Criteria = EpmSyndicationCriteria.None;
                        break;
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
        /// Entity type that has the <see cref="EntityPropertyMappingAttribute"/>.
        /// </summary>
        internal IEdmEntityType DefiningType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.definingType;
            }
        }

        /// <summary>
        /// Entity type whose property is to be read.
        /// </summary>
        internal IEdmEntityType ActualPropertyType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.actualPropertyType;
            }
        }

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
        internal IEdmTypeReference MultiValueItemTypeReference
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.multiValueItemTypeReference;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.multiValueItemTypeReference == null, "The MultiValue item type can only be set once.");
                this.multiValueItemTypeReference = value;
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
        internal string CriteriaValue
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

            if (criteriaValue != null)
            {
                // Leading and trailing whitespaces in the criteria value are insignificant for the comparison.
                criteriaValue = criteriaValue.Trim();
                if (!EntityPropertyMappingInfo.IllegalLinkRelCriteriaValues.Contains(criteriaValue, StringComparer.OrdinalIgnoreCase) &&
                    !criteriaValue.StartsWith(AtomConstants.ODataNamespace, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
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

            if (criteriaValue != null)
            {
                // Leading and trailing whitespaces in the criteria value are insignificant for the comparison.
                criteriaValue = criteriaValue.TrimStart();
                if (Uri.IsWellFormedUriString(criteriaValue, UriKind.Absolute) &&
                    !criteriaValue.StartsWith(AtomConstants.ODataNamespace, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
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

            return this.DefiningType.IsEquivalentTo(other.DefiningType);
        }
    }
}
