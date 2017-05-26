//---------------------------------------------------------------------
// <copyright file="ODataEntryNavigationLinksObjectModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// An OData object model annotation for entries to capture all the navigation links of the entry.
    /// </summary>
    public sealed class ODataEntryNavigationLinksObjectModelAnnotation
    {
        /// <summary>
        /// A dictionary of all the navigation links keyed off the position among the list of all properties where the navigation navigationLink was read.
        /// </summary>
        private readonly Dictionary<int, ODataNestedResourceInfo> navigationLinks = new Dictionary<int, ODataNestedResourceInfo>();

        /// <summary>
        /// The number of navigation links stored in this annotation.
        /// </summary>
        public int Count
        {
            get
            {
                return this.navigationLinks.Count;
            }
        }

        /// <summary>
        /// The navigation links stored in this annotation.
        /// </summary>
        public IEnumerable<ODataNestedResourceInfo> NavigationLinks
        {
            get
            {
                return this.navigationLinks.Values;
            }
        }

        /// <summary>
        /// Add a new navigation link and its position in the list of properties from the payload to the annotation.
        /// </summary>
        /// <param name="navigationLink">The navigation link read from the payload.</param>
        /// <param name="position">The position of the navigation link among all the properties being from the payload.</param>
        public void Add(ODataNestedResourceInfo link, int position)
        {
            this.navigationLinks.Add(position, link);
        }

        /// <summary>
        /// Try to get the navigation link for the specified position.
        /// </summary>
        /// <param name="position">The position to get the navigation link for.</param>
        /// <param name="navigationLink">The navigation link for the specified position or null if none exists for that position.</param>
        /// <returns>true if navigation link was found for the specified position; otherwise false.</returns>
        public bool TryGetNavigationLinkAt(int position, out ODataNestedResourceInfo link)
        {
            return this.navigationLinks.TryGetValue(position, out link);
        }
    }
}
