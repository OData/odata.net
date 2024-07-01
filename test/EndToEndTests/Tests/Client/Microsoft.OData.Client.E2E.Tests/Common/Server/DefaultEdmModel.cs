//-----------------------------------------------------------------------------
// <copyright file="DefaultModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server
{
    public class DefaultEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Person>("People");
            builder.EntitySet<Account>("Accounts");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<ProductDetail>("ProductDetails");

            return builder.GetEdmModel();
        }
    }
}
