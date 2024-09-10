//---------------------------------------------------------------------
// <copyright file="SequenceMethodsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests to check sequence methods support.
    /// </summary>
    public class SequenceMethodsTests
    {
        private const string MockCustomer1 = "{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\"}";

        private const string MockCustomer2 = "{\"CustomerID\":\"CHOPS\",\"CompanyName\":\"Chop-suey Chinese\",\"ContactName\":\"Yang Wang\",\"Address\":\"Hauptstr. 29\",\"City\":\"Bern\"}";

        private readonly DataServiceQuery<Customer> _customers;

        private readonly DataServiceContext _ctx;

        private readonly string _rootUriStr;

        private Action<string> _onRequestUriBuilt = null;

        public SequenceMethodsTests()
        {
            _rootUriStr = "https://mock.odata.service";
            Uri uri = new Uri(_rootUriStr);

            _ctx = new DataServiceContext(uri);
            _customers = _ctx.CreateQuery<Customer>("Customers");

            EdmModel model = BuildEdmModel();
            _ctx.Format.UseJson(model);
            _ctx.ResolveName = (type) => $"NS.{type.Name}";
            _ctx.KeyComparisonGeneratesFilterQuery = true;

            _ctx.BuildingRequest += (obj, args) =>
            {
                if (_onRequestUriBuilt == null) return;
                string actualUri = args.RequestUri.OriginalString;
                _onRequestUriBuilt(actualUri);
            };
        }

        [Fact]
        public void Any()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("91");
            Assert.True(_customers.Any());
        }

        [Fact]
        public void Any_ReturnsFalse_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count?$filter=contains(ContactName,'thisdoesntexist')");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("0");
            Assert.False(_customers.Where(c => c.Name.Contains("thisdoesntexist")).Any());
        }

        [Fact]
        public void AnyPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count?$filter=contains(ContactName,'ab')");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("6");
            Assert.True(_customers.Any(c => c.Name.Contains("ab")));
        }

        [Fact]
        public void AnyPredicate_ReturnsFalse_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count?$filter=contains(ContactName,'thisdoesntexist')");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("0");
            Assert.False(_customers.Any(c => c.Name.Contains("thisdoesntexist")));
        }

        [Fact]
        public void CountPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count?$filter=contains(ContactName,'ab')");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("6");
            int count = _customers.Count(c => c.Name.Contains("ab"));
            Assert.Equal(6, count);
        }

        [Fact]
        public void LongCountPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers/$count?$filter=contains(ContactName,'ab')");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponse("6");
            long count = _customers.LongCount(c => c.Name.Contains("ab"));
            Assert.Equal(6, count);
        }

        [Fact]
        public void FirstPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName ne 'John'&$top=1");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer1 + "]");
            Customer customer = _customers.First(c => c.Name != "John");
            Assert.Equal("ALFKI", customer.Id);
            Assert.Equal("Maria Anders", customer.Name);
            Assert.Equal("Berlin", customer.City);
        }

        [Fact]
        public void FirstPredicate_ThrowsException_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName eq 'thisdoesntexist'&$top=1");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Throws<InvalidOperationException>(() => _customers.First(c => c.Name == "thisdoesntexist"));
        }

        [Fact]
        public void FirstOrDefaultPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName ne 'John'&$top=1");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer1 + "]");
            Customer customer = _customers.FirstOrDefault(c => c.Name != "John");
            Assert.Equal("ALFKI", customer.Id);
            Assert.Equal("Maria Anders", customer.Name);
            Assert.Equal("Berlin", customer.City);
        }

        [Fact]
        public void FirstOrDefaultPredicate_ReturnsNull_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName eq 'John'&$top=1");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Null(_customers.FirstOrDefault(c => c.Name == "John"));
        }

        [Fact]
        public void SinglePredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=CustomerID eq 'CHOPS'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer2 + "]");
            Customer customer = _customers.Single(c => c.Id == "CHOPS");
            Assert.Equal("CHOPS", customer.Id);
            Assert.Equal("Yang Wang", customer.Name);
            Assert.Equal("Bern", customer.City);
        }

        [Fact]
        public void SinglePredicate_ThrowsException_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName eq 'thisdoesntexist'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Throws<InvalidOperationException>(() => _customers.Single(c => c.Name == "thisdoesntexist"));
        }

        [Fact]
        public void SinglePredicate_ThrowsException_WhenMoreThanOneMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName ne 'thisdoesntexist'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer1 + "," + MockCustomer2 + "]");
            Assert.Throws<InvalidOperationException>(() => _customers.Single(c => c.Name != "thisdoesntexist"));
        }

        [Fact]
        public void SingleOrDefaultPredicate()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=CustomerID eq 'CHOPS'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer2 + "]");
            Customer customer = _customers.SingleOrDefault(c => c.Id == "CHOPS");
            Assert.Equal("CHOPS", customer.Id);
            Assert.Equal("Yang Wang", customer.Name);
            Assert.Equal("Bern", customer.City);
        }

        [Fact]
        public void SingleOrDefaultPredicate_ReturnsNull_WhenNoMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=CustomerID eq '234111'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Null(_customers.SingleOrDefault(c => c.Id == "234111"));
        }

        [Fact]
        public void SingleOrDefaultPredicate_ThrowsException_WhenMoreThanOneMatchExists()
        {
            _onRequestUriBuilt = (string builtUri) =>
            {
                string expectedUri = BuildUriFromPath("/Customers?$filter=ContactName ne 'thisdoesntexist'&$top=2");
                Assert.Equal(expectedUri, builtUri);
            };
            InterceptRequestAndMockResponseValue("Customers", "[" + MockCustomer1 + "," + MockCustomer2 + "]");
            Assert.Throws<InvalidOperationException>(() => _customers.SingleOrDefault(c => c.Name != "thisdoesntexist"));
        }

        private string BuildUriFromPath(string uriPath)
        {
            return _rootUriStr + uriPath;
        }

        private void InterceptRequestAndMockResponse(string mockResponse)
        {
            _ctx.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new TestHttpWebRequestMessage(args,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    },
                    () => new MemoryStream(Encoding.UTF8.GetBytes(mockResponse)));
            };
        }

        private void InterceptRequestAndMockResponseValue(string entitySetName, string mockResponseValue)
        {
            string mockResponse = "{\"@odata.context\":\"" + _rootUriStr + "/$metadata#" + entitySetName + "\",\"value\":" + mockResponseValue + "}";

            InterceptRequestAndMockResponse(mockResponse);
        }

        private static EdmModel BuildEdmModel()
        {
            var model = new EdmModel();

            // Create the Customer entity type
            var customerType = new EdmEntityType("NS", "Customer");
            var customerId = customerType.AddStructuralProperty("CustomerID", EdmPrimitiveTypeKind.String, false);
            customerType.AddKeys(customerId);
            customerType.AddStructuralProperty("CompanyName", EdmPrimitiveTypeKind.String, false);
            customerType.AddStructuralProperty("ContactName", EdmPrimitiveTypeKind.String, true);
            customerType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, true);
            model.AddElement(customerType);

            // Create the EntityContainer
            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            // Create Entity Sets
            container.AddEntitySet("Customers", customerType);

            return model;
        }

        [Key("CustomerID")]
        public class Customer
        {
            [OriginalName("CustomerID")]
            public string Id { get; set; }

            public string City { get; set; }

            [OriginalName("ContactName")]
            public string Name { get; set; }
        }
    }
}
