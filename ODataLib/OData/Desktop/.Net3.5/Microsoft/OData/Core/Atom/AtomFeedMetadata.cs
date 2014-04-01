//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
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
        /// <summary>Gets or sets a collection of authors of a feed.</summary>
        /// <returns>A collection of authors of a feed.</returns>
        public IEnumerable<AtomPersonMetadata> Authors
        {
            get;
            set;
        }

        /// <summary>Gets or sets the categories of a feed.</summary>
        /// <returns>The categories of a feed.</returns>
        public IEnumerable<AtomCategoryMetadata> Categories
        {
            get;
            set;
        }

        /// <summary>Gets or sets a collection of contributors of a feed.</summary>
        /// <returns>A collection of contributors of a feed.</returns>
        public IEnumerable<AtomPersonMetadata> Contributors
        {
            get;
            set;
        }

        /// <summary>Gets or sets the generator of a feed.</summary>
        /// <returns>The generator of a feed.</returns>
        public AtomGeneratorMetadata Generator
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI of the icon for a feed.</summary>
        /// <returns>The URI of the icon for a feed.</returns>
        public Uri Icon
        {
            get;
            set;
        }

        /// <summary>Gets or sets the collection of all Atom link information except for the next page and self links.</summary>
        /// <returns>The collection of all Atom link information except for the next page and self links.</returns>
        public IEnumerable<AtomLinkMetadata> Links
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI for the feed's logo.</summary>
        /// <returns>The URI for the feed’s logo.</returns>
        public Uri Logo
        {
            get;
            set;
        }

        /// <summary>Gets or sets the rights text of a feed.</summary>
        /// <returns>The rights text of a feed.</returns>
        public AtomTextConstruct Rights
        {
            get;
            set;
        }

        /// <summary>Gets or sets the self link of the feed. This link should point to the source of the feed.</summary>
        /// <returns>The self link of the feed.</returns>
        public AtomLinkMetadata SelfLink
        {
            get;
            set;
        }

        /// <summary>Gets the next page link of the feed. This link should point to the next page of results.</summary>
        public AtomLinkMetadata NextPageLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets the identifier for the feed if used as metadata of an Atom:source element.</summary>
        /// <returns>The identifier for the feed if used as metadata of an Atom:source element.</returns>
        public Uri SourceId
        {
            get;
            set;
        }

        /// <summary>Gets or sets the subtitle of a feed.</summary>
        /// <returns>The subtitle of a feed.</returns>
        public AtomTextConstruct Subtitle
        {
            get;
            set;
        }

        /// <summary>Gets or sets the title of the feed.</summary>
        /// <returns>The title of the feed.</returns>
        public AtomTextConstruct Title
        {
            get;
            set;
        }
 
        /// <summary>Gets or sets the date and time of last update to the source.</summary>
        /// <returns>The date and time of last update to the source.</returns>
        public DateTimeOffset? Updated
        {
            get;
            set;
        }
    }
}
