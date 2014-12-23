//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Spatial.SpatialBuilder" /> class.</summary>
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
