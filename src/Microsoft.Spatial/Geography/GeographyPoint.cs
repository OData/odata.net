//---------------------------------------------------------------------
// <copyright file="GeographyPoint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents a geography point.</summary>
    public abstract class GeographyPoint : Geography
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeographyPoint" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the latitude.</summary>
        /// <returns>The latitude.</returns>
        public abstract double Latitude { get; }

        /// <summary>Gets the longitude.</summary>
        /// <returns>The longitude.</returns>
        public abstract double Longitude { get; }

        /// <summary>Gets the nullable Z.</summary>
        /// <returns>The nullable Z.</returns>
        /// <remarks>Z is the altitude portion of position.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
        public abstract double? Z { get; }

        /// <summary>Gets the nullable M.</summary>
        /// <returns>The nullable M.</returns>
        /// <remarks>M is the arbitrary measure associated with a position.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "m is meaningful")]
        public abstract double? M { get; }

        /// <summary>Creates a geography point using the specified latitude and longitude.</summary>
        /// <returns>The geography point that was created.</returns>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public static GeographyPoint Create(double latitude, double longitude)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, null, null);
        }

        /// <summary>Creates a geography point using the specified latitude, longitude and dimension.</summary>
        /// <returns>The geography point that was created.</returns>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
        public static GeographyPoint Create(double latitude, double longitude, double? z)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, null);
        }

        /// <summary>Creates a geography point using the specified latitude, longitude and dimensions.</summary>
        /// <returns>The geography point that was created.</returns>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        public static GeographyPoint Create(double latitude, double longitude, double? z, double? m)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
        }

        /// <summary>Creates a geography point using the specified coordinate system, latitude, longitude and dimensions.</summary>
        /// <returns>The geography point that was created.</returns>
        /// <param name="coordinateSystem">The coordinate system to use.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        public static GeographyPoint Create(CoordinateSystem coordinateSystem, double latitude, double longitude, double? z, double? m)
        {
            var builder = SpatialBuilder.Create();
            var pipeline = builder.GeographyPipeline;
            pipeline.SetCoordinateSystem(coordinateSystem);
            pipeline.BeginGeography(SpatialType.Point);
            pipeline.BeginFigure(new GeographyPosition(latitude, longitude, z, m));
            pipeline.EndFigure();
            pipeline.EndGeography();
            return (GeographyPoint)builder.ConstructedGeography;
        }

        /// <summary>Determines whether this instance and another specified geography instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeographyPoint other)
        {
            return this.BaseEquals(other) ?? this.Latitude == other.Latitude && this.Longitude == other.Longitude && this.Z == other.Z && this.M == other.M;
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyPoint);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, new[] { this.IsEmpty ? 0 : this.Latitude, this.IsEmpty ? 0 : this.Longitude, this.Z ?? 0, this.M ?? 0 });
        }
    }
}
