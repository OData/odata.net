//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server
{
    public class AnnotationSerializationDataSource
    {
        private static readonly List<Order> orders;

        static AnnotationSerializationDataSource()
        {
            orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    Status = OrderStatus.Pending,
                    StatusHistory = new List<OrderStatus> { OrderStatus.Pending },
                    Tags = new List<string> { "Urgent" },
                    Amount = 130.00m,
                    ShippingAddress = new Address
                    {
                        Street = "123 Main St",
                        City = new City
                        {
                            Name = "Seattle",
                            DynamicProperties = new Dictionary<string, object> { { "State", new State { Name = "Washington" } } }
                        },
                        DynamicProperties = new Dictionary<string, object> { { "NeighborState", new State { Name = "Oregon" } } }
                    },
                    WarehouseAddresses = new List<Address>
                    {
                        new Address
                        {
                            Street = "456 Warehouse St",
                            City = new City
                            {
                                Name = "Las Vegas",
                                DynamicProperties = new Dictionary<string, object> { { "State", new State { Name = "Nevada" } } }
                            },
                            DynamicProperties = new Dictionary<string, object> { { "NeighborState", new State { Name = "California" } } }
                        },
                    },
                    DynamicProperties = new Dictionary<string, object>
                    {
                        { "NextStatus", OrderStatus.Processing },
                        { "ProhibitedStatuses", new List<OrderStatus> { OrderStatus.Returned } },
                        { "TagsHistory", new List<string> { "Express" } },
                        {
                            "PickupAddress",
                            new Address
                            {
                                Street = "789 Pickup Rd",
                                City = new City
                                {
                                    Name = "Houston",
                                    DynamicProperties = new Dictionary<string, object> { { "State", new State { Name = "Texas" } } }
                                },
                                DynamicProperties = new Dictionary<string, object> { { "NeighborState", new State { Name = "New Mexico" } } }
                            }
                        },
                        {
                            "ReturnAddresses",
                            new List<Address>
                            {
                                new Address
                                {
                                    Street = "321 Return St",
                                    City = new City
                                    {
                                        Name = "Miami",
                                        DynamicProperties = new Dictionary<string, object> { { "State", new State { Name = "Florida" } } }
                                    },
                                    DynamicProperties = new Dictionary<string, object> { { "NeighborState", new State { Name = "Georgia" } } }
                                }
                            }
                        }
                    }
                },
                new VipOrder
                {
                    Id = 2,
                    Status = OrderStatus.Processing,
                    StatusHistory = new List<OrderStatus> { OrderStatus.Pending, OrderStatus.Processing },
                    Tags = new List<string> { "VIP", "Priority" },
                    Amount = 900.00m,
                    ShippingAddress = new Address
                    {
                        Street = "456 VIP St",
                        City = new VipCity
                        {
                            Name = "New York",
                            AreaCode = "10001",
                            DynamicProperties = new Dictionary<string, object> { { "State", new VipState { Name = "New York", TwoLetterCode = "NY" } } }
                        },
                        DynamicProperties = new Dictionary<string, object> { { "NeighborState", new VipState { Name = "Massachusetts", TwoLetterCode = "MA" } } }
                    },
                    WarehouseAddresses = new List<Address>
                    {
                        new Address
                        {
                            Street = "789 VIP Warehouse St",
                            City = new VipCity
                            {
                                Name = "Los Angeles",
                                AreaCode = "90001",
                                DynamicProperties = new Dictionary<string, object> { { "State", new VipState { Name = "California", TwoLetterCode = "CA" } } }
                            },
                            DynamicProperties = new Dictionary<string, object> { { "NeighborState", new VipState { Name = "Arizona", TwoLetterCode = "AZ" } } }
                        },
                    },
                    TrackingNumber = 87543,
                }
            };
        }

        public static List<Order> Orders  => orders;
    }
}
