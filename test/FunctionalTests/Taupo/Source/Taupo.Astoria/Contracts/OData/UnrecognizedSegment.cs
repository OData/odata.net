//---------------------------------------------------------------------
// <copyright file="UnrecognizedSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a segment who's type is unknown or not present in the service metadata
    /// </summary>
    public class UnrecognizedSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the UnrecognizedSegment class
        /// </summary>
        /// <param name="value">The segment's value</param>
        internal UnrecognizedSegment(string value)
            : base()
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.Unrecognized; }
        }

        /// <summary>
        /// Gets the string value of the segment
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
