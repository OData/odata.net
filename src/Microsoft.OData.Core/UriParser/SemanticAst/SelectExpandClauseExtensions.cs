//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extension methods for <see cref="SelectExpandClause"/>.
    /// </summary>
    internal static class SelectExpandClauseExtensions
    {
        /// <summary>
        /// Get sub select and expand clause by property name, if the propertyname is in form of TypeCast/Property, the typeSegment would also be returned.
        /// </summary>
        /// <param name="clause">The root clause.</param>
        /// <param name="propertyName">The property name to navigate to.</param>
        /// <param name="subSelectExpand">The sub select and expand clause under sub property.</param>
        /// <param name="typeSegment">Type cast segment, if exists.</param>
        internal static void GetSubSelectExpandClause(this SelectExpandClause clause, string propertyName, out SelectExpandClause subSelectExpand, out TypeSegment typeSegment)
        {
            subSelectExpand = null;
            typeSegment = null;

            ExpandedNavigationSelectItem selectedItem = clause
                                                        .SelectedItems
                                                        .OfType<ExpandedNavigationSelectItem>()
                                                        .FirstOrDefault(
                                                            m => m.PathToNavigationProperty.LastSegment != null
                                                                && m.PathToNavigationProperty.LastSegment.TranslateWith(PathSegmentToStringTranslator.Instance) == propertyName);

            if (selectedItem != null)
            {
                subSelectExpand = selectedItem.SelectAndExpand;
                typeSegment = selectedItem.PathToNavigationProperty.FirstSegment as TypeSegment;
            }
        }

        /// <summary>
        /// Gets the select and expand clauses as strings.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetSelectExpandPaths(this SelectExpandClause selectExpandClause, out string selectClause, out string expandClause)
        {
            Debug.Assert(selectExpandClause != null, "selectExpandCluase != null");

            StringBuilder selectClauseBuilder, expandClauseBuilder;
            selectExpandClause.GetSelectExpandPaths(out selectClauseBuilder, out expandClauseBuilder);

            selectClause = selectClauseBuilder.ToString();
            expandClause = expandClauseBuilder.ToString();
        }

        /// <summary>
        /// Gets the select and expand clauses as <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetSelectExpandPaths(this SelectExpandClause selectExpandClause, out StringBuilder selectClause, out StringBuilder expandClause)
        {
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            selectClause = new StringBuilder();
            expandClause = new StringBuilder();
            selectClause.Append(BuildTopLevelSelect(selectExpandClause));
            expandClause.Append(BuildExpandsForNode(selectExpandClause));
        }

        /// <summary>
        /// Gets a list of strings representing current selected property name in current level.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause used.</param>
        /// <returns>String list generated from selected items</returns>
        internal static List<string> GetCurrentLevelSelectList(this SelectExpandClause selectExpandClause)
        {
            return selectExpandClause.SelectedItems.Select(GetSelectString).Where(i => i != null).ToList();
        }

        /// <summary>
        /// Traverse a SelectExpandClause using given functions.
        /// </summary>
        /// <typeparam name="T">Type of the sub processing result for expand items.</typeparam>
        /// <param name="selectExpandClause">The select expand clause for evaluation.</param>
        /// <param name="processSubResult">The method to deal with sub expand result.</param>
        /// <param name="combineSelectAndExpand">The method to combine select and expand result lists.</param>
        /// <param name="result">The result of the traversing.</param>
        internal static void Traverse<T>(this SelectExpandClause selectExpandClause, Func<string, T, T> processSubResult, Func<IList<string>, IList<T>, T> combineSelectAndExpand, out T result)
        {
            List<string> selectList = selectExpandClause.GetCurrentLevelSelectList();
            List<T> expandList = new List<T>();

            foreach (ExpandedNavigationSelectItem expandSelectItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)))
            {
                string currentExpandClause = String.Join("/", expandSelectItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                T subResult = default(T);
                if (expandSelectItem.SelectAndExpand.SelectedItems.Any())
                {
                    Traverse(expandSelectItem.SelectAndExpand, processSubResult, combineSelectAndExpand, out subResult);
                }

                var expandItem = processSubResult(currentExpandClause, subResult);
                if (expandItem != null)
                {
                    expandList.Add(expandItem);
                }
            }

            foreach (ExpandedReferenceSelectItem expandSelectItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)))
            {
                string currentExpandClause = String.Join("/", expandSelectItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                currentExpandClause += "/$ref";

                var expandItem = processSubResult(currentExpandClause, default(T));
                if (expandItem != null)
                {
                    expandList.Add(expandItem);
                }
            }

            result = combineSelectAndExpand(selectList, expandList);
        }

        /// <summary>
        /// Build the top level select clause as a string.
        /// </summary>
        /// <param name="selectExpandClause">top level select expand clause.</param>
        /// <returns>the string representation of the top level select clause.</returns>
        private static string BuildTopLevelSelect(SelectExpandClause selectExpandClause)
        {
            // Special case to build the top level select clause (this it the only time that we actualy
            // modify the selectClause because in V4 the the top level select clause can only modify the top
            // level).
            return String.Join(",", selectExpandClause.GetCurrentLevelSelectList().ToArray());
        }

        /// <summary>
        /// Get the string representation of a select item (that isn't an expandedNavPropSelectItem
        /// </summary>
        /// <param name="selectedItem">the select item to translate</param>
        /// <returns>the string representation of this select item, or null if the select item is an expandedNavPropSelectItem</returns>
        private static string GetSelectString(SelectItem selectedItem)
        {
            WildcardSelectItem wildcardSelect = selectedItem as WildcardSelectItem;
            NamespaceQualifiedWildcardSelectItem namespaceQualifiedWildcard = selectedItem as NamespaceQualifiedWildcardSelectItem;
            PathSelectItem pathSelectItem = selectedItem as PathSelectItem;

            if (wildcardSelect != null)
            {
                return "*";
            }
            else if (namespaceQualifiedWildcard != null)
            {
                return namespaceQualifiedWildcard.Namespace + ".*";
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
        private static string BuildExpandsForNode(SelectExpandClause selectExpandClause)
        {
            List<string> currentLevelExpandClauses = new List<string>();
            foreach (ExpandedNavigationSelectItem expandItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)))
            {
                string currentExpandClause = String.Join("/", expandItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                string expandStr;
                expandItem.SelectAndExpand.Traverse(ProcessSubExpand, CombineSelectAndExpandResult, out expandStr);
                if (!string.IsNullOrEmpty(expandStr))
                {
                    currentExpandClause += "(" + expandStr + ")";
                }

                currentLevelExpandClauses.Add(currentExpandClause);
            }

            foreach (ExpandedReferenceSelectItem expandItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)))
            {
                string currentExpandClause = String.Join("/", expandItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                currentExpandClause += "/$ref";
                currentLevelExpandClauses.Add(currentExpandClause);
            }

            return String.Join(",", currentLevelExpandClauses.ToArray());
        }

        /// <summary>Process sub expand node, contact with subexpad result</summary>
        /// <param name="expandNode">The current expanded node.</param>
        /// <param name="subExpand">Generated sub expand node.</param>
        /// <returns>The generated expand string.</returns>
        private static string ProcessSubExpand(string expandNode, string subExpand)
        {
            return string.IsNullOrEmpty(subExpand) ? expandNode : expandNode + "(" + subExpand + ")";
        }

        /// <summary>Create combined result string using selected items list and expand items list.</summary>
        /// <param name="selectList">A list of selected item names.</param>
        /// <param name="expandList">A list of sub expanded item names.</param>
        /// <returns>The generated expand string.</returns>
        private static string CombineSelectAndExpandResult(IList<string> selectList, IList<string> expandList)
        {
            string currentExpandClause = "";
            if (selectList.Any())
            {
                currentExpandClause += "$select=" + String.Join(",", selectList.ToArray());
            }

            if (expandList.Any())
            {
                if (!string.IsNullOrEmpty(currentExpandClause))
                {
                    currentExpandClause += ";";
                }

                currentExpandClause += "$expand=" + String.Join(",", expandList.ToArray());
            }

            return currentExpandClause;
        }
    }
}