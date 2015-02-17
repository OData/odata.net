//---------------------------------------------------------------------
// <copyright file="NavigationSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A segment that corresponds to a navigation property in the metadata
    /// </summary>
    public class NavigationSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the NavigationSegment class
        /// </summary>
        /// <param name="property">The navigation property</param>
        internal NavigationSegment(NavigationProperty property)
            : base()
        {
            this.NavigationProperty = property;
        }

        /// <summary>
        /// Gets the navigation property the segment corresponds to
        /// </summary>
        public NavigationProperty NavigationProperty { get; private set; }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.NavigationProperty; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
