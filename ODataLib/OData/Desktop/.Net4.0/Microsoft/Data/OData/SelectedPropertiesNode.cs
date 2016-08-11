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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Represents a tree of selected properties based on the $select query option.
    /// </summary>
    /// <remarks>
    /// When reading, it controls the template expansion in JSON Light.
    /// </remarks>
    internal sealed class SelectedPropertiesNode
    {
        /// <summary>Singleton which indicates that the nothing is selected.</summary>
        private static readonly SelectedPropertiesNode Empty = new SelectedPropertiesNode(SelectionType.Empty);

        /// <summary>Singleton which indicates that the entire subtree is selected.</summary>
        private static readonly SelectedPropertiesNode EntireSubtree = new SelectedPropertiesNode(SelectionType.EntireSubtree);

        /// <summary>An empty set of stream properties to return when nothing is selected.</summary>
        private static readonly Dictionary<string, IEdmStructuralProperty> EmptyStreamProperties = new Dictionary<string, IEdmStructuralProperty>(StringComparer.Ordinal);

        /// <summary>An empty set of navigation properties to return when nothing is selected.</summary>
        private static readonly IEnumerable<IEdmNavigationProperty> EmptyNavigationProperties = Enumerable.Empty<IEdmNavigationProperty>();

        /// <summary>The type of the current node.</summary>
        private readonly SelectionType selectionType;

        /// <summary>The separator character used to separate property names in a path.</summary>
        private const char PathSeparator = '/';

        /// <summary>The separator character used to separate paths from each other.</summary>
        private const char ItemSeparator = ',';

        /// <summary>The list of selected properties at the current level.</summary>
        private HashSet<string> selectedProperties;

        /// <summary>A dictionary of property name to child nodes.</summary>
        private Dictionary<string, SelectedPropertiesNode> children;

        /// <summary>Indicates that this node had a wildcard selection and all properties at this level should be reported.</summary>
        private bool hasWildcard;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectClause">The string representation of the selected property hierarchy using 
        /// the same format as in the $select query option.</param>
        internal SelectedPropertiesNode(string selectClause)
            : this(SelectionType.PartialSubtree)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(selectClause), "!string.IsNullOrEmpty(selectClause)");

            // NOTE: The select clause is a list of paths separated by comma (',').
            string[] paths = selectClause.Split(ItemSeparator);
            Debug.Assert(paths.Length > 0, "Paths should never be empty");
            foreach (string t in paths)
            {
                string[] segments = t.Split(PathSeparator);
                this.ParsePathSegment(segments, 0);
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SelectedPropertiesNode"/> class from being created.
        /// </summary>
        /// <param name="selectionType">Type of the selection.</param>
        private SelectedPropertiesNode(SelectionType selectionType)
        {
            this.selectionType = selectionType;
        }

        /// <summary>
        /// Enum representing the different special cases of selection.
        /// </summary>
        private enum SelectionType
        {
            /// <summary>
            /// Represents the case where no properties are selected.
            /// </summary>
            Empty = 0,

            /// <summary>
            /// Represents the case where an entire subtree is selected.
            /// </summary>
            EntireSubtree = 1,

            /// <summary>
            /// The normal case where a partial subtree has been selected.
            /// </summary>
            PartialSubtree = 2,
        }

        /// <summary>
        /// Creates a node from the given raw $select query option value.
        /// </summary>
        /// <param name="selectQueryOption">The value of the $select query option.</param>
        /// <returns>A tree representation of the selected properties specified in the query option.</returns>
        internal static SelectedPropertiesNode Create(string selectQueryOption)
        {
            DebugUtils.CheckNoExternalCallers();
            if (selectQueryOption == null)
            {
                return EntireSubtree;
            }

            selectQueryOption = selectQueryOption.Trim();

            // NOTE: an empty select query option is interpreted as not selecting anything.
            if (selectQueryOption.Length == 0)
            {
                return Empty;
            }

            return new SelectedPropertiesNode(selectQueryOption);
        }

        /// <summary>
        /// Recursively combines the left and right nodes. Used when there are type segments present in the select paths which
        /// causes there to be multiple children for the same property/navigation.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        /// <returns>The combined node.</returns>
        internal static SelectedPropertiesNode CombineNodes(SelectedPropertiesNode left, SelectedPropertiesNode right)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            // if either one includes the entire subtree, then so does the result
            if (left.selectionType == SelectionType.EntireSubtree || right.selectionType == SelectionType.EntireSubtree)
            {
                return EntireSubtree;
            }

            // if the left hand side is empty, then use the right hand side
            if (left.selectionType == SelectionType.Empty)
            {
                // even if this is empty too, this all works
                return right;
            }

            // likewise, if the right hand side is empty, use the left
            if (right.selectionType == SelectionType.Empty)
            {
                return left;
            }

            Debug.Assert(left.selectionType == SelectionType.PartialSubtree, "left.selectionType == SelectionType.PartialSubtree");
            Debug.Assert(right.selectionType == SelectionType.PartialSubtree, "right.selectionType == SelectionType.PartialSubtree");

            var combined = new SelectedPropertiesNode(SelectionType.PartialSubtree)
            {
                hasWildcard = left.hasWildcard | right.hasWildcard
            };

            // copy over selected properties, combining as needed
            if (left.selectedProperties != null && right.selectedProperties != null)
            {
                combined.selectedProperties = CreateSelectedPropertiesHashSet(left.selectedProperties.AsEnumerable().Concat(right.selectedProperties));
            }
            else if (left.selectedProperties != null)
            {
                combined.selectedProperties = CreateSelectedPropertiesHashSet(left.selectedProperties);
            }
            else if (right.selectedProperties != null)
            {
                combined.selectedProperties = CreateSelectedPropertiesHashSet(right.selectedProperties);
            }

            // copy over children, combining as needed
            if (left.children != null && right.children != null)
            {
                combined.children = new Dictionary<string, SelectedPropertiesNode>(left.children);
                foreach (var child in right.children)
                {
                    SelectedPropertiesNode fromLeft;
                    if (combined.children.TryGetValue(child.Key, out fromLeft))
                    {
                        combined.children[child.Key] = CombineNodes(fromLeft, child.Value);
                    }
                    else
                    {
                        combined.children[child.Key] = child.Value;
                    }
                }
            }
            else if (left.children != null)
            {
                combined.children = new Dictionary<string, SelectedPropertiesNode>(left.children);
            }
            else if (right.children != null)
            {
                combined.children = new Dictionary<string, SelectedPropertiesNode>(right.children);
            }

            return combined;
        }

        /// <summary>
        /// Gets the selected properties node for the specified navigation property.
        /// </summary>
        /// <param name="entityType">The current entity type.</param>
        /// <param name="navigationPropertyName">The name of the navigation property.</param>
        /// <returns>The selected properties node for the property with name <paramref name="navigationPropertyName"/>.</returns>
        internal SelectedPropertiesNode GetSelectedPropertiesForNavigationProperty(IEdmEntityType entityType, string navigationPropertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.selectionType == SelectionType.Empty)
            {
                return Empty;
            }

            if (this.selectionType == SelectionType.EntireSubtree)
            {
                return EntireSubtree;
            }

            // We cannot determine the selected navigation properties without the user model. This means we won't be computing the missing navigation properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitely told us to write.
            if (entityType == null)
            {
                return EntireSubtree;
            }

            // $select=Orders will include the entire subtree
            if (this.selectedProperties.Contains(navigationPropertyName))
            {
                return EntireSubtree;
            }

            if (this.children != null)
            {
                SelectedPropertiesNode child;
                
                // try to find an immediate child.
                if (!this.children.TryGetValue(navigationPropertyName, out child))
                {
                    child = Empty;
                }

                // try to find a child with a type segment before it that matches the current type.
                // Note: the result of this aggregation will be either empty or a found child node.
                return this.GetMatchingTypeSegments(entityType)
                    .Select(typeSegmentChild => typeSegmentChild.GetSelectedPropertiesForNavigationProperty(entityType, navigationPropertyName))
                    .Aggregate(child, CombineNodes);
            }

            // $select=* will include Orders, but none of its properties
            return Empty;
        }

        /// <summary>
        /// Gets the selected navigation properties for the current node.
        /// </summary>
        /// <param name="entityType">The current entity type.</param>
        /// <returns>The set of selected navigation properties.</returns>
        internal IEnumerable<IEdmNavigationProperty> GetSelectedNavigationProperties(IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.selectionType == SelectionType.Empty)
            {
                return EmptyNavigationProperties;
            }

            // We cannot determine the selected navigation properties without the user model. This means we won't be computing the missing navigation properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitely told us to write.
            if (entityType == null)
            {
                return EmptyNavigationProperties;
            }

            if (this.selectionType == SelectionType.EntireSubtree)
            {
                return EmptyNavigationProperties;
            }

            // Find all the selected navigation properties
            // NOTE: the assumption is that the number of selected properties usually is a lot smaller
            //       than the number of all properties on the type and that FindProperty for each selected
            //       property is faster than iterating through all the properties on the type.
            Debug.Assert(this.selectedProperties != null, "selectedProperties != null");
            IEnumerable<string> navigationPropertyNames = this.selectedProperties;
            if (this.children != null)
            {
                navigationPropertyNames = this.children.Keys.Concat(navigationPropertyNames);
            }

            IEnumerable<IEdmNavigationProperty> selectedNavigationProperties = navigationPropertyNames
                .Select(entityType.FindProperty)
                .OfType<IEdmNavigationProperty>();

            // gather up the selected navigations from any child nodes that have type segments matching the current type and append them.
            foreach (SelectedPropertiesNode typeSegmentChild in this.GetMatchingTypeSegments(entityType))
            {
                selectedNavigationProperties = selectedNavigationProperties.Concat(typeSegmentChild.GetSelectedNavigationProperties(entityType));
            }

            // ensure no duplicates are returned.
            return selectedNavigationProperties.Distinct();
        }

        /// <summary>
        /// Gets the selected stream properties for the current node.
        /// </summary>
        /// <param name="entityType">The current entity type.</param>
        /// <returns>The selected stream properties.</returns>
        internal IDictionary<string, IEdmStructuralProperty> GetSelectedStreamProperties(IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.selectionType == SelectionType.Empty)
            {
                return EmptyStreamProperties;
            }

            // We cannot determine the selected stream properties without the user model. This means we won't be computing the missing stream properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitely told us to write.
            if (entityType == null)
            {
                return EmptyStreamProperties;
            }

            if (this.selectionType == SelectionType.EntireSubtree || this.hasWildcard)
            {
                return entityType.StructuralProperties().Where(sp => sp.Type.IsStream()).ToDictionary(sp => sp.Name, StringComparer.Ordinal);
            }

            Debug.Assert(this.selectedProperties != null, "selectedProperties != null");

            IDictionary<string, IEdmStructuralProperty> selectedStreamProperties = this.selectedProperties
                .Select(entityType.FindProperty)
                .OfType<IEdmStructuralProperty>()
                .Where(p => p.Type.IsStream())
                .ToDictionary(p => p.Name, StringComparer.Ordinal);

            // gather up the selected stream from any child nodes that have type segments matching the current type and add them to the dictionary.
            foreach (SelectedPropertiesNode typeSegmentChild in this.GetMatchingTypeSegments(entityType))
            {
                var streamPropertiesForTypeSegment = typeSegmentChild.GetSelectedStreamProperties(entityType);
                foreach (var kvp in streamPropertiesForTypeSegment)
                {
                    selectedStreamProperties[kvp.Key] = kvp.Value;
                }
            }

            return selectedStreamProperties;
        }

        /// <summary>
        /// Determines whether or not the given operation is selected and takes type-segments into account.
        /// </summary>
        /// <param name="entityType">The current entity type.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="mustBeContainerQualified">Whether or not the operation name must be container qualified in the $select string.</param>
        /// <returns>
        ///   <c>true</c> if the operation is selected; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsOperationSelected(IEdmEntityType entityType, IEdmFunctionImport operation, bool mustBeContainerQualified)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");
            Debug.Assert(operation != null, "operatio != null");
            
            // If the functionImport name conflicts with a property name then it must be container qualified
            mustBeContainerQualified = mustBeContainerQualified || entityType.FindProperty(operation.Name) != null;

            return this.IsOperationSelectedAtThisLevel(operation, mustBeContainerQualified) || this.GetMatchingTypeSegments(entityType).Any(typeSegment => typeSegment.IsOperationSelectedAtThisLevel(operation, mustBeContainerQualified));
        }

        /// <summary>
        /// Gets an enumerable containing the given type and all of its base/ancestor types.
        /// </summary>
        /// <param name="entityType">The starting entity type. Will be included in the returned enumeration.</param>
        /// <returns>An enumerable containing the given type and all of its base/ancestor types.</returns>
        private static IEnumerable<IEdmEntityType> GetBaseTypesAndSelf(IEdmEntityType entityType)
        {
            for (IEdmEntityType currentType = entityType; currentType != null; currentType = currentType.BaseEntityType())
            {
                yield return currentType;
            }
        }

        /// <summary>
        /// Creates a new hash set for storing the names of selected properties.
        /// </summary>
        /// <param name="properties">The initial set of selected properties to store in the hash set.</param>
        /// <returns>The hash set.</returns>
        private static HashSet<string> CreateSelectedPropertiesHashSet(IEnumerable<string> properties)
        {
            HashSet<string> propertiesHashSet = CreateSelectedPropertiesHashSet();
            
            // doing this so that it works on platforms that don't have the constructor parameter.
            foreach (var property in properties)
            {
                propertiesHashSet.Add(property);
            }

            return propertiesHashSet;
        }

        /// <summary>
        /// Creates a new hash set for storing the names of selected properties.
        /// </summary>
        /// <returns>The hash set.</returns>
        private static HashSet<string> CreateSelectedPropertiesHashSet()
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the possible identifiers that could cause the given operation to be selected.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="mustBeContainerQualified">Whether the operations must be container qualified.</param>
        /// <returns>The identifiers to look for in the $select string when determining if this action is selected.</returns>
        private static IEnumerable<string> GetPossibleMatchesForSelectedOperation(IEdmFunctionImport operation, bool mustBeContainerQualified)
        {
            string operationName = operation.Name;
            string operationNameWithParameters = operation.NameWithParameters();

            // if the operation is defined on an open type, it will need to be container qualified, so skip over the unqualified names.
            if (!mustBeContainerQualified)
            {
                // first, try matching just the name. If there are multiple overloads, this intentionally matches all of them.
                yield return operationName;
            }

            // then, try matching the name with parameters. This would refer to a specific overload.
            yield return operationNameWithParameters;

            // then, try matching wildcards and specific names, but qualified with the container name.
            string containerName = operation.Container.Name + ".";
            yield return containerName + ProjectedPropertiesAnnotation.StarSegment;
            yield return containerName + operationName;
            yield return containerName + operationNameWithParameters;

            // last, try matching wildcards and specific names, but qualified with the namespace-qualified container name.
            string qualifiedContainerName = operation.Container.FullName() + ".";
            yield return qualifiedContainerName + ProjectedPropertiesAnnotation.StarSegment;
            yield return qualifiedContainerName + operationName;
            yield return qualifiedContainerName + operationNameWithParameters;
        }

        /// <summary>
        /// Gets the matching type segments for the given type based on this node's children.
        /// </summary>
        /// <param name="entityType">The entity type to match.</param>
        /// <returns>All child nodes which start with a type segment in the given types hierarchy.</returns>
        private IEnumerable<SelectedPropertiesNode> GetMatchingTypeSegments(IEdmEntityType entityType)
        {
            if (this.children != null)
            {
                foreach (IEdmEntityType currentType in GetBaseTypesAndSelf(entityType))
                {
                    SelectedPropertiesNode typeSegmentChild;
                    if (this.children.TryGetValue(currentType.FullName(), out typeSegmentChild))
                    {
                        if (typeSegmentChild.hasWildcard)
                        {
                            throw new ODataException(ODataErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
                        }

                        yield return typeSegmentChild;
                    }
                }
            }
        }

        /// <summary>
        /// Parses the segments of a path in the select clause.
        /// </summary>
        /// <param name="segments">The segments of the select path.</param>
        /// <param name="index">The index of the segment to parse.</param>
        private void ParsePathSegment(string[] segments, int index)
        {
            Debug.Assert(segments != null, "segments != null");
            Debug.Assert(index >= 0 && index < segments.Length, "index >= 0 && index < segments.Length");

            // NOTE: Each path is the name of a property or a series of property names
            //       separated by slash ('/'). The special star ('*') character is only supported at the end of a path.
            string currentSegment = segments[index].Trim();
            if (this.selectedProperties == null)
            {
                this.selectedProperties = CreateSelectedPropertiesHashSet();
            }

            bool isStar = string.CompareOrdinal(ProjectedPropertiesAnnotation.StarSegment, currentSegment) == 0;
            bool isLastSegment = index == segments.Length - 1;
            if (!isLastSegment)
            {
                if (isStar)
                {
                    throw new ODataException(ODataErrorStrings.SelectedPropertiesNode_StarSegmentNotLastSegment);
                }

                SelectedPropertiesNode childNode = this.EnsureChildAnnotation(currentSegment);
                childNode.ParsePathSegment(segments, index + 1);
            }
            else
            {
                this.selectedProperties.Add(currentSegment);
            }

            this.hasWildcard |= isStar;
        }

        /// <summary>
        /// Ensures that a child annotation for the specified segment name already exists; if not creates one.
        /// </summary>
        /// <param name="segmentName">The segment name to get the child annotation for.</param>
        /// <returns>The existing or newly created child annotation for the <paramref name="segmentName"/>.</returns>
        private SelectedPropertiesNode EnsureChildAnnotation(string segmentName)
        {
            Debug.Assert(segmentName != null, "segmentName != null");

            if (this.children == null)
            {
                this.children = new Dictionary<string, SelectedPropertiesNode>(StringComparer.Ordinal);
            }

            SelectedPropertiesNode childNode;
            if (!this.children.TryGetValue(segmentName, out childNode))
            {
                childNode = new SelectedPropertiesNode(SelectionType.PartialSubtree);
                this.children.Add(segmentName, childNode);
            }

            return childNode;
        }

        /// <summary>
        /// Determines whether or not the given operation is selected without taking type segments into account.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="mustBeContainerQualified">Whether the operations must be container qualified.</param>
        /// <returns>
        ///   <c>true</c> if the operation is selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOperationSelectedAtThisLevel(IEdmFunctionImport operation, bool mustBeContainerQualified)
        {
            Debug.Assert(operation != null, "operation != null");

            if (this.selectionType == SelectionType.Empty)
            {
                return false;
            }

            if (this.selectionType == SelectionType.EntireSubtree)
            {
                return true;
            }

            return GetPossibleMatchesForSelectedOperation(operation, mustBeContainerQualified).Any(possibleMatch => this.selectedProperties.Contains(possibleMatch));
        }
    }
}
