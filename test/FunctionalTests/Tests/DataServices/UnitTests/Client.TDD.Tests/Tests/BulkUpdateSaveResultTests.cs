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
    /// Unit tests for the BulkUpdateSaveResult class.
    /// </summary>
    public class BulkUpdateSaveResultTests
    {
        private readonly DataServiceContext context;
        private readonly RequestInfo requestInfo;
        private readonly Serializer serializer;
        private readonly HeaderCollection headers;
        private BulkUpdateGraph bulkUpdateGraph;
        private readonly Dictionary<Descriptor, List<LinkDescriptor>> linkDescriptors;

        public BulkUpdateSaveResultTests()
        {
            this.context = new DataServiceContext(new Uri("http://www.odata.org/service.svc")).ReConfigureForNetworkLoadingTests();
            this.requestInfo = new RequestInfo(context);
            this.serializer = new Serializer(this.requestInfo);
            this.headers = new HeaderCollection();
            bulkUpdateGraph = new BulkUpdateGraph(this.requestInfo);
            this.linkDescriptors = new Dictionary<Descriptor, List<LinkDescriptor>>();
        }

        [Fact]
        public void SerializeADeltaEntry_With_TwoNavigationLinks()
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

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1001)\"},{\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}]}]}");
        }

        [Fact]
        public void SerializeADeltaEntry_With_ADeleteLink()
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

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"http://www.odata.org/service.svc/Cars(1002)\"}]}]}");
        }

        [Fact]
        public void Serialize_MultipleTopLevel_Resources()
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

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person, person2);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\"},{\"ID\":200,\"Name\":\"Edge\"}]}");
        }

        [Fact]
        public void Serialize_Resources_WithCyclicRelations()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };

            context.AttachTo("Persons", person);
            context.AddRelatedObject(person, "Cars", car1);
            context.AddLink(car1, "Owners", person);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":1001,\"Owners@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Persons(100)\"}]}]}]}");
        }

        [Fact]
        public void Serialize_MultipleResources_ReferencingSameResource()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge",
            };

            var car1 = new Car { ID = 1001 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car1);
            this.context.AddLink(person, "Cars", car1);
            this.context.AddLink(person2, "Cars", car1);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person, person2);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1001)\"}]},{\"ID\":200,\"Name\":\"Edge\",\"Cars@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Cars(1001)\"}]}]}");
        }

        [Fact]
        public void Serialize_AnEntry_With4LevelsOfNesting()
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

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":1001,\"Manufacturers@delta\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries@delta\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]}]}");
        }

        [Fact]
        public void Serialize_MultipleEntries_WithDifferentLevelsOfNesting()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var person2 = new Person
            {
                ID = 200,
                Name = "Edge",
            };

            var car1 = new Car { ID = 1001 };
            var manufacturer = new Manufacturer { ID = 11, Name = "Manu-A" };
            var country = new Country { ID = 101, Name = "CountryA" };
            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AddRelatedObject(person, "Cars", car1);
            this.context.AddRelatedObject(car1, "Manufacturers", manufacturer);
            this.context.AddRelatedObject(manufacturer, "Countries", country);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person, person2);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Persons/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Persons", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Persons/$delta\",\"value\":[{\"ID\":100,\"Name\":\"Bing\",\"Cars@delta\":[{\"ID\":1001,\"Manufacturers@delta\":[{\"ID\":11,\"Name\":\"Manu-A\",\"Countries@delta\":[{\"ID\":101,\"Name\":\"CountryA\"}]}]}]},{\"ID\":200,\"Name\":\"Edge\"}]}");
        }

        [Fact]
        public void Serialize_ADeltaEntry_WithASingleValuedNavigationProperty()
        {
            var car = new Car
            {
                ID = 100,
            };

            var person = new Person
            {
                ID = 200,
                Name = "Edge",
            };

            this.context.AttachTo("Cars", car);
            //this.context.AttachTo("Persons", person);

            this.context.UpdateRelatedObject(car, "Owner", person);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(car);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Cars/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Cars", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            //@delta not appended to single-valued navigation properties. 
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Cars/$delta\",\"value\":[{\"ID\":100,\"Owner\":{\"ID\":200,\"Name\":\"Edge\"}}]}");
        }

        [Fact]
        public void Serialize_ADeltaEntry_WithTwoDifferentNavigationProps()
        {
            var car = new Car
            {
                ID = 100,
            };

            var person = new Person
            {
                ID = 200,
                Name = "Edge",
            };

            var manufacturer = new Manufacturer
            {
                ID = 1001,
                Name = "Man-A"
            };

            this.context.AttachTo("Cars", car);
            this.context.AddRelatedObject(car, "Owners", person);
            this.context.AddRelatedObject(car, "Manufacturers", manufacturer);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(car);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Cars/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Cars", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Cars/$delta\",\"value\":[{\"ID\":100,\"Owners@delta\":[{\"ID\":200,\"Name\":\"Edge\"}],\"Manufacturers@delta\":[{\"ID\":1001,\"Name\":\"Man-A\"}]}]}");
        }

        [Fact]
        public void BulkUpdateGraphWithOneTopLevelElement_CreatedSuccessfully()
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

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person);

            Assert.True(this.bulkUpdateGraph != null);
            Assert.Equal(2, this.bulkUpdateGraph.GetRelatedDescriptors(this.bulkUpdateGraph.TopLevelDescriptors.FirstOrDefault()).Count);
        }

        [Fact]
        public void BulkUpdateGraphWithTwoTopLevelElement_CreatedSuccessfully()
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

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

            this.context.AttachTo("Persons", person);
            this.context.AttachTo("Persons", person2);
            this.context.AttachTo("Cars", car1);
            this.context.AttachTo("Cars", car2);

            this.context.AddLink(person, "Cars", car1);
            this.context.AddLink(person, "Cars", car2);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(person, person2);

            Assert.True(this.bulkUpdateGraph != null);
            Assert.Equal(2, this.bulkUpdateGraph.GetRelatedDescriptors(this.bulkUpdateGraph.TopLevelDescriptors.FirstOrDefault()).Count);
        }

        [Fact]
        public void SerializeODataIdWithSomePropertiesToUpdate()
        {
            var car = new Car
            {
                ID = 100,
            };

            var persons = new List<Person>
            {
                new Person{
                    ID = 200,
                    Name = "Edge",
                }
            };

            this.context.AttachTo("Persons", persons[0]);
            DataServiceCollection<Person> personsCollection = new DataServiceCollection<Person>(this.context);
            personsCollection.Load(persons);
            personsCollection[0].Name = "Bing";

            this.context.AttachTo("Cars", car);
            this.context.AddLink(car, "Owners", personsCollection[0]);

            this.bulkUpdateGraph = this.GetBulkUpdateGraph(car);

            var requestMessageArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://www.odata.org/service.svc/$metadata#Cars/$delta"), this.headers, this.bulkUpdateGraph.TopLevelDescriptors[0], HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(odataRequestMessageWrapper, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, "Cars", this.requestInfo.Configurations.RequestPipeline, odataRequestMessageWrapper, this.requestInfo);
                serializer.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, entryWriter);
            }

            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be("{\"@context\":\"http://www.odata.org/service.svc/$metadata#Cars/$delta\",\"value\":[{\"ID\":100,\"Owners@delta\":[{\"@id\":\"http://www.odata.org/service.svc/Persons(200)\",\"Name\":\"Bing\"}]}]}");
        }

        private BulkUpdateGraph GetBulkUpdateGraph<T>(params T[] objects)
        {
            List<Descriptor> changedEntries = this.context.EntityTracker.Entities.Cast<Descriptor>()
                                  .Union(context.EntityTracker.Links.Cast<Descriptor>())
                                  .Union(context.EntityTracker.Entities.SelectMany(e => e.StreamDescriptors).Cast<Descriptor>())
                                  .Where(o => o.IsModified && o.ChangeOrder != UInt32.MaxValue)
                                  .OrderBy(o => o.ChangeOrder)
                                  .ToList();

            var result = new BulkUpdateSaveResult(this.context, Util.SaveChangesMethodName, SaveChangesOptions.BulkUpdate, null, null);
            result.BuildDescriptorGraph<T>(changedEntries, true, (T[])objects);
            this.bulkUpdateGraph = result.BulkUpdateGraph;

            return this.bulkUpdateGraph;
        }

        public class Car
        {
            public int ID { get; set; }
            public Person Owner { get; set; }
            public List<Person> Owners { get; set; }
            public List<Manufacturer> Manufacturers { get; set; }
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
    }
}
