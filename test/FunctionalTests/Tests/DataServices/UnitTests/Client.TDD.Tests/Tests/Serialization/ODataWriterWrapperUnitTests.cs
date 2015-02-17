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
    using System.Text.RegularExpressions;
    using Microsoft.OData.Client;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    /// <summary>
    /// Unit tests for the ODataWriterWrapperUnitTests class.
    /// </summary>
    [TestClass]
    public class ODataWriterWrapperUnitTests
    {
        [TestMethod]
        public void EndToEndShortIntegrationWriteEntryEventTest()
        {
            List<KeyValuePair<string, object>> eventArgsCalled = new List<KeyValuePair<string, object>>();
            var dataServiceContext = new DataServiceContext(new Uri("http://www.odata.org/Service.svc"));
            dataServiceContext.Configurations.RequestPipeline.OnEntityReferenceLink((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntityReferenceLink", args)));
            dataServiceContext.Configurations.RequestPipeline.OnEntryEnding((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntryEnded", args)));
            dataServiceContext.Configurations.RequestPipeline.OnEntryStarting((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnEntryStarted", args)));
            dataServiceContext.Configurations.RequestPipeline.OnNavigationLinkEnding((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnNavigationLinkEnded", args)));
            dataServiceContext.Configurations.RequestPipeline.OnNavigationLinkStarting((args) => eventArgsCalled.Add(new KeyValuePair<string, object>("OnNavigationLinkStarted", args)));

            Person person = SetupSerializerAndCallWriteEntry(dataServiceContext);

            eventArgsCalled.Should().HaveCount(8);
            eventArgsCalled[0].Key.Should().Be("OnEntryStarted");
            eventArgsCalled[0].Value.Should().BeOfType<WritingEntryArgs>();
            eventArgsCalled[0].Value.As<WritingEntryArgs>().Entity.Should().BeSameAs(person);
            eventArgsCalled[1].Key.Should().Be("OnNavigationLinkStarted");
            eventArgsCalled[1].Value.Should().BeOfType<WritingNavigationLinkArgs>();
            eventArgsCalled[2].Key.Should().Be("OnEntityReferenceLink");
            eventArgsCalled[2].Value.Should().BeOfType<WritingEntityReferenceLinkArgs>();
            eventArgsCalled[3].Key.Should().Be("OnNavigationLinkEnded");
            eventArgsCalled[3].Value.Should().BeOfType<WritingNavigationLinkArgs>();
            eventArgsCalled[4].Key.Should().Be("OnNavigationLinkStarted");
            eventArgsCalled[4].Value.Should().BeOfType<WritingNavigationLinkArgs>();
            eventArgsCalled[5].Key.Should().Be("OnEntityReferenceLink");
            eventArgsCalled[5].Value.Should().BeOfType<WritingEntityReferenceLinkArgs>();
            eventArgsCalled[6].Key.Should().Be("OnNavigationLinkEnded");
            eventArgsCalled[6].Value.Should().BeOfType<WritingNavigationLinkArgs>();
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

        [TestMethod]
        public void EntityPropertiesShouldBePopulatedBeforeCallingWriteStart()
        {
            DataServiceContext context = new DataServiceContext(new Uri("http://www.odata.org/service.svc"));
            context.Configurations.RequestPipeline.OnEntryStarting(args =>
            {
                args.Entry.Should().NotBeNull();
            });

            Person person = SetupSerializerAndCallWriteEntry(context);
        }

        [TestMethod]
        public void OnEntryStartShouldBeFired()
        {
            ODataEntry entry = new ODataEntry();
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

        [TestMethod]
        public void OnEntryEndShouldBeFired()
        {
            ODataEntry entry = new ODataEntry();
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

        [TestMethod]
        public void OnNavLinkStartShouldBeFired()
        {
            Person p = new Person();
            Address a = new Address();
            ODataNavigationLink link = new ODataNavigationLink();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnNavigationLinkStarting((args) =>
                {
                    args.Source.Should().BeSameAs(p);
                    args.Target.Should().BeSameAs(a);
                    args.Link.Name = "HomeAddress";
                });
            });

            wrappedWriter.WriteStart(link, p, a);
            link.Name.Should().Be("HomeAddress");
        }

        [TestMethod]
        public void OnNavLinkEndShouldBeFired()
        {
            Person p = new Person();
            Address a = new Address();
            ODataNavigationLink link = new ODataNavigationLink();
            var wrappedWriter = this.SetupTestActionExecuted((context, requestPipeline) =>
            {
                requestPipeline.OnNavigationLinkEnding((args) =>
                {
                    args.Source.Should().BeSameAs(p);
                    args.Target.Should().BeSameAs(a);
                    args.Link.Name = "foo";
                });
            });

            wrappedWriter.WriteEnd(link, p, a);
            link.Name.Should().Be("foo");
        }

        [TestMethod]
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

        [TestMethod]
        public void SerializeEnity_TwoNavigationLinksInJsonFormat()
        {
            var person = new Person
            {
                ID = 100,
                Name = "Bing",
            };

            var car1 = new Car { ID = 1001 };
            var car2 = new Car { ID = 1002 };

            DataServiceContext dataServiceContext = new DataServiceContext(new Uri("http://www.odata.org/service.svc"));
            dataServiceContext.AttachTo("Persons", person);
            dataServiceContext.AttachTo("Cars", car1);
            dataServiceContext.AttachTo("Cars", car2);
            dataServiceContext.AddLink(person, "Cars", car1);
            dataServiceContext.AddLink(person, "Cars", car2);

            var requestInfo = new RequestInfo(dataServiceContext);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityDescriptor = new EntityDescriptor(clientModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = person;
            entityDescriptor.EditLink = new Uri("http://www.foo.com/custom");
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.foo.com/Northwind"), headers, entityDescriptor, HttpStack.Auto);
            var linkDescriptors = new LinkDescriptor[] { new LinkDescriptor(person, "Cars", car1, clientModel), new LinkDescriptor(person, "Cars", car2, clientModel)};
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);

            // read result:
            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(
                "{\"ID\":100,\"Name\":\"Bing\",\"Cars@odata.bind\":[\"http://www.odata.org/service.svc/Cars(1001)\",\"http://www.odata.org/service.svc/Cars(1002)\"]}");
        }

        [TestMethod]
        public void SerializeEnity_EnumProperty()
        {
            MyEntity1 myEntity1 = new MyEntity1()
            {
                ID = 2,
                MyColorValue = MyColor.Yellow,
                MyFlagsColorValue = MyFlagsColor.Blue,
                ComplexValue1Value = new ComplexValue1() { MyColorValue = MyColor.Green, MyFlagsColorValue = MyFlagsColor.Red },
                MyFlagsColorCollection1 = new List<MyFlagsColor>() { MyFlagsColor.Blue, MyFlagsColor.Red, MyFlagsColor.Red },
                MyColorCollection = new List<MyColor?>()
            };

            DataServiceContext dataServiceContext = new DataServiceContext(new Uri("http://www.odata.org/service.svc"));
            dataServiceContext.EnableAtom = true;
            dataServiceContext.Format.UseAtom();
            dataServiceContext.AttachTo("MyEntitySet1", myEntity1);

            var requestInfo = new RequestInfo(dataServiceContext);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            headers.SetHeader("Content-Type", "application/atom+xml;odata.metadata=minimal");
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityDescriptor = new EntityDescriptor(clientModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = myEntity1;
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.foo.com/Northwind"), headers, entityDescriptor, HttpStack.Auto);
            var linkDescriptors = new LinkDescriptor[] { };
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);

            // read result:
            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload = Regex.Replace(payload, "<updated>[^<]*</updated>", "");
            payload.Should().Be(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" " +
                    "xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<id />" +
                    "<title />" +
                //"<updated>2013-11-11T19:29:54Z</updated>" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ComplexValue1Value>" +
                                "<d:MyColorValue m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests_MyColor\">Green</d:MyColorValue>" +
                                "<d:MyFlagsColorValue m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests_MyFlagsColor\">Red</d:MyFlagsColorValue>" +
                                "<d:StringValue m:null=\"true\" />" +
                            "</d:ComplexValue1Value>" +
                            "<d:ID m:type=\"Int64\">2</d:ID>" +
                            "<d:MyColorCollection />" +
                            "<d:MyColorValue m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests_MyColor\">Yellow</d:MyColorValue>" +
                            "<d:MyFlagsColorCollection1>" +
                                "<m:element m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests+MyFlagsColor\">Blue</m:element>" +
                                "<m:element m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests+MyFlagsColor\">Red</m:element>" +
                                "<m:element m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests+MyFlagsColor\">Red</m:element>" +
                            "</d:MyFlagsColorCollection1>" +
                            "<d:MyFlagsColorValue m:type=\"#AstoriaUnitTests.TDD.Tests.Client.ODataWriterWrapperUnitTests_MyFlagsColor\">Blue</d:MyFlagsColorValue>" +
                        "</m:properties>" +
                    "</content>" +
                    "</entry>");
        }

        [TestMethod]
        public void SerializeEnity_NullableEnumProperty()
        {
            MyEntity1 myEntity1 = new MyEntity1()
            {
                ID = 2,
                MyColorValue = null,
                MyFlagsColorValue = MyFlagsColor.Blue,
                ComplexValue1Value = new ComplexValue1() { MyColorValue = MyColor.Green, MyFlagsColorValue = MyFlagsColor.Red },
                MyFlagsColorCollection1 = new List<MyFlagsColor>() { MyFlagsColor.Blue, MyFlagsColor.Red, MyFlagsColor.Red },
                MyColorCollection = new List<MyColor?> { MyColor.Green, null } 
            };

            DataServiceContext dataServiceContext = new DataServiceContext(new Uri("http://www.odata.org/service.svc"));
            dataServiceContext.EnableAtom = true;
            dataServiceContext.Format.UseAtom();
            dataServiceContext.AttachTo("MyEntitySet1", myEntity1);

            var requestInfo = new RequestInfo(dataServiceContext);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityDescriptor = new EntityDescriptor(clientModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = myEntity1;
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://www.foo.com/Northwind"), headers, entityDescriptor, HttpStack.Auto);
            var linkDescriptors = new LinkDescriptor[] { };
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);

            // read result:
            MemoryStream stream = (MemoryStream)(odataRequestMessageWrapper.CachedRequestStream.Stream);
            stream.Position = 0;

            string payload = (new StreamReader(stream)).ReadToEnd();
            payload = Regex.Replace(payload, "<updated>[^<]*</updated>", "");
            payload.Should().Be(
                "{\"ComplexValue1Value\":{\"MyColorValue\":\"Green\",\"MyFlagsColorValue\":\"Red\",\"StringValue\":null},\"ID\":2,\"MyColorCollection\":[\"Green\",null],\"MyColorValue\":null,\"MyFlagsColorCollection1\":[\"Blue\",\"Red\",\"Red\"],\"MyFlagsColorValue\":\"Blue\"}");
        }

        internal ODataWriterWrapper SetupTestActionExecuted(Action<DataServiceContext, DataServiceClientRequestPipelineConfiguration> setup)
        {
            var dataServiceContext = new DataServiceContext();
            var writer = new TestODataWriter();
            setup(dataServiceContext, dataServiceContext.Configurations.RequestPipeline);
            return ODataWriterWrapper.CreateForEntryTest(writer, dataServiceContext.Configurations.RequestPipeline);
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
            public override void WriteStart(ODataFeed feed)
            {
            }

            public override Task WriteStartAsync(ODataFeed feed)
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override void WriteStart(ODataEntry entry)
            {
            }

            public override Task WriteStartAsync(ODataEntry entry)
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override void WriteStart(ODataNavigationLink navigationLink)
            {
            }

            public override Task WriteStartAsync(ODataNavigationLink navigationLink)
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override void WriteEnd()
            {
            }

            public override Task WriteEndAsync()
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
            {
            }

            public override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override void Flush()
            {
                throw new InternalTestFailureException("Should not hit this code");
            }

            public override Task FlushAsync()
            {
                throw new InternalTestFailureException("Should not hit this code");
            }
        }
    }
}
