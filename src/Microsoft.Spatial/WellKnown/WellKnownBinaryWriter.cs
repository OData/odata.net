//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal sealed class WellKnownBinaryWriter : DrawBoth, IDisposable
    {
        /// <summary>
        /// The writer settings.
        /// </summary>
        private WellKnownBinaryWriterSettings _settings;

        /// <summary>
        /// The underlying binary writer
        /// </summary>
        private BinaryWriter _writer;

        /// <summary>
        /// Stack of spatial types currently been built.
        /// </summary>
        private Stack<Scope> _stack;

        /// <summary>
        /// The coordinate system.
        /// </summary>
        private CoordinateSystem _coordinateSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownBinaryWriter"/> class using default writer settings.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public WellKnownBinaryWriter(Stream stream)
            : this(stream, new WellKnownBinaryWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlWriter"/> class.
        /// </summary>
        /// <param name="stream">The output streamr.</param>
        /// <param name="settings">The writer settings.</param>
        public WellKnownBinaryWriter(Stream stream, WellKnownBinaryWriterSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _writer = new BinaryWriter(stream);
            _stack = new Stack<Scope>();
            Reset();
        }

        #region DrawBoth
        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>
        /// The position to be passed down the pipeline
        /// </returns>
        protected override GeographyPosition OnLineTo(GeographyPosition position)
        {
            this.AddLineTo(position.Longitude, position.Latitude, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>
        /// The position to be passed down the pipeline
        /// </returns>
        protected override GeometryPosition OnLineTo(GeometryPosition position)
        {
            this.AddLineTo(position.X, position.Y, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        /// <returns>
        /// The type to be passed down the pipeline
        /// </returns>
        protected override SpatialType OnBeginGeography(SpatialType type)
        {
            BeginGeo(type);
            return type;
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        /// <returns>
        /// The type to be passed down the pipeline
        /// </returns>
        protected override SpatialType OnBeginGeometry(SpatialType type)
        {
            BeginGeo(type);
            return type;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeographyPosition OnBeginFigure(GeographyPosition position)
        {
            BeginFigure(position.Longitude, position.Latitude, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeometryPosition OnBeginFigure(GeometryPosition position)
        {
            BeginFigure(position.X, position.Y, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Ends the current figure
        /// </summary>
        protected override void OnEndFigure()
        {
            EndFigure();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void OnEndGeography()
        {
            EndGeo();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void OnEndGeometry()
        {
            EndGeo();
        }

        /// <summary>
        /// Set the coordinate system
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>
        /// the coordinate system to be passed down the pipeline
        /// </returns>
        protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            _coordinateSystem = coordinateSystem;
            return coordinateSystem;
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        protected override void OnReset()
        {
            Reset();
        }
        #endregion

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        private void Reset()
        {
            _writer.Seek(0, SeekOrigin.Begin);
            _stack.Clear();
            _coordinateSystem = null;
        }

        /// <summary>
        /// Start to Begin a new Geography/Geometry
        /// </summary>
        /// <param name="type">The SpatialType</param>
        private void BeginGeo(SpatialType type)
        {
            Scope parent = _stack.Count == 0 ? null : _stack.Peek();

            if (parent != null)
            {
                switch (parent.Type)
                {
                    case SpatialType.Point:
                    case SpatialType.LineString:
                    case SpatialType.Polygon:
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, type, parent.Type, $"{parent.Type} cannot contain other spatial types"));

                    case SpatialType.MultiPoint:
                        if (type != SpatialType.Point)
                        {
                            throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, type, parent.Type, "MultiPoint can only contain Points."));
                        }
                        break;

                    case SpatialType.MultiLineString:
                        if (type != SpatialType.LineString)
                        {
                            throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, type, parent.Type, "MultiLineString can only contain LineStrings."));
                        }

                        break;

                    case SpatialType.MultiPolygon:
                        if (type != SpatialType.Polygon)
                        {
                            throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, type, parent.Type, "MultiPolygon can only contain Polygons."));
                        }
                        break;

                    case SpatialType.Collection:
                        // Collection accepts all kind of spatial types.
                        break;
                }
            }

            int? srid = _coordinateSystem?.EpsgId;
            IWKBObject current = null;
            switch (type)
            {
                case SpatialType.Point:
                    current = new WKBPoint { X = double.NaN, Y = double.NaN, Z = _settings.HandleZ ? double.NaN : null, M = _settings.HandleM ? double.NaN : null };
                    break;
                case SpatialType.LineString:
                    current = new WKBLineString();
                    break;
                case SpatialType.Polygon:
                    current = new WKBPolygon();
                    break;
                case SpatialType.MultiPoint:
                    current = new WKBMultiPoint();
                    break;
                case SpatialType.MultiLineString:
                    current = new WKBMultiLineString();
                    break;
                case SpatialType.MultiPolygon:
                    current = new WKBMultiPolygon();
                    break;
                case SpatialType.Collection:
                    current = new WKBCollection();
                    break;

                default:
                    throw new NotSupportedException(Error.Format(SRResources.WellKnownBinary_NotSupportedSpatial, type));
            }

            _stack.Push(new Scope { Type = type, Value = current });
            return;
        }

        /// <summary>
        /// Start to begin a figure, begin a figure should be only on 'Point, LineString, Polygon'
        /// </summary>
        /// <param name="coordinate1">The coordinate1 (X).</param>
        /// <param name="coordinate2">The coordinate2.(Y)</param>
        /// <param name="coordinate3">The coordinate3.(Z)</param>
        /// <param name="coordinate4">The coordinate4.(M)</param>
        private void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            if (_stack.Count == 0)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "BeginFigure"));
            }

            Debug.Assert(_stack.Count > 0, "Should have called BeginGeo");
            Scope current = _stack.Peek();

            if (current.Type == SpatialType.MultiPoint ||
                current.Type == SpatialType.MultiLineString ||
                current.Type == SpatialType.MultiPolygon ||
                current.Type == SpatialType.Collection)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginFigureOnSpatial, current.Type));
            }

            if (current.IsFigureBegin)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginFigureWithoutEndingPrevious, current.Type));
            }

            int? srid = _coordinateSystem?.EpsgId;
            WKBPoint point = new WKBPoint { X = coordinate1, Y = coordinate2, Z = coordinate3, M = coordinate4 };

            if (current.Type == SpatialType.Point)
            {
                current.Value = point; // this one will replace the default (dummy) point created when calling BeginGeo on Point.
            }
            else if (current.Type == SpatialType.LineString)
            {
                WKBLineString wkbLineString = (WKBLineString)current.Value;
                wkbLineString.Points.Add(point);
            }
            else if (current.Type == SpatialType.Polygon)
            {
                WKBPolygon wkbPg = (WKBPolygon)current.Value;
                WKBLineString wkbLs = new WKBLineString();
                wkbLs.Points.Add(point);
                wkbPg.Rings.Add(wkbLs);
            }
            else
            {
                // should never be here, since BeginGeo makes the correct type
                Debug.Assert(false, $"BeginFigure on unknown type: {current.Type}");
                throw new InvalidOperationException($"BeginFigure on unknown type: {current.Type}");
            }

            current.IsFigureBegin = true;
        }

        /// <summary>
        /// Adds the control point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="m">The m.</param>
        private void AddLineTo(double x, double y, double? z, double? m)
        {
            if (_stack.Count == 0)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "LineTo"));
            }

            Debug.Assert(_stack.Count > 0, "Should have called BeginGeo");
            Scope current = _stack.Peek();

            int? srid = _coordinateSystem?.EpsgId;
            switch (current.Type)
            {
                case SpatialType.LineString:
                    WKBLineString wkbLs = current.Value as WKBLineString;
                    if (wkbLs == null || wkbLs.Points.Count == 0)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidAddLineTo, current.Type, "Should call BeginFigure first on LineString."));
                    }
                    wkbLs.Points.Add(new WKBPoint { X = x, Y = y, Z = z, M = m });
                    break;

                case SpatialType.Polygon:
                    WKBPolygon wkbPg = current.Value as WKBPolygon;
                    if (wkbPg == null || wkbPg.Rings.Count == 0)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidAddLineTo, current.Type, "Should call BeginFigure first on Polygon."));
                    }
                    wkbPg.Rings.Last().Points.Add(new WKBPoint { X = x, Y = y, Z = z, M = m });
                    break;

                case SpatialType.Point:
                case SpatialType.MultiPoint:
                case SpatialType.MultiLineString:
                case SpatialType.MultiPolygon:
                case SpatialType.Collection:
                default:
                    throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidAddLineTo, current.Type, string.Empty));
            }
        }

        /// <summary>
        /// Ends the figure.
        /// </summary>
        private void EndFigure()
        {
            // End to write the figure.
            if (_stack.Count == 0)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "EndFigure"));
            }

            Debug.Assert(_stack.Count > 0, "Should have called BeginGeo");
            Scope current = _stack.Peek();

            if (current.Type == SpatialType.Point ||
                current.Type == SpatialType.LineString ||
                current.Type == SpatialType.Polygon)
            {
                if (!current.IsFigureBegin)
                {
                    throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndFigure, current.Type, "You haven't begun the figure."));
                }

                current.IsFigureBegin = false; // EndFigure
            }
            else
            {
                // for other spatial types (Multi*, Collection)
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndFigure, current.Type, string.Empty));
            }
        }

        /// <summary>
        /// End the current Geography/Geometry
        /// </summary>
        private void EndGeo()
        {
            if (_stack.Count <= 0)
            {
                throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "EndGeo"));
            }

            Scope current = _stack.Pop();
            if (_stack.Count == 0)
            {
                // Now, we are in the top level EndGeo, let's write it.
                WriteWKB(current.Value, true);
                return;
            }

            Scope parent = _stack.Peek();
            switch (parent.Type)
            {
                case SpatialType.Point:
                case SpatialType.LineString:
                case SpatialType.Polygon:
                    throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndGeo, current.Type, parent.Type));

                case SpatialType.MultiPoint:
                    if (current.Type != SpatialType.Point)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndGeo, current.Type, parent.Type));
                    }

                    ((WKBMultiPoint)parent.Value).Points.Add((WKBPoint)current.Value);

                    break;

                case SpatialType.MultiLineString:
                    if (current.Type != SpatialType.LineString)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndGeo, current.Type, parent.Type));
                    }

                    ((WKBMultiLineString)parent.Value).LineStrings.Add((WKBLineString)current.Value);
                    break;

                case SpatialType.MultiPolygon:
                    if (current.Type != SpatialType.Polygon)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.WellKnownBinary_InvalidEndGeo, current.Type, parent.Type));
                    }

                    ((WKBMultiPolygon)parent.Value).Polygons.Add((WKBPolygon)current.Value);
                    break;

                case SpatialType.Collection:
                    ((WKBCollection)parent.Value).Items.Add(current.Value);
                    break;

                default:
                    // should never be here, since BeginGeo makes the correct type
                    Debug.Assert(false, $"EndGeo on unknown type: {current.Type}");
                    throw new InvalidOperationException($"EndGeo on unknown type: {current.Type}");
            }
        }

        private void WriteWKB(IWKBObject wkbObj, bool includeSRID)
        {
            switch (wkbObj.SpatialType)
            {
                case SpatialType.Point:
                    Write((WKBPoint)wkbObj, includeSRID);
                    break;
                case SpatialType.LineString:
                    Write((WKBLineString)wkbObj, includeSRID);
                    break;
                case SpatialType.Polygon:
                    Write((WKBPolygon)wkbObj, includeSRID);
                    break;
                case SpatialType.MultiPoint:
                    Write((WKBMultiPoint)wkbObj, includeSRID);
                    break;
                case SpatialType.MultiLineString:
                    Write((WKBMultiLineString)wkbObj, includeSRID);
                    break;
                case SpatialType.MultiPolygon:
                    Write((WKBMultiPolygon)wkbObj, includeSRID);
                    break;
                case SpatialType.Collection:
                    Write((WKBCollection)wkbObj, includeSRID);
                    break;
                default:
                    Debug.Assert(false, "Write unknown spatial type {wkbObj.SpatialType}");
                    throw new InvalidOperationException($"Write unknown spatial type {wkbObj.SpatialType}. Should never be here");
            }
        }

        /// <summary>
        /// Write the WKB Header for the geometry & geography
        /// </summary>
        /// <param name="type">The spatial type.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        /// <remarks>
        /// The Header typically contains:
        /// 1 byte  : Required, (0 -> Big Endian, 1 -> Little Endian)
        /// 4 bytes : 32 bit unsigned integer, Required for spatial type and flags for SRID, Z and M
        /// 4 bytes : 32 bit unsigned integer, Optional for SRID
        /// </remarks>
        private void WriteHeader(SpatialType type, bool includeSRID)
        {
            uint intSpatialType = (uint)type & 0xff;

            if (_settings.HandleZ)
            {
                // compatible for ISO WKB
                if (_settings.IsoWKB)
                {
                    intSpatialType += 1000;
                }

                // support Extended WKB
                intSpatialType |= 0x80000000;
            }

            if (_settings.HandleM)
            {
                // compatible for ISO WKB
                if (_settings.IsoWKB)
                {
                    intSpatialType += 2000;
                }

                // support Extended WKB
                intSpatialType |= 0x40000000;
            }

            bool hasSRID = HasSRID(out int srid);
            includeSRID &= _settings.HandleSRID;

            if (includeSRID && hasSRID)
            {
                intSpatialType |= 0x20000000;
            }

            // Required, 1 byte for bit order
            _writer.Write((byte)_settings.Order);

            // Required, 4 bytes for spatial type and flags for SRID, Z and M
            _writer.Write(intSpatialType, _settings.Order);

            // Optional, 4 bytes for SRID value.
            if (includeSRID && hasSRID)
            {
                _writer.Write(srid, _settings.Order);
            }
        }

        private bool HasSRID(out int srid)
        {
            srid = 0;
            if (_coordinateSystem == null || !_coordinateSystem.EpsgId.HasValue)
            {
                return false;
            }

            srid = _coordinateSystem.EpsgId.Value;
            return true;
        }

        /// <summary>
        /// Point {
        ///   double x;
        ///   double y;
        ///   doulbe z; (optional)
        ///   doulbe m; (optional)
        /// }
        /// 
        /// WKBPoint {
        ///    byte byteOrder;
        ///    uint32 wkbType; // 1
        ///    Point point;
        /// }
        /// </summary>
        /// <param name="point">The WKBPoint.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBPoint point, bool includeSRID)
        {
            WriteHeader(SpatialType.Point, includeSRID);
            Write(point.X, point.Y, point.Z, point.M);
        }

        /// <summary>
        /// WKBLineString {
        ///    byte byteOrder;
        ///    uint32 wkbType; // 2
        ///    uint32  numPoints;
        ///    Point points[numPoints];
        /// }
        /// </summary>
        /// <param name="lineString">The WKBLineString.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBLineString lineString, bool includeSRID)
        {
            WriteHeader(SpatialType.LineString, includeSRID);

            // Write the number of Points in LineString
            _writer.Write(lineString.Points.Count, _settings.Order);

            Write(lineString.Points, false);
        }

        /// <summary>
        /// LinearRing {
        ///  uint32 numPoints;
        ///  Point points[numPoints];
        /// };
        ///
        /// WKBPolygon {
        ///    byte byteOrder;
        ///    uint32 wkbType; // 3
        ///    uint32 numRings;
        ///    LinearRing rings[numRings]
        /// }
        /// </summary>
        /// <param name="polygon">The WKBPolygon.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBPolygon polygon, bool includeSRID)
        {
            WriteHeader(SpatialType.Polygon, includeSRID);

            // Write the number of Rings in Polygon
            _writer.Write(polygon.Rings.Count, _settings.Order);

            // Write all points for each ring
            foreach (var ring in polygon.Rings)
            {
                Write(ring.Points, true);
            }
        }

        /// <summary>
        /// WKBMultiPoint {
        ///   byte byteOrder;
        ///   uint32 wkbType; // 4
        ///   uint32 numWkbPoints;
        ///   WKBPoint WKBPoints[numWkbPoints];
        /// }
        /// </summary>
        /// <param name="multiPoint">The Multipoint.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBMultiPoint multiPoint, bool includeSRID)
        {
            WriteHeader(SpatialType.MultiPoint, includeSRID);

            _writer.Write(multiPoint.Points.Count, _settings.Order);

            foreach (var point in multiPoint.Points)
            {
                // So far, the sub points should have the same SRID, so should skip the SRID for all sub points.
                Write(point, false);
            }
        }

        /// <summary>
        /// WKBMultiLineString {
        ///   byte byteOrder;
        ///   uint32 wkbType; // 5
        ///   uint32 numWkbLineStrings;
        ///   WKBLineString WKBLineStrings[numWkbLineStrings];
        /// }
        /// </summary>
        /// <param name="multiLineString">The MultiLineString.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBMultiLineString multiLineString, bool includeSRID)
        {
            WriteHeader(SpatialType.MultiLineString, includeSRID);

            _writer.Write(multiLineString.LineStrings.Count, _settings.Order);

            // Write all points for each LineString
            foreach (var lineString in multiLineString.LineStrings)
            {
                // So far, the sub LineString should have the same SRID, so should skip the SRID for all sub LineStrings.
                Write(lineString, false);
            }
        }

        /// <summary>
        /// WKBMultiPolygon {
        ///   byte byteOrder;
        ///   uint32 wkbType; // 6
        ///   uint32  numWkbPolygons;
        ///   WKBPolygon wkbPolygons[numWkbPolygons];
        /// }
        /// </summary>
        /// <param name="WKBMultiPolygon">The MultiPolygon.</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBMultiPolygon multiPolygon, bool includeSRID)
        {
            WriteHeader(SpatialType.MultiPolygon, includeSRID);

            _writer.Write(multiPolygon.Polygons.Count, _settings.Order);

            foreach (var polygon in multiPolygon.Polygons)
            {
                // So far, the sub Polygon should have the same SRID, so should skip the SRID for all sub Polygons.
                Write(polygon, false);
            }
        }

        /// <summary>
        /// Write a GeometryCollection in its WKB format.
        /// WKBGeometry {
        ///   union {
        ///       WKBPoint point;
        ///       WKBLineString linestring;
        ///       WKBPolygon polygon;
        ///       WKBGeometryCollection collection;
        ///       WKBMultiPoint mpoint;
        ///       WKBMultiLineString mlinestring;
        ///       WKBMultiPolygon mpolygon;
        ///    }
        /// }
        ///
        ///  WKBGeometryCollection {
        ///    byte byteOrder;
        ///    uint32 wkbType; // 7
        ///    uint32 numWkbGeometries;
        ///    WKBGeometry wkbGeometries[numWkbGeometries];
        /// }
        /// </summary>
        /// <param name="collection">The WKBCollection</param>
        /// <param name="includeSRID">A boolean value indicating to write the SRID or not.</param>
        private void Write(WKBCollection collection, bool includeSRID)
        {
            WriteHeader(SpatialType.Collection, includeSRID);

            _writer.Write(collection.Items.Count, _settings.Order);

            foreach (var item in collection.Items)
            {
                // So far, the sub item should have the same SRID, so should skip the SRID for all sub items.
                WriteWKB(item, false);
            }
        }

        private void Write(IList<WKBPoint> points, bool writeSize)
        {
            if (writeSize)
            {
                _writer.Write(points.Count, _settings.Order);
            }

            foreach (var point in points)
            {
                if (point == null)
                {
                    Write(double.NaN, double.NaN, double.NaN, double.NaN);
                }
                else
                {
                    Write(point.X, point.Y, point.Z, point.M);
                }
            }
        }

        private void Write(double x, double y, double? z, double? m)
        {
            _writer.Write(x, _settings.Order);
            _writer.Write(y, _settings.Order);

            double zValue = z.HasValue ? z.Value : double.NaN;
            if (_settings.HandleZ)
            {
                _writer.Write(zValue, _settings.Order);
            }

            double mValue = m.HasValue ? m.Value : double.NaN;
            if (_settings.HandleM)
            {
                _writer.Write(mValue, _settings.Order);
            }
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }

    internal class Scope
    {
        public SpatialType Type { get; set; }

        public IWKBObject Value { get; set; }

        public bool IsFigureBegin { get; set; } = false;
    }

    internal interface IWKBObject
    {
        SpatialType SpatialType { get; }
    }

    internal class WKBPoint : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.Point;
        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
        public double? M { get; set; }
    };

    internal class WKBLineString : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.LineString;
        public IList<WKBPoint> Points { get; set; } = new List<WKBPoint>();
    }

    internal class WKBPolygon : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.Polygon;
        public IList<WKBLineString> Rings { get; set; } = new List<WKBLineString>();
    }

    internal class WKBMultiPoint : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.MultiPoint;
        public IList<WKBPoint> Points { get; set; } = new List<WKBPoint>();
    }

    internal class WKBMultiLineString : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.MultiLineString;
        public IList<WKBLineString> LineStrings { get; set; } = new List<WKBLineString>();
    }

    internal class WKBMultiPolygon : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.MultiPolygon;
        public IList<WKBPolygon> Polygons { get; set; } = new List<WKBPolygon>();
    }

    internal class WKBCollection : IWKBObject
    {
        public SpatialType SpatialType => SpatialType.Collection;
        public IList<IWKBObject> Items { get; set; } = new List<IWKBObject>();
    }
}
