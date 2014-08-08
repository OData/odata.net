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

#if ASTORIA_SERVER
namespace System.Data.Services.Serializers
#else
namespace System.Data.Services.Client.Serializers
#endif
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
#if ASTORIA_CLIENT
    using System.Data.Services.Client;
#else
    using System.Data.Services;
    using System.Data.Services.Providers;
#endif
    using System.Data.Services.Common;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Tree representing the targetName properties in all the EntityPropertyMappingAttributes for a resource type
    /// </summary>
    internal sealed class EpmTargetTree
    {
        /// <summary>Number of properties that have V2 mapping with KeepInContent false</summary>
        private int countOfNonContentV2mappings;

        /// <summary>Initializes the sub-trees for syndication and non-syndication content</summary>
        internal EpmTargetTree()
        {
            this.SyndicationRoot = new EpmTargetPathSegment();
            this.NonSyndicationRoot = new EpmTargetPathSegment();
        }

        /// <summary>Root of the sub-tree for syndication content</summary>
        internal EpmTargetPathSegment SyndicationRoot
        {
            get; 
            private set;
        }

        /// <summary>Root of the sub-tree for custom content</summary>
        internal EpmTargetPathSegment NonSyndicationRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Minimum DSPV required to serialize this target tree.
        /// </summary>
        internal DataServiceProtocolVersion MinimumDataServiceProtocolVersion
        {
            get 
            {
                if (this.countOfNonContentV2mappings > 0)
                {
                    return DataServiceProtocolVersion.V2;
                }

                return DataServiceProtocolVersion.V1;
            }
        }

        /// <summary>
        /// Adds a path to the tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the target path</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");

            String targetPath = epmInfo.Attribute.TargetPath;
            String namespaceUri = epmInfo.Attribute.TargetNamespaceUri;

            EpmTargetPathSegment currentSegment = epmInfo.IsSyndicationMapping ? this.SyndicationRoot : this.NonSyndicationRoot;
            IList<EpmTargetPathSegment> activeSubSegments = currentSegment.SubSegments;

            Debug.Assert(!String.IsNullOrEmpty(targetPath), "Must have been validated during EntityPropertyMappingAttribute construction");
            String[] targetSegments = targetPath.Split('/');

            EpmTargetPathSegment foundSegment = null;
            for (int i = 0; i < targetSegments.Length; i++)
            {
                String targetSegment = targetSegments[i];

                if (targetSegment.Length == 0)
                {
                    throw new InvalidOperationException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
                }

                if (targetSegment[0] == '@' && i != targetSegments.Length - 1)
                {
                    throw new InvalidOperationException(Strings.EpmTargetTree_AttributeInMiddle(targetSegment));
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
                    currentSegment = new EpmTargetPathSegment(targetSegment, namespaceUri, currentSegment);

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
                throw new ArgumentException(Strings.EpmTargetTree_DuplicateEpmAttrsWithSameTargetName(EpmTargetTree.GetPropertyNameFromEpmInfo(currentSegment.EpmInfo), currentSegment.EpmInfo.DefiningType.Name, currentSegment.EpmInfo.Attribute.SourcePath, epmInfo.Attribute.SourcePath));
            }

            // Increment the number of properties for which KeepInContent is false
            if (!epmInfo.Attribute.KeepInContent)
            {
                this.countOfNonContentV2mappings++;
            }

            currentSegment.EpmInfo = epmInfo;
            
            // Mixed content is dis-allowed. Since root has no ancestor, pass in false for ancestorHasContent
            if (EpmTargetTree.HasMixedContent(this.NonSyndicationRoot, false))
            {
                throw new InvalidOperationException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
            }
        }

        /// <summary>
        /// Removes a path in the tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the target path</param>
        internal void Remove(EntityPropertyMappingInfo epmInfo)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");

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
                    this.countOfNonContentV2mappings--;
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

        /// <summary>Validates the target tree.</summary>
        /// <remarks>This also cleans up the tree if necessary.</remarks>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs access to this in Debug only.")]
        internal void Validate()
        {
#if DEBUG
            DebugValidate(this.SyndicationRoot);
            DebugValidate(this.NonSyndicationRoot);
#endif
        }

#if DEBUG
        /// <summary>Checks the validity of a tree.</summary>
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
        
        /// <summary>Checks if mappings could potentially result in mixed content and dis-allows it.</summary>
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
