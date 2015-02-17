//---------------------------------------------------------------------
// <copyright file="ODataAtomReaderNavigationLinkDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Instance of this class describes a navigation link when it's found in the payload.
    /// </summary>
    internal sealed class ODataAtomReaderNavigationLinkDescriptor
    {
        /// <summary>The navigation link.</summary>
        private ODataNavigationLink navigationLink;

        /// <summary>The navigation property for the link, is it's available.</summary>
        private IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        /// <param name="navigationProperty">The navigation property for the link, if it's available.</param>
        internal ODataAtomReaderNavigationLinkDescriptor(ODataNavigationLink navigationLink, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            this.navigationLink = navigationLink;
            this.navigationProperty = navigationProperty;
        }

        /// <summary>The navigation link.</summary>
        internal ODataNavigationLink NavigationLink
        {
            get
            {
                return this.navigationLink;
            }
        }

        /// <summary>The navigation property for the link, if it's available.</summary>
        internal IEdmNavigationProperty NavigationProperty
        {
            get
            {
                return this.navigationProperty;
            }
        }
    }
}