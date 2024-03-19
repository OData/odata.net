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
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class DataServiceContextHttpClientHandlerProviderTests
    {
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
                var provider = new MockHttpClientFactory(handler);
                Func<Uri, string, IHttpClientFactory, DataServiceContext> createRequestForEntityWithDynamicCollectionOfUntypedValues = 
                    (uri, set, handlerProvider) =>
                    {
                        var directoryDataService = new DirectoryDataService(rootUri);
                        directoryDataService.HttpClientFactory = provider;

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
                var provider = new MockHttpClientFactory(handler);

                var context = new DataServiceContext(new Uri(BaseUri));
                context.HttpClientFactory = provider;

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
