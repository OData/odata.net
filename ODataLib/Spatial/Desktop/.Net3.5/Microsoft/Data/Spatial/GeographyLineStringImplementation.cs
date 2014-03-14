//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System.Collections.ObjectModel;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    using System.Spatial;

    /// <summary>
    /// A Geography linestring consist of an array of GeoPoints
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
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

#if WINDOWS_PHONE
        /// <summary>
        /// internal GeographyPoint array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal GeographyPoint[] PointsArray
        {
            get { return this.points; }

            set { this.points = value; }
        }
#endif

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
