//-----------------------------------------------------------------------------
// <copyright file="PayloadValueConverterEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Server
{
    public class PayloadValueConverterEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Person>("People");
            builder.EntitySet<Product>("Products");
            var action = builder.EntityType<Product>()
                .Action("AddProduct");
            action.CollectionParameter<Product>("Set");
            action.Parameter<Product>("Value");
            action.Parameter<bool>("Override");

            builder.Action("ResetDataSource");

            return builder.GetEdmModel();
        }
    }
}
