//---------------------------------------------------------------------
// <copyright file="MultipleTypeSegmentsIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.ServiceModel;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [TestClass]
    public class MultipleTypeSegmentsIntegrationTests
    {
        private static readonly string MostBaseTypeName = typeof(MultipleTypeSegmentsIntegrationTests).FullName + "_MostBaseType";
        private static readonly string EntitySetBaseTypeName = typeof(MultipleTypeSegmentsIntegrationTests).FullName + "_EntitySetBaseType";
        private static readonly string DerivedTypeName = typeof(MultipleTypeSegmentsIntegrationTests).FullName + "_DerivedType";
        private static readonly string VeryDerivedTypeName = typeof(MultipleTypeSegmentsIntegrationTests).FullName + "_VeryDerivedType";
        private static readonly string UnrelatedVeryDerivedTypeName = typeof(MultipleTypeSegmentsIntegrationTests).FullName + "_UnrelatedVeryDerivedType";

        [Ignore] // Remove Atom
        // [TestCategory("Partition1")]
        // [TestMethod]
        public void FilteringPathToMostDerivedTypeThenToBase()
        {
            string requestUriString = "/Entities/" + VeryDerivedTypeName + "/" + MostBaseTypeName;
            string expectedEditLink = "Entities('VeryDerived')/" + VeryDerivedTypeName;
            ResponseShouldContainEntities(requestUriString, expectedEditLink);
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition1")]
        // [TestMethod]
        public void LotsOfTypeSegments()
        {
            string requestUriString = string.Join("/", "/Entities", VeryDerivedTypeName, MostBaseTypeName, MostBaseTypeName, EntitySetBaseTypeName, DerivedTypeName, VeryDerivedTypeName, MostBaseTypeName, EntitySetBaseTypeName);
            string expectedEditLink = "Entities('VeryDerived')/" + VeryDerivedTypeName;
            ResponseShouldContainEntities(requestUriString, expectedEditLink);
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition1")]
        // [TestMethod]
        public void FilteringPathWithBaseTypeShouldIncludeAll()
        {
            string requestUriString = "/Entities/" + MostBaseTypeName;
            ResponseShouldContainEntities(
                requestUriString, 
                "Entities('VeryDerived')/" + VeryDerivedTypeName, 
                "Entities('Unrelated')/" + UnrelatedVeryDerivedTypeName, 
                "Entities('Derived')/" + DerivedTypeName, 
                "Entities('EntitySetBase')");
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition1")]
        // [TestMethod]
        public void AddressingNavigationShouldAllowMultipleTypeSegments()
        {
            string requestUriString = "/Entities('Derived')/" + DerivedTypeName + "/Reference/" + EntitySetBaseTypeName + "/" + MostBaseTypeName;
            ResponseShouldContainEntities(requestUriString, "Entities('FromNavigation')/" + VeryDerivedTypeName);
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilteringPathWithCompletelyUnrelatedTypeShouldBe404()
        {
            const string requestUriString = "/Entities/AstoriaUnitTests.Stubs.Customer";
            RunGetRequest(requestUriString, request => Assert.AreEqual(400, request.ResponseStatusCode));
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilteringPathWithUnrelatedTypeAfterTypeSegmentShouldBe404()
        {
            string requestUriString = "/Entities/" + VeryDerivedTypeName + "/" + UnrelatedVeryDerivedTypeName;
            RunGetRequest(requestUriString, request => Assert.AreEqual(400, request.ResponseStatusCode));
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition1")]
        // [TestMethod]
        public void FilteringPathWithDerivedProperty()
        {
            string requestUriString = "/Entities/" + MostBaseTypeName + "('VeryDerived')/" + DerivedTypeName + "/" + VeryDerivedTypeName + "/VeryDerivedTypeProperty";
            RunGetRequest(requestUriString, request => Assert.AreEqual(200, request.ResponseStatusCode));
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void InsertWithUpcastInUriShouldStillCreateInstanceOfPayloadType()
        {
            string requestUriString = "/Entities/" + VeryDerivedTypeName + "/" + EntitySetBaseTypeName;
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                request.HttpMethod = "POST";
                request.RequestContentType = "application/json";
                request.SetRequestStreamAsText(@"{""@odata.type"":""" + DerivedTypeName + @""",""ID"": ""Inserted""}");
                TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(201, request.ResponseStatusCode);
                Assert.AreEqual("http://host/Entities('Inserted')/AstoriaUnitTests.Tests.MultipleTypeSegmentsIntegrationTests_DerivedType", request.ResponseLocation);
            }
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void UpdateWithUpcast()
        {
            string requestUriString = "/Entities/" + MostBaseTypeName + "('Derived')/" + DerivedTypeName + "/" + EntitySetBaseTypeName;
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                request.HttpMethod = "PATCH";
                request.RequestContentType = "application/json";
                request.SetRequestStreamAsText(@"{""@odata.type"":""" + DerivedTypeName + @"""}");
                TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(204, request.ResponseStatusCode);
            }
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void UpdateWithDowncast()
        {
            string requestUriString = "/Entities('Derived')/" + EntitySetBaseTypeName + "/" + DerivedTypeName;
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                request.HttpMethod = "PATCH";
                request.RequestContentType = "application/json";
                request.SetRequestStreamAsText(@"{""@odata.type"":""" + DerivedTypeName + @"""}");
                TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(204, request.ResponseStatusCode);
            }
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void DeleteWithUpcast()
        {
            string requestUriString = "/Entities/" + MostBaseTypeName + "('Derived')/" + DerivedTypeName + "/" + EntitySetBaseTypeName;
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                request.HttpMethod = "DELETE";
                TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(204, request.ResponseStatusCode);
            }
        }
        [TestCategory("Partition1")]
        [TestMethod]
        public void DeleteWithDowncast()
        {
            string requestUriString = "/Entities/" + MostBaseTypeName + "('Derived')/" + EntitySetBaseTypeName + "/" + DerivedTypeName;
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                request.HttpMethod = "DELETE";
                TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(204, request.ResponseStatusCode);
            }
        }

        private static void ResponseShouldContainEntities(string requestUriString, params string[] expectedEditLinks)
        {
            RunGetRequest(
                requestUriString, 
                request =>
                {
                    Assert.AreEqual(200, request.ResponseStatusCode);
                    var responsePayload = request.GetResponseStreamAsXDocument();

                    const string xpath = "//atom:entry/atom:link[@rel='edit' and @href=\"{0}\"]";
                    var xpaths = expectedEditLinks.Select(e => string.Format(xpath, e)).Concat(new[] {"count(//atom:entry)=" + expectedEditLinks.Length}).ToArray();
                    UnitTestsUtil.VerifyXPaths(responsePayload, xpaths);
                });
        }

        private static void RunGetRequest(string requestUriString, Action<TestWebRequest> verify = null)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.Accept = "application/atom+xml,application/xml";
                request.DataServiceType = typeof(SimpleInheritanceService);

                request.RequestUriString = requestUriString;
                TestUtil.RunCatching(request.SendRequest);

                if (verify != null)
                {
                    verify(request);
                }
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class SimpleInheritanceService : DataService<SimpleInheritanceContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }
        }

        public class SimpleInheritanceContext : CustomDataContext
        {
            public IQueryable<EntitySetBaseType> Entities
            {
                get
                {
                    return new List<EntitySetBaseType> 
                    { 
                        new EntitySetBaseType { ID = "EntitySetBase" },
                        new DerivedType { ID = "Derived" },
                        new VeryDerivedType { ID = "VeryDerived" },
                        new UnrelatedVeryDerivedType { ID = "Unrelated" },
                    }.AsQueryable();
                }
            }

            public override object CreateResource(string containerName, string fullTypeName)
            {
                if (containerName == "Entities")
                {
                   if (fullTypeName == DerivedTypeName)
                   {
                       var instance = new DerivedType();
                       return this.CreateToken(instance);
                   }
                }

                return base.CreateResource(containerName, fullTypeName);
            }

            public override void DeleteResource(object token)
            {
            }
        }

        public class MostBaseType
        {
            public string ID { get; set; }
        }

        public class EntitySetBaseType : MostBaseType
        {
            public string EntitySetBaseTypeProperty { get; set; }
        }

        public class DerivedType : EntitySetBaseType
        {
            public string DerivedTypeProperty { get; set; }
            public VeryDerivedType Reference { get { return new VeryDerivedType {ID = "FromNavigation"}; } }
        }

        public class VeryDerivedType : DerivedType
        {
            public string VeryDerivedTypeProperty { get; set; }
        }

        public class UnrelatedVeryDerivedType : DerivedType
        {
        }
    }
}
