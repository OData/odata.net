//---------------------------------------------------------------------
// <copyright file="ReadingNavigationLinkArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData.Core;

    /// <summary>
    /// The reading navigation link arguments
    /// </summary>
    public sealed class ReadingNavigationLinkArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadingNavigationLinkArgs" /> class.
        /// </summary>
        /// <param name="link">The link.</param>
        public ReadingNavigationLinkArgs(ODataNavigationLink link)
        {
            Util.CheckArgumentNull(link, "link");
            this.Link = link;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <value>
        /// The link.
        /// </value>
        public ODataNavigationLink Link { get; private set; }
    }
}
