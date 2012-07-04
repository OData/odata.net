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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Type for Atom Syndication Format (Atom) feed annotationsAsArray.
    /// </summary>
    public sealed class AtomFeedMetadata : ODataAnnotatable
    {
        /// <summary>
        /// Collection of authors of a feed.
        /// </summary>
        public IEnumerable<AtomPersonMetadata> Authors
        {
            get;
            set;
        }

        /// <summary>
        /// The categories of a feed.
        /// </summary>
        public IEnumerable<AtomCategoryMetadata> Categories
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of contributors of a feed.
        /// </summary>
        public IEnumerable<AtomPersonMetadata> Contributors
        {
            get;
            set;
        }

        /// <summary>
        /// The generator of a feed.
        /// </summary>
        public AtomGeneratorMetadata Generator
        {
            get;
            set;
        }

        /// <summary>
        /// The URI of the icon for a feed.
        /// </summary>
        public Uri Icon
        {
            get;
            set;
        }

        /// <summary>
        /// All atom link information except for the next page and self links.
        /// </summary>
        public IEnumerable<AtomLinkMetadata> Links
        {
            get;
            set;
        }

        /// <summary>
        /// The Uri for the feed's logo.
        /// </summary>
        public Uri Logo
        {
            get;
            set;
        }

        /// <summary>
        /// The rights text of a feed.
        /// </summary>
        public AtomTextConstruct Rights
        {
            get;
            set;
        }

        /// <summary>
        /// The self link of the feed. This link should point to the source of the feed.
        /// </summary>
        public AtomLinkMetadata SelfLink
        {
            get;
            set;
        }

        /// <summary>
        /// The next page link of the feed. This link should point to the next page of results.
        /// </summary>
        public AtomLinkMetadata NextPageLink
        {
            get;
            set;
        }

        /// <summary>
        /// The identifier for the feed if used as metadata of an atom:source element.
        /// </summary>
        public string SourceId
        {
            get;
            set;
        }

        /// <summary>
        /// The subtitle of a feed.
        /// </summary>
        public AtomTextConstruct Subtitle
        {
            get;
            set;
        }

        /// <summary>
        /// The title of the feed.
        /// </summary>
        public AtomTextConstruct Title
        {
            get;
            set;
        }
 
        /// <summary>
        /// Date/Time of last update to the source.
        /// </summary>
        public DateTimeOffset? Updated
        {
            get;
            set;
        }
    }
}
