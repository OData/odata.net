//---------------------------------------------------------------------
// <copyright file="MaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using Microsoft.Spatial;
using System.Xml.Linq;
using AstoriaUnitTests.TDD.Common;
using DataSpatialUnitTests.Utils;
using Microsoft.OData.Core;
using Microsoft.Spatial.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Util = Microsoft.OData.Client.Util;

namespace AstoriaUnitTests.Tests
{
    [TestClass]
    public class MaterializerUnitTests
    {
        public class SpatialEntity
        {
            public int ID { get; set; }
            public Geography BaseGeography { get; set; }
            public GeographyPoint GeoPoint { get; set; }
            public GeographyLineString GeoLine { get; set; }
            public double Data { get; set; }
        }

        [TestMethod]
        public void MaterializeGeographyInEntry()
        {
            XElement xel = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:gml=""http://www.opengis.net/gml"">
<title type=""text"">Entities</title>
<updated>2010-01-27T18:06:26Z</updated>
<link rel=""self"" title=""Entities"" href=""Entities"" />
<entry>
    <id>http://localhost/TheTest/Entities(16584)</id>
    <title type=""text""></title>
    <updated>2009-09-30T01:44:35Z</updated>
    <author>
        <name>Foo</name>
    </author>
    <category term=""#AstoriaUnitTests.Tests.MaterializerUnitTests.SpatialEntity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
        <m:properties>
            <d:ID m:type=""Edm.Int32"">16584</d:ID>
            <d:BaseGeography m:type=""Edm.Geography"">
                    <gml:Point srsName=""http://www.opengis.net/def/crs/EPSG/0/4326"">
                        <gml:pos>49.234 -86.555</gml:pos>
                    </gml:Point>
            </d:BaseGeography>
            <d:GeoPoint m:type=""Edm.GeographyPoint"">
                    <gml:Point srsName=""http://www.opengis.net/def/crs/EPSG/0/4326"">
                        <gml:pos>45.256 -71.92</gml:pos>
                    </gml:Point>
            </d:GeoPoint>
            <d:GeoLine m:type=""Edm.GeographyPoint"">
                    <gml:LineString srsName=""http://www.opengis.net/def/crs/EPSG/0/4326"">
                        <gml:pos>45.256 -71.92</gml:pos>
                        <gml:pos>45.111 -71.222</gml:pos>
                    </gml:LineString>
            </d:GeoLine>
            <d:Data m:type=""Edm.Int32"">123</d:Data>
        </m:properties>
    </content>
</entry>
</feed>");

            MaterializeAtom m = CreateMaterializer<SpatialEntity>(new Uri("http://localhost/TheTest/Entities"), xel.ToString(), TestConstants.MimeApplicationAtom, ODataPayloadKind.Feed);
            Assert.IsTrue(m.MoveNext());
            Assert.IsNotNull(m.Current);

            SpatialEntity entity = (SpatialEntity)m.Current;
            Assert.AreEqual(16584, entity.ID);
            Assert.AreEqual(123.0, entity.Data);

            entity.BaseGeography.VerifyAsPoint(new PositionData(49.234, -86.555));
            entity.GeoPoint.VerifyAsPoint(new PositionData(45.256, -71.92));
            entity.GeoLine.VerifyAsLineString(new PositionData(45.256, -71.92), new PositionData(45.111, -71.222));
        }

        [TestMethod]
        public void MaterializeGeographyTopLevel_BaseType()
        {
            TestMaterializeGeographyTopLevel(true);
        }

        [TestMethod]
        public void MaterializeGeographyTopLevel_DerivedType()
        {
            TestMaterializeGeographyTopLevel(false);
        }

        private void TestMaterializeGeographyTopLevel(bool useBaseGeography)
        {
            string typeName = useBaseGeography ? "Edm.Geography" : "Edm.GeographyPoint";

            XElement xel = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
            <m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:gml=""http://www.opengis.net/gml"" m:type=""" + typeName + @""">
                    <gml:Point srsName=""http://www.opengis.net/def/crs/EPSG/0/4326"">
                        <gml:pos>45.256 -71.92</gml:pos>
                    </gml:Point>
            </m:value>");

            MaterializeAtom m = CreateMaterializer<GeographyPoint>(new Uri("http://localhost/TheTest/Entities"), xel.ToString(), TestConstants.MimeApplicationXml, ODataPayloadKind.Property);
            Assert.IsTrue(m.MoveNext());
            Assert.IsNotNull(m.Current);

            GeographyPoint p = m.Current as GeographyPoint;
            Assert.IsNotNull(p);
            Assert.AreEqual(GeographyFactory.Point(45.256, -71.92).Build(), p);
        }

        [TestMethod]
        public void TestReadNullSpatialProperty()
        {
            foreach (bool useBaseGeography in new bool[] { true, false })
            {
                string typeName = useBaseGeography ? "Edm.Geography" : "Edm.GeographyPoint";

                XElement xel = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
            <m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:gml=""http://www.opengis.net/gml"" m:null=""true"" m:type=""" + typeName + @""">
            </m:value>");

                MaterializeAtom m = CreateMaterializer<GeographyPoint>(new Uri("http://localhost/TheTest/Entities"), xel.ToString(), TestConstants.MimeApplicationXml, ODataPayloadKind.Property);
                Assert.IsTrue(m.MoveNext());
                Assert.IsNull(m.Current);
            }
        }

        [TestMethod]
        public void TestReadNullSpatialProperty_HasContent()
        {
            foreach (bool useBaseGeography in new bool[] { true, false })
            {
                string typeName = useBaseGeography ? "Edm.Geography" : "Edm.GeographyPoint";

                XElement xel = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
            <m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:gml=""http://www.opengis.net/gml"" m:null=""true"" m:type=""" + typeName + @""">
                    <gml:Point srsName=""http://www.opengis.net/def/crs/EPSG/0/4326"">
                        <gml:pos>10 20 30 40</gml:pos>
                    </gml:Point>
            </m:value>");

                MaterializeAtom m = CreateMaterializer<GeographyPoint>(new Uri("http://localhost/TheTest/Entities"), xel.ToString(), TestConstants.MimeApplicationXml, ODataPayloadKind.Property);
                Assert.IsTrue(m.MoveNext());
                Assert.IsNull(m.Current);
            }
        }

        [TestMethod]
        public void Continuation_Creates_Incorrect_DataServiceVersion()
        {
            // Regression test for:Client always sends DSV=2.0 when following a continuation token
            ProjectionPlan plan = new ProjectionPlan()
            {
                LastSegmentType = typeof(int),
                ProjectedType = typeof(int)
            };

            var continuationToken = DataServiceQueryContinuation.Create(new Uri("http://localhost/Set?$skiptoken='Me'"), plan);
            QueryComponents queryComponents = continuationToken.CreateQueryComponents();
            Assert.AreSame(queryComponents.Version, Util.ODataVersionEmpty, "OData-Version of  query components should be empty for Continuation token");
        }

        private MaterializeAtom CreateMaterializer<T>(Uri requestUri, string content, string contentType, ODataPayloadKind payloadKind)
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), Microsoft.OData.Client.ODataProtocolVersion.V4);
            ctx.EnableAtom = true;
            ResponseInfo info = new ResponseInfo(new RequestInfo(ctx), MergeOption.AppendOnly);
            QueryComponents qc = new QueryComponents(requestUri, Util.ODataVersion4, typeof(T), null, null);
            var headers = new HeaderCollection();
            headers.SetHeader("Content-Type", contentType);
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(content);
            HttpWebResponseMessage responseMessage = new HttpWebResponseMessage(headers, (int)System.Net.HttpStatusCode.OK, () => new System.IO.MemoryStream(bytes));
            return new MaterializeAtom(info, qc, null, responseMessage, payloadKind);
        }
    }
}
