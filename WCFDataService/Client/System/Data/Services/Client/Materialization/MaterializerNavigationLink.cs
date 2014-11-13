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
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Materializer state for a given ODataNavigationLink
    /// </summary>
    internal class MaterializerNavigationLink
    {
        /// <summary>The navigation link.</summary>
        private readonly ODataNavigationLink link;

        /// <summary>An object field for the feed or enty.</summary>
        private readonly object feedOrEntry;

        /// <summary>
        /// Prevents a default instance of the <see cref="MaterializerNavigationLink"/> struct from being created.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="materializedFeedOrEntry">Value of the link.</param>
        private MaterializerNavigationLink(ODataNavigationLink link, object materializedFeedOrEntry)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(materializedFeedOrEntry != null, "materializedFeedOrEntry != null");
            Debug.Assert(materializedFeedOrEntry is MaterializerEntry || materializedFeedOrEntry is ODataFeed, "must be feed or entry");
            this.link = link;
            this.feedOrEntry = materializedFeedOrEntry;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        public ODataNavigationLink Link
        {
            get { return this.link; }
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        public MaterializerEntry Entry
        {
            get { return this.feedOrEntry as MaterializerEntry; }
        }

        /// <summary>
        /// Gets the feed.
        /// </summary>
        public ODataFeed Feed
        {
            get { return this.feedOrEntry as ODataFeed; }
        }

        /// <summary>
        /// Creates the materializer link with an entry.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink CreateLink(ODataNavigationLink link, MaterializerEntry entry)
        {
            Debug.Assert(link.GetAnnotation<MaterializerNavigationLink>() == null, "there should be no MaterializerNavigationLink annotation on the entry link yet");
            MaterializerNavigationLink materializedNavigationLink = new MaterializerNavigationLink(link, entry);
            link.SetAnnotation<MaterializerNavigationLink>(materializedNavigationLink);
            return materializedNavigationLink;
        }

        /// <summary>
        /// Creates the materializer link with a feed.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="feed">The feed.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink CreateLink(ODataNavigationLink link, ODataFeed feed)
        {
            Debug.Assert(link.GetAnnotation<MaterializerNavigationLink>() == null, "there should be no MaterializerNavigationLink annotation on the feed link yet");
            MaterializerNavigationLink materializedNavigationLink = new MaterializerNavigationLink(link, feed);
            link.SetAnnotation<MaterializerNavigationLink>(materializedNavigationLink);
            return materializedNavigationLink;
        }

        /// <summary>
        /// Gets the materializer link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink GetLink(ODataNavigationLink link)
        {
            return link.GetAnnotation<MaterializerNavigationLink>();
        }
    }
}
