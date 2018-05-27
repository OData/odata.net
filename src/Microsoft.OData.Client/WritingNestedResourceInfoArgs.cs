//---------------------------------------------------------------------
// <copyright file="WritingNestedResourceInfoArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData;

    /// <summary>
    /// Writing navigation link arguments
    /// </summary>
    public sealed class WritingNestedResourceInfoArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WritingNestedResourceInfoArgs"/> class.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public WritingNestedResourceInfoArgs(ODataNestedResourceInfo link, object source, object target)
        {
            Util.CheckArgumentNull(link, "link");
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(target, "target");
            this.Link = link;
            this.Source = source;
            this.Target = target;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <value>
        /// The link.
        /// </value>
        public ODataNestedResourceInfo Link { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public object Target { get; private set; }
    }
}
