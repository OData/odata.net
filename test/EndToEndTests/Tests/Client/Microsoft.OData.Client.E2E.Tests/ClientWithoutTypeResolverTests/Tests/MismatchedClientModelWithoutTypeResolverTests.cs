//-----------------------------------------------------------------------------
// <copyright file="MismatchedClientModelWithoutTypeResolverTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server;
using Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Clients.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Tests
{
    public class MismatchedClientModelWithoutTypeResolverTests : EndToEndTestBase<MismatchedClientModelWithoutTypeResolverTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(MismatchedClientModelWithoutTypeResolverTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.EnableQueryFeatures()
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public MismatchedClientModelWithoutTypeResolverTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void FeedQueries_ExecuteSuccessfully()
        {
            var customerResults = _context.Execute<Clients.Customer>(new Uri(_baseUri.OriginalString + "Customers")).ToArray();
            
            Assert.NotNull(customerResults);
            Assert.NotEmpty(customerResults);

            var messageResults = _context.Execute<Clients.Message>(new Uri(_baseUri.OriginalString + "Messages")).ToArray();

            Assert.NotNull(messageResults);
            Assert.NotEmpty(messageResults);

            var pageViewResults = _context.Execute<Clients.PageView>(new Uri(_baseUri.OriginalString + "PageViews")).ToArray();

            Assert.NotNull(pageViewResults);
            Assert.NotEmpty(pageViewResults);
        }

        [Fact]
        public void ExpandedFeedQueries_ExecuteSuccessfully()
        {
            var messageWithAttachmentsResults = _context.CreateQuery<Clients.Message>("Messages").Expand(m => m.Attachments).ToArray();

            // Assert
            Assert.NotNull(messageWithAttachmentsResults); // Check that the result array is not null
            Assert.NotEmpty(messageWithAttachmentsResults); // Check that the result array is not empty

            // Check that each message in the result has attachments and that the attachments are not null
            foreach (var message in messageWithAttachmentsResults)
            {
                Assert.NotNull(message);
                Assert.NotNull(message.Attachments);
                Assert.NotEmpty(message.Attachments);
            }
        }

        [Fact]
        public void SingleEntryQuery_ByKey_ExecutesSuccessfully()
        {
            var customerResults = _context.CreateQuery<Clients.Customer>("Customers").Where(c => c.CustomerId == -10).Single();

            // Assert
            Assert.NotNull(customerResults); // Check that the customer result is not null
            Assert.Equal(-10, customerResults.CustomerId); // Check that the retrieved customer has the expected ID

            var pageViewResults = _context.CreateQuery<Clients.PageView>("PageViews").Where(p => p.PageViewId == -2).Single();

            Assert.NotNull(pageViewResults); // Check that the page view result is not null
            Assert.Equal(-2, pageViewResults.PageViewId); // Check that the retrieved page view has the expected ID
        }

        [Fact]
        public void LoadProperty_LoadsProperties_Successfully()
        {
            foreach (var message in _context.Messages)
            {
                _context.LoadProperty(message, "Attachments");
                Guid testGuid;
                foreach (Clients.MessageAttachment attachment in message.Attachments)
                {
                    // AttachmentId is a string on client and a guid on the server
                    Assert.True(Guid.TryParse(attachment.AttachmentId, out testGuid), "Failed to parse attachment id as guid");
                }
            }
        }

        [Fact]
        public void PropertyQuery_WithMismatchedTypes_ExecutesSuccessfully()
        {
            var customerIdResults = _context.Customers.Select(c => new { c.CustomerId }).ToArray();
            var messageIdResults = _context.Messages.Select(m => new { m.MessageId }).ToArray();
            int testInt;
            foreach (var messageIdResult in messageIdResults)
            {
                // Message.MessageId is string on client and int32 on server
                Assert.True(Int32.TryParse(messageIdResult.MessageId, out testInt), "Failed to parse message.messageid as int32");
            }

            var messageIsReadResults = _context.Messages.Select(m => new { m.IsRead }).ToArray();
            bool testBool;
            foreach (var messageIsReadResult in messageIsReadResults)
            {
                // Message.IsRead is string on client and boolean on server
                Assert.True(bool.TryParse(messageIsReadResult.IsRead, out testBool), "Failed to parse message.isread as boolean");
            }

            var viewedResults = _context.PageViews.Select(p => new { p.Viewed }).ToArray();
        }
    }
}
