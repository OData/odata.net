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
