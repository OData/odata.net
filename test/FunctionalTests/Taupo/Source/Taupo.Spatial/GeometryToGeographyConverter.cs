//---------------------------------------------------------------------
// <copyright file="GeometryToGeographyConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default implementation of the geometry to geography converter
    /// </summary>
    [ImplementationName(typeof(IGeometryToGeographyConverter), "Default")]
    public class GeometryToGeographyConverter : IGeometryToGeographyConverter
    {
        /// <summary>
        /// Converts the given geometry into a geography.
        /// </summary>
        /// <param name="geometry">The geometry to convert.</param>
        /// <param name="treatXCoordinateAsLatitude">if set to <c>true</c> the x coordinate should be converted to latitude. Otherwise, it should be longitude.</param>
        /// <returns>The converted geography</returns>
        public object Convert(object geometry, bool treatXCoordinateAsLatitude)
        {
            ExceptionUtilities.CheckArgumentNotNull(geometry, "geometry");
            var afterCast = geometry as Geometry;
            ExceptionUtilities.CheckObjectNotNull(afterCast, "Given object '{0}' was not of type {1}", geometry, typeof(Geometry));

            var builder = SpatialBuilder.Create();
            var chain = CreateConversionChain(builder, GetConvertDelegate(treatXCoordinateAsLatitude));
            afterCast.SendTo(chain);
            return builder.ConstructedGeography;
        }

        /// <summary>
        /// Creates the conversion chain.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="convertPosition">The delegate to use for converting positions.</param>
        /// <returns>The conversion chain</returns>
        internal static GeometryPipeline CreateConversionChain(GeographyPipeline destination, Func<GeometryPosition, GeographyPosition> convertPosition)
        {
            return new GeometryToGeographyPipeline(destination, convertPosition);
        }

        /// <summary>
        /// Gets the convert delegate given whether x is longitude or latitude.
        /// </summary>
        /// <param name="treatXCoordinateAsLatitude">if set to <c>true</c> the x coordinate should be converted to latitude. Otherwise, it should be longitude.</param>
        /// <returns>The convert delegate</returns>
        internal static Func<GeometryPosition, GeographyPosition> GetConvertDelegate(bool treatXCoordinateAsLatitude)
        {
            if (treatXCoordinateAsLatitude)
            {
                return ConvertWithXCoordinateAsLatitude;
            }
            else
            {
                return ConvertWithYCoordinateAsLatitude;
            }
        }

        /// <summary>
        /// Converts the position with the X coordinate as latitude.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>The converted position</returns>
        internal static GeographyPosition ConvertWithXCoordinateAsLatitude(GeometryPosition position)
        {
            return new GeographyPosition(position.X, position.Y, position.Z, position.M);
        }

        /// <summary>
        /// Converts the position with the Y coordinate as latitude.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>The converted position</returns>
        internal static GeographyPosition ConvertWithYCoordinateAsLatitude(GeometryPosition position)
        {
            return new GeographyPosition(position.Y, position.X, position.Z, position.M);
        }

        /// <summary>
        /// Geometry chain which converts geometry to geography given a geography chain to send to and a delegate to convert positions
        /// </summary>
        internal class GeometryToGeographyPipeline : GeometryPipeline
        {
            public GeometryToGeographyPipeline(GeographyPipeline destination, Func<GeometryPosition, GeographyPosition> convertPosition)
            {
                ExceptionUtilities.CheckArgumentNotNull(destination, "destination");
                ExceptionUtilities.CheckArgumentNotNull(convertPosition, "convertPosition");
                this.Destination = destination;
                this.ConvertPosition = convertPosition;
            }

            /// <summary>
            /// Gets the delegate for converting positions.
            /// </summary>
            internal Func<GeometryPosition, GeographyPosition> ConvertPosition { get; private set; }

            /// <summary>
            /// Gets the destination chain.
            /// </summary>
            internal GeographyPipeline Destination { get; private set; }

            /// <summary>
            /// Adds a geometric control point.
            /// </summary>
            /// <param name="position">The position of the control point.</param>
            public override void LineTo(GeometryPosition position)
            {
                this.Destination.LineTo(this.ConvertPosition(position));
            }

            /// <summary>
            /// Begins a figure.
            /// </summary>
            /// <param name="position">The position of the control point.</param>
            public override void BeginFigure(GeometryPosition position)
            {
                this.Destination.BeginFigure(this.ConvertPosition(position));
            }

            /// <summary>
            /// Begins a geometry.
            /// </summary>
            /// <param name="type">The type of the geometry.</param>
            public override void BeginGeometry(SpatialType type)
            {
                this.Destination.BeginGeography(type);
            }

            /// <summary>
            /// Ends the current figure.
            /// </summary>
            public override void EndFigure()
            {
                this.Destination.EndFigure();
            }

            /// <summary>
            /// Ends the current geometry.
            /// </summary>
            public override void EndGeometry()
            {
                this.Destination.EndGeography();
            }

            /// <summary>
            /// Resets this chain.
            /// </summary>
            public override void Reset()
            {
                this.Destination.Reset();
            }

            /// <summary>
            /// Sets the coordinate system.
            /// </summary>
            /// <param name="coordinateSystem">The coordinate system.</param>
            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                this.Destination.SetCoordinateSystem(coordinateSystem);
            }
        }
    }
}
