//---------------------------------------------------------------------
// <copyright file="OperationsEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Operations;

public class OperationsEdmModel
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();

        builder.EntitySet<Customer>("Customers");
        builder.EntitySet<Order>("Orders");

        builder.ComplexType<Address>();
        builder.ComplexType<CompanyAddress>();
        builder.ComplexType<OrderDetail>();
        builder.ComplexType<InfoFromCustomer>();

        builder.Action("ResetDefaultDataSource");

        builder.EntityType<Customer>().Collection
            .Function("GetCustomerForAddress")
            .ReturnsEntityViaEntitySetPath<Customer>("bindingParameter")
            .Parameter<Address>("address");

        builder.EntityType<Customer>().Collection
            .Function("GetCustomersForAddresses")
            .ReturnsCollectionViaEntitySetPath<Customer>("bindingParameter")
            .CollectionParameter<Address>("addresses");

        builder.EntityType<Customer>().Collection
            .Function("GetCustomerForAddresses")
            .ReturnsEntityViaEntitySetPath<Customer>("bindingParameter")
            .CollectionParameter<Address>("addresses");

        builder.EntityType<Customer>()
            .Function("GetCustomerAddress")
            .Returns<Address>();

        builder.EntityType<Customer>()
            .Function("VerifyCustomerAddress")
            .ReturnsEntityViaEntitySetPath<Customer>("bindingParameter")
            .Parameter<Address>("address");

        builder.EntityType<Customer>()
            .Function("VerifyCustomerByOrder")
            .ReturnsEntityViaEntitySetPath<Customer>("bindingParameter")
            .Parameter<Order>("order");

        builder.EntityType<Customer>()
            .Function("GetOrdersFromCustomerByNotes")
            .ReturnsCollectionViaEntitySetPath<Order>("bindingParameter/Orders")
            .CollectionParameter<string>("notes");

        builder.EntityType<Order>().Collection
            .Function("GetOrdersByNote")
            .ReturnsCollectionViaEntitySetPath<Order>("bindingParameter")
            .Parameter<string>("note");

        builder.EntityType<Order>().Collection
            .Function("GetOrderByNote")
            .ReturnsEntityViaEntitySetPath<Order>("bindingParameter")
            .CollectionParameter<string>("notes");

        builder.EntityType<Customer>().Collection
            .Function("GetCustomersByOrders")
            .ReturnsCollectionViaEntitySetPath<Customer>("bindingParameter")
            .CollectionParameter<Order>("orders");

        builder.EntityType<Customer>().Collection
            .Function("GetCustomerByOrder")
            .ReturnsEntityViaEntitySetPath<Customer>("bindingParameter")
            .Parameter<Order>("order");

        builder.Function("GetCustomersByOrders")
            .ReturnsCollection<Customer>()
            .CollectionParameter<Order>("orders");

        builder.Function("GetCustomerByOrder")
            .Returns<Customer>()
            .Parameter<Order>("order");

        builder.Function("GetCustomerAddress")
            .Returns<Address>()
            .Parameter<Customer>("customer");

        return builder.GetEdmModel();
    }
}
