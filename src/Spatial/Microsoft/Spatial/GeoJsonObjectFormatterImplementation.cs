//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.Data.Spatial
{
    using System.Collections.Generic;
    using Microsoft.Spatial;

    /// <summary>
    /// Formatter for Json Object
    /// </summary>
    internal class GeoJsonObjectFormatterImplementation : GeoJsonObjectFormatter
    {
        /// <summary>
        /// The implementation that created this instance.
        /// </summary>
        private readonly SpatialImplementation creator;
        
        /// <summary>
        /// Spatial builder
        /// </summary>
        private SpatialBuilder builder;

        /// <summary>
        /// The parse pipeline
        /// </summary>
        private SpatialPipeline parsePipeline;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The SpatialImplementation that created this instance</param>
        public GeoJsonObjectFormatterImplementation(SpatialImplementation creator)
        {
            this.creator = creator;
        }

        /// <summary>
        /// Read from the source
        /// </summary>
        /// <typeparam name="T">The spatial type to read</typeparam>
        /// <param name="source">The source json object</param>
        /// <returns>The read instance</returns>
        public override T Read<T>(IDictionary<string, object> source)
        {
            this.EnsureParsePipeline();
            if (typeof(Geometry).IsAssignableFrom(typeof(T)))
            {
                new GeoJsonObjectReader(this.builder).ReadGeometry(source);
                return this.builder.ConstructedGeometry as T;
            }

            new GeoJsonObjectReader(this.builder).ReadGeography(source);
            return this.builder.ConstructedGeography as T;
        }

        /// <summary>
        /// Convert spatial value to a Json Object
        /// </summary>
        /// <param name="value">The spatial value</param>
        /// <returns>The json object</returns>
        public override IDictionary<string, object> Write(ISpatial value)
        {
            var writer = new GeoJsonObjectWriter();
            value.SendTo(new ForwardingSegment(writer));
            return writer.JsonObject;
        }

        /// <summary> Creates the writerStream. </summary>
        /// <returns>The writerStream that was created.</returns>
        /// <param name="writer">The actual stream to write Json.</param>
        public override SpatialPipeline CreateWriter(IGeoJsonWriter writer)
        {
            return new ForwardingSegment(new WrappedGeoJsonWriter(writer));
        }

        /// <summary>
        /// Initialize the pipeline
        /// </summary>
        private void EnsureParsePipeline()
        {
            if (this.parsePipeline == null)
            {
                this.builder = this.creator.CreateBuilder();
                this.parsePipeline = this.creator.CreateValidator().ChainTo(this.builder);
            }
            else
            {
                this.parsePipeline.GeographyPipeline.Reset();
                this.parsePipeline.GeometryPipeline.Reset();
            }
        }
    }
}
