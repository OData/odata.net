//---------------------------------------------------------------------
// <copyright file="MaterializerNestedEntry.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Client.Materialization
{
    /// <summary>
    /// Materializer state for a given <see cref="ODataNestedResourceInfo"/>
    /// </summary>
    internal class MaterializerNestedEntry : IMaterializerState
    {
        /// <summary>The <see cref="ODataNestedResourceInfo"/> entry.</summary>
        private readonly ODataNestedResourceInfo nestedResourceInfo;

        /// <summary>The <see cref="IMaterializerState"/> entries.</summary>
        private readonly List<IMaterializerState> nestedItems;

        /// <summary>
        /// Creates a new instance of MaterializerNestedEntry.
        /// </summary>
        /// <param name="nestedResourceInfo">The <see cref="ODataNestedResourceInfo"/>.</param>
        /// <param name="nestedItems">The <see cref="IMaterializerState"/> entries.</param>
        private MaterializerNestedEntry(ODataNestedResourceInfo nestedResourceInfo, List<IMaterializerState> nestedItems)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedItems != null, "nestedItems != null");

            this.nestedResourceInfo = nestedResourceInfo;
            this.nestedItems = nestedItems;
        }

        /// <summary>
        /// Gets the nested resource info.
        /// </summary>
        public ODataNestedResourceInfo NestedResourceInfo
        {
            get { return this.nestedResourceInfo; }
        }

        /// <summary>
        /// Gets the <see cref="IMaterializerState"/> items nested within the <see cref="ODataNestedResourceInfo"/>.
        /// </summary>
        public List<IMaterializerState> NestedItems
        {
            get { return this.nestedItems; }
        }

        /// <summary>
        /// Adds an <see cref="IMaterializerState"/> item to the <see cref="ODataNestedResourceInfo"/>'s nested entries.
        /// </summary>
        /// <param name="nestedItem">The <see cref="IMaterializerState"/> entry to be added to the <see cref="ODataNestedResourceInfo"/>'s nested items.</param>
        public void AddNestedItem(IMaterializerState nestedItem)
        {
            this.nestedItems.Add(nestedItem);
        }

        /// <summary>
        /// Creates a <see cref="MaterializerNestedEntry"/> for the nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info instance.</param>
        /// <param name="nestedItems">The items nested within the <see cref="ODataNestedResourceInfo"/>.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The <see cref="MaterializerNestedEntry"/> of the created <see cref="ODataNestedResourceInfo"/>.</returns>
        public static MaterializerNestedEntry CreateNestedResourceInfo(ODataNestedResourceInfo nestedResourceInfo, List<IMaterializerState> nestedItems, IODataMaterializerContext materializerContext)
        {
            Debug.Assert(materializerContext.GetAnnotation<List<IMaterializerState>>(nestedResourceInfo) == null, "nestedResourceInfo state has already been created.");
            
            if (nestedItems == null)
            {
                nestedItems = new List<IMaterializerState>();
            }
            else
            {
                materializerContext.SetAnnotation<List<IMaterializerState>>(nestedResourceInfo, nestedItems);
            }

            return new MaterializerNestedEntry(nestedResourceInfo, nestedItems);
        }
    }
}

