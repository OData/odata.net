//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;

    /// <summary>
    /// Materializer state for a given ODataFeed
    /// </summary>
    internal struct MaterializerFeed
    {
        /// <summary>The feed.</summary>
        private readonly ODataFeed feed;

        /// <summary>The entries.</summary>
        private readonly IEnumerable<ODataEntry> entries;

        /// <summary>
        /// Prevents a default instance of the <see cref="MaterializerFeed"/> struct from being created.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        private MaterializerFeed(ODataFeed feed, IEnumerable<ODataEntry> entries)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(entries != null, "entries != null");

            this.feed = feed;
            this.entries = entries;
        }

        /// <summary>
        /// Gets the feed.
        /// </summary>
        public ODataFeed Feed
        {
            get { return this.feed; }
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        public IEnumerable<ODataEntry> Entries
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
        /// Creates the materializer feed.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        /// <returns>The materializer feed.</returns>
        public static MaterializerFeed CreateFeed(ODataFeed feed, IEnumerable<ODataEntry> entries)
        {
            Debug.Assert(feed.GetAnnotation<IEnumerable<ODataEntry>>() == null, "Feed state has already been created.");
            if (entries == null)
            {
                entries = Enumerable.Empty<ODataEntry>();
            }
            else
            {
                feed.SetAnnotation<IEnumerable<ODataEntry>>(entries);
            }

            return new MaterializerFeed(feed, entries);
        }

        /// <summary>
        /// Gets the materializer feed.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <returns>The materializer feed.</returns>
        public static MaterializerFeed GetFeed(ODataFeed feed)
        {
            IEnumerable<ODataEntry> entries = feed.GetAnnotation<IEnumerable<ODataEntry>>();
            return new MaterializerFeed(feed, entries);
        }
    }
}
