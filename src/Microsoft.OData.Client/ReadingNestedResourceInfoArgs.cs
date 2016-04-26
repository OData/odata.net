//---------------------------------------------------------------------
// <copyright file="ReadingNestedResourceInfoArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData;

    /// <summary>
    /// The reading navigation link arguments
    /// </summary>
    public sealed class ReadingNestedResourceInfoArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadingNestedResourceInfoArgs" /> class.
        /// </summary>
        /// <param name="link">The link.</param>
        public ReadingNestedResourceInfoArgs(ODataNestedResourceInfo link)
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
        public ODataNestedResourceInfo Link { get; private set; }
    }
}
