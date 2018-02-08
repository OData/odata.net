//---------------------------------------------------------------------
// <copyright file="SpatialTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Service.Providers;
using System.Data.Test.Astoria;
using System.Linq;
using System.Net;
using Microsoft.Spatial;
using System.Xml.Linq;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.Test.ModuleCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    using System.IO;
    using ServiceOperation = Microsoft.OData.Service.Providers.ServiceOperation;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [TestClass]
    public class SpatialTests
    {
        private static Dictionary<String, String> edmTypeMapping = new Dictionary<string, string>
            {
                { "Geography", "Geography" },
                { "GeographyPoint", "GeographyPoint" },
                { "GeographyLineString", "GeographyLineString" },
                { "GeographyPolygon", "GeographyPolygon" },
                { "GeographyMultiPoint", "GeographyMultiPoint" },
                { "GeographyMultiLineString", "GeographyMultiLineString" },
                { "GeographyMultiPolygon", "GeographyMultiPolygon" },
                { "GeographyCollection", "GeographyCollection" },
                
                { "Geometry", "Geometry" },
                { "GeometryPoint", "GeometryPoint" },
                { "GeometryLineString", "GeometryLineString" },
                { "GeometryPolygon", "GeometryPolygon" },
                { "GeometryMultiPoint", "GeometryMultiPoint" },
                { "GeometryMultiLineString", "GeometryMultiLineString" },
                { "GeometryMultiPolygon", "GeometryMultiPolygon" },
                { "GeometryCollection", "GeometryCollection" }
            };

        #region Test Cases

        public static string GetUriLiteral(Type type, ISpatial spatial)
        {
            string typePrefix = typeof(Geometry).IsAssignableFrom(type) ? "geometry" : "geography";
            return string.Format("{0}'{1}'", typePrefix, WellKnownTextSqlFormatter.Create().Write(spatial));
        }

        [TestCategory("Partition2"), TestMethod, Variation("Validate that geography and geometry error the same on equality comparison")]
        public void TestEqualityHandling()
        {
            string geographyLiteral = GetUriLiteral(typeof(GeographyPoint),
                                                    TestPoint.DefaultValues.TripLegGeography1.AsGeography());
            string geometryLiteral = GetUriLiteral(typeof(GeometryPoint),
                                                    TestPoint.DefaultValues.TripLegGeography1.AsGeography());
            var testCases = new[]
                                {
                                    new
                                        {
                                            Type = typeof(GeographyPoint),
                                            Left = "GeographyProperty1",
                                            Right = geographyLiteral,
                                            Operator = "eq",
                                            OperatorName = "Equal"

                                        },
                                    new
                                        {
                                            Type = typeof(GeographyPoint),
                                            Left = geographyLiteral,
                                            Right = "GeographyProperty1",
                                            Operator = "eq",
                                            OperatorName = "Equal"

                                        },
                                    new
                                        {
                                            Type = typeof(GeometryPoint),
                                            Left = "GeographyProperty1",
                                            Right = geometryLiteral,
                                            Operator = "eq",
                                            OperatorName = "Equal"
                                        },
                                    new
                                        {
                                            Type = typeof(GeometryPoint),
                                            Left = geometryLiteral,
                                            Right = "GeographyProperty1",
                                            Operator = "eq",
                                            OperatorName = "Equal"
                                        },

                                };

            TestUtil.RunCombinations(testCases,
                                     (testCase) =>
                                     {
                                         DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(testCase.Type, TestPoint.DefaultValues);
                                         using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcess())
                                         {
                                             request.StartService();
                                             // GET
                                             request.Accept = UnitTestsUtil.MimeAny;
                                             request.RequestContentType = null;
                                             request.HttpMethod = "GET";
                                             request.RequestUriString = string.Format("/TripLegs/?$filter={0} {1} {2}", testCase.Left, testCase.Operator, testCase.Right);
                                             int operatorPos = testCase.Left.Length + 1;
                                             WebException e = TestUtil.RunCatching<WebException>(() => request.SendRequest());
                                             Assert.IsNotNull(e, "didn't get the exception we should have gotten");
                                             string expectedMessage = ODataLibResourceUtil.GetString(
                                                     "MetadataBinder_IncompatibleOperandsError",
                                                     "Edm." + testCase.Type.Name,
                                                     "Edm." + testCase.Type.Name,
                                                     testCase.OperatorName,
                                                     operatorPos);
                                             Assert.AreEqual(expectedMessage, e.InnerException.Message, "didn't get the correct error");
                                         }

                                     });
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod, Variation("Validate that ServiceOperations with spatial parameters work correctly")]
        public void TestServiceOperationsWithSpatialParameters()
        {

            Action<string, Type, DSPMetadata> AddIdentityServiceOp = (name, type, dspMetadata) =>
            {
                var primitiveType = Microsoft.OData.Service.Providers.ResourceType.GetPrimitiveResourceType(type);
                var parameter = new ServiceOperationParameter("param1", primitiveType);
                dspMetadata.AddServiceOperation(name, ServiceOperationResultKind.DirectValue, primitiveType, null, "GET",
                                      new ServiceOperationParameter[] { parameter });
            };

            DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(typeof(GeographyPoint), TestPoint.DefaultValues, false, false,
                (m) =>
                {
                    AddIdentityServiceOp("GetGeographyPoint", typeof(GeographyPoint), m);
                    AddIdentityServiceOp("GetGeometryPoint", typeof(GeometryPoint), m);
                    AddIdentityServiceOp("GetDouble", typeof(double), m);
                });


            Func<object[], object> sopCallBack = (args) =>
                                                     {
                                                         return args[0];
                                                     };
            roadTripServiceDefinition.CreateDataSource = (metadata) =>
                                                             {
                                                                 return new DSPContext()
                                                                            {
                                                                                ServiceOperations =
                                                                                    new Dictionary
                                                                                    <string, Func<object[], object>>()
                                                                                        {
                                                                                         {"GetGeographyPoint", sopCallBack},
                                                                                         {"GetGeometryPoint", sopCallBack},
                                                                                         {"GetDouble", sopCallBack},
                                                                                        }
                                                                            };
                                                             };

            using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcess())
            {
                request.StartService();

                // GET
                request.Accept = "application/xml";
                request.RequestContentType = null;
                request.HttpMethod = "GET";
                request.RequestUriString = "/GetDouble?param1=1.2";
                request.SendRequest();
                var response = request.GetResponseStreamAsText();
                StringAssert.Contains(response, "1.2", "didn't get the identity back");

                const string wellKnownText = "SRID=4326;POINT (177.508 51.9917)";

                // GET
                request.Accept = "application/xml";

                request.RequestContentType = null;
                request.HttpMethod = "GET";
                request.RequestUriString = string.Format("/GetGeographyPoint?param1=geography'{0}'", wellKnownText);
                request.SendRequest();
                response = request.GetResponseStreamAsText();
                var geographyPoint = WellKnownTextSqlFormatter.Create().Read<Geography>(new StringReader(wellKnownText));
                string geographyPointReturn = GmlFormatter.Create().Write(geographyPoint);
                // The gml namespace is already declared on the top-level element
                geographyPointReturn = geographyPointReturn.Replace(" xmlns:gml=\"http://www.opengis.net/gml\"", string.Empty);
                StringAssert.Contains(response, geographyPointReturn, "didn't get the identity back");

                // GET
                request.Accept = "application/xml";
                request.RequestContentType = null;
                request.HttpMethod = "GET";
                request.RequestUriString = string.Format("/GetGeometryPoint?param1=geometry'{0}'", wellKnownText);
                request.SendRequest();
                response = request.GetResponseStreamAsText();
                var geometryPoint = WellKnownTextSqlFormatter.Create().Read<Geometry>(new StringReader(wellKnownText));
                string geometryPointReturn = GmlFormatter.Create().Write(geometryPoint);
                // The gml namespace is already declared on the top-level element
                geometryPointReturn = geometryPointReturn.Replace(" xmlns:gml=\"http://www.opengis.net/gml\"", string.Empty);
                StringAssert.Contains(response, geometryPointReturn, "didn't get the identity back");
            }
        }

        [TestCategory("Partition2"), TestMethod, Variation("validate that the proper error is given for $value requests on spatial typed properties")]
        public void DollarValueFailsClosedTypes()
        {
            DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(typeof(GeographyPoint), TestPoint.DefaultValues);
            Action<TestWebRequest> sendRequestAndAssertError = (req) =>
            {
                // we only support $value for spatial type for GET and DELETE now.
                if (req.HttpMethod == "GET")
                {
                    req.SendRequest();
                    Assert.AreEqual(200, req.ResponseStatusCode);
                }
                else if (req.HttpMethod == "POST")
                {
                    WebException e = TestUtil.RunCatching<WebException>(() => req.SendRequest());
                    TestUtil.AssertExceptionStatusCode(e, 405, "get can't be serialized, will be an instream error");
                }
                else if (req.HttpMethod == "DELETE")
                {
                    req.SendRequest();
                    Assert.AreEqual(204, req.ResponseStatusCode);
                }
                else
                {
                    WebException e = TestUtil.RunCatching<WebException>(() => req.SendRequest());
                    TestUtil.AssertExceptionStatusCode(e, 400, "get can't be serialized, will be an instream error");
                }
            };
            DollarValueGivesErrorForAllVerbs(roadTripServiceDefinition, sendRequestAndAssertError);
        }

        [TestCategory("Partition2"), TestMethod, Variation("validate that the proper error is given for $value requests on spatial typed properties")]
        public void DollarValueFailsOpenTypes()
        {
            DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(typeof(GeographyPoint), TestPoint.DefaultValues, false, true);
            Action<TestWebRequest> sendRequestAndAssertBehavior = (req) =>
            {
                if (req.HttpMethod == "GET")
                {
                    //WebException e = TestUtil.RunCatching<WebException>(() => req.SendRequest());
                    //TestUtil.AssertExceptionStatusCode(e, 400, "get can't be serialized, will be an instream error");
                    req.SendRequest();
                    Assert.AreEqual(200, req.ResponseStatusCode);
                }
                else if (req.HttpMethod == "POST")
                {
                    WebException e = TestUtil.RunCatching<WebException>(() => req.SendRequest());
                    TestUtil.AssertExceptionStatusCode(e, 400, "get can't be serialized, will be an instream error");
                }
                else
                {
                    // PUT/PATCH/DELETE
                    req.SendRequest();
                    Assert.AreEqual(204, req.ResponseStatusCode);
                }
            };
            DollarValueGivesErrorForAllVerbs(roadTripServiceDefinition, sendRequestAndAssertBehavior);
        }

        public void DollarValueGivesErrorForAllVerbs(DSPUnitTestServiceDefinition roadTripServiceDefinition, Action<TestWebRequest> sendRequestAndAssertBehavior)
        {
            using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcess())
            {
                request.StartService();


                // GET
                request.Accept = UnitTestsUtil.MimeTextPlain;
                request.RequestContentType = null;
                request.HttpMethod = "GET";
                request.RequestUriString = string.Format("/TripLegs({0})/GeographyProperty1/$value", SpatialTestUtil.DefaultId);
                sendRequestAndAssertBehavior(request);

                // PUT
                request.HttpMethod = "PUT";
                request.RequestContentType = UnitTestsUtil.MimeTextPlain;
                request.RequestUriString = string.Format("/TripLegs({0})/GeographyProperty1/$value", SpatialTestUtil.DefaultId);
                string newValue = GmlFormatter.Create().Write(TestPoint.NewValues.TripLegGeography1.AsGeography());
                request.RequestStream = IOUtil.CreateStream(newValue);
                sendRequestAndAssertBehavior(request);

                // POST
                request.HttpMethod = "POST";
                sendRequestAndAssertBehavior(request);

                // PATCH
                request.HttpMethod = "PATCH";
                sendRequestAndAssertBehavior(request);

                // DELETE
                request.HttpMethod = "DELETE";
                string uri = string.Format("/TripLegs({0})/GeographyProperty1/$value", SpatialTestUtil.DefaultId);
                request.RequestUriString = uri;
                sendRequestAndAssertBehavior(request);
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod, Variation("Serialize Geodetic properties on entities using the ObjectContextServiceProvider and Atom and Json formats")]
        public void SerializeGeodeticPropertiesInResource()
        {
            var testCases = new[] 
            {
                new
                {
                    GeographyType = typeof(GeographyPoint),
                    DefaultValues = TestPoint.DefaultValues
                },
                new
                {
                    GeographyType = typeof(GeographyLineString),
                    DefaultValues = TestLineString.DefaultValues
                },
            };

            var responseFromats = new string[] {UnitTestsUtil.AtomFormat};
            TestUtil.RunCombinations(testCases, responseFromats, (testCase, responseFormat) =>
            {
                GeographyPropertyValues defaultValues = testCase.DefaultValues;

                DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(testCase.GeographyType, defaultValues);
                using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcess())
                {
                    request.StartService();
                    request.Accept = responseFormat;

                    ResourceVerification verification = GetResourceVerification(responseFormat, SpatialTestUtil.DefaultId, defaultValues, request);

                    // Geography property followed by another geography property
                    verification.GetAndVerifyTripLeg();

                    // Geography property followed by another non-geography property
                    verification.GetAndVerifyAmusementPark();

                    // Geography property at the end of the entry
                    verification.GetAndVerifyRestStop();
                }
            });
        }

        private static ResourceVerification GetResourceVerification(string responseFormat, int id, GeographyPropertyValues defaultValues, TestWebRequest request)
        {
            Assert.IsTrue(responseFormat == UnitTestsUtil.AtomFormat || responseFormat == UnitTestsUtil.JsonLightMimeType, "Response format {0} not recognized in GetResourceVerification.", responseFormat);
            DSPResourceSerializerFormat payloadFormat = responseFormat == UnitTestsUtil.AtomFormat ? DSPResourceSerializerFormat.Atom : DSPResourceSerializerFormat.Json;
            return new ResourceVerification(request, payloadFormat, id, defaultValues);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod, Variation("Insert and update Geodetic properties on entities using the ObjectContextServiceProvider and all supported format")]
        public void InsertAndUpdateGeodeticProperties()
        {
            var testCases = new[] 
            {
                new
                {
                    GeographyType = typeof(Geography),
                    DefaultValues = TestLineString.DefaultValues,
                    NewValues = TestLineString.NewValues,
                },
                new
                {
                    GeographyType = typeof(GeographyPoint),
                    DefaultValues = TestPoint.DefaultValues,
                    NewValues = TestPoint.NewValues,
                },
                new
                {
                    GeographyType = typeof(GeographyLineString),
                    DefaultValues = TestLineString.DefaultValues,
                    NewValues = TestLineString.NewValues,
                },
            };

            TestUtil.RunCombinations(testCases, UnitTestsUtil.BooleanValues, UnitTestsUtil.ResponseFormats, (testCase, enableTypeConversion, payloadFormat) =>
            {
                GeographyPropertyValues defaultValues = testCase.DefaultValues;
                bool useComplexType = false;
                DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(testCase.GeographyType, defaultValues, useComplexType);

                // EnableTypeConversion is interesting here because it affects whether or not we pay attention to the type that's specified on the wire, instead of metadata
                // Since spatial types can specify Geography in metadata but GeographyPoint as the value, we need to verify it works either way
                roadTripServiceDefinition.EnableTypeConversion = enableTypeConversion;

                using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcess())
                {
                    request.StartService();

                    TestUtil.RunCombinations(
                        new string[] { "POST", "PUT", "PATCH", "PATCH" },
                        new string[] { null, "return=representation", "return=minimal" },
                        UnitTestsUtil.BooleanValues,
                        (httpMethod, preferHeader, useBatch) =>
                        {
                            bool isPost = httpMethod == "POST";
                            bool expectedReturnContent = preferHeader == "return=representation" || isPost && preferHeader == null;

                            DSPMetadata roadTripMetadata = roadTripServiceDefinition.Metadata;
                            DSPContext changeScopeData = SpatialTestUtil.PopulateRoadTripData(roadTripMetadata, defaultValues, useComplexType);
                            using (IDisposable changeScope = roadTripServiceDefinition.CreateChangeScope(changeScopeData))
                            {
                                int id = isPost ? 2 : SpatialTestUtil.DefaultId;

                                ResourceVerification verification = GetResourceVerification(payloadFormat, id, testCase.NewValues, request);

                                TestTripLegUpdate(verification, httpMethod, preferHeader, useBatch, roadTripMetadata);
                                TestAmusementParkUpdate(verification, httpMethod, preferHeader, useBatch, roadTripMetadata);
                                TestRestStopUpdate(verification, httpMethod, preferHeader, useBatch, roadTripMetadata);
                            }
                        });
                }
            });
        }

        [TestCategory("Partition2"), TestMethod, Variation("Verify spatial properties are represented correctly in $metadata for IDSMP")]
        public void SpatialMetadataIDSMP()
        {
            TestSpatialMetadata(DSPDataProviderKind.CustomProvider);
        }

        #endregion

        #region Helper Methods

        internal static DSPUnitTestServiceDefinition GetRoadTripServiceDefinition(Type geographyType, GeographyPropertyValues defaultValues, bool useComplexType = false, bool useOpenTypes = false, Action<DSPMetadata> modifyMetadata = null)
        {
            DSPMetadata roadTripMetadata = SpatialTestUtil.CreateRoadTripMetadata(geographyType, useComplexType, useOpenTypes, modifyMetadata);
            return SpatialTestUtil.CreateRoadTripServiceDefinition(roadTripMetadata, defaultValues, DSPDataProviderKind.CustomProvider, useComplexType);
        }

        private static void TestSpatialMetadata(DSPDataProviderKind providerKind)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialMetadata", "AstoriaUnitTests.Tests");

            // Entity with all types of geography properties
            KeyValuePair<string, Type>[] geographyProperties = new KeyValuePair<string, Type>[] {
                new KeyValuePair<string, Type>("GeographyProperty", typeof(Geography)),
                new KeyValuePair<string, Type>("PointProperty", typeof(GeographyPoint)),
                new KeyValuePair<string, Type>("LineStringProperty", typeof(GeographyLineString)),
            };
            string entityTypeName = "AllGeographyTypes";
            SpatialTestUtil.AddEntityType(metadata, entityTypeName, geographyProperties, useComplexType: false, useOpenTypes: false);
            metadata.SetReadOnly();

            var service = new DSPUnitTestServiceDefinition(metadata, providerKind, new DSPContext());

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.StartService();

                request.HttpMethod = "GET";
                XDocument response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/$metadata");

                XElement schemaElement = response.Root.Element(UnitTestsUtil.EdmxNamespace + "DataServices").Elements().Single(e => e.Name.LocalName == "Schema");
                XNamespace edmNs = schemaElement.Name.Namespace;
                XElement entityElement = schemaElement.Elements(edmNs + "EntityType").Single(e => (string)e.Attribute("Name") == entityTypeName);

                foreach (KeyValuePair<string, Type> property in geographyProperties)
                {
                    XElement propertyElement = entityElement.Elements(edmNs + "Property").Single(e => (string)e.Attribute("Name") == property.Key);

                    // EF provider currently represents all types as Edm.Geography in metadata (property is DbGeography on the entity), otherwise it should reflect the actual type
                    string expectedEdmTypeName = providerKind == DSPDataProviderKind.EF ? "Edm.Geography" : GetExpectedEdmTypeName(property.Value);

                    Assert.AreEqual(expectedEdmTypeName, propertyElement.Attribute("Type").Value, "Wrong property type for property {0},", property.Key);

                    XAttribute attribute = propertyElement.Attributes().Where(a => a.Name == "SRID").Single();
                    Assert.AreEqual("Variable", attribute.Value);
                }
            }
        }

        private static void TestTripLegUpdate(ResourceVerification verification, string httpMethod, string preferHeader, bool useBatch, DSPMetadata roadTripMetadata)
        {
            var tripLeg = SpatialTestUtil.CreateTripLegResource(
                roadTripMetadata,
                verification.Id,
                verification.PropertyValues.TripLegGeography1,
                verification.PropertyValues.TripLegGeography2,
                useComplexType: false,
                modifyPropertyValues: null);

            ExecuteUpdate(tripLeg, verification.Id, verification.Request, httpMethod, preferHeader, useBatch, verification.PayloadFormat);

            // Verify that the operation was successful by querying the same data again
            verification.GetAndVerifyTripLeg();
        }

        private static void TestAmusementParkUpdate(ResourceVerification verification, string httpMethod, string preferHeader, bool useBatch, DSPMetadata roadTripMetadata)
        {
            var amusementPark = SpatialTestUtil.CreateAmusementParkResource(
                roadTripMetadata,
                verification.Id,
                verification.PropertyValues.AmusementParkGeography,
                verification.PropertyValues.AmusementParkName,
                useComplexType: false,
                modifyPropertyValues: null);

            ExecuteUpdate(amusementPark, verification.Id, verification.Request, httpMethod, preferHeader, useBatch, verification.PayloadFormat);

            // Verify that the operation was successful by querying the same data again
            verification.GetAndVerifyAmusementPark();
        }

        private static void TestRestStopUpdate(ResourceVerification verification, string httpMethod, string preferHeader, bool useBatch, DSPMetadata roadTripMetadata)
        {
            var restStop = SpatialTestUtil.CreateRestStopResource(
                roadTripMetadata,
                verification.Id,
                verification.PropertyValues.RestStopGeography,
                useComplexType: false,
                modifyPropertyValues: null);

            ExecuteUpdate(restStop, verification.Id, verification.Request, httpMethod, preferHeader, useBatch, verification.PayloadFormat);

            // Verify that the operation was successful by querying the same data again
            verification.GetAndVerifyRestStop();
        }

        private static void ExecuteUpdate(DSPResource resource, int id, TestWebRequest baseRequest, string httpMethod, string preferHeader, bool useBatch, DSPResourceSerializerFormat payloadFormat)
        {
            string payload = DSPResourceSerializer.WriteEntity(resource, payloadFormat);

            bool isPost = httpMethod == "POST";
            bool expectedReturnContent = preferHeader == "return=representation" || isPost && preferHeader == null;

            string uriSuffix = isPost ? String.Empty : String.Format("({0})", id);

            TestWebRequest request = useBatch ? new InMemoryWebRequest() : baseRequest;
            request.RequestUriString = String.Format("/{0}s{1}", resource.ResourceType.Name, uriSuffix);
            request.HttpMethod = httpMethod;
            request.RequestVersion = "4.0;";
            request.RequestMaxVersion = "4.0;";
            request.RequestHeaders["Prefer"] = preferHeader;
            request.Accept = payloadFormat == DSPResourceSerializerFormat.Atom ? "application/atom+xml,application/xml" : UnitTestsUtil.JsonLightMimeType;
            request.RequestContentType = "application/atom+xml";
            request.SetRequestStreamAsText(payload);

            if (useBatch)
            {
                var batchRequest = new BatchWebRequest();
                var changeset = new BatchWebRequest.Changeset();
                changeset.Parts.Add((InMemoryWebRequest)request);
                batchRequest.Changesets.Add(changeset);
                batchRequest.SendRequest(baseRequest);

                Assert.IsTrue(UnitTestsUtil.IsSuccessStatusCode(baseRequest.ResponseStatusCode), "Unexpected error occurred on batch.");
            }
            else
            {
                request.SendRequest();
            }

            Assert.IsTrue(UnitTestsUtil.IsSuccessStatusCode(request.ResponseStatusCode), "Unexpected error occurred when sending the request.");
            if (expectedReturnContent)
            {
                // If the request is expected to return content, verify there were no instream errors
                Exception e = request.ParseResponseInStreamError();
                string errorMessage = e != null ? e.Message : string.Empty;
                Assert.IsNull(e, "Expected no exception, but got the following error", errorMessage);
            }
        }

        private class ResourceVerification
        {
            public ResourceVerification(TestWebRequest request, DSPResourceSerializerFormat payloadFormat, int id, GeographyPropertyValues propertyValues)
            {
                this.Request = request;
                this.Id = id;
                this.PropertyValues = propertyValues;
                this.PayloadFormat = payloadFormat;
            }

            public TestWebRequest Request { get; private set; }
            public int Id { get; private set; }
            public GeographyPropertyValues PropertyValues { get; private set; }
            public DSPResourceSerializerFormat PayloadFormat { get; private set; }

            public void GetAndVerifyTripLeg()
            {
                XElement tripLegProperties = GetAtomEntryProperties(this.Request, "TripLegs", this.Id);
                VerifyKey(tripLegProperties, this.Id);
                VerifyGeographyProperty(tripLegProperties, "GeographyProperty1", this.PropertyValues.TripLegGeography1);
                VerifyGeographyProperty(tripLegProperties, "GeographyProperty2", this.PropertyValues.TripLegGeography2);
            }

            public void GetAndVerifyAmusementPark()
            {
                XElement amusementParkProperties = GetAtomEntryProperties(this.Request, "AmusementParks", this.Id);
                VerifyKey(amusementParkProperties, this.Id);
                VerifyGeographyProperty(amusementParkProperties, "GeographyProperty", this.PropertyValues.AmusementParkGeography);
                VerifyPropertyAsString(amusementParkProperties, "Name", this.PropertyValues.AmusementParkName);
            }

            public void GetAndVerifyRestStop()
            {
                XElement restStopProperties = GetAtomEntryProperties(this.Request, "RestStops", this.Id);
                VerifyKey(restStopProperties, this.Id);
                VerifyGeographyProperty(restStopProperties, "GeographyProperty", this.PropertyValues.RestStopGeography);
            }
        }

        private static XElement GetAtomEntryProperties(TestWebRequest request, string resourceSetName, int id)
        {
            request.HttpMethod = "GET";
            XDocument response = UnitTestsUtil.GetResponseAsAtomXLinq(request, string.Format("/{0}({1})", resourceSetName, id), "application/atom+xml,application/xml");
            XElement entry = response.Root.Element(UnitTestsUtil.AtomNamespace + "content");
            Assert.IsNotNull(entry, "Expected the request to produce an ATOM entry.");
            return entry.Element(UnitTestsUtil.MetadataNamespace + "properties");
        }

        private static void VerifyKey(XElement properties, int id)
        {
            VerifyPropertyAsString(properties, "ID", id.ToString());
        }

        private static void VerifyGeographyProperty(XElement properties, string propertyName, ITestGeography expectedValue)
        {
            XElement property = GetProperty(properties, propertyName);
            XAttribute typeAttribute = property.Attribute(UnitTestsUtil.MetadataNamespace + "type");
            if (typeAttribute != null)
            {
                string actualType = property.Attribute(UnitTestsUtil.MetadataNamespace + "type").Value;
                string expectedtype = GetExpectedEdmShortQualifiedTypeName(expectedValue.AsGeography().GetType());
                Assert.AreEqual(expectedtype, actualType, "Type on Geography property is incorrect");
            }

            expectedValue.VerifyGmlContent(property);
        }

        private static void VerifyPropertyAsString(XElement properties, string propertyName, string expectedValue)
        {
            XElement property = GetProperty(properties, propertyName);
            Assert.AreEqual(expectedValue, property.Value, "Property {0} does not have the expected value.", propertyName);
        }

        private static XElement GetProperty(XElement properties, string propertyName)
        {
            XElement property = properties.Element(UnitTestsUtil.DataNamespace + propertyName);
            Assert.IsNotNull(property, "Payload does not contain the expected property {0}", propertyName);
            return property;
        }

        private static string GetExpectedEdmTypeName(Type type)
        {
            string typeName = type.Name;

            if (typeName.EndsWith("Implementation"))
            {
                typeName = typeName.Replace("Implementation", null);
            }

            string edmName;
            if (edmTypeMapping.TryGetValue(typeName, out edmName))
            {
                return "Edm." + edmName;
            }
            else
            {
                throw new InternalTestFailureException("TypeName to EdmName table has changed. Update the 'edmTypeMapping' table");
            }
        }

        private static string GetExpectedEdmShortQualifiedTypeName(Type type)
        {
            string typeName = type.Name;

            if (typeName.EndsWith("Implementation"))
            {
                typeName = typeName.Replace("Implementation", null);
            }

            string edmName;
            if (edmTypeMapping.TryGetValue(typeName, out edmName))
            {
                return edmName;
            }
            else
            {
                throw new InternalTestFailureException("TypeName to EdmName table has changed. Update the 'edmTypeMapping' table");
            }
        }
        #endregion
    }
}
