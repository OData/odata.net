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
    /// Geography polygon
    /// </summary>
    internal class GeographyPolygonImplementation : GeographyPolygon
    {
        /// <summary>
        /// Rings
        /// </summary>
        private GeographyLineString[] rings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeographyPolygonImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyLineString[] rings)
            : base(coordinateSystem, creator)
        {
            this.rings = rings ?? new GeographyLineString[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeographyPolygonImplementation(SpatialImplementation creator, params GeographyLineString[] rings)
            : this(CoordinateSystem.DefaultGeography, creator, rings)
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
        public override ReadOnlyCollection<GeographyLineString> Rings
        {
            get { return new ReadOnlyCollection<GeographyLineString>(this.rings); }
        }

        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.Polygon);

            for (int i = 0; i < this.rings.Length; ++i)
            {
                this.rings[i].SendFigure(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
