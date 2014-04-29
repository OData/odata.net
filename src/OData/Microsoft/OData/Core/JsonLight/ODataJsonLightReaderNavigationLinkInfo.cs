//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class which holds information about navigation link to be reported by the reader.
    /// </summary>
    internal sealed class ODataJsonLightReaderNavigationLinkInfo
    {
        /// <summary>
        /// The navigation link to report.
        /// </summary>
        private readonly ODataNavigationLink navigationLink;

        /// <summary>
        /// The navigation property for which the link will be reported.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// true if the navigation link has a value (is expanded).
        /// </summary>
        private readonly bool isExpanded;

        /// <summary>
        /// The expanded feed for expanded navigation link to be reported.
        /// </summary>
        private ODataFeed expandedFeed;

        /// <summary>
        /// List of entity reference links to be reported to the navigation link.
        /// </summary>
        /// <remarks>
        /// If the navigation link is a singleton this will hold up to 1 item.
        /// If the navigation link is a collection this will hold any number of items.
        /// When the entity reference link is reported it is removed from this list.
        /// </remarks>
        private LinkedList<ODataEntityReferenceLink> entityReferenceLinks;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        private ODataJsonLightReaderNavigationLinkInfo(ODataNavigationLink navigationLink, IEdmNavigationProperty navigationProperty, bool isExpanded)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationProperty == null || navigationProperty.Name == navigationLink.Name, "The name of the navigation link doesn't match the name of the property.");

            this.navigationLink = navigationLink;
            this.navigationProperty = navigationProperty;
            this.isExpanded = isExpanded;
        }

        /// <summary>
        /// The navigation link to report.
        /// </summary>
        internal ODataNavigationLink NavigationLink
        {
            get
            {
                return this.navigationLink;
            }
        }

        /// <summary>
        /// The navigation property for which the link will be reported.
        /// </summary>
        internal IEdmNavigationProperty NavigationProperty
        {
            get
            {
                return this.navigationProperty;
            }
        }

        /// <summary>
        /// true if the navigation link is expanded (has a value).
        /// </summary>
        internal bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
        }

        /// <summary>
        /// The expanded feed for expanded navigation link to be reported.
        /// </summary>
        internal ODataFeed ExpandedFeed
        {
            get
            {
                return this.expandedFeed;
            }
        }

        /// <summary>
        /// true if the link info has entity reference link which was not yet reported, false otherwise.
        /// </summary>
        internal bool HasEntityReferenceLink
        {
            get
            {
                return this.entityReferenceLinks != null && this.entityReferenceLinks.First != null;
            }
        }

        /// <summary>
        /// Creates a navigation link info for a deferred link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateDeferredLinkInfo(
            ODataNavigationLink navigationLink,
            IEdmNavigationProperty navigationProperty)
        {
            return new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, /*isExpanded*/ false);
        }

        /// <summary>
        /// Creates a navigation link info for an expanded entry link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateExpandedEntryLinkInfo(
            ODataNavigationLink navigationLink,
            IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.IsCollection == false, "Expanded entry can only be reported for a singleton navigation link.");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, /*isExpanded*/ true);
            return navigationLinkInfo;
        }

        /// <summary>
        /// Creates a navigation link info for an expanded feed link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="expandedFeed">The expanded feed for the navigation link to report.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateExpandedFeedLinkInfo(
            ODataNavigationLink navigationLink,
            IEdmNavigationProperty navigationProperty,
            ODataFeed expandedFeed)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.IsCollection == true, "Expanded feeds can only be reported for collection navigation links.");
            Debug.Assert(expandedFeed != null, "expandedFeed != null");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, /*isExpanded*/ true);
            navigationLinkInfo.expandedFeed = expandedFeed;
            return navigationLinkInfo;
        }

        /// <summary>
        /// Creates a navigation link info for a singleton entity reference link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="entityReferenceLink">The entity reference link for the navigation link to report.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateSingletonEntityReferenceLinkInfo(
            ODataNavigationLink navigationLink,
            IEdmNavigationProperty navigationProperty,
            ODataEntityReferenceLink entityReferenceLink,
            bool isExpanded)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.IsCollection == false, "Singleton entity reference can only be reported for a singleton navigation links.");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, isExpanded);
            if (entityReferenceLink != null)
            {
                navigationLinkInfo.entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();
                navigationLinkInfo.entityReferenceLinks.AddFirst(entityReferenceLink);
            }

            return navigationLinkInfo;
        }

        /// <summary>
        /// Creates a navigation link info for a collection of entity reference links.
        /// </summary>
        /// <param name="navigationLink">The navigation link to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="entityReferenceLinks">The entity reference links for the navigation link to report.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateCollectionEntityReferenceLinksInfo(
            ODataNavigationLink navigationLink,
            IEdmNavigationProperty navigationProperty,
            LinkedList<ODataEntityReferenceLink> entityReferenceLinks,
            bool isExpanded)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.IsCollection == true, "Collection entity reference can only be reported for a collection navigation links.");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");
            Debug.Assert(entityReferenceLinks == null || entityReferenceLinks.Count > 0, "entityReferenceLinks == null || entityReferenceLinks.Count > 0");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, isExpanded);
            navigationLinkInfo.entityReferenceLinks = entityReferenceLinks;
            return navigationLinkInfo;
        }

        /// <summary>
        /// Creates a navigation link info for a projected navigation link that is missing from the payload.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <returns>The navigation link info created.</returns>
        internal static ODataJsonLightReaderNavigationLinkInfo CreateProjectedNavigationLinkInfo(IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNavigationLink navigationLink = new ODataNavigationLink { Name = navigationProperty.Name, IsCollection = navigationProperty.Type.IsCollection() };
            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = new ODataJsonLightReaderNavigationLinkInfo(navigationLink, navigationProperty, /*isExpanded*/ false);
            return navigationLinkInfo;
        }

        /// <summary>
        /// Gets the next entity reference link to report and removes it from the internal storage.
        /// </summary>
        /// <returns>The entity reference link to report or null.</returns>
        internal ODataEntityReferenceLink ReportEntityReferenceLink()
        {
            if (this.entityReferenceLinks != null && this.entityReferenceLinks.First != null)
            {
                ODataEntityReferenceLink entityRefernceLink = this.entityReferenceLinks.First.Value;
                this.entityReferenceLinks.RemoveFirst();
                return entityRefernceLink;
            }

            return null;
        }
    }
}
