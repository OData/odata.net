//---------------------------------------------------------------------
// <copyright file="MismatchedClientModelWithoutTypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ClientWithoutTypeResolverTests
{
    using System;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultClientTypeMismatchServiceReference;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Tests for the scenario where a client side model has slight differences to the server model,
    /// and does not specify a type or name resolver. These tests cover cases that the product should
    /// support successful queries against the mismatched model.
    /// </summary>
    public class MismatchedClientModelWithoutTypeResolverTests : EndToEndTestBase
    {
        public MismatchedClientModelWithoutTypeResolverTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        [Fact]
        public void FeedQuery()
        {
            var contextWrapper = this.CreateContextWrapper();

            var customerResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer")).ToArray();
            var messageResults = contextWrapper.Execute<Message>(new Uri(this.ServiceUri.OriginalString + "/Message")).ToArray();
            var pageViewResults = contextWrapper.Execute<PageView>(new Uri(this.ServiceUri.OriginalString + "/PageView")).ToArray();
        }

        [Fact]
        public void ExpandedFeedQuery()
        {
            var contextWrapper = this.CreateContextWrapper();
            var messageWithAttachmentsResults = contextWrapper.CreateQuery<Message>("Message").Expand(m => m.Attachments).ToArray();
        }

        [Fact]
        public void EntryQuery()
        {
            var contextWrapper = this.CreateContextWrapper();

            var customerResults = contextWrapper.CreateQuery<Customer>("Customer").Where(c => c.CustomerId == -10).Single();
            var pageViewResults = contextWrapper.CreateQuery<PageView>("PageView").Where(p => p.PageViewId == -2).Single();
        }

        [Fact]
        public void LoadProperty()
        {
            var contextWrapper = this.CreateContextWrapper();

            foreach (var message in contextWrapper.Context.Message)
            {
                contextWrapper.LoadProperty(message, "Attachments");
                Guid testGuid;
                foreach (var attachment in message.Attachments)
                {
                    // AttachmentId is a string on client and a guid on the server
                    Assert.True(Guid.TryParse(attachment.AttachmentId, out testGuid), "Failed to parse attachment id as guid");
                }
            } 
        }

        [Fact]
        public void PropertyQuery()
        {
            var contextWrapper = this.CreateContextWrapper();

            var customerIdResults = contextWrapper.Context.Customer.Select(c => new { c.CustomerId }).ToArray();
            var messageIdResults = contextWrapper.Context.Message.Select(m => new { m.MessageId }).ToArray();
            int testInt;
            foreach (var messageIdResult in messageIdResults)
            {
                // Message.MessageId is string on client and int32 on server
                Assert.True(Int32.TryParse(messageIdResult.MessageId, out testInt), "Failed to parse message.messageid as int32");     
            }

            var messageIsReadResults = contextWrapper.Context.Message.Select(m => new { m.IsRead }).ToArray();
            bool testBool;
            foreach (var messageIsReadResult in messageIsReadResults)
            {
                // Message.IsRead is string on client and boolean on server
                Assert.True(bool.TryParse(messageIsReadResult.IsRead, out testBool), "Failed to parse message.isread as boolean");     
            }

            var viewedResults = contextWrapper.Context.PageView.Select(p => new { p.Viewed }).ToArray();
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContextWrapper()
        {
            // Return a context based on similar but not exactly matching client types.
            var contextWrapper = new DataServiceContextWrapper<DefaultContainer>(new DefaultContainer(this.ServiceUri));
            contextWrapper.Format.UseJson();
            contextWrapper.ResolveName = null;
            contextWrapper.ResolveType = null;

            return contextWrapper;
        }
    }
}
