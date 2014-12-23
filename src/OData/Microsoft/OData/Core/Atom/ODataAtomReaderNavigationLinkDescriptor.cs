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
