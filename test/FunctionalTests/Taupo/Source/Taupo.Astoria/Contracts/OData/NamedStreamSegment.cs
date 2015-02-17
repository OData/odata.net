//---------------------------------------------------------------------
// <copyright file="NamedStreamSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a segment who's type is unknown or not present in the service metadata
    /// </summary>
    public class NamedStreamSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the NamedStreamSegment class
        /// </summary>
        /// <param name="value">The segment's value</param>
        internal NamedStreamSegment(string value)
            : base()
        {
            this.Name = value;
        }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.NamedStream; }
        }

        /// <summary>
        /// Gets the string value of the segment
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
