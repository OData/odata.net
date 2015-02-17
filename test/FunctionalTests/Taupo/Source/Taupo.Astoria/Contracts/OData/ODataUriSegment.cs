//---------------------------------------------------------------------
// <copyright file="ODataUriSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a metadata-rich segment in an OData uri
    /// </summary>
    public abstract class ODataUriSegment
    {
        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public abstract ODataUriSegmentType SegmentType { get; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal abstract bool HasPrecedingSlash { get; }
    }
}
