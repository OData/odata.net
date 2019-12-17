//---------------------------------------------------------------------
// <copyright file="GmlFormatterImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Xml;

    /// <summary>
    /// The object to move spatial types to and from the GML format
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the accepted name in the industry")]
    internal class GmlFormatterImplementation : GmlFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GmlFormatterImplementation"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GmlFormatterImplementation(SpatialImplementation creator)
            : base(creator)
        {
        }

        /// <summary>
        /// Create the writer
        /// </summary>
        /// <param name="target">The object that should be the target of the ISpatialPipeline writer.</param>
        /// <returns>A writer that implements ISpatialPipeline.</returns>
        public override SpatialPipeline CreateWriter(XmlWriter target)
        {
            return new ForwardingSegment(new GmlWriter(target));
        }

        /// <summary>
        /// Reads the geography.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeography(XmlReader readerStream, SpatialPipeline pipeline)
        {
            new GmlReader(pipeline).ReadGeography(readerStream);
        }

        /// <summary>
        /// Reads the geometry.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected override void ReadGeometry(XmlReader readerStream, SpatialPipeline pipeline)
        {
            new GmlReader(pipeline).ReadGeometry(readerStream);
        }
    }
}
