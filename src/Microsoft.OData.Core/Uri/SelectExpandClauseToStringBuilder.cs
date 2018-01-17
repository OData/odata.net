//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseToStringBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Build SelectExpandQueryNode to String Representation
    /// </summary>
    internal sealed class SelectExpandClauseToStringBuilder : SelectItemTranslator<string>
    {
        /// <summary>
        /// the flag used to mark the SelectItem first appeared
        /// </summary>
        private bool isFirstSelectItem = true;

        /// <summary>
        /// Build the expand clause for a given level in the selectExpandClause
        /// </summary>
        /// <param name="selectExpandClause">the current level select expand clause</param>
        /// <param name="firstFlag">whether is inner SelectExpandClause</param>
        /// <returns>the select and expand segment for context url in this level.</returns>
        public string TranslateSelectExpandClause(SelectExpandClause selectExpandClause, bool firstFlag)
        {
            ExceptionUtils.CheckArgumentNotNull(selectExpandClause, "selectExpandClause");

            List<string> selectList = selectExpandClause.GetCurrentLevelSelectList();
            string selectClause = null;
            if (selectList.Any())
            {
                selectClause = String.Join(ODataConstants.ContextUriProjectionPropertySeparator, selectList.ToArray());
            }

            selectClause = string.IsNullOrEmpty(selectClause) ? null : string.Concat("$select", ExpressionConstants.SymbolEqual, isFirstSelectItem ? Uri.EscapeDataString(selectClause) : selectClause);
            isFirstSelectItem = false;

            string expandClause = null;
            foreach (ExpandedNavigationSelectItem expandSelectItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)))
            {
                if (string.IsNullOrEmpty(expandClause))
                {
                    expandClause = firstFlag ? expandClause : string.Concat("$expand", ExpressionConstants.SymbolEqual);
                }
                else
                {
                    expandClause += ExpressionConstants.SymbolComma;
                }

                expandClause += this.Translate(expandSelectItem);
            }

            foreach (ExpandedReferenceSelectItem expandSelectItem in selectExpandClause.SelectedItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)))
            {
                if (string.IsNullOrEmpty(expandClause))
                {
                    expandClause = firstFlag ? expandClause : string.Concat("$expand", ExpressionConstants.SymbolEqual);
                }
                else
                {
                    expandClause += ExpressionConstants.SymbolComma;
                }

                expandClause += this.Translate(expandSelectItem) + "/$ref";
            }

            if (string.IsNullOrEmpty(expandClause))
            {
                return selectClause;
            }
            else
            {
                if (firstFlag)
                {
                    return string.IsNullOrEmpty(selectClause) ? string.Concat("$expand=", Uri.EscapeDataString(expandClause)) : string.Concat(selectClause, "&$expand=", Uri.EscapeDataString(expandClause));
                }
                else
                {
                    return string.IsNullOrEmpty(selectClause) ? expandClause : string.Concat(selectClause, ";" + expandClause);
                }
            }
        }

        /// <summary>
        /// Translate a WildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(WildcardSelectItem item)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a PathSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(PathSelectItem item)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a ContainerQualifiedWildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(NamespaceQualifiedWildcardSelectItem item)
        {
            return item.Namespace;
        }

        /// <summary>
        /// Translate an ExpandedNavigationSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(ExpandedNavigationSelectItem item)
        {
            string res = string.Empty;
            if (item.SelectAndExpand != null)
            {
                string selectExpand = this.TranslateSelectExpandClause(item.SelectAndExpand, false);
                res = string.Concat(res, !string.IsNullOrEmpty(res) && !string.IsNullOrEmpty(selectExpand) ? ";" : null, selectExpand);
            }

            if (item.LevelsOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += ExpressionConstants.QueryOptionLevels;
                res += ExpressionConstants.SymbolEqual;
                res += NodeToStringBuilder.TranslateLevelsClause(item.LevelsOption);
            }

            string currentExpandClause = Translate((ExpandedReferenceSelectItem)item);
            if (currentExpandClause.EndsWith(ExpressionConstants.SymbolClosedParen, StringComparison.Ordinal))
            {
                return currentExpandClause.Insert(currentExpandClause.Length - 1, string.IsNullOrEmpty(res) ? string.Empty : ";" + res);
            }
            else
            {
                return string.Concat(currentExpandClause, string.IsNullOrEmpty(res) ? null : string.Concat(ExpressionConstants.SymbolOpenParen, res, ExpressionConstants.SymbolClosedParen));
            }
        }

        /// <summary>
        /// Translate an ExpandedReferenceSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(ExpandedReferenceSelectItem item)
        {
            NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
            string currentExpandClause = String.Join("/", item.PathToNavigationProperty.WalkWith(PathSegmentToStringTranslator.Instance).ToArray());
            string res = string.Empty;
            if (item.FilterOption != null)
            {
                res += "$filter=" + nodeToStringBuilder.TranslateFilterClause(item.FilterOption);
            }

            if (item.OrderByOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += "$orderby=" + nodeToStringBuilder.TranslateOrderByClause(item.OrderByOption);
            }

            if (item.TopOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += "$top=" + item.TopOption.ToString();
            }

            if (item.SkipOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += "$skip=" + item.SkipOption.ToString();
            }

            if (item.CountOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += "$count";
                res += ExpressionConstants.SymbolEqual;
                if (item.CountOption == true)
                {
                    res += ExpressionConstants.KeywordTrue;
                }
                else
                {
                    res += ExpressionConstants.KeywordFalse;
                }
            }

            if (item.SearchOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ";";
                res += "$search";
                res += ExpressionConstants.SymbolEqual;
                res += nodeToStringBuilder.TranslateSearchClause(item.SearchOption);
            }

            return string.Concat(currentExpandClause, string.IsNullOrEmpty(res) ? null : string.Concat(ExpressionConstants.SymbolOpenParen, res, ExpressionConstants.SymbolClosedParen));
        }
    }
}
