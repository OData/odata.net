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
    /// Geometry Point
    /// </summary>
    public abstract class GeometryPoint : Geometry
    {
        /// <summary>
        /// Empty Point constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>
        /// Latitude
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "X is meaningful")]
        public abstract double X { get; }

        /// <summary>
        /// Longitude
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Y is meaningful")]
        public abstract double Y { get; }

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
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x and y are meaningful")]
        public static GeometryPoint Create(double x, double y)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, null, null);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y and z are meaningful")]
        public static GeometryPoint Create(double x, double y, double? z)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, z, null);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryPoint Create(double x, double y, double? z, double? m)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, z, m);
        }

        /// <summary>
        /// Creates the specified latitude.
        /// </summary>
        /// <param name="coordinateSystem">the coordinate system to use</param>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        /// <returns>The GeographyPoint that was created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryPoint Create(CoordinateSystem coordinateSystem, double x, double y, double? z, double? m)
        {
            var builder = SpatialBuilder.Create();
            var pipeline = builder.GeometryPipeline;
            pipeline.SetCoordinateSystem(coordinateSystem);
            pipeline.BeginGeometry(SpatialType.Point);
            pipeline.BeginFigure(new GeometryPosition(x, y, z, m));
            pipeline.EndFigure();
            pipeline.EndGeometry();
            return (GeometryPoint)builder.ConstructedGeometry;
        }

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="other">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeometryPoint other)
        {
            return this.BaseEquals(other) ?? this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.M == other.M;
        }

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="obj">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeometryPoint);
        }

        /// <summary>
        /// Get Hashcode
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, new[] { this.IsEmpty ? 0 : this.X, this.IsEmpty ? 0 : this.Y, this.Z ?? 0, this.M ?? 0 });
        }
    }
}
