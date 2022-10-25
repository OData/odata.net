//---------------------------------------------------------------------
// <copyright file="DataServiceContextHttpClientHandlerProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using Microsoft.OData.Client.Tests.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

/*namespace Microsoft.DirectoryServices
{
    [EntityType]
    [Key(nameof(Id))]
    public class DirectoryObject
    {
        public string Id { get; set; }
    }

    [EntityType]
    public class User : DirectoryObject
    {
        public bool AccountEnabled { get; set; }

        public string City { get; set; }

        public string GivenName { get; set; }

        public string MailNickname { get; set; }

        public string Password { get; set; }

        public string StreetAddress { get; set; }

        public string Surname { get; set; }

        public string UsageLocation { get; set; }

        public AuthorizationInfo AuthorizationInfo { get; set; }

        public string UserPrincipalName { get; set; }

        public string DisplayName { get; set; }
    }

    public class AuthorizationInfo //// TODO
    {
    }
}*/

namespace Microsoft.OData.Client.Tests
{
    using Microsoft.WindowsAzure.ActiveDirectory;

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

        [Fact]
        public void RdsExtendedTest()
        {
            var rootUri = new Uri(BaseUri);
            var entitySet = "users";
            using (var handler = new MockHttpClientHandler(request =>
            {
                string contents;
                if (request.RequestUri.AbsolutePath.EndsWith("$metadata"))
                {
                    // NOTE: if the property *is* in the metadata, the rds test passes
                    // TODO
                    // try to repro a nested collection
                    //
                    // checkout correct version
                    // try with full class set
                    contents = System.IO.File.ReadAllText(@"c:\users\gdebruin\desktop\metadata.txt");
                }
                else if (request.RequestUri.AbsolutePath.Contains(entitySet))
                {
                    contents = System.IO.File.ReadAllText(@"c:\users\gdebruin\desktop\failedresponse.json");
                    //// this still passed the test
                    ////contents = System.IO.File.ReadAllText(@"c:\users\gdebruin\desktop\nestedcollection.json");
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                var stringContent = new StringContent(contents, Encoding.UTF8, "application/json");
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = stringContent,
                };
                response.Headers.Add("Location", new Uri(new Uri(rootUri, entitySet), new Uri("080a1a0e-71bf-4582-b141-fd61bdd35a40", UriKind.Relative)).ToString());
                return response;
            }))
            {
                var provider = new MockHttpClientHandlerProvider(handler);

                ////var context = new DataServiceContext(rootUri);
                var context = new DirectoryDataService(rootUri);
                context.HttpClientHandlerProvider = provider;
                context.HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;

                //// minimal repro of rds extended test code
                {
                    var domainName = "garrett.com";
                    var user = GenerateRestUser(domainName);

                    context.AddObject(entitySet, user);
                    var response = context.SaveChanges();
                }
            }
        }

        public static User GenerateRestUser(string domainName)
        {
            var user = new User
            {
                accountEnabled = true,
                city = "Seattle",
                givenName = "emannevig", //// Utils.GetRandomString(),
                mailNickname = "emankcinliam", //// Utils.GetRandomString(30),
                ////Password = "drowssap", //// PasswordGenerator.GeneratePlaintextPassword(),
                streetAddress = "sserddateerts", ////  Utils.GetRandomString(),
                surname = "emanrus", ////  Utils.GetRandomString(),
                usageLocation = "US",
                authorizationInfo = new AuthorizationInfo(),
                userPrincipalName = string.Format("{0}U@{1}", "emanlapicnirpresu" /*Utils.GetRandomString()*/, domainName)
            };

            user.displayName = string.Format("{0} {1} REST", user.givenName, user.surname);

            return user;
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
