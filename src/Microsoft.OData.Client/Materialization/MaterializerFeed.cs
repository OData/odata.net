//---------------------------------------------------------------------
// <copyright file="MaterializerFeed.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;

    /// <summary>
    /// ObjectMaterializer state for a given ODataResourceSet
    /// </summary>
    internal struct MaterializerFeed : IMaterializerState
    {
        /// <summary>The feed.</summary>
        private readonly ODataResourceSet feed;

        /// <summary>The entries.</summary>
        private readonly IEnumerable<ODataResource> entries;

        /// <summary>The items.</summary>
        private readonly List<IMaterializerState> items;

        /// <summary>
        /// Prevents a default instance of the <see cref="MaterializerFeed"/> struct from being created.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        private MaterializerFeed(ODataResourceSet feed, IEnumerable<ODataResource> entries)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(entries != null, "entries != null");

            this.feed = feed;
            this.entries = entries;
            this.items = new List<IMaterializerState>();
        }

        /// <summary>
        /// Gets the feed.
        /// </summary>
        public ODataResourceSet Feed
        {
            get { return this.feed; }
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        public IEnumerable<ODataResource> Entries
        {
            get { return this.entries; }
        }

        /// <summary>
        /// URI representing the next page link.
        /// </summary>
        public Uri NextPageLink
        {
            get { return this.feed.NextPageLink; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public List<IMaterializerState> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Adds an <see cref="IMaterializerState"/> item to the items collection.
        /// </summary>
        /// <param name="item">The <see cref="IMaterializerState"/> item to be added.</param>
        public void AddItem(IMaterializerState item)
        {
            this.items.Add(item);
        }

        /// <summary>
        /// Creates the materializer feed.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The materializer feed.</returns>
        public static MaterializerFeed CreateFeed(ODataResourceSet feed, IEnumerable<ODataResource> entries, IODataMaterializerContext materializerContext)
        {
            Debug.Assert(materializerContext.GetAnnotation<IEnumerable<ODataResource>>(feed) == null, "Feed state has already been created.");
            if (entries == null)
            {
                entries = Enumerable.Empty<ODataResource>();
            }
            else
            {
                materializerContext.SetAnnotation<IEnumerable<ODataResource>>(feed, entries);
            }

            return new MaterializerFeed(feed, entries);
        }

        /// <summary>
        /// Gets the materializer feed.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="materializerContext">The current materializer context.</param>
        /// <returns>The materializer feed.</returns>
        public static MaterializerFeed GetFeed(ODataResourceSet feed, IODataMaterializerContext materializerContext)
        {
            IEnumerable<ODataResource> entries = materializerContext.GetAnnotation<IEnumerable<ODataResource>>(feed);
            return new MaterializerFeed(feed, entries);
        }
    }
}