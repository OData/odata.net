//---------------------------------------------------------------------
// <copyright file="GeoJsonObjectFormatterImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.Generic;

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
