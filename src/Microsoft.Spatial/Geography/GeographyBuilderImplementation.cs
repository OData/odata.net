//---------------------------------------------------------------------
// <copyright file="GeographyBuilderImplementation.cs" company="Microsoft">
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
    /// Builder for Geography types
    /// </summary>
    internal class GeographyBuilderImplementation : GeographyPipeline, IGeographyProvider
    {
        /// <summary>
        /// The tree builder
        /// </summary>
        private readonly SpatialTreeBuilder<Geography> builder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        public GeographyBuilderImplementation(SpatialImplementation creator)
        {
            this.builder = new GeographyTreeBuilder(creator);
        }

        /// <summary>
        /// Fires when the provider constructs a geography object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        public event Action<Geography> ProduceGeography
        {
            add { this.builder.ProduceInstance += value; }
            remove { this.builder.ProduceInstance -= value; }
        }

        /// <summary>
        /// Constructed Geography
        /// </summary>
        public Geography ConstructedGeography
        {
            get { return this.builder.ConstructedInstance; }
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
        public override void LineTo(GeographyPosition position)
        {
            Debug.Assert(position != null, "ForwardingSegment should have validated nullness");
            this.builder.LineTo(position.Latitude, position.Longitude, position.Z, position.M);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
        public override void BeginFigure(GeographyPosition position)
        {
            Debug.Assert(position != null, "ForwardingSegment should have validated nullness");
            this.builder.BeginFigure(position.Latitude, position.Longitude, position.Z, position.M);
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        public override void BeginGeography(SpatialType type)
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
        public override void EndGeography()
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
        private class GeographyTreeBuilder : SpatialTreeBuilder<Geography>
        {
            /// <summary>
            /// The implementation that created this instance.
            /// </summary>
            private readonly SpatialImplementation creator;

            /// <summary>
            /// CoordinateSystem for the building geography
            /// </summary>
            private CoordinateSystem currentCoordinateSystem;

            /// <summary>
            /// Initializes a new instance of the <see cref="GeographyTreeBuilder"/> class.
            /// </summary>
            /// <param name="creator">The implementation that created this instance.</param>
            public GeographyTreeBuilder(SpatialImplementation creator)
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
                this.currentCoordinateSystem = CoordinateSystem.Geography(epsgId);
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
            protected override Geography CreatePoint(bool isEmpty, double x, double y, double? z, double? m)
            {
                return isEmpty ? new GeographyPointImplementation(this.currentCoordinateSystem, this.creator) : new GeographyPointImplementation(this.currentCoordinateSystem, this.creator, x, y, z, m);
            }

            /// <summary>
            /// Create a new instance of T
            /// </summary>
            /// <param name="type">The spatial type to create</param>
            /// <param name="spatialData">The arguments</param>
            /// <returns>A new instance of T</returns>
            protected override Geography CreateShapeInstance(SpatialType type, IEnumerable<Geography> spatialData)
            {
                switch (type)
                {
                    case SpatialType.LineString:
                        return new GeographyLineStringImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPoint>().ToArray());
                    case SpatialType.Polygon:
                        return new GeographyPolygonImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyLineString>().ToArray());
                    case SpatialType.MultiPoint:
                        return new GeographyMultiPointImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPoint>().ToArray());
                    case SpatialType.MultiLineString:
                        return new GeographyMultiLineStringImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyLineString>().ToArray());
                    case SpatialType.MultiPolygon:
                        return new GeographyMultiPolygonImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPolygon>().ToArray());
                    case SpatialType.Collection:
                        return new GeographyCollectionImplementation(this.currentCoordinateSystem, this.creator, spatialData.ToArray());
                    case SpatialType.FullGlobe:
                        return new GeographyFullGlobeImplementation(this.currentCoordinateSystem, this.creator);
                    default:
                        Debug.Assert(false, "Point type should not call CreateShapeInstance, use CreatePoint instead.");
                        return null;
                }
            }
        }
    }
}
