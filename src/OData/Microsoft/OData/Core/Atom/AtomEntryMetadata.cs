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
    /// Type for Atom Syndication Format (Atom) entry annotationsAsArray.
    /// </summary>
    public sealed class AtomEntryMetadata : ODataAnnotatable
    {
        /// <summary>Gets or sets a collection of authors of an entry.</summary>
        /// <returns>A collection of authors of an entry.</returns>
        public IEnumerable<AtomPersonMetadata> Authors
        {
            get;
            set;
        }

        /// <summary>Gets or sets the ATOM metadata for the category element which stores the type name of the entry.</summary>
        public AtomCategoryMetadata CategoryWithTypeName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the categories of an entry.</summary>
        /// <returns>The categories of an entry.</returns>
        public IEnumerable<AtomCategoryMetadata> Categories
        {
            get;
            set;
        }

        /// <summary>Gets or sets a collection of contributors of an entry.</summary>
        /// <returns>A collection of contributors of an entry.</returns>
        public IEnumerable<AtomPersonMetadata> Contributors
        {
            get;
            set;
        }

        /// <summary>Gets or sets an Atom link metadata for the self link.</summary>
        /// <returns>An Atom link metadata for the self link.</returns>
        public AtomLinkMetadata SelfLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets an Atom link metadata for the edit link.</summary>
        /// <returns>An Atom link metadata for the edit link.</returns>
        public AtomLinkMetadata EditLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets the collection of all Atom link information except for the self/edit links and the navigation property links.</summary>
        /// <returns>The collection of all Atom link information except for the self/edit links and the navigation property links.</returns>
        public IEnumerable<AtomLinkMetadata> Links
        {
            get;
            set;
        }

        /// <summary>Gets or sets the date and time when the entry was published.</summary>
        /// <returns>The date and time when the entry was published.</returns>
        public DateTimeOffset? Published
        {
            get;
            set;
        }

        /// <summary>Gets or sets the rights text of an entry.</summary>
        /// <returns>The rights text of an entry.</returns>
        public AtomTextConstruct Rights
        {
            get;
            set;
        }

        /// <summary>Gets or sets the source of an entry and if the entry was copied from a different stream the property contains the feed metadata of the original feed.</summary>
        /// <returns>The source of an entry.</returns>
        public AtomFeedMetadata Source
        {
            get;
            set;
        }

        /// <summary>Gets or sets the summary of the entry.</summary>
        /// <returns>The summary of the entry.</returns>
        public AtomTextConstruct Summary
        {
            get;
            set;
        }

        /// <summary>Gets or sets the title of the entry.</summary>
        /// <returns>The title of the entry.</returns>
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
