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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Tree representing the targetName properties in all the EntityPropertyMappingAttributes for a type.
    /// </summary>
    internal sealed class EpmTargetTree
    {
        /// <summary>
        /// Root of the sub-tree for syndication content.
        /// </summary>
        private readonly EpmTargetPathSegment syndicationRoot;

        /// <summary>
        /// Root of the sub-tree for custom content.
        /// </summary>
        private readonly EpmTargetPathSegment nonSyndicationRoot;

        /// <summary>
        /// Number of properties that have V2 mapping with KeepInContent false.
        /// </summary>
        private int countOfNonContentV2Mappings;

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

                if (this.countOfNonContentV2Mappings > 0)
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

            string targetPath = epmInfo.Attribute.TargetPath;
            string namespaceUri = epmInfo.Attribute.TargetNamespaceUri;
            string namespacePrefix = epmInfo.Attribute.TargetNamespacePrefix;

            EpmTargetPathSegment currentSegment = epmInfo.IsSyndicationMapping ? this.SyndicationRoot : this.NonSyndicationRoot;
            IList<EpmTargetPathSegment> activeSubSegments = currentSegment.SubSegments;

            Debug.Assert(!string.IsNullOrEmpty(targetPath), "Must have been validated during EntityPropertyMappingAttribute construction");
            string[] targetSegments = targetPath.Split('/');

            EpmTargetPathSegment foundSegment = null;
            for (int i = 0; i < targetSegments.Length; i++)
            {
                string targetSegment = targetSegments[i];

                if (targetSegment.Length == 0)
                {
                    throw new ODataException(Strings.EpmTargetTree_InvalidTargetPath_EmptySegment(targetPath));
                }

                if (targetSegment[0] == '@' && i != targetSegments.Length - 1)
                {
                    throw new ODataException(Strings.EpmTargetTree_AttributeInMiddle(targetSegment));
                }

                foundSegment = activeSubSegments.SingleOrDefault(
                    segment => segment.SegmentName == targetSegment &&
                    (epmInfo.IsSyndicationMapping || segment.SegmentNamespaceUri == namespaceUri));

                if (foundSegment != null)
                {
                    currentSegment = foundSegment;
                }
                else
                {
                    currentSegment = new EpmTargetPathSegment(targetSegment, namespaceUri, namespacePrefix, currentSegment);

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

            if (currentSegment.EpmInfo != null)
            {
                // Two EpmAttributes with same TargetName in the inheritance hierarchy
                throw new ODataException(Strings.EpmTargetTree_DuplicateEpmAttributesWithSameTargetName(currentSegment.EpmInfo.DefiningType.ODataFullName(), EpmTargetTree.GetPropertyNameFromEpmInfo(currentSegment.EpmInfo), currentSegment.EpmInfo.Attribute.SourcePath, epmInfo.Attribute.SourcePath));
            }

            // Increment the number of properties for which KeepInContent is false
            if (!epmInfo.Attribute.KeepInContent)
            {
                this.countOfNonContentV2Mappings++;
            }

            currentSegment.EpmInfo = epmInfo;
            
            // Mixed content is dis-allowed.
            List<EntityPropertyMappingAttribute> conflictingAttributes = new List<EntityPropertyMappingAttribute>(2);
            if (EpmTargetTree.HasMixedContent(this.NonSyndicationRoot, conflictingAttributes))
            {
                Debug.Assert(conflictingAttributes.Count == 2, "Expected to find exactly two conflicting attributes.");
                throw new ODataException(Strings.EpmTargetTree_InvalidTargetPath_MixedContent(conflictingAttributes[0].TargetPath, conflictingAttributes[1].TargetPath));
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

            string targetName = epmInfo.Attribute.TargetPath;
            string namespaceUri = epmInfo.Attribute.TargetNamespaceUri;

            EpmTargetPathSegment currentSegment = epmInfo.IsSyndicationMapping ? this.SyndicationRoot : this.NonSyndicationRoot;
            List<EpmTargetPathSegment> activeSubSegments = currentSegment.SubSegments;

            Debug.Assert(!string.IsNullOrEmpty(targetName), "Must have been validated during EntityPropertyMappingAttribute construction");
            string[] targetSegments = targetName.Split('/');
            for (int i = 0; i < targetSegments.Length; i++)
            {
                string targetSegment = targetSegments[i];

                Debug.Assert(targetSegment.Length > 0 && (targetSegment[0] != '@' || i == targetSegments.Length - 1), "Target segments should have been checked when adding the path to the tree");

                EpmTargetPathSegment foundSegment = activeSubSegments.FirstOrDefault(
                                                        segment => segment.SegmentName == targetSegment &&
                                                        (epmInfo.IsSyndicationMapping || segment.SegmentNamespaceUri == namespaceUri));
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
                   this.countOfNonContentV2Mappings--;     
                }

                EpmTargetPathSegment parentSegment = null;
                do
                {
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
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs access to this in Debug only.")]
        internal void Validate()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            DebugValidate(this.SyndicationRoot);
            DebugValidate(this.NonSyndicationRoot);
#endif
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
        /// <param name="currentSegment">StartPath being processed.</param>
        /// <param name="ancestorsWithContent">A list of ancestor attributes that have content. 
        /// Can contain a maximum of one attribute when the method is called, must never contain more than two.</param>
        /// <returns>boolean indicating if the tree is valid or not.</returns>
        private static bool HasMixedContent(EpmTargetPathSegment currentSegment, List<EntityPropertyMappingAttribute> ancestorsWithContent)
        {
            Debug.Assert(ancestorsWithContent != null && ancestorsWithContent.Count < 2, "Expected at most 1 ancestor with content.");

            foreach (EpmTargetPathSegment childSegment in currentSegment.SubSegments.Where(s => !s.IsAttribute))
            {
                if (childSegment.HasContent && ancestorsWithContent.Count == 1)
                {
                    ancestorsWithContent.Add(childSegment.EpmInfo.Attribute);
                    return true;
                }

                if (childSegment.HasContent)
                {
                    Debug.Assert(ancestorsWithContent.Count == 0, "Should have already returned if we had ancestors with content.");
                    ancestorsWithContent.Add(childSegment.EpmInfo.Attribute);
                }

                if (HasMixedContent(childSegment, ancestorsWithContent))
                {
                    Debug.Assert(ancestorsWithContent.Count == 2, "Must have two attributes with content to return true.");
                    return true;
                }

                if (childSegment.HasContent)
                {
                    Debug.Assert(ancestorsWithContent.Count == 1, "Should have the one item in the list that we added before.");
                    ancestorsWithContent.Clear();
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Given an <see cref="EntityPropertyMappingInfo"/> gives the correct target path for it
        /// </summary>
        /// <param name="epmInfo">Given <see cref="EntityPropertyMappingInfo"/></param>
        /// <returns>string with the correct value for the target path</returns>
        private static string GetPropertyNameFromEpmInfo(EntityPropertyMappingInfo epmInfo)
        {
            if (epmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.CustomProperty)
            {
                return epmInfo.Attribute.TargetPath;
            }

            // for EF provider we want to return a name that corresponds to attribute in the edmx file while for CLR provider 
            // and the client we want to return a name that corresponds to the enum value used in EntityPropertyMapping attribute.
            return 
#if ASTORIA_SERVER                
                epmInfo.IsEFProvider ? EpmTranslate.MapSyndicationPropertyToEpmTargetPath(epmInfo.Attribute.TargetSyndicationItem) :
#endif
                epmInfo.Attribute.TargetSyndicationItem.ToString();
        }
    }
}
