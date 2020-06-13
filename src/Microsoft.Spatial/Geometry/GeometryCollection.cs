//---------------------------------------------------------------------
// <copyright file="GeometryCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Represents the geometry collection.</summary>
    public abstract class GeometryCollection : Geometry
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeometryCollection" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryCollection(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the geometry instances in this collection.</summary>
        /// <returns>A collection of geometries.</returns>
        public abstract ReadOnlyCollection<Geometry> Geometries { get; }

        /// <summary>Determines whether this instance and another specified geometry instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geometry to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeometryCollection other)
        {
            return this.BaseEquals(other) ?? this.Geometries.SequenceEqual(other.Geometries);
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeometryCollection);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Microsoft.Spatial.Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Geometries);
        }
    }
}