//-----------------------------------------------------------------------------
// <copyright file="PayloadValueConverterDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Server
{
    public class PayloadValueConverterDataSource
    {
        public static PayloadValueConverterDataSource CreateInstance()
        {
            return new PayloadValueConverterDataSource();
        }

        public PayloadValueConverterDataSource()
        {
            ResetDataSource();
            Initialize();
        }

        public IList<Person>? People { get; private set; }
        public IList<Product>? Products { get; private set; }

        /// <summary>
        /// Populate the data source.
        /// </summary>
        public void Initialize()
        {
            this.People = new List<Person>()
            {
                new Person()
                {
                    Id = 31,
                    Picture = new byte[] { 5, 8 },
                    Numbers = new List<int> { 3, 5, 7 },
                    BusinessCard = new ContactInfo()
                    {
                        N = "Name1"
                    }
                },
                new Person()
                {
                    Id = 32,
                    Numbers = new List<int>{},
                    BusinessCard = new ContactInfo()
                    {
                        N = "Name2"
                    }
                },
            };

            this.Products = new[]
            {
                new Product()
                {
                    Id = 1,
                    Name = "Pear"
                },
                new Product()
                {
                    Id = 2,
                    Name = "Banana",
                    Info = new ProductInfo()
                    {
                        Site = "G2",
                        Serial = 0x3FF
                    }
                },
                new Product()
                {
                    Id = 3,
                    Name = "Mangosteen",
                    Info = new ProductInfo()
                    {
                        Site = "G3",
                        Serial = 0x3FE,
                    }
                }
            };
        }

        /// <summary>
        /// Resets the data source
        /// </summary>
        public void ResetDataSource()
        {
            People?.Clear();
            Products?.Clear();
        }
    }
}
