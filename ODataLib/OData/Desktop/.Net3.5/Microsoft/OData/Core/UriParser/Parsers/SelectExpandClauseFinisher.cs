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
    /// Fixup step for a completed select expand clause.
    /// </summary>
    internal sealed class SelectExpandClauseFinisher
    {
        /// <summary>
        /// Add any explicit nav prop links to a select expand clause as necessary.
        /// </summary>
        /// <param name="clause">the select expand clause to modify.</param>
        public static void AddExplicitNavPropLinksWhereNecessary(SelectExpandClause clause)
        {
            IEnumerable<SelectItem> selectItems = clause.SelectedItems;

            // make sure that there are already selects for this level, otherwise we change the select semantics.
            bool anyPathSelectItems = selectItems.Any(x => x is PathSelectItem);

            // if there are selects for this level, then we need to add nav prop select items for each
            // expanded nav prop
            IEnumerable<ODataSelectPath> selectedPaths = selectItems.OfType<PathSelectItem>().Select(item => item.SelectedPath);
            foreach (ExpandedNavigationSelectItem navigationSelect in selectItems.OfType<ExpandedNavigationSelectItem>())
            {
                if (anyPathSelectItems && !selectedPaths.Any(x => x.Equals(navigationSelect.PathToNavigationProperty.ToSelectPath())))
                {
                    clause.AddToSelectedItems(new PathSelectItem(navigationSelect.PathToNavigationProperty.ToSelectPath()));
                }

                AddExplicitNavPropLinksWhereNecessary(navigationSelect.SelectAndExpand);
            }
        }
    }
}
