//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

#if !INTERNAL_DROP && !EFRTM
namespace System.Data.Services
{
    #region Namespaces

    using System.Data.Services.Parsing;
    using System.Spatial;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Data.Spatial;

    #endregion

    /// <summary>
    /// Helper class for converting between Entity Framework spatial types and the equivalent classes in System.Data.Spatial.
    /// </summary>
    internal static class ObjectContextSpatialUtil
    {
        /// <summary>
        /// geo.distance signature for EF provider
        /// </summary>
        internal static FunctionDescription[] GeoDistanceSignatures = new FunctionDescription[] 
        {         
            new FunctionDescription("geo.distance", new Type[] { typeof(System.Data.Spatial.DbGeography), typeof(GeographyPoint) }, GeoDistanceEf),
            new FunctionDescription("geo.distance", new Type[] { typeof(System.Data.Spatial.DbGeography), typeof(System.Data.Spatial.DbGeography) }, GeoDistanceEf),
            new FunctionDescription("geo.distance", new Type[] { typeof(GeographyPoint), typeof(System.Data.Spatial.DbGeography) }, GeoDistanceEf)
        };

        /// <summary>
        /// Checks if the specified type is a DbGeography.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type is DbGeography, false otherwise.</returns>
        internal static bool IsDbGeography(Type type)
        {
            return type == typeof(DbGeography);
        }

        /// <summary>
        /// Converts the specified object to a Geography instance.
        /// </summary>
        /// <param name="instance">Object to convert.</param>
        /// <returns>New Geography instance.</returns>
        internal static Geography ConvertDbGeography(object instance)
        {
            DbGeography geography = (DbGeography)instance;
            switch (geography.GeometryType)
            {
                case "Point":
                    return GeographyFactory.Point((double)geography.Latitude, (double)geography.Longitude);
                case "LineString":
                    int numPoints = (int)geography.NumPoints;
                    var factory = GeographyFactory.LineString();

                    for (int n = 0; n < numPoints; n++)
                    {
                        DbGeography pointN = geography.PointN(n + 1); // PointN uses a 1-based index
                        Debug.Assert(pointN != null, "Expected PointN to return a non-null value for this position in the linestring");
                        Debug.Assert(pointN.GeometryType == "Point", "Expected PointN to return a Point for this position in the linestring");

                        factory.LineTo(pointN.Latitude.Value, pointN.Longitude.Value, pointN.Z, pointN.M);
                    }

                    return factory.Build();
                default:
                    Debug.Fail("Unsupported conversion from DbGeography of type: " + geography.GeometryType);
                    return null;
            }
        }

        /// <summary>
        /// Converts a DbGeography to a Geography.
        /// </summary>
        /// <param name="geography">Geography instance to convert.</param>
        /// <returns>New DbGeography instance representing the same value as <paramref name="geography"/>.</returns>
        internal static DbGeography ConvertGeography(Geography geography)
        {
            // DbGeography supports a different WKT format than Geography, so need to strip off the SRID first
            string geographyEWKT = geography.ToString();
            int semicolon = geographyEWKT.IndexOf(';');

            Debug.Assert(semicolon >= 0, "Expected to find a semicolon in the WKT format of Geography instance.");

            string geographyWKT = geographyEWKT.Substring(semicolon + 1);
            return DbGeography.FromText(geographyWKT, geography.CoordinateSystem.Id);
        }

        /// <summary>geo.distance for EF - call DbGeography.Distance</summary>
        /// <param name="target">Target of query; not used.</param>
        /// <param name="arguments">Arguments to function.</param>
        /// <returns>The conversion for this method.</returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Backward compatibility")]
        internal static Expression GeoDistanceEf(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 2, "arguments.Length == 2");

            // Convert the DbGeography type into Point
            Expression[] convertedArgs = new Expression[arguments.Length];

            for (int i = 0; i < convertedArgs.Length; ++i)
            {
                if (ObjectContextSpatialUtil.IsDbGeography(arguments[i].Type))
                {
                    convertedArgs[i] = arguments[i];
                }
                else
                {
                    Debug.Assert(arguments[i].Type == typeof(GeographyPoint), "This function should only be declared between DbGeography and Point");
                    Debug.Assert(arguments[i].NodeType == ExpressionType.Convert, "Must be a convert");
                    Debug.Assert(((UnaryExpression)arguments[i]).Operand.NodeType == ExpressionType.Constant, "Must be a literal comparsion - ef provider should never expose a Point property");

                    ConstantExpression e = (ConstantExpression)((UnaryExpression)arguments[i]).Operand;
                    convertedArgs[i] = Expression.Constant(ObjectContextSpatialUtil.ConvertGeography((Geography)e.Value));
                }
            }

            // public double Distance(DbGeography other)
            MethodInfo distanceMethod = typeof(System.Data.Spatial.DbGeography).GetMethod("Distance", BindingFlags.Public | BindingFlags.Instance);
            return Expression.Call(convertedArgs[0], distanceMethod, convertedArgs[1]);
        }
    }
}
#endif
