//---------------------------------------------------------------------
// <copyright file="SpatialBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Creates a geometry or geography instances from spatial data pipelines.</summary>
    public class SpatialBuilder : SpatialPipeline, IShapeProvider
    {
        /// <summary>
        ///   The builder to be delegated to when this class is accessed from the IGeographyPipeline or IGeographyProvider interfaces.
        /// </summary>
        private readonly IGeographyProvider geographyOutput;

        /// <summary>
        ///   The builder to be delegated to when this class is accessed from the IGeometryPipeline or IGeometryProvider interfaces.
        /// </summary>
        private readonly IGeometryProvider geometryOutput;

        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.SpatialBuilder" /> class.</summary>
        /// <param name="geographyInput">The geography input.</param>
        /// <param name="geometryInput">The geometry input.</param>
        /// <param name="geographyOutput">The geography output.</param>
        /// <param name="geometryOutput">The geometry output.</param>
        public SpatialBuilder(GeographyPipeline geographyInput, GeometryPipeline geometryInput, IGeographyProvider geographyOutput, IGeometryProvider geometryOutput)
            : base(geographyInput, geometryInput)
        {
            this.geographyOutput = geographyOutput;
            this.geometryOutput = geometryOutput;
        }

        /// <summary>Fires when the provider constructs geography object.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        public event Action<Geography> ProduceGeography
        {
            add { this.geographyOutput.ProduceGeography += value; }
            remove { this.geographyOutput.ProduceGeography -= value; }
        }

        /// <summary>Fires when the provider constructs geometry object.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        public event Action<Geometry> ProduceGeometry
        {
            add { this.geometryOutput.ProduceGeometry += value; }
            remove { this.geometryOutput.ProduceGeometry -= value; }
        }

        /// <summary>Gets the geography object that was constructed most recently.</summary>
        /// <returns>The geography object that was constructed.</returns>
        public Geography ConstructedGeography
        {
            get { return this.geographyOutput.ConstructedGeography; }
        }

        /// <summary>Gets the geometry object that was constructed most recently.</summary>
        /// <returns>The geometry object that was constructed.</returns>
        public Geometry ConstructedGeometry
        {
            get { return this.geometryOutput.ConstructedGeometry; }
        }

        /// <summary>Creates an implementation of the builder.</summary>
        /// <returns>The created SpatialBuilder implementation.</returns>
        public static SpatialBuilder Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateBuilder();
        }
    }
}