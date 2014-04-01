//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Spatial;

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
