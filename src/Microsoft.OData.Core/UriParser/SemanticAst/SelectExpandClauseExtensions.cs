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
    using Microsoft.OData.UriParser.Aggregation;

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
        /// <param name="version">OData version.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetSelectExpandPaths(this SelectExpandClause selectExpandClause, ODataVersion version, out string selectClause, out string expandClause)
        {
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            StringBuilder selectClauseBuilder, expandClauseBuilder;
            selectExpandClause.GetSelectExpandPaths(version, out selectClauseBuilder, out expandClauseBuilder);

            selectClause = selectClauseBuilder.ToString();
            expandClause = expandClauseBuilder.ToString();
        }

        /// <summary>
        /// Gets the select and expand clauses as <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="selectExpandClause">The select expand clause to get the paths from.</param>
        /// <param name="version">OData version.</param>
        /// <param name="selectClause">Returns the select clause.</param>
        /// <param name="expandClause">Returns the expand clause.</param>
        internal static void GetSelectExpandPaths(this SelectExpandClause selectExpandClause, ODataVersion version, out StringBuilder selectClause, out StringBuilder expandClause)
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
            HashSet<string> levelSelectList = new HashSet<string>();
            List<string> masterSelectList = new List<string>();

            foreach (var selectItem in selectExpandClause.SelectedItems)
            {
                if (selectItem is WildcardSelectItem)
                {
                    levelSelectList.Add("*");
                    continue;
                }

                NamespaceQualifiedWildcardSelectItem namespaceQualifiedWildcard = selectItem as NamespaceQualifiedWildcardSelectItem;
                if (namespaceQualifiedWildcard != null)
                {
                    levelSelectList.Add(string.Concat(namespaceQualifiedWildcard.Namespace, ".*"));
                    continue;
                }

                PathSelectItem pathSelectItem = selectItem as PathSelectItem;
                if (pathSelectItem != null)
                {
                    IList<string> pathSelectItems = GetSelectStringFromPathSelectItem(pathSelectItem);

                    for (int i = 0; i < pathSelectItems.Count; i++)
                    {
                        levelSelectList.Add(pathSelectItems[i]);                       
                    }
                }
            }

            // If a select item is a child of a complex property, and that complex property already has all properties selected,
            // then the child is redundant and should not be included in the contextUrl. i.e.; $select=address,address/city => address
            foreach (string item in levelSelectList)
            {
                if (!levelSelectList.Contains(GetPreviousSegments(item)))
                {
                    masterSelectList.Add(item);
                }
            }

            return masterSelectList;
        }

        /// <summary>
        /// Traverse a SelectExpandClause using given functions.
        /// </summary>
        /// <typeparam name="T">Type of the sub processing result for expand items.</typeparam>
        /// <param name="selectExpandClause">The select expand clause for evaluation.</param>
        /// <param name="processSubResult">The method to deal with sub expand result.</param>
        /// <param name="combineSelectAndExpand">The method to combine select and expand result lists.</param>
        /// <param name="processApply">The method to deal with apply result.</param>
        /// <param name="result">The result of the traversing.</param>
        internal static void Traverse<T>(this SelectExpandClause selectExpandClause, Func<string, T, T> processSubResult, Func<IList<string>, IList<T>, T> combineSelectAndExpand, Func<ApplyClause, T> processApply, out T result)
        {
            List<string> selectList = selectExpandClause.GetCurrentLevelSelectList();
            List<T> expandList = new List<T>();

            foreach (SelectItem selectItem in selectExpandClause.SelectedItems)
            {
                // $expand=..../$ref
                ExpandedNavigationSelectItem expandSelectItem = selectItem as ExpandedNavigationSelectItem;

                if (expandSelectItem != null)
                {
                    string currentExpandClause = string.Join("/", expandSelectItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                    T subResult = default(T);
                    if (expandSelectItem.SelectAndExpand.SelectedItems.Any())
                    {
                        Traverse(expandSelectItem.SelectAndExpand, processSubResult, combineSelectAndExpand, processApply, out subResult);
                    }

                    if (expandSelectItem.ApplyOption != null && processApply != null)
                    {
                        subResult = processApply(expandSelectItem.ApplyOption);
                    }

                    var expandItem = processSubResult(currentExpandClause, subResult);
                    if (expandItem != null)
                    {
                        expandList.Add(expandItem);
                    }
                }
                else
                {
                    ExpandedReferenceSelectItem expandRefItem = selectItem as ExpandedReferenceSelectItem;

                    if (expandRefItem != null)
                    {
                        string currentExpandClause = String.Join("/", expandRefItem.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
                        currentExpandClause += "/$ref";

                        var expandItem = processSubResult(currentExpandClause, default(T));
                        if (expandItem != null)
                        {
                            expandList.Add(expandItem);
                        }
                    }
                }
            }

            result = combineSelectAndExpand(selectList, expandList);
        }

        private static string GetPreviousSegments(string item)
        {
            int index = item.LastIndexOf('/');

            if (index > 0)
            {
                return item.Substring(0, index);
            }

            return string.Empty;
        }

        /// <summary>
        /// Build the top level select clause as a string.
        /// </summary>
        /// <param name="selectExpandClause">top level select expand clause.</param>
        /// <returns>the string representation of the top level select clause.</returns>
        private static string BuildTopLevelSelect(SelectExpandClause selectExpandClause)
        {
            // Special case to build the top level select clause (this it the only time that we actually
            // modify the selectClause because in V4 the the top level select clause can only modify the top
            // level).
            return String.Join(",", selectExpandClause.GetCurrentLevelSelectList().ToArray());
        }

        /// <summary>
        /// Get the string representation of a Pathselect item (that isn't an expandedNavPropSelectItem
        /// </summary>
        /// <param name="pathSelectItem">the pathselect item to translate</param>
        /// <returns>the string representation of this select pathselectitem</returns>
        private static IList<string> GetSelectStringFromPathSelectItem(PathSelectItem pathSelectItem)
        {
            IList<string> nextLevelSelectList = null;

            // Recursively call next level to get all the nested select items
            if (pathSelectItem.SelectAndExpand != null)
            {
                nextLevelSelectList = GetCurrentLevelSelectList(pathSelectItem.SelectAndExpand);
            }

            string selectListItem = String.Join("/", pathSelectItem.SelectedPath.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());

            if (nextLevelSelectList != null && nextLevelSelectList.Any())
            {
                List<string> selectList = new List<string>();

                foreach (var listItem in nextLevelSelectList)
                {
                    selectList.Add(string.Concat(selectListItem, "/", listItem));
                }

                return selectList;
            }
            else
            {
                return new List<string>() { selectListItem };
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
                expandItem.SelectAndExpand.Traverse(ProcessSubExpand, CombineSelectAndExpandResult, null, out expandStr);
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

        /// <summary>Process sub expand node, contact with subexpand result</summary>
        /// <param name="expandNode">The current expanded node.</param>
        /// <param name="subExpand">Generated sub expand node.</param>
        /// <returns>The generated expand string.</returns>
        private static string ProcessSubExpand(string expandNode, string subExpand)
        {
            return string.IsNullOrEmpty(subExpand) ? expandNode : string.Concat(expandNode, "(", subExpand, ")");
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