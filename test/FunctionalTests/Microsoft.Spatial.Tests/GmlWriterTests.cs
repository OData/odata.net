//---------------------------------------------------------------------
// <copyright file="GmlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml;
using System.IO;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GmlWriterTests
    {
        [Fact]
        public void GeographyWritePoint2D()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeography();
            }, "gml:Point/gml:pos[text()='0.1 0.1']");
        }

        [Fact]
        public void GeographyWritePoint3D()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, 0.1, null));
                p.EndFigure();
                p.EndGeography();
            }, "gml:Point/gml:pos[text()='0.1 0.1 0.1']");
        }
        
        [Fact]
        public void GeographyWritePoint4D()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, 0.1, 0.1));
                p.EndFigure();
                p.EndGeography();
            }, "gml:Point/gml:pos[text()='0.1 0.1 0.1 0.1']");

            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, 0.1));
                p.EndFigure();
                p.EndGeography();
            }, "gml:Point/gml:pos[text()='0.1 0.1 NaN 0.1']");
        }

        [Fact]
        public void GeographyWritePointEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Point);
                p.EndGeography();
            }, "gml:Point/gml:pos[not(text())]");
        }

        [Fact]
        public void GeographyWriteMultiPoint()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiPoint);
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeography();

                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeography();
                p.EndGeography();
            }, 
            "gml:MultiPoint/gml:pointMembers/gml:Point[1]/gml:pos[text()='0.1 0.1']", 
            "gml:MultiPoint/gml:pointMembers/gml:Point[2]/gml:pos[text()='0.2 0.2']");
        }

        [Fact]
        public void GeographyWriteMultiPointEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiPoint);
                p.EndGeography();
            },
            "gml:MultiPoint[not(text())]");

            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiPoint);
                p.BeginGeography(SpatialType.Point);
                p.EndGeography();
                p.EndGeography();
            },
            "gml:MultiPoint/gml:pointMembers/gml:Point[not(text())]");
        }

        [Fact]
        public void GeographyWriteLineString()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.LineString);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeography();
            }, "gml:LineString/gml:pos[position() = 1 and text()='0.1 0.1']",
            "gml:LineString/gml:pos[position() = 2 and text()='0.2 0.2']");
        }
        
        [Fact]
        public void GeographyWriteLineStringEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.LineString);
                p.EndGeography();
            }, "gml:LineString/gml:posList[not(text())]");
        }

        [Fact]
        public void GeographyWriteMultiLineString()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiLineString);

                p.BeginGeography(SpatialType.LineString);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeography();

                p.BeginGeography(SpatialType.LineString);
                p.BeginFigure(new GeographyPosition(0.3, 0.3, null, null));
                p.LineTo(new GeographyPosition(0.4, 0.4, null, null));
                p.EndFigure();
                p.EndGeography();

                p.EndGeography();
            }, "gml:MultiCurve/gml:curveMembers/gml:LineString[1]/gml:pos[position() = 1 and text()='0.1 0.1']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[1]/gml:pos[position() = 2 and text()='0.2 0.2']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[2]/gml:pos[position() = 1 and text()='0.3 0.3']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[2]/gml:pos[position() = 2 and text()='0.4 0.4']");
        }

        [Fact]
        public void GeographyWriteMultiLineStringEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiLineString);
                p.EndGeography();
            }, "gml:MultiCurve[not(text())]");
        }

        [Fact]
        public void GeographyWritePolygon()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Polygon);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.LineTo(new GeographyPosition(0.3, 0.3, null, null));
                p.LineTo(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeographyPosition(1.1, 1.1, null, null));
                p.LineTo(new GeographyPosition(1.2, 1.2, null, null));
                p.LineTo(new GeographyPosition(1.3, 1.3, null, null));
                p.LineTo(new GeographyPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeographyPosition(2.1, 2.1, null, null));
                p.LineTo(new GeographyPosition(2.2, 2.2, null, null));
                p.LineTo(new GeographyPosition(2.3, 2.3, null, null));
                p.LineTo(new GeographyPosition(2.1, 2.1, null, null));
                p.EndFigure();
                p.EndGeography();
            }, 
            "count(gml:Polygon/gml:exterior) = 1",
            "count(gml:Polygon/gml:interior) = 2",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 1 and text() = '2.1 2.1']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 2 and text() = '2.2 2.2']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 3 and text() = '2.3 2.3']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 4 and text() = '2.1 2.1']");
        }
        
        [Fact]
        public void GeographyWritePolygonEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Polygon);
                p.EndGeography();
            },
            "gml:Polygon[not(text())]");
        }

        [Fact]
        public void GeographyWriteMultiPolygon()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiPolygon);
                p.BeginGeography(SpatialType.Polygon);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.LineTo(new GeographyPosition(0.3, 0.3, null, null));
                p.LineTo(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeographyPosition(1.1, 1.1, null, null));
                p.LineTo(new GeographyPosition(1.2, 1.2, null, null));
                p.LineTo(new GeographyPosition(1.3, 1.3, null, null));
                p.LineTo(new GeographyPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.EndGeography();
                p.BeginGeography(SpatialType.Polygon);
                p.BeginFigure(new GeographyPosition(2.1, 2.1, null, null));
                p.LineTo(new GeographyPosition(2.2, 2.2, null, null));
                p.LineTo(new GeographyPosition(2.3, 2.3, null, null));
                p.LineTo(new GeographyPosition(2.1, 2.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeographyPosition(3.1, 3.1, null, null));
                p.LineTo(new GeographyPosition(3.2, 3.2, null, null));
                p.LineTo(new GeographyPosition(3.3, 3.3, null, null));
                p.LineTo(new GeographyPosition(3.1, 3.1, null, null));
                p.EndFigure();
                p.EndGeography();
                p.EndGeography();
            },
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '2.1 2.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '2.2 2.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '2.3 2.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '2.1 2.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '3.1 3.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '3.2 3.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '3.3 3.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '3.1 3.1']");
        }
        
        [Fact]
        public void GeographyWriteMultiPolygonEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.MultiPolygon);
                p.EndGeography();
            }, "gml:MultiSurface[not(text())]");
        }

        [Fact]
        public void GeographyWriteCollection()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Collection);

                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeography();

                p.BeginGeography(SpatialType.Polygon);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.LineTo(new GeographyPosition(0.3, 0.3, null, null));
                p.LineTo(new GeographyPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeographyPosition(1.1, 1.1, null, null));
                p.LineTo(new GeographyPosition(1.2, 1.2, null, null));
                p.LineTo(new GeographyPosition(1.3, 1.3, null, null));
                p.LineTo(new GeographyPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.EndGeography();

                p.BeginGeography(SpatialType.Collection);
                p.BeginGeography(SpatialType.LineString);
                p.BeginFigure(new GeographyPosition(0.1, 0.1, null, null));
                p.LineTo(new GeographyPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeography();
                p.EndGeography();

                p.EndGeography();
            },
            "gml:MultiGeometry/gml:geometryMembers/gml:Point/gml:pos[text() = '0.1 0.1']",

            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry/gml:geometryMembers/gml:LineString/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry/gml:geometryMembers/gml:LineString/gml:pos[position() = 2 and text() = '0.2 0.2']"
            );
        }
        
        [Fact]
        public void GeographyWriteCollectionEmpty()
        {
            GeographyGmlWriterTest(p =>
            {
                p.BeginGeography(SpatialType.Collection);    // c1
                p.BeginGeography(SpatialType.Collection);    // c2
                p.EndGeography();                            // c2

                p.BeginGeography(SpatialType.Collection);    // c3
                p.BeginGeography(SpatialType.Point);
                p.BeginFigure(new GeographyPosition(10, 10, null, null));
                p.EndFigure();
                p.EndGeography();    // point
                p.EndGeography();    // collection 3
                p.EndGeography();    // collection 1
            },
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry[position() = 1 and not(text())]",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry[2]/gml:geometryMembers/gml:Point");
        }

        [Fact]
        public void GeographyWriteFullGlobe()
        {
            GeographyGmlWriterTest(p =>
            {
                p.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
                p.BeginGeography(SpatialType.FullGlobe);    
                p.EndGeography();  
            },
            "sqlgeo:FullGlobe[@gml:srsName = 'http://www.opengis.net/def/crs/EPSG/0/4326' and not(text())]");
        }

        private static void GeographyGmlWriterTest(Action<GeographyPipeline> pipelineCalls, params string[] expectedXPaths)
        {
            var ms = new MemoryStream();
            var w = XmlWriter.Create(ms, new XmlWriterSettings { Indent = false });
            DrawBoth gw = new GmlWriter(w);
            gw.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);

            pipelineCalls(gw.GeographyPipeline);
            w.Flush();
            w.Close();

            // use XElement to validate basic XML integrity
            ms.Seek(0, SeekOrigin.Begin);
            XmlNameTable nt = new NameTable();
            nt.Add(GmlConstants.GmlPrefix);

            // XPath or string contains
            var xnm = new XmlNamespaceManager(nt);
            xnm.AddNamespace(GmlConstants.GmlPrefix, GmlConstants.GmlNamespace);
            xnm.AddNamespace("sqlgeo", GmlConstants.FullGlobeNamespace);

            var xDoc = new XmlDocument(nt);
            xDoc.Load(ms);
            
            var nav = xDoc.CreateNavigator();
            SpatialTestUtils.VerifyXPaths(nav, xnm, "/node()[@gml:srsName = '" + GmlConstants.SrsPrefix + CoordinateSystem.DefaultGeography.EpsgId + "']");                                          
            SpatialTestUtils.VerifyXPaths(nav, xnm, expectedXPaths);
        }

        [Fact]
        public void GeometryWritePoint2D()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeometry();
            }, "gml:Point/gml:pos[text()='0.1 0.1']");
        }

        [Fact]
        public void GeometryWritePoint3D()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, 0.1, null));
                p.EndFigure();
                p.EndGeometry();
            }, "gml:Point/gml:pos[text()='0.1 0.1 0.1']");
        }

        [Fact]
        public void GeometryWritePoint4D()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, 0.1, 0.1));
                p.EndFigure();
                p.EndGeometry();
            }, "gml:Point/gml:pos[text()='0.1 0.1 0.1 0.1']");

            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, 0.1));
                p.EndFigure();
                p.EndGeometry();
            }, "gml:Point/gml:pos[text()='0.1 0.1 NaN 0.1']");
        }

        [Fact]
        public void GeometryWritePointEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Point);
                p.EndGeometry();
            }, "gml:Point[not(text())]");
        }

        [Fact]
        public void GeometryWriteMultiPoint()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiPoint);
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeometry();

                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeometry();
                p.EndGeometry();
            },
            "gml:MultiPoint/gml:pointMembers/gml:Point[1]/gml:pos[text()='0.1 0.1']",
            "gml:MultiPoint/gml:pointMembers/gml:Point[2]/gml:pos[text()='0.2 0.2']");
        }

        [Fact]
        public void GeometryWriteMultiPointEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiPoint);
                p.EndGeometry();
            },
            "gml:MultiPoint[not(text())]");

            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiPoint);
                p.BeginGeometry(SpatialType.Point);
                p.EndGeometry();
                p.EndGeometry();
            },
            "gml:MultiPoint/gml:pointMembers/gml:Point[not(text())]");
        }

        [Fact]
        public void GeometryWriteLineString()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.LineString);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeometry();
            }, "gml:LineString/gml:pos[position() = 1 and text()='0.1 0.1']",
            "gml:LineString/gml:pos[position() = 2 and text()='0.2 0.2']");
        }

        [Fact]
        public void GeometryWriteLineStringEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.LineString);
                p.EndGeometry();
            }, "gml:LineString[not(text())]");
        }

        [Fact]
        public void GeometryWriteMultiLineString()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiLineString);

                p.BeginGeometry(SpatialType.LineString);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeometry();

                p.BeginGeometry(SpatialType.LineString);
                p.BeginFigure(new GeometryPosition(0.3, 0.3, null, null));
                p.LineTo(new GeometryPosition(0.4, 0.4, null, null));
                p.EndFigure();
                p.EndGeometry();

                p.EndGeometry();
            }, "gml:MultiCurve/gml:curveMembers/gml:LineString[1]/gml:pos[position() = 1 and text()='0.1 0.1']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[1]/gml:pos[position() = 2 and text()='0.2 0.2']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[2]/gml:pos[position() = 1 and text()='0.3 0.3']",
            "gml:MultiCurve/gml:curveMembers/gml:LineString[2]/gml:pos[position() = 2 and text()='0.4 0.4']");
        }

        [Fact]
        public void GeometryWriteMultiLineStringEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiLineString);
                p.EndGeometry();
            }, "gml:MultiCurve[not(text())]");
        }

        [Fact]
        public void GeometryWritePolygon()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Polygon);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.LineTo(new GeometryPosition(0.3, 0.3, null, null));
                p.LineTo(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeometryPosition(1.1, 1.1, null, null));
                p.LineTo(new GeometryPosition(1.2, 1.2, null, null));
                p.LineTo(new GeometryPosition(1.3, 1.3, null, null));
                p.LineTo(new GeometryPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeometryPosition(2.1, 2.1, null, null));
                p.LineTo(new GeometryPosition(2.2, 2.2, null, null));
                p.LineTo(new GeometryPosition(2.3, 2.3, null, null));
                p.LineTo(new GeometryPosition(2.1, 2.1, null, null));
                p.EndFigure();
                p.EndGeometry();
            },
            "count(gml:Polygon/gml:exterior) = 1",
            "count(gml:Polygon/gml:interior) = 2",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:Polygon/gml:interior[1]/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 1 and text() = '2.1 2.1']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 2 and text() = '2.2 2.2']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 3 and text() = '2.3 2.3']",
            "gml:Polygon/gml:interior[2]/gml:LinearRing/gml:pos[position() = 4 and text() = '2.1 2.1']");
        }

        [Fact]
        public void GeometryWritePolygonEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Polygon);
                p.EndGeometry();
            },
            "gml:Polygon[not(text())]");
        }

        [Fact]
        public void GeometryWriteMultiPolygon()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiPolygon);
                p.BeginGeometry(SpatialType.Polygon);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.LineTo(new GeometryPosition(0.3, 0.3, null, null));
                p.LineTo(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeometryPosition(1.1, 1.1, null, null));
                p.LineTo(new GeometryPosition(1.2, 1.2, null, null));
                p.LineTo(new GeometryPosition(1.3, 1.3, null, null));
                p.LineTo(new GeometryPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.EndGeometry();
                p.BeginGeometry(SpatialType.Polygon);
                p.BeginFigure(new GeometryPosition(2.1, 2.1, null, null));
                p.LineTo(new GeometryPosition(2.2, 2.2, null, null));
                p.LineTo(new GeometryPosition(2.3, 2.3, null, null));
                p.LineTo(new GeometryPosition(2.1, 2.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeometryPosition(3.1, 3.1, null, null));
                p.LineTo(new GeometryPosition(3.2, 3.2, null, null));
                p.LineTo(new GeometryPosition(3.3, 3.3, null, null));
                p.LineTo(new GeometryPosition(3.1, 3.1, null, null));
                p.EndFigure();
                p.EndGeometry();
                p.EndGeometry();
            },
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[1]/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '2.1 2.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '2.2 2.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '2.3 2.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '2.1 2.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '3.1 3.1']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '3.2 3.2']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '3.3 3.3']",
            "gml:MultiSurface/gml:surfaceMembers/gml:Polygon[2]/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '3.1 3.1']"
            );
        }

        [Fact]
        public void GeometryWriteMultiPolygonEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.MultiPolygon);
                p.EndGeometry();
            }, "gml:MultiSurface[not(text())]");
        }

        [Fact]
        public void GeometryWriteCollection()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Collection);

                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.EndGeometry();

                p.BeginGeometry(SpatialType.Polygon);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.LineTo(new GeometryPosition(0.3, 0.3, null, null));
                p.LineTo(new GeometryPosition(0.1, 0.1, null, null));
                p.EndFigure();
                p.BeginFigure(new GeometryPosition(1.1, 1.1, null, null));
                p.LineTo(new GeometryPosition(1.2, 1.2, null, null));
                p.LineTo(new GeometryPosition(1.3, 1.3, null, null));
                p.LineTo(new GeometryPosition(1.1, 1.1, null, null));
                p.EndFigure();
                p.EndGeometry();

                p.BeginGeometry(SpatialType.Collection);
                p.BeginGeometry(SpatialType.LineString);
                p.BeginFigure(new GeometryPosition(0.1, 0.1, null, null));
                p.LineTo(new GeometryPosition(0.2, 0.2, null, null));
                p.EndFigure();
                p.EndGeometry();
                p.EndGeometry();

                p.EndGeometry();
            },
            "gml:MultiGeometry/gml:geometryMembers/gml:Point/gml:pos[text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 2 and text() = '0.2 0.2']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 3 and text() = '0.3 0.3']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:exterior/gml:LinearRing/gml:pos[position() = 4 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 1 and text() = '1.1 1.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 2 and text() = '1.2 1.2']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 3 and text() = '1.3 1.3']",
            "gml:MultiGeometry/gml:geometryMembers/gml:Polygon/gml:interior/gml:LinearRing/gml:pos[position() = 4 and text() = '1.1 1.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry/gml:geometryMembers/gml:LineString/gml:pos[position() = 1 and text() = '0.1 0.1']",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry/gml:geometryMembers/gml:LineString/gml:pos[position() = 2 and text() = '0.2 0.2']"
            );
        }

        [Fact]
        public void GeometryWriteCollectionEmpty()
        {
            GeometryGmlWriterTest(p =>
            {
                p.BeginGeometry(SpatialType.Collection);    // c1
                p.BeginGeometry(SpatialType.Collection);    // c2
                p.EndGeometry();                            // c2

                p.BeginGeometry(SpatialType.Collection);    // c3
                p.BeginGeometry(SpatialType.Point);
                p.BeginFigure(new GeometryPosition(10, 10, null, null));
                p.EndFigure();
                p.EndGeometry();    // point
                p.EndGeometry();    // collection 3
                p.EndGeometry();    // collection 1
            },
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry[position() = 1 and not(text())]",
            "gml:MultiGeometry/gml:geometryMembers/gml:MultiGeometry[2]/gml:geometryMembers/gml:Point");
        }

        private static void GeometryGmlWriterTest(Action<GeometryPipeline> pipelineCalls, params string[] expectedXPaths)
        {
            var ms = new MemoryStream();
            var w = XmlWriter.Create(ms, new XmlWriterSettings { Indent = false });
            DrawBoth gw = new GmlWriter(w);
            gw.GeometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);

            pipelineCalls(gw.GeometryPipeline);
            w.Flush();
            w.Close();

            // use XElement to validate basic XML integrity
            ms.Seek(0, SeekOrigin.Begin);
            XmlNameTable nt = new NameTable();
            nt.Add(GmlConstants.GmlPrefix);
            // XPath or string contains
            var xnm = new XmlNamespaceManager(nt);
            xnm.AddNamespace(GmlConstants.GmlPrefix, GmlConstants.GmlNamespace);

            var xDoc = new XmlDocument(nt);
            xDoc.Load(ms);

            var nav = xDoc.CreateNavigator();
            SpatialTestUtils.VerifyXPaths(nav, xnm, "/node()[@gml:srsName = '" + GmlConstants.SrsPrefix + CoordinateSystem.DefaultGeometry.EpsgId + "']");

            SpatialTestUtils.VerifyXPaths(nav, xnm, expectedXPaths);
        }
    }
}
