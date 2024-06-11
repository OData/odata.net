//-----------------------------------------------------------------------------
// <copyright file="SampleEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Server
{
    public class SampleEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<OrderLine>("OrderLines");

            builder.Function("RetrieveProduct").Returns<int>();
            builder.Function("RetrieveProductWithOrderLine").Returns<int>().Parameter<OrderLine>("orderLine");
            builder.Function("RetrieveProductWithProduct").Returns<int>().Parameter<Product>("product");

            return builder.GetEdmModel();
        }
    }
}
