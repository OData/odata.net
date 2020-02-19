//---------------------------------------------------------------------
// <copyright file="GeometrySurface.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents a geometry surface.</summary>
    public abstract class GeometrySurface : Geometry
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GeometrySurface(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }
    }
}