//---------------------------------------------------------------------
// <copyright file="SpatialFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base Spatial Factory
    /// </summary>
    public abstract class SpatialFactory
    {
        /// <summary>
        /// Stack of Containers
        /// </summary>
        private Stack<SpatialType> containers;

        /// <summary>
        /// Whether a figure has been started
        /// </summary>
        private bool figureDrawn;

        /// <summary>
        /// Inside a Polygon Ring
        /// </summary>
        private bool inRing;

        /// <summary>
        /// Current polygon ring has been closed
        /// </summary>
        private bool ringClosed;

        /// <summary>
        /// X coordinate of the current ring's starting position
        /// </summary>
        private double ringStartX;

        /// <summary>
        /// Y coordinate of the current ring's starting position
        /// </summary>
        private double ringStartY;

        /// <summary>
        /// Z coordinate of the current ring's starting position
        /// </summary>
        private double? ringStartZ;

        /// <summary>
        /// M coordinate of the current ring's starting position
        /// </summary>
        private double? ringStartM;

        /// <summary>
        /// Initializes a new instance of the BaseSpatialFactory class
        /// </summary>
        internal SpatialFactory()
        {
            this.containers = new Stack<SpatialType>();
        }

        /// <summary>
        /// Gets the current container Definition
        /// </summary>
        private SpatialType CurrentType
        {
            get
            {
                if (this.containers.Count == 0)
                {
                    return SpatialType.Unknown;
                }
                else
                {
                    return this.containers.Peek();
                }
            }
        }

        /// <summary>
        /// Begin Geo
        /// </summary>
        /// <param name="type">The spatial type</param>
        protected virtual void BeginGeo(SpatialType type)
        {
            // close on nesting types until we find a container suitable for the current type
            while (!this.CanContain(type))
            {
                this.EndGeo();
            }

            this.containers.Push(type);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="x">X or Latitude Coordinate</param>
        /// <param name="y">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected virtual void BeginFigure(double x, double y, double? z, double? m)
        {
            Debug.Assert(!this.figureDrawn, "Figure already started");
            this.figureDrawn = true;
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="x">X or Latitude Coordinate</param>
        /// <param name="y">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected virtual void AddLine(double x, double y, double? z, double? m)
        {
            Debug.Assert(this.figureDrawn, "Figure not yet started");

            if (this.inRing)
            {
                this.ringClosed = x == this.ringStartX && y == this.ringStartY;
            }
        }

        /// <summary>
        /// Ends the figure set on the current node
        /// </summary>
        protected virtual void EndFigure()
        {
            Debug.Assert(this.figureDrawn, "Figure not yet started");
            if (this.inRing)
            {
                if (!this.ringClosed)
                {
                    this.AddLine(this.ringStartX, this.ringStartY, this.ringStartZ, this.ringStartM);
                }

                this.inRing = false;
                this.ringClosed = true;
            }

            this.figureDrawn = false;
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected virtual void EndGeo()
        {
            if (this.figureDrawn)
            {
                this.EndFigure();
            }

            this.containers.Pop();
        }

        /// <summary>
        /// Finish the current instance
        /// </summary>
        protected virtual void Finish()
        {
            while (this.containers.Count > 0)
            {
                this.EndGeo();
            }
        }

        /// <summary>
        /// Add a new position to the current line figure
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected virtual void AddPos(double x, double y, double? z, double? m)
        {
            if (!this.figureDrawn)
            {
                this.BeginFigure(x, y, z, m);
            }
            else
            {
                this.AddLine(x, y, z, m);
            }
        }

        /// <summary>
        /// Start a new polygon ring
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        protected virtual void StartRing(double x, double y, double? z, double? m)
        {
            if (this.figureDrawn)
            {
                this.EndFigure();
            }

            this.BeginFigure(x, y, z, m);

            this.ringStartX = x;
            this.ringStartY = y;
            this.ringStartM = m;
            this.ringStartZ = z;
            this.inRing = true;
            this.ringClosed = false;
        }

        /// <summary>
        /// Can the current container contain the spatial type
        /// </summary>
        /// <param name="type">The spatial type to test</param>
        /// <returns>A boolean value indicating whether the current container can contain the spatial type</returns>
        private bool CanContain(SpatialType type)
        {
            switch (this.CurrentType)
            {
                case SpatialType.Unknown:
                case SpatialType.Collection:
                    // top level or collection
                    return true;
                case SpatialType.MultiPoint:
                    return type == SpatialType.Point;
                case SpatialType.MultiLineString:
                    return type == SpatialType.LineString;
                case SpatialType.MultiPolygon:
                    return type == SpatialType.Polygon;
                default:
                    return false;
            }
        }
    }
}