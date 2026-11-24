//---------------------------------------------------------------------
// <copyright file="HttpClientRequestMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    public class HttpClientRequestMessageTests
    {
        [Fact]
        public async Task GetResponseAsync_ReturnsContentAndHeaders()
        {
            using (var handler = new MockHttpClientHandler("native async"))
            using (var request = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>(), new MockHttpClientHandlerProvider(handler))))
            {
                IODataResponseMessage response = await request.GetResponseAsync(CancellationToken.None);
                using (var reader = new StreamReader(response.GetStream()))
                {
                    Assert.Equal("native async", await reader.ReadToEndAsync());
                }

                Assert.Equal(200, response.StatusCode);
                Assert.Contains("text/plain", response.GetHeader("Content-Type"));
                Assert.Single(handler.Requests);
            }
        }

        [Fact]
        public async Task GetResponseAsync_PreCanceledTokenDoesNotSend()
        {
            using (var handler = new MockHttpClientHandler("unused"))
            using (var request = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>(), new MockHttpClientHandlerProvider(handler))))
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request.GetResponseAsync(cancellation.Token));
                Assert.Empty(handler.Requests);
            }
        }

        [Fact]
        public async Task GetResponseAsync_CancelsInFlightRequest()
        {
            using (var handler = new MockUnresponsiveHttpClientHandler())
            using (var request = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>(), new MockHttpClientHandlerProvider(handler))))
            using (var cancellation = new CancellationTokenSource())
            {
                handler.OnRequestStarted = cancellation.Cancel;
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request.GetResponseAsync(cancellation.Token));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetResponseAsync_PreservesAbortAndTimeout(bool abort)
        {
            using (var handler = new MockUnresponsiveHttpClientHandler())
            using (var request = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>(), new MockHttpClientHandlerProvider(handler))))
            {
                request.Timeout = 1;
                if (abort)
                {
                    handler.OnRequestStarted = request.Abort;
                }

                var error = await Assert.ThrowsAsync<DataServiceTransportException>(() => request.GetResponseAsync(CancellationToken.None));
                Assert.IsAssignableFrom<OperationCanceledException>(error.InnerException);
            }
        }

        [Fact]
        public async Task GetResponseAsync_CustomTransportKeepsApmFallback()
        {
            var args = new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>());
            var request = new TestHttpWebRequestMessage(args, new Dictionary<string, string>(), () => new MemoryStream(new byte[] { 1, 2, 3 }));
            IODataResponseMessage response = await request.GetResponseAsync(CancellationToken.None);
            using (Stream stream = response.GetStream())
            {
                Assert.Equal(3, stream.Length);
            }
        }

        [Fact]
        public async Task GetResponseAsync_DerivedHttpClientTransportKeepsApmFallback()
        {
            var args = new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>());
            using (var request = new Tracking.CustomizedRequestMessage(args, "legacy response", new Dictionary<string, string> { { "X-Transport", "custom" } }))
            {
                IODataResponseMessage response = await request.GetResponseAsync(CancellationToken.None);
                Assert.Equal("custom", response.GetHeader("X-Transport"));
                using (var reader = new StreamReader(response.GetStream()))
                {
                    Assert.Equal("legacy response", await reader.ReadToEndAsync());
                }
            }
        }

        [Fact]
        public void GetHttpWebResponse_PreservesNativeTransportErrorAndResponse()
        {
            var innerException = new HttpRequestException("native transport failure");
            var expectedResponse = new HttpWebResponseMessage(new Dictionary<string, string>(), 503, () => Stream.Null);
            var exception = new DataServiceTransportException(expectedResponse, innerException);
            IODataResponseMessage response = null;

            Assert.Same(exception, WebUtil.GetHttpWebResponse(exception, ref response));
            Assert.Same(expectedResponse, response);
            Assert.Same(innerException, exception.InnerException);
        }

        [Fact]
        public void UnwrapAggregateException()
        {
            var msg = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>()));
            var task = new Task<HttpResponseMessage>(() => throw new Exception("single exception"));
            task.RunSynchronously();

            var exception = Assert.Throws<DataServiceTransportException>(() => msg.EndGetResponse(task));
            Assert.StartsWith("System.Exception: single exception\r\n", exception.Message);
        }

        [Fact]
        public void WhenHttpClientHandlerProviderIsSet_UsesReturnedHttpClientHandler_ToMakeRequest()
        {
            // Arrange
            string expectedResponse = "Foo";
            using (var handler = new MockHttpClientHandler(expectedResponse))
            {

                var httpClientHandlerProvider = new MockHttpClientHandlerProvider(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    useDefaultCredentials: false,
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    httpClientHandlerProvider);

                using (var request = new HttpClientRequestMessage(args))
                {
                    // Act
                    IODataResponseMessage response = request.GetResponse();
                    string contents = "";
                    using (var stream = response.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        contents = reader.ReadToEnd();
                    }

                    // Assert
                    Assert.Equal(expectedResponse, contents);
                    Assert.Equal(1, handler.Requests.Count);
                    Assert.Equal("GET http://localhost/", handler.Requests[0]);
                    Assert.Equal(1, httpClientHandlerProvider.NumCalls);
                }
            }
        }

        [Fact]
        public async void WhenHttpClientHandlerIsProvided_DoesNotDisposeHandler()
        {
            // Arrange
            string expectedResponse = "Foo";
            using (var handler = new MockHttpClientHandler(expectedResponse))
            {

                var httpClientProvider = new MockHttpClientHandlerProvider(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    useDefaultCredentials: false,
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    httpClientProvider);

                //var request = new HttpClientRequestMessage(args);

                // Act
                using (var request = new HttpClientRequestMessage(args))
                {
                    IODataResponseMessage response = request.GetResponse();
                    using (var stream = response.GetStream())
                    {
                        request.Dispose();
                        stream.Dispose();
                    }
                }

                // Re-use handler with a different HttpClient after initial request is disposed
                var client = new HttpClient(handler, disposeHandler: false);
                var newResponse = await client.GetAsync("http://foo");

                // Assert
                // We can still make request from the client after HttpClientRequestMessage is disposed
                var contents = await newResponse.Content.ReadAsStringAsync();
                Assert.Equal(expectedResponse, contents);
                Assert.Equal(2, handler.Requests.Count);
                Assert.Equal("GET http://localhost/", handler.Requests[0]);
                Assert.Equal("GET http://foo/", handler.Requests[1]);
                Assert.False(handler.Disposed);
            }
        }

        [Fact]
        public async void Abort_CancelsPendingRequest()
        {
            // Arrange
            using (var handler = new MockUnresponsiveHttpClientHandler())
            {

                var httpClientProvider = new MockHttpClientHandlerProvider(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    useDefaultCredentials: false,
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    httpClientProvider);

                using (var request = new HttpClientRequestMessage(args))
                {

                    // Act
                    // Call request.Abort() only after request has started
                    handler.OnRequestStarted = () => request.Abort();
                    Task<IODataResponseMessage> getResponseTask =
                        Task.Run(() => Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null));

                    // Assert
                    await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    {
                        await getResponseTask;
                    });
                }
            }
        }

        [Fact]
        public async void Timeout_CancelsPendingRequestAfterTimeout()
        {
            // Arrange
            using (var handler = new MockUnresponsiveHttpClientHandler())
            {

                var httpClientProvider = new MockHttpClientHandlerProvider(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    useDefaultCredentials: false,
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    httpClientProvider);

                using (var request = new HttpClientRequestMessage(args))
                {
                    request.Timeout = 1;

                    // Act
                    Task<IODataResponseMessage> getResponseTask =
                        Task.Run(() => Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null));

                    // Assert
                    await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    {
                        await getResponseTask;
                    });
                }
            }
        }

        [Fact]
        public void WhenClientHandlerIsProvided_CredentialsProperty_AccessHandlerCredentials()
        {
            // Arrange
            using (var handler = new MockHttpClientHandler("Foo"))
            {

                var httpClientProvider = new MockHttpClientHandlerProvider(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    useDefaultCredentials: false,
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    httpClientProvider);

                using (var request = new HttpClientRequestMessage(args))
                {

                    // Act & Assert
#pragma warning disable CS0618 // Type or member is obsolete
                    request.Credentials = new NetworkCredential();
                    Assert.Same(request.Credentials, handler.Credentials);

                    handler.Credentials = new NetworkCredential();
                    Assert.Same(handler.Credentials, request.Credentials);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }
        }
    }
}
