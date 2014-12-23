//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Visitors;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values passed to this method are model elements like property names or keywords.")]
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
            foreach (ExpandedNavigationSelectItem expandSelectItem in selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
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

            if (item.SelectAndExpand != null)
            {
                string selectExpand = this.TranslateSelectExpandClause(item.SelectAndExpand, false);
                res = string.Concat(res, !string.IsNullOrEmpty(res) && !string.IsNullOrEmpty(selectExpand) ? ";" : null, selectExpand);
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
