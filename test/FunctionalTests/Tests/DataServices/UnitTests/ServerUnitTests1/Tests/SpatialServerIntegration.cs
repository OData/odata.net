//---------------------------------------------------------------------
// <copyright file="SpatialServerIntegration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Test.Astoria;
using System.IO;
using System.Linq;
using Microsoft.Spatial;
using System.Text;
using System.Xml;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace AstoriaUnitTests.Tests
{
    [TestClass]
    public class SpatialServerIntegration
    {
        private static WellKnownTextSqlFormatter wktFormatter;
        private static GeoJsonObjectFormatter jsonFormatter;
        private static GmlFormatter gmlFormatter;

        private static Dictionary<SpatialType, String[]> testData = new Dictionary<SpatialType, String[]>()
        {
            { SpatialType.Point, new String[] { 
                "POINT EMPTY",
                "POINT(10 20)", 
                "SRID=1234;POINT(10 20)",
                "POINT(10 20 30)",                
                "POINT(10 20 NULL 40)",
                "POINT(10 20 30 40)" }},
            { SpatialType.LineString, new String[] {
                "LINESTRING EMPTY",
                "LINESTRING(10 20 30, 20 30, 30 40 50 60)",
                "LINESTRING(10 20 NULL 30, 20 30 NULL 40, 30 40, 40 50 NULL 60)",
                "LINESTRING(10 20, 20 30)",
                "SRID=1234;LINESTRING(10 20, 30 40)" }},
            { SpatialType.Polygon, new String[] {
                "POLYGON EMPTY",
                "POLYGON((10 20, 20 30 NULL 40, 30 40 NULL 50, 10 20))",
                "POLYGON((10 20, 20 30, 30 40, 10 20))",
                "SRID=1234;POLYGON((10 20, 20 30, 30 40, 10 20))" }},
            { SpatialType.MultiPoint, new String[] {
                "MULTIPOINT EMPTY",
                "MULTIPOINT((10 20), (20 30))",
                "SRID=1234;MULTIPOINT((20 30), (30 40), (40 50))" }},
            { SpatialType.MultiLineString, new String[] {
                "MULTILINESTRING EMPTY",
                "MULTILINESTRING((10 20, 20 30), (20 30, 30 40))",
                "SRID=1234;MULTILINESTRING((20 30, 30 40), (30 40, 40 50))" }},
            { SpatialType.MultiPolygon, new String[] {
                "MULTIPOLYGON EMPTY", 
                "MULTIPOLYGON(((10 20, 20 30, 30 40, 10 20)), ((15 25, 25 35, 35 45, 15 25)))",
                "SRID=1234;MULTIPOLYGON (((10 10, 10 20, 20 20, 20 15 , 10 10), (50 40, 50 50, 60 50, 60 40, 50 40)))" }},
            { SpatialType.Collection, new String[] {
                "GEOMETRYCOLLECTION EMPTY",
                "GEOMETRYCOLLECTION(POINT(10 20), LINESTRING(10 20, 20 30), MULTIPOINT((10 20), (20 30)))",
                "SRID=1234; GEOMETRYCOLLECTION(POLYGON((10 20, 20 30, 30 40, 10 20)), MULTILINESTRING((10 20, 20 30), (20 30, 30 40)))" }},
            // JSON does not handle FullGlobe
            /* { SpatialType.FullGlobe, new String[] {
                "FULLGLOBE",
                "SRID=1234;FULLGLOBE" }} */
        };

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            wktFormatter = WellKnownTextSqlFormatter.Create();
            jsonFormatter = GeoJsonObjectFormatter.Create();
            gmlFormatter = GmlFormatter.Create();
        }

        #region Collection

        [TestCategory("Partition2"), TestMethod]
        public void GeographyCollection_Serialize()
        {
            Dictionary<Type, object> data = new Dictionary<Type, object>();

            foreach (var sample in testData)
            {
                data.Add(SpatialTestUtil.GeographyTypeFor(sample.Key), sample.Value.Select(wkt => wktFormatter.Read<Geography>(new StringReader(wkt))).ToList());
            }

            using (TestWebRequest request = CreateCollectionReadService(data).CreateForInProcess())
            {
                System.Data.Test.Astoria.TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                    {
                        var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities", format);

                        // feed/entry/content/properties/CollectionGeographyPoint[@type = \"Collection(Edm.Point)\"]"
                        UnitTestsUtil.VerifyXPaths(response,
                            testData.Keys.Select(type =>
                                String.Format("atom:feed/atom:entry/atom:content/adsm:properties/ads:Collection{0}[@adsm:type = \"#Collection({1})\"]",
                                SpatialTestUtil.GeographyTypeFor(type).Name, SpatialTestUtil.GeographyEdmNameFor(type))).ToArray());

                        UnitTestsUtil.VerifyXPaths(response,
                            testData.Select(d =>
                                String.Format("count(atom:feed/atom:entry/atom:content/adsm:properties/ads:Collection{0}/adsm:element) = {1}",
                                SpatialTestUtil.GeographyTypeFor(d.Key).Name, d.Value.Length)).ToArray());
                    });
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeographyCollection_Deserialize()
        {
            StringBuilder payload = new StringBuilder();
            payload.Append("{ \"__metadata\":{ \"uri\": \"http://host/Entities(0)\" }, \"ID\": 0");
            foreach (var kvp in testData)
            {
                payload.AppendLine(",");
                payload.Append(JsonCollectionPropertyFromWkt(SpatialTestUtil.GeographyTypeFor(kvp.Key).Name, kvp.Value, SpatialTestUtil.GeographyEdmNameFor(kvp.Key)));
            }

            payload.AppendLine("}");

            var svc = CreateCollectionWriteService(testData.Keys.Select(t => SpatialTestUtil.GeographyTypeFor(t)).ToList());
            using (TestWebRequest request = svc.CreateForInProcess())
            {
                System.Data.Test.Astoria.TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                    {
                        request.RequestUriString = "/Entities";
                        request.HttpMethod = "POST";
                        request.Accept = "application/atom+xml,application/xml";
                        request.RequestContentType = format;

                        if (format == UnitTestsUtil.JsonLightMimeType)
                        {
                            request.SetRequestStreamAsText(payload.ToString());
                        }
                        else
                        {
                            var xDoc = JsonValidator.ConvertToXDocument(payload.ToString());
                            var atomPayload = UnitTestsUtil.Json2AtomXLinq(xDoc, true).ToString();
                            request.SetRequestStreamAsText(atomPayload);
                        }

                        request.SendRequest();

                        var response = request.GetResponseStreamAsXDocument();

                        UnitTestsUtil.VerifyXPaths(response,
                            testData.Select(d =>
                                String.Format("count(atom:entry/atom:content/adsm:properties/ads:Collection{0}/adsm:element) = {1}",
                                SpatialTestUtil.GeographyTypeFor(d.Key).Name, d.Value.Length)).ToArray());
                    });
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeometryCollection_Serialize()
        {
            Dictionary<Type, object> data = new Dictionary<Type, object>();

            foreach (var sample in testData)
            {
                data.Add(SpatialTestUtil.GeometryTypeFor(sample.Key), sample.Value.Select(wkt => wktFormatter.Read<Geometry>(new StringReader(wkt))).ToList());
            }

            using (TestWebRequest request = CreateCollectionReadService(data).CreateForInProcess())
            {
                System.Data.Test.Astoria.TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                {
                    var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities", format);

                    // feed/entry/content/properties/CollectionGeometryPoint[@type = \"Collection(Edm.Point)\"]"
                    UnitTestsUtil.VerifyXPaths(response,
                        testData.Keys.Select(type =>
                            String.Format("atom:feed/atom:entry/atom:content/adsm:properties/ads:Collection{0}[@adsm:type = \"#Collection({1})\"]",
                            SpatialTestUtil.GeometryTypeFor(type).Name, SpatialTestUtil.GeometryEdmNameFor(type))).ToArray());

                    UnitTestsUtil.VerifyXPaths(response,
                        testData.Select(d =>
                            String.Format("count(atom:feed/atom:entry/atom:content/adsm:properties/ads:Collection{0}/adsm:element) = {1}",
                            SpatialTestUtil.GeometryTypeFor(d.Key).Name, d.Value.Length)).ToArray());
                });
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeometryCollection_Deserialize()
        {
            StringBuilder payload = new StringBuilder();
            payload.Append("{ \"__metadata\":{ \"uri\": \"http://host/Entities(0)\" }, \"ID\": 0");
            foreach (var kvp in testData)
            {
                payload.AppendLine(",");
                payload.Append(JsonCollectionPropertyFromWkt(SpatialTestUtil.GeometryTypeFor(kvp.Key).Name, kvp.Value, SpatialTestUtil.GeometryEdmNameFor(kvp.Key)));
            }

            payload.AppendLine("}");

            var svc = CreateCollectionWriteService(testData.Keys.Select(t => SpatialTestUtil.GeometryTypeFor(t)).ToList());
            using (TestWebRequest request = svc.CreateForInProcess())
            {
                System.Data.Test.Astoria.TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentType = format;

                    if (format == UnitTestsUtil.JsonLightMimeType)
                    {
                        request.SetRequestStreamAsText(payload.ToString());
                    }
                    else
                    {
                        var xDoc = JsonValidator.ConvertToXDocument(payload.ToString());
                        var atomPayload = UnitTestsUtil.Json2AtomXLinq(xDoc, true).ToString();
                        request.SetRequestStreamAsText(atomPayload);
                    }

                    request.SendRequest();

                    var response = request.GetResponseStreamAsXDocument();

                    UnitTestsUtil.VerifyXPaths(response,
                        testData.Select(d =>
                            String.Format("count(atom:entry/atom:content/adsm:properties/ads:Collection{0}/adsm:element) = {1}",
                            SpatialTestUtil.GeometryTypeFor(d.Key).Name, d.Value.Length)).ToArray());
                });
            }
        }

        private String JsonCollectionPropertyFromWkt(String propertyName, String[] wktData, String edmTypeName = null)
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("\"Collection{0}\": {{", propertyName);
            if (edmTypeName != null)
            {
                output.AppendFormat("\"__metadata\": {{ \"type\": \"Collection({0})\" }},", edmTypeName);
            }

            output.AppendLine("\"results\": [");

            StringWriter w = new StringWriter(output);

            bool first = true;
            foreach (string wkt in wktData)
            {
                if (!first)
                {
                    w.Write(',');
                }

                var spatial = wktFormatter.Read<Geography>(new StringReader(wkt));
                var dictionary = jsonFormatter.Write(spatial);
                JsonSpatialValueFromDictionary(dictionary, output);
                
                first = false;
            }

            output.Append("]}");

            return output.ToString();
        }
                
        #endregion

        #region Open Types - Serialization

        [TestCategory("Partition2"), TestMethod]
        public void GeographyAsOpenProperty_Serialize()
        {
            var testCases = testData.Select(kvp =>
                new
                {
                    Type = SpatialTestUtil.GeographyTypeFor(kvp.Key),
                    EdmName = SpatialTestUtil.GeographyEdmNameFor(kvp.Key),
                    Data = kvp.Value.Select(wkt => wktFormatter.Read<Geography>(new StringReader(wkt))).ToArray()
                });

            TestUtil.RunCombinations(testCases, UnitTestsUtil.ResponseFormats, (tcase, format) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(tcase.Data, tcase.Type, true).CreateForInProcessWcf())
                {
                    var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities", format);
                    string rootXpath = "atom:feed/atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    UnitTestsUtil.VerifyXPaths(response, tcase.Data.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray());
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeometryAsOpenProperty_Serialize()
        {
            var testCases = testData.Select(kvp =>
                new
                {
                    Type = SpatialTestUtil.GeometryTypeFor(kvp.Key),
                    EdmName = SpatialTestUtil.GeometryEdmNameFor(kvp.Key),
                    Data = kvp.Value.Select(wkt => wktFormatter.Read<Geometry>(new StringReader(wkt))).ToArray()
                });

            TestUtil.RunCombinations(testCases, UnitTestsUtil.ResponseFormats, (tcase, format) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(tcase.Data, tcase.Type, true).CreateForInProcessWcf())
                {
                    var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities", format);
                    string rootXpath = "atom:feed/atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    if (format == UnitTestsUtil.AtomFormat)
                    {
                        UnitTestsUtil.VerifyXPaths(response, tcase.Data.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray());
                    }
                }
            });
        }

        #endregion

        #region OpenTypes - Deserialization

        [TestCategory("Partition2"), TestMethod]
        public void GeographyAsOpenProperty_AtomWithTypeDeserialize()
        {
            // tests atom deserialization with m:type information
            var testCases = testData.Select(kvp =>
                new
                {
                    Type = SpatialTestUtil.GeographyTypeFor(kvp.Key),
                    EdmName = SpatialTestUtil.GeographyEdmNameFor(kvp.Key),
                    WktData = kvp.Value
                });

            TestUtil.RunCombinations(testCases, (tcase) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(new Geography[tcase.WktData.Length], tcase.Type, true, true).CreateForInProcessWcf())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentType = UnitTestsUtil.AtomFormat;
                    request.SetRequestStreamAsText(AggregateAtomPayloadFromWkt(tcase.Type.Name, tcase.WktData, tcase.EdmName));
                    request.SendRequest();

                    var response = request.GetResponseStreamAsXDocument();
                    string rootXpath = "atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    UnitTestsUtil.VerifyXPaths(response, tcase.WktData.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray());
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeographyAsOpenProperty_Deserialize()
        {
            var testCases = testData.Select(kvp => new
            {
                Type = SpatialTestUtil.GeographyTypeFor(kvp.Key),
                EdmName = SpatialTestUtil.GeographyEdmNameFor(kvp.Key),
                Data = new Geography[kvp.Value.Length],
                Payload = AggregateJsonPayloadFromWkt(SpatialTestUtil.GeographyTypeFor(kvp.Key).Name, kvp.Value, SpatialTestUtil.GeographyEdmNameFor(kvp.Key))
            });

            TestUtil.RunCombinations(testCases, UnitTestsUtil.ResponseFormats, (tcase, format) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(tcase.Data, tcase.Type, true, true).CreateForInProcess())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentType = format;

                    if (format == UnitTestsUtil.JsonLightMimeType)
                    {
                        request.SetRequestStreamAsText(tcase.Payload);
                    }
                    else
                    {
                        // atom from Json - this payload has no m:type
                        var xDoc = JsonValidator.ConvertToXDocument(tcase.Payload.ToString());
                        var atomPayload = UnitTestsUtil.Json2AtomXLinq(xDoc, true).ToString();
                        request.SetRequestStreamAsText(atomPayload);
                    }

                    request.SendRequest();

                    var response = request.GetResponseStreamAsXDocument();
                    string rootXpath = "atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    string[] xpaths = tcase.Data.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray();
                    UnitTestsUtil.VerifyXPaths(response, xpaths);
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeometryAsOpenProperty_AtomWithTypeDeserialize()
        {
            // tests atom deserialization with m:type information
            var testCases = testData.Select(kvp =>
                new
                {
                    Type = SpatialTestUtil.GeometryTypeFor(kvp.Key),
                    EdmName = SpatialTestUtil.GeometryEdmNameFor(kvp.Key),
                    WktData = kvp.Value
                });

            TestUtil.RunCombinations(testCases, (tcase) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(new Geometry[tcase.WktData.Length], tcase.Type, true, true).CreateForInProcessWcf())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentType = UnitTestsUtil.AtomFormat;
                    request.SetRequestStreamAsText(AggregateAtomPayloadFromWkt(tcase.Type.Name, tcase.WktData, tcase.EdmName));
                    request.SendRequest();

                    var response = request.GetResponseStreamAsXDocument();
                    string rootXpath = "atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    UnitTestsUtil.VerifyXPaths(response, tcase.WktData.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray());
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void GeometryAsOpenProperty_Deserialize()
        {
            var testCases = testData.Select(kvp => new
            {
                Type = SpatialTestUtil.GeometryTypeFor(kvp.Key),
                EdmName = SpatialTestUtil.GeometryEdmNameFor(kvp.Key),
                Data = new Geometry[kvp.Value.Length],
                Payload = AggregateJsonPayloadFromWkt(SpatialTestUtil.GeometryTypeFor(kvp.Key).Name, kvp.Value, SpatialTestUtil.GeometryEdmNameFor(kvp.Key))
            });

            TestUtil.RunCombinations(testCases, UnitTestsUtil.ResponseFormats, (tcase, format) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(tcase.Data, tcase.Type, true, true).CreateForInProcessWcf())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentType = format;

                    if (format == UnitTestsUtil.JsonLightMimeType)
                    {
                        request.SetRequestStreamAsText(tcase.Payload);
                    }
                    else
                    {
                        // atom from Json - this payload has no m:type
                        var xDoc = JsonValidator.ConvertToXDocument(tcase.Payload.ToString());
                        var atomPayload = UnitTestsUtil.Json2AtomXLinq(xDoc, true).ToString();
                        request.SetRequestStreamAsText(atomPayload);
                    }

                    request.SendRequest();

                    var response = request.GetResponseStreamAsXDocument();
                    string rootXpath = "atom:entry/atom:content/adsm:properties/ads:" + tcase.Type.Name;
                    string[] xpaths = tcase.Data.Select((v, i) => rootXpath + i + "[@adsm:type = '" + tcase.EdmName + "' and namespace-uri(*) = 'http://www.opengis.net/gml']").ToArray();
                    UnitTestsUtil.VerifyXPaths(response, xpaths);
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void SpatialOpenProperty_AtomWrongTypeDeserialize()
        {
            var testCases = testData.Select(kvp =>
                new
                {
                    Type = SpatialTestUtil.GeographyTypeFor(kvp.Key),
                    WktData = new String[] { kvp.Value.Last() }
                });

            String[] edmCases = { "Edm.Double", "Edm.String" };

            TestUtil.RunCombinations(testCases, edmCases, (tcase, edmName) =>
            {
                using (TestWebRequest request = CreateSpatialPropertyService(new Geography[tcase.WktData.Length], tcase.Type, true, true).CreateForInProcess())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.RequestContentType = UnitTestsUtil.AtomFormat;
                    request.SetRequestStreamAsText(AggregateAtomPayloadFromWkt(tcase.Type.Name, tcase.WktData, edmName));
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(ex);
                    while (ex.InnerException != null) ex = ex.InnerException;
                    Assert.AreEqual(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element"), ex.Message);
                }
            });
        }
        
        #endregion

        [TestCategory("Partition2"), TestMethod]
        public void SpatialAsKeys()
        {
            var types = testData.Select(kvp => SpatialTestUtil.GeographyTypeFor(kvp.Key))
                .Union(testData.Select(kvp => SpatialTestUtil.GeometryTypeFor(kvp.Key)))
                .Union(new Type[] { typeof(Geography), typeof(Geometry) })
                .ToArray();

            TestUtil.RunCombinations(types, (t) =>
            {
                DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
                var entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddResourceSet("Entities", entityType);
                Exception ex = TestUtil.RunCatching<InvalidOperationException>(
                    () => metadata.AddKeyProperty(entityType, "ID", t));
                
                Assert.IsNotNull(ex);
                Assert.AreEqual(DataServicesResourceUtil.GetString("ResourceType_SpatialKeyOrETag", "ID", "EntityType"), ex.Message);
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void SpatialAsETags()
        {
            var types = testData.Select(kvp => SpatialTestUtil.GeographyTypeFor(kvp.Key))
                .Union(testData.Select(kvp => SpatialTestUtil.GeometryTypeFor(kvp.Key)))
                .Union(new Type[] { typeof(Geography), typeof(Geometry) })
                .ToArray();

            TestUtil.RunCombinations(types, (t) =>
            {
                DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
                var entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddResourceSet("Entities", entityType);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                Exception ex = TestUtil.RunCatching<InvalidOperationException>(
                    () => metadata.AddPrimitiveProperty(entityType, "Property", t, true));

                Assert.IsNotNull(ex);
                Assert.AreEqual(DataServicesResourceUtil.GetString("ResourceType_SpatialKeyOrETag", "Property", "EntityType"), ex.Message);
            });
        }
        
        private string AggregateAtomPayloadFromWkt(String spatialTypeName, String[] wktData, String edmName)
        {
            StringBuilder payload = new StringBuilder();
            payload.Append(@"<entry xml:base=""http://pqianvm2:34688/TheTest/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"">
  <id>http://pqianvm2:34688/TheTest/Entities(0)</id>
  <category term=""#AstoriaUnitTests.Tests.EntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <link rel=""edit"" title=""EntityType"" href=""Entities(0)"" />
  <title />
  <updated>2011-10-21T22:47:02Z</updated>
  <author>
    <name />
  </author>
  <content type=""application/xml"">
    <m:properties>
      <d:ID m:type=""Edm.Int32"">0</d:ID>");
            XmlWriter writer = XmlWriter.Create(payload, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            var gmlWriter = gmlFormatter.CreateWriter(writer);
            for (int i = 0; i < wktData.Length; ++i)
            {
                payload.AppendFormat("<d:{0}{1} m:type=\"{2}\">", spatialTypeName, i, edmName);
                wktFormatter.Read<Geometry>(new StringReader(wktData[i]), gmlWriter);
                writer.Flush();
                payload.AppendFormat("</d:{0}{1}>", spatialTypeName, i);
            }
            payload.Append("</m:properties></content></entry>");

            return payload.ToString();
        }

        private String AggregateJsonPayloadFromWkt(String spatialTypeName, String[] wktData, string edmTypeName)
        {
            StringBuilder payload = new StringBuilder();
            payload.Append("{ \"__metadata\":{ \"uri\": \"http://host/Entities(0)\" }, \"ID\": 0");

            for (int i = 0; i < wktData.Length; ++i)
            {
                payload.Append(",");
                payload.AppendFormat("\"{0}\": ", spatialTypeName + i);
                payload.Append(JsonPropertyFromWkt(wktData[i], edmTypeName));
            }

            payload.Append("}");

            return payload.ToString();
        }

        private String JsonPropertyFromWkt(String wktData, string edmTypeName)
        {
            StringBuilder output = new StringBuilder();
            
            var spatial = wktFormatter.Read<Geography>(new StringReader(wktData));
            var dictionary = jsonFormatter.Write(spatial);
            JsonSpatialValueFromDictionary(dictionary, output);
            output.Insert(1, "__metadata: { type : '" + edmTypeName + "' }, ");
            return output.ToString();
        }

        private void JsonSpatialValueFromDictionary(IDictionary<string, object> dictionary, StringBuilder output)
        {
            output.Append("{");
            bool first = true;
            foreach (var property in dictionary)
            {
                if (!first)
                {
                    output.Append(", ");
                }

                output.AppendFormat("'{0}': ", property.Key);
                JsonSpatialValueFromObject(property.Value, output);
                first = false;
            }

            output.Append("}");
        }

        private void JsonSpatialValueFromObject(object jsonObject, StringBuilder output)
        {
            if (jsonObject == null)
            {
                output.Append("null");
                return;
            }

            var valueAsDictionary = jsonObject as IDictionary<string, object>;
            if (valueAsDictionary != null)
            {
                JsonSpatialValueFromDictionary(valueAsDictionary, output);
                return;
            }

            var valueAsString = jsonObject as string;
            if (valueAsString != null)
            {
                output.AppendFormat("'{0}'", valueAsString);
                return;
            }

            var valueAsArray = jsonObject as IEnumerable;
            if (valueAsArray != null)
            {
                output.Append("[");
                bool first = true;
                foreach (var element in valueAsArray)
                {
                    if (!first)
                    {
                        output.Append(", ");
                    }

                    JsonSpatialValueFromObject(element, output);
                    first = false;
                }

                output.Append("]");
                return;
            }

            output.Append(jsonObject.ToString());
        }

        /// <summary>
        /// Create a service for a specific spatial property type.
        /// </summary>
        /// <remarks>
        /// DEVNOTE(pqian):
        /// The service will populate with properties named after the property type, with a sequence number
        /// indicating which test sample it holds. For example, if the method is called with 3 sample data, 
        /// and the target type is GeographyPoint, then the service will look like
        /// EntityType:
        /// ID
        /// GeographyPoint1
        /// GeographyPoint2
        /// GeographyPoint3
        /// </remarks>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="data">Sample Data</param>
        /// <param name="overrideType">Use this type instead of typeof(T)</param>
        /// <param name="openProperty">Use Open Type</param>
        /// <param name="writable">Writable service</param>
        /// <returns>The constructed service definition</returns>
        private DSPServiceDefinition CreateSpatialPropertyService<T>(T[] data, Type overrideType = null, bool openProperty = false, bool writable = false)
        {
            Type propertyType = overrideType ?? typeof(T);

            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));

            if (!openProperty)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    metadata.AddPrimitiveProperty(entityType, propertyType.Name + i, propertyType);
                }
            }

            entityType.IsOpenType = openProperty;
            
            var definition = new DSPServiceDefinition()
            {
                Metadata = metadata
            };

            if (writable)
            {
                definition.CreateDataSource = (m) => new DSPContext();
                definition.Writable = true;
            }
            else
            {
                DSPContext dataSource = new DSPContext();
                var entities = dataSource.GetResourceSetEntities("Entities");
                var resource = new DSPResource(entityType);
                resource.SetValue("ID", 0);

                for (int i = 0; i < data.Length; ++i)
                {
                    if (!openProperty)
                    {
                        metadata.AddPrimitiveProperty(entityType, propertyType.Name + i, propertyType);
                    }

                    resource.SetValue(propertyType.Name + i, data[i]);
                }

                entities.Add(resource);
                definition.DataSource = dataSource;
            }

            return definition;
        }

        private DSPServiceDefinition CreateCollectionWriteService(List<Type> types)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            foreach (Type t in types)
            {
                metadata.AddCollectionProperty(entityType, "Collection" + t.Name, t);
            }
            
            return new DSPServiceDefinition()
            {
                Metadata = metadata,
                Writable = true,
                CreateDataSource = (m) => new DSPContext()
            };
        }

        private DSPServiceDefinition CreateCollectionReadService(Dictionary<Type, object> data)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            foreach (Type t in data.Keys)
            {
                metadata.AddCollectionProperty(entityType, "Collection" + t.Name, t);
            }

            DSPContext dataSource = new DSPContext();
            var entities = dataSource.GetResourceSetEntities("Entities");
            var resource = new DSPResource(entityType);
            resource.SetValue("ID", 0);

            foreach (KeyValuePair<Type, object> d in data)
            {
                resource.SetValue("Collection" + d.Key.Name, d.Value);
            }

            entities.Add(resource);

            return new DSPServiceDefinition()
            {
                Metadata = metadata,
                DataSource = dataSource
            };
        }
    }
}

