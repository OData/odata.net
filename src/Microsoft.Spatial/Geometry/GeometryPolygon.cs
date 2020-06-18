//---------------------------------------------------------------------
// <copyright file="GeometryPolygon.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Represents the Geometry polygon.</summary>
    public abstract class GeometryPolygon : GeometrySurface
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeometryPolygon" /> class.</summary>
        /// <param name="coordinateSystem">The CoordinateSystem.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryPolygon(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the set of rings.</summary>
        public abstract ReadOnlyCollection<GeometryLineString> Rings { get; }

        /// <summary> Determines whether this instance and another specified geography instance have the same value.  </summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeometryPolygon other)
        {
            return this.BaseEquals(other) ?? this.Rings.SequenceEqual(other.Rings);
        }

        /// <summary> Determines whether this instance and another specified geography instance have the same value.  </summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The geography to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeometryPolygon);
        }

        /// <summary>Indicates the Get Hashcode.</summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Rings);
        }
    }
}