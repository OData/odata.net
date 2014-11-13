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

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Spatial;
    using System.Text;

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
