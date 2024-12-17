//---------------------------------------------------------------------
// <copyright file="EntityReferenceLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests
{
    public class EntityReferenceLinkTests : EndToEndTestBase<EntityReferenceLinkTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(QueryOptionTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt =>
                    opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public EntityReferenceLinkTests(TestWebApplicationFactory<EntityReferenceLinkTests.TestsStartup> fixture) 
            : base(fixture)
        {
            if (Client.BaseAddress == null)
            {
                throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
            }

            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        public static IEnumerable<object[]> MimeTypesData
        {
            get
            {
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
            }
        }

        #region $ref link with annotation

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task EntityReferenceLinkWithAnnotationShouldWorkAsync(string mimeType)
        {
            // Arrange & Act
            var link = await this.TestsHelper.QueryReferenceLinkAsync("People(2)/Parent/$ref", mimeType);

            // Assert
            Assert.NotNull(link);
            Assert.Single(link.InstanceAnnotations);

            var annotation = link.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Link.Annotation");
            Assert.NotNull(annotation);
            AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
        }

        #endregion

        #region $ref links with annotation

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task EntryWithAnnotationInReferenceLinksShouldWorkAsync(string mimeType)
        {
            // Arrange & Act
            var links = await this.TestsHelper.QueryReferenceLinksAsync("Products(5)/Details/$ref", mimeType);

            // Assert
            Assert.NotNull(links);
            Assert.Single(links.InstanceAnnotations);

            var annotation = links.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.Annotation");
            Assert.NotNull(annotation);
            AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
        }

        #endregion

        #region $ref - Entry With Annotation In Reference Link

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task ODataEntryWithAnnotationInReferenceLinkShouldWorkAsync(string mimeType)
        {
            // Arrange & Act
            var result = await this.TestsHelper.QueryResourceEntriesAsync("People(2)?$expand=Parent/$ref", mimeType);
            var entries = result.Where(e => e != null && (e.TypeName.EndsWith("Customer") || e.TypeName.EndsWith("Person"))).ToList();

            // Assert
            Assert.Equal(2, entries.Count);

            var annotation = entries.First().InstanceAnnotations.FirstOrDefault(ia => ia.Name == "Link.AnnotationByEntry");
            Assert.NotNull(annotation);
            AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
        }

        #endregion

        #region $ref - Entry With Annotation In Reference Links

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task ODataEntryWithAnnotationInReferenceLinksShouldWorkAsync(string mimeType)
        {
            // Arrange & Act
            var feed = await this.TestsHelper.QueryInnerFeedAsync("Products(5)?$expand=Details/$ref", mimeType);

            // Assert
            Assert.NotNull(feed);
            Assert.Single(feed.InstanceAnnotations);

            var annotation = feed.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.AnnotationByFeed");
            Assert.NotNull(annotation);
            AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
        }

        #endregion

        #region Private methods

        private static void AssertODataPrimitiveValueEqual(ODataValue firstValue, ODataValue secondValue)
        {
            Assert.NotNull(firstValue);
            Assert.NotNull(secondValue);

            var firstPrimitiveValue = firstValue as ODataPrimitiveValue;
            Assert.NotNull(firstPrimitiveValue);

            var secondPrimitiveValue = secondValue as ODataPrimitiveValue;
            Assert.NotNull(secondPrimitiveValue);

            Assert.Equal(firstPrimitiveValue.Value, secondPrimitiveValue.Value);
        }

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(_baseUri, _model, Client);
            }
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "queryoption/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }

        #endregion
    }
}
