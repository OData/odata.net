//-----------------------------------------------------------------------------
// <copyright file="CollectionNullableFacetTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.E2E.Tests.CollectionTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.CollectionTests.Tests
{
    public class CollectionNullableFacetTest : EndToEndTestBase<CollectionNullableFacetTest.TestsStartup>
    {
        private readonly Uri _baseUri;
        private IEdmModel _model = null;
        private readonly Container _context;
        private static string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default.";
        protected readonly string[] mimeTypes =
        [
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        ];

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(CollectionNullableFacetTestController), typeof(MetadataController));

                services.AddControllers().AddOData(opt =>
                {
                    opt.EnableQueryFeatures();
                    opt.AddRouteComponents("odata", DefaultEdmModel.GetEdmModel());
                });
            }
        }

        public CollectionNullableFacetTest(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = DefaultEdmModel.GetEdmModel();
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDefaultDataSource();
        }

        /// <summary>
        /// Verify collection structrual property with nullable facet specified false cannot have null element
        /// And collection can be empty
        /// </summary>
        [Fact]
        public async Task CollectionNullableFalseInStructrualProperty()
        {
            var personToAdd = new ODataResource
            {
                TypeName = NameSpacePrefix + "Customer",
                Properties = new[]
                {
                    new ODataProperty {Name = "Numbers", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new[] {"222-222-221", null}}},
                    new ODataProperty {Name = "Emails", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new string[] {}}}
                }
            };

            await UpdateEntityWithCollectionContainsNullAsync(personToAdd, "Numbers");
        }

        /// <summary>
        /// Verify collection in structrual property with nullable facet specified false can have null element
        /// And collection can be empty
        /// </summary>
        [Fact]
        public async Task CollectionNullableTrueInStructrualProperty()
        {
            var personToAdd = new ODataResource
            {
                TypeName = NameSpacePrefix + "Customer",
                Properties = new[]
                {
                    new ODataProperty {Name = "Numbers", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new string[] {}}},
                    new ODataProperty {Name = "Emails", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new[] {"a@a.b", "b@b.b", null}}}
                }
            };

            await UpdateEntityWithCollectionContainsNullAsync(personToAdd, "Emails");
        }

        #region private methods
        /// <summary>
        /// Update entity with null element in collection 
        /// testProperty is a structual property and its collection value contains null element 
        /// </summary>
        private async Task UpdateEntityWithCollectionContainsNullAsync(ODataResource personToAdd, string testProperty)
        {
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = _model.EntityContainer.FindEntitySet("Customers");

            // get the IsNullable value of testProperty
            bool isNullable = true;
            foreach (IEdmStructuralProperty property in customerType.BaseEntityType().DeclaredStructuralProperties())
            {
                if (property.Name.Equals(testProperty))
                {
                    IEdmCollectionTypeReference typeRef = property.Type as IEdmCollectionTypeReference;
                    Assert.NotNull(typeRef);
                    isNullable = typeRef.IsNullable;
                }
            }

            foreach (var mimeType in mimeTypes)
            {
                var requestUri = new Uri(_baseUri.AbsoluteUri + "Customers(1)", UriKind.Absolute);

                var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
                {
                    Method = "PUT"
                };

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);

                try
                {
                    //write request message
                    using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
                    {
                        var odataWriter = await messageWriter.CreateODataResourceWriterAsync(customerSet, customerType);
                        await odataWriter.WriteStartAsync(personToAdd);
                        await odataWriter.WriteEndAsync();
                    }

                    // send the http request
                    var responseMessage = await requestMessage.GetResponseAsync();

                    // verify the update
                    Assert.Equal(204, responseMessage.StatusCode);
                    ODataResource updatedProduct = await QueryEntityItemAsync("Customers(1)") as ODataResource;
                    ODataCollectionValue testCollection = updatedProduct.Properties.OfType<ODataProperty>().Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    ODataCollectionValue expectValue = personToAdd.Properties.OfType<ODataProperty>().Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    var actIter = testCollection.Items.GetEnumerator();
                    var expIter = expectValue.Items.GetEnumerator();
                    while (actIter.MoveNext() && expIter.MoveNext())
                    {
                        Assert.Equal(actIter.Current, expIter.Current);
                    }
                }
                catch (Exception exception)
                {
                    if (!isNullable)
                    {
                        Assert.Equal(exception.Message, "A null value was detected in the items of a collection property value; non-nullable instances of collection types do not support null values as items.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private async Task<ODataItem> QueryEntityItemAsync(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute);

            var queryRequestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "GET"
            };

            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = await queryRequestMessage.GetResponseAsync();
            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model))
                {
                    var reader = await messageReader.CreateODataResourceReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            item = reader.Item;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }
        #endregion

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "collectionnullable/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
