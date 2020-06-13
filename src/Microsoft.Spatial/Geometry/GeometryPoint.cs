//---------------------------------------------------------------------
// <copyright file="GeometryPoint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the Geometry Point.</summary>
    public abstract class GeometryPoint : Geometry
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GeometryPoint" /> class. Empty Point constructor.</summary>
        /// <param name="coordinateSystem">The CoordinateSystem.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the Latitude.</summary>
        /// <returns>The Latitude.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "X is meaningful")]
        public abstract double X { get; }

        /// <summary>Gets the Longitude.</summary>
        /// <returns>The Longitude.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Y is meaningful")]
        public abstract double Y { get; }

        /// <summary>Gets the Nullable Z.</summary>
        /// <returns>The Nullable Z.</returns>
        /// <remarks>Z is the altitude portion of position.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
        public abstract double? Z { get; }

        /// <summary>Gets the Nullable M.</summary>
        /// <returns>The Nullable M.</returns>
        /// <remarks>M is the arbitrary measure associated with a position.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "m is meaningful")]
        public abstract double? M { get; }

        /// <summary> Creates the specified latitude. </summary>
        /// <returns>The GeographyPoint that was created.</returns>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x and y are meaningful")]
        public static GeometryPoint Create(double x, double y)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, null, null);
        }

        /// <summary> Creates the specified latitude. </summary>
        /// <returns>The GeographyPoint that was created.</returns>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y and z are meaningful")]
        public static GeometryPoint Create(double x, double y, double? z)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, z, null);
        }

        /// <summary> Creates the specified latitude. </summary>
        /// <returns>The GeographyPoint that was created.</returns>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryPoint Create(double x, double y, double? z, double? m)
        {
            return Create(CoordinateSystem.DefaultGeometry, x, y, z, m);
        }

        /// <summary> Creates the specified latitude. </summary>
        /// <returns>The GeographyPoint that was created.</returns>
        /// <param name="coordinateSystem">The coordinate system to use.</param>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        /// <param name="m">The m dimension.</param>
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

        /// <summary> Determines whether this instance and another specified geography instance have the same value.  </summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeometryPoint other)
        {
            return this.BaseEquals(other) ?? this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.M == other.M;
        }

        /// <summary> Determines whether this instance and another specified geography instance have the same value.  </summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The geography to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeometryPoint);
        }

        /// <summary> Gets the Hashcode.</summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, new[] { this.IsEmpty ? 0 : this.X, this.IsEmpty ? 0 : this.Y, this.Z ?? 0, this.M ?? 0 });
        }
    }
}
