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

    /// <summary>
    /// Class that represents a partial subset of items on a given type that have been selected at this level of the select expand tree.
    /// </summary>
    internal sealed class PartialSelection : Selection
    {
        /// <summary>
        /// The subset of items that has been selected at this level.
        /// </summary>
        private readonly IEnumerable<SelectItem> selectedItems;

        /// <summary>
        /// Creates a <see cref="PartialSelection"/> with the specified set of <see cref="SelectItem"/>.
        /// </summary>
        /// <param name="selectedItems">The list of items on the that has been selected.</param>
        public PartialSelection(IEnumerable<SelectItem> selectedItems)
        {
            DebugUtils.CheckNoExternalCallers();
            this.selectedItems = selectedItems ?? new SelectItem[0];
        }

        /// <summary>
        /// The subset of items that has been selected at this level.
        /// </summary>
        public IEnumerable<SelectItem> SelectedItems
        {
            get
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.selectedItems;
            }
        }
    }
}
