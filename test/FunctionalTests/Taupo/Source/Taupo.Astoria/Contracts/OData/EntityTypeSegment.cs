//---------------------------------------------------------------------
// <copyright file="EntityTypeSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// A cast in the path segment of the URI.
    /// </summary>
    public class EntityTypeSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the EntityTypeSegment class
        /// </summary>
        /// <param name="value">The segment's value</param>
        internal EntityTypeSegment(EntityType value)
            : base()
        {
            this.EntityType = value;
        }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.EntityType; }
        }

        /// <summary>
        /// Gets the Type to filter against
        /// </summary>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
