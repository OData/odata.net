//---------------------------------------------------------------------
// <copyright file="MaterializerDeletedEntry.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData.Client.Materialization
{
    /// <summary>
    /// ObjectMaterializer state for a given <see cref="ODataDeletedResource"/>.
    /// </summary>
    internal class MaterializerDeletedEntry : IMaterializerState
    {
        /// <summary>The <see cref="ODataDeletedResource"/> entry.</summary>
        private readonly ODataDeletedResource deletedEntry;

        /// <summary>
        /// Creates a new instance of <see cref="MaterializerDeletedEntry"/>.
        /// </summary>
        /// <param name="deletedEntry">The <see cref="ODataDeletedResource"/> of the deleted entry.</param>
        private MaterializerDeletedEntry(ODataDeletedResource deletedEntry)
        {
            this.deletedEntry = deletedEntry;
        }

        /// <summary>
        /// Gets the <see cref="ODataDeletedResource"/>.
        /// </summary>
        public ODataDeletedResource DeletedResource
        {
            get { return this.deletedEntry; }
        }

        /// <summary>
        /// Creates the <see cref="MaterializerDeletedEntry"/> of the <see cref="ODataDeletedResource"/> entry.
        /// </summary>
        /// <param name="deletedEntry">The <see cref="ODataDeletedResource"/> entry.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The <see cref="MaterializerDeletedEntry"/> of the <see cref="ODataDeletedResource"/> entry.</returns>
        public static MaterializerDeletedEntry CreateDeletedEntry(ODataDeletedResource deletedEntry, IODataMaterializerContext materializerContext)
        {
            Debug.Assert(materializerContext.GetAnnotation<ODataDeletedResource>(deletedEntry) == null, "MaterializerDeletedEntry state has already been created.");

            MaterializerDeletedEntry materializerDeletedEntry = new MaterializerDeletedEntry(deletedEntry);
            materializerContext.SetAnnotation<MaterializerDeletedEntry>(deletedEntry, materializerDeletedEntry);

            return materializerDeletedEntry;
        }
    }
}
