//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if !INTERNAL_DROP || ODATALIB

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// Extension methods for <see cref="SelectExpandClause"/>.
    /// </summary>
    internal static class SelectExpandClauseExtensions
    {
        /// <summary>
        /// Gets the select and expand clauses as strings.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        //// TODO 1466134 Get Rid of these and use only V4
        internal static void GetV3SelectExpandPaths(this SelectExpandClause selectExpandClause, out string selectClause, out string expandClause)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            StringBuilder selectClauseBuilder, expandClauseBuilder;
            selectExpandClause.GetV3SelectExpandPaths(out selectClauseBuilder, out expandClauseBuilder);

            selectClause = selectClauseBuilder.ToString();
            expandClause = expandClauseBuilder.ToString();
        }

        /// <summary>
        /// Gets the select and expand clauses as <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        //// TODO 1466134 Get Rid of these and use only V4
        internal static void GetV3SelectExpandPaths(this SelectExpandClause selectExpandClause, out StringBuilder selectClause, out StringBuilder expandClause)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            selectClause = new StringBuilder();
            expandClause = new StringBuilder();
            BuildSelectAndExpandPathsForNode(/*parentPathSegments*/new List<string>(), selectClause, expandClause, selectExpandClause);
        }

        /// <summary>
        /// Gets the select and expand clauses as strings.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetV4SelectExpandPaths(this SelectExpandClause selectExpandClause, out string selectClause, out string expandClause)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(selectExpandClause != null, "selectExpandCluase != null");

            StringBuilder selectClauseBuilder, expandClauseBuilder;
            selectExpandClause.GetV4SelectExpandPaths(out selectClauseBuilder, out expandClauseBuilder);

            selectClause = selectClauseBuilder.ToString();
            expandClause = expandClauseBuilder.ToString();
        }

        /// <summary>
        /// Gets the select and expand clauses as <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetV4SelectExpandPaths(this SelectExpandClause selectExpandClause, out StringBuilder selectClause, out StringBuilder expandClause)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            selectClause = new StringBuilder();
            expandClause = new StringBuilder();
            selectClause.Append(BuildV4TopLevelSelect(selectExpandClause));
            expandClause.Append(BuildV4ExpandsForNode(selectExpandClause));
        }

        /// <summary>
        /// Build the top level select clause as a string.
        /// </summary>
        /// <param name="selectExpandClause">top level select expand clause.</param>
        /// <returns>the string representation of the top level select clause.</returns>
        private static string BuildV4TopLevelSelect(SelectExpandClause selectExpandClause)
        {
            // Special case to build the top level select clause (this it the only time that we actualy
            // modify the selectClause because in V4 the the top level select clause can only modify the top
            // level).
            List<string> selects = selectExpandClause.SelectedItems.Select(GetSelectString).Where(i => i != null).ToList();
            return String.Join(",", selects.ToArray());
        }

        /// <summary>
        /// Get the string representation of a select item (that isn't an expandedNavPropSelectItem
        /// </summary>
        /// <param name="selectedItem">the select item to translate</param>
        /// <returns>the string representation of this select item, or null if the select item is an expandedNavPropSelectItem</returns>
        private static string GetSelectString(SelectItem selectedItem)
        {
            WildcardSelectItem wildcardSelect = selectedItem as WildcardSelectItem;
            ContainerQualifiedWildcardSelectItem containerQualifiedWildcard = selectedItem as ContainerQualifiedWildcardSelectItem;
            PathSelectItem pathSelectItem = selectedItem as PathSelectItem;
            
            if (wildcardSelect != null)
            {
                return "*";
            }
            else if (containerQualifiedWildcard != null)
            {
                return containerQualifiedWildcard.Container.Name + ".*";
            }
            else if (pathSelectItem != null)
            {
                return String.Join("/", pathSelectItem.SelectedPath.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Build the expand clause for a given level in the selectExpandClause
        /// </summary>
        /// <param name="selectExpandClause">the current level select expand clause</param>
        /// <returns>the expand clause for this level.</returns>
        private static string BuildV4ExpandsForNode(SelectExpandClause selectExpandClause)
        {
            List<string> currentLevelExpandClauses = new List<string>();
            foreach (SelectItem selectedItem in selectExpandClause.SelectedItems)
            {
                ExpandedNavigationSelectItem expandItem = selectedItem as ExpandedNavigationSelectItem;
                if (expandItem != null)
                {
                    string currentExpandClause = String.Join("/", expandItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                    if (expandItem.SelectAndExpand.SelectedItems.Any())
                    {
                        currentExpandClause += "(";
                        List<string> selectsForThisLevel = expandItem.SelectAndExpand.SelectedItems.Select(GetSelectString).Where(i => i != null).ToList();
                        if (selectsForThisLevel.Count > 0)
                        {
                            currentExpandClause += "$select=" + String.Join(",", selectsForThisLevel.ToArray()) + ";";
                        }

                        // then recurse to build sub levels
                        string subExpand = BuildV4ExpandsForNode(expandItem.SelectAndExpand);
                        if (subExpand != "")
                        {
                            currentExpandClause += "$expand=" + subExpand;
                        }

                        currentExpandClause += ")";
                    }

                    currentLevelExpandClauses.Add(currentExpandClause);
                }
            }

            return String.Join(",", currentLevelExpandClauses.ToArray());
        }

        /// <summary>Recursive method which builds the $expand and $select paths for the specified node.</summary>
        /// <param name="parentPathSegments">List of path segments which lead up to this node. 
        /// So for example if the specified node is Orders/OrderDetails the list will contains two strings
        /// "Orders" and "OrderDetails".</param>
        /// <param name="selectClause">The result to which the select paths are appended as a comma separated list.</param>
        /// <param name="expandClause">The result to which the expand paths are appended as a comma separated list.</param>
        /// <param name="selectExpandClause">The select expand clause inspect.</param>
        private static void BuildSelectAndExpandPathsForNode(List<string> parentPathSegments, StringBuilder selectClause, StringBuilder expandClause, SelectExpandClause selectExpandClause)
        {
            foreach (var selectItem in selectExpandClause.SelectedItems)
            {
                if (selectItem is WildcardSelectItem)
                {
                    AppendSelectOrExpandPath(selectClause, parentPathSegments, "*");
                    continue;
                }

                ContainerQualifiedWildcardSelectItem containerQualifiedWildcardSelectItem = selectItem as ContainerQualifiedWildcardSelectItem;
                if (containerQualifiedWildcardSelectItem != null)
                {
                    AppendSelectOrExpandPath(selectClause, parentPathSegments, containerQualifiedWildcardSelectItem.Container.Name + ".*");
                    continue;
                }

                string path;

                var expandItem = selectItem as ExpandedNavigationSelectItem;
                if (expandItem != null)
                {
                    path = String.Join("/", expandItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());

                    // If the next level is AllSelected and the current level is partially selected, add the expanded navigation property to the select clause.
                    if (expandItem.SelectAndExpand.AllSelected && !selectExpandClause.AllSelected)
                    {
                        AppendSelectOrExpandPath(selectClause, parentPathSegments, path);                        
                    }

                    // If the expanded nav prop has no deeper expansions, add the path to $expand.
                    if (!expandItem.SelectAndExpand.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any())
                    {
                        AppendSelectOrExpandPath(expandClause, parentPathSegments, path);
                    }

                    // Build path for nested select and expand clause.
                    parentPathSegments.Add(path);
                    BuildSelectAndExpandPathsForNode(parentPathSegments, selectClause, expandClause, expandItem.SelectAndExpand);
                    parentPathSegments.RemoveAt(parentPathSegments.Count - 1);

                    continue;
                }
                
                PathSelectItem pathSelectionItem = selectItem as PathSelectItem;
                Debug.Assert(pathSelectionItem != null, "Unexpeced selection item type: " + selectItem.GetType());

                path = String.Join("/", pathSelectionItem.SelectedPath.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                AppendSelectOrExpandPath(selectClause, parentPathSegments, path);
            }
        }

        /// <summary>Helper method to append a path to the $expand or $select path list.</summary>
        /// <param name="pathsBuilder">The <see cref="StringBuilder"/> to which to append the path.</param>
        /// <param name="parentPathSegments">The segments of the path up to the last segment.</param>
        /// <param name="lastPathSegment">The last segment of the path.</param>
        private static void AppendSelectOrExpandPath(StringBuilder pathsBuilder, IEnumerable<string> parentPathSegments, string lastPathSegment)
        {
            if (pathsBuilder.Length != 0)
            {
                pathsBuilder.Append(',');
            }

            foreach (string parentPathSegment in parentPathSegments)
            {
                pathsBuilder.Append(parentPathSegment).Append('/');
            }

            pathsBuilder.Append(lastPathSegment);
        }
    }
}

#endif
