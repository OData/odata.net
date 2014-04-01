//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
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

        /// <summary> Initializes a new instance of the <see cref="T:Microsoft.Spatial.SpatialPipeline" /> class. </summary>
        public SpatialPipeline()
        {
            this.startingLink = this;
        }

        /// <summary> Initializes a new instance of the <see cref="T:Microsoft.Spatial.SpatialPipeline" /> class. </summary>
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
        /// Performs an implicit conversion from <see cref="Microsoft.Spatial.SpatialPipeline"/> to <see cref="Microsoft.Spatial.GeographyPipeline"/>.
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
        /// Performs an implicit conversion from <see cref="Microsoft.Spatial.SpatialPipeline"/> to <see cref="Microsoft.Spatial.GeometryPipeline"/>.
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
