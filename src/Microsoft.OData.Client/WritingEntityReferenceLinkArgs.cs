//---------------------------------------------------------------------
// <copyright file="WritingEntityReferenceLinkArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData;

    /// <summary>
    /// The writing entity reference link arguments
    /// </summary>
    public sealed class WritingEntityReferenceLinkArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WritingEntityReferenceLinkArgs"/> class.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public WritingEntityReferenceLinkArgs(ODataEntityReferenceLink entityReferenceLink, object source, object target)
        {
            Util.CheckArgumentNull(entityReferenceLink, "entityReferenceLink");
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(target, "target");
            this.EntityReferenceLink = entityReferenceLink;
            this.Source = source;
            this.Target = target;
        }

        /// <summary>
        /// Gets the feed.
        /// </summary>
        /// <value>
        /// The feed.
        /// </value>
        public ODataEntityReferenceLink EntityReferenceLink { get; private set; }

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
