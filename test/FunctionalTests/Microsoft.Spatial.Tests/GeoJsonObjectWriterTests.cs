//---------------------------------------------------------------------
// <copyright file="GeoJsonObjectWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.Spatial;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    /// <summary>
    /// Tests functionalities of the GeoJsonObject writer
    /// This test class shares similar test cases with the GeoJsonWriter
    /// These test cases can be extracted out and used on both test classes
    /// </summary>
    public class GeoJsonObjectWriterTests
    {
        [Fact]
        public void WriteEmptyShapes()
        {
            foreach (SpatialType spatialType in Enum.GetValues(typeof(SpatialType)))
            {
                if (spatialType != SpatialType.Unknown && spatialType != SpatialType.FullGlobe)
                {
                    WriteGeoJsonTest(g =>
                    {
                        g.BeginGeo(spatialType);
                        g.EndGeo();
                    },
                    GetExpectedGeoJson(spatialType));
                }
            }
        }

        [Fact]
        public void WritePoint2D()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0));
        }

        [Fact]
        public void WritePoint3D()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, 30, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0, 30.0));
        }

        [Fact]
        public void WritePoint4D()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, 30, 40);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0, 30.0, 40.0));
        }

        [Fact]
        public void WritePoint4DNullZ()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, 40);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0, null, 40.0));
        }

        [Fact]
        public void WritePoint2DNullZM()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0));
        }

        [Fact]
        public void WritePoint3DNullM()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, 30, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0, 30.0));
        }

        [Fact]
        public void WriteLineStringSinglePoint()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.LineString, Array(10.0, 20.0)));
        }

        [Fact]
        public void WriteLineStringMultiplePoints()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(10, 20, -1.1, null);
                g.LineTo(20, -30, 4.99, null);
                g.LineTo(40.5, 25, null, null);
                g.LineTo(26, 22, 87, -156);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.LineString,
                Array(10.0, 20.0, -1.1),
                Array(20.0, -30.0, 4.99),
                Array(40.5, 25.0),
                Array(26.0, 22.0, 87.0, -156.0)));
        }
        
        [Fact]
        public void WritePolygonSingleFigure()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Polygon);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Polygon,
                Array(                      // ring 1
                    Array(10.0, 20.0),          // point 1
                    Array(20.0, 30.0),
                    Array(30.0, 40.0),
                    Array(10.0, 20.0))));
        }

        [Fact]
        public void WritePolygonMultipleFigures()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Polygon);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();

                g.BeginFigure(-10, -20, null, null);
                g.LineTo(-20, -30, null, null);
                g.LineTo(-30, -40, null, null);
                g.LineTo(-10, -20, null, null);
                g.EndFigure();

                g.BeginFigure(-10.5, -20.5, null, null);
                g.LineTo(-20.5, -30.5, null, null);
                g.LineTo(-30.5, -40.5, null, null);
                g.LineTo(-10.5, -20.5, null, null);
                g.EndFigure();
                g.EndGeo();
            }, GetExpectedGeoJson(SpatialType.Polygon,
                Array(
                    Array(10.0, 20.0),
                    Array(20.0, 30.0),
                    Array(30.0, 40.0),
                    Array(10.0, 20.0)),
                Array(
                    Array(-10.0, -20.0),
                    Array(-20.0, -30.0),
                    Array(-30.0, -40.0),
                    Array(-10.0, -20.0)),
                Array(
                    Array(-10.5, -20.5),
                    Array(-20.5, -30.5),
                    Array(-30.5, -40.5),
                    Array(-10.5, -20.5))));
        }

        [Fact]
        public void WriteMultiPointSingleChild()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiPoint);
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiPoint, Array(10.0, 20.0)));
        }

        [Fact]
        public void WriteMultiPointMultipleChildren()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiPoint);

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(-30, -40, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(80, 90, null, null);
                g.EndFigure();
                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiPoint,
                Array(10.0, 20.0),
                Array(-30.0, -40.0),
                Array(80.0, 90.0)));
        }

        [Fact]
        public void WriteMultiLineStringSingleChild()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiLineString);
                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.EndFigure();
                g.EndGeo();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiLineString,
                Array(
                    Array(10.0, 20.0),
                    Array(20.0, 30.0))));
        }

        [Fact]
        public void WriteMultiLineStringMultipleChildren()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiLineString);

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(30, 40, null, null);
                g.LineTo(40, 50, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(-40, 50, null, null);
                g.LineTo(60, -70, null, null);
                g.EndFigure();
                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiLineString, 
                Array(
                    Array(10.0, 20.0),
                    Array(20.0, 30.0)),
                Array(
                    Array(30.0, 40.0),
                    Array(40.0, 50.0)),
                Array(
                    Array(-40.0, 50.0),
                    Array(60.0, -70.0))));
        }

        [Fact]
        public void WriteMultiPolygonSingleChild()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiPolygon);

                g.BeginGeo(SpatialType.Polygon);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();

                g.BeginFigure(-10.5, -20.5, null, null);
                g.LineTo(-20.5, -30.5, null, null);
                g.LineTo(-30.5, -40.5, null, null);
                g.LineTo(-10.5, -20.5, null, null);
                g.EndFigure();
                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiPolygon, 
                Array(              // polygon 1
                    Array(          // ring 1
                        Array(10.0, 20.0),
                        Array(20.0, 30.0),
                        Array(30.0, 40.0),
                        Array(10.0, 20.0)),
                    Array(          // ring 2
                        Array(-10.5, -20.5),
                        Array(-20.5, -30.5),
                        Array(-30.5, -40.5),
                        Array(-10.5, -20.5)))));
        }

        [Fact]
        public void WriteMultiPolygonMultipleChildren()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.MultiPolygon);

                g.BeginGeo(SpatialType.Polygon);

                g.BeginFigure(60, 70, null, null);
                g.LineTo(80, 90, null, null);
                g.LineTo(60, 70, null, null);
                g.EndFigure();

                g.EndGeo();

                g.BeginGeo(SpatialType.Polygon);

                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();

                g.BeginFigure(-10.5, -20.5, null, null);
                g.LineTo(-20.5, -30.5, null, null);
                g.LineTo(-30.5, -40.5, null, null);
                g.LineTo(-10.5, -20.5, null, null);
                g.EndFigure();

                g.EndGeo();

                g.BeginGeo(SpatialType.Polygon);

                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();

                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.MultiPolygon, 
                Array(              // polygon 1
                    Array(          // ring 1
                        Array(60.0, 70.0),
                        Array(80.0, 90.0),
                        Array(60.0, 70.0))),
                Array(              // polygon 2
                    Array(          // ring 1
                        Array(10.0, 20.0),
                        Array(20.0, 30.0),
                        Array(30.0, 40.0),
                        Array(10.0, 20.0)),
                    Array(          // ring 2
                        Array(-10.5, -20.5),
                        Array(-20.5, -30.5),
                        Array(-30.5, -40.5),
                        Array(-10.5, -20.5))),
                Array(              // polygon 3
                    Array(          // ring 1
                        Array(10.0, 20.0),
                        Array(20.0, 30.0),
                        Array(30.0, 40.0),
                        Array(10.0, 20.0)))));
        }

        [Fact]
        public void WriteCollectionSingleChild()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Collection);
                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();
                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Collection,
                GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0)));
        }

        [Fact]
        public void WriteCollectionMultipleChildren()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Collection);

                g.BeginGeo(SpatialType.MultiLineString);

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(30, 40, null, null);
                g.LineTo(40, 50, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.LineString);
                g.BeginFigure(-40, 50, null, null);
                g.LineTo(60, -70, null, null);
                g.EndFigure();
                g.EndGeo();

                g.EndGeo();

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.MultiPoint);

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(20, 30, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(30, 40, null, null);
                g.EndFigure();
                g.EndGeo();

                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Collection,
                GetExpectedGeoJson(SpatialType.MultiLineString,
                    Array(
                        Array(10.0, 20.0),
                        Array(20.0, 30.0)),
                    Array(
                        Array(30.0, 40.0),
                        Array(40.0, 50.0)),
                    Array(
                        Array(-40.0, 50.0),
                        Array(60.0, -70.0))),
                GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0),
                GetExpectedGeoJson(SpatialType.MultiPoint,
                    Array(20.0, 30.0),
                    Array(30.0, 40.0))));
        }

        [Fact]
        public void WriteCollectionWithCollection()
        {
            WriteGeoJsonTest(g =>
            {
                g.BeginGeo(SpatialType.Collection);

                g.BeginGeo(SpatialType.Point);
                g.BeginFigure(10, 20, null, null);
                g.EndFigure();
                g.EndGeo();

                g.BeginGeo(SpatialType.Collection);

                g.BeginGeo(SpatialType.MultiPolygon);

                g.BeginGeo(SpatialType.Polygon);

                g.BeginFigure(10, 20, null, null);
                g.LineTo(20, 30, null, null);
                g.LineTo(30, 40, null, null);
                g.LineTo(10, 20, null, null);
                g.EndFigure();

                g.BeginFigure(-10.5, -20.5, null, null);
                g.LineTo(-20.5, -30.5, null, null);
                g.LineTo(-30.5, -40.5, null, null);
                g.LineTo(-10.5, -20.5, null, null);
                g.EndFigure();

                g.EndGeo();

                g.EndGeo();

                g.EndGeo();

                g.EndGeo();
            },
            GetExpectedGeoJson(SpatialType.Collection,
                GetExpectedGeoJson(SpatialType.Point, 10.0, 20.0),
                GetExpectedGeoJson(SpatialType.Collection,
                    GetExpectedGeoJson(SpatialType.MultiPolygon,
                        Array(
                            Array(
                                Array(10.0, 20.0),
                                Array(20.0, 30.0),
                                Array(30.0, 40.0),
                                Array(10.0, 20.0)),
                            Array(
                                Array(-10.5, -20.5),
                                Array(-20.5, -30.5),
                                Array(-30.5, -40.5),
                                Array(-10.5, -20.5)))))));
        }

        private static void WriteGeoJsonTest(Action<TypeWashedPipeline> pipelineCalls, IDictionary<String, Object> expectedJsonObject)
        {
            WriteGeoJsonTest(pipelineCalls, expectedJsonObject, true, CoordinateSystem.DefaultGeometry);
            WriteGeoJsonTest(pipelineCalls, expectedJsonObject, false, CoordinateSystem.DefaultGeography);

            WriteGeoJsonTest(pipelineCalls, expectedJsonObject, true, CoordinateSystem.Geometry(1234));
            WriteGeoJsonTest(pipelineCalls, expectedJsonObject, false, CoordinateSystem.Geography(4321));
        }

        private static void WriteGeoJsonTest(Action<TypeWashedPipeline> pipelineCalls, IDictionary<String, Object> expectedJsonObject, bool isGeometry, CoordinateSystem crs)
        {
            var w = new GeoJsonObjectWriter();
            TypeWashedPipeline p = isGeometry ?
                (TypeWashedPipeline)new TypeWashedToGeographyLongLatPipeline(new ForwardingSegment(w))
                : new TypeWashedToGeometryPipeline(new ForwardingSegment(w));

            p.SetCoordinateSystem(crs.EpsgId.Value);
            pipelineCalls(p);

            var actualObject = w.JsonObject;

            AddCoordinateSystem(expectedJsonObject, crs);
            AssertJsonObjectEquals(expectedJsonObject, actualObject);
        }

        private static void AssertJsonObjectEquals(IDictionary<String, Object> expected, IDictionary<String, Object> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach (var kvp in expected)
            {
                object actualValue = actual[kvp.Key];
                AssertJsonObjectEquals(kvp.Value, actualValue);
            }
        }

        private static void AssertJsonObjectEquals(IEnumerable expected, IEnumerable actual)
        {
            var left = expected.GetEnumerator();
            var right = actual.GetEnumerator();

            while (left.MoveNext())
            {
                Assert.True(right.MoveNext());
                AssertJsonObjectEquals(left.Current, right.Current);
            }

            Assert.False(right.MoveNext());
        }

        private static void AssertJsonObjectEquals(object expected, object actual)
        {
            if (expected == null)
            {
                Assert.Null(actual);
            }
            else if (expected is IDictionary<String, Object>)
            {
                AssertJsonObjectEquals((IDictionary<String, Object>)expected, actual as IDictionary<String, Object>);
            }
            else if (expected is String)
            {
                Assert.Equal(expected, actual);
            }
            else if (expected is IEnumerable)
            {
                AssertJsonObjectEquals((IEnumerable)expected, actual as IEnumerable);
            }
            else
            {
                Assert.Equal(expected, actual);
            }
        }

        private static object Array(params object[] members)
        {
            return members;
        }

        private static IDictionary<String, Object> GetExpectedGeoJson(SpatialType type, params object[] members)
        {
            var json = new Dictionary<String, Object>();
            AddType(json, type);
            json.Add(type == SpatialType.Collection ? "geometries" : "coordinates", members);
            return json;
        }

        private static void AddType(IDictionary<String, Object> jsonObject, SpatialType type)
        {
            jsonObject.Add("type", GetExpectedJsonTypeName(type));
        }

        private static void AddCoordinateSystem(IDictionary<String, Object> jsonObject, CoordinateSystem crs)
        {
            IDictionary<String, Object> crsObject = new Dictionary<String, Object>();
            IDictionary<String, Object> properties = new Dictionary<String, Object>();

            properties.Add("name", "EPSG:" + crs.Id);
            crsObject.Add("type", "name");
            crsObject.Add("properties", properties);

            // we may have already added a CRS
            jsonObject["crs"] = crsObject;
        }

        private static string GetExpectedJsonTypeName(SpatialType type)
        {
            switch (type)
            {
                case SpatialType.Point:
                    return "Point";
                case SpatialType.LineString:
                    return "LineString";
                case SpatialType.Polygon:
                    return "Polygon";
                case SpatialType.MultiPoint:
                    return "MultiPoint";
                case SpatialType.MultiLineString:
                    return "MultiLineString";
                case SpatialType.MultiPolygon:
                    return "MultiPolygon";
                case SpatialType.Collection:
                    return "GeometryCollection";
                default:
                    Assert.True(false, string.Format("Unrecognized GeoJson type '{0}'", type));
                    return null;
            }
        }
    }
}
