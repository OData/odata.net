//---------------------------------------------------------------------
// <copyright file="DataServiceContextHttpClientHandlerProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OData.Client.Tests.Serialization;
using Microsoft.OData.Edm.Csdl;
using Microsoft.WindowsAzure.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
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

            private const string ExpectedRequestPayloadUsingJson1 = @"{
      ""requests"": [
        {
          ""id"": ""1"",
          ""atomicityGroup"": ""93dd314c-48e2-442c-a016-d17be61cc7f0"",
          ""method"": ""POST"",
          ""url"": ""http://service.org/Banks"",
          ""headers"": {
            ""odata-version"": ""4.0"",
            ""odata-maxversion"": ""4.0"",
            ""content-type"": ""application/json;odata.metadata=minimal"",
            ""accept"": ""application/json;odata.metadata=minimal"",
            ""accept-charset"": ""UTF-8"",
            ""user-agent"": ""Microsoft.OData.Client/8.0.1""
          },
          ""body"": {
            ""Id"": 45,
            ""Name"": ""Test Bank""
          }
        },
        {
          ""id"": ""2"",
          ""atomicityGroup"": ""93dd314c-48e2-442c-a016-d17be61cc7f0"",
          ""dependsOn"": [""1""],
          ""method"": ""POST"",
          ""url"": ""http://service.org/$1/BankAccounts"",
          ""headers"": {
            ""odata-version"": ""4.0"",
            ""odata-maxversion"": ""4.0"",
            ""content-type"": ""application/json;odata.metadata=minimal"",
            ""accept"": ""application/json;odata.metadata=minimal"",
            ""accept-charset"": ""UTF-8"",
            ""user-agent"": ""Microsoft.OData.Client/8.0.1""
          },
          ""body"": {
            ""Id"": 890
          }
        }
      ]
    }
    ";

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

       

        [Fact]
        public void TestDependsOnIDJsonBatchSingleChangeSetSupportAddObjectAPITest()
        {
            using (var handler = new MockHttpClientHandler((httpRequest) =>
            {
                var contents = httpRequest.Content.ReadAsStringAsync().Result;
                var stringContent = new StringContent(contents, Encoding.UTF8, "application/json");

                Assert.Contains("\"dependsOn\":[\"1\"]", contents);

                var normalizedRequestPayload = GetNormalizedJsonMessage(contents);
                var normalizedExpectedPayload = GetNormalizedJsonMessage(ExpectedRequestPayloadUsingJson1);

                Assert.Equal(normalizedExpectedPayload, normalizedRequestPayload);

                var response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("odata.metadata", "minimal"));
                response.Headers.Add("OData-Version", "4.0");
                response.Headers.Add("OData-MaxVersion", "4.0");
                return response;
            } ))
            {
                var provider = new MockHttpClientFactory(handler);

                var context = new Container(new Uri(BaseUri));
                context.HttpClientFactory = provider;

                // Create new Bank object
                var bank = new Bank
                {
                    Id = 45,
                    Name = "Test Bank",
                    BankAccounts = new List<BankAccount>()
                };

                // Create new BankAccount object
                var bankAccount = new BankAccount
                {
                    Id = 890
                };

                // Establish the relationship between Bank and BankAccount
                bank.BankAccounts.Add(bankAccount);

                // Add the Bank entity to the context
                context.AddObject("Banks", bank);

                // Add the related BankAccount entity
                context.AddRelatedObject(bank, "BankAccounts", bankAccount);

                context.BeginSaveChanges(
                SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch,
                (ar) =>
                {
                    var response = context.EndSaveChanges(ar);
                },
                null);
            }
        }

        private string GetNormalizedJsonMessage(string jsonMessage)
        {
            const string myIdProperty = @"""id"":""my_id_guid""";
            const string myAtomicGroupProperty = @"""atomicityGroup"":""my_groupid_guid""";
            const string myDependsOnProperty = @"""dependsOn"":""[my_ids]""";
            const string myODataVersionProperty = @"""odata-version"":""myODataVer""";

            string result = Regex.Replace(jsonMessage, @"\s*", "", RegexOptions.Multiline);
            result = Regex.Replace(result, "\"id\":\"[^\"]*\"", myIdProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"atomicityGroup\":\"[^\"]*\"", myAtomicGroupProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"dependsOn\":\\[\"[^\\]]*\\]", myDependsOnProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"odata-version\":\"[^\"]*\"", myODataVersionProperty, RegexOptions.Multiline);
            return result;
        }
    }

    [Key("Id")]
    public class TestPerson : BaseEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Key("Id")]
    public class Bank : BaseEntityType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<BankAccount> BankAccounts { get; set; }
    }

    [Key("Id")]
    public class BankAccount
    {
        public int Id { get; set; }
    }

    class Container : DataServiceContext
    {
        public Container(Uri serviceRoot) :
            base(serviceRoot, ODataProtocolVersion.V4)
        {
            Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(@"
        <edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
          <edmx:DataServices>
            <Schema Namespace=""Microsoft.OData.Client.Tests"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      
              <!-- Entity Type: Bank -->
              <EntityType Name=""Bank"">
                <Key>
                  <PropertyRef Name=""Id""/>
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false""/>
                <NavigationProperty Name=""BankAccounts"" Type=""Collection(Microsoft.OData.Client.Tests.BankAccount)""/>
              </EntityType>
      
              <!-- Entity Type: BankAccount -->
              <EntityType Name=""BankAccount"">
                <Key>
                  <PropertyRef Name=""Id""/>
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                <NavigationProperty Name=""Bank"" Type=""Microsoft.OData.Client.Tests.Bank""/>
              </EntityType>

              <!-- Entity Container -->
              <EntityContainer Name=""Container"">
                <EntitySet Name=""Banks"" EntityType=""Microsoft.OData.Client.Tests.Bank"">
                  <NavigationPropertyBinding Path=""BankAccounts"" Target=""BankAccounts""/>
                </EntitySet>
              </EntityContainer>

            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>
        ")));
            Format.UseJson();
            Banks = base.CreateQuery<Bank>("Banks");
            BankAccounts = base.CreateQuery<BankAccount>("BankAccounts");
        }

        public DataServiceQuery<Bank> Banks { get; private set; }
        public DataServiceQuery<BankAccount> BankAccounts { get; private set; }
    }
}
