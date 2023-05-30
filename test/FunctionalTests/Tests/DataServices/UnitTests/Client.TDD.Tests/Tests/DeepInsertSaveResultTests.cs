//---------------------------------------------------------------------
// <copyright file="BulkUpdateSaveResultTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    /// <summary>
    /// Unit tests for the DeepInsertSaveResult class.
    /// </summary>
    public class DeepInsertSaveResultTests
    {
        private readonly DataServiceContext context;
        private readonly RequestInfo requestInfo;
        private readonly Serializer serializer;
        private readonly HeaderCollection headers;
        private BulkUpdateGraph bulkUpdateGraph;

        public DeepInsertSaveResultTests()
        {
            this.context = new DataServiceContext(new Uri("http://www.odata.org/service.svc")).ReConfigureForNetworkLoadingTests();
            this.requestInfo = new RequestInfo(context);
            this.serializer = new Serializer(this.requestInfo);
            this.headers = new HeaderCollection();
            bulkUpdateGraph = new BulkUpdateGraph(this.requestInfo);

            ConfigureHeaders();
        }

        private void ConfigureHeaders()
        {
            this.requestInfo.Context.Format.SetRequestContentTypeForEntry(headers);

            headers.SetHeader("OData-Version", "4.01");
            headers.SetHeader("OData-MaxVersion", "4.01");

            this.requestInfo.Format.SetRequestAcceptHeader(headers);
        }

        [Fact]
        public void SerializeEntry_With_TwoNavigationLinks()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.AddLink(person, "Cars", car1);
            this.context.AddLink(person, "Cars", car2);

            EntityDescriptor car1Descriptor = context.GetEntityDescriptor(car1);
            EntityDescriptor car2Descriptor = context.GetEntityDescriptor(car2);

            Assert.Equal(EntityStates.Unchanged, car1Descriptor.State);
            Assert.Equal(EntityStates.Unchanged, car2Descriptor.State);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.odata.org/service.svc/Persons"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, requestInfo.Configurations.RequestPipeline);
                serializer.WriteDeepInsertEntry(this.bulkUpdateGraph.TopLevelDescriptors[0], bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"ID\":100,\"Name\":\"Bing\",\"Cars\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1001)\"},{\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}]}");
        }

        [Fact]
        public void SerializeEntry_With_SingleValueNavigation()
        {
            var vipPerson = new VipPerson
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };

            this.context.AttachTo("Persons", vipPerson);
            this.context.SetRelatedObject(vipPerson, "Car", car1);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(vipPerson);

            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.odata.org/service.svc/VipPersons"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, requestInfo.Configurations.RequestPipeline);
                serializer.WriteDeepInsertEntry(this.bulkUpdateGraph.TopLevelDescriptors[0], bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"ID\":100,\"Name\":\"Bing\",\"Car\":{\"ID\":1001,\"Name\":null}}");
        }

        [Fact]
        public void SerializeEntry_With_SingleValueNavigationLink()
        {
            var vipPerson = new VipPerson
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };

            this.context.AttachTo("Persons", vipPerson);
            this.context.AttachTo("Cars", car1);
            this.context.SetRelatedObjectLink(vipPerson, "Car", car1);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(vipPerson);

            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.odata.org/service.svc/VipPersons"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, requestInfo.Configurations.RequestPipeline);
                serializer.WriteDeepInsertEntry(this.bulkUpdateGraph.TopLevelDescriptors[0], bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"ID\":100,\"Name\":\"Bing\",\"Car\":{\"@id\":\"http://www.odata.org/service.svc/Cars(1001)\"}}");
        }

        [Fact]
        public void SerializeEntry_With_NoNavigationLinks()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            this.context.AttachTo("Persons", person);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.odata.org/service.svc/Persons"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, requestInfo.Configurations.RequestPipeline);
                serializer.WriteDeepInsertEntry(this.bulkUpdateGraph.TopLevelDescriptors[0], bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"ID\":100,\"Name\":\"Bing\"}");
        }

        [Fact]
        public void SerializeEntry_With3LevelsOfNesting()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var country = new Country { ID = 101, Name = "CountryA" };
            this.context.AttachTo("Persons", person);
            this.context.AddRelatedObject(person, "Cars", car1);
            this.context.AddRelatedObject(car1, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(manufacturer, "Countries", country);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.odata.org/service.svc/Persons"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, this.requestInfo.Configurations.RequestPipeline);
                serializer.WriteDeepInsertEntry(this.bulkUpdateGraph.TopLevelDescriptors[0], this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"ID\":100,\"Name\":\"Bing\",\"Cars\":[{\"ID\":1001,\"Name\":null,\"Manufacturers\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}");
        }

        [Fact]
        public void SerializeEntry_With_DeleteLink_Fails()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);
            this.context.DeleteLink(person, "Cars", car2);

            Action action = () =>
            {
                this.GetBulkUpdateGraph(person);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_DeepInsertDeletedOrModified("Cars(1002)"));
        }

        [Fact]
        public void SerializeEntry_With_No_TopLevelDescriptor_Fails()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);

            EntityDescriptor personDescriptor = this.context.GetEntityDescriptor(person);
            this.context.EntityTracker.DetachResource(personDescriptor);

            Action action = () =>
            {
                this.GetBulkUpdateGraph(person);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_EntityNotContained);
        }

        [Fact]
        public void SerializeEntry_With_UpdateObject_Fails()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var cars = new List<Car>
            {
                new Car
                {
                    ID = 200,
                    Name = "Car 1",
                }
            };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Cars", cars[0]);

            DataServiceCollection<Car> carsCollection = new DataServiceCollection<Car>(this.context);
            carsCollection.Load(cars);
            carsCollection[0].Name = "Updated car";

            this.context.AddLink(person, "Cars", carsCollection[0]);

            Action action = () =>
            {
                this.GetBulkUpdateGraph(person);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_DeepInsertDeletedOrModified("Cars(200)"));
        }

        [Fact]
        public void Serialize_MultipleTopLevel_Resources_Fails()
        {
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

            Action action = () =>
            {
                this.GetBulkUpdateGraph(person, person2);
            };

            action.ShouldThrow<InvalidOperationException>(Strings.Context_DeepInsertOneTopLevelEntity);
        }

        private BulkUpdateGraph GetBulkUpdateGraph<T>(params T[] objects)
        {
            List<Descriptor> changedEntries = this.context.EntityTracker.Entities.Cast<Descriptor>()
                                  .Union(context.EntityTracker.Links.Cast<Descriptor>())
                                  .Union(context.EntityTracker.Entities.SelectMany(e => e.StreamDescriptors).Cast<Descriptor>())
                                  .Where(o => o.IsModified && o.ChangeOrder != UInt32.MaxValue)
                                  .OrderBy(o => o.ChangeOrder)
                                  .ToList();

            var result = new DeepInsertSaveResult(this.context, Util.SaveChangesMethodName, SaveChangesOptions.DeepInsert, null, null);
            result.BuildDescriptorGraph<T>(changedEntries, true, (T[])objects);
            this.bulkUpdateGraph = result.BulkUpdateGraph;

            return this.bulkUpdateGraph;
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

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        public class Address
        {
            public int ID { get; set; }
            public string Name { get; set; }
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
            public Address HomeAddress { get; set; }

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
            public Car Car { get; set; }
        }
    }
}
