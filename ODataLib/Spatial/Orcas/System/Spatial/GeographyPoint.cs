//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Spatial
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Geography point
    /// </summary>
    public abstract class GeographyPoint : Geography
    {
        /// <summary>
        /// Create a empty point
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>
        /// Latitude
        /// </summary>
        public abstract double Latitude { get; }

        /// <summary>
        /// Longitude
        /// </summary>
        public abstract double Longitude { get; }

        /// <summary>
        /// Nullable Z
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
        public abstract double? Z { get; }

        /// <summary>
        /// Nullable M
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "m is meaningful")]
        public abstract double? M { get; }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The GeographyPoint that was created</returns>
        public static GeographyPoint Create(double latitude, double longitude)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, null, null);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
        public static GeographyPoint Create(double latitude, double longitude, double? z)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, null);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        public static GeographyPoint Create(double latitude, double longitude, double? z, double? m)
        {
            return Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="coordinateSystem">the coordinate system to use</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
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

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="other">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeographyPoint other)
        {
            return this.BaseEquals(other) ?? this.Latitude == other.Latitude && this.Longitude == other.Longitude && this.Z == other.Z && this.M == other.M;
        }

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="obj">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyPoint);
        }

        /// <summary>
        /// Get Hashcode
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, new[] { this.IsEmpty ? 0 : this.Latitude, this.IsEmpty ? 0 : this.Longitude, this.Z ?? 0, this.M ?? 0 });
        }
    }
}
