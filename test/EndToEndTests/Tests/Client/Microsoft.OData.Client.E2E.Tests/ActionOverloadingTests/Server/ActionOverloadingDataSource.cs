//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class ActionOverloadingDataSource
    {
        static ActionOverloadingDataSource()
        {
            GenerateProducts();
            GenerateOrderLines();
            GeneratePeople();
        }
        public static IList<Product> Products { get; private set; }

        public static IList<OrderLine> OrderLines { get; private set; }

        public static IList<Person> People { get; private set; }

        private static void GenerateProducts()
        {
            Products = new List<Product>();

            for (int i = -10; i < 0; i++)
            {
                var product = new Product
                {
                    ProductId = i,
                    Description = $"Product {Math.Abs(i)} Description",
                    BaseConcurrency = Guid.NewGuid().ToString()
                };

                Products.Add(product);
            }
        }

        private static void GenerateOrderLines()
        {
            OrderLines = new List<OrderLine>();

            for (int i = -10; i < 0; i++)
            {
                var product = new Product
                {
                    ProductId = i,
                    Description = $"Product {Math.Abs(i)} Description",
                    BaseConcurrency = Guid.NewGuid().ToString()
                };

                OrderLine orderLine = new OrderLine
                {
                    OrderId = i,
                    ProductId = product.ProductId,
                    Quantity = 10,
                    Product = product
                };
                OrderLines.Add(orderLine);
            }
        }

        private static void GeneratePeople()
        {
            People = new List<Person>
            {
                new SpecialEmployee { PersonId = -10, Name = "SpecialEmployee-10" },
                new SpecialEmployee { PersonId = -9, Name = "SpecialEmpoyee-9" },
                new SpecialEmployee { PersonId = -8, Name = "SpecialEmployee-8" },
                new SpecialEmployee { PersonId = -7, Name = "SpecialEmployee-7", Salary = 2016141256 },
                new Employee { PersonId = -6, Name = "Employee-6", Salary = 0 },
                new Person { PersonId = -5, Name = "Person-5" },
                new Person { PersonId = -4, Name = "Person-4" },
                new Employee { PersonId = -3, Name = "Employee-3", Salary = 0 },
                new Person { PersonId = -2, Name = "Person-2" },
                new Person { PersonId = -1, Name = "Person-1" },
                new Employee { PersonId = 0, Name = "Employee-0", Salary = 85 },
                new Contractor { PersonId = 1, Name = "Contractor-1" },
                new Contractor { PersonId = 2, Name = "Contractor-2" }
            };
        }
    }
}
