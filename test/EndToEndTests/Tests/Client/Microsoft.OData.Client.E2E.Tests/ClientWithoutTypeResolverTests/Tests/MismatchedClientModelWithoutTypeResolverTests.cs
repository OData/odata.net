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
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
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
        public void FeedQuery()
        { 
            var customerResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers")).ToArray();
            var messageResults = _context.Execute<Common.Clients.EndToEnd.Message>(new Uri(_baseUri.OriginalString + "Messages")).ToArray();
            var pageViewResults = _context.Execute<Common.Clients.EndToEnd.PageView>(new Uri(_baseUri.OriginalString + "PageViews")).ToArray();
        }

        [Fact]
        public void ExpandedFeedQuery()
        {
            var messageWithAttachmentsResults = _context.CreateQuery<Common.Clients.EndToEnd.Message>("Messages").Expand(m => m.Attachments).ToArray();
        }

        [Fact]
        public void EntryQuery()
        {
            var customerResults = _context.CreateQuery<Common.Clients.EndToEnd.Customer>("Customers").Where(c => c.CustomerId == -10).Single();
            var pageViewResults = _context.CreateQuery<Common.Clients.EndToEnd.PageView>("PageViews").Where(p => p.PageViewId == -2).Single();
        }

        [Fact]
        public void LoadProperty()
        {
            foreach (var message in _context.Messages)
            {
                _context.LoadProperty(message, "Attachments");
                Guid testGuid;
                foreach (Common.Clients.EndToEnd.MessageAttachment attachment in message.Attachments)
                {
                    // AttachmentId is a string on client and a guid on the server
                   // Assert.True(Guid.TryParse(attachment.AttachmentId, out testGuid), "Failed to parse attachment id as guid");
                }
            }
        }

        [Fact]
        public void PropertyQuery()
        {
            var customerIdResults = _context.Customers.Select(c => new { c.CustomerId }).ToArray();
            var messageIdResults = _context.Messages.Select(m => new { m.MessageId }).ToArray();
            int testInt;
            foreach (var messageIdResult in messageIdResults)
            {
                // Message.MessageId is string on client and int32 on server
                //Assert.True(Int32.TryParse(messageIdResult.MessageId, out testInt), "Failed to parse message.messageid as int32");
            }

            var messageIsReadResults = _context.Messages.Select(m => new { m.IsRead }).ToArray();
            bool testBool;
            foreach (var messageIsReadResult in messageIsReadResults)
            {
                // Message.IsRead is string on client and boolean on server
                //Assert.True(bool.TryParse(messageIsReadResult.IsRead, out testBool), "Failed to parse message.isread as boolean");
            }

            var viewedResults = _context.PageViews.Select(p => new { p.Viewed }).ToArray();
        }
    }
}
