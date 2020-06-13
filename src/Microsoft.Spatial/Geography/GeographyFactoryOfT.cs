//---------------------------------------------------------------------
// <copyright file="GeographyFactoryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Geography Spatial Factory
    /// </summary>
    /// <typeparam name="T">The target type</typeparam>
    public class GeographyFactory<T> : SpatialFactory where T : Geography
    {
        /// <summary>
        /// The provider of the built type
        /// </summary>
        private IGeographyProvider provider;

        /// <summary>
        /// The chain to build through
        /// </summary>
        private GeographyPipeline buildChain;

        /// <summary>
        /// Initializes a new instance of the GeographyFactory class
        /// </summary>
        /// <param name="coordinateSystem">The coordinate system</param>
        internal GeographyFactory(CoordinateSystem coordinateSystem)
        {
            var builder = SpatialBuilder.Create();
            this.provider = builder;
            this.buildChain = SpatialValidator.Create().ChainTo(builder).StartingLink;
            this.buildChain.SetCoordinateSystem(coordinateSystem);
        }

        /// <summary>
        /// Using implicit cast to trigger the Finalize call
        /// </summary>
        /// <param name="factory">The factory</param>
        /// <returns>The built instance of the target type</returns>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static implicit operator T(GeographyFactory<T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.Build();
        }

        #region Public Construction Calls

        /// <summary>
        /// Start a new Point
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M Value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeographyFactory<T> Point(double latitude, double longitude, double? z, double? m)
        {
            this.BeginGeo(SpatialType.Point);
            this.LineTo(latitude, longitude, z, m);
            return this;
        }

        /// <summary>
        /// Start a new Point
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> Point(double latitude, double longitude)
        {
            return this.Point(latitude, longitude, null, null);
        }

        /// <summary>
        /// Start a new empty Point
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> Point()
        {
            this.BeginGeo(SpatialType.Point);
            return this;
        }

        /// <summary>
        /// Start a new LineString
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M Value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeographyFactory<T> LineString(double latitude, double longitude, double? z, double? m)
        {
            this.BeginGeo(SpatialType.LineString);
            this.LineTo(latitude, longitude, z, m);
            return this;
        }

        /// <summary>
        /// Start a new LineString
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> LineString(double latitude, double longitude)
        {
            return this.LineString(latitude, longitude, null, null);
        }

        /// <summary>
        /// Start a new empty LineString
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> LineString()
        {
            this.BeginGeo(SpatialType.LineString);
            return this;
        }

        /// <summary>
        /// Start a new Polygon
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> Polygon()
        {
            this.BeginGeo(SpatialType.Polygon);
            return this;
        }

        /// <summary>
        /// Start a new MultiPoint
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeographyFactory<T> MultiPoint()
        {
            this.BeginGeo(SpatialType.MultiPoint);
            return this;
        }

        /// <summary>
        /// Start a new MultiLineString
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeographyFactory<T> MultiLineString()
        {
            this.BeginGeo(SpatialType.MultiLineString);
            return this;
        }

        /// <summary>
        /// Start a new MultiPolygon
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public GeographyFactory<T> MultiPolygon()
        {
            this.BeginGeo(SpatialType.MultiPolygon);
            return this;
        }

        /// <summary>
        /// Start a new Collection
        /// </summary>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> Collection()
        {
            this.BeginGeo(SpatialType.Collection);
            return this;
        }

        /// <summary>
        /// Start a new Polygon Ring
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M Value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeographyFactory<T> Ring(double latitude, double longitude, double? z, double? m)
        {
            this.StartRing(latitude, longitude, z, m);
            return this;
        }

        /// <summary>
        /// Start a new Polygon Ring
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> Ring(double latitude, double longitude)
        {
            return this.Ring(latitude, longitude, null, null);
        }

        /// <summary>
        /// Add a new point in the current line figure
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M Value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public GeographyFactory<T> LineTo(double latitude, double longitude, double? z, double? m)
        {
            this.AddPos(latitude, longitude, z, m);
            return this;
        }

        /// <summary>
        /// Add a new point in the current line figure
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>The current instance of GeographyFactory</returns>
        public GeographyFactory<T> LineTo(double latitude, double longitude)
        {
            return this.LineTo(latitude, longitude, null, null);
        }

        #endregion

        /// <summary>
        /// Finish the current geography
        /// </summary>
        /// <returns>The constructed instance</returns>
        public T Build()
        {
            this.Finish();
            return (T)this.provider.ConstructedGeography;
        }

        #region GeoDataPipeline overrides

        /// <summary>
        /// Begin a new geography
        /// </summary>
        /// <param name="type">The spatial type</param>
        protected override void BeginGeo(SpatialType type)
        {
            base.BeginGeo(type);
            this.buildChain.BeginGeography(type);
        }

        /// <summary>
        /// Begin drawing a figure
        /// TODO: longitude and latitude should be swapped !!! per ABNF.
        /// </summary>
        /// <param name="latitude">X or Latitude Coordinate</param>
        /// <param name="longitude">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected override void BeginFigure(double latitude, double longitude, double? z, double? m)
        {
            base.BeginFigure(latitude, longitude, z, m);
            this.buildChain.BeginFigure(new GeographyPosition(latitude, longitude, z, m));
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="latitude">X or Latitude Coordinate</param>
        /// <param name="longitude">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected override void AddLine(double latitude, double longitude, double? z, double? m)
        {
            base.AddLine(latitude, longitude, z, m);
            this.buildChain.LineTo(new GeographyPosition(latitude, longitude, z, m));
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
            this.buildChain.EndGeography();
        }

        #endregion
    }
}