//---------------------------------------------------------------------
// <copyright file="MaterializerDeltaFeed.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Client.Materialization
{
    /// <summary>
    /// ObjectMaterializer state for a given <see cref="ODataDeltaResourceSet"/>.
    /// </summary>
    internal class MaterializerDeltaFeed : IMaterializerState
    {
        /// <summary>The delta feed.</summary>
        private readonly ODataDeltaResourceSet deltaFeed;

        /// <summary>The entries.</summary>
        private readonly List<IMaterializerState> entries;

        /// <summary>
        /// Creates an instance of the <see cref="MaterializerDeltaFeed"/>.
        /// </summary>
        /// <param name="deltaFeed">The delta feed.</param>
        /// <param name="entries">The entries.</param>
        private MaterializerDeltaFeed(ODataDeltaResourceSet deltaFeed, List<IMaterializerState> entries)
        {
            Debug.Assert(deltaFeed != null, "deltaFeed != null");
            Debug.Assert(entries != null, "entries != null");

            this.deltaFeed = deltaFeed;
            this.entries = entries;
        }

        /// <summary>
        /// Gets the delta feed.
        /// </summary>
        public ODataDeltaResourceSet DeltaFeed
        {
            get { return this.deltaFeed; }
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        public List<IMaterializerState> Entries
        {
            get { return this.entries; }
        }

        /// <summary>
        /// Adds an <see cref="IMaterializerState"/> entry to the entries collection.
        /// </summary>
        /// <param name="entry">The <see cref="IMaterializerState"/> entry to be added.</param>
        public void AddEntry(IMaterializerState entry)
        {
            this.entries.Add(entry);
        }

        /// <summary>
        /// Creates the <see cref="MaterializerDeltaFeed"/>.
        /// </summary>
        /// <param name="deltaFeed">The delta feed to be created.</param>
        /// <param name="entries">The entries.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The materializer delta feed.</returns>
        public static MaterializerDeltaFeed CreateDeltaFeed(ODataDeltaResourceSet deltaFeed, List<IMaterializerState> entries, IODataMaterializerContext materializerContext)
        {
            Debug.Assert(materializerContext.GetAnnotation<List<IMaterializerState>>(deltaFeed) == null, "Delta feed state has already been created.");
            
            if (entries == null)
            {
                entries = new List<IMaterializerState>();
            }
            else
            {
                materializerContext.SetAnnotation(deltaFeed, entries);
            }

            return new MaterializerDeltaFeed(deltaFeed, entries);
        }

        /// <summary>
        /// Gets the delta feed.
        /// </summary>
        /// <param name="deltaFeed">The delta feed.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The materializer delta feed.</returns>
        public static MaterializerDeltaFeed GetDeltaFeed(ODataDeltaResourceSet deltaFeed, IODataMaterializerContext materializerContext)
        {
            List<IMaterializerState> entries = materializerContext.GetAnnotation<List<IMaterializerState>>(deltaFeed);
            return new MaterializerDeltaFeed(deltaFeed, entries);
        }
    }
}
