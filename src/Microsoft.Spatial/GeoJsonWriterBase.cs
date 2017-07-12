//---------------------------------------------------------------------
// <copyright file="GeoJsonWriterBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Base Writer for GeoJson
    /// </summary>
    internal abstract class GeoJsonWriterBase : DrawBoth
    {
        /// <summary>
        /// Stack to track the current type being written.
        /// </summary>
        private readonly Stack<SpatialType> stack;

        /// <summary>
        /// CoordinateSystem for the types being written.
        /// </summary>
        private CoordinateSystem currentCoordinateSystem;

        /// <summary>
        /// Figure added in current shape
        /// </summary>
        private bool figureDrawn;

        /// <summary>
        /// Creates a new instance of the GeoJsonWriter.
        /// </summary>
        public GeoJsonWriterBase()
        {
            this.stack = new Stack<SpatialType>();
        }

        /// <summary>
        /// True if the shape should write start and end object scope, otherwise false.
        /// </summary>
        private bool ShapeHasObjectScope
        {
            get
            {
                return this.IsTopLevel || this.stack.Peek() == SpatialType.Collection;
            }
        }

        /// <summary>
        /// True if the shape is not a child of another shape.
        /// </summary>
        private bool IsTopLevel
        {
            get
            {
                return this.stack.Count == 0;
            }
        }

        /// <summary>
        /// True if the shape should write start and end object scope, otherwise false.
        /// </summary>
        private bool FigureHasArrayScope
        {
            get
            {
                return this.stack.Peek() != SpatialType.Point;
            }
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
            // GeoJSON specification is that longitude is first
            WriteControlPoint(position.Longitude, position.Latitude, position.Z, position.M);
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
            this.WriteControlPoint(position.X, position.Y, position.Z, position.M);
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
            BeginShape(type, CoordinateSystem.DefaultGeography);
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
            BeginShape(type, CoordinateSystem.DefaultGeometry);
            return type;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeographyPosition OnBeginFigure(GeographyPosition position)
        {
            BeginFigure();
            WriteControlPoint(position.Longitude, position.Latitude, position.Z, position.M);
            return position;
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        /// <returns>The position to be passed down the pipeline</returns>
        protected override GeometryPosition OnBeginFigure(GeometryPosition position)
        {
            BeginFigure();
            this.WriteControlPoint(position.X, position.Y, position.Z, position.M);
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
            EndShape();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        protected override void OnEndGeometry()
        {
            EndShape();
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
            SetCoordinateSystem(coordinateSystem);
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

        #region Abstract Defs

        /// <summary>
        /// Add a property name to the current json object
        /// </summary>
        /// <param name="name">The name to add</param>
        protected abstract void AddPropertyName(String name);

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected abstract void AddValue(String value);

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected abstract void AddValue(double value);

        /// <summary>
        /// Start a new json object scope
        /// </summary>
        protected abstract void StartObjectScope();

        /// <summary>
        /// Start a new json array scope
        /// </summary>
        protected abstract void StartArrayScope();

        /// <summary>
        /// End the current json object scope
        /// </summary>
        protected abstract void EndObjectScope();

        /// <summary>
        /// End the current json array scope
        /// </summary>
        protected abstract void EndArrayScope();

        #endregion

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        protected virtual void Reset()
        {
            this.stack.Clear();
            this.currentCoordinateSystem = default(CoordinateSystem);
        }

        #region Private Methods

        /// <summary>
        /// Gets the GeoJson type name to use when writing the specified type.
        /// </summary>
        /// <param name="type">SpatialType being written.</param>
        /// <returns>GeoJson type name corresponding to the specified <paramref name="type"/>.</returns>
        private static string GetSpatialTypeName(SpatialType type)
        {
            switch (type)
            {
                case SpatialType.Point:
                    return GeoJsonConstants.TypeMemberValuePoint;
                case SpatialType.LineString:
                    return GeoJsonConstants.TypeMemberValueLineString;
                case SpatialType.Polygon:
                    return GeoJsonConstants.TypeMemberValuePolygon;
                case SpatialType.MultiPoint:
                    return GeoJsonConstants.TypeMemberValueMultiPoint;
                case SpatialType.MultiLineString:
                    return GeoJsonConstants.TypeMemberValueMultiLineString;
                case SpatialType.MultiPolygon:
                    return GeoJsonConstants.TypeMemberValueMultiPolygon;
                case SpatialType.Collection:
                    return GeoJsonConstants.TypeMemberValueGeometryCollection;
                default:
                    Debug.Assert(false, "SpatialType " + type + " is not currently supported in GeoJsonWriter.");
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the name of the GeoJson member to use when writing the body of the spatial object.
        /// </summary>
        /// <param name="type">SpatialType being written.</param>
        /// <returns>Name of the GeoJson member to use when writing the body of the spatial object.</returns>
        private static string GetDataName(SpatialType type)
        {
            switch (type)
            {
                case SpatialType.Point:
                case SpatialType.LineString:
                case SpatialType.Polygon:
                case SpatialType.MultiPoint:
                case SpatialType.MultiLineString:
                case SpatialType.MultiPolygon:
                    return GeoJsonConstants.CoordinatesMemberName;
                case SpatialType.Collection:
                    return GeoJsonConstants.GeometriesMemberName;
                default:
                    Debug.Assert(false, "SpatialType " + type + " is not currently supported in GeoJsonWriter.");
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Whether or not the specified type wraps its data in an outer array.
        /// </summary>
        /// <param name="type">SpatialType being written.</param>
        /// <returns>True if the type uses an outer array, otherwise false.</returns>
        private static bool TypeHasArrayScope(SpatialType type)
        {
            return type != SpatialType.Point && type != SpatialType.LineString;
        }

        /// <summary>
        /// Sets the CoordinateSystem for Geography and Geometry shapes.
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem value to set.</param>
        private void SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            this.currentCoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Start writing a Geography or Geometry shape.
        /// </summary>
        /// <param name="type">SpatialType to use when writing the shape.</param>
        /// <param name="defaultCoordinateSystem">Default CoordinateSystem to use if SetCoordinateSystem is never called on this shape.</param>
        private void BeginShape(SpatialType type, CoordinateSystem defaultCoordinateSystem)
        {
            if (this.currentCoordinateSystem == null)
            {
                this.currentCoordinateSystem = defaultCoordinateSystem;
            }

            if (this.ShapeHasObjectScope)
            {
                this.WriteShapeHeader(type);
            }

            if (TypeHasArrayScope(type))
            {
                this.StartArrayScope();
            }

            this.stack.Push(type);
            this.figureDrawn = false;
        }

        /// <summary>
        /// Write the type header information for a shape.
        /// </summary>
        /// <param name="type">SpatialType being written.</param>
        private void WriteShapeHeader(SpatialType type)
        {
            this.StartObjectScope();
            this.AddPropertyName(GeoJsonConstants.TypeMemberName);
            this.AddValue(GetSpatialTypeName(type));
            this.AddPropertyName(GetDataName(type));
        }

        /// <summary>
        /// Start writing a figure in a Geography or Geometry shape.
        /// </summary>
        private void BeginFigure()
        {
            if (this.FigureHasArrayScope)
            {
                this.StartArrayScope();
            }

            this.figureDrawn = true;
        }

        /// <summary>
        /// Write a position in a Geography or Geometry figure.
        /// </summary>
        /// <param name="first">First (X/Longitude) Coordinate</param>
        /// <param name="second">Second (Y/Latitude) Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        private void WriteControlPoint(double first, double second, double? z, double? m)
        {
            this.StartArrayScope();
            this.AddValue(first);
            this.AddValue(second);

            if (z.HasValue)
            {
                this.AddValue(z.Value);

                if (m.HasValue)
                {
                    this.AddValue(m.Value);
                }
            }
            else if (m.HasValue)
            {
                this.AddValue(null);
                this.AddValue(m.Value);
            }

            this.EndArrayScope();
        }

        /// <summary>
        /// Ends a Geography or Geometry figure.
        /// </summary>
        private void EndFigure()
        {
            if (this.FigureHasArrayScope)
            {
                this.EndArrayScope();
            }
        }

        /// <summary>
        /// Ends a Geography or Geometry shape.
        /// </summary>
        private void EndShape()
        {
            SpatialType type = this.stack.Pop();

            if (TypeHasArrayScope(type))
            {
                this.EndArrayScope();
            }
            else if (!this.figureDrawn)
            {
                // type hasn't write out at least one set of [] yet
                this.StartArrayScope();
                this.EndArrayScope();
            }

            if (this.IsTopLevel)
            {
                this.WriteCrs();
            }

            if (this.ShapeHasObjectScope)
            {
                this.EndObjectScope();
            }
        }

        /// <summary>
        /// Writes the coordinate reference system footer for the GeoJson object.
        /// </summary>
        private void WriteCrs()
        {
            // "crs": {
            //      "type": "name",
            //      "properties": {
            //          "name": "EPSG:4326"
            //      }
            // }
            this.AddPropertyName(GeoJsonConstants.CrsMemberName);
            this.StartObjectScope();
            this.AddPropertyName(GeoJsonConstants.TypeMemberName);
            this.AddValue(GeoJsonConstants.CrsTypeMemberValue);
            this.AddPropertyName(GeoJsonConstants.CrsPropertiesMemberName);
            this.StartObjectScope();
            this.AddPropertyName(GeoJsonConstants.CrsTypeMemberValue);
            this.AddValue(String.Concat(GeoJsonConstants.CrsValuePrefix, ':', this.currentCoordinateSystem.Id));
            this.EndObjectScope();
            this.EndObjectScope();
        }

        #endregion
    }
}
