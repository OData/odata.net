//---------------------------------------------------------------------
// <copyright file="GeometryBuilderImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Builder for Geometry types
    /// </summary>
    internal class GeometryBuilderImplementation : GeometryPipeline, IGeometryProvider
    {
        /// <summary>
        /// The tree builder
        /// </summary>
        private readonly SpatialTreeBuilder<Geometry> builder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        public GeometryBuilderImplementation(SpatialImplementation creator)
        {
            this.builder = new GeometryTreeBuilder(creator);
        }

        /// <summary>
        /// Fires when the provider constructs a geometry object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        public event Action<Geometry> ProduceGeometry
        {
            add { this.builder.ProduceInstance += value; }
            remove { this.builder.ProduceInstance -= value; }
        }

        /// <summary>
        /// Constructed Geography
        /// </summary>
        public Geometry ConstructedGeometry
        {
            get { return this.builder.ConstructedInstance; }
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
        public override void LineTo(GeometryPosition position)
        {
            Debug.Assert(position != null, "ForwardingSegment should have validated nullness");
            this.builder.LineTo(position.X, position.Y, position.Z, position.M);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
        public override void BeginFigure(GeometryPosition position)
        {
            Debug.Assert(position != null, "ForwardingSegment should have validated nullness");
            this.builder.BeginFigure(position.X, position.Y, position.Z, position.M);
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        public override void BeginGeometry(SpatialType type)
        {
            this.builder.BeginGeo(type);
        }

        /// <summary>
        /// Ends the current figure
        /// </summary>
        public override void EndFigure()
        {
            this.builder.EndFigure();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        public override void EndGeometry()
        {
            this.builder.EndGeo();
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        public override void Reset()
        {
            this.builder.Reset();
        }

        /// <summary>
        /// Set the coordinate system
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            Util.CheckArgumentNull(coordinateSystem, "coordinateSystem");
            this.builder.SetCoordinateSystem(coordinateSystem.EpsgId);
        }

        /// <summary>
        /// Geography Tree Builder
        /// </summary>
        private class GeometryTreeBuilder : SpatialTreeBuilder<Geometry>
        {
            /// <summary>
            /// The implementation that created this instance.
            /// </summary>
            private readonly SpatialImplementation creator;

            /// <summary>
            /// CoordinateSystem for the building geography
            /// </summary>
            private CoordinateSystem buildCoordinateSystem;

            /// <summary>
            /// Initializes a new instance of the <see cref="GeometryTreeBuilder"/> class.
            /// </summary>
            /// <param name="creator">The implementation that created this instance.</param>
            public GeometryTreeBuilder(SpatialImplementation creator)
            {
                Util.CheckArgumentNull(creator, "creator");
                this.creator = creator;
            }

            /// <summary>
            /// Set the coordinate system based on the given ID
            /// </summary>
            /// <param name="epsgId">The coordinate system ID to set. Null indicates the default should be used</param>
            internal override void SetCoordinateSystem(int? epsgId)
            {
                this.buildCoordinateSystem = CoordinateSystem.Geometry(epsgId);
            }

            /// <summary>
            /// Create a new instance of Point
            /// </summary>
            /// <param name="isEmpty">Whether the point is empty</param>
            /// <param name="x">X</param>
            /// <param name="y">Y</param>
            /// <param name="z">Z</param>
            /// <param name="m">M</param>
            /// <returns>A new instance of point</returns>
            protected override Geometry CreatePoint(bool isEmpty, double x, double y, double? z, double? m)
            {
                return isEmpty ? new GeometryPointImplementation(this.buildCoordinateSystem, this.creator) : new GeometryPointImplementation(this.buildCoordinateSystem, this.creator, x, y, z, m);
            }

            /// <summary>
            /// Create a new instance of T
            /// </summary>
            /// <param name="type">The spatial type to create</param>
            /// <param name="spatialData">The arguments</param>
            /// <returns>A new instance of T</returns>
            protected override Geometry CreateShapeInstance(SpatialType type, IEnumerable<Geometry> spatialData)
            {
                switch (type)
                {
                    case SpatialType.LineString:
                        return new GeometryLineStringImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPoint>().ToArray());
                    case SpatialType.Polygon:
                        return new GeometryPolygonImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryLineString>().ToArray());
                    case SpatialType.MultiPoint:
                        return new GeometryMultiPointImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPoint>().ToArray());
                    case SpatialType.MultiLineString:
                        return new GeometryMultiLineStringImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryLineString>().ToArray());
                    case SpatialType.MultiPolygon:
                        return new GeometryMultiPolygonImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPolygon>().ToArray());
                    case SpatialType.Collection:
                        return new GeometryCollectionImplementation(this.buildCoordinateSystem, this.creator, spatialData.ToArray());
                    default:
                        Debug.Assert(false, "Point type should not call CreateShapeInstance, use CreatePoint instead.");
                        return null;
                }
            }
        }
    }
}
