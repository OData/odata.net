//---------------------------------------------------------------------
// <copyright file="GeographyFullGlobeImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Implementation of FullGlobe
    /// </summary>
    internal class GeographyFullGlobeImplementation : GeographyFullGlobe
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GeographyFullGlobeImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GeographyFullGlobeImplementation(SpatialImplementation creator)
            : this(CoordinateSystem.DefaultGeography, creator)
        {
        }

        /// <summary>
        /// Is FullGlobe empty
        /// </summary>
        public override bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Sends the spatial geography object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.FullGlobe);
            pipeline.EndGeography();
        }
    }
}