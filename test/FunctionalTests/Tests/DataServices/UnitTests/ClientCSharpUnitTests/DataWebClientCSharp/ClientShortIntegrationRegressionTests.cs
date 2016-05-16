//---------------------------------------------------------------------
// <copyright file="ClientShortIntegrationRegressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AstoriaUnitTests.ClientExtensions;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientShortIntegrationUtils
    {
        private static IEdmModel Model;
        private static Func<string, Type> ResolveType;
        private static Func<Type, string> ResolveName;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Model = SimpleNorthwind.BuildSimplifiedNorthwindModel();
            ResolveType = (string name) =>
            {
                string updatedName = name;
                updatedName = updatedName.Replace("ODataDemo.", "AstoriaUnitTests.SimpleNorthwind+");
                var foundType = typeof(SimpleNorthwind).GetNestedTypes().Single(t => t.FullName == updatedName);
                return foundType;
            };

            ResolveName = (Type t) =>
            {
                return t.FullName.Replace("AstoriaUnitTests.SimpleNorthwind+", "ODataDemo.");
            };
        }

        private const string JsonLightExtraPropertyPayload = @"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Suppliers"",
  ""value"":
  [
    {
      ""ID"":1,
      ""Name"":""MyName"",
      ""ExtraProperty"":""foo"",
      ""Concurrency"":1
    }
  ]
}";

        [TestMethod]
        public void ServiceOperationCollectionComplex()
        {
            // Get Collection of complex
            var context = CreateTransportLayerContext(@"{
   ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Collection(ODataDemo.Address)"",
   ""value"":[
      {
         ""Street"":""includemanualshrlunisolar"",
         ""City"":""includemanualshrlunisolar"",
         ""State"":""WA"",
         ""ZipCode"":""dynamicbgeescapecombineserialdirtyreorderhandlespersist""
      },
      {
         ""Street"":""includemalar"",
         ""City"":""indemanunisolar"",
         ""State"":""CT"",
         ""ZipCode"":""mbineserialdirtyreorderhandlespersist""
      }
   ]
}", "4.0");
            var addresses = context.Execute<SimpleNorthwind.Address>(new Uri("/GetAddresses", UriKind.Relative),"GET", false).ToList();
            Assert.IsNotNull(addresses);
            addresses.Should().HaveCount(2);
            addresses[0].State.Should().Be("WA");
            addresses[1].State.Should().Be("CT");
        }

        [TestMethod]
        public void ServiceOperationCollectionPrimitive()
        {
            // Get collection of primitive
            var context = CreateTransportLayerContext(@"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Collection(Edm.Int32)"",
  ""value"":[
    1,
    2
  ]
}", "4.0");
            context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            var numbers = context.Execute<int>(new Uri("/GetAddresses", UriKind.Relative), "GET", false).ToList();
            Assert.IsNotNull(numbers);
            numbers.Should().HaveCount(2);
            numbers[0].Should().Be(1);
            numbers[1].Should().Be(2);
        }

        [TestMethod]
        public void IgnoreMissingPropertiesTestJsonLight()
        {
            // Ignore missing properties
            var context = CreateTransportLayerContext(JsonLightExtraPropertyPayload, "4.0");
            var supplier = context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(supplier);
            supplier.ID.Should().Be(1);
            supplier.Name.Should().Be("MyName");
            supplier.Concurrency.Should().Be(1);
        }

        [TestMethod]
        public void IgnoreMissingPropertiesTestJsonLightShouldThrow()
        {
            // Ignore missing properties
            var context = CreateTransportLayerContext(JsonLightExtraPropertyPayload, "4.0");
            context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException;
            Action test = ()=> context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            test.ShouldThrow<InvalidOperationException>().WithMessage("The property 'ExtraProperty' does not exist on type 'ODataDemo.Supplier'. Make sure to only use property names that are defined by the type.");
        }

        private const string JsonLightUnknownNavigationLink = @"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Suppliers"",
  ""value"":
  [
    {
      ""ID"":1,
      ""Name"":""MyName"",
      ""UnknownNavigation@odata.navigationLink"":""Suppliers(1)/UnknownNavigation"",
      ""UnknownNavigation@odata.associationLink"":""Suppliers(1)/UnknownNavigation/$ref"",
      ""Concurrency"":1
    }
  ]
}";

        [TestMethod]
        public void TestUnknownNavigationLink()
        {
            var context = CreateTransportLayerContext(JsonLightUnknownNavigationLink, "4.0");
            var supplier = context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(supplier);
            supplier.ID.Should().Be(1);
        }

        private const string JsonLightUnknownNavigationProperty = @"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Suppliers"",
  ""value"":
  [
    {
      ""ID"":1,
      ""Name"":""MyName"",
      ""Concurrency"":1,
      ""UnknownNavigation"": [
        {
            ""ID"": 0,
            ""Name"": ""UnknownNavigation""
        }
      ]
    }
  ]
}";
        [TestMethod]
        public void TestUnknownNavigationProperty()
        {
            var context = CreateTransportLayerContext(JsonLightUnknownNavigationProperty, "4.0");
            var supplier = context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(supplier);
            supplier.ID.Should().Be(1);
        }

        private const string JsonLightWithMetadataEtag = @"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Suppliers"",
  ""@odata.metadataEtag"": ""W/\""A1FF3E230954908F\"""",
  ""value"":
  [
    {
      ""ID"":1,
      ""Name"":""MyName"",
      ""Concurrency"":1
    }
  ]
}";
        [TestMethod]
        public void TestMetadataEtag()
        {
            var context = CreateTransportLayerContext(JsonLightWithMetadataEtag, "4.0");
            var supplier = context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(supplier);
            supplier.ID.Should().Be(1);
        }

        private const string JsonLightUnknownODataAnnotation = @"{
  ""@odata.context"":""http://localhost:9000/Service.svc/$metadata#Suppliers"",
  ""@odata.unknown1"": ""abc"",
  ""value"":
  [
    {
      ""@odata.unknown2"": ""abc"",
      ""ID"":1,
      ""Name"":""MyName"",
      ""Concurrency"":1,
      ""@odata.unknown3"": ""abc""
    }
  ]
}";
        [TestMethod]
        public void TestUnknownODataAnnotation()
        {
            var context = CreateTransportLayerContext(JsonLightUnknownODataAnnotation, "4.0");
            var supplier = context.Execute<SimpleNorthwind.Supplier>(new Uri("/Suppliers", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(supplier);
            supplier.ID.Should().Be(1);
        }

        [TestMethod]
        public void NotModifiedTest()
        {
            var odataRequestMessage = new ODataTestMessage();
            var odataResponseMessage = new ODataTestMessage()
            {
                StatusCode = 304,
                MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(@""))
            };
            odataResponseMessage.SetHeader("Content-Type", "0");

            DataServiceContextWithCustomTransportLayer context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, () => odataRequestMessage, () => odataResponseMessage);

            Action test = () => context.CreateQuery<SimpleNorthwind.Product>("Products").ToList();
            test.ShouldThrow<DataServiceQueryException>().WithInnerMessage("NotModified");
        }

        private DataServiceContextWithCustomTransportLayer CreateTransportLayerContext(string payload, string odataVersion)
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.SetHeader("Content-Type", "application/json");
            responseMessage.SetHeader("OData-Version", odataVersion);
            responseMessage.StatusCode = 200;
            responseMessage.WriteToStream(payload);
            responseMessage.SetHeader("Content-Length", responseMessage.MemoryStream.Length.ToString());

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            context.ResolveName = ResolveName;
            context.ResolveType = ResolveType;
            context.Format.UseJson(Model);
            return context;
        }
    }
}