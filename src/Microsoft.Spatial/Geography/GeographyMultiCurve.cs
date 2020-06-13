//---------------------------------------------------------------------
// <copyright file="GeographyMultiCurve.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the multi-curve of geography.</summary>
    public abstract class GeographyMultiCurve : GeographyCollection
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyMultiCurve" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyMultiCurve(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }
    }
}