//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseFinisher.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Linq;

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
            foreach (ExpandedNavigationSelectItem navigationSelect in selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)))
            {
                if (anyPathSelectItems && !selectedPaths.Any(x => x.Equals(navigationSelect.PathToNavigationProperty.ToSelectPath())))
                {
                    clause.AddToSelectedItems(new PathSelectItem(navigationSelect.PathToNavigationProperty.ToSelectPath()));
                }

                AddExplicitNavPropLinksWhereNecessary(navigationSelect.SelectAndExpand);
            }

            foreach (ExpandedReferenceSelectItem navigationSelect in selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)))
            {
                if (anyPathSelectItems && !selectedPaths.Any(x => x.Equals(navigationSelect.PathToNavigationProperty.ToSelectPath())))
                {
                    clause.AddToSelectedItems(new PathSelectItem(navigationSelect.PathToNavigationProperty.ToSelectPath()));
                }
            }
        }
    }
}