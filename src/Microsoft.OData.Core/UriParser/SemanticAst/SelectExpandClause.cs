//---------------------------------------------------------------------
// <copyright file="SelectExpandClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Class representing the combined semantic meaning of any select or expand clauses in the uri.
    /// </summary>
    public sealed class SelectExpandClause
    {
        /// <summary>
        /// The selected properties and operations.
        /// </summary>
        /// <remarks>This list includes expanded navigations properties, which may have additional nested selections and expansions.</remarks>
        private ReadOnlyCollection<SelectItem> selectedItems;

        /// <summary>
        /// Gets a flag indicating that everything at this level has been selected.
        /// </summary>
        /// <remarks>
        /// If true, then all structural properties, bound actions and functions, and all navigations in the SelectedItems list have been selected.
        /// </remarks>
        private bool? allSelected;

        /// <summary>
        /// Constructs a <see cref="SelectExpandClause"/> from the given parameters.
        /// </summary>
        /// <param name="selectedItems">The selected properties and operations. This list should include any expanded navigation properties.</param>
        /// <param name="allSelected">Flag indicating if all items have been selected at this level.</param>
        public SelectExpandClause(IEnumerable<SelectItem> selectedItems, bool allSelected)
        {
            this.selectedItems = selectedItems != null ? new ReadOnlyCollection<SelectItem>(selectedItems.ToList()) : new ReadOnlyCollection<SelectItem>(new List<SelectItem>());
            this.allSelected = allSelected;
        }

        /// <summary>
        /// Gets the selected properties and operations.
        /// </summary>
        /// <remarks>This list includes expanded navigations properties, which may have additional nested selections and expansions.</remarks>
        public IEnumerable<SelectItem> SelectedItems
        {
            get
            {
                ////Debug.Assert(!this.usedInternalLegacyConstructor || this.selectedItems != null, "You cannot get the list of selected items until processing is complete.");
                return this.selectedItems.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets a flag indicating that everything at this level has been selected.
        /// </summary>
        /// <remarks>
        /// If true, then all structural properties, bound actions and functions, and all navigations in the SelectedItems list have been selected.
        /// </remarks>
        public bool AllSelected
        {
            get
            {
                return this.allSelected.Value;
            }
        }

        /// <summary>
        /// Add a select item to the current list of selection items
        /// </summary>
        /// <param name="itemToAdd">The item to add</param>
        /// <param name="getOnlySubset">A boolean value indicating whether to get subset only</param>
        internal void AddToSelectedItems(SelectItem itemToAdd, bool getOnlySubset = false)
        {
            ExceptionUtils.CheckArgumentNotNull(itemToAdd, "itemToAdd");

            if (this.selectedItems.Any(x => x is WildcardSelectItem) && IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
            {
                return;
            }

            bool isWildcard = itemToAdd is WildcardSelectItem;

            List<SelectItem> newSelectedItems = new List<SelectItem>();

            foreach (SelectItem selectedItem in this.selectedItems)
            {
                if (isWildcard)
                {
                    if (!IsStructuralSelectionItem(selectedItem))
                    {
                        newSelectedItems.Add(selectedItem);
                    }
                    else if (getOnlySubset)
                    {
                        var pathItem = selectedItem as PathSelectItem;
                        if (pathItem != null && pathItem.SelectAndExpand != null)
                        {
                            newSelectedItems.Add(selectedItem);
                        }
                    }
                }
                else
                {
                    newSelectedItems.Add(selectedItem);
                }
            }

            newSelectedItems.Add(itemToAdd);
            this.selectedItems = new ReadOnlyCollection<SelectItem>(newSelectedItems);
        }


        /// <summary>
        /// Sets all the value of AllSelected
        /// </summary>
        /// <param name="newValue">the new value to set</param>
        internal void SetAllSelected(bool newValue)
        {
            this.allSelected = newValue;
        }

        /// <summary>
        /// is this selection item a structural or navigation property selection item.
        /// </summary>
        /// <param name="selectItem">the selection item to check</param>
        /// <returns>true if this selection item is a structural property selection item.</returns>
        private static bool IsStructuralOrNavigationPropertySelectionItem(SelectItem selectItem)
        {
            PathSelectItem pathSelectItem = selectItem as PathSelectItem;
            return pathSelectItem != null && (pathSelectItem.SelectedPath.LastSegment is NavigationPropertySegment || pathSelectItem.SelectedPath.LastSegment is PropertySegment);
        }

        /// <summary>
        /// is this selection item a structural selection item.
        /// </summary>
        /// <param name="selectItem">the selection item to check</param>
        /// <returns>true if this selection item is a structural property selection item.</returns>
        private static bool IsStructuralSelectionItem(SelectItem selectItem)
        {
            PathSelectItem pathSelectItem = selectItem as PathSelectItem;
            return pathSelectItem != null && (pathSelectItem.SelectedPath.LastSegment is PropertySegment);
        }
    }
}