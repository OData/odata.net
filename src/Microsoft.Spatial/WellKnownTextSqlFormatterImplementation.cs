//---------------------------------------------------------------------
// <copyright file="WellKnownTextSqlFormatterImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownTextSql format
    /// </summary>
    internal class WellKnownTextSqlFormatterImplementation : WellKnownTextSqlFormatter
    {
        /// <summary>
        /// restricts the writer and reader to allow only two dimensions.
        /// </summary>
        private readonly bool allowOnlyTwoDimensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlFormatterImplementation"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        internal WellKnownTextSqlFormatterImplementation(SpatialImplementation creator) : base(creator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlFormatterImplementation"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="allowOnlyTwoDimensions">restricts the reader to allow only two dimensions.</param>
        internal WellKnownTextSqlFormatterImplementation(SpatialImplementation creator, bool allowOnlyTwoDimensions) : base(creator)
        {
            this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
        }

        /// <summary>
        /// Create the writer
        /// </summary>
        /// <param name="target">The object that should be the target of the ISpatialPipeline writer.</param>
        /// <returns>A writer that implements ISpatialPipeline.</returns>
        public override SpatialPipeline CreateWriter(TextWriter target)
        {
            return new ForwardingSegment(new WellKnownTextSqlWriter(target, this.allowOnlyTwoDimensions));
        }

        /// <summary>
        /// Reads the geography.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeography(TextReader readerStream, SpatialPipeline pipeline)
        {
            new WellKnownTextSqlReader(pipeline, allowOnlyTwoDimensions).ReadGeography(readerStream);
        }

        /// <summary>
        /// Reads the geometry.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeometry(TextReader readerStream, SpatialPipeline pipeline)
        {
            new WellKnownTextSqlReader(pipeline, allowOnlyTwoDimensions).ReadGeometry(readerStream);
        }
    }
}
