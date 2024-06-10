//---------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Models
{
    public class DataSource
    {
        private static IList<Product> _products;

        static DataSource()
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
                    //  Dimensions = new Dimensions { Width = 10, Height = 20, Depth = 5 },
                    BaseConcurrency = Guid.NewGuid().ToString(),
                    /*ComplexConcurrency = new ConcurrencyInfo
                    {
                        Token = Guid.NewGuid().ToString(),
                        QueriedDateTime = DateTimeOffset.Now
                    },*/
                    /*NestedComplexConcurrency = new AuditInfo
                    {
                        ModifiedDate = DateTimeOffset.Now,
                        ModifiedBy = "Admin",
                        Concurrency = new ConcurrencyInfo
                        {
                            Token = Guid.NewGuid().ToString(),
                            QueriedDateTime = DateTimeOffset.Now
                        }
                    },*/
                    //RelatedProducts = new List<Product>(),
                    //Detail = new ProductDetail { ProductId = i, Details = "Red" },
                    // Reviews = new List<ProductReview>(),
                    //Photos = new List<ProductPhoto>()
                };

                _products.Add(product);
            }
        }
    }
}
