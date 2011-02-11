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
    /// Tree representing the sourceName properties in all the EntityPropertyMappingAttributes for a resource type
    /// </summary>
    internal sealed class EpmSourceTree
    {
        #region Fields
        /// <summary>
        /// Primitive string resource type.
        /// </summary>
        private static readonly ResourceType PrimitiveStringResourceType = ResourceType.GetPrimitiveResourceType(typeof(string));

        /// <summary>
        /// Root of the tree.
        /// </summary>
        private readonly EpmSourcePathSegment root;
        
        /// <summary>
        /// <see cref="EpmTargetTree"/> corresponding to this tree.
        /// </summary>
        private readonly EpmTargetTree epmTargetTree;
        #endregion

        /// <summary>
        /// Constructor which creates an empty root.
        /// </summary>
        /// <param name="epmTargetTree">Target xml tree</param>
        internal EpmSourceTree(EpmTargetTree epmTargetTree)
        {
            DebugUtils.CheckNoExternalCallers();

            this.root = new EpmSourcePathSegment();
            this.epmTargetTree = epmTargetTree;
        }

        #region Properties
        /// <summary>
        /// Root of the tree
        /// </summary>
        internal EpmSourcePathSegment Root
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.root;
            }
        }
        #endregion

        /// <summary>
        /// Adds a path to the source and target tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the source path</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            DebugUtils.CheckNoExternalCallers();

            List<EpmSourcePathSegment> pathToCurrentSegment = new List<EpmSourcePathSegment>();
            EpmSourcePathSegment currentSourceSegment = this.Root;
            EpmSourcePathSegment foundSourceSegment = null;
            ResourceType currentType = epmInfo.ActualPropertyType;
            EpmSourcePathSegment multiValuePropertySegment = null;

            Debug.Assert(!string.IsNullOrEmpty(epmInfo.Attribute.SourcePath), "Invalid source path");
            string[] propertyPath = epmInfo.Attribute.SourcePath.Split('/');

            if (epmInfo.CriteriaValue != null)
            {
                ValidateConditionalMapping(epmInfo);
            }

            Debug.Assert(propertyPath.Length > 0, "Must have been validated during EntityPropertyMappingAttribute construction");
            for (int sourcePropertyIndex = 0; sourcePropertyIndex < propertyPath.Length; sourcePropertyIndex++)
            {
                string propertyName = propertyPath[sourcePropertyIndex];

                if (propertyName.Length == 0)
                {
                    throw new ODataException(Strings.EpmSourceTree_InvalidSourcePath(epmInfo.DefiningType.Name, epmInfo.Attribute.SourcePath));
                }

                bool isMultiValueProperty;
                currentType = GetPropertyType(currentType, propertyName, out isMultiValueProperty);

                foundSourceSegment = currentSourceSegment.SubProperties.SingleOrDefault(e => e.PropertyName == propertyName);
                if (foundSourceSegment != null)
                {
                    currentSourceSegment = foundSourceSegment;
                }
                else
                {
                    EpmSourcePathSegment newSourceSegment = new EpmSourcePathSegment(propertyName);
                    currentSourceSegment.SubProperties.Add(newSourceSegment);
                    currentSourceSegment = newSourceSegment;
                }

                pathToCurrentSegment.Add(currentSourceSegment);

                if (isMultiValueProperty)
                {
                    Debug.Assert(
                        currentSourceSegment.EpmInfo == null || currentSourceSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                        "MultiValue property must have EpmInfo marked as MultiValue or none at all.");
                    Debug.Assert(
                        currentSourceSegment.EpmInfo != null || foundSourceSegment == null,
                        "The only way to get a propety without info attached yet on a MultiValue property is when we just created it.");

                    if (multiValuePropertySegment != null)
                    {
                        // Nested MultiValue - not allowed to be mapped
                        throw new ODataException(Strings.EpmSourceTree_NestedMultiValue(
                            multiValuePropertySegment.EpmInfo.Attribute.SourcePath,
                            multiValuePropertySegment.EpmInfo.DefiningType.Name,
                            epmInfo.Attribute.SourcePath));
                    }

                    multiValuePropertySegment = currentSourceSegment;

                    // MultiValue properties can only be mapped to a top-level element, so we can blindly use the first part
                    //   of the target path as the target path for the MultiValue property.
                    Debug.Assert(!string.IsNullOrEmpty(epmInfo.Attribute.TargetPath), "Target path should have been checked by the EpmAttribute constructor.");
                    string multiValuePropertyTargetPath = epmInfo.Attribute.TargetPath.Split('/')[0];

                    if (currentSourceSegment.EpmInfo == null || !currentSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo))
                    {
                        if (currentSourceSegment.EpmInfo != null)
                        {
                            Debug.Assert(!currentSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo), "Just verifying that the ifs are correct.");
                            Debug.Assert(foundSourceSegment != null, "Can't have existing node with EpmInfo on it here and not found it before.");

                            // If the MultiValue property we're trying to add is from a different type than the one we already have
                            //   just overwrite the epm info. This means that the derived type defines a different mapping for the property than the base type.
                            //   We also need to walk all the children of the base type mapping here and remove them from the target tree
                            //   since we're overriding the entire MultiValue property mapping (not just one item property)
                            // Note that for MultiValue properties, removing the MultiValue property node itself will remove all the MultiValue item properties as well
                            //   as they have to be children of the MultiValue property node in the target tree.
                            this.epmTargetTree.Remove(foundSourceSegment.EpmInfo);

                            // We also need to remove all children of the MultiValue property node from the source tree
                            //   as the derived type is overriding it completely. If the derived type doesn't override all of the properties
                            //   we should fail as if it did't map all of them.
                            currentSourceSegment.SubProperties.Clear();
                        }

                        // This is the first time we've seen this MultiValue property mapped
                        //   (on this type, we might have seen it on the base type, but that has been removed)

                        // The source path is the path we have so far for the property
                        string multiValuePropertySourcePath = string.Join("/", propertyPath, 0, sourcePropertyIndex + 1);

                        if (!epmInfo.IsSyndicationMapping)
                        {
                            // Custom EPM for MultiValue is not supported yet
                            // Note: This has already been implemented, but then removed from the code. To see what it takes to implement this
                            //   please see the change which adds this comment into the sources.
                            throw new ODataException(Strings.EpmSourceTree_MultiValueNotAllowedInCustomMapping(
                                multiValuePropertySourcePath,
                                epmInfo.DefiningType.Name));
                        }

                        // Create a new EPM attribute to represent the MultiValue property mapping
                        //   note that this attribute is basically implicitly declared whenever the user declares EPM attribute
                        //   for a property from some MultiValue property. (the declaration happens right here)
                        EntityPropertyMappingAttribute multiValueEpmAttribute = new EntityPropertyMappingAttribute(
                                multiValuePropertySourcePath,
                                multiValuePropertyTargetPath,
                                epmInfo.Attribute.TargetNamespacePrefix,
                                epmInfo.Attribute.TargetNamespaceUri,
                                epmInfo.Attribute.KeepInContent);

                        // Create a special EpmInfo from the above special attribute which represents just the MultiValue property itself
                        EntityPropertyMappingInfo multiValueEpmInfo = new EntityPropertyMappingInfo(
                            multiValueEpmAttribute, 
                            epmInfo.DefiningType, 
                            epmInfo.ActualPropertyType);
                        multiValueEpmInfo.MultiValueStatus = EntityPropertyMappingMultiValueStatus.MultiValueProperty;
                        multiValueEpmInfo.MultiValueItemType = currentType;

                        // We need to mark the info as syndication/custom mapping explicitely since the attribute we create (From which it's infered) is always custom mapping
                        Debug.Assert(epmInfo.IsSyndicationMapping, "Only syndication mapping is allowed for MultiValue properties.");
                        multiValueEpmInfo.SetMultiValuePropertySyndicationMapping();

                        multiValueEpmInfo.SetPropertyValuePath(pathToCurrentSegment.ToArray());

                        multiValueEpmInfo.Criteria = epmInfo.Criteria;
                        multiValueEpmInfo.CriteriaValue = epmInfo.CriteriaValue;

                        // Now associate the current source tree segment with the new info object (or override the one from base)
                        currentSourceSegment.EpmInfo = multiValueEpmInfo;

                        // And add the new info to the target tree
                        this.epmTargetTree.Add(multiValueEpmInfo);

                        // And continue with the walk of the source path.
                        // This means that the EPM attribute specified as the input to this method is still to be added
                        // It might be added to the source tree if the path is longer (property on an item in the MultiValue property of complex types)
                        //   or it might not be added to the source tree if this segment is the last (MultiValue property of primitive types).
                        // In any case it will be added to the target tree (so even if the MultiValue property itself is mapped to the top-level element only
                        //   the items in the MultiValue property can be mapped to child element/attribute of that top-level element).
                    }
                    else
                    {
                        Debug.Assert(currentSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo), "The condition in the surrounding if is broken.");

                        // We have already found a MultiValue property mapped from this source node.
                        // If it's on the same defining type we need to make sure that it's the same MultiValue property being mapped
                        // First verify that the mapping for the other property has the same top-level element for the MultiValue property
                        //   since we only allow properties from one MultiValue property to be mapped to the same top-level element
                        if (multiValuePropertyTargetPath != currentSourceSegment.EpmInfo.Attribute.TargetPath ||
                            epmInfo.Attribute.TargetNamespacePrefix != currentSourceSegment.EpmInfo.Attribute.TargetNamespacePrefix ||
                            epmInfo.Attribute.TargetNamespaceUri != currentSourceSegment.EpmInfo.Attribute.TargetNamespaceUri ||
                            epmInfo.Criteria != currentSourceSegment.EpmInfo.Criteria ||
                            String.Compare(epmInfo.Attribute.CriteriaValue, currentSourceSegment.EpmInfo.CriteriaValue, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw new ODataException(Strings.EpmSourceTree_PropertiesFromSameMultiValueMappedToDifferentTopLevelElements(currentSourceSegment.EpmInfo.Attribute.SourcePath, currentSourceSegment.EpmInfo.DefiningType.Name));
                        }

                        // Second verify that the mappings for both properties have the same KeepInContent value
                        if (epmInfo.Attribute.KeepInContent != currentSourceSegment.EpmInfo.Attribute.KeepInContent)
                        {
                            throw new ODataException(Strings.EpmSourceTree_PropertiesFromSameMultiValueMappedWithDifferentKeepInContent(currentSourceSegment.EpmInfo.Attribute.SourcePath, currentSourceSegment.EpmInfo.DefiningType.Name));
                        }
                    }
                }
            }

            // The last segment is the one being mapped from by the user specified attribute.
            // It must be a primitive type - we don't allow mappings of anything else than primitive properties directly.
            // Note that we can only verify this for non-open properties, for open properties we must assume it's a primitive type
            //   and we will make this check later during serialization again when we actually have the value of the property.
            if (currentType != null)
            {
                if (currentType.ResourceTypeKind != ResourceTypeKind.Primitive)
                {
                    throw new ODataException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(currentSourceSegment.PropertyName));
                }

                SyndicationItemProperty targetSyndicationItem = epmInfo.Attribute.TargetSyndicationItem;
                if (targetSyndicationItem == SyndicationItemProperty.LinkRel || targetSyndicationItem == SyndicationItemProperty.CategoryScheme)
                {
                    if (PrimitiveStringResourceType != currentType)
                    {
                        throw new InvalidOperationException(Strings.EpmSourceTree_NonStringPropertyMappedToConditionAttribute(
                            currentSourceSegment.PropertyName,
                            epmInfo.DefiningType.FullName,
                            targetSyndicationItem.ToString()));
                    }
                }
            }

            if (multiValuePropertySegment == currentSourceSegment)
            {
                // If the MultiValue property is the last segment it means that the MultiValue property itself is being mapped (and it must be a MultiValue of primitive types).

                // If we found the MultiValue property already in the tree, here we actually want the item of the MultiValue property (as the MultiValue one was processed above already)
                // If we have the item value already in the tree use it as the foundProperty so that we correctly check the duplicate mappings below
                if (foundSourceSegment != null)
                {
                    Debug.Assert(foundSourceSegment == currentSourceSegment, "If we found an existing segment it must be the current one.");
                    foundSourceSegment = currentSourceSegment.SubProperties.SingleOrDefault(e => e.IsMultiValueItemValue);
                }

                if (foundSourceSegment == null)
                {
                    // This is a bit of a special case. In the source tree we will create a special node to represent the item value (we need that to be able to tell
                    //   if it was not mapped twice).
                    // In the target tree, we will also create a special node which will hold the information specific
                    //   to serialization of the item value (for example the exact syndication mapping target and so on).
                    //   The creation of the special node is done in the target tree Add method.
                    EpmSourcePathSegment newSegment = EpmSourcePathSegment.CreateMultiValueItemValueSegment();
                    currentSourceSegment.SubProperties.Add(newSegment);
                    currentSourceSegment = newSegment;
                }
                else
                {
                    currentSourceSegment = foundSourceSegment;
                }
            }

            // Note that once we're here the EpmInfo we have is never the MultiValue property itself, it's always either a non-MultiValue property
            //   or MultiValue item property.
            Debug.Assert(foundSourceSegment == null || foundSourceSegment.EpmInfo != null, "Can't have a leaf node in the tree without EpmInfo.");

            // Two EpmAttributes with same PropertyName in the same ResourceType, this could be a result of inheritance
            if (foundSourceSegment != null)
            {
                Debug.Assert(Object.ReferenceEquals(foundSourceSegment, currentSourceSegment), "currentSourceSegment variable should have been updated already to foundSourceSegment");
                Debug.Assert(
                    foundSourceSegment.EpmInfo.MultiValueStatus != EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                    "We should never get here with a MultiValue property itself, we should have a node represent its item or property on the item instead.");

                // Check for duplicates on the same entity type
                Debug.Assert(foundSourceSegment.SubProperties.Count == 0, "If non-leaf, it means we allowed complex type to be a leaf node");
                if (foundSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo))
                {
                    throw new ODataException(Strings.EpmSourceTree_DuplicateEpmAttrsWithSameSourceName(epmInfo.Attribute.SourcePath, epmInfo.DefiningType.Name));
                }

                // In case of inheritance, we need to remove the node from target tree which was mapped to base type property
                this.epmTargetTree.Remove(foundSourceSegment.EpmInfo);
            }

            epmInfo.SetPropertyValuePath(pathToCurrentSegment.ToArray());
            currentSourceSegment.EpmInfo = epmInfo;

            if (multiValuePropertySegment != null)
            {
                Debug.Assert(multiValuePropertySegment.EpmInfo != null, "All MultiValue property segments must have EpmInfo assigned.");

                // We are mapping a MultiValue property - so mark the info as a MultiValue item property (since the MultiValue property itself was added above)
                epmInfo.MultiValueStatus = EntityPropertyMappingMultiValueStatus.MultiValueItemProperty;

                // Set the item type on each of the item properties, so that the segmented path know from which type to start
                epmInfo.MultiValueItemType = multiValuePropertySegment.EpmInfo.MultiValueItemType;

                // And trim its property value path to start from the MultiValue item. This path is basically a list of properties to traverse
                //   when access the value of the property on the specified resource. For non-MultiValue and MultiValue properties themselves 
                //   this path starts with the entity instance. For MultiValue item properties this path starts with the MultiValue item instance.
                //   Note that if it's a MultiValue of primitive types, the path is going to be empty meaning that the value is the item instance itself.
                epmInfo.TrimMultiValueItemPropertyPath(multiValuePropertySegment.EpmInfo);

#if DEBUG
                // Check that if the MultiValue item is of primitive type, we can only ever add a single child source segment which points directly to the MultiValue property itself
                // If we would allow this here, we would fail later, but with a much weirder error message
                Debug.Assert(
                    multiValuePropertySegment.EpmInfo.MultiValueItemType.ResourceTypeKind != ResourceTypeKind.Primitive || epmInfo.PropertyValuePath.Length == 0,
                    "We shoud have failed to map a subproperty of a primitive MultiValue item.");
#endif
            }

            this.epmTargetTree.Add(epmInfo);
        }

        /// <summary>
        /// Validates the source tree.
        /// </summary>
        /// <param name="resourceType">The resource type for which the validation is performed.</param>
        internal void Validate(ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Validate(this.Root, resourceType);
        }

        /// <summary>
        /// Validates the specified segment and all its subsegments.
        /// </summary>
        /// <param name="pathSegment">The path segment to validate.</param>
        /// <param name="resourceType">The resource type of the property represented by this segment (null for open properties).</param>
        private static void Validate(EpmSourcePathSegment pathSegment, ResourceType resourceType)
        {
            Debug.Assert(pathSegment != null, "pathSegment != null");

            foreach (EpmSourcePathSegment subSegment in pathSegment.SubProperties)
            {
                bool isMultiValueProperty;
                ResourceType subSegmentResourceType = GetPropertyType(resourceType, subSegment.PropertyName, out isMultiValueProperty);

                if (isMultiValueProperty)
                {
                    ValidateMultiValueSegment(subSegment.EpmInfo, subSegment, subSegmentResourceType);
                }
                else
                {
                    Validate(subSegment, subSegmentResourceType);
                }
            }
        }

        /// <summary>
        /// Validates the specified segment which is a subsegment of a MultiValue property or the MultiValue property segment itself.
        /// </summary>
        /// <param name="multiValuePropertyInfo">Info about the MultiValue property being processed. Used for exception messages only.</param>
        /// <param name="multiValueSegment">The segment belonging to a MultiValue property to validate.</param>
        /// <param name="resourceType">The resource type of the property represented by this segment (item type for the MultiValue property itself).</param>
        private static void ValidateMultiValueSegment(
            EntityPropertyMappingInfo multiValuePropertyInfo, 
            EpmSourcePathSegment multiValueSegment, 
            ResourceType resourceType)
        {
            Debug.Assert(
                multiValuePropertyInfo != null && multiValuePropertyInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                "multiValuePropertyName must be non-null and must be a MultiValue info.");
            Debug.Assert(multiValueSegment != null, "multiValueSegment != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(
                multiValueSegment.EpmInfo == null ||
                multiValueSegment.EpmInfo == multiValuePropertyInfo ||
                multiValueSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty,
                "The specified segment does not belong to a MultiValue property subtree.");

            if (resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                Debug.Assert(
                    multiValueSegment.EpmInfo == null || multiValueSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                    "EPM source segment representing a complex property of a MultiValue property must not have an EpmInfo for the property itself.");

                // Verify that all properties of the complex type are mapped (have respective source segments)
                foreach (ResourceProperty property in resourceType.Properties)
                {
                    string propertyName = property.Name;
                    string resourceTypeName = resourceType.Name;
                    EpmSourcePathSegment subSegment = multiValueSegment.SubProperties.SingleOrDefault(e => e.PropertyName == propertyName);
                    if (subSegment == null)
                    {
                        throw new ODataException(Strings.EpmSourceTree_NotAllMultiValueItemPropertiesMapped(
                            multiValuePropertyInfo.Attribute.SourcePath,
                            multiValuePropertyInfo.DefiningType.Name,
                            propertyName,
                            resourceTypeName));
                    }
                    else
                    {
                        ResourceType propertyType = property.ResourceType;

                        // Recursive call to verify the sub segment
                        // Note that we don't need to check for enless loops and recursion depth because we are effectively walking the EPM source tree
                        //   which itself can't have loops in it, and can't be infinite either. So if the metadata for a MultiValue property has loops in it
                        //   we would eventually fail to find a matching segment in the source tree and throw.
                        ValidateMultiValueSegment(multiValuePropertyInfo, subSegment, propertyType);
                    }
                }
            }
            else
            {
                Debug.Assert(multiValueSegment.EpmInfo != null, "Primitive value must have EpmInfo.");

                if (multiValueSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty)
                {
                    // MultiValue of primitive types
                    Debug.Assert(multiValueSegment.SubProperties.Count == 1, "Exactly one subproperty should be on a node representing a MultiValue property of primitive types.");
                    Debug.Assert(
                        multiValueSegment.SubProperties[0].IsMultiValueItemValue && multiValueSegment.SubProperties[0].EpmInfo != null &&
                        multiValueSegment.SubProperties[0].EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty,
                        "The only subproperty of a collectin of primitive types should be the special item's value node.");
                    EpmSourcePathSegment leafSegment = multiValueSegment.SubProperties[0];
                    if (leafSegment.EpmInfo.IsAtomLinkMapping)
                    {
                        if (leafSegment.EpmInfo.CriteriaValue == null)
                        {
                            throw new ODataException(Strings.EpmSourceTree_MultiValueOfPrimitiveMappedToLinkWithoutCriteria(
                                multiValuePropertyInfo.Attribute.SourcePath, 
                                multiValuePropertyInfo.DefiningType.Name, 
                                leafSegment.EpmInfo.Attribute.TargetPath));
                        }
                        else if (leafSegment.EpmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.LinkHref)
                        {
                            throw new ODataException(Strings.EpmTargetTree_ConditionalMappingLinkHrefIsRequired(
                                multiValuePropertyInfo.Attribute.SourcePath, 
                                multiValuePropertyInfo.DefiningType.Name, 
                                leafSegment.EpmInfo.Attribute.TargetPath));
                        }
                    }
                    else if (leafSegment.EpmInfo.IsAtomCategoryMapping)
                    {
                        if (leafSegment.EpmInfo.CriteriaValue == null && leafSegment.EpmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CategoryTerm)
                        {
                            throw new ODataException(Strings.EpmTargetTree_ConditionalMappingCategoryTermIsRequired(
                                multiValuePropertyInfo.Attribute.SourcePath, 
                                multiValuePropertyInfo.DefiningType.Name, 
                                leafSegment.EpmInfo.Attribute.TargetPath));
                        }
                    }
                }
                else
                {
                    // MultiValue of complex types, we're on a leaf primitive property.
                    Debug.Assert(
                        multiValueSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty,
                        "Only multiValue property or multiValue item property nodes should get here.");
                    Debug.Assert(multiValueSegment.SubProperties.Count == 0, "Primtive item property value should have no sub properties.");
                    Debug.Assert(!multiValueSegment.IsMultiValueItemValue, "Special MultiValue item value must not be a child of a complex property.");

                    EntityPropertyMappingInfo multiValueSegmentEpmInfo = multiValueSegment.EpmInfo;
                    if (multiValueSegmentEpmInfo.IsAtomLinkMapping != multiValueSegmentEpmInfo.IsAtomLinkMapping ||
                        multiValueSegmentEpmInfo.IsAtomCategoryMapping != multiValueSegmentEpmInfo.IsAtomCategoryMapping ||
                        String.Compare(multiValueSegmentEpmInfo.CriteriaValue, multiValueSegmentEpmInfo.CriteriaValue, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        throw new ODataException(Strings.EpmSourceTree_MultiValueOfComplexTypesDifferentConditionalMapping(
                            multiValuePropertyInfo.Attribute.SourcePath, 
                            multiValuePropertyInfo.DefiningType.Name,
                            multiValuePropertyInfo.Attribute.TargetPath));
                    }
                }
            }
        }

        /// <summary>
        /// Validates conditional mapping.
        /// </summary>
        /// <param name="epmInfo">Epm mapping info</param>
        private static void ValidateConditionalMapping(EntityPropertyMappingInfo epmInfo)
        {
            Debug.Assert(epmInfo.CriteriaValue != null, "epmInfo.CriteriaValue != null");

            String criteriaValue = epmInfo.CriteriaValue;
            if (epmInfo.Criteria == EpmSyndicationCriteria.LinkRel && !EntityPropertyMappingInfo.IsValidLinkRelCriteriaValue(criteriaValue))
            {
                throw new ODataException(Strings.EpmSourceTree_ConditionalMappingInvalidLinkRelCriteriaValue(
                    criteriaValue,
                    epmInfo.Attribute.SourcePath, 
                    epmInfo.DefiningType.Name));
            }

            if (epmInfo.Criteria == EpmSyndicationCriteria.CategoryScheme && !EntityPropertyMappingInfo.IsValidCategorySchemeCriteriaValue(criteriaValue))
            {
                throw new ODataException(Strings.EpmSourceTree_ConditionalMappingInvalidCategorySchemeCriteriaValue(
                    criteriaValue,
                    epmInfo.Attribute.SourcePath, 
                    epmInfo.DefiningType.Name));
            }

            if (epmInfo.IsAtomLinkMapping)
            {
                if (epmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.LinkRel)
                {
                    throw new ODataException(Strings.EpmTargetTree_ConditionalMappingToCriteriaAttribute(
                        epmInfo.Attribute.SourcePath, 
                        epmInfo.DefiningType.Name,
                        epmInfo.Attribute.TargetPath));
                }
            }
            else if (epmInfo.IsAtomCategoryMapping)
            {
                if (epmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.CategoryScheme)
                {
                    throw new ODataException(Strings.EpmTargetTree_ConditionalMappingToCriteriaAttribute(
                        epmInfo.Attribute.SourcePath, 
                        epmInfo.DefiningType.Name,
                        epmInfo.Attribute.TargetPath));
                }
            }
            else
            {
                throw new ODataException(Strings.EpmTargetTree_ConditionalMappingToNonConditionalSyndicationItem(
                    epmInfo.Attribute.SourcePath, 
                    epmInfo.DefiningType.Name,
                    epmInfo.Attribute.TargetPath));
            }
        }

        /// <summary>
        /// Returns a resource type of the property on the specified resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to look for the property on.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <param name="isMultiValueProperty">return true if the property was a MultiValue property.</param>
        /// <returns>The type of the property specified. Note that for MultiValue properties this returns the type of the item of the MultiValue property.</returns>
        private static ResourceType GetPropertyType(ResourceType resourceType, string propertyName, out bool isMultiValueProperty)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            isMultiValueProperty = false;
            ResourceProperty resourceProperty = resourceType != null ? resourceType.TryResolvePropertyName(propertyName) : null;

            if (resourceProperty != null)
            {
                if (resourceProperty.IsOfKind(ResourcePropertyKind.MultiValue))
                {
                    isMultiValueProperty = true;
                    Debug.Assert(resourceProperty.ResourceType is MultiValueResourceType, "MultiValue property must be of the MultiValueResourceType.");
                    MultiValueResourceType multiValueResourceType = (MultiValueResourceType)resourceProperty.ResourceType;
                    return multiValueResourceType.ItemType;
                }
                else
                {
                    return resourceProperty.ResourceType;
                }
            }
            else
            {
                if (resourceType != null && !resourceType.IsOpenType)
                {
                    // could be a named stream
                    resourceProperty = resourceType.TryResolveNamedStream(propertyName);
                    if (resourceProperty != null)
                    {
                        throw new ODataException(Strings.EpmSourceTree_NamedStreamCannotBeMapped(propertyName, resourceType.FullName));
                    }
                    else
                    {
                        throw new ODataException(Strings.EpmSourceTree_MissingPropertyOnType(propertyName, resourceType.FullName));
                    }
                }

                return null;
            }
        }
    }
}
