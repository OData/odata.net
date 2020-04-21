//---------------------------------------------------------------------
// <copyright file="GeographyHelperMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

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