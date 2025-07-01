//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models
{
    public partial class Order
    {
        public OrderStatus NextStatus { get; set; }
        public ObservableCollection<OrderStatus>? ProhibitedStatuses { get; set; }
        public ObservableCollection<string>? TagsHistory { get; set; }
        public Address? PickupAddress { get; set; }
        public ObservableCollection<Address>? ReturnAddresses { get; set; }
    }

    public partial class Address
    {
        public State? NeighborState { get; set; }
    }

    public partial class City
    {
        public State? State { get; set; }
    }
}
