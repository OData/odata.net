//---------------------------------------------------------------------
// <copyright file="GeographyMultiPointImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geography Multi-Point
    /// </summary>
    internal class GeographyMultiPointImplementation : GeographyMultiPoint
    {
        /// <summary>
        /// Points
        /// </summary>
        private GeographyPoint[] points;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">Points</param>
        internal GeographyMultiPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyPoint[] points)
            : base(coordinateSystem, creator)
        {
            this.points = points ?? new GeographyPoint[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">Points</param>
        internal GeographyMultiPointImplementation(SpatialImplementation creator, params GeographyPoint[] points)
            : this(CoordinateSystem.DefaultGeography, creator, points)
        {
        }

        /// <summary>
        /// Is MultiPoint Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.points.Length == 0;
            }
        }

        /// <summary>
        /// Geography
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.points); }
        }

        /// <summary>
        /// Points
        /// </summary>
        public override ReadOnlyCollection<GeographyPoint> Points
        {
            get { return new ReadOnlyCollection<GeographyPoint>(this.points); }
        }

        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.MultiPoint);

            for (int i = 0; i < this.points.Length; ++i)
            {
                this.points[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
