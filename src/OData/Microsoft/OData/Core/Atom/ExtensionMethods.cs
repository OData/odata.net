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

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Atom specific extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>Determines an extension method to get the <see cref="T:Microsoft.OData.Core.Atom.AtomEntryMetadata" /> for an annotatable entry.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Core.Atom.AtomEntryMetadata" /> instance or null if no annotation of that type exists.</returns>
        /// <param name="entry">The entry instance to get the annotation from.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "ODataEntry subtype is intentional here since we want to return an AtomEntryMetadata.")]
        public static AtomEntryMetadata Atom(this ODataEntry entry)
        {
            ExceptionUtils.CheckArgumentNotNull(entry, "entry");

            AtomEntryMetadata entryMetadata = entry.GetAnnotation<AtomEntryMetadata>();
            if (entryMetadata == null)
            {
                entryMetadata = new AtomEntryMetadata();
                entry.SetAnnotation(entryMetadata);
            }

            return entryMetadata;
        }

        /// <summary>Determines an extension method to get the <see cref="T:Microsoft.OData.Core.Atom.AtomFeedMetadata" /> for an annotatable feed.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Core.Atom.AtomFeedMetadata" /> instance or null if no annotation of that type exists.</returns>
        /// <param name="feed">The feed instance to get the annotation from.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "ODataFeed subtype is intentional here since we want to return an AtomFeedMetadata.")]
        public static AtomFeedMetadata Atom(this ODataFeed feed)
        {
            ExceptionUtils.CheckArgumentNotNull(feed, "feed");

            AtomFeedMetadata feedMetadata = feed.GetAnnotation<AtomFeedMetadata>();
            if (feedMetadata == null)
            {
                feedMetadata = new AtomFeedMetadata();
                feed.SetAnnotation(feedMetadata);
            }

            return feedMetadata;
        }

        /// <summary>Determines an extension method to get the <see cref="T:Microsoft.OData.Core.Atom.AtomLinkMetadata" /> for an annotatable navigation link.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Core.Atom.AtomLinkMetadata" /> instance or null if no annotation of that type exists.</returns>
        /// <param name="navigationLink">The navigation link instance to get the annotation from.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "ODataNavigationLink subtype is intentional here since we want to return an AtomLinkMetadata.")]
        public static AtomLinkMetadata Atom(this ODataNavigationLink navigationLink)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationLink, "navigationLink");

            AtomLinkMetadata linkMetadata = navigationLink.GetAnnotation<AtomLinkMetadata>();
            if (linkMetadata == null)
            {
                linkMetadata = new AtomLinkMetadata();
                navigationLink.SetAnnotation(linkMetadata);
            }

            return linkMetadata;
        }

        /// <summary>Determines an extension method to get the <see cref="T:System.Data.OData.Atom.AtomWorkspaceMetadata" /> for an annotatable serviceDocument.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Core.Atom.AtomWorkspaceMetadata" /> instance or null if no annotation of that type exists.</returns>
        /// <param name="serviceDocument">The serviceDocument to get the annotation from.</param>
        public static AtomWorkspaceMetadata Atom(this ODataServiceDocument serviceDocument)
        {
            ExceptionUtils.CheckArgumentNotNull(serviceDocument, "serviceDocument");

            AtomWorkspaceMetadata workspaceMetadata = serviceDocument.GetAnnotation<AtomWorkspaceMetadata>();
            if (workspaceMetadata == null)
            {
                workspaceMetadata = new AtomWorkspaceMetadata();
                serviceDocument.SetAnnotation(workspaceMetadata);
            }

            return workspaceMetadata;
        }

        /// <summary>Determines an extension method to get the <see cref="T:Microsoft.OData.Core.Atom.AtomResourceCollectionMetadata" /> for an entity set in service document.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Core.Atom.AtomResourceCollectionMetadata" /> instance or null if no annotation of that type exists.</returns>
        /// <param name="entitySet">The entity set in service document to get the annotation from.</param>
        public static AtomResourceCollectionMetadata Atom(this ODataEntitySetInfo entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(entitySet, "entitySet");

            AtomResourceCollectionMetadata collectionMetadata = entitySet.GetAnnotation<AtomResourceCollectionMetadata>();
            if (collectionMetadata == null)
            {
                collectionMetadata = new AtomResourceCollectionMetadata();
                entitySet.SetAnnotation(collectionMetadata);
            }

            return collectionMetadata;
        }

        // TODO: Association Link - Add back support for customizing association link element in Atom
    }
}
