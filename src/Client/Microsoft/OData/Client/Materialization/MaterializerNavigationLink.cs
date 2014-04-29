//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using Microsoft.OData.Core;

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
