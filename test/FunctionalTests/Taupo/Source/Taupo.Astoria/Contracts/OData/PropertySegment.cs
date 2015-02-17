//---------------------------------------------------------------------
// <copyright file="PropertySegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// A segment corresponding to a member property in the metadata
    /// </summary>
    public class PropertySegment : ODataUriSegment
    {
        private ODataUriSegmentType segmentType;

        /// <summary>
        /// Initializes a new instance of the PropertySegment class
        /// </summary>
        /// <param name="property">The property this segment corresponds to</param>
        internal PropertySegment(MemberProperty property)
            : base()
        {
            this.Property = property;

            if (property.PropertyType is ComplexDataType)
            {
                this.segmentType = ODataUriSegmentType.ComplexProperty;
            }
            else if (property.PropertyType is CollectionDataType)
            {
                this.segmentType = ODataUriSegmentType.MultiValueProperty;
            }
            else
            {
                this.segmentType = ODataUriSegmentType.PrimitiveProperty;
            }
        }

        /// <summary>
        /// Gets the segment's type
        /// </summary>
        public override ODataUriSegmentType SegmentType 
        { 
            get 
            { 
                return this.segmentType; 
            } 
        }

        /// <summary>
        /// Gets the metadata property this segment corresponds to
        /// </summary>
        public MemberProperty Property { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
