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
    /// Geography Multi-Polygon
    /// </summary>
    internal class GeographyMultiPolygonImplementation : GeographyMultiPolygon
    {
        /// <summary>
        /// Polygons
        /// </summary>
        private GeographyPolygon[] polygons;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="polygons">Polygons</param>
        internal GeographyMultiPolygonImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyPolygon[] polygons)
            : base(coordinateSystem, creator)
        {
            this.polygons = polygons;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="polygons">Polygons</param>
        internal GeographyMultiPolygonImplementation(SpatialImplementation creator, params GeographyPolygon[] polygons)
            : this(CoordinateSystem.DefaultGeography, creator, polygons)
        {
        }

        /// <summary>
        /// Is MultiPolygon Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.polygons.Length == 0;
            }
        }

        /// <summary>
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.polygons); }
        }

        /// <summary>
        /// Polygons
        /// </summary>
        public override ReadOnlyCollection<GeographyPolygon> Polygons
        {
            get { return new ReadOnlyCollection<GeographyPolygon>(this.polygons); }
        }
        
        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.MultiPolygon);
            for (int i = 0; i < this.polygons.Length; ++i)
            {
                this.polygons[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
