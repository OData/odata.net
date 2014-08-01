//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Query
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
        /// the flag used to mark the ExpandNavigation first appeared
        /// </summary>
        private bool isFirstExpandItem = true;

        /// <summary>
        /// mark whether the selectClause appears the first time
        /// </summary>
        private bool isFirstSelectClause = true;

        /// <summary>
        /// Build the expand clause for a given level in the selectExpandClause
        /// </summary>
        /// <param name="selectExpandClause">the current level select expand clause</param>
        /// <returns>the select and expand segment for context url in this level.</returns>
        public string TranslateSelectExpandClause(SelectExpandClause selectExpandClause)
        {
            ExceptionUtils.CheckArgumentNotNull(selectExpandClause, "selectExpandClause");

            List<string> selectList = selectExpandClause.GetCurrentLevelSelectList();
            string selectClause = string.Empty;
            if (selectExpandClause.AllSelected == false && selectList.Any())
            {
                string tmp = String.Join(ODataConstants.ContextUriProjectionPropertySeparator, selectList.ToArray());
                selectClause = string.IsNullOrEmpty(tmp) ? null : ("$select" + ExpressionConstants.SymbolEqual + tmp);
            }
            else
            {
                if (isFirstSelectClause == true && selectExpandClause.AllSelected == false)
                {
                    selectClause = "$select" + ExpressionConstants.SymbolEqual;
                }
            }

            isFirstSelectClause = false;
            string expandClause = string.Empty;
            foreach (ExpandedNavigationSelectItem expandSelectItem in selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
            {
                if (expandClause == string.Empty)
                {
                    expandClause = isFirstExpandItem && !string.IsNullOrEmpty(selectClause) ? ExpressionConstants.SymbolQueryConcatenate : (string.IsNullOrEmpty(selectClause) ? null : ExpressionConstants.SymbolComma);
                    expandClause += "$expand" + ExpressionConstants.SymbolEqual;
                    isFirstExpandItem = false;
                }
                else
                {
                    expandClause += ExpressionConstants.SymbolComma;
                }

                expandClause += this.Translate(expandSelectItem);
            }

             return string.Concat(selectClause, expandClause);
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
            return string.Empty;
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
                res += string.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolQueryConcatenate;
                res += "$orderby=" + nodeToStringBuilder.TranslateOrderByClause(item.OrderByOption);
            }

            if (item.SelectAndExpand != null)
            {
                string selectExpand = this.TranslateSelectExpandClause(item.SelectAndExpand);
                res = string.Concat(res, string.IsNullOrEmpty(res) || string.IsNullOrEmpty(selectExpand) ? null : ExpressionConstants.SymbolQueryConcatenate, selectExpand);
            }

            if (item.TopOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolQueryConcatenate;
                res += "$top=" + item.TopOption.ToString();
            }

            if (item.SkipOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolQueryConcatenate;
                res += "$skip=" + item.SkipOption.ToString();
            }

            if (item.CountOption != null)
            {
                res += string.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolQueryConcatenate;
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
                res += string.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolQueryConcatenate;
                res += "$skip";
                res += ExpressionConstants.SymbolEqual;
                res += nodeToStringBuilder.TranslateSearchClause(item.SearchOption);
            }

            return string.Concat(currentExpandClause, string.IsNullOrEmpty(res) ? null : string.Concat(ExpressionConstants.SymbolOpenParen, res, ExpressionConstants.SymbolClosedParen));
        }
    }
}
