//   OData .NET Libraries ver. 5.6.3
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

namespace System.Spatial
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// One link of a geospatial pipeline
    /// </summary>
    public class SpatialPipeline
    {
        /// <summary>
        /// the geography side of the pipeline
        /// </summary>
        private GeographyPipeline geographyPipeline;

        /// <summary>
        /// the geometry side of the pipeline
        /// </summary>
        private GeometryPipeline geometryPipeline;

        /// <summary>
        /// A reference to the begining link of the chain
        /// useful for getting the startingLink when creating the chain fluently
        /// e.g.  new ForwardingSegment(new Node()).ChainTo(new Node()).ChainTo(new Node).StartingLink
        /// </summary>
        private SpatialPipeline startingLink;

        /// <summary> Initializes a new instance of the <see cref="T:System.Spatial.SpatialPipeline" /> class. </summary>
        public SpatialPipeline()
        {
            this.startingLink = this;
        }

        /// <summary> Initializes a new instance of the <see cref="T:System.Spatial.SpatialPipeline" /> class. </summary>
        /// <param name="geographyPipeline">The geography chain.</param>
        /// <param name="geometryPipeline">The geometry chain.</param>
        public SpatialPipeline(GeographyPipeline geographyPipeline, GeometryPipeline geometryPipeline)
        {
            this.geographyPipeline = geographyPipeline;
            this.geometryPipeline = geometryPipeline;
            this.startingLink = this;
        }

        /// <summary> Gets the geography side of the pipeline. </summary>
        public virtual GeographyPipeline GeographyPipeline
        {
            get { return geographyPipeline; }
        }

        /// <summary> Gets the geometry side of the pipeline. </summary>
        public virtual GeometryPipeline GeometryPipeline
        {
            get { return geometryPipeline; }
        }

        /// <summary> Gets or sets the starting link. </summary>
        /// <returns> The starting link. </returns>
        public SpatialPipeline StartingLink
        {
            get { return this.startingLink; }
            set { this.startingLink = value; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Spatial.SpatialPipeline"/> to <see cref="System.Spatial.GeographyPipeline"/>.
        /// </summary>
        /// <param name="spatialPipeline">The spatial chain.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "we have DrawGeometry and DrawGeography properties that can be used instead of the implicit cast")]
        public static implicit operator GeographyPipeline(SpatialPipeline spatialPipeline)
        {
            if (spatialPipeline != null)
            {
                return spatialPipeline.GeographyPipeline;
            }
            
            return null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Spatial.SpatialPipeline"/> to <see cref="System.Spatial.GeometryPipeline"/>.
        /// </summary>
        /// <param name="spatialPipeline">The spatial chain.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "we have DrawGeometry and DrawGeography properties that can be used instead of the implicit cast")]
        public static implicit operator GeometryPipeline(SpatialPipeline spatialPipeline)
        {
            if (spatialPipeline != null)
            {
                return spatialPipeline.GeometryPipeline;
            }

            return null;
        }

        /// <summary> Adds the next pipeline.</summary>
        /// <returns>The last pipesegment in the chain, usually the one just created.</returns>
        /// <param name="destination">The next pipeline.</param>
        public virtual SpatialPipeline ChainTo(SpatialPipeline destination)
        {
            throw new NotImplementedException();
        }
    }
}
