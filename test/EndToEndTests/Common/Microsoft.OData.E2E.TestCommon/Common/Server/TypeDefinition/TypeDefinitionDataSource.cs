//---------------------------------------------------------------------
// <copyright file="TypeDefinitionDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;

public class TypeDefinitionDataSource
{
    public static TypeDefinitionDataSource CreateInstance()
    {
        return new TypeDefinitionDataSource();
    }

    public TypeDefinitionDataSource()
    {
        ResetDataSource();
        Initialize();
    }

    public IList<Person>? People { get; private set; }
    public IList<Product>? Products { get; private set; }

    private void Initialize()
    {
        this.People =
        [
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
                Height = new Height { Value = 1.8, Unit = Height.DistanceUnit.M },
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
                Height = new Height { Value = 160, Unit = Height.DistanceUnit.CM },
                Descriptions = ["Nice"]
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
                Height = new Height { Value = 1.8, Unit = Height.DistanceUnit.M },
                Descriptions = ["Easy going", "Smile"]
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
                Height = new Height { Value = 62, Unit = Height.DistanceUnit.IN },
                Descriptions = ["Patient"]
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
                Height = new Height { Value = 7, Unit = Height.DistanceUnit.FT },
                Descriptions = []
            }
        ];

        this.Products =
        [
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
                LargeNumbers = new Collection<UInt64> { 36,12, 9 },
                Temperature = new Temperature(10.57, TemperatureKind.Fahrenheit)
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
                LargeNumbers = new Collection<UInt64> { UInt64.MaxValue, UInt64.MaxValue },
                Temperature = new Temperature(30.67, TemperatureKind.Celsius)
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
                LargeNumbers = new Collection<UInt64> { UInt64.MinValue },
                Temperature = new Temperature(-0.23, TemperatureKind.Celsius)
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
                LargeNumbers = new Collection<UInt64>(),
                Temperature = new Temperature(-19.4, TemperatureKind.Fahrenheit)
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
                LargeNumbers = new Collection<UInt64> { 99, 38 },
                Temperature = new Temperature(10.57, TemperatureKind.Fahrenheit)
            }
        ];
    }

    private void ResetDataSource()
    {
        this.People = new List<Person>();
        this.Products = new List<Product>();
    }
}
