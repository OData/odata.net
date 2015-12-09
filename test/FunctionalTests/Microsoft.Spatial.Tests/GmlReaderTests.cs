//---------------------------------------------------------------------
// <copyright file="GmlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GmlReaderTests
    {
        [Fact]
        public void ReadCoordinateSystemGmlNamespace()
        {
            var reader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0 1.0</gml:pos></gml:Point>").CreateReader();

            var target = new SpatialToPositionPipeline();
            new GmlReader(target).ReadGeography(reader);
            Assert.Equal(CoordinateSystem.Geography(1234), target.CoordinateSystem);
        }

        [Fact]
        public void ReadCoordinateSystemDefaultNamespace()
        {
            var reader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml' srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0 1.0</gml:pos></gml:Point>").CreateReader();

            var target = new SpatialToPositionPipeline();
            new GmlReader(target).ReadGeography(reader);
            Assert.Equal(CoordinateSystem.Geography(1234), target.CoordinateSystem);
        }

        [Fact]
        public void ReadGeographyCoordinateSystem()
        {
            var reader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml'><gml:pos>1.0 1.0</gml:pos></gml:Point>").CreateReader();

            var target = new SpatialToPositionPipeline();
            new GmlReader(target).ReadGeography(reader);
            Assert.Equal(CoordinateSystem.DefaultGeography, target.CoordinateSystem);
        }

        [Fact]
        public void ReadGeometryCoordinateSystem()
        {
            var reader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml'><gml:pos>1.0 1.0</gml:pos></gml:Point>").CreateReader();

            var target = new SpatialToPositionPipeline();
            new GmlReader(target).ReadGeometry(reader);
            Assert.Equal(CoordinateSystem.DefaultGeometry, target.CoordinateSystem);
        }

        [Fact]
        public void ReadGmlEmptyLinearRing()
        {
            var reader = XElement.Parse(@"<gml:Polygon xmlns:gml='http://www.opengis.net/gml'>
      <gml:exterior>
         <gml:LinearRing></gml:LinearRing>
      </gml:exterior>
   </gml:Polygon>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_EmptyRingsNotAllowed);
        }

        [Fact]
        public void ReadUnexpectedElement()
        {
            var reader = XElement.Parse(@"<gml:LineString xmlns:gml='http://www.opengis.net/gml'>
      <gml:posList>
3 7 4 5 <MyWeirdElement/>
</gml:posList>
   </gml:LineString>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_UnexpectedElement("MyWeirdElement"));
        }

        [Fact]
        public void ReadInvalidShape()
        {
            var reader = XElement.Parse(@"<gml:Surface xmlns:gml='http://www.opengis.net/gml'>
      <gml:posList>
3 7 4 5
</gml:posList>
   </gml:Surface>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidSpatialType("Surface"));
        }

        [Fact]
        public void ReadXmlNotOnElement()
        {
            var reader = XmlReader.Create(new StringReader("<gml:Point xmlns:gml='http://www.opengis.net/gml'><gml:pos>1.0 1.0</gml:pos></gml:Point>"));
            reader.ReadStartElement();
            reader.ReadStartElement();
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_ExpectReaderAtElement);
        }

        [Fact]
        public void ReadGmlInvalidSrsName()
        {
            var reader = XElement.Parse(@"<gml:Polygon xmlns:gml='http://www.opengis.net/gml' srsName='test'></gml:Polygon>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidSrsName(GmlConstants.SrsPrefix));
        }

        [Fact]
        public void ReadGmlInvalidTopLevelAttribute()
        {
            // invalid attributes on top level elements
            var reader = XElement.Parse(@"<gml:Polygon xmlns:gml='http://www.opengis.net/gml' foo='bar'></gml:Polygon>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidAttribute("foo", "gml:Polygon"));
        }

        [Fact]
        public void ReadGmlInvalidAttribute()
        {
            // invalid attributes on inner elements
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml'><pos srsDimension='3' /></gml:Point>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidAttribute("srsDimension", "pos"));
        }

        [Fact]
        public void ReadGmlUnexpectedSrsName()
        {
            // changing the srsName inside a tag is invalid
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml'><pos srsName='foo' /></gml:Point>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidAttribute("srsName", "pos"));
        }

        [Fact]
        public void ReadGmlUnexpectedSrsName_Whitespace()
        {
            // Whitespace between tags should not affect how attributes are handled
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml'>
            <pos srsName='foo' /></gml:Point>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_InvalidAttribute("srsName", "pos"));
        }

        [Fact]
        public void ReadGmlPosNumbers_WhitespaceSeparated()
        {
            var payloads = new[] 
            {
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0\t1.0</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0\r1.0</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0\n1.0</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos> 1.0&#x9;1.0\t</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos> 1.0&#xD;1.0\t</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos> 1.0&#xA;1.0\t</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0  1.0</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>\t\t1.0 1.0</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0 1.0\n\n</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos> 1.0\t1.0\t</gml:pos></gml:Point>",
                "<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>1.0\t\n \r\n1.0</gml:pos></gml:Point>",
            };

            var target = new SpatialToPositionPipeline();
            foreach (var payload in payloads)
            {
                new GmlReader(target).ReadGeography(XmlReader.Create(new StringReader(payload)));
            }
        }

        [Fact]
        public void ReadGmlPosSingleNumber()
        {
            var reader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml'><gml:pos>10</gml:pos></gml:Point>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_PosNeedTwoNumbers);
        }

        [Fact]
        public void ReadGmlPosListSingleNumber()
        {
            var reader = XElement.Parse("<gml:LineString xmlns:gml='http://www.opengis.net/gml'><gml:posList>10</gml:posList></gml:LineString>").CreateReader();

            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(
                () => new GmlReader(new SpatialToPositionPipeline()).ReadGeography(reader),
                Strings.GmlReader_PosListNeedsEvenCount);
        }

        [Fact]
        public void ReaderIgnoreCoordinateSystemAttributes()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml' axisLabels='foo' uomLabels='bar'><gml:pos axisLabels='foo' uomLabels='bar'/></gml:Point>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 0);
        }

        [Fact]
        public void ReaderIgnoreIdAttribute()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:id='id1' id='id2'><gml:pos axisLabels='foo' uomLabels='bar'/></gml:Point>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 0);
        }

        [Fact]
        public void ReaderIgnoreCountAttributes()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:LineString xmlns:gml='http://www.opengis.net/gml'><gml:posList count='foo'/></gml:LineString>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 0);
        }

        [Fact]
        public void ReaderIgnoreNameAndDescriptionOnPoint()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:Point xmlns:gml='http://www.opengis.net/gml'><gml:name>Foo</gml:name>
<gml:description/><!--empty--><gml:name><complexName/></gml:name><!--empty-->
<gml:pos/><gml:name/><gml:descriptionReference/>
   <gml:identifier codeSpace=""http://www.example.com/"">string</gml:identifier></gml:Point>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 0);
        }

        [Fact]
        public void ReaderIgnoreNameAndDescriptionOnLineString()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:LineString xmlns:gml='http://www.opengis.net/gml'><gml:name>Foo</gml:name>
<gml:description/><!--empty--><gml:name><complexName/></gml:name><!--empty-->
<gml:posList/><gml:name/><gml:descriptionReference/>
   <gml:identifier codeSpace=""http://www.example.com/"">string</gml:identifier></gml:LineString>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 0);
        }

        [Fact]
        public void ReaderIgnoreNameAndDescriptionOnPolygon()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:Polygon gml:id=""ID"" xmlns:gml='http://www.opengis.net/gml'>
        <gml:metaDataProperty>
            <gml:GenericMetaData>Any text, intermingled with:
               <!--any element-->
            </gml:GenericMetaData>
        </gml:metaDataProperty>
        <gml:name>Foo</gml:name>
        <gml:description/>
        <!--empty-->
        <gml:exterior>
           <!--empty-->
           <gml:LinearRing>
              <gml:posList>1.0 1.0 2.0 2.0 3.0 3.0 1.0 1.0</gml:posList>
           </gml:LinearRing>
        </gml:exterior>
        <gml:name><complexName/></gml:name>
        <gml:interior>
           <gml:LinearRing>
              <gml:posList>1.0 1.0 2.0 2.0 3.0 3.0 1.0 1.0</gml:posList>
           </gml:LinearRing>
        </gml:interior>
        <gml:descriptionReference/>
        <gml:interior>
           <gml:LinearRing>
              <gml:posList>1.0 1.0 2.0 2.0 3.0 3.0 1.0 1.0</gml:posList>
           </gml:LinearRing>
        </gml:interior>
        <gml:identifier codeSpace=""http://www.example.com/"">string</gml:identifier>
    </gml:Polygon>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 12);
        }

        [Fact]
        public void ReaderIgnoreNameAndDescriptionOnMultiType()
        {
            // should ignore axisLabels and uomLabels attribute on any element
            var reader = XElement.Parse(@"<gml:MultiPoint gml:id=""ID"" xmlns:gml='http://www.opengis.net/gml'>
   <gml:metaDataProperty>
      <gml:GenericMetaData>Any text, intermingled with:
         <!--any element-->
      </gml:GenericMetaData>
   </gml:metaDataProperty>
   <gml:description>string</gml:description>
   <gml:pointMember>
      <gml:Point gml:id=""ID"">
         <gml:pos>1.0 1.0</gml:pos>
      </gml:Point>
   </gml:pointMember>
   <gml:identifier codeSpace=""http://www.example.com/"">string</gml:identifier>
   <gml:pointMember>
      <gml:Point gml:id=""ID"">
         <gml:pos>2.0 2.0</gml:pos>
      </gml:Point>
   </gml:pointMember>
   <gml:name>string</gml:name>
   <gml:pointMembers>
      <gml:Point gml:id=""ID"">
         <gml:metaDataProperty>...
         </gml:metaDataProperty>
         <gml:description>string</gml:description>
         <gml:descriptionReference/>
         <gml:identifier codeSpace=""http://www.example.com/"">string</gml:identifier>
         <gml:name>string</gml:name>
         <gml:pos>1.0 1.0</gml:pos>
      </gml:Point>
   </gml:pointMembers>
   <gml:descriptionReference/>
</gml:MultiPoint>").CreateReader();
            var pipeline = new SpatialToPositionPipeline();
            new GmlReader(pipeline).ReadGeography(reader);

            Assert.Equal(pipeline.Coordinates.Count, 3);
        }

        [Fact]
        public void ReadGeographyPoint2D()
        {
            ReadPointTest(new PositionData(0, 0), CoordinateSystem.Geography(0));
            ReadPointTest(new PositionData(0, 0), CoordinateSystem.DefaultGeography);

            ReadPointTest(null, CoordinateSystem.Geography(1234));
            ReadPointTest(new PositionData(0, 0), CoordinateSystem.Geography(1234));
            ReadPointTest(new PositionData(10, 10), CoordinateSystem.Geography(1234));
            ReadPointTest(new PositionData(-10, -10), CoordinateSystem.Geography(1234));
            ReadPointTest(new PositionData(double.MinValue, double.MaxValue), CoordinateSystem.Geography(1234));
        }

        [Fact]
        public void ReadGeographyPoint3D()
        {
            ReadPointTest(new PositionData(10, 10, 10, null), CoordinateSystem.DefaultGeography);
            ReadPointTest(new PositionData(10, 10, double.MaxValue, null), CoordinateSystem.DefaultGeography);
            ReadPointTest(new PositionData(10, 10, double.MinValue, null), CoordinateSystem.DefaultGeography);
        }

        [Fact]
        public void ReadGeographyPoint4D()
        {
            ReadPointTest(new PositionData(10, 10, 10, 10), CoordinateSystem.DefaultGeography);
            ReadPointTest(new PositionData(10, 10, 10, double.MaxValue), CoordinateSystem.DefaultGeography);
            ReadPointTest(new PositionData(10, 10, 10, double.MinValue), CoordinateSystem.DefaultGeography);
            ReadPointTest(new PositionData(10, 10, null, double.MaxValue), CoordinateSystem.DefaultGeography);
        }

        [Fact]
        public void ReadGeographyLineString()
        {
            ReadLineStringTest(CoordinateSystem.Geography(0),
                null);

            ReadLineStringTest(CoordinateSystem.Geography(0),
                new PositionData(10, 10), new PositionData(20, 20));

            ReadLineStringTest(CoordinateSystem.Geography(0),
                new PositionData(-10, -10), new PositionData(-20, -20));

            ReadLineStringTest(CoordinateSystem.Geography(0),
                new PositionData(double.MinValue, double.MaxValue), new PositionData(double.MinValue, double.MaxValue));
        }

        [Fact]
        public void ReadGeographyPolygon()
        {
            ReadPolygonTest(CoordinateSystem.Geography(0), null);

            // 1 ring
            ReadPolygonTest(CoordinateSystem.Geography(0),
                new[] { new PositionData(10, 10), new PositionData(20, 20) });

            // n-ring
            ReadPolygonTest(CoordinateSystem.Geography(0),
                new[] { new PositionData(10, 10), new PositionData(20, 20) },
                new[] { new PositionData(-10, -10), new PositionData(-20, -20) },
                new[] { new PositionData(double.MinValue, double.MaxValue), new PositionData(double.MinValue, double.MaxValue) });
        }

        [Fact]
        public void ReadGeographyMultiPoint()
        {
            ReadMultiPointTest(CoordinateSystem.Geography(0), null);

            ReadMultiPointTest(CoordinateSystem.Geography(0),
                new PositionData(10, 10), new PositionData(20, 20));

            ReadMultiPointTest(CoordinateSystem.Geography(0),
                new PositionData(-10, -10), new PositionData(-20, -20));

            ReadMultiPointTest(CoordinateSystem.Geography(0),
                new PositionData(double.MinValue, double.MaxValue), new PositionData(double.MinValue, double.MaxValue));
        }

        [Fact]
        public void ReadGeographyMultiLineString()
        {
            ReadMultiLineStringTest(CoordinateSystem.Geography(0),
                new[] { new PositionData(10, 10), new PositionData(20, 20) });

            ReadMultiLineStringTest(CoordinateSystem.Geography(0),
                new[] { new PositionData(10, 10), new PositionData(20, 20) },
                new[] { new PositionData(-10, -10), new PositionData(-20, -20) },
                new[] { new PositionData(double.MinValue, double.MaxValue), new PositionData(double.MinValue, double.MaxValue) });
        }

        [Fact]
        public void ReadGeographyMultiPolygon()
        {
            ReadMultiPolygonTest(CoordinateSystem.Geography(0),
                new[]
                    {
                    new[] { new PositionData(10, 10), new PositionData(20, 20) }},
                new[]
                    {                            
                    new[] { new PositionData(10, 10), new PositionData(20, 20) },
                    new[] { new PositionData(-10, -10), new PositionData(-20, -20) },
                    new[] { new PositionData(double.MinValue, double.MaxValue), new PositionData(double.MinValue, double.MaxValue) }});
        }

        [Fact]
        public void ReadGeographyCollection()
        {
            ReadCollectionTest(true);
            ReadCollectionTest(false);
        }

        [Fact]
        public void ReadEmptyGeography_Point()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.Point, CoordinateSystem.DefaultGeography));
            PointPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_LineString()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.LineString, CoordinateSystem.DefaultGeography));
            LineStringPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_Polygon()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.Polygon, CoordinateSystem.DefaultGeography));
            PolygonPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_MultiPoint()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.MultiPoint, CoordinateSystem.DefaultGeography));
            MultiPointPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_MultiLineString()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.MultiLineString, CoordinateSystem.DefaultGeography));
            MultiLineStringPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_MultiPolygon()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.MultiPolygon, CoordinateSystem.DefaultGeography));
            MultiPolygonPipelineBaseline(CoordinateSystem.DefaultGeography, null).VerifyPipeline(target);
        }

        [Fact]
        public void ReadEmptyGeography_Collection()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedEmptyGml(SpatialType.Collection, CoordinateSystem.DefaultGeography));

            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            expected.BeginShape(SpatialType.Collection);
            expected.EndShape();
            expected.VerifyPipeline(target);
        }

        [Fact]
        public void ReadFullGlobe()
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedFullGlobeGml(CoordinateSystem.DefaultGeography));

            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            expected.BeginShape(SpatialType.FullGlobe);
            expected.EndShape();
            expected.VerifyPipeline(target);
        }

        [Fact]
        public void ResetReader()
        {
            var invalidReader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/5555\"><gml:pos>1</gml:pos></gml:Point>").CreateReader();
            var validReader = XElement.Parse("<gml:Point xmlns:gml='http://www.opengis.net/gml' gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/1234\"><gml:pos>2 2</gml:pos></gml:Point>").CreateReader();

            var target = new SpatialToPositionPipeline();
            var gmlReader = new GmlReader(target);
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => gmlReader.ReadGeography(invalidReader), Strings.GmlReader_PosNeedTwoNumbers);
            gmlReader.Reset();
            gmlReader.ReadGeography(validReader);
            Assert.Equal(CoordinateSystem.Geography(1234), target.CoordinateSystem);
        }

        private static void ReadPointTest(PositionData data, CoordinateSystem coordinateSystem)
        {
            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedPointGml(data, coordinateSystem));
            PointPipelineBaseline(coordinateSystem, data).VerifyPipeline(target);
        }

        private static void ReadLineStringTest(CoordinateSystem coordinateSystem, params PositionData[] data)
        {
            var baseline = LineStringPipelineBaseline(coordinateSystem, data);

            var target = new CallSequenceLoggingPipeline();

            // <LineString><posList>x1 y1 x2 y2 ...</posList></LineString>
            new GmlReader(target).ReadGeography(ExpectedLineStringGml(data, coordinateSystem, BuildPosList));
            baseline.VerifyPipeline(target);

            if (data != null)
            {
                target = new CallSequenceLoggingPipeline();
                // <LineString><pos>x1 y1</pos><pos>x2 y2 ...</pos></LineString>
                new GmlReader(target).ReadGeography(ExpectedLineStringGml(data, coordinateSystem, BuildPosArray));
                baseline.VerifyPipeline(target);

                target = new CallSequenceLoggingPipeline();
                // <LineString><pointProperty><pos>x1 y1</pos></pointProperty><pointProperty><pos>x2 y2 ...</pos></pointProperty></LineString>
                new GmlReader(target).ReadGeography(ExpectedLineStringGml(data, coordinateSystem, BuildPointMembers));
                baseline.VerifyPipeline(target);
            }
        }

        private static void ReadPolygonTest(CoordinateSystem coordinateSystem, params PositionData[][] data)
        {
            var baseline = PolygonPipelineBaseline(coordinateSystem, data);

            var target = new CallSequenceLoggingPipeline();

            // <LineString><posList>x1 y1 x2 y2 ...</posList></LineString>
            new GmlReader(target).ReadGeography(ExpectedPolygonGml(data, coordinateSystem, BuildPosList));
            baseline.VerifyPipeline(target);

            target = new CallSequenceLoggingPipeline();
            // <LineString><pos>x1 y1</pos><pos>x2 y2 ...</pos></LineString>
            new GmlReader(target).ReadGeography(ExpectedPolygonGml(data, coordinateSystem, BuildPosArray));
            baseline.VerifyPipeline(target);

            target = new CallSequenceLoggingPipeline();
            // <LineString><pointProperty><pos>x1 y1</pos></pointProperty><pointProperty><pos>x2 y2 ...</pos></pointProperty></LineString>
            new GmlReader(target).ReadGeography(ExpectedPolygonGml(data, coordinateSystem, BuildPointMembers));
            baseline.VerifyPipeline(target);
        }

        private static void ReadMultiPointTest(CoordinateSystem coordinateSystem, params PositionData[] data)
        {
            var baseline = MultiPointPipelineBaseline(coordinateSystem, data);

            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiPointGml(data, coordinateSystem, true));
            baseline.VerifyPipeline(target);

            target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiPointGml(data, coordinateSystem, false));
            baseline.VerifyPipeline(target);
        }

        private static void ReadMultiLineStringTest(CoordinateSystem coordinateSystem, params PositionData[][] data)
        {
            var baseline = MultiLineStringPipelineBaseline(coordinateSystem, data);

            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiLineStringGml(data, coordinateSystem, true));
            baseline.VerifyPipeline(target);

            target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiLineStringGml(data, coordinateSystem, false));
            baseline.VerifyPipeline(target);
        }

        private static void ReadMultiPolygonTest(CoordinateSystem coordinateSystem, params PositionData[][][] data)
        {
            var baseline = MultiPolygonPipelineBaseline(coordinateSystem, data);

            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiPolygonGml(data, coordinateSystem, true));
            baseline.VerifyPipeline(target);

            target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(ExpectedMultiPolygonGml(data, coordinateSystem, false));
            baseline.VerifyPipeline(target);
        }

        private static void ReadCollectionTest(bool buildAsList)
        {
            var coordinateSystem = CoordinateSystem.Geography(0);
            PositionData[] data = { new PositionData(10, 10), new PositionData(20, 20) };

            var payloadBuilder = new StringBuilder();
            payloadBuilder.Append(GmlShapeElement("MultiGeometry", coordinateSystem.EpsgId));
            BuildMultiGml(buildAsList ? "geometryMembers" : "geometryMember", buildAsList, payloadBuilder, data,
                (o, b) => BuildPointGml(o, b, null));
            payloadBuilder.Append(GmlEndElement("MultiGeometry"));

            var xel = XElement.Parse(payloadBuilder.ToString());
            var reader = xel.CreateReader();

            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.Collection);
            expected.BeginShape(SpatialType.Point);
            expected.BeginFigure(10, 10, null, null);
            expected.EndFigure();
            expected.EndShape();
            expected.BeginShape(SpatialType.Point);
            expected.BeginFigure(20, 20, null, null);
            expected.EndFigure();
            expected.EndShape();
            expected.EndShape();

            var target = new CallSequenceLoggingPipeline();
            new GmlReader(target).ReadGeography(reader);
            expected.VerifyPipeline(target);
        }

        #region Pipeline Baselines

        private static ICommonLoggingPipeline PointPipelineBaseline(CoordinateSystem coordinateSystem, PositionData point)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.Point);
            if (point != null)
            {
                expected.BeginFigure(point.Latitude, point.Longitude, point.Z, point.M);
                expected.EndFigure();
            }

            expected.EndShape();

            return expected;
        }

        private static ICommonLoggingPipeline LineStringPipelineBaseline(CoordinateSystem coordinateSystem, PositionData[] points)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.LineString);

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    AddPointToPipeline(expected, points[i], i == 0);
                }

                if (points.Length > 0)
                {
                    expected.EndFigure();
                }
            }

            expected.EndShape();

            return expected;
        }

        private static ICommonLoggingPipeline PolygonPipelineBaseline(CoordinateSystem coordinateSystem, PositionData[][] points)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.Polygon);

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    for (int j = 0; j < points[i].Length; ++j)
                    {
                        AddPointToPipeline(expected, points[i][j], j == 0);
                    }

                    if (points[i].Length > 0)
                    {
                        expected.EndFigure();
                    }
                }
            }

            expected.EndShape();
            return expected;
        }

        private static ICommonLoggingPipeline MultiPointPipelineBaseline(CoordinateSystem coordinateSystem, PositionData[] points)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.MultiPoint);

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    expected.BeginShape(SpatialType.Point);
                    AddPointToPipeline(expected, points[i], true);
                    expected.EndFigure();
                    expected.EndShape();
                }
            }

            expected.EndShape();
            return expected;
        }

        private static ICommonLoggingPipeline MultiLineStringPipelineBaseline(CoordinateSystem coordinateSystem, PositionData[][] points)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.MultiLineString);

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    expected.BeginShape(SpatialType.LineString);
                    for (int j = 0; j < points[i].Length; ++j)
                    {
                        AddPointToPipeline(expected, points[i][j], j == 0);
                    }

                    if (points[i].Length > 0)
                    {
                        expected.EndFigure();
                    }

                    expected.EndShape();
                }
            }

            expected.EndShape();
            return expected;
        }

        private static ICommonLoggingPipeline MultiPolygonPipelineBaseline(CoordinateSystem coordinateSystem, PositionData[][][] points)
        {
            ICommonLoggingPipeline expected = new GeographyLoggingPipeline(false);
            expected.SetCoordinateSystem(coordinateSystem);
            expected.BeginShape(SpatialType.MultiPolygon);

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    expected.BeginShape(SpatialType.Polygon);
                    for (int j = 0; j < points[i].Length; ++j)
                    {
                        for (int k = 0; k < points[i][j].Length; ++k)
                        {
                            AddPointToPipeline(expected, points[i][j][k], k == 0);
                        }

                        if (points[i][j].Length > 0)
                        {
                            expected.EndFigure();
                        }
                    }
                    expected.EndShape();
                }
            }

            expected.EndShape();
            return expected;
        }

        private static void AddPointToPipeline(ICommonLoggingPipeline pipeline, PositionData point, bool firstPoint)
        {
            if (firstPoint)
            {
                pipeline.BeginFigure(point.Latitude, point.Longitude, point.Z, point.M);
            }
            else
            {
                pipeline.AddLineTo(point.Latitude, point.Longitude, point.Z, point.M);
            }
        }

        #endregion

        #region Gml Generation

        private static XmlReader ExpectedEmptyGml(SpatialType type, CoordinateSystem coordinateSystem)
        {
            StringBuilder payloadBuilder = new StringBuilder();

            switch (type)
            {
                case SpatialType.Point:
                    payloadBuilder.Append(GmlShapeElement("Point", coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlElement("pos"));
                    payloadBuilder.Append(GmlEndElement("pos"));
                    payloadBuilder.Append(GmlEndElement("Point"));
                    break;
                case SpatialType.LineString:
                    payloadBuilder.Append(GmlShapeElement("LineString", coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlElement("posList"));
                    payloadBuilder.Append(GmlEndElement("posList"));
                    payloadBuilder.Append(GmlEndElement("LineString"));
                    break;
                case SpatialType.Polygon:
                case SpatialType.MultiPoint:
                    payloadBuilder.Append(GmlShapeElement(type.ToString(), coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlEndElement(type.ToString()));
                    break;
                case SpatialType.MultiLineString:
                    payloadBuilder.Append(GmlShapeElement("MultiCurve", coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlEndElement("MultiCurve"));
                    break;
                case SpatialType.MultiPolygon:
                    payloadBuilder.Append(GmlShapeElement("MultiSurface", coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlEndElement("MultiSurface"));
                    break;
                case SpatialType.Collection:
                    payloadBuilder.Append(GmlShapeElement("MultiGeometry", coordinateSystem.EpsgId));
                    payloadBuilder.Append(GmlEndElement("MultiGeometry"));
                    break;
                default:
                    Assert.True(false, "unknown spatial type");
                    break;
            }

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedPointGml(PositionData point, CoordinateSystem coordinateSystem)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            BuildPointGml(point, payloadBuilder, coordinateSystem);

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedLineStringGml(PositionData[] points, CoordinateSystem coordinateSystem, Action<PositionData[], StringBuilder> lineBuilder)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.Append(GmlShapeElement("LineString", coordinateSystem.EpsgId));
            lineBuilder(points, payloadBuilder);
            payloadBuilder.Append(GmlEndElement("LineString"));

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedPolygonGml(PositionData[][] points, CoordinateSystem coordinateSystem, Action<PositionData[], StringBuilder> lineBuilder)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            BuildPolygonGml(points, coordinateSystem, lineBuilder, payloadBuilder);
            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedFullGlobeGml(CoordinateSystem coordinateSystem)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.AppendFormat(@"<FullGlobe gml:srsName=""http://www.opengis.net/def/crs/EPSG/0/{0}"" xmlns:gml=""http://www.opengis.net/gml"" xmlns=""http://schemas.microsoft.com/sqlserver/2011/geography""/>", coordinateSystem.EpsgId);
            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static void BuildMultiGml<T>(string memberText, bool buildAsList, StringBuilder builder, T[] data, Action<T, StringBuilder> buildInnerGml)
        {
            if (buildAsList)
            {
                builder.Append(GmlElement(memberText));
            }

            if (data != null)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    if (!buildAsList)
                    {
                        builder.Append(GmlElement(memberText));
                    }

                    buildInnerGml(data[i], builder);

                    if (!buildAsList)
                    {
                        builder.Append(GmlEndElement(memberText));
                    }
                }
            }

            if (buildAsList)
            {
                builder.Append(GmlEndElement(memberText));
            }
        }

        private static XmlReader ExpectedMultiPointGml(PositionData[] points, CoordinateSystem coordinateSystem, bool buildAsList)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.Append(GmlShapeElement("MultiPoint", coordinateSystem.EpsgId));
            BuildMultiGml(buildAsList ? "pointMembers" : "pointMember", buildAsList, payloadBuilder, points,
                (o, b) => BuildPointGml(o, b, null));
            payloadBuilder.Append(GmlEndElement("MultiPoint"));

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedMultiLineStringGml(PositionData[][] points, CoordinateSystem coordinateSystem, bool buildAsList)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.Append(GmlShapeElement("MultiCurve", coordinateSystem.EpsgId));

            BuildMultiGml(buildAsList ? "curveMembers" : "curveMember", buildAsList, payloadBuilder, points,
                (o, b) =>
                {
                    payloadBuilder.Append(GmlElement("LineString"));
                    BuildPosList(o, payloadBuilder);
                    payloadBuilder.Append(GmlEndElement("LineString"));
                });

            payloadBuilder.Append(GmlEndElement("MultiCurve"));

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static XmlReader ExpectedMultiPolygonGml(PositionData[][][] points, CoordinateSystem coordinateSystem, bool buildAsList)
        {
            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.Append(GmlShapeElement("MultiSurface", coordinateSystem.EpsgId));
            BuildMultiGml(buildAsList ? "surfaceMembers" : "surfaceMember", buildAsList, payloadBuilder, points,
                (o, b) => BuildPolygonGml(o, null, BuildPosList, payloadBuilder));
            payloadBuilder.Append(GmlEndElement("MultiSurface"));

            XElement xel = XElement.Parse(payloadBuilder.ToString());
            return xel.CreateReader();
        }

        private static void BuildPointGml(PositionData point, StringBuilder builder, CoordinateSystem coordinateSystem = null)
        {
            if (coordinateSystem != null)
            {
                builder.Append(GmlShapeElement("Point", coordinateSystem.EpsgId));
            }
            else
            {
                builder.Append(GmlElement("Point"));
            }

            builder.Append(GmlElement("pos"));
            if (point != null)
            {
                builder.Append(GmlPostionText(point.Latitude, point.Longitude, point.Z, point.M));
            }
            builder.Append(GmlEndElement("pos"));
            builder.Append(GmlEndElement("Point"));
        }

        private static void BuildPointMembers(PositionData[] points, StringBuilder builder)
        {
            for (int i = 0; i < points.Length; ++i)
            {
                builder.Append(GmlElement("pointProperty"));
                BuildPointGml(points[i], builder, null);
                builder.Append(GmlEndElement("pointProperty"));
            }
        }

        private static void BuildPosArray(PositionData[] points, StringBuilder builder)
        {
            for (int i = 0; i < points.Length; ++i)
            {
                builder.Append(GmlElement("pos"));
                builder.Append(GmlPostionText(points[i].Latitude, points[i].Longitude, points[i].Z, points[i].M));
                builder.Append(GmlEndElement("pos"));
            }
        }

        private static void BuildPosList(PositionData[] points, StringBuilder builder)
        {
            builder.Append(GmlElement("posList"));

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    builder.Append(GmlPostionText(points[i].Latitude, points[i].Longitude, null, null));
                    builder.Append(' ');
                }
            }

            builder.Append(GmlEndElement("posList"));
        }

        private static void BuildPolygonGml(PositionData[][] points, CoordinateSystem coordinateSystem, Action<PositionData[], StringBuilder> lineBuilder, StringBuilder builder)
        {
            builder.Append(coordinateSystem == null ? GmlElement("Polygon") : GmlShapeElement("Polygon", coordinateSystem.EpsgId));

            if (points != null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    builder.Append(GmlElement(i == 0 ? "exterior" : "interior"));
                    builder.Append(GmlElement("LinearRing"));
                    lineBuilder(points[i], builder);
                    builder.Append(GmlEndElement("LinearRing"));
                    builder.Append(GmlEndElement(i == 0 ? "exterior" : "interior"));
                }
            }

            builder.Append(GmlEndElement("Polygon"));
        }

        private static String GmlShapeElement(string shapeType, int? epsgId)
        {
            return String.Format(@"<gml:{0} xmlns:gml=""http://www.opengis.net/gml"" gml:srsName=""http://www.opengis.net/def/crs/EPSG/0/{1}"">", shapeType, epsgId.Value);
        }

        private static String GmlElement(string elementName)
        {
            return String.Format(@"<gml:{0}>", elementName);
        }

        private static String GmlEndElement(string shapeType)
        {
            return String.Format("</gml:{0}>", shapeType);
        }

        private static String GmlPostionText(double x, double y, double? z, double? m)
        {
            var result = new StringBuilder();
            result.Append(XmlConvert.ToString(x));
            result.Append(" ");
            result.Append(XmlConvert.ToString(y));

            if (z.HasValue)
            {
                result.Append(" ");
                result.Append(XmlConvert.ToString(z.Value));

                if (m.HasValue)
                {
                    result.Append(" ");
                    result.Append(XmlConvert.ToString(m.Value));
                }
            }
            else if (m.HasValue)
            {
                result.Append(" ");
                result.Append(XmlConvert.ToString(double.NaN));
                result.Append(" ");
                result.Append(XmlConvert.ToString(m.Value));
            }

            return result.ToString();
        }

        #endregion
    }
}
