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
    using System.Collections.ObjectModel;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    using System.Spatial;

    /// <summary>
    /// Geometry Multi-Point
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    internal class GeometryMultiPointImplementation : GeometryMultiPoint
    {
        /// <summary>
        /// Points
        /// </summary>
        private GeometryPoint[] points;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">Points</param>
        internal GeometryMultiPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeometryPoint[] points)
            : base(coordinateSystem, creator)
        {
            this.points = points ?? new GeometryPoint[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="points">Points</param>
        internal GeometryMultiPointImplementation(SpatialImplementation creator, params GeometryPoint[] points)
            : this(CoordinateSystem.DefaultGeometry, creator, points)
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
        /// Geometry
        /// </summary>
        public override ReadOnlyCollection<Geometry> Geometries
        {
            get { return new ReadOnlyCollection<Geometry>(this.points); }
        }

        /// <summary>
        /// Points
        /// </summary>
        public override ReadOnlyCollection<GeometryPoint> Points
        {
            get { return new ReadOnlyCollection<GeometryPoint>(this.points); }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// internal GeometryPoint array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal GeometryPoint[] PointsArray
        {
            get { return this.points; }

            set { this.points = value; }
        }
#endif
        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.MultiPoint);

            for (int i = 0; i < this.points.Length; ++i)
            {
                this.points[i].SendTo(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
