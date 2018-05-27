//---------------------------------------------------------------------
// <copyright file="GeometryHelperMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
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