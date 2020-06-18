//---------------------------------------------------------------------
// <copyright file="GeographyMultiPolygon.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Represents the multi-polygon of geography.</summary>
    public abstract class GeographyMultiPolygon : GeographyMultiSurface
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyMultiPolygon" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyMultiPolygon(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets a collection of polygons.</summary>
        /// <returns>A collection of polygons.</returns>
        public abstract ReadOnlyCollection<GeographyPolygon> Polygons { get; }

        /// <summary>Determines whether this instance and another specified geography instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeographyMultiPolygon other)
        {
            return this.BaseEquals(other) ?? this.Polygons.SequenceEqual(other.Polygons);
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyMultiPolygon);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Microsoft.Spatial.Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Polygons);
        }
    }
}