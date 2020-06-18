//---------------------------------------------------------------------
// <copyright file="GeographyMultiSurface.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the multi-surface of geography.</summary>
    public abstract class GeographyMultiSurface : GeographyCollection
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyMultiSurface" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyMultiSurface(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }
    }
}