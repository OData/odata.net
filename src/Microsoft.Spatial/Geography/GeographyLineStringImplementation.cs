//---------------------------------------------------------------------
// <copyright file="GeographyLineStringImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A Geography linestring consist of an array of GeoPoints
    /// </summary>
    internal class GeographyLineStringImplementation : GeographyLineString
    {
        /// <summary>
        /// Points array
        /// </summary>
        private GeographyPoint[] points;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">The point list</param>
        internal GeographyLineStringImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyPoint[] points)
            : base(coordinateSystem, creator)
        {
            this.points = points ?? new GeographyPoint[0];
        }

        /// <summary>
        /// Is LineString Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.points.Length == 0;
            }
        }

        /// <summary>
        /// Point list
        /// </summary>
        public override ReadOnlyCollection<GeographyPoint> Points
        {
            get
            {
                return new ReadOnlyCollection<GeographyPoint>(this.points);
            }
        }

        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.LineString);
            this.SendFigure(pipeline);
            pipeline.EndGeography();
        }
    }
}
