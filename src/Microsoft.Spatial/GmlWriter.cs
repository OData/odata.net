//---------------------------------------------------------------------
// <copyright file="GmlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;

    /// <summary>
    /// Gml Writer
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the common name for this format")]
    internal sealed class GmlWriter : DrawBoth
    {
        /// <summary>
        /// The underlying writer
        /// </summary>
        private XmlWriter writer;

        /// <summary>
        /// Stack of spatial types currently been built
        /// </summary>
        private Stack<SpatialType> parentStack;

        /// <summary>
        /// If an SRID has been written already.
        /// </summary>
        private bool coordinateSystemWritten;

        /// <summary>
        /// The Coordinate System to write
        /// </summary>
        private CoordinateSystem currentCoordinateSystem;

        /// <summary>
        /// Figure has been written to the current spatial type
        /// </summary>
        private bool figureWritten;

        /// <summary>
        /// Whether there are shapes written in the current container
        /// </summary>
        private bool shouldWriteContainerWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The Xml Writer to output to</param>
        public GmlWriter(XmlWriter writer)
        {
            this.writer = writer;
            this.OnReset();
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        /// <returns>The type to be passed down the pipeline</returns>
        protected override SpatialType OnBeginGeography(SpatialType type)
        {
            this.BeginGeo(type);
            return type;
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeographyPosition OnLineTo(GeographyPosition position)
        {
            this.WritePoint(position.Latitude, position.Longitude, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void OnEndGeography()
        {
            this.EndGeo();
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        /// <returns>The type to be passed down the pipeline</returns>
        protected override SpatialType OnBeginGeometry(SpatialType type)
        {
            this.BeginGeo(type);
            return type;
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeometryPosition OnLineTo(GeometryPosition position)
        {
            this.WritePoint(position.X, position.Y, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void OnEndGeometry()
        {
            this.EndGeo();
        }

        /// <summary>
        /// Set the coordinate system
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>The coordinateSystem to be passed down the pipeline</returns>
        protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            this.currentCoordinateSystem = coordinateSystem;
            return coordinateSystem;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeographyPosition OnBeginFigure(GeographyPosition position)
        {
            this.BeginFigure(position.Latitude, position.Longitude, position.Z, position.M);
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
            if (this.parentStack.Peek() == SpatialType.Polygon)
            {
                this.writer.WriteEndElement();
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        protected override void OnReset()
        {
            this.parentStack = new Stack<SpatialType>();
            this.coordinateSystemWritten = false;
            this.currentCoordinateSystem = null;
            this.figureWritten = false;
            this.shouldWriteContainerWrapper = false;
        }

        /// <summary>
        /// Begin a figure
        /// </summary>
        /// <param name="x">The first coordinate</param>
        /// <param name="y">The second coordinate</param>
        /// <param name="z">The optional third coordinate</param>
        /// <param name="m">The optional fourth coordinate</param>
        private void BeginFigure(double x, double y, double? z, double? m)
        {
            if (this.parentStack.Peek() == SpatialType.Polygon)
            {
                // first figure is exterior, all subsequent figures are interior
                this.WriteStartElement(this.figureWritten ? GmlConstants.InteriorRing : GmlConstants.ExteriorRing);
                this.WriteStartElement(GmlConstants.LinearRing);
            }

            this.figureWritten = true;
            this.WritePoint(x, y, z, m);
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        private void BeginGeo(SpatialType type)
        {
            if (this.shouldWriteContainerWrapper)
            {
                Debug.Assert(this.parentStack.Count > 0, "should be inside a container");

                // when the first shape in the collection is beginning, write the container's wrapper
                switch (this.parentStack.Peek())
                {
                    case SpatialType.MultiPoint:
                        this.WriteStartElement(GmlConstants.PointMembers);
                        break;
                    case SpatialType.MultiLineString:
                        this.WriteStartElement(GmlConstants.LineStringMembers);
                        break;
                    case SpatialType.MultiPolygon:
                        this.WriteStartElement(GmlConstants.PolygonMembers);
                        break;
                    case SpatialType.Collection:
                        this.WriteStartElement(GmlConstants.CollectionMembers);
                        break;
                    default:
                        Debug.Assert(false, "unknown container type, incorrect flag status");
                        break;
                }

                this.shouldWriteContainerWrapper = false;
            }

            this.figureWritten = false;
            this.parentStack.Push(type);

            switch (type)
            {
                case SpatialType.Point:
                    this.WriteStartElement(GmlConstants.Point);
                    break;
                case SpatialType.LineString:
                    this.WriteStartElement(GmlConstants.LineString);
                    break;
                case SpatialType.Polygon:
                    this.WriteStartElement(GmlConstants.Polygon);
                    break;
                case SpatialType.MultiPoint:
                    this.shouldWriteContainerWrapper = true;
                    this.WriteStartElement(GmlConstants.MultiPoint);
                    break;
                case SpatialType.MultiLineString:
                    this.shouldWriteContainerWrapper = true;
                    this.WriteStartElement(GmlConstants.MultiLineString);
                    break;
                case SpatialType.MultiPolygon:
                    this.shouldWriteContainerWrapper = true;
                    this.WriteStartElement(GmlConstants.MultiPolygon);
                    break;
                case SpatialType.Collection:
                    this.shouldWriteContainerWrapper = true;
                    this.WriteStartElement(GmlConstants.Collection);
                    break;
                case SpatialType.FullGlobe:
                    this.writer.WriteStartElement(GmlConstants.FullGlobe, GmlConstants.FullGlobeNamespace);
                    break;
                default:
                    throw new NotSupportedException(Strings.Validator_InvalidType(type));
            }

            this.WriteCoordinateSystem();
        }

        /// <summary>
        /// Write the element with namespaces
        /// </summary>
        /// <param name="elementName">The element name</param>
        private void WriteStartElement(string elementName)
        {
            this.writer.WriteStartElement(GmlConstants.GmlPrefix, elementName, GmlConstants.GmlNamespace);
        }

        /// <summary>
        /// Write coordinate system
        /// </summary>
        private void WriteCoordinateSystem()
        {
            Debug.Assert(this.writer.WriteState == WriteState.Element, "Writer at element start");

            if (!coordinateSystemWritten && this.currentCoordinateSystem != null)
            {
                this.coordinateSystemWritten = true;
                var crsValue = GmlConstants.SrsPrefix + this.currentCoordinateSystem.Id;
                this.writer.WriteAttributeString(GmlConstants.GmlPrefix, GmlConstants.SrsName, GmlConstants.GmlNamespace, crsValue);
            }
        }

        /// <summary>
        /// Write a Point
        /// </summary>
        /// <param name="x">The first coordinate</param>
        /// <param name="y">The second coordinate</param>
        /// <param name="z">The optional third coordinate</param>
        /// <param name="m">The optional fourth coordinate</param>
        private void WritePoint(double x, double y, double? z, double? m)
        {
            this.WriteStartElement(GmlConstants.Position);

            this.writer.WriteValue(x);
            this.writer.WriteValue(" ");
            this.writer.WriteValue(y);

            if (z.HasValue)
            {
                this.writer.WriteValue(" ");
                this.writer.WriteValue(z.Value);

                if (m.HasValue)
                {
                    this.writer.WriteValue(" ");
                    this.writer.WriteValue(m.Value);
                }
            }
            else
            {
                if (m.HasValue)
                {
                    this.writer.WriteValue(" ");
                    this.writer.WriteValue(double.NaN);
                    this.writer.WriteValue(" ");
                    this.writer.WriteValue(m.Value);
                }
            }

            this.writer.WriteEndElement();
        }

        /// <summary>
        /// End Geography/Geometry
        /// </summary>
        private void EndGeo()
        {
            SpatialType type = this.parentStack.Pop();

            switch (type)
            {
                case SpatialType.MultiPoint:
                case SpatialType.MultiLineString:
                case SpatialType.MultiPolygon:
                case SpatialType.Collection:
                    if (!this.shouldWriteContainerWrapper)
                    {
                        // container wrapper has been written, end it
                        this.writer.WriteEndElement();
                    }

                    this.writer.WriteEndElement();

                    // On EndGeo we should turn off wrapper, nested empty collection types relies on this call.
                    this.shouldWriteContainerWrapper = false;
                    break;
                case SpatialType.Point:
                    if (!figureWritten)
                    {
                        // empty point needs an empty pos
                        this.WriteStartElement(GmlConstants.Position);
                        this.writer.WriteEndElement();
                    }

                    this.writer.WriteEndElement();
                    break;
                case SpatialType.LineString:
                    if (!figureWritten)
                    {
                        // empty linestring needs an empty posList
                        this.WriteStartElement(GmlConstants.PositionList);
                        this.writer.WriteEndElement();
                    }

                    this.writer.WriteEndElement();
                    break;
                case SpatialType.Polygon:
                case SpatialType.FullGlobe:
                    this.writer.WriteEndElement();
                    break;
                default:
                    Debug.Assert(false, "Unknown type in stack, mismatch switch between BeginGeo() and EndGeo()?");
                    break;
            }
        }
    }
}
