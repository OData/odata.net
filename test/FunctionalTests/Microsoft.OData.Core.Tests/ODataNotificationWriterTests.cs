//---------------------------------------------------------------------
// <copyright file="ODataNotificationWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNotificationWriterTests
    {
        private MemoryStream stream;
        private TextWriter writer;
        private IODataStreamListener streamListener;

        public ODataNotificationWriterTests()
        {
            this.stream = new MemoryStream();
            this.writer = new StreamWriter(this.stream);
            this.streamListener = new MockODataStreamListener(this.writer);
        }

        [Fact]
        public void NotificationWriterDisposeShouldInvokeStreamDisposed()
        {
            // We care about the notification writer being disposed
            // We don't care about the writer passed to the notification writer
            using (var notificationWriter = new ODataNotificationWriter(
                new StreamWriter(new MemoryStream()),
                this.streamListener))
            {

            }

            var result = ReadStreamContents();

            Assert.Equal("StreamDisposed", result);
        }

#if NETCOREAPP3_1
        [Fact]
        public async Task NotificationWriterDisposeShouldInvokeStreamDisposedAsync()
        {
            await using (var notificationWriter = new ODataNotificationWriter(
                new StreamWriter(new MemoryStream()),
                this.streamListener)) // `synchronous` argument becomes irrelevant
            {

            }

            var result = await this.ReadStreamContentsAsync();

            Assert.Equal("StreamDisposedAsync", result);
        }

#else
        [Fact]
        public async Task NotificationWriterDisposeShouldInvokeStreamDisposedAsync()
        {
            using (var notificationWriter = new ODataNotificationWriter(
                new StreamWriter(new MemoryStream()),
                this.streamListener,
                /*synchronous*/ false))
            {

            }

            var result = await this.ReadStreamContentsAsync();

            Assert.Equal("StreamDisposedAsync", result);
        }
#endif

        private string ReadStreamContents()
        {
            this.stream.Position = 0;
            return new StreamReader(this.stream).ReadToEnd();
        }

        private Task<string> ReadStreamContentsAsync()
        {
            this.stream.Position = 0;
            return new StreamReader(this.stream).ReadToEndAsync();
        }

        private class MockODataStreamListener : IODataStreamListener
        {
            private TextWriter writer;

            public MockODataStreamListener(TextWriter writer)
            {
                this.writer = writer;
            }

            public void StreamDisposed()
            {
                writer.Write("StreamDisposed");
                writer.Flush();
            }

            public async Task StreamDisposedAsync()
            {
                await writer.WriteAsync("StreamDisposedAsync").ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            public void StreamRequested()
            {
                throw new NotImplementedException();
            }

            public Task StreamRequestedAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}
