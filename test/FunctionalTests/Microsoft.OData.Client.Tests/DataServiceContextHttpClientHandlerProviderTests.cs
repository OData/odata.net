//---------------------------------------------------------------------
// <copyright file="DataServiceContextHttpClientHandlerProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Tests.Serialization;
using Microsoft.WindowsAzure.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class DataServiceContextHttpClientHandlerProviderTests
    {
        [Fact]
        public async Task GetAllPagesAsync_FetchesContinuationsBeforeReturning()
        {
            int requests = 0;
            using (var handler = new MockHttpClientHandler(request =>
            {
                requests++;
                string payload = requests == 1
                    ? "{\"@odata.context\":\"http://service.org/$metadata#People\",\"value\":[{\"Id\":1,\"Name\":\"First\"}],\"@odata.nextLink\":\"http://service.org/People?$skip=1\"}"
                    : "{\"@odata.context\":\"http://service.org/$metadata#People\",\"value\":[{\"Id\":2,\"Name\":\"Second\"}]}";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(payload, Encoding.UTF8, "application/json") };
            }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                int responses = 0;
                context.ReceivingResponse += (sender, args) => responses++;
                var people = await context.CreateQuery<TestPerson>("People").GetAllPagesAsync();
                Assert.Equal(2, requests);
                Assert.Equal(2, responses);
                Assert.Equal(new[] { "First", "Second" }, people.Select(p => p.Name));
                Assert.Equal(2, requests);
            }
        }

        [Fact]
        public async Task ExecuteAsync_NonGenericAndContinuationPreserveResponses()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"@odata.context\":\"http://service.org/$metadata#People\",\"value\":[{\"Id\":1,\"Name\":\"First\"}],\"@odata.nextLink\":\"http://service.org/People?$skip=1\"}", Encoding.UTF8, "application/json") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                DataServiceQuery query = context.CreateQuery<TestPerson>("People");
                var response = Assert.IsType<QueryOperationResponse<TestPerson>>(await query.ExecuteAsync());
                Assert.Single(response);
                var next = Assert.IsType<QueryOperationResponse<TestPerson>>(await context.ExecuteAsync(response.GetContinuation(), CancellationToken.None));
                Assert.Single(next);
                Assert.Equal(200, next.StatusCode);
                Assert.Equal(2, handler.Requests.Count);
            }
        }

        [Fact]
        public async Task LoadPropertyAsync_UsesNativeTransportAndUpdatesEntity()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"@odata.context\":\"http://service.org/$metadata#Edm.String\",\"value\":\"Updated\"}", Encoding.UTF8, "application/json") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var person = new TestPerson { Id = 1, Name = "Original" };
                context.AttachTo("People", person);
                var response = await context.LoadPropertyAsync(person, "Name", CancellationToken.None);
                Assert.Equal(200, response.StatusCode);
                Assert.Equal("Updated", person.Name);
                Assert.Single(handler.Requests);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetReadStreamAsync_ReadsDefaultAndNamedStreams(bool named)
        {
            using (var handler = new MockHttpClientHandler("stream content"))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var person = new TestPerson { Id = 1 };
                context.AttachTo("People", person);
                EntityDescriptor descriptor = context.GetEntityDescriptor(person);
                descriptor.ReadStreamUri = new Uri(BaseUri + "/People(1)/$value");
                descriptor.AddStreamInfoIfNotPresent("Photo").SelfLink = new Uri(BaseUri + "/People(1)/Photo");
                using (var response = named
                    ? await context.GetReadStreamAsync(person, "Photo", new DataServiceRequestArgs(), CancellationToken.None)
                    : await context.GetReadStreamAsync(person, new DataServiceRequestArgs(), CancellationToken.None))
                using (var reader = new StreamReader(response.Stream))
                {
                    Assert.Equal("stream content", await reader.ReadToEndAsync());
                }

                Assert.Single(handler.Requests);
            }
        }

        [Fact]
        public async Task SaveChangesAsync_UpdatesEntityAndFiresChangesSaved()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var person = new TestPerson { Id = 1, Name = "Updated" };
                context.AttachTo("People", person);
                context.UpdateObject(person);
                int saved = 0;
                context.ChangesSaved += (sender, args) => saved++;
                var response = await context.SaveChangesAsync(CancellationToken.None);
                Assert.Equal(204, Assert.Single(response).StatusCode);
                Assert.Equal(EntityStates.Unchanged, context.GetEntityDescriptor(person).State);
                Assert.Equal(1, saved);
                Assert.Single(handler.Requests);
            }
        }

        [Fact]
        public async Task ExecuteAsync_VoidResponsePreservesStatusAndMethod()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var response = await context.ExecuteAsync(new Uri(BaseUri + "/Operation"), "POST", CancellationToken.None);
                Assert.Equal(204, response.StatusCode);
                Assert.Equal("POST " + BaseUri + "/Operation", Assert.Single(handler.Requests));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task LoadPropertyAllPagesAsync_CompletesPagingBeforeLaterCancellation(bool fail)
        {
            var model = new Microsoft.OData.Edm.EdmModel();
            var customerType = new Microsoft.OData.Edm.EdmEntityType("NS", "TwoLevelCustomer2");
            customerType.AddKeys(customerType.AddStructuralProperty("CustomerID", Microsoft.OData.Edm.EdmPrimitiveTypeKind.String));
            var orderType = new Microsoft.OData.Edm.EdmEntityType("NS", "Order");
            orderType.AddKeys(orderType.AddStructuralProperty("OrderID", Microsoft.OData.Edm.EdmPrimitiveTypeKind.Int32));
            var ordersProperty = customerType.AddUnidirectionalNavigation(new Microsoft.OData.Edm.EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = Microsoft.OData.Edm.EdmMultiplicity.Many });
            model.AddElement(customerType);
            model.AddElement(orderType);
            var container = new Microsoft.OData.Edm.EdmEntityContainer("NS", "Container");
            var customers = container.AddEntitySet("Customers", customerType);
            customers.AddNavigationTarget(ordersProperty, container.AddEntitySet("Orders", orderType));
            model.AddElement(container);
            int requests = 0;
            using (var handler = new MockHttpClientHandler(request =>
            {
                int page = ++requests;
                string payload = page == 1
                    ? "{\"@odata.context\":\"http://service.org/$metadata#Orders\",\"value\":[{\"OrderID\":1}],\"@odata.nextLink\":\"http://service.org/Customers('1')/Orders?$skip=1\"}"
                    : "{\"@odata.context\":\"http://service.org/$metadata#Orders\",\"value\":[{\"OrderID\":2}]}";
                return new HttpResponseMessage(fail && page == 2 ? HttpStatusCode.NotFound : HttpStatusCode.OK) { Content = new StringContent(payload, Encoding.UTF8, "application/json") };
            }))
            using (var cancellation = new CancellationTokenSource())
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.ResolveName = type => "NS." + type.Name;
                context.Format.UseJson(model);
                var customer = new ALinq.PreserveTypesAndAttributesTests.TwoLevelCustomer2 { Id = "1", OrdersList = new List<ALinq.PreserveTypesAndAttributesTests.Order>() };
                context.AttachTo("Customers", customer);
                if (fail)
                {
                    var error = await Assert.ThrowsAsync<AggregateException>(() => context.LoadPropertyAllPagesAsync(customer, "Orders", cancellation.Token));
                    Assert.IsAssignableFrom<InvalidOperationException>(error.InnerException);
                }
                else
                {
                    var response = await context.LoadPropertyAllPagesAsync(customer, "Orders", cancellation.Token);
                    Assert.Equal(200, response.StatusCode);
                    Assert.Equal(new[] { 1, 2 }, customer.OrdersList.Select(order => order.OrderID));
                }

                cancellation.Cancel();
                Assert.True(cancellation.IsCancellationRequested);
                Assert.Equal(2, requests);
            }
        }

        [Fact]
        public async Task ExecuteBatchAsync_ReturnsQueryResponse()
        {
            string payload = "--batchresponse\r\nContent-Type: application/http\r\nContent-Transfer-Encoding: binary\r\n\r\nHTTP/1.1 200 OK\r\nContent-Type: application/json\r\n\r\n{\"@odata.context\":\"http://service.org/$metadata#People\",\"value\":[{\"Id\":1,\"Name\":\"First\"}]}\r\n--batchresponse--\r\n";
            using (var handler = new MockHttpClientHandler(request =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(payload) };
                response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/mixed; boundary=batchresponse");
                return response;
            }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var response = await context.ExecuteBatchAsync(CancellationToken.None, context.CreateQuery<TestPerson>("People"));
                var queryResponse = Assert.IsType<QueryOperationResponse<TestPerson>>(Assert.Single(response));
                Assert.Equal("First", Assert.Single(queryResponse).Name);
                Assert.Equal(200, queryResponse.StatusCode);
                Assert.Equal("POST " + BaseUri + "/$batch", Assert.Single(handler.Requests));
            }
        }

        [Fact]
        public async Task ActionGetValueAsync_ReturnsScalarResponse()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"@odata.context\":\"http://service.org/$metadata#Edm.String\",\"value\":\"result\"}", Encoding.UTF8, "application/json") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                var action = new DataServiceActionQuerySingle<string>(context, BaseUri + "/Operation", new BodyOperationParameter("input", "value"));
                Assert.Equal("result", await action.GetValueAsync(CancellationToken.None));
                Assert.Equal("POST " + BaseUri + "/Operation", Assert.Single(handler.Requests));
            }
        }

        [Fact]
        public async Task ExecuteAsync_NotFoundHonorsIgnoreResourceNotFound()
        {
            using (var handler = new MockHttpClientHandler(request => new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("", Encoding.UTF8, "application/json") }))
            using (var text = new StringReader(Edmx))
            using (var xml = XmlReader.Create(text))
            {
                var context = new DataServiceContext(new Uri(BaseUri)) { HttpClientHandlerProvider = new MockHttpClientHandlerProvider(handler), HttpRequestTransportMode = HttpRequestTransportMode.HttpClient };
                context.Format.UseJson(CsdlReader.Parse(xml));
                await Assert.ThrowsAsync<DataServiceQueryException>(() => context.ExecuteAsync<TestPerson>(new Uri(BaseUri + "/People")));
                context.IgnoreResourceNotFoundException = true;
                Assert.Empty(await context.ExecuteAsync<TestPerson>(new Uri(BaseUri + "/People")));
            }
        }

        private const string BaseUri = "http://service.org";
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Microsoft.OData.Client.Tests"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""TestPerson"">
        <Key><PropertyRef Name=""Id"" /> </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
      </EntityType>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""People"" EntityType=""Microsoft.OData.Client.Tests.TestPerson"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string PersonNameValue = "John Doe";

        /// <summary>
        /// Asserts that a dynamic property which represents a collection of untyped values throws an exception when the
        /// <see cref="ODataMessageReaderSettings.EnableUntypedCollections"/> behavior flag is not enabled, and that the exception is not present when the flag is
        /// enabled
        /// </summary>
        [Fact]
        public void DynamicPropertyCollectionOfUntypedValues()
        { 
            var rootUri = new Uri(BaseUri);
            var entitySet = "users";
            using (var handler = new MockHttpClientHandler(request =>
            {
                var assembly = this.GetType().Assembly;
                var resourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith("UntypedCollection.failedresponse.json"));
                using (var streamReader = new StreamReader(assembly.GetManifestResourceStream(resourcePath)))
                {
                    var contents = streamReader.ReadToEnd();
                    var stringContent = new StringContent(contents, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = stringContent,
                        };
                        try
                        {
                            response.Headers.Add(
                                "Location",
                                new Uri(rootUri, $"{entitySet}/080a1a0e-71bf-4582-b141-fd61bdd35a40").ToString());
                            return response;
                        }
                        catch
                        {
                            response.Dispose();
                            throw;
                        }
                    }
                    catch
                    {
                        stringContent.Dispose();
                        throw;
                    }
                }
            }))
            {
                var provider = new MockHttpClientHandlerProvider(handler);
                Func<Uri, string, IHttpClientHandlerProvider, DataServiceContext> createRequestForEntityWithDynamicCollectionOfUntypedValues = 
                    (uri, set, handlerProvider) =>
                    {
                        var directoryDataService = new DirectoryDataService(rootUri);
                        directoryDataService.HttpClientHandlerProvider = provider;
                        directoryDataService.HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;

                        directoryDataService.AddObject(entitySet, new User());

                        return directoryDataService;
                    };

                var context = createRequestForEntityWithDynamicCollectionOfUntypedValues(rootUri, entitySet, provider);

                // This test is asserting behavior about a bug that has been fixed in order to demonstrate the difference between the fixed behavior and the bugged
                // behavior. The bugged behavior fails a <see cref="System.Diagnostics.Debug.Assert(bool)"/>, so we don't run the test with the behavior flag disabled in
                // the Debug configuration
#if !DEBUG
                // when disabled, we expect an exception due to a bug
                context.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated(args => args.Settings.EnableUntypedCollections = false);
                Assert.Throws<DataServiceRequestException>(() => context.SaveChanges());
#endif

                context = createRequestForEntityWithDynamicCollectionOfUntypedValues(rootUri, entitySet, provider);

                // when enabled, the exception should go away and we should receive an actual repsonse
                context.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated(args => args.Settings.EnableUntypedCollections = true);
                var response = context.SaveChanges();
                Assert.True(response.First().StatusCode >= 200 && response.First().StatusCode < 300);
            }
        }

        [Fact]
        public async Task UsesProvidedHttpClientToMakeRequest()
        {
            using (var handler = new MockHttpClientHandler(HandleRequest))
            {
                var provider = new MockHttpClientHandlerProvider(handler);

                var context = new DataServiceContext(new Uri(BaseUri));
                context.HttpClientHandlerProvider = provider;
                context.HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;

                var nameQuery = new DataServiceQuerySingle<string>(context, "People(1)/Name/$value");
                string personName = await nameQuery.GetValueAsync();

                Assert.Equal(2, handler.Requests.Count);
                Assert.Equal(2, provider.NumCalls);
                Assert.Equal($"GET {BaseUri}/$metadata", handler.Requests[0]);
                Assert.Equal($"GET {BaseUri}/People(1)/Name/$value", handler.Requests[1]);
                Assert.Equal(PersonNameValue, personName);
            }
        }

        private HttpResponseMessage HandleRequest(HttpRequestMessage request)
        {
            string contents;

            if (request.RequestUri.AbsolutePath.EndsWith("$metadata"))
            {
                contents = Edmx;
            }
            else if (request.RequestUri.AbsolutePath.EndsWith("Name/$value"))
            {
                contents = PersonNameValue;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(contents)
            };

            return response;
        }
    }

    [Key("Id")]
    public class TestPerson : BaseEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
