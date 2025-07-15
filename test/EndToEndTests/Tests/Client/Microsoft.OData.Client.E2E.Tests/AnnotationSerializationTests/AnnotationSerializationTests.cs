//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Net;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Default;
using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models;
using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests
{
    public class AnnotationSerializationTests : EndToEndTestBase<AnnotationSerializationTests.TestsStartup>
    {
        private readonly Uri baseUri;
        private readonly Container context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(OrdersController), typeof(MetadataController));
                services.AddControllers().AddOData(
                    options => options.EnableQueryFeatures().AddRouteComponents(
                        AnnotationSerializationEdmModel.GetEdmModel()));
            }
        }

        public AnnotationSerializationTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            if (Client.BaseAddress == null)
            {
                throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
            }

            baseUri = Client.BaseAddress;
            context = new Container(baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };
        }

        [Fact]
        public void SerializeEntity_ShouldIncludeOnlyRequiredODataAnnotations()
        {
            // Arrange
            var order = InitializeOrderEntity(orderId: 3);

            var httpInterceptingHandler = new TestHttpInterceptingHandler(
                HttpStatusCode.Created,
                new Uri(baseUri, "Orders(3)"),
                $"{{\"@odata.context\": \"{baseUri}/$metadata#Orders/$entity\",\"Id\":3}}");
            var httpClient = new HttpClient(httpInterceptingHandler)
            {
                BaseAddress = baseUri
            };

            var dataServiceContext = new Container(httpClient.BaseAddress, ODataProtocolVersion.V4);
            dataServiceContext.HttpClientFactory = new TestHttpClientFactory(httpClient);

            dataServiceContext.AddToOrders(order);

            // Act
            var dataServiceResponse = dataServiceContext.SaveChanges();

            // Assert
            var interceptedBody = httpInterceptingHandler.InterceptedBody;
            Assert.Equal(
                "{\"Amount\":130.0," +
                "\"Id\":3," +
                "\"NextStatus@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus\"," +
                "\"NextStatus\":\"Processing\"," +
                "\"ProhibitedStatuses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus)\"," +
                "\"ProhibitedStatuses\":[\"Returned\"]," +
                "\"Status\":\"Pending\"," +
                "\"StatusHistory\":[\"Pending\"]," +
                "\"Tags\":[\"Urgent\"]," +
                "\"TagsHistory@odata.type\":\"#Collection(String)\"," +
                "\"TagsHistory\":[\"Express\"]," +
                "\"PickupAddress\":{" +
                "\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.Address\"," +
                "\"Street\":\"789 Pickup Rd\"," +
                "\"City\":{" +
                "\"Name\":\"Houston\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Texas\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"New Mexico\"}}," +
                "\"ReturnAddresses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.Address)\"," +
                "\"ReturnAddresses\":[{" +
                "\"Street\":\"321 Return St\"," +
                "\"City\":{" +
                "\"Name\":\"Miami\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Florida\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Georgia\"}}]," +
                "\"ShippingAddress\":{" +
                "\"Street\":\"123 Main St\"," +
                "\"City\":{" +
                "\"Name\":\"Seattle\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Washington\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Oregon\"}}," +
                "\"WarehouseAddresses\":[{" +
                "\"Street\":\"456 Warehouse St\"," +
                "\"City\":{" +
                "\"Name\":\"Las Vegas\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Nevada\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"California\"}}]}",
                interceptedBody);
        }

        [Fact]
        public void SerializeDerivedEntity_ShouldIncludeOnlyRequiredODataAnnotations()
        {
            // Arrange
            var vipOrder = InitializeVipOrderEntity(orderId: 4);

            var httpInterceptingHandler = new TestHttpInterceptingHandler(
                HttpStatusCode.Created,
                new Uri(baseUri, "Orders(4)"),
                $"{{\"@odata.context\": \"{baseUri}/$metadata#Orders/Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipOrder/$entity\",\"Id\":4}}");
            var httpClient = new HttpClient(httpInterceptingHandler)
            {
                BaseAddress = baseUri
            };

            var dataServiceContext = new Container(httpClient.BaseAddress, ODataProtocolVersion.V4);
            dataServiceContext.HttpClientFactory = new TestHttpClientFactory(httpClient);

            dataServiceContext.AddToOrders(vipOrder);

            // Act
            var dataServiceResponse = dataServiceContext.SaveChanges();

            // Assert
            var interceptedBody = httpInterceptingHandler.InterceptedBody;
            Assert.Equal(
                "{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipOrder\"," +
                "\"Amount\":900.0," +
                "\"Id\":4," +
                "\"NextStatus@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus\"," +
                "\"NextStatus\":\"Pending\"," +
                "\"ProhibitedStatuses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus)\"," +
                "\"ProhibitedStatuses\":[]," +
                "\"Status\":\"Processing\"," +
                "\"StatusHistory\":[\"Pending\",\"Processing\"]," +
                "\"Tags\":[\"VIP\",\"Priority\"]," +
                "\"TagsHistory@odata.type\":\"#Collection(String)\"," +
                "\"TagsHistory\":[]," +
                "\"TrackingNumber\":87543," +
                "\"PickupAddress\":null," +
                "\"ReturnAddresses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.Address)\"," +
                "\"ReturnAddresses\":[]," +
                "\"ShippingAddress\":{" +
                "\"Street\":\"456 VIP St\"," +
                "\"City\":{" +
                "\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipCity\"," +
                "\"AreaCode\":\"10001\"," +
                "\"Name\":\"New York\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipState\",\"Name\":\"New York\",\"TwoLetterCode\":\"NY\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipState\",\"Name\":\"Massachusetts\",\"TwoLetterCode\":\"MA\"}}," +
                "\"WarehouseAddresses\":[{" +
                "\"Street\":\"789 VIP Warehouse St\"," +
                "\"City\":{" +
                "\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipCity\"," +
                "\"AreaCode\":\"90001\"," +
                "\"Name\":\"Los Angeles\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipState\",\"Name\":\"California\",\"TwoLetterCode\":\"CA\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipState\",\"Name\":\"Arizona\",\"TwoLetterCode\":\"AZ\"}}]}",
                interceptedBody);
        }

        [Fact]
        public void SerializeEntityWithDerivedComplexInCollection_ShouldIncludeOnlyRequiredAnnotations()
        {
            var order = InitializedOrderEntityWithDerivedComplexInCollection(orderId: 5);

            var httpInterceptingHandler = new TestHttpInterceptingHandler(
                HttpStatusCode.Created,
                new Uri(baseUri, "Orders(5)"),
                $"{{\"@odata.context\": \"{baseUri}/$metadata#Orders/$entity\",\"Id\":5}}");
            var httpClient = new HttpClient(httpInterceptingHandler)
            {
                BaseAddress = baseUri
            };

            var dataServiceContext = new Container(httpClient.BaseAddress, ODataProtocolVersion.V4);
            dataServiceContext.HttpClientFactory = new TestHttpClientFactory(httpClient);

            dataServiceContext.AddToOrders(order);

            // Act
            var dataServiceResponse = dataServiceContext.SaveChanges();

            // Assert
            var interceptedBody = httpInterceptingHandler.InterceptedBody;
            Assert.Equal(
                "{\"Amount\":170.0," +
                "\"Id\":5," +
                "\"NextStatus@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus\"," +
                "\"NextStatus\":\"Delivered\"," +
                "\"ProhibitedStatuses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.OrderStatus)\"," +
                "\"ProhibitedStatuses\":[]," +
                "\"Status\":\"Shipped\"," +
                "\"StatusHistory\":[]," +
                "\"Tags\":[]," +
                "\"TagsHistory@odata.type\":\"#Collection(String)\"," +
                "\"TagsHistory\":[]," +
                "\"PickupAddress\":null," +
                "\"ReturnAddresses@odata.type\":\"#Collection(Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.Address)\"," +
                "\"ReturnAddresses\":[]," +
                "\"ShippingAddress\":{" +
                "\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipAddress\"," +
                "\"PostalCode\":\"98101\"," +
                "\"Street\":\"123 Main St\"," +
                "\"City\":{" +
                "\"Name\":\"Seattle\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Washington\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Oregon\"}}," +
                "\"WarehouseAddresses\":[{" +
                "\"Street\":\"456 Warehouse St\"," +
                "\"City\":{" +
                "\"Name\":\"Las Vegas\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Florida\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Georgia\"}}," +
                "{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.VipAddress\"," +
                "\"PostalCode\":\"99501\"," +
                "\"Street\":\"789 VIP Warehouse St\"," +
                "\"City\":{" +
                "\"Name\":\"Anchorage\"," +
                "\"State\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Alaska\"}}," +
                "\"NeighborState\":{\"@odata.type\":\"#Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.State\",\"Name\":\"Hawaii\"}}]}",
                interceptedBody);
        }

        [Fact]
        public void PostEntity_ShouldWorkWithOnlyRequiredODataAnnotations()
        {
            var order = InitializeOrderEntity(orderId: 3);

            context.AddToOrders(order);

            var dataServiceResponse = context.SaveChanges();

            var changeOperationResponse = Assert.IsType<ChangeOperationResponse>(Assert.Single(dataServiceResponse));
            var location = changeOperationResponse.HeaderCollection.GetHeader("Location");
            Assert.NotNull(location);
            Assert.Equal(new Uri(baseUri, "Orders(3)").OriginalString, location);
            Assert.Equal(201, changeOperationResponse.StatusCode);
        }

        [Fact]
        public void PostDerivedEntity_ShouldWorkWithOnlyRequiredODataAnnotations()
        {
            var order = InitializeVipOrderEntity(orderId: 4);

            context.AddToOrders(order);

            var dataServiceResponse = context.SaveChanges();

            var changeOperationResponse = Assert.IsType<ChangeOperationResponse>(Assert.Single(dataServiceResponse));
            var location = changeOperationResponse.HeaderCollection.GetHeader("Location");
            Assert.NotNull(location);
            Assert.Equal(new Uri(baseUri, "Orders(4)").OriginalString, location);
            Assert.Equal(201, changeOperationResponse.StatusCode);
        }

        [Fact]
        public void PostEntityWithDerivedComplexInCollection_ShouldWorkWithOnlyRequiredODataAnnotations()
        {
            var order = InitializeOrderEntity(orderId: 5);

            context.AddToOrders(order);

            var dataServiceResponse = context.SaveChanges();

            var changeOperationResponse = Assert.IsType<ChangeOperationResponse>(Assert.Single(dataServiceResponse));
            var location = changeOperationResponse.HeaderCollection.GetHeader("Location");
            Assert.NotNull(location);
            Assert.Equal(new Uri(baseUri, "Orders(5)").OriginalString, location);
            Assert.Equal(201, changeOperationResponse.StatusCode);
        }

        private Order InitializeOrderEntity(int orderId)
        {
            return new Order
            {
                Id = orderId,
                Tags = new ObservableCollection<string> { "Urgent" },
                Status = OrderStatus.Pending,
                StatusHistory = new ObservableCollection<OrderStatus>
                {
                    OrderStatus.Pending
                },
                Amount = 130.0m,
                NextStatus = OrderStatus.Processing,
                ProhibitedStatuses = new ObservableCollection<OrderStatus>
                {
                    OrderStatus.Returned
                },
                TagsHistory = new ObservableCollection<string>
                {
                    "Express"
                },
                ShippingAddress = new Address
                {
                    Street = "123 Main St",
                    City = new City
                    {
                        Name = "Seattle",
                        State = new State { Name = "Washington" }
                    },
                    NeighborState = new State { Name = "Oregon" }
                },
                WarehouseAddresses = new ObservableCollection<Address>
                {
                    new Address
                    {
                        Street = "456 Warehouse St",
                        City = new City
                        {
                            Name = "Las Vegas",
                            State = new State { Name = "Nevada" }
                        },
                        NeighborState = new State { Name = "California" }
                    }
                },
                PickupAddress = new Address
                {
                    Street = "789 Pickup Rd",
                    City = new City
                    {
                        Name = "Houston",
                        State = new State { Name = "Texas" }
                    },
                    NeighborState = new State { Name = "New Mexico" }
                },
                ReturnAddresses = new ObservableCollection<Address>
                {
                    new Address
                    {
                        Street = "321 Return St",
                        City = new City
                        {
                            Name = "Miami",
                            State = new State { Name = "Florida" }
                        },
                        NeighborState = new State { Name = "Georgia" }
                    },
                },
            };
        }

        private VipOrder InitializeVipOrderEntity(int orderId)
        {
            return new VipOrder
            {
                Id = orderId,
                Status = OrderStatus.Processing,
                StatusHistory = new ObservableCollection<OrderStatus> { OrderStatus.Pending, OrderStatus.Processing },
                Tags = new ObservableCollection<string> { "VIP", "Priority" },
                Amount = 900.0m,
                ShippingAddress = new Address
                {
                    Street = "456 VIP St",
                    City = new VipCity
                    {
                        Name = "New York",
                        AreaCode = "10001",
                        State = new VipState { Name = "New York", TwoLetterCode = "NY" }
                    },
                    NeighborState = new VipState { Name = "Massachusetts", TwoLetterCode = "MA" }
                },
                WarehouseAddresses = new ObservableCollection<Address>
                {
                    new Address
                    {
                        Street = "789 VIP Warehouse St",
                        City = new VipCity
                        {
                            Name = "Los Angeles",
                            AreaCode = "90001",
                            State = new VipState { Name = "California", TwoLetterCode = "CA" }
                        },
                        NeighborState = new VipState { Name = "Arizona", TwoLetterCode = "AZ" }
                    },
                },
                TrackingNumber = 87543,
            };
        }

        private Order InitializedOrderEntityWithDerivedComplexInCollection(int orderId)
        {
            return new Order
            {
                Id = orderId,
                Status = OrderStatus.Shipped,
                Amount = 170.0m,
                NextStatus = OrderStatus.Delivered,
                ShippingAddress = new VipAddress
                {
                    Street = "123 Main St",
                    City = new City
                    {
                        Name = "Seattle",
                        State = new State { Name = "Washington" }
                    },
                    NeighborState = new State { Name = "Oregon" },
                    PostalCode = "98101"
                },
                WarehouseAddresses = new ObservableCollection<Address>
                {
                    new Address
                    {
                        Street = "456 Warehouse St",
                        City = new City
                        {
                            Name = "Las Vegas",
                            State = new State { Name = "Florida" }
                        },
                        NeighborState = new State { Name = "Georgia" }
                    },
                    new VipAddress
                    {
                        Street = "789 VIP Warehouse St",
                        City = new City
                        {
                            Name = "Anchorage",
                            State = new State { Name = "Alaska" }
                        },
                        NeighborState = new State { Name = "Hawaii" },
                        PostalCode = "99501"
                    }
                }
            };
        }
    }

    public class TestHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient client;

        public TestHttpClientFactory(HttpClient client)
        {
            this.client = client;
        }

        public HttpClient CreateClient(string name) => client;
    }
}
