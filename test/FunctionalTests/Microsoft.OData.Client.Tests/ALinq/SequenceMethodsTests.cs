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
        private readonly TestContext _ctx;

        public SequenceMethodsTests()
        {
            _ctx = new TestContext();
        }

        [Fact]
        public void Any()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers/$count");
            _ctx.InterceptRequestAndMockResponse("91");
            Assert.True(_ctx.Customers.Any());

            _ctx.InterceptRequestAndAssertUri("/Customers/$count?$filter=contains(ContactName,'thisdoesntexist')");
            _ctx.InterceptRequestAndMockResponse("0");
            Assert.False(_ctx.Customers.Where(c => c.Name.Contains("thisdoesntexist")).Any());
        }

        [Fact]
        public void AnyPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers/$count?$filter=contains(ContactName,'ab')");
            _ctx.InterceptRequestAndMockResponse("6");
            Assert.True(_ctx.Customers.Any(c => c.Name.Contains("ab")));

            _ctx.InterceptRequestAndAssertUri("/Customers/$count?$filter=contains(ContactName,'thisdoesntexist')");
            _ctx.InterceptRequestAndMockResponse("0");
            Assert.False(_ctx.Customers.Any(c => c.Name.Contains("thisdoesntexist")));
        }

        [Fact]
        public void CountPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers/$count?$filter=contains(ContactName,'ab')");
            _ctx.InterceptRequestAndMockResponse("6");
            int count = _ctx.Customers.Count(c => c.Name.Contains("ab"));
            Assert.Equal(6, count);
        }

        [Fact]
        public void LongCountPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers/$count?$filter=contains(ContactName,'ab')");
            _ctx.InterceptRequestAndMockResponse("6");
            long count = _ctx.Customers.LongCount(c => c.Name.Contains("ab"));
            Assert.Equal(6, count);
        }

        [Fact]
        public void FirstPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName ne 'John'&$top=1");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer1 + "]");
            Customer customer = _ctx.Customers.First(c => c.Name != "John");
            Assert.Equal("ALFKI", customer.Id);
            Assert.Equal("Maria Anders", customer.Name);
            Assert.Equal("Berlin", customer.City);
        }

        [Fact]
        public void FirstPredicate_ThrowsException_WhenNoMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName eq 'thisdoesntexist'&$top=1");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Throws<InvalidOperationException>(() => _ctx.Customers.First(c => c.Name == "thisdoesntexist"));
        }

        [Fact]
        public void FirstOrDefaultPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName ne 'John'&$top=1");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer1 + "]");
            Customer customer = _ctx.Customers.FirstOrDefault(c => c.Name != "John");
            Assert.Equal("ALFKI", customer.Id);
            Assert.Equal("Maria Anders", customer.Name);
            Assert.Equal("Berlin", customer.City);
        }

        [Fact]
        public void FirstOrDefaultPredicate_ReturnsNull_WhenNoMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName eq 'John'&$top=1");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Null(_ctx.Customers.FirstOrDefault(c => c.Name == "John"));
        }

        [Fact]
        public void SinglePredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=CustomerID eq 'CHOPS'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer2 + "]");
            Customer customer = _ctx.Customers.Single(c => c.Id == "CHOPS");
            Assert.Equal("CHOPS", customer.Id);
            Assert.Equal("Yang Wang", customer.Name);
            Assert.Equal("Bern", customer.City);
        }

        [Fact]
        public void SinglePredicate_ThrowsException_WhenNoMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName eq 'thisdoesntexist'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Throws<InvalidOperationException>(() => _ctx.Customers.Single(c => c.Name == "thisdoesntexist"));
        }

        [Fact]
        public void SinglePredicate_ThrowsException_WhenMoreThanOneMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName ne 'thisdoesntexist'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer1 + "," + TestContext.MockCustomer2 + "]");
            Assert.Throws<InvalidOperationException>(() => _ctx.Customers.Single(c => c.Name != "thisdoesntexist"));
        }

        [Fact]
        public void SingleOrDefaultPredicate()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=CustomerID eq 'CHOPS'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer2 + "]");
            Customer customer = _ctx.Customers.SingleOrDefault(c => c.Id == "CHOPS");
            Assert.Equal("CHOPS", customer.Id);
            Assert.Equal("Yang Wang", customer.Name);
            Assert.Equal("Bern", customer.City);
        }

        [Fact]
        public void SingleOrDefaultPredicate_ReturnsNull_WhenNoMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=CustomerID eq '234111'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[]");
            Assert.Null(_ctx.Customers.SingleOrDefault(c => c.Id == "234111"));
        }

        [Fact]
        public void SingleOrDefaultPredicate_ThrowsException_WhenMoreThanOneMatchExists()
        {
            _ctx.InterceptRequestAndAssertUri("/Customers?$filter=ContactName ne 'thisdoesntexist'&$top=2");
            _ctx.InterceptRequestAndMockResponseValue("Customers", "[" + TestContext.MockCustomer1 + "," + TestContext.MockCustomer2 + "]");
            Assert.Throws<InvalidOperationException>(() => _ctx.Customers.SingleOrDefault(c => c.Name != "thisdoesntexist"));
        }
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

    public class TestContext
    {
        public const string MockCustomer1 = "{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\"}";

        public const string MockCustomer2 = "{\"CustomerID\":\"CHOPS\",\"CompanyName\":\"Chop-suey Chinese\",\"ContactName\":\"Yang Wang\",\"Address\":\"Hauptstr. 29\",\"City\":\"Bern\"}";

        public readonly DataServiceQuery<Customer> Customers;

        private readonly DataServiceContext _ctx;

        private readonly string _rootUriStr;

        private string _assertRequestUriPath = "";

        public TestContext()
        {
            _rootUriStr = "https://mock.odata.service";
            Uri uri = new Uri(_rootUriStr);

            _ctx = new DataServiceContext(uri);
            Customers = _ctx.CreateQuery<Customer>("Customers");

            EdmModel model = BuildEdmModel();
            _ctx.Format.UseJson(model);
            _ctx.ResolveName = (type) => $"NS.{type.Name}";
            _ctx.KeyComparisonGeneratesFilterQuery = true;

            _ctx.BuildingRequest += (obj, args) =>
            {
                if (_assertRequestUriPath == "") return;
                string expectedUri = _rootUriStr + _assertRequestUriPath;
                string actualUri = args.RequestUri.ToString();
                Assert.Equal(expectedUri, actualUri);
                _assertRequestUriPath = "";
            };
        }

        public void InterceptRequestAndAssertUri(string uriPath)
        {
            _assertRequestUriPath = uriPath;
        }

        public void InterceptRequestAndMockResponse(string mockResponse)
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

        public void InterceptRequestAndMockResponseValue(string entitySetName, string mockResponseValue)
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
    }
}
