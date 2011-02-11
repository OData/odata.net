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
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// Tree representing the targetName properties in all the EntityPropertyMappingAttributes for a resource type
    /// </summary>
    internal sealed class EpmTargetTree
    {
        /// <summary>
        /// Number of properties that have V2 mapping with KeepInContent false.
        /// </summary>
        private int countOfNonContentV2mappings;

        /// <summary>
        /// Number of properties that have V3 mapping with KeepInContent false.
        /// </summary>
        private int countOfNonContentV3mappings;

        /// <summary>
        /// List of LinkRel criteria values defined in the tree.
        /// </summary>
        private List<String> linkRelCriteriaValues;

        /// <summary>
        /// List of CategoryScheme criteria values defined in the tree.
        /// </summary>
        private List<String> categorySchemeCriteriaValues;

        /// <summary>
        /// Root of the sub-tree for syndication content.
        /// </summary>
        private EpmTargetPathSegment syndicationRoot;

        /// <summary>
        /// Root of the sub-tree for custom content.
        /// </summary>
        private EpmTargetPathSegment nonSyndicationRoot;

        /// <summary>
        /// Initializes the sub-trees for syndication and non-syndication content.
        /// </summary>
        internal EpmTargetTree()
        {
            DebugUtils.CheckNoExternalCallers();
            this.syndicationRoot = new EpmTargetPathSegment();
            this.nonSyndicationRoot = new EpmTargetPathSegment();
        }

        /// <summary>
        /// Root of the sub-tree for syndication content.
        /// </summary>
        internal EpmTargetPathSegment SyndicationRoot
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.syndicationRoot;
            }
        }

        /// <summary>
        /// Root of the sub-tree for custom content.
        /// </summary>
        internal EpmTargetPathSegment NonSyndicationRoot
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.nonSyndicationRoot;
            }
        }

        /// <summary>
        /// Minimum protocol version required to serialize this target tree.
        /// </summary>
        internal ODataVersion MinimumODataProtocolVersion
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();

                if (this.countOfNonContentV3mappings > 0)
                {
                    return ODataVersion.V3;
                }
                else if (this.countOfNonContentV2mappings > 0)
                {
                    return ODataVersion.V2;
                }

                return ODataVersion.V1;
            }
        }

        /// <summary>
        /// Adds a path to the tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>.
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the target path</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmInfo != null, "epmInfo != null");

            String targetPath = epmInfo.Attribute.TargetPath;
            String namespaceUri = epmInfo.Attribute.TargetNamespaceUri;
            String namespacePrefix = epmInfo.Attribute.TargetNamespacePrefix;

            EpmTargetPathSegment currentSegment = epmInfo.IsSyndicationMapping ? this.SyndicationRoot : this.NonSyndicationRoot;
            IList<EpmTargetPathSegment> activeSubSegments = currentSegment.SubSegments;

            Debug.Assert(!String.IsNullOrEmpty(targetPath), "Must have been validated during EntityPropertyMappingAttribute construction");
            String[] targetSegments = targetPath.Split('/');

            EpmTargetPathSegment foundSegment = null;
            EpmTargetPathSegment multiValueSegment = null;
            for (int i = 0; i < targetSegments.Length; i++)
            {
                String targetSegment = targetSegments[i];

                if (targetSegment.Length == 0)
                {
                    throw new ODataException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
                }

                if (targetSegment[0] == '@' && i != targetSegments.Length - 1)
                {
                    throw new ODataException(Strings.EpmTargetTree_AttributeInMiddle(targetSegment));
                }

                foundSegment = activeSubSegments.SingleOrDefault(
                    segment => segment.SegmentName == targetSegment &&
                    (epmInfo.IsSyndicationMapping || segment.SegmentNamespaceUri == namespaceUri) &&
                    segment.MatchCriteria(epmInfo.CriteriaValue, epmInfo.Criteria));

                if (foundSegment != null)
                {
                    currentSegment = foundSegment;
                    if (currentSegment.EpmInfo != null && currentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty)
                    {
                        multiValueSegment = currentSegment;
                    }
                }
                else
                {
                    currentSegment = new EpmTargetPathSegment(targetSegment, namespaceUri, namespacePrefix, currentSegment);
                    currentSegment.Criteria = epmInfo.Criteria;
                    currentSegment.CriteriaValue = epmInfo.CriteriaValue;

                    if (targetSegment[0] == '@')
                    {
                        activeSubSegments.Insert(0, currentSegment);
                    }
                    else
                    {
                        activeSubSegments.Add(currentSegment);
                    }
                }

                activeSubSegments = currentSegment.SubSegments;
            }

            // If we're adding a multiValue property to already existing segment which maps to a non-multiValue property (no EpmInfo or one pointing to a non-multiValue property)
            // OR if we're adding a non-multiValue property to a segment which has multiValue in its path
            //   we need to fail, since it's invalid to have multiValue property being mapped to the same top-level element as anything else.
            if ((epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty && 
                    foundSegment != null &&
                    (foundSegment.EpmInfo == null || foundSegment.EpmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueProperty)) ||
                (epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.None &&
                    multiValueSegment != null))
            {
                EntityPropertyMappingInfo multiValuePropertyEpmInfo = epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty ?
                    epmInfo : multiValueSegment.EpmInfo;

                // Trying to map MultiValue property to the same target as something else which was already mapped there
                // It is ok to map to atom:category and atom:link elements with different sources only if the criteria values are different.
                if (epmInfo.CriteriaValue != null)
                {
                    throw new ODataException(Strings.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameConditionalTopLevelElement(
                        multiValuePropertyEpmInfo.Attribute.SourcePath,
                        epmInfo.DefiningType.Name,
                        EpmTargetTree.GetPropertyNameFromEpmInfo(multiValuePropertyEpmInfo),
                        epmInfo.CriteriaValue));
                }
                else
                {
                    throw new ODataException(Strings.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameTopLevelElement(
                        multiValuePropertyEpmInfo.Attribute.SourcePath,
                        epmInfo.DefiningType.Name,
                        EpmTargetTree.GetPropertyNameFromEpmInfo(multiValuePropertyEpmInfo)));
                }
            }

            Debug.Assert(
                epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.None || epmInfo.IsSyndicationMapping,
                "Custom EPM mapping is not supported for multiValue properties.");

            // We only allow multiValues to map to ATOM constructs which can be repeated.
            if (epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty)
            {
                Debug.Assert(
                    epmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty, 
                    "Trying to add custom mapped property to a syndication target tree.");

                // Right now all the SyndicationItemProperty targets which do not have SyndicationParent.Entry are valid targets for multiValues
                // and all the ones which have SyndicationParent.Entry (Title, Updated etc.) are not valid targets for multiValues.
                if (epmInfo.SyndicationParent == EpmSyndicationParent.Entry)
                {
                    throw new ODataException(Strings.EpmTargetTree_MultiValueMappedToNonRepeatableAtomElement(
                        epmInfo.Attribute.SourcePath,
                        epmInfo.DefiningType.Name,
                        EpmTargetTree.GetPropertyNameFromEpmInfo(epmInfo)));
                }
            }

            Debug.Assert(
                epmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueProperty || targetSegments.Length == 1,
                "MultiValue property itself can only be mapped to the top-level element.");

            if (currentSegment.EpmInfo != null)
            {
                if (currentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty)
                {
                    Debug.Assert(
                        epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                        "MultiValue property values can't be mapped directly to the top-level element content (no such syndication mapping exists).");

                    // The info we're trying to add is a multiValue property, which would mean two multiValue properties trying to map to the same top-level element.
                    // This can happen if the base type defines mapping for a multiValue property and the derived type defines it again
                    //   in which case we will try to add the derived type mapping again.
                    // So we need to check that these properties are from the same source
                    // It is ok to map to atom:category and atom:link elements with different sources only if the criteria values are different.
                    if (epmInfo.Attribute.SourcePath != currentSegment.EpmInfo.Attribute.SourcePath)
                    {
                        if (epmInfo.CriteriaValue != null)
                        {
                            throw new ODataException(Strings.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameConditionalTopLevelElement(
                                currentSegment.EpmInfo.Attribute.SourcePath,
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.Name,
                                epmInfo.CriteriaValue));
                        }
                        else
                        {
                            throw new ODataException(Strings.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameTopLevelElement(
                                currentSegment.EpmInfo.Attribute.SourcePath,
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.Name));
                        }
                    }

                    Debug.Assert(
                        !foundSegment.EpmInfo.DefiningTypesAreEqual(epmInfo),
                        "Trying to add a multiValue property mapping for the same property on the same type twice. The souce tree should have prevented this from happening.");

                    // If the sources are the same (and the types are different), we can safely overwrite the epmInfo
                    // with the new one (which is for the derived type)
                    // The epm info is stored below.
                }
                else
                {
                    Debug.Assert(
                        epmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                        "Only non-multiValue propeties should get here, we cover the rest above.");

                    // Two EpmAttributes with same TargetName in the inheritance hierarchy
                    throw new ODataException(Strings.EpmTargetTree_DuplicateEpmAttrsWithSameTargetName(EpmTargetTree.GetPropertyNameFromEpmInfo(currentSegment.EpmInfo), currentSegment.EpmInfo.DefiningType.Name, currentSegment.EpmInfo.Attribute.SourcePath, epmInfo.Attribute.SourcePath));
                }
            }

            // Increment the number of properties for which KeepInContent is false
            if (!epmInfo.Attribute.KeepInContent)
            {
                if (epmInfo.IsAtomLinkMapping || epmInfo.IsAtomCategoryMapping)
                {
                    this.countOfNonContentV3mappings++;
                }
                else
                {
                    this.countOfNonContentV2mappings++;
                }
            }

            currentSegment.EpmInfo = epmInfo;
            
            // Mixed content is dis-allowed. Since root has no ancestor, pass in false for ancestorHasContent
            if (EpmTargetTree.HasMixedContent(this.NonSyndicationRoot, false))
            {
                throw new ODataException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
            }
        }

        /// <summary>
        /// Removes a path in the tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>.
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the target path</param>
        internal void Remove(EntityPropertyMappingInfo epmInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmInfo != null, "epmInfo != null");

            // We should never try to remove a multiValue item property from the target tree.
            // If derived type redefines mapping for a given multiValue, the multiValue node itself should be removed first and then all its new items
            //   should be added (but since the multiValue node, that is their parent, was replaces, there should be no collisions and thus no need to remove anything)
            Debug.Assert(epmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueItemProperty, "We should never try to remove a multiValue item property.");

            String targetName = epmInfo.Attribute.TargetPath;
            String namespaceUri = epmInfo.Attribute.TargetNamespaceUri;

            EpmTargetPathSegment currentSegment = epmInfo.IsSyndicationMapping ? this.SyndicationRoot : this.NonSyndicationRoot;
            List<EpmTargetPathSegment> activeSubSegments = currentSegment.SubSegments;

            Debug.Assert(!String.IsNullOrEmpty(targetName), "Must have been validated during EntityPropertyMappingAttribute construction");
            String[] targetSegments = targetName.Split('/');
            for (int i = 0; i < targetSegments.Length; i++)
            {
                String targetSegment = targetSegments[i];

                Debug.Assert(targetSegment.Length > 0 && (targetSegment[0] != '@' || i == targetSegments.Length - 1), "Target segments should have been checked when adding the path to the tree");

                EpmTargetPathSegment foundSegment = activeSubSegments.FirstOrDefault(
                                                        segment => segment.SegmentName == targetSegment &&
                                                        (epmInfo.IsSyndicationMapping || segment.SegmentNamespaceUri == namespaceUri) &&
                                                        segment.MatchCriteria(epmInfo.CriteriaValue, epmInfo.Criteria));
                if (foundSegment != null)
                {
                    currentSegment = foundSegment;
                }
                else
                {
                    return;
                }

                activeSubSegments = currentSegment.SubSegments;
            }

            // Recursively remove all the parent segments which will have no more children left 
            // after removal of the current segment node 
            if (currentSegment.EpmInfo != null)
            {
                // Since we are removing a property with KeepInContent false, we should decrement the count
                if (!currentSegment.EpmInfo.Attribute.KeepInContent)
                {
                    if (currentSegment.EpmInfo.IsAtomLinkMapping || currentSegment.EpmInfo.IsAtomCategoryMapping)
                    {
                        this.countOfNonContentV3mappings--;
                    }
                    else
                    {
                        this.countOfNonContentV2mappings--;
                    }
                }

                EpmTargetPathSegment parentSegment = null;
                do
                {
                    // We should never be removing the multiValue property due to its children being removed
                    Debug.Assert(
                        currentSegment.EpmInfo == null || currentSegment.EpmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueProperty || parentSegment == null,
                        "We should never be removing the multiValue property due to its child being removed. The source tree Add method should remove the multiValue property node itself first in that case.");

                    parentSegment = currentSegment.ParentSegment;
                    parentSegment.SubSegments.Remove(currentSegment);
                    currentSegment = parentSegment;
                }
                while (currentSegment.ParentSegment != null && !currentSegment.HasContent && currentSegment.SubSegments.Count == 0);
            }
        }

        /// <summary>
        /// Validates the target tree.
        /// </summary>
        /// <remarks>This also cleans up the tree if necessary.</remarks>
        internal void Validate()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            DebugValidate(this.SyndicationRoot);
            DebugValidate(this.NonSyndicationRoot);
#endif
            this.ValidateConditionalMappings();
        }

        /// <summary>
        /// Used to look up if there is a matching criteriaValue for unconditional atom:link/@rel values.
        /// </summary>
        /// <param name="criteriaValue">criteria value</param>
        /// <returns>true if there is a atom:link element with the same criteriaValue in the tree</returns>
        internal bool HasLinkRelCriteriaValue(String criteriaValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.linkRelCriteriaValues == null)
            {
                this.linkRelCriteriaValues = this.SyndicationRoot.SubSegments.Where(s => s.CriteriaValue != null && s.Criteria == EpmSyndicationCriteria.LinkRel)
                                                        .Select(s => s.CriteriaValue).ToList();
            }

            return this.linkRelCriteriaValues.Any(s => s.Equals(criteriaValue, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Used to look up if there is a matching criteriaValue for unconditional atom:category/@scheme values.
        /// </summary>
        /// <param name="criteriaValue">criteria value</param>
        /// <returns>true if there is a atom:category element with the same criteriaValue in the tree</returns>
        internal bool HasCategorySchemeCriteriaValue(String criteriaValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.categorySchemeCriteriaValues == null)
            {
                this.categorySchemeCriteriaValues = this.SyndicationRoot.SubSegments.Where(s => s.CriteriaValue != null && s.Criteria == EpmSyndicationCriteria.CategoryScheme)
                                                        .Select(s => s.CriteriaValue).ToList();
            }

            return this.categorySchemeCriteriaValues.Any(s => s.Equals(criteriaValue, StringComparison.OrdinalIgnoreCase));
        }

#if DEBUG
        /// <summary>
        /// Checks the validity of a tree.
        /// </summary>
        /// <param name="currentSegment">The segment to validate.</param>
        private static void DebugValidate(EpmTargetPathSegment currentSegment)
        {
            if (currentSegment.ParentSegment != null)
            {
                Debug.Assert(!currentSegment.ParentSegment.IsAttribute, "Attributes must be leaf nodes.");
                if (currentSegment.EpmInfo != null)
                {
                    switch (currentSegment.EpmInfo.MultiValueStatus)
                    {
                        case EntityPropertyMappingMultiValueStatus.None:
                            Debug.Assert(
                                currentSegment.ParentSegment.EpmInfo == null ||
                                currentSegment.ParentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.None,
                                "non-multiValue property can only be a child of another non-multiValue property.");
                            break;
                        case EntityPropertyMappingMultiValueStatus.MultiValueProperty:
                            Debug.Assert(
                                currentSegment.ParentSegment.EpmInfo == null ||
                                (currentSegment.ParentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.None && currentSegment.ParentSegment.ParentSegment == null),
                                "MultiValue must be child of the root only.");
                            break;
                        case EntityPropertyMappingMultiValueStatus.MultiValueItemProperty:
                            Debug.Assert(
                                currentSegment.ParentSegment.EpmInfo == null ||
                                (currentSegment.ParentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty ||
                                 currentSegment.ParentSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty),
                                "MultiValue item must be child of multiValue or another multiValue item only.");
                            break;
                    }
                }
            }

            // Walk recursively subsegments and validate them
            foreach (EpmTargetPathSegment subSegment in currentSegment.SubSegments)
            {
                DebugValidate(subSegment);
            }
        }
#endif
        
        /// <summary>
        /// Checks if mappings could potentially result in mixed content and dis-allows it.
        /// </summary>
        /// <param name="currentSegment">Segment being processed.</param>
        /// <param name="ancestorHasContent">Does any of the ancestors have content.</param>
        /// <returns>boolean indicating if the tree is valid or not.</returns>
        private static bool HasMixedContent(EpmTargetPathSegment currentSegment, bool ancestorHasContent)
        {
            foreach (EpmTargetPathSegment childSegment in currentSegment.SubSegments.Where(s => !s.IsAttribute))
            {
                if (childSegment.HasContent && ancestorHasContent)
                {
                    return true;
                }

                if (HasMixedContent(childSegment, childSegment.HasContent || ancestorHasContent))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Given an <see cref="EntityPropertyMappingInfo"/> gives the correct target path for it
        /// </summary>
        /// <param name="epmInfo">Given <see cref="EntityPropertyMappingInfo"/></param>
        /// <returns>String with the correct value for the target path</returns>
        private static String GetPropertyNameFromEpmInfo(EntityPropertyMappingInfo epmInfo)
        {
            if (epmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.CustomProperty)
            {
                return epmInfo.Attribute.TargetPath;
            }
            else
            {
                // for EF provider we want to return a name that corresponds to attribute in the edmx file while for CLR provider 
                // and the client we want to return a name that corresponds to the enum value used in EntityPropertyMapping attribute.
                return 
#if ASTORIA_SERVER                
                epmInfo.IsEFProvider ? EpmTranslate.MapSyndicationPropertyToEpmTargetPath(epmInfo.Attribute.TargetSyndicationItem) :
#endif
                epmInfo.Attribute.TargetSyndicationItem.ToString();
            }
        }

        /// <summary>
        /// Validate conditional mapping in target tree
        /// </summary>
        private void ValidateConditionalMappings()
        {
            foreach (EpmTargetPathSegment segment in this.SyndicationRoot.SubSegments)
            {
                if (segment.SegmentName == AtomConstants.AtomCategoryElementName)
                {
                    if (!segment.SubSegments.Any(s => s.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.CategoryTerm))
                    {
                        throw new ODataException(Strings.EpmTargetTree_ConditionalMappingCategoryTermIsRequired(
                            segment.SubSegments[0].EpmInfo.Attribute.SourcePath,
                            segment.SubSegments[0].EpmInfo.DefiningType.Name,
                            segment.SubSegments[0].EpmInfo.Attribute.TargetPath));
                    }
                }
                else if (segment.SegmentName == AtomConstants.AtomLinkElementName)
                {
                    if (!segment.SubSegments.Any(s => s.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.LinkHref))
                    {
                        throw new ODataException(Strings.EpmTargetTree_ConditionalMappingLinkHrefIsRequired(
                            segment.SubSegments[0].EpmInfo.Attribute.SourcePath,
                            segment.SubSegments[0].EpmInfo.DefiningType.Name,
                            segment.SubSegments[0].EpmInfo.Attribute.TargetPath));
                    }

                    if (segment.CriteriaValue == null)
                    {
                        if (!segment.SubSegments.Any(s => s.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.LinkRel))
                        {
                            throw new ODataException(Strings.EpmTargetTree_ConditionalMappingRelIsRequiredForNonConditional(
                                segment.SubSegments[0].EpmInfo.Attribute.SourcePath,
                                segment.SubSegments[0].EpmInfo.DefiningType.Name,
                                segment.SubSegments[0].EpmInfo.Attribute.TargetPath));
                        }
                    }
                }
            }
        }
    }
}
