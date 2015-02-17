//---------------------------------------------------------------------
// <copyright file="SpatialShapeKindFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using Microsoft.Test.Taupo.Contracts.Spatial;

    /// <summary>
    /// Shape kind of a spatial type (such as point or linestring).
    /// </summary>
    public class SpatialShapeKindFacet : PrimitiveDataTypeFacet<SpatialShapeKind>
    {
        /// <summary>
        /// Initializes a new instance of the SpatialShapeKindFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SpatialShapeKindFacet(SpatialShapeKind value)
            : base(value)
        {
        }
    }
}
