//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Spatial;

    /// <summary>
    /// Helper methods for Geometry types
    /// </summary>
    internal static class GeometryHelperMethods
    {
        /// <summary>
        /// Sends the current spatial object to the given pipeline with a figure that represents this LineString
        /// </summary>
        /// <param name="GeometryLineString">GeometryLineString instance for which the figure needs to be drawn.</param>
        /// <param name="pipeline">The pipeline to populate to</param>
        internal static void SendFigure(this GeometryLineString GeometryLineString, GeometryPipeline pipeline)
        {
            Util.CheckArgumentNull(GeometryLineString, "GeometryLineString");
            for (int i = 0; i < GeometryLineString.Points.Count; ++i)
            {
                GeometryPoint currentPoint = GeometryLineString.Points[i];
                var position = new GeometryPosition(currentPoint.X, currentPoint.Y, currentPoint.Z, currentPoint.M);
                if (i == 0)
                {
                    pipeline.BeginFigure(position);
                }
                else
                {
                    pipeline.LineTo(position);
                }
            }

            if (GeometryLineString.Points.Count > 0)
            {
                pipeline.EndFigure();
            }
        }
    }
}
