//---------------------------------------------------------------------
// <copyright file="GeometryFactoryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Geometry Spatial Factory
    /// </summary>
    /// <typeparam name="T">The target type</typeparam>
    public class GeometryFactory<T> : SpatialFactory where T : Geometry
    {
        /// <summary>
        /// The provider of the built type
        /// </summary>
        private IGeometryProvider provider;

        /// <summary>
        /// The chain to build through
        /// </summary>
        private GeometryPipeline buildChain;

        /// <summary>
        /// Initializes a new instance of the GeometryFactory class
        /// </summary>
        /// <param name="coordinateSystem">The coordinate system</param>
        internal GeometryFactory(CoordinateSystem coordinateSystem)
        {
            var builder = SpatialBuilder.Create();
            this.provider = builder;
            this.buildChain = SpatialValidator.Create().ChainTo(builder).StartingLink;
            this.buildChain.SetCoordinateSystem(coordinateSystem);
        }

        /// <summary>
        /// Cast a factory to the target type
        /// </summary>
        /// <param name="factory">The factory</param>
        /// <returns>The built instance of the target type</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Operator used to build")]
        public static implicit operator T(GeometryFactory<T> factory)
        {
            if (factory != null)
            {
                return factory.Build();
            }

            return null;
        }

        #region Public Construction Calls

        /// <summary>
        /// Start a new Point
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> Point(double x, double y, double? z, double? m)
        {
            this.BeginGeo(SpatialType.Point);
            this.LineTo(x, y, z, m);
            return this;
        }

        /// <summary>
        /// Start a new Point
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> Point(double x, double y)
        {
            return this.Point(x, y, null, null);
        }

        /// <summary>
        /// Start a new empty Point
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        public GeometryFactory<T> Point()
        {
            this.BeginGeo(SpatialType.Point);
            return this;
        }

        /// <summary>
        /// Start a new LineString
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> LineString(double x, double y, double? z, double? m)
        {
            this.BeginGeo(SpatialType.LineString);
            this.LineTo(x, y, z, m);
            return this;
        }

        /// <summary>
        /// Start a new LineString
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> LineString(double x, double y)
        {
            return this.LineString(x, y, null, null);
        }

        /// <summary>
        /// Start a new empty LineString
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        public GeometryFactory<T> LineString()
        {
            this.BeginGeo(SpatialType.LineString);
            return this;
        }

        /// <summary>
        /// Start a new Polygon
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        public GeometryFactory<T> Polygon()
        {
            this.BeginGeo(SpatialType.Polygon);
            return this;
        }

        /// <summary>
        /// Start a new MultiPoint
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeometryFactory<T> MultiPoint()
        {
            this.BeginGeo(SpatialType.MultiPoint);
            return this;
        }

        /// <summary>
        /// Start a new MultiLineString
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeometryFactory<T> MultiLineString()
        {
            this.BeginGeo(SpatialType.MultiLineString);
            return this;
        }

        /// <summary>
        /// Start a new MultiPolygon
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeometryFactory<T> MultiPolygon()
        {
            this.BeginGeo(SpatialType.MultiPolygon);
            return this;
        }

        /// <summary>
        /// Start a new Collection
        /// </summary>
        /// <returns>The current instance of GeometryFactory</returns>
        public GeometryFactory<T> Collection()
        {
            this.BeginGeo(SpatialType.Collection);
            return this;
        }

        /// <summary>
        /// Start a new Polygon Ring
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> Ring(double x, double y, double? z, double? m)
        {
            this.StartRing(x, y, z, m);
            return this;
        }

        /// <summary>
        /// Start a new Polygon Ring
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> Ring(double x, double y)
        {
            return this.Ring(x, y, null, null);
        }

        /// <summary>
        /// Add a new point in the current line figure
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> LineTo(double x, double y, double? z, double? m)
        {
            this.AddPos(x, y, z, m);
            return this;
        }

        /// <summary>
        /// Add a new point in the current line figure
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>The current instance of GeometryFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeometryFactory<T> LineTo(double x, double y)
        {
            return this.LineTo(x, y, null, null);
        }

        #endregion

        /// <summary>
        /// Finish the current Geometry
        /// </summary>
        /// <returns>The constructed instance</returns>
        public T Build()
        {
            this.Finish();
            return (T)this.provider.ConstructedGeometry;
        }

        #region GeoDataPipeline overrides

        /// <summary>
        /// Begin a new geometry
        /// </summary>
        /// <param name="type">The spatial type</param>
        protected override void BeginGeo(SpatialType type)
        {
            base.BeginGeo(type);
            this.buildChain.BeginGeometry(type);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="x">X or Latitude Coordinate</param>
        /// <param name="y">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        protected override void BeginFigure(double x, double y, double? z, double? m)
        {
            base.BeginFigure(x, y, z, m);
            this.buildChain.BeginFigure(new GeometryPosition(x, y, z, m));
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="x">X or Latitude Coordinate</param>
        /// <param name="y">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        protected override void AddLine(double x, double y, double? z, double? m)
        {
            base.AddLine(x, y, z, m);
            this.buildChain.LineTo(new GeometryPosition(x, y, z, m));
        }

        /// <summary>
        /// Ends the figure set on the current node
        /// </summary>
        protected override void EndFigure()
        {
            base.EndFigure();
            this.buildChain.EndFigure();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void EndGeo()
        {
            base.EndGeo();
            this.buildChain.EndGeometry();
        }

        #endregion
    }
}