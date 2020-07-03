//---------------------------------------------------------------------
// <copyright file="EntityParameterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;

namespace Microsoft.OData.Client.TDDUnitTests.Tests.EntityParameter
{
    using System;
    using System.Text;
    using Microsoft.OData.Client;
    using Xunit;

    public class EntityParameterTests
    {
        private readonly byte[] payloadBuffer = new byte[1024];

        private readonly InMemoryEntities dsc = new InMemoryEntities(new Uri("http://service-root/"));

        private const string ExpectedPayloadOfEntityWithFullProperties = "{\"order\":{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderDate@odata.type\":\"#DateTimeOffset\",\"OrderDate\":\"0001-01-01T00:00:00Z\",\"OrderID\":100,\"OrderShelfLifes@odata.type\":\"#Collection(Duration)\",\"OrderShelfLifes\":[],\"ShelfLife\":null,\"ShipDate@odata.type\":\"#Date\",\"ShipDate\":\"0001-01-01\",\"ShipTime@odata.type\":\"#TimeOfDay\",\"ShipTime\":\"00:00:00.0000000\"}}";

        private const string ExpectedPayloadOfEntityWithOnlySetProperties = "{\"order\":{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderID\":100}}";

        private const string ExpectedPayloadOfEntityCollectionWithFullProperties = "{\"orders\":[{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderDate@odata.type\":\"#DateTimeOffset\",\"OrderDate\":\"0001-01-01T00:00:00Z\",\"OrderID\":100,\"OrderShelfLifes@odata.type\":\"#Collection(Duration)\",\"OrderShelfLifes\":[],\"ShelfLife\":null,\"ShipDate@odata.type\":\"#Date\",\"ShipDate\":\"0001-01-01\",\"ShipTime@odata.type\":\"#TimeOfDay\",\"ShipTime\":\"00:00:00.0000000\"},{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderDate@odata.type\":\"#DateTimeOffset\",\"OrderDate\":\"0001-01-01T00:00:00Z\",\"OrderID\":101,\"OrderShelfLifes@odata.type\":\"#Collection(Duration)\",\"OrderShelfLifes\":[],\"ShelfLife\":null,\"ShipDate@odata.type\":\"#Date\",\"ShipDate\":\"0001-01-01\",\"ShipTime@odata.type\":\"#TimeOfDay\",\"ShipTime\":\"00:00:00.0000000\"}]}";

        private const string ExpectedPayloadOfEntityCollectionWithOnlySetProperties = "{\"orders\":[{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderID\":100},{\"@odata.type\":\"#Microsoft.Test.OData.Services.ODataWCFService.Order\",\"OrderID\":101}]}";

        [Fact]
        public void PostFullPropertiesOfEntityParameterToActionUsingContext()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            CallPlaceOrderFromContext(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithFullProperties, GetPayload());
        }

        [Fact]
        public void TestDefaultSendOptionToPostPropertiesOfEntityParameterToActionUsingContext()
        {
            InitializeDataServiceContext();

            CallPlaceOrderFromContext(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithFullProperties, GetPayload());
        }

        [Fact]
        public void PostOnlySetPropertiesOfEntityParameterToActionUsingContext()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            CallPlaceOrderFromContext(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithOnlySetProperties, GetPayload());
        }

        [Fact]
        public void PostFullPropertiesOfEntityParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            CallPlaceOrderFromActionQuery(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithFullProperties, GetPayload());
        }

        [Fact]
        public void TestDefaultSendOptionToPostPropertiesOfEntityParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext();

            CallPlaceOrderFromActionQuery(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithFullProperties, GetPayload());
        }

        [Fact]
        public void PostOnlySetPropertiesOfEntityParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            CallPlaceOrderFromActionQuery(GetTrackedOrder());

            Assert.Equal(ExpectedPayloadOfEntityWithOnlySetProperties, GetPayload());
        }

        [Fact]
        public void ShouldThrowExceptionWhenSendOnlySetPropertiesUsingContextIfEntityIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            Action exec = () => CallPlaceOrderFromContext(GetUntrackedOrder());

            exec.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_MustBeUsedWith("EntityParameterSendOption.SendOnlySetProperties", "DataServiceCollection"));
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenSendFullPropertiesUsingContextIfEntityIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            Action exec = () => CallPlaceOrderFromContext(GetUntrackedOrder());

            exec.ShouldNotThrow();
        }

        [Fact]
        public void ShouldThrowExceptionWhenSendOnlySetPropertiesUsingActionQueryIfEntityIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            Action exec = () => CallPlaceOrderFromActionQuery(GetUntrackedOrder());

            exec.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_MustBeUsedWith("EntityParameterSendOption.SendOnlySetProperties", "DataServiceCollection"));
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenSendFullPropertiesUsingActionQueryIfEntityIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            Action exec = () => CallPlaceOrderFromActionQuery(GetUntrackedOrder());

            exec.ShouldNotThrow();
        }

        [Fact]
        public void PostFullPropertiesOfEntityCollectionParameterToActionUsingContext()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            CallPlaceOrdersFromContext(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithFullProperties, GetPayload());
        }

        [Fact]
        public void TestDefaultSendOptionToPostPropertiesOfEntityCollectionParameterToActionUsingContext()
        {
            InitializeDataServiceContext();

            CallPlaceOrdersFromContext(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithFullProperties, GetPayload());
        }

        [Fact]
        public void PostOnlySetPropertiesOfEntityCollectionParameterToActionUsingContext()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            CallPlaceOrdersFromContext(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithOnlySetProperties, GetPayload());
        }

        [Fact]
        public void ShouldThrowExceptionWhenSendOnlySetPropertiesUsingContextIfAnyEntityInCollectionIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            Action exec = () => CallPlaceOrdersFromContext(GetUntrackedOrders());

            exec.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_MustBeUsedWith("EntityParameterSendOption.SendOnlySetProperties", "DataServiceCollection"));
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenSendFullPropertiesUsingContextIfAnyEntityInCollectionIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            Action exec = () => CallPlaceOrdersFromContext(GetUntrackedOrders());

            exec.ShouldNotThrow();
        }

        [Fact]
        public void PostFullPropertiesOfEntityCollectionParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            CallPlaceOrdersFromActionQuery(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithFullProperties, GetPayload());
        }

        [Fact]
        public void TestDefaultSendOptionToPostPropertiesOfEntityCollectionParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext();

            CallPlaceOrdersFromActionQuery(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithFullProperties, GetPayload());
        }

        [Fact]
        public void PostOnlySetPropertiesOfEntityCollectionParameterToActionUsingActionQuery()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            CallPlaceOrdersFromActionQuery(GetTrackedOrders());

            Assert.Equal(ExpectedPayloadOfEntityCollectionWithOnlySetProperties, GetPayload());
        }

        [Fact]
        public void ShouldThrowExceptionWhenSendOnlySetPropertiesUsingActionQueryIfAnyEntityInCollectionIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendOnlySetProperties);

            Action exec = () => CallPlaceOrdersFromActionQuery(GetUntrackedOrders());

            exec.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_MustBeUsedWith("EntityParameterSendOption.SendOnlySetProperties", "DataServiceCollection"));
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenSendFullPropertiesUsingActionQueryIfAnyEntityInCollectionIsNotAddedToDataServiceCollection()
        {
            InitializeDataServiceContext(EntityParameterSendOption.SendFullProperties);

            Action exec = () => CallPlaceOrdersFromActionQuery(GetUntrackedOrders());

            exec.ShouldNotThrow();
        }

        private Order GetUntrackedOrder()
        {
            return new Order { OrderID = (new Random()).Next() };
        }

        private Order GetTrackedOrder()
        {
            var order = GetUntrackedOrder();

            // Add the entity to a DataServiceCollection to track it.
            var collection = new DataServiceCollection<Order>(dsc, "Orders", null, null);
            collection.Add(order);

            // Change some property intentionally.
            order.OrderID = 100;

            return order;
        }

        private ICollection<Order> GetTrackedOrders()
        {
            var order1 = GetUntrackedOrder();
            var order2 = GetUntrackedOrder();

            // Add the entities to a DataServiceCollection to track them.
            var collection = new DataServiceCollection<Order>(dsc, "Orders", null, null);
            collection.Add(order1);
            collection.Add(order2);

            // Change some properties intentionally.
            order1.OrderID = 100;
            order2.OrderID = 101;

            return new List<Order> { order1, order2 };
        }
        private ICollection<Order> GetUntrackedOrders()
        {
            var order1 = GetUntrackedOrder();
            var order2 = GetUntrackedOrder();

            return new List<Order> { order1, order2 };
        }

        private void CallPlaceOrderFromContext(Order order)
        {
            dsc.Execute(new Uri("http://service-root/Customers(0)/PlaceOrder"), "POST", new BodyOperationParameter("order", order));
        }

        private void CallPlaceOrdersFromContext(ICollection<Order> orders)
        {
            dsc.Execute(new Uri("http://service-root/Customers(0)/PlaceOrders"), "POST", new BodyOperationParameter("orders", orders));
        }

        private void CallPlaceOrderFromActionQuery(Order order)
        {
            try
            {
                var query = new DataServiceActionQuerySingle<Order>(dsc, "http://service-root/Customers(0)/Microsoft.Test.OData.Services.ODataWCFService.PlaceOrder", new BodyOperationParameter("order", order));
                query.GetValue();
            }
            catch (InvalidOperationException ex)
            {
                // Ignore the exception thrown by Single() as no actual response is provided.
                if (ex.Message != "Sequence contains no elements")
                {
                    throw;
                }
            }
        }

        private void CallPlaceOrdersFromActionQuery(ICollection<Order> orders)
        {
            var query = new DataServiceActionQuery<Order>(dsc, "http://service-root/Customers(0)/Microsoft.Test.OData.Services.ODataWCFService.PlaceOrders", new BodyOperationParameter("orders", orders));
            query.Execute();
        }

        /// <summary>
        /// Initialize DataServiceContext with default EntityParameterSendOption.
        /// </summary>
        private void InitializeDataServiceContext()
        {
            dsc.Configurations.RequestPipeline.OnMessageCreating = args => new EntityParameterRequestMessage(args, payloadBuffer);
        }

        /// <summary>
        /// Initialize DataServiceContext with the specified EntityParameterSendOption.
        /// </summary>
        /// <param name="option">The option to send entity parameters.</param>
        private void InitializeDataServiceContext(EntityParameterSendOption option)
        {
            InitializeDataServiceContext();

            dsc.EntityParameterSendOption = option;
            Assert.Equal(option, dsc.EntityParameterSendOption);
        }

        /// <summary>
        /// Get the payload string from the payload buffer.
        /// </summary>
        /// <returns>The payload string.</returns>
        private string GetPayload()
        {
            return Encoding.UTF8.GetString(payloadBuffer, 0, GetActualPayloadLength());
        }

        /// <summary>
        /// Get the actual length of the payload.
        /// </summary>
        /// <remarks>This method find the index of the first occurrence of '\0'.</remarks>
        /// <returns>The actual length of the payload.</returns>
        private int GetActualPayloadLength()
        {
            for (int i = 0; i < payloadBuffer.Length; ++i)
            {
                if (payloadBuffer[i] == 0)
                {
                    return i;
                }
            }

            return payloadBuffer.Length;
        }
    }
}