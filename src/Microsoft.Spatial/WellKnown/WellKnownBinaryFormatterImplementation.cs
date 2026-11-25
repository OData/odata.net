//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryFormatterImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownBinary (WKB) formatter.
    /// </summary>
    internal class WellKnownBinaryFormatterImplementation : WellKnownBinaryFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownBinaryFormatterImplementation"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="settings">The setting for WKB writering.</param>
        internal WellKnownBinaryFormatterImplementation(SpatialImplementation creator, WellKnownBinaryWriterSettings settings)
            : base(creator)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Gets the WKB writer settings.
        /// </summary>
        protected WellKnownBinaryWriterSettings Settings { get; }

        /// <summary>
        /// Create the writer
        /// </summary>
        /// <param name="writerStream">The wrtier stream.</param>
        /// <returns>A writer that implements ISpatialPipeline.</returns>
        public override SpatialPipeline CreateWriter(Stream writerStream)
        {
            return new ForwardingSegment(new WellKnownBinaryWriter(writerStream, Settings));
        }

        /// <summary>
        /// Reads the geography.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeography(Stream readerStream, SpatialPipeline pipeline)
        {
            new WellKnownBinaryReader(pipeline).ReadGeography(readerStream);
        }

        /// <summary>
        /// Reads the geometry.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeometry(Stream readerStream, SpatialPipeline pipeline)
        {
            new WellKnownBinaryReader(pipeline).ReadGeometry(readerStream);
        }
    }
}
