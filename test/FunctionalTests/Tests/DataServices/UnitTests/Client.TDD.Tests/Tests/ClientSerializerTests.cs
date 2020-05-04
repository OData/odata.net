//---------------------------------------------------------------------
// <copyright file="ClientSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.OData.Client.TDDUnitTests;
    using Xunit;

    /// <summary>
    /// Unit tests for client request serialization code.
    /// </summary>
    public class ClientSerializerTests
    {
        private DataServiceContext context;
        public ClientSerializerTests()
        {
            context = new DataServiceContext(new Uri("http://www.odata.org/service.svc")) { ResolveName = type => type.FullName }.ReConfigureForNetworkLoadingTests();
        }

        [Fact]
        public void ClientShouldNotIncludeIdInJsonLightUpdates()
        {
            Action<EntityDescriptor> configureDescriptor = d =>
            {
                d.Identity = new Uri("http://foo.org");
                d.State = EntityStates.Modified;
            };

            var testSubject = CreateODataEntry(configureDescriptor, f => f.UseJson(new EdmModel()));
            testSubject.Id.Should().BeNull();
        }

        [Fact]
        public void ClientShouldSetTypeNameFromClientTypeAnnotation()
        {
            var testSubject = CreateODataEntry(clientTypeName: "Fake.Type");
            testSubject.TypeName.Should().Be("Fake.Type");
        }

        [Fact]
        public void ClientShouldAddTypeAnnotationIfServerNameIsDifferent()
        {
            var testSubject = CreateODataEntry(serverTypeName: "Foo", clientTypeName: "Fake.Type");
            var annotation = testSubject.TypeAnnotation;
            annotation.Should().NotBeNull();
            annotation.TypeName.Should().Be("Foo");
        }

        [Fact]
        public void ClientShouldNotAddTypeAnnotationIfServerNameIsTheSame()
        {
            var testSubject = CreateODataEntry(serverTypeName: "Fake.Type", clientTypeName: "Fake.Type");
            testSubject.TypeAnnotation.Should().BeNull();
        }

        [Fact]
        public void ClientShouldSetMediaResourceIfTypeIsMediaLinkEntry()
        {
            var testSubject = CreateODataEntry<MyMleType>();
            testSubject.MediaResource.Should().NotBeNull();
        }

        [Fact]
        public void ClientShouldSetMediaResourceIfDescriptorIsMediaLinkEntry()
        {
            var testSubject = CreateODataEntry(d => d.EditStreamUri = new Uri("http://example.com/odata.svc/streamsRule"));
            testSubject.MediaResource.Should().NotBeNull();
        }

        private static ODataResource CreateODataEntry(Action<EntityDescriptor> configureDescriptor = null, Action<DataServiceClientFormat> configureFormat = null, string serverTypeName = "serverTypeName", string clientTypeName = "clientTypeName")
        {
            return CreateODataEntry<object>(configureDescriptor, configureFormat, serverTypeName, clientTypeName);
        }

        private static ODataResource CreateODataEntry<T>(Action<EntityDescriptor> configureDescriptor = null, Action<DataServiceClientFormat> configureFormat = null, string serverTypeName = "serverTypeName", string clientTypeName = "clientTypeName")
        {
            ClientEdmModel model = new ClientEdmModel(ODataProtocolVersion.V4);
            var ctx = new DataServiceContext(new Uri("http://www.example.com/odata.svc"), ODataProtocolVersion.V4, model);

            EntityDescriptor entityDescriptor = new EntityDescriptor(model);
            if (configureDescriptor != null)
            {
                configureDescriptor(entityDescriptor);
            }

            if (configureFormat != null)
            {
                configureFormat(ctx.Format);
            }

            ClientTypeAnnotation clientTypeAnnotation = new ClientTypeAnnotation(new EdmEntityType("Fake", "Fake"), typeof(T), clientTypeName, model);
            return Serializer.CreateODataEntry(entityDescriptor, serverTypeName, clientTypeAnnotation, ctx.Format);
        }

        [HasStream]
        private abstract class MyMleType
        {
        }

        [Fact]
        public void WriteNullUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("p1", null) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            Assert.Equal(new Uri("http://www.odata.org/service.svc/Function(p1=null)"), requestUri);
        }

        [Fact]
        public void WriteOneUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            const int p1 = 1;
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("p1", p1) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            Assert.Equal(new Uri("http://www.odata.org/service.svc/Function(p1=1)"), requestUri);
        }

        [Fact]
        public void WriteTwoUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            const int p1 = 1;
            const double p2 = 1;
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("p1", p1), new UriOperationParameter("p2", p2) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            Assert.Equal(new Uri("http://www.odata.org/service.svc/Function(p1=1,p2=1.0)"), requestUri);
        }

        [Fact]
        public void WriteStringAndBoolUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            const string name = "Henry";
            const bool p2 = true;
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("name", name), new UriOperationParameter("p2", p2) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            Assert.Equal(new Uri("http://www.odata.org/service.svc/Function(name='Henry',p2=true)"), requestUri);
        }

        [Fact]
        public void WritePrimitiveUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
            TimeSpan duration = XmlConvert.ToTimeSpan("P104DT7H50M13.133759S");
            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse("2000-12-12T12:00Z");
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("guid", guid), new UriOperationParameter("duration", duration), new UriOperationParameter("dateTimeOffset", dateTimeOffset) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string uriString = "http://www.odata.org/service.svc/Function(guid=00000000-0000-0000-0000-000000000000,duration=duration'P104DT7H50M13.133759S',dateTimeOffset=2000-12-12T12:00:00Z)";
            Uri expectedUri = new Uri(uriString);
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteDateAndTimeUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            Date date = new Date(2014, 9, 25);
            TimeOfDay time = new TimeOfDay(11, 52, 5, 123);
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("date", date), new UriOperationParameter("time", time) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string uriString = "http://www.odata.org/service.svc/Function(date=2014-09-25,time=11:52:05.1230000)";
            Uri expectedUri = new Uri(uriString);
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteGeographyUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            Geography geography = SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(false).Read<Geography>(new StringReader("SRID=4326;POINT (10.22 10)"));
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("geography", geography) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string parameterString = "geography'SRID=4326;POINT (10.22 10)'";
            Uri expectedUri = new Uri("http://www.odata.org/service.svc/Function(geography=" + Uri.EscapeDataString(parameterString) + ")");
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteCollectionUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            List<int> collection = new List<int> { 1, 2, 3 };
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("collection", collection) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string parameterString = "[1,2,3]";
            Uri expectedUri = new Uri("http://www.odata.org/service.svc/Function(collection=%40collection)?%40collection=" + Uri.EscapeDataString(parameterString));
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteEnumUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri functionBaseRequestUri = new Uri("http://www.odata.org/service.svc/Function");
            const Color color = Color.Blue;
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("color", color) };
            var requestUri = serializer.WriteUriOperationParametersToUri(functionBaseRequestUri, parameters);
            string parameterString = "AstoriaUnitTests.TDD.Tests.Client.Color'blue'";
            Uri expectedUri = new Uri("http://www.odata.org/service.svc/Function(color=" + Uri.EscapeDataString(parameterString) + ")");
            Assert.Equal(expectedUri, requestUri);

            const AccessLevel accessLevel = AccessLevel.Execute | AccessLevel.Write;
            parameters = new List<UriOperationParameter> { new UriOperationParameter("accessLevel", accessLevel) };
            requestUri = serializer.WriteUriOperationParametersToUri(functionBaseRequestUri, parameters);
            parameterString = "AstoriaUnitTests.TDD.Tests.Client.AccessLevel'Write,execute'";
            expectedUri = new Uri("http://www.odata.org/service.svc/Function(accessLevel=" + Uri.EscapeDataString(parameterString) + ")");
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteComplexTypeUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            Address address = new Address { Street = "Microsoft Street" };
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("address", address) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string parameterString = "{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Address\",\"Street\":\"Microsoft Street\"}";
            Uri expectedUri = new Uri("http://www.odata.org/service.svc/Function(address=%40address)?%40address=" + Uri.EscapeDataString(parameterString));
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteCollectionOfComplexTypeInUri()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            List<Address> addresses = new List<Address>()
            {
                new Address { Street = "Microsoft Street" },
                new HomeAddress { Street = "Chinese Street", Number = "999" },
                
            };
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("addresses", addresses) };
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            const string parameterString = "[{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Address\",\"Street\":\"Microsoft Street\"},{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.HomeAddress\",\"Number\":\"999\",\"Street\":\"Chinese Street\"}]";
            Uri expectedUri = new Uri("http://www.odata.org/service.svc/Function(addresses=%40addresses)?%40addresses=" + Uri.EscapeDataString(parameterString));
            Assert.Equal(expectedUri, requestUri);
        }

        [Fact]
        public void WriteCollectionOfComplexTypeInBody()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            List<Address> addresses = new List<Address>()
            {
                new Address { Street = "Microsoft Street" },
                new HomeAddress { Street = "Chinese Street", Number = "999" },
                
            };
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("addresses", addresses) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = "{\"addresses\":[{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Address\",\"Street\":\"Microsoft Street\"},{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.HomeAddress\",\"Number\":\"999\",\"Street\":\"Chinese Street\"}]}";
            VerifyMessageBody(requestMessage, parameterString);
        }

        [Fact]
        public void WriteEntryAsBodyOperationParameter()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Customer customer = new Customer()
            {
                Id = 1,
                Address = new Address(){ Street = "Microsoft Street" },
                Emails = new List<string>(){"tom@microsoft.com", "jerry@microsoft.com"},
            };

            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("customer", customer) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = "{\"customer\":{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Customer\",\"Emails@odata.type\":\"#Collection(String)\",\"Emails\":[\"tom@microsoft.com\",\"jerry@microsoft.com\"],\"Id\":1,\"Address\":{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Address\",\"Street\":\"Microsoft Street\"}}}";
            VerifyMessageBody(requestMessage, parameterString);
        }

        [Fact]
        public void WriteNullAsBodyOperationParameter()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("customer", null) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = "{\"customer\":null}";
            VerifyMessageBody(requestMessage, parameterString);
        }

        [Fact]
        public void WriteFeedAsBodyOperationParameter()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Customer customer1 = new Customer()
            {
                Id = 1,
                Address = new Address() { Street = "Microsoft Street" },
                Emails = new List<string>() { "tom@microsoft.com", "jerry@microsoft.com" },
            };

            Customer customer2 = new Customer()
            {
                Id = 2,
                Emails = new List<string>(),
            };

            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("customer", new List<Customer>(){customer1, customer2}) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = "{\"customer\":[{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Customer\",\"Emails@odata.type\":\"#Collection(String)\",\"Emails\":[\"tom@microsoft.com\",\"jerry@microsoft.com\"],\"Id\":1,\"Address\":{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Address\",\"Street\":\"Microsoft Street\"}},{\"@odata.type\":\"#AstoriaUnitTests.TDD.Tests.Client.Customer\",\"Emails@odata.type\":\"#Collection(String)\",\"Emails\":[],\"Id\":2,\"Address\":null}]}";
            VerifyMessageBody(requestMessage, parameterString);
        }

        [Fact]
        public void WriteEmptyFeedAsBodyOperationParameter()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("customer", new List<Customer>()) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = "{\"customer\":[]}";
            VerifyMessageBody(requestMessage, parameterString);
        }

        private ODataRequestMessageWrapper CreateRequestMessageForPost(RequestInfo requestInfo)
        {
            HeaderCollection headers = new HeaderCollection();
            headers.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

            return ODataRequestMessageWrapper.CreateRequestMessageWrapper(
                new BuildingRequestEventArgs("POST", new Uri("http://service.svc/randomuri"), headers, null, HttpStack.Auto), requestInfo);
        }

        private void VerifyMessageBody(ODataRequestMessageWrapper requestMessage, string expected)
        {
            MemoryStream stream = (MemoryStream)requestMessage.CachedRequestStream.Stream;
            StreamReader streamReader = new StreamReader(stream);
            String body = streamReader.ReadToEnd();
            Assert.Equal(expected, body);
        }

        [Fact]
        public void WriteEnumTypeUriOperationParameterWithNonExistingValueShouldThrow()
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            const Color color = 0;
            List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("color", color) };
            Action action = () => serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            action.ShouldThrow<NotSupportedException>().WithMessage("The enum type 'Color' has no member named '0'.");
        }

        [Fact]
        public void WriteEnumTypeInFilterShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));

            // equal
            var entity = Context.EntitySet.Where(e => e.ColorProp == Color.Blue) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp eq namespace.test.color'blue'");

            // greater than
            entity = Context.EntitySet.Where(e => e.ColorProp > Color.Blue) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp gt namespace.test.color'blue'");

            entity = Context.EntitySet.Where(e => Color.Blue < e.ColorProp) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=namespace.test.color'blue' lt colorProp");
        }

        [Fact]
        public void WriteEnumAsNullInFilterShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.ColorProp == null) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp eq null");
        }

        [Fact]
        public void WriteAllBinaryOperandsAsEnumTypeShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.ColorProp == e.ComplexProp.Color) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp eq complexProp/color");

            entity = Context.EntitySet.Where(e => e.ColorProp >= e.ComplexProp.Color) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp ge complexProp/color");
        }

        // Any
        [Fact]
        public void WriteEnumCollectionAnyInFilterShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.EnumCollection.Any(i => i == Color.Blue)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=enumCollection/any(i:i eq namespace.test.color'blue')");
        }

        // All
        [Fact]
        public void WriteEnumCollectionAllInFilterShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.EnumCollection.Any(i => i == Color.Blue) && e.EnumCollection.All(i => i == Color.White)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=enumCollection/any(i:i eq namespace.test.color'blue') and enumCollection/all(i:i eq namespace.test.color'white')");
        }

        // OrderBy
        [Fact]
        public void WriteEnumTypeInOrderByShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.ComplexProp.Color == Color.Blue).OrderBy(e => e.ColorProp) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=complexProp/color eq namespace.test.color'blue'&$orderby=colorProp");

        }

        // Select
        [Fact]
        public void WriteEnumTypeInSelectShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Select(e => new EntityType() { KeyProp = e.KeyProp, ColorProp = e.ColorProp }) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$select=keyProp,colorProp");
        }

        // HasFlags
        [Fact]
        public void WriteEnumTypeHasFlagShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.Access.HasFlag(AccessLevel.Read)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=access has namespace.test.accessLevel'read'");

            entity = Context.EntitySet.Where(e => ((Color)e.ColorProp).HasFlag(e.ColorProp)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp has colorProp");

            entity = Context.EntitySet.Where(e => ((Color)e.ColorProp).HasFlag(e.ColorProp.Value)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp has colorProp");
        }

        // Cast
        [Fact]
        public void WriteEnumTypeCastAnsHasFlagShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => ((Color)e.ColorProp).HasFlag(Color.Blue)) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=colorProp has namespace.test.color'blue'");

            entity = Context.EntitySet.Where(e => ((Color)3) == e.ColorProp) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=namespace.test.color'blue' eq colorProp");
        }

        // Isof
        [Fact]
        public void WriteEnumTypeCastShouldSerializeEnumType()
        {
            Container Context = new Container(new Uri("http://www.odata.org/service.svc"));
            var entity = Context.EntitySet.Where(e => e.ColorProp is Color) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet?$filter=isof(colorProp, 'namespace.test.color')");
        }
    }
}
