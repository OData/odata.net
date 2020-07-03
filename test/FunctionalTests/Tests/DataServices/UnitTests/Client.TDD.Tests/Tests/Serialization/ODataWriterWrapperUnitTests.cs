//---------------------------------------------------------------------
// <copyright file="ODataWriterWrapperUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.Client;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using FluentAssertions;
    using Microsoft.OData.Client.TDDUnitTests;
    using Xunit;

    /// <summary>
    /// Unit tests for the ODataWriterWrapperUnitTests class.
    /// </summary>
    public class ODataWriterWrapperUnitTests
    {
        private DataServiceContext context;

        public ODataWriterWrapperUnitTests()
        {
            context = new DataServiceContext(new Uri("http://www.odata.org/service.svc")).ReConfigureForNetworkLoadingTests();
        }

        [Fact]
        public void EndToEndShortIntegrationWriteEntryEventTest()
        {
            List<KeyValuePair<string, object>> eventArgsCalled = new List<KeyValuePair<string, object>>();
            context.Configurations.RequestPipeline.OnEntityReferenceLink((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntityReferenceLink", args)));
            context.Configurations.RequestPipeline.OnEntryEnding((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntryEnded", args)));
            context.Configurations.RequestPipeline.OnEntryStarting((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntryStarted", args)));
            context.Configurations.RequestPipeline.OnNestedResourceInfoEnding((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnNestedResourceInfoEnded", args)));
            context.Configurations.RequestPipeline.OnNestedResourceInfoStarting((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnNestedResourceInfoStarted", args)));

            Person person = SetupSerializerAndCallWriteEntry(context);

            eventArgsCalled.Should().HaveCount(8);
            eventArgsCalled[0].Key.Should().Be("OnEntryStarted");
            eventArgsCalled[0].Value.Should().BeOfType<WritingEntryArgs>();
            eventArgsCalled[0].Value.As<WritingEntryArgs>().Entity.Should().BeSameAs(person);
            eventArgsCalled[1].Key.Should().Be("OnNestedResourceInfoStarted");
            eventArgsCalled[1].Value.Should().BeOfType<WritingNestedResourceInfoArgs>();
            eventArgsCalled[2].Key.Should().Be("OnEntityReferenceLink");
            eventArgsCalled[2].Value.Should().BeOfType<WritingEntityReferenceLinkArgs>();
            eventArgsCalled[3].Key.Should().Be("OnNestedResourceInfoEnded");
            eventArgsCalled[3].Value.Should().BeOfType<WritingNestedResourceInfoArgs>();
            eventArgsCalled[4].Key.Should().Be("OnNestedResourceInfoStarted");
            eventArgsCalled[4].Value.Should().BeOfType<WritingNestedResourceInfoArgs>();
            eventArgsCalled[5].Key.Should().Be("OnEntityReferenceLink");
            eventArgsCalled[5].Value.Should().BeOfType<WritingEntityReferenceLinkArgs>();
            eventArgsCalled[6].Key.Should().Be("OnNestedResourceInfoEnded");
            eventArgsCalled[6].Value.Should().BeOfType<WritingNestedResourceInfoArgs>();
            eventArgsCalled[7].Key.Should().Be("OnEntryEnded");
            eventArgsCalled[7].Value.Should().BeOfType<WritingEntryArgs>();
            eventArgsCalled[7].Value.As<WritingEntryArgs>().Entity.Should().BeSameAs(person);
        }

        /// <summary>
        /// Instantiates a new Serializer class and calls WriteEntry method on it.
        /// </summary>
        /// <param name="dataServiceContext"></param>
        /// <returns></returns>
        private static Person SetupSerializerAndCallWriteEntry(DataServiceContext dataServiceContext)
        {
            Person person = new Person();
            Address address = new Address();
            Car car1 = new Car();
            person.Cars.Add(car1);
            person.HomeAddress = address;

            dataServiceContext.AttachTo("Cars", car1);
            dataServiceContext.AttachTo("Addresses", address);

            var requestInfo = new RequestInfo(dataServiceContext);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityDescriptor = new EntityDescriptor(clientModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = person;
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.foo.com/Northwind"), headers, entityDescriptor, HttpStack.Auto);
            var linkDescriptors = new LinkDescriptor[] { new LinkDescriptor(person, "Cars", car1, clientModel), new LinkDescriptor(person, "HomeAddress", address, clientModel) };
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);
            return person;
        }

        [Fact]
        public void EntityPropertiesShouldBePopulatedBeforeCallingWriteStart()
        {
            context.Configurations.RequestPipeline.OnEntryStarting(args =>
            {
                args.Entry.Should().NotBeNull();
            });

            Person person = SetupSerializerAndCallWriteEntry(context);
        }

        [Fact]
        public void OnEntryStartShouldBeFired()
        {
            ODataResource entry = new ODataResource();
            var customer = new Customer();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnEntryStarting((WritingEntryArgs args) =>
                {
                    args.Entry.Id = new Uri("http://foo.org");
                    args.Entity.Should().BeSameAs(customer);
                });
            });

            wrappedWriter.WriteStart(entry, customer);
            entry.Id.Should().Be(new Uri("http://foo.org"));
        }

        [Fact]
        public void OnEntryEndShouldBeFired()
        {
            ODataResource entry = new ODataResource();
            var customer = new Customer();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnEntryEnding((args) =>
                {
                    args.Entry.Id = new Uri("http://foo.org");
                    args.Entity.Should().BeSameAs(customer);
                });
            });

            wrappedWriter.WriteEnd(entry, customer);
            entry.Id.Should().Be(new Uri("http://foo.org"));
        }

        [Fact]
        public void OnNavLinkStartShouldBeFired()
        {
            Person p = new Person();
            Address a = new Address();
            ODataNestedResourceInfo link = new ODataNestedResourceInfo();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnNestedResourceInfoStarting((args) =>
                {
                    args.Source.Should().BeSameAs(p);
                    args.Target.Should().BeSameAs(a);
                    args.Link.Name = "HomeAddress";
                });
            });

            wrappedWriter.WriteStart(link, p, a);
            link.Name.Should().Be("HomeAddress");
        }

        [Fact]
        public void OnNavLinkEndShouldBeFired()
        {
            Person p = new Person();
            Address a = new Address();
            ODataNestedResourceInfo link = new ODataNestedResourceInfo();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnNestedResourceInfoEnding((args) =>
                {
                    args.Source.Should().BeSameAs(p);
                    args.Target.Should().BeSameAs(a);
                    args.Link.Name = "foo";
                });
            });

            wrappedWriter.WriteEnd(link, p, a);
            link.Name.Should().Be("foo");
        }

        [Fact]
        public void OnEntityReferenceLink()
        {
            Person p = new Person();
            Address a = new Address();
            var testUrl = new Uri("http://foo.com/link", UriKind.Absolute);
            ODataEntityReferenceLink refLink = new ODataEntityReferenceLink();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnEntityReferenceLink((args) =>
                {
                    args.Source.Should().BeSameAs(p);
                    args.Target.Should().BeSameAs(a);
                    args.EntityReferenceLink.Url = testUrl;
                });
            });

            wrappedWriter.WriteEntityReferenceLink(refLink, p, a);
            refLink.Url.Should().BeSameAs(testUrl);
        }

        [Fact]
        public void SerializeEnity_TwoNavigationLinksInJsonFormat()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

           
            context.AttachTo("Persons", person);
            context.AttachTo("Cars", car1);
            context.AttachTo("Cars", car2);
            context.AddLink(person, "Cars", car1);
            context.AddLink(person, "Cars", car2);

            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityDescriptor = new EntityDescriptor(clientModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = person;
            entityDescriptor.EditLink = new Uri("http://www.foo.com/custom");
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.foo.com/Northwind"), headers, entityDescriptor, HttpStack.Auto);
            var linkDescriptors = new LinkDescriptor[] { new LinkDescriptor(person, "Cars", car1, clientModel), new LinkDescriptor(person, "Cars", car2, clientModel) };
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);

            // read result:
            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(
                "{\"ID\":100,\"Name\":\"Bing\",\"Cars@odata.bind\":[\"http://www.odata.org/service.svc/Cars(1001)\",\"http://www.odata.org/service.svc/Cars(1002)\"]}");
        }



        internal ODataWriterWrapper SetupTestActionExecuted(Action<DataServiceContext, DataServiceClientRequestPipelineConfiguration> setup)
        {
            var writer = new TestODataWriter();
            setup(context, context.Configurations.RequestPipeline);
            return ODataWriterWrapper.CreateForEntryTest(writer, context.Configurations.RequestPipeline);
        }

        public enum MyColor : long
        {
            Green = 1,
            Yellow = 4
        }

        [Flags]
        public enum MyFlagsColor
        {
            Red = 2,
            Blue = 8
        }

        public class ComplexValue1
        {
            public MyColor? MyColorValue { get; set; }
            public MyFlagsColor MyFlagsColorValue { get; set; }
            public string StringValue { get; set; }
        }

        public class MyEntity1
        {
            public long ID { get; set; }
            public MyColor? MyColorValue { get; set; }
            public MyFlagsColor MyFlagsColorValue { get; set; }
            public ComplexValue1 ComplexValue1Value { get; set; }
            public List<MyColor?> MyColorCollection { get; set; }
            public List<MyFlagsColor> MyFlagsColorCollection1 { get; set; }
        }

        public class Car
        {
            public int ID { get; set; }
            public Person Owner { get; set; }
        }

        public class Address
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class Person
        {
            public Person()
            {
                this.Cars = new List<Car>();
            }

            public int ID { get; set; }
            public string Name { get; set; }
            public List<Car> Cars { get; set; }
            public Address HomeAddress { get; set; }
        }

        internal class TestODataWriter : ODataWriter
        {
            public override void WriteStart(ODataResourceSet feed)
            {
            }

            public override Task WriteStartAsync(ODataResourceSet feed)
            {
                throw new Exception("Should not hit this code");
            }

            public override void WriteStart(ODataResource entry)
            {
            }

            public override Task WriteStartAsync(ODataResource entry)
            {
                throw new Exception("Should not hit this code");
            }

            public override void WriteStart(ODataNestedResourceInfo navigationLink)
            {
            }

            public override Task WriteStartAsync(ODataNestedResourceInfo navigationLink)
            {
                throw new Exception("Should not hit this code");
            }

            public override void WriteEnd()
            {
            }

            public override Task WriteEndAsync()
            {
                throw new Exception("Should not hit this code");
            }

            public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
            {
            }

            public override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
            {
                throw new Exception("Should not hit this code");
            }

            public override void Flush()
            {
                throw new Exception("Should not hit this code");
            }

            public override Task FlushAsync()
            {
                throw new Exception("Should not hit this code");
            }
        }
    }
}
