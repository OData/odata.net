//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData;

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
