//---------------------------------------------------------------------
// <copyright file="ODataNotificationStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNotificationStreamTests
    {
        private MemoryStream stream;
        private TextWriter writer;
        private IODataStreamListener streamListener;

        public ODataNotificationStreamTests()
        {
            this.stream = new MemoryStream();
            this.writer = new StreamWriter(this.stream);
            this.streamListener = new MockODataStreamListener(this.writer);
        }

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void NotificationStreamDisposeShouldInvokeStreamDisposed(bool synchronous, string expected)
        {
            // We care about the notification stream being disposed
            // We don't care about the stream passed to the notification stream
            using (var notificationStream = new ODataNotificationStream(
                this.stream,
                this.streamListener,
                synchronous))
            {
            }

            var result = ReadStreamContents();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void NotificationStreamDisposeShouldBeIdempotent(bool synchronous, string expected)
        {
            var notificationStream = new ODataNotificationStream(
                this.stream,
                this.streamListener,
                synchronous);

            // 1st call to Dispose
            notificationStream.Dispose();
            // 2nd call to Dispose
            notificationStream.Dispose();

            var result = ReadStreamContents();

            // StreamDisposed/StreamDisposeAsync was written only once
            Assert.Equal(expected, result);
        }

#if NETCOREAPP3_1
        [Fact]
        public async Task NotificationStreamDisposeShouldInvokeStreamDisposedAsync()
        {
            await using (var notificationStream = new ODataNotificationStream(
                this.stream,
                this.streamListener)) // `synchronous` argument becomes irrelevant
            {
            }

            var result = await this.ReadStreamContentsAsync();

            Assert.Equal("StreamDisposedAsync", result);
        }

        [Fact]
        public async Task NotificationStreamDisposeAsyncShouldBeIdempotent()
        {
            var notificationStream = new ODataNotificationStream(
                this.stream,
                this.streamListener);

            // 1st call to DisposeAsync
            await notificationStream.DisposeAsync();
            // 2nd call to DisposeAsync
            await notificationStream.DisposeAsync();

            var result = await this.ReadStreamContentsAsync();

            // StreamDisposeAsync was written only once
            Assert.Equal("StreamDisposedAsync", result);
        }

#else
        [Fact]
        public async Task NotificationStreamDisposeShouldInvokeStreamDisposedAsync()
        {
            using (var notificationStream = new ODataNotificationStream(
                this.stream,
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