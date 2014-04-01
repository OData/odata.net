//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;

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
        /// TODO 1466134 We don't need this class at all anymore once V4 is always used.
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
                            childExpand.CountQueryOption, 
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
