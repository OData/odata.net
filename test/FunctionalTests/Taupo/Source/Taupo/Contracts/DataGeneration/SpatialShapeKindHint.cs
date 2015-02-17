//---------------------------------------------------------------------
// <copyright file="SpatialShapeKindHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using Microsoft.Test.Taupo.Contracts.Spatial;

    /// <summary>
    /// Represents a hint to generate certiain type of spatial types.
    /// </summary>
    public sealed class SpatialShapeKindHint : DataGenerationHint<SpatialShapeKind>
    {
        /// <summary>
        /// Initializes a new instance of the SpatialShapeKindHint class.
        /// </summary>
        /// <param name="value">The shape kind supported by openGIS.</param>
        /// <example>one of {Geometry, GeographyPoint, Curve, GeographyLineString, Surface, GeographyPolygon, PolyhedralSurface GeographyCollection, MultiCurve, GeographyMultiLineString, MultiSurface, GeographyMultiPolygon, and GeographyMultiPoint}</example>
        internal SpatialShapeKindHint(SpatialShapeKind value)
            : base(value)
        {
        }
    }
}
