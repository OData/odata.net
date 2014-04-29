//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System.Collections.ObjectModel;
    using Microsoft.Spatial;

    /// <summary>
    /// Geometry Line String
    /// </summary>
    internal class GeometryLineStringImplementation : GeometryLineString
    {
        /// <summary>
        /// Points array
        /// </summary>
        private GeometryPoint[] points;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">The point list</param>
        internal GeometryLineStringImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeometryPoint[] points)
            : base(coordinateSystem, creator)
        {
            this.points = points ?? new GeometryPoint[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">The point list</param>
        internal GeometryLineStringImplementation(SpatialImplementation creator, params GeometryPoint[] points)
            : this(CoordinateSystem.DefaultGeometry, creator, points)
        {
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
        public override ReadOnlyCollection<GeometryPoint> Points
        {
            get
            {
                return new ReadOnlyCollection<GeometryPoint>(this.points);
            }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.LineString);
            this.SendFigure(pipeline);
            pipeline.EndGeometry();
        }
    }
}
