//---------------------------------------------------------------------
// <copyright file="WellKnownTextSqlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// WellKnownText Writer
    /// </summary>
    internal sealed class WellKnownTextSqlWriter : DrawBoth
    {
        /// <summary>
        /// restricts the writer to allow only two dimensions.
        /// </summary>
        private bool allowOnlyTwoDimensions;

        /// <summary>
        /// The underlying writer
        /// </summary>
        private TextWriter writer;

        /// <summary>
        /// Stack of spatial types currently been built
        /// </summary>
        private Stack<SpatialType> parentStack;

        /// <summary>
        /// Detects if a CoordinateSystem (SRID) has been written already.
        /// </summary>
        private bool coordinateSystemWritten;

        /// <summary>
        /// Figure has been written to the current spatial type
        /// </summary>
        private bool figureWritten;

        /// <summary>
        /// A shape has been written in the current nesting level
        /// </summary>
        private bool shapeWritten;

        /// <summary>
        /// Wells the known text SQL format. -- 2D writer
        /// </summary>
        /// <param name="writer">The writer.</param>
        public WellKnownTextSqlWriter(TextWriter writer)
        : this(writer, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="allowOnlyTwoDimensions">if set to <c>true</c> allows only two dimensions.</param>
        public WellKnownTextSqlWriter(TextWriter writer, bool allowOnlyTwoDimensions)
        {
            this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
            this.writer = writer;
            this.parentStack = new Stack<SpatialType>();
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
            WriteFigureScope(position.Longitude, position.Latitude, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeometryPosition OnBeginFigure(GeometryPosition position)
        {
            WriteFigureScope(position.X, position.Y, position.Z, position.M);
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
            WriteCoordinateSystem(coordinateSystem);
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
        /// Write the coordinate system
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        private void WriteCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            if (!this.coordinateSystemWritten)
            {
                // SRID can only be set once in WKT, but can be set once per BeginGeo in collection types
                this.writer.Write(WellKnownTextConstants.WktSrid);
                this.writer.Write(WellKnownTextConstants.WktEquals);
                this.writer.Write(coordinateSystem.Id);
                this.writer.Write(WellKnownTextConstants.WktSemiColon);

                this.coordinateSystemWritten = true;
            }
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        private void Reset()
        {
            this.figureWritten  = default(bool);
            this.parentStack.Clear();
            this.shapeWritten = default(bool);
            this.coordinateSystemWritten = default(bool);

            // we are unable to reset the text writer, we will just
            // noop and start writing fresh
            ////this.writer.
        }

        /// <summary>
        /// Start to write a new Geography/Geometry
        /// </summary>
        /// <param name="type">The SpatialType to write</param>
        private void BeginGeo(SpatialType type)
        {
            SpatialType parentType = this.parentStack.Count == 0 ? SpatialType.Unknown : this.parentStack.Peek();

            if (parentType == SpatialType.MultiPoint || parentType == SpatialType.MultiLineString || parentType == SpatialType.MultiPolygon || parentType == SpatialType.Collection)
            {
                // container, first element should write out (, subsequent ones write out ","
                this.writer.Write(this.shapeWritten ? WellKnownTextConstants.WktDelimiterWithWhiteSpace : WellKnownTextConstants.WktOpenParen);
            }

            // Write tagged text
            // Only write if toplevel, or the parent is a collection type
            if (parentType == SpatialType.Unknown || parentType == SpatialType.Collection)
            {
                this.WriteTaggedText(type);
            }

            this.figureWritten = false;
            this.parentStack.Push(type);
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
            this.writer.Write(WellKnownTextConstants.WktDelimiterWithWhiteSpace);
            this.WritePoint(x, y, z, m);
        }

        /// <summary>
        /// Ends the figure.
        /// </summary>
        private void EndFigure()
        {
            this.writer.Write(WellKnownTextConstants.WktCloseParen);
        }

        /// <summary>
        /// write tagged text for type
        /// </summary>
        /// <param name="type">the spatial type</param>
        private void WriteTaggedText(SpatialType type)
        {
            switch (type)
            {
                case SpatialType.Point:
                    this.writer.Write(WellKnownTextConstants.WktPoint);
                    break;
                case SpatialType.LineString:
                    this.writer.Write(WellKnownTextConstants.WktLineString);
                    break;
                case SpatialType.Polygon:
                    this.writer.Write(WellKnownTextConstants.WktPolygon);
                    break;
                case SpatialType.Collection:
                    this.shapeWritten = false;
                    this.writer.Write(WellKnownTextConstants.WktCollection);
                    break;
                case SpatialType.MultiPoint:
                    this.shapeWritten = false;
                    this.writer.Write(WellKnownTextConstants.WktMultiPoint);
                    break;
                case SpatialType.MultiLineString:
                    this.shapeWritten = false;
                    this.writer.Write(WellKnownTextConstants.WktMultiLineString);
                    break;
                case SpatialType.MultiPolygon:
                    this.shapeWritten = false;
                    this.writer.Write(WellKnownTextConstants.WktMultiPolygon);
                    break;
                case SpatialType.FullGlobe:
                    this.writer.Write(WellKnownTextConstants.WktFullGlobe);
                    break;
            }

            if (type != SpatialType.FullGlobe)
            {
                this.writer.Write(WellKnownTextConstants.WktWhitespace);
            }
        }

        /// <summary>
        /// Start to write a figure
        /// </summary>
        /// <param name="coordinate1">The coordinate1.</param>
        /// <param name="coordinate2">The coordinate2.</param>
        /// <param name="coordinate3">The coordinate3.</param>
        /// <param name="coordinate4">The coordinate4.</param>
        private void WriteFigureScope(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            Debug.Assert(this.parentStack.Count > 0, "Should have called BeginGeo");

            if (this.figureWritten)
            {
                this.writer.Write(WellKnownTextConstants.WktDelimiterWithWhiteSpace);
            }
            else
            {
                // first figure
                if (this.parentStack.Peek() == SpatialType.Polygon)
                {
                    // Polygon has additional set of paren to separate out rings
                    this.writer.Write(WellKnownTextConstants.WktOpenParen);
                }
            }

            // figure scope
            this.writer.Write(WellKnownTextConstants.WktOpenParen);
            this.figureWritten = true;

            this.WritePoint(coordinate1, coordinate2, coordinate3, coordinate4);
        }

        /// <summary>
        /// End the current Geography/Geometry
        /// </summary>
        private void EndGeo()
        {
            switch (this.parentStack.Pop())
            {
                case SpatialType.Point:
                case SpatialType.LineString:
                    if (!figureWritten)
                    {
                        this.writer.Write(WellKnownTextConstants.WktEmpty);
                    }

                    break;
                case SpatialType.Polygon:
                    this.writer.Write(figureWritten ? WellKnownTextConstants.WktCloseParen : WellKnownTextConstants.WktEmpty);
                    break;
                case SpatialType.Collection:
                case SpatialType.MultiPoint:
                case SpatialType.MultiLineString:
                case SpatialType.MultiPolygon:
                    this.writer.Write(this.shapeWritten ? WellKnownTextConstants.WktCloseParen : WellKnownTextConstants.WktEmpty);
                    break;
                case SpatialType.FullGlobe:
                    this.writer.Write(WellKnownTextConstants.WktCloseParen);
                    break;
            }

            this.shapeWritten = true;
            this.writer.Flush();
        }

        /// <summary>
        /// Write out a point
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="z">The z coordinate</param>
        /// <param name="m">The m coordinate</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m are meaningful")]
        private void WritePoint(double x, double y, double? z, double? m)
        {
            this.writer.WriteRoundtrippable(x);
            this.writer.Write(WellKnownTextConstants.WktWhitespace);
            this.writer.WriteRoundtrippable(y);

            if (!this.allowOnlyTwoDimensions && z.HasValue)
            {
                this.writer.Write(WellKnownTextConstants.WktWhitespace);
                this.writer.WriteRoundtrippable(z.Value);

                if (!this.allowOnlyTwoDimensions && m.HasValue)
                {
                    this.writer.Write(WellKnownTextConstants.WktWhitespace);
                    this.writer.WriteRoundtrippable(m.Value);
                }
            }
            else
            {
                if (!this.allowOnlyTwoDimensions && m.HasValue)
                {
                    this.writer.Write(WellKnownTextConstants.WktWhitespace);
                    this.writer.Write(WellKnownTextConstants.WktNull);
                    this.writer.Write(WellKnownTextConstants.WktWhitespace);
                    this.writer.Write(m.Value);
                }
            }
        }
    }
}
