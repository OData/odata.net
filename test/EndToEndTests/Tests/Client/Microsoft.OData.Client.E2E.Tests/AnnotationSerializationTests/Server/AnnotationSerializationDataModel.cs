//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models
{
    public class Address
    {
        public string Street { get; set; } = null!;
        public City City { get; set; } = null!;
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }

    public class City
    {
        public string Name { get; set; } = null!;
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }

    public class State
    {
        public string Name { get; set; } = null!;
    }

    public class VipAddress : Address
    {
        public string? PostalCode { get; set; }
    }

    public class VipCity : City
    {
        public string? AreaCode { get; set; }
    }

    public class VipState : State
    {
        public string? TwoLetterCode { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderStatus>? StatusHistory { get; set; }
        public List<string>? Tags { get; set; }
        public decimal Amount { get; set; }
        public Address? ShippingAddress { get; set; }
        public List<Address>? WarehouseAddresses { get; set; }
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }

    public class VipOrder : Order
    {
        public int TrackingNumber { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }
}
