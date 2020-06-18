//---------------------------------------------------------------------
// <copyright file="GeographyCurve.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the curve of geography.</summary>
    public abstract class GeographyCurve : Geography
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyCurve" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this geography curve.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyCurve(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }
    }
}