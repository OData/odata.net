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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.OData.Atom;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for reading ATOM metadata.
    /// </summary>
    internal static class AtomMetadataReaderUtils
    {
        /// <summary>
        /// Empty list of ATOM persons.
        /// </summary>
        private static readonly ReadOnlyEnumerable<AtomPersonMetadata> EmptyPersonsList = new ReadOnlyEnumerable<AtomPersonMetadata>();

        /// <summary>
        /// Empty list of ATOM categories.
        /// </summary>
        private static readonly ReadOnlyEnumerable<AtomCategoryMetadata> EmptyCategoriesList = new ReadOnlyEnumerable<AtomCategoryMetadata>();

        /// <summary>
        /// Empty list of ATOM links.
        /// </summary>
        private static readonly ReadOnlyEnumerable<AtomLinkMetadata> EmptyLinksList = new ReadOnlyEnumerable<AtomLinkMetadata>();

        /// <summary>
        /// Creates a new instance of ATOM entry metadata.
        /// </summary>
        /// <returns>The newly created ATOM entry metadata.</returns>
        internal static AtomEntryMetadata CreateNewAtomEntryMetadata()
        {
            DebugUtils.CheckNoExternalCallers();

            return new AtomEntryMetadata
            {
                Authors = EmptyPersonsList,
                Categories = EmptyCategoriesList,
                Contributors = EmptyPersonsList,
                Links = EmptyLinksList,
            };
        }

        /// <summary>
        /// Adds a new author to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the author to.</param>
        /// <param name="authorMetadata">The author metadata to add.</param>
        internal static void AddAuthorToEntryMetadata(AtomEntryMetadata entryMetadata, AtomPersonMetadata authorMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(authorMetadata != null, "authorMetadata != null");

            if (object.ReferenceEquals(entryMetadata.Authors, EmptyPersonsList))
            {
                entryMetadata.Authors = new ReadOnlyEnumerable<AtomPersonMetadata>();
            }

            ReaderUtils.GetSourceListOfEnumerable(entryMetadata.Authors, "Authors").Add(authorMetadata);
        }

        /// <summary>
        /// Adds a new contributor to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the contributor to.</param>
        /// <param name="contributorMetadata">The contributor metadata to add.</param>
        internal static void AddContributorToEntryMetadata(AtomEntryMetadata entryMetadata, AtomPersonMetadata contributorMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(contributorMetadata != null, "contributorMetadata != null");

            if (object.ReferenceEquals(entryMetadata.Contributors, EmptyPersonsList))
            {
                entryMetadata.Contributors = new ReadOnlyEnumerable<AtomPersonMetadata>();
            }

            ReaderUtils.GetSourceListOfEnumerable(entryMetadata.Contributors, "Contributors").Add(contributorMetadata);
        }

        /// <summary>
        /// Adds a new link to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the link to.</param>
        /// <param name="linkMetadata">The link metadata to add.</param>
        internal static void AddLinkToEntryMetadata(AtomEntryMetadata entryMetadata, AtomLinkMetadata linkMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(linkMetadata != null, "linkMetadata != null");

            if (object.ReferenceEquals(entryMetadata.Links, EmptyLinksList))
            {
                entryMetadata.Links = new ReadOnlyEnumerable<AtomLinkMetadata>();
            }

            ReaderUtils.GetSourceListOfEnumerable(entryMetadata.Links, "Links").Add(linkMetadata);
        }

        /// <summary>
        /// Adds a new category to entry metadata.
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to add the category to.</param>
        /// <param name="categoryMetadata">The category metadata to add.</param>
        internal static void AddCategoryToEntryMetadata(AtomEntryMetadata entryMetadata, AtomCategoryMetadata categoryMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            Debug.Assert(categoryMetadata != null, "categoryMetadata != null");

            if (object.ReferenceEquals(entryMetadata.Categories, EmptyCategoriesList))
            {
                entryMetadata.Categories = new ReadOnlyEnumerable<AtomCategoryMetadata>();
            }

            ReaderUtils.GetSourceListOfEnumerable(entryMetadata.Categories, "Categories").Add(categoryMetadata);
        }
    }
}
