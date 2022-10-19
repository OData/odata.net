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

        [Fact]
        public void RdsExtendedTest()
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
                                Combine(rootUri, entitySet, "080a1a0e-71bf-4582-b141-fd61bdd35a40").ToString());
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
                var context = new DirectoryDataService(rootUri);
                context.HttpClientHandlerProvider = provider;
                context.HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;

                context.AddObject(entitySet, new User());
                context.SaveChanges();
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

        /// <summary>
        /// Creates a new <see cref="Uri"/> by combining <paramref name="uri"/> with each element of <paramref name="uris"/>
        /// </summary>
        /// <param name="uri">The base URI that will be added to</param>
        /// <param name="uris">The URIs to combine with <paramref name="uri"/></param>
        /// <returns>The combination of <paramref name="uri"/> and <paramref name="uris"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> or <paramref name="uris"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="uri"/> is not an absolute URI or if one of <paramref name="uris"/> is not a relative URI
        /// </exception>
        private static Uri Combine(Uri uri, params string[] uris)
        {
            return Combine(uri, uris.Select(_ => new Uri(_, UriKind.Relative)));
        }

        /// <summary>
        /// Creates a new <see cref="Uri"/> by combining <paramref name="uri"/> with each element of <paramref name="uris"/>
        /// </summary>
        /// <param name="uri">The base URI that will be added to</param>
        /// <param name="uris">The <see cref="Uri"/>s to combine with <paramref name="uri"/></param>
        /// <returns>The combination of <paramref name="uri"/> and <paramref name="uris"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> or <paramref name="uris"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is not an absolute URI</exception>
        private static Uri Combine(Uri uri, IEnumerable<Uri> uris)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The URI is not an absolute URI", nameof(uri));
            }

            if (uris == null)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            var result = uri;
            foreach (var relativeUri in uris)
            {
                if (relativeUri.IsAbsoluteUri)
                {
                    result = relativeUri;
                }
                else
                {
                    result = new Uri(result, relativeUri);
                }
            }

            return result;
        }
    }

    [Key("Id")]
    public class TestPerson : BaseEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
