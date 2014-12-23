//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Core.Atom;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for reading ATOM metadata.
    /// </summary>
    internal static class AtomMetadataReaderUtils
    {
        /// <summary>
        /// Creates a new instance of ATOM entry metadata.
        /// </summary>
        /// <returns>The newly created ATOM entry metadata.</returns>
        internal static AtomEntryMetadata CreateNewAtomEntryMetadata()
        {
            return new AtomEntryMetadata
            {
                Authors = ReadOnlyEnumerable<AtomPersonMetadata>.Empty(),
                Categories = ReadOnlyEnumerable<AtomCategoryMetadata>.Empty(),
                Contributors = ReadOnlyEnumerable<AtomPersonMetadata>.Empty(),
                Links = ReadOnlyEnumerable<AtomLinkMetadata>.Empty(),
            };
        }

        /// <summary>
        /// Creates a new instance of ATOM feed metadata.
        /// </summary>
        /// <returns>The newly created ATOM feed metadata.</returns>
        internal static AtomFeedMetadata CreateNewAtomFeedMetadata()
        {
            return new AtomFeedMetadata
            {
                Authors = ReadOnlyEnumerable<AtomPersonMetadata>.Empty(),
                Categories = ReadOnlyEnumerable<AtomCategoryMetadata>.Empty(),
                Contributors = ReadOnlyEnumerable<AtomPersonMetadata>.Empty(),
                Links = ReadOnlyEnumerable<AtomLinkMetadata>.Empty(),
            };
        }

        /// <summary>
        /// Adds a new author to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the author to.</param>
        /// <param name="authorMetadata">The author metadata to add.</param>
        internal static void AddAuthor(this AtomEntryMetadata entryMetadata, AtomPersonMetadata authorMetadata)
        {
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(authorMetadata != null, "authorMetadata != null");

            entryMetadata.Authors = entryMetadata.Authors.ConcatToReadOnlyEnumerable("Authors", authorMetadata);
        }

        /// <summary>
        /// Adds a new contributor to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the contributor to.</param>
        /// <param name="contributorMetadata">The contributor metadata to add.</param>
        internal static void AddContributor(this AtomEntryMetadata entryMetadata, AtomPersonMetadata contributorMetadata)
        {
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(contributorMetadata != null, "contributorMetadata != null");

            entryMetadata.Contributors = entryMetadata.Contributors.ConcatToReadOnlyEnumerable("Contributors", contributorMetadata);
        }

        /// <summary>
        /// Adds a new link to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the link to.</param>
        /// <param name="linkMetadata">The link metadata to add.</param>
        internal static void AddLink(this AtomEntryMetadata entryMetadata, AtomLinkMetadata linkMetadata)
        {
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(linkMetadata != null, "linkMetadata != null");

            entryMetadata.Links = entryMetadata.Links.ConcatToReadOnlyEnumerable("Ref", linkMetadata);
        }

        /// <summary>
        /// Adds a new link to feed metadata.
        /// </summary>
        /// <param name="feedMetadata">The feed metadata to add the link to.</param>
        /// <param name="linkMetadata">The link metadata to add.</param>
        internal static void AddLink(this AtomFeedMetadata feedMetadata, AtomLinkMetadata linkMetadata)
        {
            Debug.Assert(feedMetadata != null, "feedMetadata != null");
            Debug.Assert(linkMetadata != null, "linkMetadata != null");

            feedMetadata.Links = feedMetadata.Links.ConcatToReadOnlyEnumerable("Ref", linkMetadata);
        }

        /// <summary>
        /// Adds a new category to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the category to.</param>
        /// <param name="categoryMetadata">The category metadata to add.</param>
        internal static void AddCategory(this AtomEntryMetadata entryMetadata, AtomCategoryMetadata categoryMetadata)
        {
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(categoryMetadata != null, "categoryMetadata != null");

            entryMetadata.Categories = entryMetadata.Categories.ConcatToReadOnlyEnumerable("Categories", categoryMetadata);
        }

        /// <summary>
        /// Adds a new category to feed metadata.
        /// </summary>
        /// <param name="feedMetadata">The feed metadata to add the category to.</param>
        /// <param name="categoryMetadata">The category metadata to add.</param>
        internal static void AddCategory(this AtomFeedMetadata feedMetadata, AtomCategoryMetadata categoryMetadata)
        {
            Debug.Assert(feedMetadata != null, "feedMetadata != null");
            Debug.Assert(categoryMetadata != null, "categoryMetadata != null");

            feedMetadata.Categories = feedMetadata.Categories.ConcatToReadOnlyEnumerable("Categories", categoryMetadata);
        }

        /// <summary>
        /// Adds a new author to feed metadata.
        /// </summary>
        /// <param name="feedMetadata">The feed metadata to add the author to.</param>
        /// <param name="authorMetadata">The author metadata to add.</param>
        internal static void AddAuthor(this AtomFeedMetadata feedMetadata, AtomPersonMetadata authorMetadata)
        {
            Debug.Assert(feedMetadata != null, "feedMetadata != null");
            Debug.Assert(authorMetadata != null, "authorMetadata != null");

            feedMetadata.Authors = feedMetadata.Authors.ConcatToReadOnlyEnumerable("Authors", authorMetadata);
        }

        /// <summary>
        /// Adds a new contributor to feed metadata.
        /// </summary>
        /// <param name="feedMetadata">The feed metadata to add the contributor to.</param>
        /// <param name="contributorMetadata">The author metadata to add.</param>
        internal static void AddContributor(this AtomFeedMetadata feedMetadata, AtomPersonMetadata contributorMetadata)
        {
            Debug.Assert(feedMetadata != null, "feedMetadata != null");
            Debug.Assert(contributorMetadata != null, "contributorMetadata != null");

            feedMetadata.Contributors = feedMetadata.Contributors.ConcatToReadOnlyEnumerable("Contributors", contributorMetadata);
        }
    }
}
