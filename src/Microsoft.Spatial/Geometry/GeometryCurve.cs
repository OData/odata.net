//---------------------------------------------------------------------
// <copyright file="GeometryCurve.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the geometry curve.</summary>
    public abstract class GeometryCurve : Geometry
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeometryCurve" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryCurve(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }
    }
}