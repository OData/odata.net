//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
