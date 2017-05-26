//---------------------------------------------------------------------
// <copyright file="ObjectModelExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Extension methods used by the reader tests for the OData object model.
    /// </summary>
    public static class ObjectModelExtensions
    {
        /// <summary>
        /// Gets all navigation link instances attached to the <paramref name="entry"/> via the
        /// <see cref="ODataEntryNavigationLinksObjectModelAnnotation"/> annotation.
        /// </summary>
        /// <param name="entry">The <see cref="ODataResource"/> to get the navigation links for.</param>
        /// <returns>The annotation representing the navigation link instances or null if none were found.</returns>
        public static ODataEntryNavigationLinksObjectModelAnnotation NavigationLinks(this ODataResource entry)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");
            return entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
        }

        /// <summary>
        /// Gets all entries attached to the <paramref name="feed"/> via the
        /// <see cref="ODataFeedEntriesObjectModelAnnotation"/> annotation.
        /// </summary>
        /// <param name="feed">The <see cref="ODataResourceSet"/> to get the entries for.</param>
        /// <returns>A list of entries or null if none were found.</returns>
        public static IList<ODataResource> Entries(this ODataResourceSet resourceCollection)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCollection, "feed");
            return resourceCollection.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
        }

        /// <summary>
        /// Gets the expanded content of a navigation link.
        /// </summary>
        /// <param name="navigationLink">The <see cref="ODataNestedResourceInfo"/> to get the navigation content for.</param>
        /// <param name="expandedContent">The expanded content (if the method returns null), which can be either 
        /// null (null expanded entry), or <see cref="ODataResource"/> or <see cref="ODataResourceSet"/>.</param>
        /// <returns>true if the <paramref name="navigationLink"/> is expanded, or false otherwise.</returns>
        public static bool TryGetExpandedContent(this ODataNestedResourceInfo navigationLink, out object expandedContent)
        {
            ExceptionUtilities.CheckArgumentNotNull(navigationLink, "navigationLink");
            var expandedItemAnnotation = navigationLink.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
            if (expandedItemAnnotation != null)
            {
                expandedContent = expandedItemAnnotation.ExpandedItem;
                return true;
            }
            else
            {
                expandedContent = null;
                return false;
            }
        }

        /// <summary>
        /// Process all properties (regular and navigation properties) while preserving their order in
        /// the payload.
        /// </summary>
        /// <param name="entry">The <see cref="ODataResource"/> to process the properties for.</param>
        public static void ProcessPropertiesPreservingPayloadOrder(this ODataResource entry, Action<ODataProperty> propertyAction, Action<ODataNestedResourceInfo> navigationLinkAction)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");
            ExceptionUtilities.CheckArgumentNotNull(propertyAction, "propertyAction");
            ExceptionUtilities.CheckArgumentNotNull(navigationLinkAction, "navigationLinkAction");

            ODataEntryNavigationLinksObjectModelAnnotation navigationLinks = entry.NavigationLinks();
            if (navigationLinks == null)
            {
                navigationLinks = new ODataEntryNavigationLinksObjectModelAnnotation();
            }

            int navigationLinksProcessed = 0;

            // NOTE we iterate over all the properties here just to get the count; since this is test code only we don't care
            int navigationLinkCount = navigationLinks == null ? 0 : navigationLinks.Count;
            int totalPropertyCount = entry.Properties.Count() + navigationLinkCount;

            // NOTE: we are preserving the same order of items as in the payload, i.e., if regular
            //       properties and navigation properties are interleaved we preserve their relative order.
            using (IEnumerator<ODataProperty> propertyEnumerator = entry.Properties.GetEnumerator())
            {
                ODataNestedResourceInfo navigationLink;
                for (int i = 0; i < totalPropertyCount; ++i)
                {
                    if (navigationLinks != null && navigationLinks.TryGetNavigationLinkAt(i, out navigationLink))
                    {
                        navigationLinkAction(navigationLink);
                        navigationLinksProcessed++;
                    }
                    else
                    {
                        bool hasMore = propertyEnumerator.MoveNext();
                        Debug.Assert(hasMore, "More properties were expected.");
                        propertyAction(propertyEnumerator.Current);
                    }
                }
            }

            Debug.Assert(navigationLinksProcessed == navigationLinkCount, "All navigation links should have been processed.");
        }
    }
}
