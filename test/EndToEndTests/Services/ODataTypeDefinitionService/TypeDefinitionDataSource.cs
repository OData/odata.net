//---------------------------------------------------------------------
// <copyright file="TypeDefinitionDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Edm;
    using microsoft.odata.sampleService.models.typedefinition;

    public class TypeDefinitionDataSource : ODataReflectionDataSource
    {
        public EntityCollection<Person> People { get; private set; }
        public EntityCollection<Product> Products { get; private set; }

        public TypeDefinitionDataSource()
        {
            this.OperationProvider = new TypeDefinitionOperationProvider();
        }

        public override void Reset()
        {
            this.People = new EntityCollection<Person>();
            this.Products = new EntityCollection<Product>();
        }

        public override void Initialize()
        {
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
                    Descriptions = new Collection<string> { "Nice", "Tall" }
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

            this.Products.AddRange(new List<Product>()
            {
                new Product
                {
                    ProductId = 11,
                    Quantity = 100,
                    NullableUInt32 = 12,
                    LifeTimeInSeconds = 3600,
                    TheCombo = new NumberCombo
                    {
                        Small = 80,
                        Middle = 196,
                        Large = 3
                    },
                    LargeNumbers = new Collection<UInt64> { 36,12, 9 }
                },
                new Product
                {
                    ProductId = 12,
                    Quantity = UInt32.MaxValue,
                    NullableUInt32 = UInt32.MaxValue,
                    LifeTimeInSeconds = UInt64.MaxValue,
                    TheCombo = new NumberCombo
                    {
                        Small = UInt16.MaxValue,
                        Middle = UInt32.MaxValue,
                        Large = UInt64.MaxValue
                    },
                    LargeNumbers = new Collection<UInt64> { UInt64.MaxValue, UInt64.MaxValue }
                },
                new Product
                {
                    ProductId = 13,
                    Quantity = UInt32.MinValue,
                    NullableUInt32 = null,
                    LifeTimeInSeconds = UInt64.MinValue,
                    TheCombo = new NumberCombo
                    {
                        Small = UInt16.MinValue,
                        Middle = UInt32.MinValue,
                        Large = UInt64.MinValue
                    },
                    LargeNumbers = new Collection<UInt64> { UInt64.MinValue }
                },
                new Product
                {
                    ProductId = 14,
                    Quantity = 30,
                    NullableUInt32 = 109,
                    LifeTimeInSeconds = 3600,
                    TheCombo = new NumberCombo
                    {
                        Small = 12,
                        Middle = 133333,
                        Large = 99889986
                    },
                    LargeNumbers = new Collection<UInt64>()
                },
                new Product
                {
                    ProductId = 15,
                    Quantity = 105,
                    NullableUInt32 = null,
                    LifeTimeInSeconds = 1800,
                    TheCombo = new NumberCombo
                    {
                        Small = 80,
                        Middle = 196,
                        Large = 3
                    },
                    LargeNumbers = new Collection<UInt64> { 99, 38 }
                }
            });


        }

        protected override IEdmModel CreateModel()
        {
            return TypeDefinitionInMemoryModel.CreateModel("microsoft.odata.sampleService.models.typedefinition");
        }
    }
}