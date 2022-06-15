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
                    var stream = response.GetStream();
                    string contents = "";
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

                var request = new HttpClientRequestMessage(args);

                // Act
                IODataResponseMessage response = request.GetResponse();
                var stream = response.GetStream();
                stream.Dispose();
                request.Dispose();

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
