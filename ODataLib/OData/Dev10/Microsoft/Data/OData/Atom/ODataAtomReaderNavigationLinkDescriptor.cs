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
    using System.Diagnostics;
    using Microsoft.Data.Edm;
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");

            this.navigationLink = navigationLink;
            this.navigationProperty = navigationProperty;
        }

        /// <summary>The navigation link.</summary>
        internal ODataNavigationLink NavigationLink
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.navigationLink;
            }
        }

        /// <summary>The navigation property for the link, if it's available.</summary>
        internal IEdmNavigationProperty NavigationProperty
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.navigationProperty;
            }
        }
    }
}
