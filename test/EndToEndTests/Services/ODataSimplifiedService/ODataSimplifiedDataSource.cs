//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.SampleService.Models.ODataSimplified;

    public class ODataSimplifiedDataSource : ODataReflectionDataSource
    {
        public EntityCollection<Person> People { get; private set; }
        public EntityCollection<Product> Products { get; private set; }

        public override void Reset()
        {
            this.People = new EntityCollection<Person>();
            this.Products = new EntityCollection<Product>();
        }

        public override void Initialize()
        {
            var product1 = new Product
            {
                ProductId = 11,
                Quantity = 100,
                LifeTimeInSeconds = 3600,
                TheCombo = new NumberCombo
                {
                    Small = 80,
                    Middle = 196,
                    Large = 3
                },
                LargeNumbers = new Collection<decimal> { 36, 12, 9 }
            };
            var product2 = new Product
            {
                ProductId = 12,
                Quantity = Int64.MaxValue,
                LifeTimeInSeconds = Decimal.MaxValue,
                TheCombo = new NumberCombo
                {
                    Small = Int32.MaxValue,
                    Middle = Int64.MaxValue,
                    Large = Decimal.MaxValue
                },
                LargeNumbers = new Collection<decimal> { Decimal.MinValue, Decimal.MaxValue }
            };
            this.People.AddRange(new List<Person>()
            {
                new Person
                {
                    PersonId = 1,
                    FirstName = "Bob",
                    LastName = "Cat",
                    Address = new Address
                    {
                        Road = "Zixing Road",
                        City = "Shanghai"
                    },
                    Descriptions = new Collection<string> { "Nice", "Tall" },
                    Products = new EntityCollection<Product> { product1, product2 }
                },
                new Person
                {
                    PersonId = 2,
                    FirstName = "Jill",
                    LastName = "Jones",
                    Address = new Address
                    {
                        Road = "Hongqiao Road",
                        City = "Shanghai"
                    },
                    Descriptions = new Collection<string> { "Nice" }
                },
                new Person
                {
                    PersonId = 3,
                    FirstName = "Jacob",
                    LastName = "Zip",
                    Address = new Address
                    {
                        Road = "1 Microsoft Way",
                        City = "Redmond"
                    },
                    Descriptions = new Collection<string> { "Easy going", "Smile" }
                },
                new Person
                {
                    PersonId = 4,
                    FirstName = "Elmo",
                    LastName = "Rogers",
                    Address = new Address
                    {
                        Road = "1 Microsoft Way",
                        City = "Redmond"
                    },
                    Descriptions = new Collection<string> { "Patient" }
                },
                new Person
                {
                    PersonId = 5,
                    FirstName = "Peter",
                    LastName = "Bee",
                    Address = new Address
                    {
                        Road = "Zixing Road",
                        City = "Shanghai"
                    },
                    Descriptions = new Collection<string>()
                }
            });

            this.Products.AddRange(new List<Product>
            {
                product1,
                product2,
                new Product
                {
                    ProductId = 13,
                    Quantity = Int64.MinValue,
                    LifeTimeInSeconds = decimal.MinValue,
                    TheCombo = new NumberCombo
                    {
                        Small = Int32.MinValue,
                        Middle = Int64.MinValue,
                        Large = decimal.MinValue
                    },
                    LargeNumbers = new Collection<decimal> { Decimal.MinValue }
                },
                new Product
                {
                    ProductId = 14,
                    Quantity = 30,
                    LifeTimeInSeconds = 3600,
                    TheCombo = new NumberCombo
                    {
                        Small = 12,
                        Middle = 133333,
                        Large = 99889986
                    },
                    LargeNumbers = new Collection<decimal>()
                },
                new Product
                {
                    ProductId = 15,
                    Quantity = 105,
                    LifeTimeInSeconds = 1800,
                    TheCombo = new NumberCombo
                    {
                        Small = 80,
                        Middle = 196,
                        Large = 3
                    },
                    LargeNumbers = new Collection<decimal> { 99, 38 }
                }
            });
        }

        protected override IEdmModel CreateModel()
        {
            return ODataSimplifiedInMemoryModel.CreateModel("Microsoft.OData.SampleService.Models.ODataSimplified");
        }

        protected override void ConfigureContainer(IContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.AddServicePrototype(new ODataSimplifiedOptions()
            {
                EnableWritingODataAnnotationWithoutPrefix = true,
                EnableReadingODataAnnotationWithoutPrefix = true,
            });
        }
    }
}