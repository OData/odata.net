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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Class representing the combined semantic meaning of any select or expand clauses in the uri.
    /// </summary>
    public sealed class SelectExpandClause
    {
        /// <summary>
        /// Mapping that contains the set of navigation properties for the associated entity that should be expanded, and respective details about the expansions. 
        /// </summary>
        private readonly Expansion expansion;

        /// <summary>
        /// Internal flag indicating that this clause was built using the legacy Selection and Expansion classes. In this case, we must 
        /// call ComputeFinalSelectedItems() before handing out the object publically.
        /// </summary>
        private readonly bool usedInternalLegacyConsturctor;

        /// <summary>
        /// The <see cref="Selection"/> object that describes what properties and functions should be selected from the associated <see cref="IEdmEntityType"/> at this level.
        /// </summary>
        private Selection selection;

        /// <summary>
        /// The selected properties and operations.
        /// </summary>
        /// <remarks>This list includes expanded navigations properties, which may have additional nested selections and expansions.</remarks>
        private ICollection<SelectItem> selectedItems;

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
        public SelectExpandClause(ICollection<SelectItem> selectedItems, bool allSelected)
        {
            DebugUtils.CheckNoExternalCallers();
            this.usedInternalLegacyConsturctor = false;
            this.selectedItems = selectedItems;
            this.allSelected = allSelected;
        }

        /// <summary>
        /// Constructs a <see cref="SelectExpandClause"/> from the given parameters.
        /// </summary>
        /// <param name="selection">The <see cref="Selection"/> object that describes what properties and functions should be selected from the associated <see cref="IEdmEntityType"/>.</param>
        /// <param name="expansion">Mapping that contains the set of navigation properties for the associated entity that should be expanded, and respective details about the expansions. </param>
        internal SelectExpandClause(Selection selection, Expansion expansion)
        {
            DebugUtils.CheckNoExternalCallers();
            this.usedInternalLegacyConsturctor = true;
            this.selection = selection;
            this.expansion = expansion ?? new Expansion(new List<ExpandedNavigationSelectItem>());
            this.selectedItems = null;
            this.allSelected = null;
        }

        /// <summary>
        /// Gets the selected properties and operations.
        /// </summary>
        /// <remarks>This list includes expanded navigations properties, which may have additional nested selections and expansions.</remarks>
        public IEnumerable<SelectItem> SelectedItems
        {
            get
            {
                Debug.Assert(!this.usedInternalLegacyConsturctor || this.selectedItems != null, "You cannot get the list of selected items until processing is complete.");
                return this.selectedItems;
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
                // If we used the internal only ctor then we compute this on the fly
                if (this.usedInternalLegacyConsturctor)
                {
                    return this.Selection is AllSelection;
                }

                return this.allSelected.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Selection"/> object that describes what properties and functions should be selected from the associated <see cref="IEdmEntityType"/>.
        /// 
        /// TODO Note that this is a legacy internal concept. 
        /// At the last minute we changed the public API but chose not to change how the implementation work to manage risk.
        /// We should clean this up and remove this property.
        /// </summary>
        internal Selection Selection
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
                return this.selection;
            }
        }

        /// <summary>
        /// Mapping that contains the set of navigation properties for the associated entity that should be expanded, and respective details about the expansions. 
        /// 
        /// TODO Note that this is a legacy internal concept. 
        /// At the last minute we changed the public API but chose not to change how the implementation work to manage risk.
        /// We should clean this up and remove this property.
        /// </summary>
        internal Expansion Expansion
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
                return this.expansion;
            }
        }

        /// <summary>
        /// Add a selection item to the current selection.
        /// </summary>
        /// <param name="itemToAdd">the new selection item to add</param>
        internal void AddSelectItem(SelectItem itemToAdd)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
            ExceptionUtils.CheckArgumentNotNull(itemToAdd, "itemToAdd");
            
            // if this is already an all selection, then adding a specific selection item is purely redundant.
            if (this.selection is AllSelection)
            {
                return;
            }

            // otherwise, if it is either an expands-only or unkown selection, convert it to a partial selection before adding this item.
            if (this.selection is ExpansionsOnly || this.selection is UnknownSelection)
            {
                this.selection = new PartialSelection(new List<SelectItem>() { itemToAdd });
            }
            else
            {
                // TODO: why do we create a whole new list?
                List<SelectItem> newSelections = ((PartialSelection)this.selection).SelectedItems.ToList();
                if (itemToAdd is WildcardSelectItem)
                {
                    IEnumerable<SelectItem> structualAndNavItems = newSelections.Where(UriUtils.IsStructuralOrNavigationPropertySelectionItem).ToArray();
                    foreach (SelectItem item in structualAndNavItems)
                    {
                        newSelections.Remove(item);
                    }
                }
                else if (UriUtils.IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
                {
                    if (newSelections.Any(item => item is WildcardSelectItem))
                    {
                        return;
                    }
                }

                newSelections.Add(itemToAdd);
                this.selection = new PartialSelection(newSelections);
            }
        }

        /// <summary>
        /// Switch to an AllSelection at this level and recursively at all levels below this one.
        /// This is non-reversable because once everything is selected, selecting a specific property or other item is redundant.
        /// </summary>
        internal void SetAllSelectionRecursively()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
            this.selection = AllSelection.Instance;
            foreach (var expandItem in this.expansion.ExpandItems)
            {
                expandItem.SelectAndExpand.SetAllSelectionRecursively();
            }
        }

        /// <summary>
        /// Initializes the selection for this clause as ExpansionsOnly if it is not already partial.
        /// </summary>
        internal void InitializeEmptySelection()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
            if (this.selection is UnknownSelection)
            {
                this.selection = ExpansionsOnly.Instance;
            }
        }

        /// <summary>
        /// Computes the list of SelectItems that will be publically availible to consumers.
        /// </summary>
        internal void ComputeFinalSelectedItems()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.usedInternalLegacyConsturctor, "Should never call this property unless we used internal ctor.");
            if (this.selectedItems != null)
            {
                throw new InvalidOperationException("This should only be called once at the end of processing.");
            }

            Debug.Assert(!(this.selection is UnknownSelection), "Selection should have been set by now.");

            // This is the public property
            this.selectedItems = new List<SelectItem>();

            var partialSelection = this.Selection as PartialSelection;
            if (partialSelection != null)
            {
                // Copy all items from a partial selection into the new list
                foreach (var item in partialSelection.SelectedItems)
                {
                    this.selectedItems.Add(item);
                }
            }

            foreach (var expandItem in this.Expansion.ExpandItems)
            {
                // Recurse this computation
                if (expandItem.SelectAndExpand != null)
                {
                    expandItem.SelectAndExpand.ComputeFinalSelectedItems();
                }

                // Expands go in the same list of selections
                this.selectedItems.Add(expandItem);
            }
        }
    }
}
