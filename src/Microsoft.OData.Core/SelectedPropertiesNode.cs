//---------------------------------------------------------------------
// <copyright file="SelectedPropertiesNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

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
        internal static readonly SelectedPropertiesNode Empty = new SelectedPropertiesNode(SelectionType.Empty);

        /// <summary>Singleton which indicates that the entire subtree is selected.</summary>
        internal static readonly SelectedPropertiesNode EntireSubtree = new SelectedPropertiesNode(SelectionType.EntireSubtree);

        /// <summary>
        /// Boolean flag indicating whether this is an expanded navigation property.
        /// </summary>
        private readonly bool isExpandedNavigationProperty;

        /// <summary>An empty set of stream properties to return when nothing is selected.</summary>
        private static readonly Dictionary<string, IEdmStructuralProperty> EmptyStreamProperties = new Dictionary<string, IEdmStructuralProperty>(StringComparer.Ordinal);

        /// <summary>An empty set of navigation properties to return when nothing is selected.</summary>
        private static readonly IEnumerable<IEdmNavigationProperty> EmptyNavigationProperties = Enumerable.Empty<IEdmNavigationProperty>();

        /// <summary>The type of the current node.</summary>
        private SelectionType selectionType = SelectionType.PartialSubtree;

        /// <summary>The Edm structured type of the current node.</summary>
        private IEdmStructuredType structuredType;

        /// <summary>The Edm model.</summary>
        private IEdmModel edmModel;

        /// <summary>The separator character used to separate property names in a path.</summary>
        private const char PathSeparator = '/';

        /// <summary>The separator character used to separate paths from each other.</summary>
        private const char ItemSeparator = ',';

        /// <summary>The special '*' segment indicating that all properties are selected.</summary>
        private const string StarSegment = "*";

        /// <summary>The list of selected properties at the current level.</summary>
        private HashSet<string> selectedProperties;

        /// <summary>A dictionary of property name to child nodes.</summary>
        private Dictionary<string, SelectedPropertiesNode> children;

        /// <summary>Indicates that this node had a wildcard selection and all properties at this level should be reported.</summary>
        private bool hasWildcard;

        /// <summary>Current node name, null if root node.</summary>
        private string nodeName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectClause">The string representation of the selected property hierarchy using
        /// the same format as in the $select query option.</param>
        /// <param name="structuredType">The Edm structured type of this node.</param>
        /// <param name="edmModel">The Edm model.</param>
        internal SelectedPropertiesNode(string selectClause, IEdmStructuredType structuredType, IEdmModel edmModel)
            : this(SelectionType.PartialSubtree)
        {
            Debug.Assert(!string.IsNullOrEmpty(selectClause), "!string.IsNullOrEmpty(selectClause)");

            this.structuredType = structuredType;
            this.edmModel = edmModel;

            // NOTE: The select clause is a list of paths and parenthesized expand tokens separated by comma (',').
            // E.g, Accounts('abc')?$expand=User($select(FirstName, LastName)), for the case of projected expanded entity.
            this.Parse(selectClause);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SelectedPropertiesNode"/> class from being created.
        /// </summary>
        /// <param name="selectionType">Type of the selection.</param>
        internal SelectedPropertiesNode(SelectionType selectionType)
            : this(selectionType, /*isExpandedNavigationProperty*/ false)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SelectedPropertiesNode"/> class from being created.
        /// </summary>
        /// <param name="selectionType">Type of the selection.</param>
        /// <param name="isExpandedNavigationProperty">Boolean flag indicating whether this is an expanded navigation property.</param>
        private SelectedPropertiesNode(SelectionType selectionType, bool isExpandedNavigationProperty)
        {
            this.selectionType = selectionType;
            this.isExpandedNavigationProperty = isExpandedNavigationProperty;
        }

        /// <summary>
        /// Enum representing the different special cases of selection.
        /// </summary>
        internal enum SelectionType
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
        /// Creates a node from the given raw $select query option value, structural type information and service model.
        /// </summary>
        /// <param name="selectQueryOption">The value of the $select query option.</param>
        /// <param name="structuredType">The structured type of this node.</param>
        /// <param name="edmModel">The Edm model.</param>
        /// <returns>A tree representation of the selected properties specified in the query option.</returns>
        internal static SelectedPropertiesNode Create(string selectQueryOption, IEdmStructuredType structuredType, IEdmModel edmModel)
        {
            if (selectQueryOption == null)
            {
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
            }

            selectQueryOption = selectQueryOption.Trim();

            // NOTE: an empty select query option is interpreted as not selecting anything.
            if (selectQueryOption.Length == 0)
            {
                return Empty;
            }

            return new SelectedPropertiesNode(selectQueryOption, structuredType, edmModel);
        }

        /// <summary>
        /// Creates a node from the given raw $select query option value without structural type information.
        /// </summary>
        /// <param name="selectQueryOption">The value of the $select query option.</param>
        /// <returns>A tree representation of the selected properties specified in the query option.</returns>
        internal static SelectedPropertiesNode Create(string selectQueryOption)
        {
            return Create(selectQueryOption, /*structuredType*/ null, /*edmModel*/ null);
        }

        /// <summary>
        /// Creates a node from the given SelectExpandClause.
        /// </summary>
        /// <param name="selectExpandClause">The value of the $select query option.</param>
        /// <returns>A tree representation of the selected properties specified in the query option.</returns>
        internal static SelectedPropertiesNode Create(SelectExpandClause selectExpandClause)
        {
            if (selectExpandClause.AllSelected
            && selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().All(_ => _.SelectAndExpand.AllSelected))
            {
                // All items are selected and all expanded entities are all-selected.
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
            }

            return CreateFromSelectExpandClause(selectExpandClause);
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
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            // if either one includes the entire subtree, then so does the result
            if (left.selectionType == SelectionType.EntireSubtree || right.selectionType == SelectionType.EntireSubtree)
            {
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
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
        /// <param name="structuredType">The current structured type.</param>
        /// <param name="navigationPropertyName">The name of the navigation property.</param>
        /// <returns>The selected properties node for the property with name <paramref name="navigationPropertyName"/>.</returns>
        internal SelectedPropertiesNode GetSelectedPropertiesForNavigationProperty(IEdmStructuredType structuredType, string navigationPropertyName)
        {
            if (this.selectionType == SelectionType.Empty)
            {
                return Empty;
            }

            if (this.selectionType == SelectionType.EntireSubtree)
            {
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
            }

            // We cannot determine the selected navigation properties without the user model. This means we won't be computing the missing navigation properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitly told us to write.
            if (structuredType == null)
            {
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
            }

            // $select=Orders will include the entire subtree when there are no same expanded entity.
            if (this.selectedProperties != null && this.selectedProperties.Contains(navigationPropertyName))
            {
                if (this.children == null)
                {
                    return new SelectedPropertiesNode(SelectionType.EntireSubtree);
                }

                bool containsExpandedNavigationProperty = this.children.TryGetValue(navigationPropertyName, out SelectedPropertiesNode child)
                    && child.isExpandedNavigationProperty;
                if (!containsExpandedNavigationProperty)
                {
                    return new SelectedPropertiesNode(SelectionType.EntireSubtree);
                }
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
                return GetSelectePropertiesForTypeSegmentsNavigationProperties(this.GetMatchingTypeSegments(structuredType), structuredType, navigationPropertyName)
                    .Aggregate(child, (left, right) => CombineNodes(left, right));
            }

            // $select=* will include Orders, but none of its properties
            return Empty;
        }

        /// <summary>
        /// Gets the selected navigation properties for the current node.
        /// </summary>
        /// <param name="structuredType">The current structured type.</param>
        /// <returns>The set of selected navigation properties.</returns>
        internal IEnumerable<IEdmNavigationProperty> GetSelectedNavigationProperties(IEdmStructuredType structuredType)
        {
            if (this.selectionType == SelectionType.Empty)
            {
                return EmptyNavigationProperties;
            }

            // We cannot determine the selected navigation properties without the user model. This means we won't be computing the missing navigation properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitly told us to write.
            if (structuredType == null)
            {
                return EmptyNavigationProperties;
            }

            if (this.selectionType == SelectionType.EntireSubtree || this.hasWildcard
                || ((this.selectedProperties == null || this.selectedProperties.Count == 0) && this.children.Values.All(n => n.isExpandedNavigationProperty)))
            {
                return structuredType.NavigationProperties();
            }

            // Find all the selected navigation properties
            // NOTE: the assumption is that the number of selected properties usually is a lot smaller
            //       than the number of all properties on the type and that FindProperty for each selected
            //       property is faster than iterating through all the properties on the type.
            IEnumerable<string> navigationPropertyNames = this.selectedProperties ?? CreateSelectedPropertiesHashSet();
            if (this.children != null)
            {
                navigationPropertyNames = this.children.Keys.Concat(navigationPropertyNames);
            }

            IEnumerable<IEdmNavigationProperty> selectedNavigationProperties = navigationPropertyNames
                .Select(structuredType.FindProperty)
                .OfType<IEdmNavigationProperty>();

            // gather up the selected navigations from any child nodes that have type segments matching the current type and append them.
            foreach (SelectedPropertiesNode typeSegmentChild in this.GetMatchingTypeSegments(structuredType))
            {
                selectedNavigationProperties = selectedNavigationProperties.Concat(typeSegmentChild.GetSelectedNavigationProperties(structuredType));
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
            if (this.selectionType == SelectionType.Empty)
            {
                return EmptyStreamProperties;
            }

            // We cannot determine the selected stream properties without the user model. This means we won't be computing the missing stream properties.
            // For reading we will report what's on the wire and for writing we just write what the user explicitly told us to write.
            if (entityType == null)
            {
                return EmptyStreamProperties;
            }

            if (this.selectionType == SelectionType.EntireSubtree || this.hasWildcard)
            {
                return entityType.StructuralProperties().Where(sp => sp.Type.IsStream()).ToDictionary(sp => sp.Name, StringComparer.Ordinal);
            }

            IDictionary<string, IEdmStructuralProperty> selectedStreamProperties = 
                this.selectedProperties == null ?
                    new Dictionary<string, IEdmStructuralProperty>() :
                    this.selectedProperties
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
        /// <param name="structuredType">The current resource type.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="mustBeNamespaceQualified">Whether or not the operation name must be container qualified in the $select string.</param>
        /// <returns>
        ///   <c>true</c> if the operation is selected; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsOperationSelected(IEdmStructuredType structuredType, IEdmOperation operation, bool mustBeNamespaceQualified)
        {
            Debug.Assert(structuredType != null, "structuredType != null");
            Debug.Assert(operation != null, "operation != null");

            // If the operation name conflicts with a property name then it must be namespace qualified
            mustBeNamespaceQualified = mustBeNamespaceQualified || structuredType.FindProperty(operation.Name) != null;

            return this.IsOperationSelectedAtThisLevel(operation, mustBeNamespaceQualified) || this.GetMatchingTypeSegments(structuredType).Any(typeSegment => typeSegment.IsOperationSelectedAtThisLevel(operation, mustBeNamespaceQualified));
        }

        /// <summary>
        /// Returns whether the selection type of this node is <code>SelectionType.EntireSubtree</code>.
        /// </summary>
        /// <returns><c>true</c> if entire subtree is selected; otherwise <c>false</c>.</returns>
        internal bool IsEntireSubtree()
        {
            return this.selectionType == SelectionType.EntireSubtree;
        }

        private void Parse(string selectClause)
        {
            string[] paths = GetTopLevelItems(selectClause);
            Debug.Assert(paths.Length > 0, "Paths should never be empty");
            foreach (string t in paths)
            {
                string[] segments = null;
                int idxLP = t.IndexOf('(');
                if (-1 == idxLP)
                {
                    segments = t.Split(PathSeparator);
                }
                else
                {
                    // Take the substring before the left parenthesis and get the path segments.
                    segments = t.Substring(0, idxLP).Split(PathSeparator);

                    // Add parenthesized part to the last path segment.
                    segments[segments.Length - 1] += t.Substring(idxLP);
                }

                this.ParsePathSegment(segments, 0);
            }

            DetermineSelectionType();
        }

        private void DetermineSelectionType()
        {
            if (this.children != null)
            {
                foreach (SelectedPropertiesNode childNode in this.children.Values)
                {
                    childNode.DetermineSelectionType();
                }
            }

            if ((this.selectedProperties == null || this.selectedProperties.Count == 0)
                && (this.children == null || this.children.Values.All(n => n.selectionType == SelectionType.EntireSubtree)))
            {
                this.selectionType = SelectionType.EntireSubtree;
            }
        }

        private static string[] GetTopLevelItems(string selectClause)
        {
            List<string> result = new List<string>();
            int parenBalance = 0;
            int startIdx = 0;
            char[] chars = selectClause.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '(':
                        ++parenBalance;
                        break;
                    case ')':
                        --parenBalance;
                        break;
                    case ItemSeparator:
                        if (parenBalance == 0)
                        {
                            string item = selectClause.Substring(startIdx, i - startIdx);
                            if (item.Length != 0)
                            {
                                // Add non-empty item only.
                                result.Add(item);
                            }

                            startIdx = i + 1;
                        }

                        break;
                }
            }

            if (startIdx < chars.Length)
            {
                // Add the last item, which is non-empty.
                result.Add(selectClause.Substring(startIdx, chars.Length - startIdx));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets an enumerable containing the given type and all of its base/ancestor types.
        /// </summary>
        /// <param name="structuredType">The starting resource type. Will be included in the returned enumeration.</param>
        /// <returns>An enumerable containing the given type and all of its base/ancestor types.</returns>
        private static IEnumerable<IEdmStructuredType> GetBaseTypesAndSelf(IEdmStructuredType structuredType)
        {
            for (IEdmStructuredType currentType = structuredType; currentType != null; currentType = currentType.BaseType())
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
        /// <param name="mustBeNamespaceQualified">Whether the operations must be namespace qualified.</param>
        /// <returns>The identifiers to look for in the $select string when determining if this action is selected.</returns>
        private static IEnumerable<string> GetPossibleMatchesForSelectedOperation(IEdmOperation operation, bool mustBeNamespaceQualified)
        {
            string operationName = operation.Name;
            string operationNameWithParameters = operation.NameWithParameters();

            // if the operation is defined on an open type, it will need to be container qualified, so skip over the unqualified names.
            if (!mustBeNamespaceQualified)
            {
                // first, try matching just the name. If there are multiple overloads, this intentionally matches all of them.
                yield return operationName;

                // then, try matching the name with parameters. This would refer to a specific overload.
                yield return operationNameWithParameters;
            }

            // last, try matching wildcards and specific names, but qualified with the namespace-qualified name.
            string qualifiedContainerName = operation.Namespace + ".";
            yield return qualifiedContainerName + StarSegment;
            yield return qualifiedContainerName + operationName;
            yield return qualifiedContainerName + operationNameWithParameters;
        }

        private bool IsValidExpandToken(string item)
        {
            // Expand token must contain '(' and end with ')', must not contain a dot, and must be related to navigation property.
            int idxLP = item.IndexOf('(');
            if (idxLP == -1
                || !item.EndsWith(")", StringComparison.Ordinal)
                || !IsNavigationPropertyToken(item.Substring(0, idxLP)))
            {
                // selected item is not properly parenthesized, or is an operation token.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the specified model and structuredType and see whether the token can be resolved to a navigation property name.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>true if token can be resolved to a navigation property; false otherwise.</returns>
        private bool IsNavigationPropertyToken(string token)
        {
            const char NameSpaceSeparator = '.';

            /* Decision tree:
             #1. if the name contains a dot => it's not a navigation property
             #2. otherwise, if it matches a defined navigation property => treat it as a navigation property
             #3. otherwise, if it matches an unqualified bound operation name => it's not a navigation property
             #4. otherwise, it's a navigation property
             */

            // For better readability, set the value in if-else branches corresponding to decision tree above.
            bool found;
            if (token.IndexOf(NameSpaceSeparator) != -1)
            {
                // #1
                found = false;
            }
            else if (this.structuredType == null || this.structuredType.NavigationProperties().Any(_ => _.Name.Equals(token, StringComparison.Ordinal)))
            {
                // #2
                // Note that action and function names in a contextUrl *SHOULD* always be qualified, 
                // So if we can't validate against the structured type, should assume it's a nav prop
                found = true;
            }
            else if (this.edmModel != null && this.edmModel.FindBoundOperations(this.structuredType).Any(op => op.Name.Equals(token, StringComparison.Ordinal)))
            {
                // #3
                found = false;
            }
            else
            {
                // #4
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Gets the matching type segments for the given type based on this node's children.
        /// </summary>
        /// <param name="structuredType">The structured type to match.</param>
        /// <returns>All child nodes which start with a type segment in the given types hierarchy.</returns>
        private IEnumerable<SelectedPropertiesNode> GetMatchingTypeSegments(IEdmStructuredType structuredType)
        {
            if (this.children != null)
            {
                foreach (IEdmStructuredType currentType in GetBaseTypesAndSelf(structuredType))
                {
                    SelectedPropertiesNode typeSegmentChild;
                    if (this.children.TryGetValue(currentType.FullTypeName(), out typeSegmentChild))
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
        /// Gets the selected properties for each of the type segment child
        /// </summary>
        /// <param name="typeSegments">The children type segments</param>
        /// <param name="structuredType">The parent type</param>
        /// <param name="navigationPropertyName">The navigation property for which to find selected properties</param>
        /// <returns>The selected properties node for each type segment</returns>
        /// <remarks>This method exists solely for performance reasons, to avoid closure allocations when IEnumerable.Select()</remarks>
        private static IEnumerable<SelectedPropertiesNode> GetSelectePropertiesForTypeSegmentsNavigationProperties(IEnumerable<SelectedPropertiesNode> typeSegments, IEdmStructuredType structuredType, string navigationPropertyName)
        {
            foreach (SelectedPropertiesNode typeSegment in typeSegments)
            {
                yield return typeSegment.GetSelectedPropertiesForNavigationProperty(structuredType, navigationPropertyName);
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
            bool isStar = string.CompareOrdinal(StarSegment, currentSegment) == 0;
            int idxLP = currentSegment.IndexOf('(');

            if (idxLP != -1 && IsValidExpandToken(currentSegment))
            {
                string token = currentSegment.Substring(0, idxLP);
                SelectedPropertiesNode childNode = this.EnsureChildNode(token, /* isExpandedNavigationProperty */ true);
                childNode.edmModel = this.edmModel;

                if (idxLP < currentSegment.Length - 2)
                {
                    string clause = currentSegment.Substring(idxLP + 1, currentSegment.Length - idxLP - 2).Trim();
                    if (!String.IsNullOrEmpty(clause))
                    {
                        // Setup the edm model and structured type for the child node before start parsing the select clause.
                        IEdmNavigationProperty navProp = this.structuredType?.DeclaredNavigationProperties()
                            ?.SingleOrDefault(p => p.Name.Equals(token, StringComparison.Ordinal));

                        if (navProp?.Type != null)
                        {
                            // navigation property could be structural type or collection of structural type.
                            childNode.structuredType = navProp.Type.Definition.AsElementType() as IEdmStructuredType;
                        }

                        childNode.Parse(clause);
                    }
                }
                else
                {
                    childNode.selectionType = SelectionType.EntireSubtree;
                }
            }
            else
            {
                bool isLastSegment = index == segments.Length - 1;
                if (!isLastSegment)
                {
                    if (isStar)
                    {
                        throw new ODataException(ODataErrorStrings.SelectedPropertiesNode_StarSegmentNotLastSegment);
                    }

                    SelectedPropertiesNode childNode = this.EnsureChildNode(currentSegment, false);
                    childNode.ParsePathSegment(segments, index + 1);
                }
                else
                {
                    if (this.selectedProperties == null)
                    {
                        this.selectedProperties = CreateSelectedPropertiesHashSet();
                    }

                    this.selectedProperties.Add(currentSegment);
                }
            }

            this.hasWildcard |= isStar;
        }

        /// <summary>
        /// Ensures that a child node for the specified segment name already exists; if not creates one.
        /// </summary>
        /// <param name="segmentName">The segment name to get the child node for.</param>
        /// <param name="isExpandedNavigationProperty">Boolean flag indicating whether this is an expanded navigation property.</param>
        /// <returns>The existing or newly created child node for the <paramref name="segmentName"/>.</returns>
        private SelectedPropertiesNode EnsureChildNode(string segmentName, bool isExpandedNavigationProperty)
        {
            Debug.Assert(segmentName != null, "segmentName != null");

            if (this.children == null)
            {
                this.children = new Dictionary<string, SelectedPropertiesNode>(StringComparer.Ordinal);
            }

            SelectedPropertiesNode childNode;
            if (!this.children.TryGetValue(segmentName, out childNode))
            {
                childNode = new SelectedPropertiesNode(SelectionType.PartialSubtree, isExpandedNavigationProperty);
                this.children.Add(segmentName, childNode);
            }

            return childNode;
        }

        /// <summary>
        /// Determines whether or not the given operation is selected without taking type segments into account.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="mustBeNamespaceQualified">Whether the operations must be container qualified.</param>
        /// <returns>
        ///   <c>true</c> if the operation is selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOperationSelectedAtThisLevel(IEdmOperation operation, bool mustBeNamespaceQualified)
        {
            Debug.Assert(operation != null, "operation != null");

            if (this.selectionType == SelectionType.EntireSubtree)
            {
                return true;
            }

            if (this.selectionType == SelectionType.Empty || this.selectedProperties == null)
            {
                return false;
            }

            return GetPossibleMatchesForSelectedOperation(operation, mustBeNamespaceQualified).Any(possibleMatch => this.selectedProperties.Contains(possibleMatch));
        }

        /// <summary>Create SelectedPropertiesNode from SelectExpandClause.</summary>
        /// <param name="selectExpandClause">The SelectExpandClause representing $select and $expand clauses.</param>
        /// <returns>SelectedPropertiesNode generated using <paramref name="selectExpandClause"/></returns>
        private static SelectedPropertiesNode CreateFromSelectExpandClause(SelectExpandClause selectExpandClause)
        {
            SelectedPropertiesNode node;
            selectExpandClause.Traverse(ProcessSubExpand, CombineSelectAndExpandResult, null, out node);
            return node;
        }

        /// <summary>Process sub expand node, set name for the node.</summary>
        /// <param name="nodeName">Node name for the subexpandnode.</param>
        /// <param name="subExpandNode">Generated sub expand node.</param>
        /// <returns>The sub expanded node passed in.</returns>
        private static SelectedPropertiesNode ProcessSubExpand(string nodeName, SelectedPropertiesNode subExpandNode)
        {
            if (subExpandNode != null)
            {
                subExpandNode.nodeName = nodeName;
            }

            return subExpandNode;
        }

        /// <summary>Create SelectedPropertiesNode using selected name list and expand node list.</summary>
        /// <param name="selectList">An enumerable of selected item names.</param>
        /// <param name="expandList">An enumerable of sub expanded nodes.</param>
        /// <returns>The generated SelectedPropertiesNode.</returns>
        private static SelectedPropertiesNode CombineSelectAndExpandResult(IEnumerable<string> selectList, IEnumerable<SelectedPropertiesNode> expandList)
        {
            HashSet<string> expandSet = new HashSet<string>();
            bool isEntireSubTree = true;

            foreach(SelectedPropertiesNode propNode in expandList)
            {
                expandSet.Add(propNode.nodeName);

                if (!propNode.IsEntireSubtree())
                {
                    isEntireSubTree = false;
                }
            }

            List<string> rawSelect = new List<string>();

            foreach(string name in selectList)
            {
                if (!expandSet.Contains(name))
                {
                    rawSelect.Add(name);
                }
            }
                       
            if (rawSelect.Count == 0 && isEntireSubTree)
            {
                return new SelectedPropertiesNode(SelectionType.EntireSubtree);
            }

            SelectedPropertiesNode node = new SelectedPropertiesNode(SelectionType.PartialSubtree)
            {
                selectedProperties = rawSelect.Count > 0 ? CreateSelectedPropertiesHashSet() : null,
                children = new Dictionary<string, SelectedPropertiesNode>(StringComparer.Ordinal)
            };

            for (int i=0; i < rawSelect.Count; i++)
            {
                if (StarSegment == rawSelect[i])
                {
                    node.hasWildcard = true;
                }
                else
                {
                    node.selectedProperties.Add(rawSelect[i]);
                }
            }

            foreach (var expandItem in expandList)
            {
                node.children[expandItem.nodeName] = expandItem;
            }

            return node;
        }
    }
}
