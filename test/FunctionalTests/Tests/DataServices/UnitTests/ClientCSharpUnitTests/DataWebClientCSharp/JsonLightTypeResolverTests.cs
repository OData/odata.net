//---------------------------------------------------------------------
// <copyright file="JsonLightTypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AstoriaUnitTests.ClientExtensions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Tests;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    [TestClass]
    public class JsonLightTypeResolverTests
    {
        private readonly EdmModel serverModel;

        public JsonLightTypeResolverTests()
        {
            this.serverModel = new EdmModel();
            var serverEntityType = new EdmEntityType("Server.Name.Space", "EntityType");
            serverEntityType.AddKeys(serverEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var parentType = new EdmEntityType("Server.Name.Space", "Parent");
            this.serverModel.AddElement(serverEntityType);
            this.serverModel.AddElement(parentType);

            var serverComplexType = new EdmComplexType("Server.Name.Space", "ComplexType");
            serverComplexType.AddStructuralProperty("Number", EdmPrimitiveTypeKind.Int32);
            this.serverModel.AddElement(serverComplexType);
            var entityContainer = new EdmEntityContainer("Fake", "Container");
            this.serverModel.AddElement(entityContainer);
            entityContainer.AddEntitySet("Entities", serverEntityType);
            entityContainer.AddEntitySet("Parents", parentType);

            parentType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation", Target = serverEntityType, TargetMultiplicity = EdmMultiplicity.Many });
        }

        [TestMethod]
        public void EntityDescriptorShouldHaveServerTypeName()
        {
            const string responseBody = @"{""@odata.context"":""http://fake.org/$metadata#Entities"", ""value"": [ { ""ID"": ""1"" } ] }";

            RunClientRequest(ctx =>
            {
                ctx.Format.UseJson(this.serverModel);

                var query = (DataServiceQuery<TestClientEntityType>)ctx.CreateQuery<TestClientEntityType>("Entities").Take(1);

                var results = (QueryOperationResponse<TestClientEntityType>)query.Execute();
                var entity = results.First();

                var descriptor = ctx.GetEntityDescriptor(entity);
                Assert.IsNotNull(descriptor);
                Assert.AreEqual("Server.Name.Space.EntityType", descriptor.ServerTypeName);
            }, 
            responsePayload: responseBody, 
            setupResponse: r =>
            {
                r.StatusCode = 200;
                r.SetHeader("Content-Type", "application/json");
            });
        }

        [TestMethod]
        public void ClientShouldSendTypeAnnotationsForOpenServerPropertiesForInsert()
        {
            this.TestClientOpenPropertyPayload((ctx, e) => ctx.AddObject("Entities", e));
        }

        [TestMethod]
        public void ClientShouldSendTypeAnnotationsForOpenServerPropertiesForUpdate()
        {
            this.TestClientOpenPropertyPayload((ctx, e) => 
            { 
                ctx.AttachTo("Entities", e); 
                ctx.UpdateObject(e);
            });
        }

        [TestMethod]
        public void ClientShouldBeAbleToReadCollectionOfComplexValuesWithoutTypeResolver()
        {
            // Regression coverage for: Query for collection of complex values fails on JSON client when type-name resolver is not defined.
            const string responseBody = @"{""@odata.context"":""http://fake.org/$metadata#Collection(Server.Name.Space.ComplexType)"", ""value"": [ { ""Number"": ""1"" } ] }";

            RunClientRequest(ctx =>
            {
                ctx.Format.UseJson(this.serverModel);

                var results = ctx.Execute<TestClientComplexType>(new Uri("http://something.org/"), "GET", false);
                Assert.AreEqual(1, results.First().Number);
            },
            responsePayload: responseBody,
            setupResponse: r =>
            {
                r.StatusCode = 200;
                r.SetHeader("Content-Type", "application/json");
            });
        }

        [TestMethod]
        public void ClientShouldSendTypeAnnotationsForOpenServerPropertiesForAddRelatedObject()
        {
            this.TestClientOpenPropertyPayload((ctx, e) =>
            {
                var parent = new TestClientEntityType { ID = 1234 };
                ctx.AttachTo("Parents", parent);
                ctx.AddRelatedObject(parent, "Navigation", e);
            });
        }

        private void TestClientOpenPropertyPayload(Action<DataServiceContext, TestClientEntityType> setupClient)
        {
            const string expectedRequestPayload = "{"
                                + "\"ID\":12345," // no type because it's declared
                                + "\"OpenInfinity@odata.type\":\"#Double\","
                                + "\"OpenInfinity\":\"INF\","
                                + "\"OpenNull\":null," // no type because it's null
                                + "\"OpenString\":\"foo\"}"; // no type because it's a string

            RunClientRequest(ctx =>
            {
                ctx.Format.UseJson(this.serverModel);
                setupClient(ctx, new TestClientEntityType {ID = 12345});
                ctx.SaveChanges();
            },
            expectedRequestPayload: expectedRequestPayload, 
            setupResponse: r =>
            {
                r.StatusCode = 204; 
                r.SetHeader("Location", "http://somewhere.org/"); 
            });
        }

        private static void RunClientRequest(Action<DataServiceContext> runTest, string expectedRequestPayload = null, string responsePayload = null, Action<IODataResponseMessage> setupResponse = null)
        {
            using (var requestStream = new MemoryStream())
            using (var responseStream = responsePayload == null ? new MemoryStream() : new MemoryStream(Encoding.UTF8.GetBytes(responsePayload)))
            {
                responseStream.Position = 0;
                var requestMessage = new InMemoryMessage {Stream = requestStream};
                var responseMessage = new InMemoryMessage {Stream = responseStream};
                if (setupResponse != null)
                {
                    setupResponse(responseMessage);
                }

                DataServiceContext ctx = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
                runTest(ctx);

                if (expectedRequestPayload != null)
                {
                    var actualRequestPayload = Encoding.UTF8.GetString(requestStream.ToArray());
                    Assert.AreEqual(expectedRequestPayload, actualRequestPayload);
                }
            }
        }

        private class TestClientComplexType
        {
            public int Number { get; set; }
        }

        [Key("ID")]
        private class TestClientEntityType
        {
            public long ID { get; set; }
            public double OpenInfinity
            {
                get { return double.PositiveInfinity; }
                set { }
            } 
            
            public string OpenString
            {
                get { return "foo"; }
                set { }
            }

            public Guid? OpenNull 
            { 
                get { return null; }
                set { }
            }

            public List<TestClientEntityType> Navigation { get; set; } 
        }
    }
}
