//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;

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
        internal void AddToSelectedItems(SelectItem itemToAdd)
        {
            ExceptionUtils.CheckArgumentNotNull(itemToAdd, "itemToAdd");

            if (this.selectedItems.Any(x => x is WildcardSelectItem) && UriUtils.IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
            {
                return;
            }

            bool isWildcard = itemToAdd is WildcardSelectItem;

            List<SelectItem> newSelectedItems = new List<SelectItem>();

            foreach (SelectItem selectedItem in this.selectedItems)
            {
                if (isWildcard)
                {
                    if (!UriUtils.IsStructuralSelectionItem(selectedItem))
                    {
                        newSelectedItems.Add(selectedItem);
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
    }
}
