//---------------------------------------------------------------------
// <copyright file="GeographyLineString.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Represents a geography line string consist of an array of geo points.</summary>
    public abstract class GeographyLineString : GeographyCurve
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyLineString" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyLineString(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the point list.</summary>
        /// <returns>The point list.</returns>
        public abstract ReadOnlyCollection<GeographyPoint> Points { get; }

        /// <summary>Determines whether this instance and another specified geography instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeographyLineString other)
        {
            return this.BaseEquals(other) ?? this.Points.SequenceEqual(other.Points);
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyLineString);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Points);
        }
    }
}