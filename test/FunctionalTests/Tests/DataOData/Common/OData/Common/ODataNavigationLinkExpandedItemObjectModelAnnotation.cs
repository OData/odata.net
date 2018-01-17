//---------------------------------------------------------------------
// <copyright file="ODataNavigationLinkExpandedItemObjectModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// An OData object model annotation for navigation link to capture its expanded value.
    /// </summary>
    /// <remarks>Non expanded links don't have this annotation at all.</remarks>
    public sealed class ODataNavigationLinkExpandedItemObjectModelAnnotation
    {
        /// <summary>
        /// The expanded item.
        /// This can be either:
        /// - ODataResourceSet - for expanded feed
        /// - ODataResource - for expanded entry
        /// - null - for null expanded entry
        /// - ODataEntityReferenceLink - for entity reference link in requests
        /// - List of ODataItem - for deep bindings in request, the list may contain ODataEntityReferenceLink or ODataResourceSet instances.
        /// </summary>
        public object ExpandedItem { get; set; }
    }
}
