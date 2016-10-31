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
    using System.Diagnostics;
    using System.IO;
    using System.Spatial;
    using System.Text;

    /// <summary>
    /// Helper methods for the geography type.
    /// </summary>
    internal static class GeographyHelperMethods
    {
        /// <summary>
        /// Sends the current spatial object to the given pipeline with a figure that represents this LineString
        /// </summary>
        /// <param name="lineString">GeographyLineString instance to serialize.</param>
        /// <param name="pipeline">The pipeline to populate to</param>
        internal static void SendFigure(this GeographyLineString lineString, GeographyPipeline pipeline)
        {
            ReadOnlyCollection<GeographyPoint> points = lineString.Points;
            for (int i = 0; i < points.Count; ++i)
            {
                if (i == 0)
                {
                    pipeline.BeginFigure(new GeographyPosition(points[i].Latitude, points[i].Longitude, points[i].Z, points[i].M));
                }
                else
                {
                    pipeline.LineTo(new GeographyPosition(points[i].Latitude, points[i].Longitude, points[i].Z, points[i].M));
                }
            }

            if (points.Count > 0)
            {
                pipeline.EndFigure();
            }
        }
    }
}
