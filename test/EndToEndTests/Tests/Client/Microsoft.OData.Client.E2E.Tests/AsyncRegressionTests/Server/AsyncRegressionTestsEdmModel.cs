//-----------------------------------------------------------------------------
// <copyright file="AsyncRegressionTestsEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Server;

public class AsyncRegressionTestsEdmModel
{
    public static IEdmModel GetEdmModel()
    {
        var modelBuilder = new ODataConventionModelBuilder();
        
        var customersEntitySetConfiguration = modelBuilder.EntitySet<Customer>("Customers");
        customersEntitySetConfiguration.EntityType.Collection.Function("GetTopCustomer")
            .ReturnsFromEntitySet<Customer>("Customers");
        
        var ordersEntitySetConfiguration = modelBuilder.EntitySet<Order>("Orders");
        ordersEntitySetConfiguration.EntityType.Collection.Function("GetTop2Orders")
            .ReturnsCollectionFromEntitySet<Order>("Orders");
        var applyDiscountAction = ordersEntitySetConfiguration.EntityType.Action("ApplyDiscount");
        applyDiscountAction.Parameter<decimal>("discountPercentage");
        applyDiscountAction.Returns<decimal>();

        modelBuilder.EntitySet<MediaAsset>("Media");

        modelBuilder.Action("ResetDataSource");

        return modelBuilder.GetEdmModel();
    }
}
