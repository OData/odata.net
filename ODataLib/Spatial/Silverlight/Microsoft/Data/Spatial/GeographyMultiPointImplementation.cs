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
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Spatial;

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
            get { return this.points.AsReadOnly<Geography>(); }
        }

        /// <summary>
        /// Points
        /// </summary>
        public override ReadOnlyCollection<GeographyPoint> Points
        {
            get { return this.points.AsReadOnly(); }
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
