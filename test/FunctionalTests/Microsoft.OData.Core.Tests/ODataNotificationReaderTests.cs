//---------------------------------------------------------------------
// <copyright file="ODataNotificationReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNotificationReaderTests
    {
        private MemoryStream stream;
        private TextReader reader;
        private IODataStreamListener streamListener;

        public ODataNotificationReaderTests()
        {
            this.stream = new MemoryStream();
            this.reader = new StreamReader(this.stream);
            this.streamListener = new MockODataStreamListener(new StreamWriter(this.stream));
        }

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void NotificationReaderDisposeShouldInvokeStreamDisposed(bool synchronous, string expected)
        {
            // We care about the notification reader being disposed
            // We don't care about the reader passed to the notification reader
            using (var notificationReader = new ODataNotificationReader(
                this.reader,
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
        public void NotificationReaderDisposeShouldBeIdempotent(bool synchronous, string expected)
        {
            var notificationReader = new ODataNotificationReader(
                this.reader,
                this.streamListener,
                synchronous);

            // 1st call to Dispose
            notificationReader.Dispose();
            // 2nd call to Dispose
            notificationReader.Dispose();

            var result = ReadStreamContents();

            // StreamDisposed/StreamDisposeAsync was written only once
            Assert.Equal(expected, result);
        }

#if NETCOREAPP3_1
        [Fact]
        public async Task NotificationReaderDisposeShouldInvokeStreamDisposedAsync()
        {
            await using (var notificationReader = new ODataNotificationReader(
                this.reader,
                this.streamListener)) // `synchronous` argument becomes irrelevant since we'll directly call DisposeAsync
            {
            }

            var result = await this.ReadStreamContentsAsync();

            Assert.Equal("StreamDisposedAsync", result);
        }

        [Fact]
        public async Task NotificationReaderDisposeAsyncShouldBeIdempotent()
        {
            var notificationReader = new ODataNotificationReader(
                this.reader,
                this.streamListener);

            // 1st call to DisposeAsync
            await notificationReader.DisposeAsync();
            // 2nd call to DisposeAsync
            await notificationReader.DisposeAsync();

            var result = await this.ReadStreamContentsAsync();

            // StreamDisposeAsync was written only once
            Assert.Equal("StreamDisposedAsync", result);
        }

#else
        [Fact]
        public async Task NotificationReaderDisposeShouldInvokeStreamDisposedAsync()
        {
            using (var notificationReader = new ODataNotificationReader(
                this.reader,
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
