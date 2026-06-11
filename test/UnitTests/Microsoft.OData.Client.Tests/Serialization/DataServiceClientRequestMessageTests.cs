//---------------------------------------------------------------------
// <copyright file="DataServiceClientRequestMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OData;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    public class DataServiceClientRequestMessageTests
    {
        /// <summary>
        /// Minimal concrete subclass of <see cref="DataServiceClientRequestMessage"/> for testing
        /// the default <see cref="DataServiceClientRequestMessage.GetResponseAsync"/> implementation.
        /// </summary>
        private class TestRequestMessage : DataServiceClientRequestMessage
        {
            private readonly Func<IODataResponseMessage> _getResponse;
            public int GetResponseCallCount { get; private set; }

            public TestRequestMessage(Func<IODataResponseMessage> getResponse)
                : base("GET")
            {
                _getResponse = getResponse;
            }

            public override IEnumerable<KeyValuePair<string, string>> Headers => Array.Empty<KeyValuePair<string, string>>();
            public override Uri Url { get; set; } = new Uri("http://localhost");
            public override string Method { get; set; } = "GET";
            public override int Timeout { get; set; }
            public override bool SendChunked { get; set; }
            public override string GetHeader(string headerName) => null;
            public override void SetHeader(string headerName, string headerValue) { }
            public override Stream GetStream() => Stream.Null;
            public override void Abort() { }
            public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state) => throw new NotImplementedException();
            public override Stream EndGetRequestStream(IAsyncResult asyncResult) => throw new NotImplementedException();
            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state) => throw new NotImplementedException();
            public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult) => throw new NotImplementedException();

            public override IODataResponseMessage GetResponse()
            {
                GetResponseCallCount++;
                return _getResponse();
            }
        }

        [Fact]
        public async Task GetResponseAsync_WithPreCancelledToken_ReturnsCancelledTaskWithoutCallingGetResponse()
        {
            // Arrange
            var message = new TestRequestMessage(() => throw new Exception("GetResponse should not be called"));
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            Task<IODataResponseMessage> task = message.GetResponseAsync(cts.Token);

            // Assert
            Assert.True(task.IsCanceled);
            Assert.Equal(0, message.GetResponseCallCount);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        }

        [Fact]
        public async Task GetResponseAsync_WithNonCancelledToken_CallsGetResponseAndReturnsResult()
        {
            // Arrange
            var expectedResponse = new TestODataResponseMessage();
            var message = new TestRequestMessage(() => expectedResponse);

            // Act
            IODataResponseMessage result = await message.GetResponseAsync(CancellationToken.None);

            // Assert
            Assert.Same(expectedResponse, result);
            Assert.Equal(1, message.GetResponseCallCount);
        }

        [Fact]
        public async Task GetResponseAsync_WhenGetResponseThrows_ReturnsFailedTask()
        {
            // Arrange
            var expectedException = new InvalidOperationException("transport error");
            var message = new TestRequestMessage(() => throw expectedException);

            // Act
            Task<IODataResponseMessage> task = message.GetResponseAsync(CancellationToken.None);

            // Assert
            Assert.True(task.IsFaulted);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => task);
            Assert.Same(expectedException, ex);
        }

        private class TestODataResponseMessage : IODataResponseMessage
        {
            public IEnumerable<KeyValuePair<string, string>> Headers => Array.Empty<KeyValuePair<string, string>>();
            public int StatusCode { get; set; } = 200;
            public string GetHeader(string headerName) => null;
            public void SetHeader(string headerName, string headerValue) { }
            public Stream GetStream() => Stream.Null;
        }
    }
}
