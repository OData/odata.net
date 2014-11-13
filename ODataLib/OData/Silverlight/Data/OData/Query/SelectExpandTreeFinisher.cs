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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Polish a combined select expand tree.
    /// </summary>
    internal static class SelectExpandTreeFinisher
    {
        /// <summary>
        /// Prune off any unneccessary expands
        /// </summary>
        /// <param name="clauseToPrune">the clause to prune</param>
        /// <returns>a pruned tree.</returns>
        internal static SelectExpandClause PruneSelectExpandTree(SelectExpandClause clauseToPrune)
        {
            DebugUtils.CheckNoExternalCallers();

            // prune all child nodes first, then prune this node (Post-Order Tree traversal)
            if (clauseToPrune == null)
            {
                return null;
            }

            // if the selection on a clause is still unknown then it means it was expanded but never
            // selected, in which case it should be pruned out of the tree.
            if (clauseToPrune.Selection is UnknownSelection)
            {
                return null;
            }

            if (clauseToPrune.Selection is AllSelection)
            {
                return clauseToPrune;
            }

            if (clauseToPrune.Expansion != null)
            {
                List<ExpandedNavigationSelectItem> newChildExpandItems = new List<ExpandedNavigationSelectItem>();
                
                // build a new list of child expand items for this level, pruning off any
                // unneccessary expand items in the process.
                foreach (ExpandedNavigationSelectItem childExpand in clauseToPrune.Expansion.ExpandItems)
                {
                    SelectExpandClause newSubExpand = PruneSelectExpandTree(childExpand.SelectAndExpand);
                    if (newSubExpand == childExpand.SelectAndExpand)
                    {
                        newChildExpandItems.Add(childExpand);
                    }
                    else if (newSubExpand != null)
                    {
                        newChildExpandItems.Add(new ExpandedNavigationSelectItem(
                            childExpand.PathToNavigationProperty, 
                            childExpand.EntitySet,
                            childExpand.FilterOption,
                            childExpand.OrderByOption,
                            childExpand.TopOption,
                            childExpand.SkipOption, 
                            childExpand.InlineCountOption, 
                            newSubExpand));
                    }
                }

                if (newChildExpandItems.Count == 0 && clauseToPrune.Selection is ExpansionsOnly)
                {
                    return null;
                }
                else
                {
                    return new SelectExpandClause(clauseToPrune.Selection, new Expansion(newChildExpandItems));
                }
            }
            else
            {
                if (clauseToPrune.Selection is ExpansionsOnly)
                {
                    return null;
                }
                else
                {
                    return clauseToPrune;
                }
            }
        }
    }
}
