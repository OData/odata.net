//---------------------------------------------------------------------
// <copyright file="GeoFunctionHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.BuiltInFunctionHelper
{
    using Microsoft.Spatial;
    using System;

    public class GeoFunctionHelper
    {
        public static double GetDistance(GeographyPoint point1, GeographyPoint point2)
        {
            if (point1 == null)
            {
                throw new ArgumentNullException("point1");
            }
            if (point2 == null)
            {
                throw new ArgumentNullException("point2");
            }
            return point1.Distance(point2).Value;
        }

        public static double GetDistance(GeometryPoint point1, GeometryPoint point2)
        {
            if (point1 == null)
            {
                throw new ArgumentNullException("point1");
            }
            if (point2 == null)
            {
                throw new ArgumentNullException("point2");
            }
            return point1.Distance(point2).Value;
        }

        public static bool GetIsIntersects(GeographyPoint point, GeographyPolygon polygon)
        {
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }
            if (polygon == null)
            {
                throw new ArgumentNullException("polygon");
            }
            return point.Intersects(polygon).Value;
        }

        public static bool GetIsIntersects(GeometryPoint point, GeometryPolygon polygon)
        {
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }
            if (polygon == null)
            {
                throw new ArgumentNullException("polygon");
            }
            return point.Intersects(polygon).Value;
        }

        public static double GetLength(GeographyLineString line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            return line.Length().Value;
        }

        public static double GetLength(GeometryLineString line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            return line.Length().Value;
        }
    }
}