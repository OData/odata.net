//---------------------------------------------------------------------
// <copyright file="BulkUpdateE2ETests.cs" company="Microsoft">
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
    /// BulkUpdate E2E tests.
    /// </summary>
    public class BulkUpdateE2ETests
    {
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
    <edmx:DataServices>
        <Schema Namespace=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityType Name=""Person"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
                <NavigationProperty Name=""Cars"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car)""/>
            </EntityType>
            <EntityType Name=""Car"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false""/>
                <NavigationProperty Name=""Owner"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person""/>
                <NavigationProperty Name=""Owners"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person)""/>
                <NavigationProperty Name=""Manufacturers"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Manufacturer)""/>
            </EntityType>
            <EntityType Name=""Manufacturer"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
                <NavigationProperty Name=""Countries"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Country)""/>
            </EntityType>
            <EntityType Name=""Country"">
                <Key>
                    <PropertyRef Name=""ID""/>
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String""/>
            </EntityType>
        </Schema>
        <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""Persons"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person"">
                    <NavigationPropertyBinding Path=""Cars"" Target=""Cars""/>
                </EntitySet>
                <EntitySet Name=""Cars"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car"">
                    <NavigationPropertyBinding Path=""Owners"" Target=""Persons""/>
                    <NavigationPropertyBinding Path=""Manufacturers"" Target=""Manufacturers""/>
                </EntitySet>
                <EntitySet Name=""Manufacturers"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Manufacturer"">
                    <NavigationPropertyBinding Path=""Countries"" Target=""Countries""/>                 
                </EntitySet>
                <EntitySet Name=""Countries"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Country""/>
            </EntityContainer>
        </Schema>
    </edmx:DataServices></edmx:Edmx>";

        private const string persons = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#Persons"",
            ""value"":[
                {""ID"":100,""Name"":""Bing"", ""Cars"":[{""ID"":1001,""Name"":""CarA"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerA""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]},
                {""ID"":200,""Name"":""Edge"", ""Cars"":[{""ID"":1002,""Name"":""CarB"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerB""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]},
                {""ID"":300,""Name"":""Teams"", ""Cars"":[{""ID"":1003,""Name"":""CarC"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerC""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}]}
            ]}";

        private const string cars = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#Cars"",
            ""value"":[
                {""ID"":1001,""Name"":""CarA"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerA""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]},
                {""ID"":1002,""Name"":""CarB"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerB""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]},
                {""ID"":1003,""Name"":""CarC"",""Manufacturers"":[{""ID"":101,""Name"":""ManufacturerC""}], ""Owners"":[{""ID"":1,""Name"":""OwnerA""}]}
            ]}";

        private readonly Container context;
        private readonly RequestInfo requestInfo;
        private readonly Serializer serializer;
        private readonly HeaderCollection headers;
        private Dictionary<Descriptor, List<LinkDescriptor>> linkDescriptors;

        public BulkUpdateE2ETests()
        {
            this.context = new Container(new Uri("http://localhost:8000"));
            this.requestInfo = new RequestInfo(context);
            this.serializer = new Serializer(this.requestInfo);
            this.headers = new HeaderCollection();
            this.linkDescriptors = new Dictionary<Descriptor, List<LinkDescriptor>>();
        }

        [Fact]
        public void CallingBulkUpdate_WithNullArguments_ShouldThrowAnException()
        {
            Assert.Throws<ArgumentException>(() => this.context.BulkUpdate<Person>(null));
        }

        [Fact]
        public void CallingBulkUpdateRequest_WithNullArguments_ShouldThrowAnException()
        {
            var result = new BulkUpdateSaveResult(this.context, Util.SaveChangesMethodName, SaveChangesOptions.BulkUpdate, null, null);
            Assert.Throws<ArgumentException>(() => result.BulkUpdateRequest<Person>(null));
        }

        [Fact]
        public async Task CallingBulkUpdateAsync_WithNullArguments_ShouldThrowAnException()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await this.context.BulkUpdateAsync<Person>(null));
        }

        [Fact]
        public void BulkUpdateAnEntry_WithOneLevelOfNesting_ReturnsOne_OperationResponse()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":\"1001\",\"Name\":\"A\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car = new Car 
            { 
                ID = 1001,
                Name = "A"
            };

            this.context.AttachTo("Persons", person);

            this.context.AddRelatedObject(person, "Cars", car);

            DataServiceResponse response = this.context.BulkUpdate<Person>(person);
 
            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            EntityDescriptor carDescriptor = nestedResponse.Descriptor as EntityDescriptor;
            var returnedCar = carDescriptor.Entity as Car;

            Assert.Equal("Bing", returnedPerson.Name);
            Assert.Equal(1001, returnedCar.ID);
        }

        [Fact(Skip ="The entity instance can be set only once.")]
        public async Task BulkUpdateAsync_ShouldThrowExceptions_RaisedDuringSeriliazation()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\"}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            this.context.AttachTo("Persons", person);

            var entitydesc = this.context.Entities[0] as EntityDescriptor;
            
            entitydesc.Entity = null;

            await Assert.ThrowsAsync<NullReferenceException>(() => this.context.BulkUpdateAsync(person));
        }

        [Fact]
        public void HandleResponse_Of_AnInvalidResponse_ThrowsException()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":null,\"Name\":null}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car = new Car
            {
                ID = 1001,
                Name = "A"
            };

            this.context.AttachTo("Persons", person);

            this.context.AddRelatedObject(person, "Cars", car);

            Assert.Throws<DataServiceRequestException>(() => this.context.BulkUpdate<Person>(person));
        }

        [Fact]
        public async Task AsyncHandlingOf_AnInvalidResponse_ThrowsException()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":null,\"Name\":null}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car = new Car
            {
                ID = 1001,
                Name = "A"
            };

            this.context.AttachTo("Persons", person);

            this.context.AddRelatedObject(person, "Cars", car);

            var exception = await Assert.ThrowsAsync<DataServiceRequestException>(async () => await this.context.BulkUpdateAsync<Person>(person));

            var exceptionMessage = "An error occurred while processing this request.";
            var expectedInnerException = "The key property 'ID' on type 'Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car' has a null value. Key properties must not have null values.";
            
            Assert.Empty(exception.Response);
            Assert.Equal(expectedInnerException, exception.InnerException.Message);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void BulkUpdateAnEntry_WithANestedDelete_DeserializesSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car = new Car
            {
                ID = 1002,
            };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Cars", car);
            this.context.DeleteLink(person, "Cars", car);

            DataServiceResponse response = this.context.BulkUpdate<Person>(person);

            Assert.Single(response);
            Assert.Single(response.Single().NestedResponses);

            var changeOperationResponse = response.First() as ChangeOperationResponse;
            EntityDescriptor entityDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var returnedPerson = entityDescriptor.Entity as Person;
            var nestedResponse = changeOperationResponse.NestedResponses[0] as ChangeOperationResponse;
            LinkDescriptor carDescriptor = nestedResponse.Descriptor as LinkDescriptor;
            var returnedCar = carDescriptor.Target as Car;

            Assert.Equal("Bing", returnedPerson.Name);
            Assert.Equal(1002, returnedCar.ID);
        }

        [Fact]
        public void BulkUpdateTwoTopLevelObjects_ReturnsTwo_OperationResponses()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\"},{\"ID\":200,\"Name\":\"Edge\"}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge"
            };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);

            DataServiceResponse response = this.context.BulkUpdate(person, person2);
            var personOperationResponse = response.First() as ChangeOperationResponse;
            var person1 = (personOperationResponse.Descriptor as EntityDescriptor).Entity as Person;

            var person2OperationResponse = response.Last() as ChangeOperationResponse;
            var person2Response = (person2OperationResponse.Descriptor as EntityDescriptor).Entity as Person;

            Assert.Equal(2, response.Count());
            Assert.Equal("Bing", person1.Name);
            Assert.Equal("Edge", person2Response.Name);
        }

        [Fact]
        public void BulkUpdateTwoTopLevelObjects_WithTheSameNestedObject_DeserializesSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]},{\"ID\":200,\"Name\":\"Edge\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge"
            };

            var car = new Car
            {
                ID = 1002,
                Name = "CarA"
            };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car);

            this.context.AddLink(person, "Cars", car);
            this.context.AddLink(person2, "Cars", car);

            DataServiceResponse response = this.context.BulkUpdate(person, person2);
            var personOperationResponse = response.First() as ChangeOperationResponse;
            var person1 = (personOperationResponse.Descriptor as EntityDescriptor).Entity as Person;
            var personcarOperationResponse = personOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var carTargetEntity = (personcarOperationResponse.Descriptor as LinkDescriptor).Target as Car;  
            var person2OperationResponse = response.Last() as ChangeOperationResponse;
            var person2Response = (person2OperationResponse.Descriptor as EntityDescriptor).Entity as Person;
            var person2carOperationResponse = personOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var car2TargetEntity = (person2carOperationResponse.Descriptor as LinkDescriptor).Target as Car;

            Assert.Equal(2, response.Count());
            Assert.Equal("Bing", person1.Name);
            Assert.Equal("Edge", person2Response.Name);
            Assert.Equal(carTargetEntity, car2TargetEntity);
        }

        [Fact]
        public async Task BulkUpdateAsyncTwoTopLevelObjects_WithTheSameNestedObject_DeserializesSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]},{\"ID\":200,\"Name\":\"Edge\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge"
            };

            var car = new Car
            {
                ID = 1002,
                Name = "CarA"
            };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car);

            this.context.AddLink(person, "Cars", car);
            this.context.AddLink(person2, "Cars", car);

            DataServiceResponse response = await this.context.BulkUpdateAsync(person, person2);

            var personOperationResponse = response.First() as ChangeOperationResponse;
            var person1 = (personOperationResponse.Descriptor as EntityDescriptor).Entity as Person;
            var personcarOperationResponse = personOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var carTargetEntity = (personcarOperationResponse.Descriptor as LinkDescriptor).Target as Car;
            var person2OperationResponse = response.Last() as ChangeOperationResponse;
            var person2Response = (person2OperationResponse.Descriptor as EntityDescriptor).Entity as Person;
            var person2carOperationResponse = personOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var car2TargetEntity = (person2carOperationResponse.Descriptor as LinkDescriptor).Target as Car;

            Assert.Equal(2, response.Count());
            Assert.Equal("Bing", person1.Name);
            Assert.Equal("Edge", person2Response.Name);
            Assert.Equal(carTargetEntity, car2TargetEntity);
        }

        [Fact]
        public void TwoTopLevelObjects_WithNestedProperties_UpdatedSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]},{\"ID\":200,\"Name\":\"Edge\",\"Cars@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}, {\"@id\":\"http://www.odata.org/service.svc/Cars(1003)\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge"
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };
            var car3 = new Car { ID = 1003 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.AttachTo("Cars", car3);

            this.context.AddLink(person, "Cars", car1);
            this.context.AddLink(person2, "Cars", car2);
            this.context.AddLink(person2, "Cars", car3);

            DataServiceResponse response = this.context.BulkUpdate(person, person2);

            Assert.Equal(2, response.Count());

            var personOperationResponse = response.FirstOrDefault() as ChangeOperationResponse;
            var person2OperationResponse = response.LastOrDefault() as ChangeOperationResponse;

            Assert.Single(personOperationResponse.NestedResponses);
            Assert.Equal(2, person2OperationResponse.NestedResponses.Count);
        }

        [Fact]
        public async Task TwoTopLevelObjects_WithNestedProperties_UpdatedSuccessfullyAsync()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://localhost:8000/Cars(1001)\"}]},{\"ID\":200,\"Name\":\"Edge\",\"Cars@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}, {\"@id\":\"http://www.odata.org/service.svc/Cars(1003)\"}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge"
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };
            var car3 = new Car { ID = 1003 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.AttachTo("Cars", car3);

            this.context.AddLink(person, "Cars", car1);
            this.context.AddLink(person2, "Cars", car2);
            this.context.AddLink(person2, "Cars", car3);

            DataServiceResponse response = await this.context.BulkUpdateAsync(person, person2);

            Assert.Equal(2, response.Count());

            var personOperationResponse = response.FirstOrDefault() as ChangeOperationResponse;
            var person2OperationResponse = response.LastOrDefault() as ChangeOperationResponse;

            Assert.Single(personOperationResponse.NestedResponses);
            Assert.Equal(2, person2OperationResponse.NestedResponses.Count);
        }

        [Fact]
        public void DeepUpdateAnElement_WithThreeLevelsOfNesting_UpdatesSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":1001,\"Name\":\"CarA\",\"Manufacturers@delta\":[{\"ID\":101,\"Name\":\"ManufactureA\"}]}]}]}";

            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car = new Car { ID = 1001, Name ="CarA" };
            var manufacturer = new Manufacturer { ID = 101, Name = "ManufactureA" };

            this.context.AttachTo("Persons", person);
            this.context.AddRelatedObject(person, "Cars", car);
            this.context.AddRelatedObject(car, "Manufacturers", manufacturer);

            DataServiceResponse response = this.context.BulkUpdate(person);

            Assert.Single(response);

            var personOperationResponse = response.FirstOrDefault() as ChangeOperationResponse;
            var person2OperationResponse = response.LastOrDefault() as ChangeOperationResponse;
            var carOperationResponse = personOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var manufacturerOperationResponse = carOperationResponse.NestedResponses.FirstOrDefault() as ChangeOperationResponse;
            var manufacturerDescriptor = manufacturerOperationResponse.Descriptor as EntityDescriptor;
            var manufacturerEntity = manufacturerDescriptor.Entity as Manufacturer;

            Assert.Single(personOperationResponse.NestedResponses);
            Assert.Single(person2OperationResponse.NestedResponses);
            Assert.Equal("ManufactureA", manufacturerEntity.Name);
        }

        [Fact]
        public void DescriptorForAFailedCreateOperation_NotUpdated()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"@removed\": {\"reason\": \"changed\"},\"@id\": \"http://localhost:8000/Persons(100)\",\"@Core.DataModificationException\": {\"@type\": \"#Org.OData.Core.V1.DataModificationExceptionType\"},\"Id\": 100,\"Name\": \"Bing\"}]}";
            
            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            this.context.AttachTo("Persons", person);

            DataServiceResponse response = this.context.BulkUpdate(person);

            var personChangeOperationResponse = response.FirstOrDefault() as ChangeOperationResponse;

            Assert.Equal(EntityStates.Unchanged, personChangeOperationResponse.Descriptor.SaveResultWasProcessed);
        }

        [Fact]
        public void DescriptorsWithNullEntitySetName_ShouldBeSerializedSuccessfully()
        {
            BulkUpdateGraph bulkUpdateGraph = null;

            SetupContextWithRequestPipelineForSaving(
                this.context,
                persons);

            var person = this.context.Persons.FirstOrDefault();

            var personEntityDescriptor = this.context.GetEntityDescriptor(person);

            Assert.Null(personEntityDescriptor.EntitySetName);

            bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, isParameterPayload:false))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, bulkUpdateGraph.EntitySetName, requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person\",\"ID\":100,\"Name\":\"Bing\"}]}");
        }

        [Fact]
        public void DeepUpdate_WithOneLevelOfNesting_ShouldBeSerializedSuccessfully()
        {
            BulkUpdateGraph bulkUpdateGraph = null;

            SetupContextWithRequestPipelineForSaving(
                this.context,
                persons);

            var person = this.context.Persons.FirstOrDefault();
            var car = person.Cars[0];

            car.Name = "UpdatedCar";

            this.context.UpdateObject(car);

            bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, isParameterPayload: false))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, bulkUpdateGraph.EntitySetName, requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person\",\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car\",\"ID\":1001,\"Name\":\"UpdatedCar\"}]}]}");
        }

        [Fact]
        public void DeepUpdate_WithTwoLevelsOfNesting_ShouldBeSerializedSuccessfully()
        {
            BulkUpdateGraph bulkUpdateGraph = null;

            SetupContextWithRequestPipelineForSaving(
                this.context,
                persons);

            var person = this.context.Persons.FirstOrDefault();
            var car = person.Cars[0];

            car.Name = "UpdatedCar";

            this.context.UpdateObject(car);

            var manufacturer = car.Manufacturers[0];
            manufacturer.Name = "UpdatedManufacturer";

            this.context.UpdateObject(manufacturer);

            bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, isParameterPayload: false))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, bulkUpdateGraph.EntitySetName, requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person\",\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car\",\"ID\":1001,\"Name\":\"UpdatedCar\",\"Manufacturers@delta\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Manufacturer\",\"ID\":101,\"Name\":\"UpdatedManufacturer\"}]}]}]}");
        }

        [Fact]
        public void DeepUpdateAnEntry_WithTwoNestedProperties_ShouldBeSerializedSuccessfully()
        {
            BulkUpdateGraph bulkUpdateGraph = null;

            SetupContextWithRequestPipelineForSaving(
                this.context,
                cars);

            var car = this.context.Cars.FirstOrDefault();
            var manufacturer = car.Manufacturers[0];

            manufacturer.Name = "UpdatedManufacturer";

            this.context.UpdateObject(manufacturer);

            var owner = car.Owners[0];
            owner.Name = "UpdatedOwner";

            this.context.UpdateObject(owner);

            bulkUpdateGraph = this.GetBulkUpdateGraph(car);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, isParameterPayload: false))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, bulkUpdateGraph.EntitySetName, requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://localhost:8000/$metadata#Cars/$delta\",\"value\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Car\",\"ID\":1001,\"Name\":\"CarA\",\"Manufacturers@delta\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Manufacturer\",\"ID\":101,\"Name\":\"UpdatedManufacturer\"}],\"Owners@delta\":[{\"@type\":\"#Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person\",\"ID\":1,\"Name\":\"UpdatedOwner\"}]}]}");
        }

        [Fact]
        public void UpdatedDescriptors_ShouldBeDerializedSuccessfully()
        {
            var expectedResponse = "{\"@context\":\"http://localhost:8000/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"UpdatedName\"}]}";
            
            SetupContextWithRequestPipelineForSaving(
                this.context,
                expectedResponse);

            var person = new Person
            {
                ID = 100,
                Name = "Bing"
            };

            this.context.AttachTo("Persons", person);

            //update person name
            person.Name = "UpdatedName";

            this.context.UpdateObject(person);

            // bulk update the changes
            DataServiceResponse response = this.context.BulkUpdate(person);

            var trackedEntity = this.context.Entities.First().Entity as Person;

            Assert.Equal("UpdatedName", trackedEntity.Name);

            var personChangeOperationResponse = response.FirstOrDefault() as ChangeOperationResponse;
            var personDescriptor = personChangeOperationResponse.Descriptor as EntityDescriptor;
            var personObj = personDescriptor.Entity as Person;

            Assert.Equal(EntityStates.Modified, personChangeOperationResponse.Descriptor.SaveResultWasProcessed);
            Assert.Equal("UpdatedName", personObj.Name);
        }

        [Fact]
        public void CreateForDeltaFeed_WithNulEntitySetName_ShouldThrowException()
        {
            BulkUpdateGraph bulkUpdateGraph = null;

            SetupContextWithRequestPipelineForSaving(
                this.context,
                persons);

            var person = this.context.Persons.Execute().FirstOrDefault();

            var personEntityDescriptor = this.context.GetEntityDescriptor(person);

            Assert.Null(personEntityDescriptor.EntitySetName);

            bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, isParameterPayload: false))
            {
                Assert.Throws<ArgumentNullException>(() => ODataWriterWrapper.CreateForDeltaFeed(messageWriter, null, requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo));
            }
        }

        [Fact]
        public void MaterializerFeed_CreateDeltaFeed_CreatesAMaterializerDeltaFeed()
        {
            ODataDeltaResourceSet deltaFeed = new ODataDeltaResourceSet()
            {
                TypeName = "Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person)",
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Persons",
                    ExpectedTypeName = "Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person",
                    NavigationSourceEntityTypeName = "Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = this.context };
            
            List<IMaterializerState> entries = new List<IMaterializerState>();
            ODataResource entry = new ODataResource
            {
                Id = new Uri("http://localhost:8000/Persons(100)"),
                Properties = new List<ODataProperty>()
                    {
                        new ODataProperty{ Name = "ID", Value = "100"},
                        new ODataProperty { Name = "Name", Value = "Bing"}
                    }
            };

            MaterializerEntry materializerEntry = MaterializerEntry.CreateEntry(entry, ODataFormat.Json, true, clientEdmModel, materializerContext);
            entries.Add(materializerEntry);

            MaterializerDeltaFeed materializerFeed = MaterializerDeltaFeed.CreateDeltaFeed(deltaFeed, entries, materializerContext);
            Assert.IsType<ODataDeltaResourceSet>(materializerFeed.DeltaFeed);
            Assert.Single(materializerFeed.Entries);       
        }

        [Fact]
        public void CreateDeltaFeed_WithNullEntries_DoesNotThrowException()
        {
            ODataDeltaResourceSet deltaFeed = new ODataDeltaResourceSet()
            {
                TypeName = "Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person)",
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Persons",
                    ExpectedTypeName = "Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person",
                    NavigationSourceEntityTypeName = "Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = this.context };
            MaterializerDeltaFeed materializerFeed = MaterializerDeltaFeed.CreateDeltaFeed(deltaFeed, null, materializerContext);
            
            Assert.IsType<ODataDeltaResourceSet>(materializerFeed.DeltaFeed);
            Assert.Empty(materializerFeed.Entries);
        }

        [Fact]
        public void MaterializerNestedEntry_CreateNestedResourceInfo_CreatesAMaterializerNestedEntry()
        {
            var nestedResourceInfo = new ODataNestedResourceInfo 
            {
                Name = "Persons", 
                IsCollection = true, 
                Url = new Uri("Person", UriKind.Relative) 
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = this.context };

            List<IMaterializerState> entries = new List<IMaterializerState>();
            ODataResource entry = new ODataResource
            {
                Id = new Uri("http://localhost:8000/Persons(100)"),
                Properties = new List<ODataProperty>()
                    {
                        new ODataProperty{ Name = "ID", Value = "100"},
                        new ODataProperty { Name = "Name", Value = "Bing"}
                    }
            };

            MaterializerEntry materializerEntry = MaterializerEntry.CreateEntry(entry, ODataFormat.Json, true, clientEdmModel, materializerContext);
            entries.Add(materializerEntry);

            MaterializerNestedEntry nestedEntry = MaterializerNestedEntry.CreateNestedEntry(nestedResourceInfo, entries, materializerContext);
            Assert.IsType<ODataNestedResourceInfo>(nestedEntry.NestedResourceInfo);
            Assert.Single(nestedEntry.NestedItems);
        }

        [Fact]
        public void MaterializerNestedEntry_WithNullEntries_DoesNotThrowException()
        {
            var nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Persons",
                IsCollection = true,
                Url = new Uri("Person", UriKind.Relative)
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = this.context };
            
            MaterializerNestedEntry nestedEntry = MaterializerNestedEntry.CreateNestedEntry(nestedResourceInfo, null, materializerContext);

            Assert.IsType<ODataNestedResourceInfo>(nestedEntry.NestedResourceInfo);
            Assert.Empty(nestedEntry.NestedItems);
        }

        [Fact]
        public void MaterializerDeletedEntry_CreateDeletedEntry_CreatesAMaterializerDeletedEntry()
        {
            var deletedEntry = new ODataDeletedResource
                (new Uri("Person", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = "Collection(Microsoft.OData.Client.TDDUnitTests.Tests.BulkUpdateE2ETests.Person)",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceName = "Persons"
                },
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = this.context };


            MaterializerDeletedEntry materializerDeletedEntry = MaterializerDeletedEntry.CreateDeletedEntry(deletedEntry, materializerContext);
            Assert.IsType<ODataDeletedResource>(materializerDeletedEntry.DeletedResource);
        }

        private void SetupContextWithRequestPipelineForSaving(DataServiceContext dataServiceContext, string response)
        {
            dataServiceContext.Configurations.RequestPipeline.OnMessageCreating =
                    (args) => new CustomizedRequestMessage(
                        args,
                        response,
                        new Dictionary<string, string>()
                        {
                            {"Content-Type", "application/json;charset=utf-8"},
                        });
        }

        private BulkUpdateGraph GetBulkUpdateGraph<T>(params T[] objects)
        {
            BulkUpdateGraph bulkUpdateGraph = new BulkUpdateGraph(this.requestInfo);
            
            List<Descriptor> changedEntries = this.context.EntityTracker.Entities.Cast<Descriptor>()
                                  .Union(context.EntityTracker.Links.Cast<Descriptor>())
                                  .Union(context.EntityTracker.Entities.SelectMany(e => e.StreamDescriptors).Cast<Descriptor>())
                                  .Where(o => o.IsModified && o.ChangeOrder != UInt32.MaxValue)
                                  .OrderBy(o => o.ChangeOrder)
                                  .ToList();

            var result = new BulkUpdateSaveResult(this.context, Util.SaveChangesMethodName, SaveChangesOptions.BulkUpdate, null, null);
            result.BuildDescriptorGraph(changedEntries, true, objects);
            
            bulkUpdateGraph = result.BulkUpdateGraph;
            this.linkDescriptors = result.LinkDescriptors;

            return bulkUpdateGraph;
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

        public class Car
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Person Owner { get; set; }
            public List<Person> Owners { get; set; }
            public List<Manufacturer> Manufacturers { get; set; }
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
    }
}
