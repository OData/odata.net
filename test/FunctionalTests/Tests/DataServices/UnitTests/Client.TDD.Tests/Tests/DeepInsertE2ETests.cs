//---------------------------------------------------------------------
// <copyright file="DeepInsertE2ETests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AstoriaUnitTests.Tests;
using FluentAssertions;
using Microsoft.OData.Client.Materialization;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    /// <summary>
    /// DeepInsert E2E tests.
    /// </summary>
    public class DeepInsertE2ETests
    {
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
    <edmx:DataServices>
        <Schema Namespace=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityType Name=""Person"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
                <NavigationProperty Name=""Cars"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Car)""/>
            </EntityType>
            <EntityType Name=""VipPerson"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
                <NavigationProperty Name=""VipCar"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Car""/>
            </EntityType>
            <EntityType Name=""Car"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false""/>
                <Property Name=""Registration"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Registration"" Nullable=""true""/>
                <NavigationProperty Name=""Owner"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Person""/>
                <NavigationProperty Name=""Owners"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Person)""/>
                <NavigationProperty Name=""Manufacturers"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Manufacturer)""/>
            </EntityType>
            <EntityType Name=""Manufacturer"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
                <NavigationProperty Name=""Countries"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Country)""/>
            </EntityType>
            <EntityType Name=""Country"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
            </EntityType>
            <ComplexType Name=""Registration"">
                <Property Name=""Date"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
                <Property Name=""LicensePlate"" Type=""Edm.String""/>
            </ComplexType>
        </Schema>
        <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""Persons"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Person"">
                    <NavigationPropertyBinding Path=""Cars"" Target=""Cars""/>
                </EntitySet>
                <EntitySet Name=""VipPersons"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.VipPerson"">
                    <NavigationPropertyBinding Path=""VipCar"" Target=""Cars""/>
                </EntitySet>
                <EntitySet Name=""Cars"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Car"">
                    <NavigationPropertyBinding Path=""Owners"" Target=""Persons""/>
                    <NavigationPropertyBinding Path=""Manufacturers"" Target=""Manufacturers""/>
                </EntitySet>
                <EntitySet Name=""Manufacturers"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Manufacturer"">
                    <NavigationPropertyBinding Path=""Countries"" Target=""Countries""/>                 
                </EntitySet>
                <EntitySet Name=""Countries"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.DeepInsertE2ETests.Country""/>
            </EntityContainer>
        </Schema>
    </edmx:DataServices></edmx:Edmx>";

        private const string persons = @"{
            ""@odata.context"":""http://localhost:5000/$metadata#Persons"",
            ""value"":[
                {""ID"":100,""Name"":""Person 100"", ""Cars"":[{""ID"":1001,""Name"":""Car 1001"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerA""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]},
                {""ID"":200,""Name"":""Person 200"", ""Cars"":[{""ID"":1002,""Name"":""Car 1002"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerB""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]},
                {""ID"":300,""Name"":""Person 300"", ""Cars"":[{""ID"":1003,""Name"":""Car 1003"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerC""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]}
            ]}";

        private const string cars = @"{
            ""@odata.context"":""http://localhost:5000/$metadata#Cars"",
            ""value"":[
                {""ID"":1001,""Name"":""Car 1001"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerA""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]},
                {""ID"":1002,""Name"":""Car 1002"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerB""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]},
                {""ID"":1003,""Name"":""Car 1003"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerC""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}
            ]}";

        private readonly Container context;
        private readonly RequestInfo requestInfo;
        private readonly Serializer serializer;
        private readonly HeaderCollection headers;
        private readonly Person person;
        private readonly Person personWithoutId;
        private readonly VipPerson vipPerson;
        private readonly Car car;
        private readonly Car car2;
        private readonly Car carWithoutId;
        private readonly Car carWithRegistration;

        public DeepInsertE2ETests()
        {
            this.context = new Container(new Uri("http://localhost:5000"));
            this.requestInfo = new RequestInfo(context);
            this.serializer = new Serializer(this.requestInfo);
            this.headers = new HeaderCollection();
            this.person = new Person
            {
                ID = 100,
                Name = "Person 100",
            };
            this.personWithoutId = new Person
            {
                Name = "Person 100",
            };
            this.car = new Car
            {
                ID = 1001,
                Name = "Car 1001"
            };
            this.car2 = new Car
            {
                ID = 1002,
                Name = "Car 1002"
            };
            this.carWithoutId = new Car
            {
                ID = 1001,
                Name = "Car 1001"
            };
            this.carWithRegistration = new Car
            {
                ID = 1001,
                Name = "Car 1001",
                Registration = new Registration
                {
                    Date = new DateTimeOffset(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
                    LicensePlate = "DL-172238"
                }
            };
            this.vipPerson = new VipPerson
            {
                ID = 100,
                Name = "VipPerson 100",
            };
        }

        private void SetupDataServiceContext(string expectedResponse)
        {
            var headers = new Dictionary<string, string>()
                        {
                            {"Content-Type", "application/json;charset=utf-8"},
                            {"Location", "http://localhost:5000/Persons(100)"},
                        };

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse,
                headers);
        }

        [Fact]
        public void CallingDeepInsert_WithNullArguments_ShouldThrowAnException()
        {
            Assert.Throws<ArgumentNullException>(() => this.context.DeepInsert<Person>(null));
        }

        [Fact]
        public void CallingDeepInsertRequest_WithNullArguments_ShouldThrowAnException()
        {
            var result = new DeepInsertSaveResult(this.context, Util.SaveChangesMethodName, SaveChangesOptions.DeepInsert, null, null);
            Assert.Throws<ArgumentNullException>(() => result.DeepInsertRequest<Person>(null));
        }

        [Fact]
        public async Task CallingDeepInsertAsyncRequest_WithUntrackedEntity_ShouldThrowAnInvalidException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => this.context.DeepInsertAsync<Person>(this.person));
        }

        [Fact]
        public void DeepInsertAnEntry_WithOneLevelOfNesting()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponse.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithOneLevelOfNesting()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponse.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
        }

        [Fact]
        public void DeepInsertAnEntry_WithComplexProperty()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\",\"Registration\":{\"Date\":\"2023-11-06T00:00:00+00:00\",\"LicensePlate\":\"DL-172238\"},\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.carWithRegistration);
            this.context.AddRelatedObject(this.carWithRegistration, "Manufacturers", manufacturer);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal("DL-172238", returnedCar.Registration.LicensePlate);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithComplexProperty()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\",\"Registration\":{\"Date\":\"2023-11-06T00:00:00+00:00\",\"LicensePlate\":\"DL-172238\"},\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.carWithRegistration);
            this.context.AddRelatedObject(this.carWithRegistration, "Manufacturers", manufacturer);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal("DL-172238", returnedCar.Registration.LicensePlate);
        }

        [Fact]
        public void DeepInsertAnEntry_WithOneLevelOfNestingAndServerGeneratedId()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.personWithoutId);
            this.context.AddRelatedObject(this.personWithoutId, "Cars", this.carWithoutId);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.personWithoutId);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponse.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(100, returnedPerson.ID);
            Assert.Equal(1001, returnedCar.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithOneLevelOfNestingAndServerGeneratedId()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.personWithoutId);
            this.context.AddRelatedObject(this.personWithoutId, "Cars", this.carWithoutId);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.personWithoutId);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponse.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(100, returnedPerson.ID);
            Assert.Equal(1001, returnedCar.ID);
        }

        [Fact]
        public void DeepInsertAnEntry_WithNestedResponseWithNoRelatedDescriptors()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(Countries())))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Empty(response.Single().NestedResponses);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithNestedResponseWithNoRelatedDescriptors()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(Countries())))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Empty(response.Single().NestedResponses);
        }

        [Fact]
        public void DeepInsertAnEntry_WithThreeLevelsOfNesting()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(Countries())))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var country = new Country { ID = 101, Name = "CountryA" };

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(manufacturer, "Countries", country);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseCountries = nestedResponseManufacturers.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor countryDescriptor = nestedResponseCountries.Descriptor as EntityDescriptor;
            var returnedCountry = countryDescriptor.Entity as Country;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedCountry.ID);
            Assert.Single(nestedResponseCars.NestedResponses);
            Assert.Single(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseCountries.NestedResponses);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithThreeLevelsOfNesting()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(Countries())))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var country = new Country { ID = 101, Name = "CountryA" };
            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(manufacturer, "Countries", country);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseCountries = nestedResponseManufacturers.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor countryDescriptor = nestedResponseCountries.Descriptor as EntityDescriptor;
            var returnedCountry = countryDescriptor.Entity as Country;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedCountry.ID);
            Assert.Single(nestedResponseCars.NestedResponses);
            Assert.Single(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseCountries.NestedResponses);
        }

        [Fact]
        public void DeepInsertAnEntry_WithMultipleRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(),Owners()))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":\"Car 1001\",\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\"}],\"Owners\":[{\"ID\":101,\"Name\":\"Car owner\"}]},{\"ID\":1002,\"Name\":\"Car 1002\"}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var owner = new Person { ID = 101, Name = "Car owner" };

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(this.car, "Owners", owner);
            this.context.AddRelatedObject(this.person, "Cars", this.car2);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Equal(2, response.Single().NestedResponses.Count);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCar1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor car1Descriptor = nestedResponseCar1.Descriptor as EntityDescriptor;
            var returnedCar1 = car1Descriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCar1.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseOwners = nestedResponseCar1.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor ownerDescriptor = nestedResponseOwners.Descriptor as EntityDescriptor;
            var returnedOwner = ownerDescriptor.Entity as Person;

            var nestedResponseCar2 = changeOperationResponse.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor car2Descriptor = nestedResponseCar2.Descriptor as EntityDescriptor;
            var returnedCar2 = car2Descriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedOwner.ID);
            Assert.Equal(2, nestedResponseCar1.NestedResponses.Count);
            Assert.Empty(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseOwners.NestedResponses);
            Assert.Equal(1002, returnedCar2.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithMultipleRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(),Owners()))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":\"Car 1001\",\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\"}],\"Owners\":[{\"ID\":101,\"Name\":\"Car owner\"}]},{\"ID\":1002,\"Name\":\"Car 1002\"}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var owner = new Person { ID = 101, Name = "Car owner" };

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(this.car, "Owners", owner);
            this.context.AddRelatedObject(this.person, "Cars", this.car2);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Equal(2, response.Single().NestedResponses.Count);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCar1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor car1Descriptor = nestedResponseCar1.Descriptor as EntityDescriptor;
            var returnedCar1 = car1Descriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCar1.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseOwners = nestedResponseCar1.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor ownerDescriptor = nestedResponseOwners.Descriptor as EntityDescriptor;
            var returnedOwner = ownerDescriptor.Entity as Person;

            var nestedResponseCar2 = changeOperationResponse.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor car2Descriptor = nestedResponseCar2.Descriptor as EntityDescriptor;
            var returnedCar2 = car2Descriptor.Entity as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedOwner.ID);
            Assert.Equal(2, nestedResponseCar1.NestedResponses.Count);
            Assert.Empty(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseOwners.NestedResponses);
            Assert.Equal(1002, returnedCar2.ID);
        }

        [Fact]
        public void DeepInsertAnEntry_WithComplexNullPropertyAndMultipleRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(),Owners()))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":\"Car 1001\",\"Registration\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\"}],\"Owners\":[{\"ID\":101,\"Name\":\"Car owner\"}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var owner = new Person { ID = 101, Name = "Car owner" };

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(this.car, "Owners", owner);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseOwners = nestedResponseCars.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor ownerDescriptor = nestedResponseOwners.Descriptor as EntityDescriptor;
            var returnedOwner = ownerDescriptor.Entity as Person;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedOwner.ID);
            Assert.Equal(2, nestedResponseCars.NestedResponses.Count);
            Assert.Empty(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseOwners.NestedResponses);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithComplexNullPropertyAndMultipleRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars(Manufacturers(),Owners()))/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":1001,\"Name\":\"Car 1001\",\"Registration\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\"}],\"Owners\":[{\"ID\":101,\"Name\":\"Car owner\"}]}]}";
            SetupDataServiceContext(expectedResponse);

            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var owner = new Person { ID = 101, Name = "Car owner" };

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);
            this.context.AddRelatedObject(this.car, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(this.car, "Owners", owner);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponseCars.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            var nestedResponseManufacturers = nestedResponseCars.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor manuDescriptor = nestedResponseManufacturers.Descriptor as EntityDescriptor;
            var returnedManufacturer = manuDescriptor.Entity as Manufacturer;

            var nestedResponseOwners = nestedResponseCars.NestedResponses[1] as ChangeOperationResponse;
            EntityDescriptor ownerDescriptor = nestedResponseOwners.Descriptor as EntityDescriptor;
            var returnedOwner = ownerDescriptor.Entity as Person;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
            Assert.Equal(11, returnedManufacturer.ID);
            Assert.Equal(101, returnedOwner.ID);
            Assert.Equal(2, nestedResponseCars.NestedResponses.Count);
            Assert.Empty(nestedResponseManufacturers.NestedResponses);
            Assert.Empty(nestedResponseOwners.NestedResponses);
        }

        [Fact]
        public void DeepInsertAnEntry_WithMultipleLinks()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"},{\"ID\":\"1002\",\"Name\":\"Car 1002\"}]}";
            SetupDataServiceContext(expectedResponse);

            var car1 = new Car { ID = 1001, Name = "Car 1001" };
            var car2 = new Car { ID = 1002, Name = "Car 1002" };

            this.context.AddObject("Persons", this.person);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.AddLink(this.person, "Cars", car1);
            this.context.AddLink(this.person, "Cars", car2);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Equal(2, response.Single().NestedResponses.Count);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            LinkDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as LinkDescriptor;
            var returnedCar1 = carDescriptor1.Target as Car;

            var nestedResponseCars2 = changeOperationResponse.NestedResponses[1] as ChangeOperationResponse;
            LinkDescriptor carDescriptor2 = nestedResponseCars2.Descriptor as LinkDescriptor;
            var returnedCar2 = carDescriptor2.Target as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
            Assert.Equal(1002, returnedCar2.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithMultipleLinks()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":\"1001\",\"Name\":\"Car 1001\"},{\"ID\":\"1002\",\"Name\":\"Car 1002\"}]}";
            SetupDataServiceContext(expectedResponse);

            var car1 = new Car { ID = 1001, Name = "Car 1001" };
            var car2 = new Car { ID = 1002, Name = "Car 1002" };

            this.context.AddObject("Persons", this.person);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.AddLink(this.person, "Cars", car1);
            this.context.AddLink(this.person, "Cars", car2);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Equal(2, response.Single().NestedResponses.Count);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            LinkDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as LinkDescriptor;
            var returnedCar1 = carDescriptor1.Target as Car;

            var nestedResponseCars2 = changeOperationResponse.NestedResponses[1] as ChangeOperationResponse;
            LinkDescriptor carDescriptor2 = nestedResponseCars2.Descriptor as LinkDescriptor;
            var returnedCar2 = carDescriptor2.Target as Car;

            Assert.Equal("Person 100", returnedPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
            Assert.Equal(1002, returnedCar2.ID);
        }

        [Fact]
        public void DeepInsertAnEntry_WithNoRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons/$entity\",\"ID\":100,\"Name\":\"Person 100\"}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            DataServiceResponse response = this.context.DeepInsert<Person>(this.person);

            Assert.Single(response);
            Assert.Empty(response.Single().NestedResponses);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithNoRelatedObjects()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons/$entity\",\"ID\":100,\"Name\":\"Person 100\"}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            DataServiceResponse response = await this.context.DeepInsertAsync<Person>(this.person);

            Assert.Single(response);
            Assert.Empty(response.Single().NestedResponses);
        }

        [Fact]
        public void DeepInsertAnEntry_WithSingleValueNavigation()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#VipPersons(VipCar())/$entity\",\"ID\":100,\"Name\":\"VipPerson 100\",\"VipCar\":{\"ID\":1001,\"Name\":null}}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("VipPersons", this.vipPerson);
            this.context.SetRelatedObject(this.vipPerson, "VipCar", this.car);
            DataServiceResponse response = this.context.DeepInsert<VipPerson>(this.vipPerson);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedVipPerson = entityDescriptor.Entity as VipPerson;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as EntityDescriptor;
            var returnedCar1 = carDescriptor1.Entity as Car;

            Assert.Equal("VipPerson 100", returnedVipPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithSingleValueNavigation()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#VipPersons(VipCar())/$entity\",\"ID\":100,\"Name\":\"VipPerson 100\",\"VipCar\":{\"ID\":1001,\"Name\":null}}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("VipPersons", this.vipPerson);
            this.context.SetRelatedObject(this.vipPerson, "VipCar", this.car);
            DataServiceResponse response = await this.context.DeepInsertAsync<VipPerson>(this.vipPerson);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedVipPerson = entityDescriptor.Entity as VipPerson;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as EntityDescriptor;
            var returnedCar1 = carDescriptor1.Entity as Car;

            Assert.Equal("VipPerson 100", returnedVipPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
        }

        [Fact]
        public void DeepInsertAnEntry_WithSingleValueNavigationLink()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#VipPersons(VipCar())/$entity\",\"ID\":100,\"Name\":\"VipPerson 100\",\"VipCar\":{\"ID\":1001,\"Name\":null}}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("VipPersons", this.vipPerson);
            this.context.AttachTo("Cars", this.car);
            this.context.SetRelatedObjectLink(this.vipPerson, "VipCar", this.car);
            DataServiceResponse response = this.context.DeepInsert<VipPerson>(this.vipPerson);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedVipPerson = entityDescriptor.Entity as VipPerson;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            LinkDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as LinkDescriptor;
            var returnedCar1 = carDescriptor1.Target as Car;

            Assert.Equal("VipPerson 100", returnedVipPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
        }

        [Fact]
        public async Task DeepInsertAsyncAnEntry_WithSingleValueNavigationLink()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#VipPersons(VipCar())/$entity\",\"ID\":100,\"Name\":\"VipPerson 100\",\"VipCar\":{\"ID\":1001,\"Name\":null}}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("VipPersons", this.vipPerson);
            this.context.AttachTo("Cars", this.car);
            this.context.SetRelatedObjectLink(this.vipPerson, "VipCar", this.car);

            DataServiceResponse response = await this.context.DeepInsertAsync<VipPerson>(this.vipPerson);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedVipPerson = entityDescriptor.Entity as VipPerson;

            var nestedResponseCars1 = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            LinkDescriptor carDescriptor1 = nestedResponseCars1.Descriptor as LinkDescriptor;
            var returnedCar1 = carDescriptor1.Target as Car;

            Assert.Equal("VipPerson 100", returnedVipPerson.Name);
            Assert.Equal(1001, returnedCar1.ID);
        }

        [Fact]
        public void HandleResponse_Of_AnInvalidResponse_ThrowsException()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:5000/$metadata#Persons(Cars())/$entity\",\"ID\":100,\"Name\":\"Person 100\",\"Cars\":[{\"ID\":null,\"Name\":null}]}";
            SetupDataServiceContext(expectedResponse);

            this.context.AddObject("Persons", this.person);
            this.context.AddRelatedObject(this.person, "Cars", this.car);

            Assert.Throws<DataServiceRequestException>(() => this.context.DeepInsert<Person>(person));
        }

        [Fact]
        public void DeepInsertAnEntry_WithANestedDelete_Fails()
        {
            this.context.AttachTo("Persons", this.person);
            this.context.AttachTo("Cars", this.car);
            this.context.DeleteLink(this.person, "Cars", this.car);

            Action action = () =>
            {
                this.context.DeepInsert<Person>(this.person);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_DeepInsertDeletedOrModified("Cars(1002)"));
        }

        [Fact]
        public void DeepInsertAnEntry_WithANestedUpdate_Fails()
        {
            var cars = new List<Car>
            {
                new Car
                {
                    ID = 200,
                    Name = "Car 1",
                }
            };

            this.context.AddObject("Persons", this.person);
            this.context.AttachTo("Cars", cars[0]);

            DataServiceCollection<Car> carsCollection = new DataServiceCollection<Car>(this.context);
            carsCollection.Load(cars);
            carsCollection[0].Name = "Updated car";

            this.context.AddLink(person, "Cars", carsCollection[0]);

            Action action = () =>
            {
                this.context.DeepInsert<Person>(this.person);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_DeepInsertDeletedOrModified("Cars(200)"));
        }

        [Fact]
        public async Task CallingDeepInsertAsync_WithNullArguments_ShouldThrowAnException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.context.DeepInsertAsync<Person>(null));
        }

        private void SetupContextWithRequestPipelineForSaving(DataServiceContext dataServiceContext, string response, Dictionary<string, string> headers)
        {
            dataServiceContext.Configurations.RequestPipeline.OnMessageCreating =
                    (args) => new CustomizedRequestMessage(
                        args,
                        response,
                        headers);
        }

        public class CustomizedRequestMessage : HttpClientRequestMessage
        {
            public string Response { get; set; }

            public Dictionary<string, string> CustomizedHeaders { get; set; }

            public CustomizedRequestMessage(DataServiceClientRequestMessageArgs args, string response, Dictionary<string, string> headers)
                : base(args)
            {
                this.Response = response;
                this.CustomizedHeaders = headers;
            }

            public override Stream GetStream()
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(Response);
                return new MemoryStream(byteArray);
            }

            public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
            {
                return GetCompletedTask(callback, state);
            }

            private static IAsyncResult GetCompletedTask(AsyncCallback callback, object state)
            {
                var tcs = new TaskCompletionSource<bool>(state);
                tcs.TrySetResult(true);
                callback(tcs.Task);
                return tcs.Task;
            }

            public override Stream EndGetRequestStream(IAsyncResult asyncResult)
            {
                return GetStream();
            }

            public override IODataResponseMessage GetResponse()
            {
                return new HttpWebResponseMessage(
                    this.CustomizedHeaders,
                    200,
                    () =>
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(this.Response);
                        return new MemoryStream(byteArray);
                    });
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                return GetCompletedTask(callback, state);
            }

            public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
            {
                return GetResponse();
            }
        }

        public class Container : DataServiceContext
        {
            public Container(global::System.Uri serviceRoot) :
                base(serviceRoot, ODataProtocolVersion.V4)
            {
                this.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                this.Format.UseJson();
                this.Persons = base.CreateQuery<Person>("Persons");
                this.Cars = base.CreateQuery<Car>("Cars");
                this.Manufacturers = base.CreateQuery<Manufacturer>("Manufacturers");
                this.Countries = base.CreateQuery<Country>("Countries");
            }
            public DataServiceQuery<Person> Persons { get; private set; }
            public DataServiceQuery<Car> Cars { get; private set; }
            public DataServiceQuery<Manufacturer> Manufacturers { get; private set; }
            public DataServiceQuery<Country> Countries { get; private set; }
        }

        public class Registration : INotifyPropertyChanged
        {
            private DateTimeOffset _date;
            public DateTimeOffset Date
            {
                get
                {
                    return _date;
                }
                set
                {
                    if (value != _date)
                    {
                        _date = value;
                        OnPropertyChanged("Date");
                    }
                }
            }

            private string _licensePlate;
            public string LicensePlate
            {
                get
                {
                    return _licensePlate;
                }
                set
                {
                    if (value != _licensePlate)
                    {
                        _licensePlate = value;
                        OnPropertyChanged("LicensePlate");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }

        }

        public class Car : INotifyPropertyChanged
        {
            private string _name;
            public int ID { get; set; }
            public string Name
            {
                get
                {
                    return _name;
                }

                set
                {
                    if (value != _name)
                    {
                        _name = value;
                        OnPropertyChanged("Name");
                    }
                }
            }
            public Person Owner { get; set; }
            public List<Person> Owners { get; set; }
            public List<Manufacturer> Manufacturers { get; set; }

            private Registration _registration;
            public Registration Registration
            {
                get
                {
                    return _registration;
                }
                set
                {
                    if (_registration != value)
                    {
                        _registration = value;
                        OnPropertyChanged("Registration");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        public class Manufacturer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public List<Country> Countries { get; set; }
        }

        public class Country
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class Person : INotifyPropertyChanged
        {
            private string _name;
            public Person()
            {
                this.Cars = new List<Car>();
            }

            public int ID { get; set; }
            public string Name
            {
                get
                {
                    return _name;
                }

                set
                {
                    if (value != _name)
                    {
                        _name = value;
                        OnPropertyChanged("Name");
                    }
                }
            }
            public List<Car> Cars { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        public class VipPerson
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Car VipCar { get; set; }
        }
    }
}
