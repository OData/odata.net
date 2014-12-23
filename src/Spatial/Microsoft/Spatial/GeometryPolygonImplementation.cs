//   OData .NET Libraries ver. 6.9
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
    using Microsoft.Spatial;

    /// <summary>
    /// Geometry polygon
    /// </summary>
    internal class GeometryPolygonImplementation : GeometryPolygon
    {
        /// <summary>
        /// Rings
        /// </summary>
        private GeometryLineString[] rings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeometryPolygonImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeometryLineString[] rings)
            : base(coordinateSystem, creator)
        {
            this.rings = rings ?? new GeometryLineString[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeometryPolygonImplementation(SpatialImplementation creator, params GeometryLineString[] rings)
            : this(CoordinateSystem.DefaultGeometry, creator, rings)
        {
        }

        /// <summary>
        /// Is Polygon Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.rings.Length == 0;
            }
        }

        /// <summary>
        /// Set of rings
        /// </summary>
        public override ReadOnlyCollection<GeometryLineString> Rings
        {
            get { return new ReadOnlyCollection<GeometryLineString>(this.rings); }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.Polygon);

            for (int i = 0; i < this.rings.Length; ++i)
            {
                this.rings[i].SendFigure(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
