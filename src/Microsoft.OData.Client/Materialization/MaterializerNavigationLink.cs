//---------------------------------------------------------------------
// <copyright file="MaterializerNavigationLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Materializer state for a given ODataNestedResourceInfo
    /// </summary>
    internal class MaterializerNavigationLink
    {
        /// <summary>The navigation link.</summary>
        private readonly ODataNestedResourceInfo link;

        /// <summary>An object field for the feed or enty.</summary>
        private readonly object feedOrEntry;

        /// <summary>
        /// Prevents a default instance of the <see cref="MaterializerNavigationLink"/> struct from being created.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="materializedFeedOrEntry">Value of the link.</param>
        private MaterializerNavigationLink(ODataNestedResourceInfo link, object materializedFeedOrEntry)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(materializedFeedOrEntry != null, "materializedFeedOrEntry != null");
            Debug.Assert(materializedFeedOrEntry is MaterializerEntry || materializedFeedOrEntry is ODataResourceSet, "must be feed or entry");
            this.link = link;
            this.feedOrEntry = materializedFeedOrEntry;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        public ODataNestedResourceInfo Link
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
        public ODataResourceSet Feed
        {
            get { return this.feedOrEntry as ODataResourceSet; }
        }

        /// <summary>
        /// Creates the materializer link with an entry.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink CreateLink(ODataNestedResourceInfo link, MaterializerEntry entry)
        {
            Debug.Assert(link.GetAnnotation<MaterializerNavigationLink>() == null, "there should be no MaterializerNestedResourceInfo annotation on the entry link yet");
            MaterializerNavigationLink materializedNestedResourceInfo = new MaterializerNavigationLink(link, entry);
            link.SetAnnotation<MaterializerNavigationLink>(materializedNestedResourceInfo);
            return materializedNestedResourceInfo;
        }

        /// <summary>
        /// Creates the materializer link with a resource set.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="resourceSet">The resource set.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink CreateLink(ODataNestedResourceInfo link, ODataResourceSet resourceSet)
        {
            Debug.Assert(link.GetAnnotation<MaterializerNavigationLink>() == null, "there should be no MaterializerNestedResourceInfo annotation on the feed link yet");
            MaterializerNavigationLink materializedNestedResourceInfo = new MaterializerNavigationLink(link, resourceSet);
            link.SetAnnotation<MaterializerNavigationLink>(materializedNestedResourceInfo);
            return materializedNestedResourceInfo;
        }

        /// <summary>
        /// Gets the materializer link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>The materializer link.</returns>
        public static MaterializerNavigationLink GetLink(ODataNestedResourceInfo link)
        {
            return link.GetAnnotation<MaterializerNavigationLink>();
        }
    }
}