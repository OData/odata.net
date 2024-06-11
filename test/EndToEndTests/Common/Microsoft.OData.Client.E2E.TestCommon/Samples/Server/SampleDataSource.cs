//-----------------------------------------------------------------------------
// <copyright file="SampleDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Server
{
    public class SampleDataSource
    {
        private static IList<Product> _products;

        static SampleDataSource()
        {
            GenerateProducts();
        }
        public static IList<Product> Products => _products;

        private static void GenerateProducts()
        {
            _products = new List<Product>();

            for (int i = -10; i < 0; i++)
            {
                var product = new Product
                {
                    ProductId = i,
                    Description = $"Product {Math.Abs(i)} Description",
                    BaseConcurrency = Guid.NewGuid().ToString()
                };

                _products.Add(product);
            }
        }
    }
}
