//   OData .NET Libraries ver. 6.8.1
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
