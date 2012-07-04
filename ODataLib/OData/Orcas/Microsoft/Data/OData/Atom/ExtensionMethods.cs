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
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Atom specific extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method to get the <see cref="AtomEntryMetadata" /> annotation for an annotatable entry.
        /// </summary>
        /// <param name="entry">The entry instance to get the annotation from.</param>
        /// <returns>An <see cref="AtomEntryMetadata" /> instance or null if no annotation of that type exists.</returns>
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

        /// <summary>
        /// Extension method to get the <see cref="AtomFeedMetadata" /> for an annotatable feed.
        /// </summary>
        /// <param name="feed">The feed instance to get the annotation from.</param>
        /// <returns>An <see cref="AtomFeedMetadata" /> instance or null if no annotation of that type exists.</returns>
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

        /// <summary>
        /// Extension method to get the <see cref="AtomLinkMetadata" /> for an annotatable navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link instance to get the annotation from.</param>
        /// <returns>An <see cref="AtomLinkMetadata" /> instance or null if no annotation of that type exists.</returns>
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

        /// <summary>
        /// Extension method to get the <see cref="AtomWorkspaceMetadata"/> for an annotatable workspace.
        /// </summary>
        /// <param name="workspace">The workspace to get the annotation from.</param>
        /// <returns>An <see cref="AtomWorkspaceMetadata" /> instance or null if no annotation of that type exists.</returns>
        public static AtomWorkspaceMetadata Atom(this ODataWorkspace workspace)
        {
            ExceptionUtils.CheckArgumentNotNull(workspace, "workspace");

            AtomWorkspaceMetadata workspaceMetadata = workspace.GetAnnotation<AtomWorkspaceMetadata>();
            if (workspaceMetadata == null)
            {
                workspaceMetadata = new AtomWorkspaceMetadata();
                workspace.SetAnnotation(workspaceMetadata);
            }

            return workspaceMetadata;
        }

        /// <summary>
        /// Extension method to get the <see cref="AtomResourceCollectionMetadata"/> for an annotatable (resource) collection.
        /// </summary>
        /// <param name="collection">The (resource) collection to get the annotation from.</param>
        /// <returns>An <see cref="AtomResourceCollectionMetadata" /> instance or null if no annotation of that type exists.</returns>
        public static AtomResourceCollectionMetadata Atom(this ODataResourceCollectionInfo collection)
        {
            ExceptionUtils.CheckArgumentNotNull(collection, "collection");

            AtomResourceCollectionMetadata collectionMetadata = collection.GetAnnotation<AtomResourceCollectionMetadata>();
            if (collectionMetadata == null)
            {
                collectionMetadata = new AtomResourceCollectionMetadata();
                collection.SetAnnotation(collectionMetadata);
            }

            return collectionMetadata;
        }

        /// <summary>
        /// Extension method to get the <see cref="AtomLinkMetadata"/> for an association link.
        /// </summary>
        /// <param name="associationLink">The association link to get the annotation from.</param>
        /// <returns>An <see cref="AtomLinkMetadata" /> instance or null if no annotation of that type exists.</returns>
        public static AtomLinkMetadata Atom(this ODataAssociationLink associationLink)
        {
            ExceptionUtils.CheckArgumentNotNull(associationLink, "associationLink");

            AtomLinkMetadata linkMetadata = associationLink.GetAnnotation<AtomLinkMetadata>();
            if (linkMetadata == null)
            {
                linkMetadata = new AtomLinkMetadata();
                associationLink.SetAnnotation(linkMetadata);
            }

            return linkMetadata;
        }
    }
}
