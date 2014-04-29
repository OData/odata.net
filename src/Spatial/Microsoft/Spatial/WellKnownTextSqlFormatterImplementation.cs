//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System.IO;
    using Microsoft.Spatial;

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
