//---------------------------------------------------------------------
// <copyright file="SpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Represents the base class for all Spatial Formats.</summary>
        /// <typeparam name="TReaderStream">The type of reader to be read from.</typeparam>
        /// <typeparam name="TWriterStream">The type of reader to be read from.</typeparam>
    public abstract class SpatialFormatter<TReaderStream, TWriterStream>
    {
        /// <summary>
        /// The implementation that created this instance.
        /// </summary>
        private readonly SpatialImplementation creator;

        /// <summary>Initializes a new instance of the &lt;see cref="Microsoft.Spatial.SpatialFormatter`2" /&gt; class. </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected SpatialFormatter(SpatialImplementation creator)
        {
            Util.CheckArgumentNull(creator, "creator");
            this.creator = creator;
        }

        /// <summary> Parses the input, and produces the object.</summary>
        /// <returns>The input.</returns>
        /// <param name="input">The input to be parsed.</param>
        /// <typeparam name="TResult">The type of object to produce.</typeparam>
        public TResult Read<TResult>(TReaderStream input) where TResult : class, ISpatial
        {
            var trans = MakeValidatingBuilder();
            IShapeProvider parsePipelineEnd = trans.Value;

            Read<TResult>(input, trans.Key);
            if (typeof(Geometry).IsAssignableFrom(typeof(TResult)))
            {
                return (TResult)(object)parsePipelineEnd.ConstructedGeometry;
            }
            else
            {
                return (TResult)(object)parsePipelineEnd.ConstructedGeography;
            }
        }

        /// <summary> Parses the input, and produces the object.</summary>
        /// <param name="input">The input to be parsed.</param>
        /// <param name="pipeline">The pipeline to call during reading.</param>
        /// <typeparam name="TResult">The type of object to produce.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type hierarchy is too deep to have a specificly typed Read for each of them.")]
        public void Read<TResult>(TReaderStream input, SpatialPipeline pipeline) where TResult : class, ISpatial
        {
            if (typeof(Geometry).IsAssignableFrom(typeof(TResult)))
            {
                ReadGeometry(input, pipeline);
            }
            else
            {
                ReadGeography(input, pipeline);
            }
        }

        /// <summary> Creates a valid format from the spatial object.</summary>
        /// <param name="spatial">The object that the format is being created for.</param>
        /// <param name="writerStream">The stream to write the formatted object to.</param>
        public void Write(ISpatial spatial, TWriterStream writerStream)
        {
            var writer = this.CreateWriter(writerStream);
            spatial.SendTo(writer);
        }

        /// <summary> Creates the writerStream. </summary>
        /// <returns>The writerStream that was created.</returns>
        /// <param name="writerStream">The stream that should be written to.</param>
        public abstract SpatialPipeline CreateWriter(TWriterStream writerStream);

        /// <summary> Reads the Geography from the readerStream and call the appropriate pipeline methods.</summary>
        /// <param name="readerStream">The stream to read from.</param>
        /// <param name="pipeline">The pipeline to call based on what is read.</param>
        protected abstract void ReadGeography(TReaderStream readerStream, SpatialPipeline pipeline);

        /// <summary> Reads the Geometry from the readerStream and call the appropriate pipeline methods.</summary>
        /// <param name="readerStream">The stream to read from.</param>
        /// <param name="pipeline">The pipeline to call based on what is read.</param>
        protected abstract void ReadGeometry(TReaderStream readerStream, SpatialPipeline pipeline);

        /// <summary> Creates the builder that will be called by the parser to build the new type. </summary>
        /// <returns>The builder that was created.</returns>
        protected KeyValuePair<SpatialPipeline, IShapeProvider> MakeValidatingBuilder()
        {
            var builder = this.creator.CreateBuilder();
            var validator = this.creator.CreateValidator();
            validator.ChainTo(builder);
            return new KeyValuePair<SpatialPipeline, IShapeProvider>(validator, builder);
        }
    }
}
