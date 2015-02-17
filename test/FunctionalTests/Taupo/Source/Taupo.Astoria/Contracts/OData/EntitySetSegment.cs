//---------------------------------------------------------------------
// <copyright file="EntitySetSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A segment that refers to a top-level entity set
    /// </summary>
    public class EntitySetSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetSegment class
        /// </summary>
        /// <param name="set">The set this segment maps to</param>
        internal EntitySetSegment(EntitySet set)
            : base()
        {
            this.EntitySet = set;
        }

        /// <summary>
        /// Initializes a new instance of the EntitySetSegment class
        /// </summary>
        /// <param name="set">The set this segment maps to</param>
        internal EntitySetSegment(IEdmEntitySet set)
            : base()
        {
            this.EdmEntitySet = set;
        }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.EntitySet; }
        }

        /// <summary>
        /// Gets the set this segment maps to
        /// </summary>
        public IEdmEntitySet EdmEntitySet { get; private set; }

        /// <summary>
        /// Gets the set this segment maps to
        /// </summary>
        public EntitySet EntitySet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
