//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
