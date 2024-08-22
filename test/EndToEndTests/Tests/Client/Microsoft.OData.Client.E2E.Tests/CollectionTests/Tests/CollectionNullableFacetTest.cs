//-----------------------------------------------------------------------------
// <copyright file="CollectionNullableFacetTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.CollectionTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.CollectionTests.Tests
{
    public class CollectionNullableFacetTest : EndToEndTestBase<CollectionNullableFacetTest.TestsStartup>
    {
        private readonly Uri _baseUri;
        private IEdmModel _model = null;
        private readonly Container _context;
        private static string NameSpacePrefix = "Microsoft.OData.Client.E2E.Tests.Common.Server.Default.";
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
        public void CollectionNullableFalseInStructrualProperty()
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
            this.UpdateEntityWithCollectionContainsNull(personToAdd, "Numbers");
        }

        /// <summary>
        /// Verify collection in structrual property with nullable facet specified false can have null element
        /// And collection can be empty
        /// </summary>
        [Fact]
        public void CollectionNullableTrueInStructrualProperty()
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
            this.UpdateEntityWithCollectionContainsNull(personToAdd, "Emails");
        }

        #region private methods
        /// <summary>
        /// Update entity with null element in collection 
        /// testProperty is a structual property and its collection value contains null element 
        /// </summary>
        private void UpdateEntityWithCollectionContainsNull(ODataResource personToAdd, String testProperty)
        {
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = _baseUri;
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

            var args = new DataServiceClientRequestMessageArgs(
                "PUT",
                new Uri(_baseUri.AbsoluteUri + "Customers(1)", UriKind.Absolute),
                usePostTunneling: false,
                new Dictionary<string, string>(),
                HttpClientFactory);

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);

                try
                {
                    //write request message
                    using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
                    {
                        var odataWriter = messageWriter.CreateODataResourceWriter(customerSet, customerType);
                        odataWriter.WriteStart(personToAdd);
                        odataWriter.WriteEnd();
                    }

                    // send the http request
                    var responseMessage = requestMessage.GetResponse();

                    // verify the update
                    Assert.Equal(204, responseMessage.StatusCode);
                    ODataResource updatedProduct = this.QueryEntityItem("Customers(1)") as ODataResource;
                    ODataCollectionValue testCollection = updatedProduct.Properties.OfType<ODataProperty>().Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    ODataCollectionValue expectValue = personToAdd.Properties.OfType<ODataProperty>().Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    var actIter = testCollection.Items.GetEnumerator();
                    var expIter = expectValue.Items.GetEnumerator();
                    while ((actIter.MoveNext()) && (expIter.MoveNext()))
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

        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            var args = new DataServiceClientRequestMessageArgs(
                "GET",
                new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute),
                usePostTunneling: false,
                new Dictionary<string, string>(),
                HttpClientFactory);

            var queryRequestMessage = new HttpClientRequestMessage(args);
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
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
