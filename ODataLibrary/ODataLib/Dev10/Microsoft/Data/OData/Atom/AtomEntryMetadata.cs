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
    /// Type for Atom Syndication Format (Atom) entry annotationsAsArray.
    /// </summary>
    public sealed class AtomEntryMetadata : ODataAnnotatable
    {
        /// <summary>
        /// Collection of authors of a entry.
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
        /// Collection of contributors of a entry.
        /// </summary>
        public IEnumerable<AtomPersonMetadata> Contributors
        {
            get;
            set;
        }

        /// <summary>
        /// The URI of the icon for a entry.
        /// </summary>
        public Uri Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Atom link metadata for the self link.
        /// </summary>
        public AtomLinkMetadata SelfLink
        {
            get;
            set;
        }

        /// <summary>
        /// Atom link metadata for the edit link.
        /// </summary>
        public AtomLinkMetadata EditLink
        {
            get;
            set;
        }

        /// <summary>
        /// All atom link information except for the self/edit links and the navigation property links.
        /// </summary>
        public IEnumerable<AtomLinkMetadata> Links
        {
            get;
            set;
        }

        /// <summary>
        /// The date/time when the entry was published.
        /// </summary>
        public DateTimeOffset? Published
        {
            get;
            set;
        }

        /// <summary>
        /// The rights text of a entry.
        /// </summary>
        public AtomTextConstruct Rights
        {
            get;
            set;
        }

        /// <summary>
        /// If the entry was copied from a different stream the property
        /// contains the feed metadata of the original feed.
        /// </summary>
        public AtomFeedMetadata Source
        {
            get;
            set;
        }

        /// <summary>
        /// Summary of the entry.
        /// </summary>
        public AtomTextConstruct Summary
        {
            get;
            set;
        }

        /// <summary>
        /// The title of the entry.
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
