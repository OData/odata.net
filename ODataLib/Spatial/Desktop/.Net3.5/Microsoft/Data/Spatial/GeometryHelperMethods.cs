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
    using System.Diagnostics;
    using System.IO;
    using System.Spatial;

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
