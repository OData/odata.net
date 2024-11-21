//---------------------------------------------------------------------
// <copyright file="ODataNotificationReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNotificationReaderTests : IDisposable
    {
        private MemoryStream stream;
        private TextReader reader;
        private TextWriter streamListenerWriter;
        private IODataStreamListener streamListener;

        public ODataNotificationReaderTests()
        {
            this.stream = new MemoryStream();
            this.reader = new StreamReader(this.stream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            this.streamListenerWriter = new StreamWriter(this.stream, encoding: Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
            this.streamListener = new MockODataStreamListener(this.streamListenerWriter);
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

        [Fact]
        public async Task NotificationReaderDisposeAsyncShouldInvokeStreamDisposedAsync()
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
        public async Task NotificationReaderDisposeAsyncShouldBeIdempotentAsync()
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

        public void Dispose() // Fired after every test is ran
        {
            this.streamListenerWriter.Dispose();
            this.reader.Dispose();
            this.stream.Dispose();
        }

        private string ReadStreamContents()
        {
            string streamContents;

            using (var reader = new StreamReader(
                this.stream,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true))
            {

                this.stream.Position = 0;
                streamContents = reader.ReadToEnd();
            }

            return streamContents;
        }

        private async Task<string> ReadStreamContentsAsync()
        {
            string  streamContents;

            using (var reader = new StreamReader(
                this.stream,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true))
            {

                this.stream.Position = 0;
                streamContents = await reader.ReadToEndAsync();
            }

            return streamContents;
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
