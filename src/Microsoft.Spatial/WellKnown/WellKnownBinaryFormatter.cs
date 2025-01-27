//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownBinary format
    /// </summary>
    public abstract class WellKnownBinaryFormatter : SpatialFormatter<Stream, Stream>
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="WellKnownBinaryFormatter"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected WellKnownBinaryFormatter(SpatialImplementation creator)
            : base(creator)
        {
        }

        /// <summary>
        /// Creates the implementation of the formatter.
        /// </summary>
        /// <returns>Returns the created <see cref="WellKnownBinaryFormatter"> implementation.</returns>
        public static WellKnownBinaryFormatter Create(WellKnownBinaryWriterSettings settings)
            => SpatialImplementation.CurrentImplementation.CreateWellKnownBinaryFormatter(settings);
    }
}
