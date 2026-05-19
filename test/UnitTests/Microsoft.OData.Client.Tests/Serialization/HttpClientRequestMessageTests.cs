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
            var msg = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, new Dictionary<string, string>()));
            var task = new Task<HttpResponseMessage>(() => throw new Exception("single exception"));
            task.RunSynchronously();

            var exception = Assert.Throws<DataServiceTransportException>(() => msg.EndGetResponse(task));
            Assert.StartsWith("System.Exception: single exception\r\n", exception.Message);
        }

        [Fact]
        public void WhenHttpClientFactoryIsSet_UsesReturnedHttpClientHandler_ToMakeRequest()
        {
            // Arrange
            string expectedResponse = "Foo";
            using (var handler = new MockHttpClientHandler(expectedResponse))
            {
                var httpClientFactory = new MockHttpClientFactory(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

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
                    Assert.Equal(1, httpClientFactory.NumCalls);
                }
            }
        }

        [Fact]
        public async Task WhenHttpClientHandlerIsProvided_DoesNotDisposeHandler()
        {
            // Arrange
            string expectedResponse = "Foo";
            using (var handler = new MockHttpClientHandler(expectedResponse))
            {

                var httpClientFactory = new MockHttpClientFactory(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

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
        public async Task Abort_CancelsTheSpecifiedRequest()
        {
            // Arrange
            using (var handler = new MockUnresponsiveHttpClientHandler())
            {

                var httpClientFactory = new MockHttpClientFactory(handler);

                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                using (var request = new HttpClientRequestMessage(args))
                {

                    // Act
                    // Call request.Abort() only after request has started
                    handler.OnRequestStarted = () => request.Abort();
                    Task<IODataResponseMessage> getResponseTask =
                        Task.Run(() => Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null));

                    // Assert
                    await Assert.ThrowsAsync<DataServiceTransportException>(async () =>
                    {
                        await getResponseTask;
                    });
                }
            }
        }

        [Fact]
        public async Task Abort_DoesNotCancelOtherRequestsFromTheSameClient()
        {
            // Arrange
            using (var handler = new MockDelayedHttpClientHandler("Success", delayMilliseconds: 2000))
            {

                var httpClientFactory = new MockHttpClientFactory(handler);

                var args1 = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost/request1"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                var args2 = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost/request2"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                using (var request1 = new HttpClientRequestMessage(args1))
                using (var request2 = new HttpClientRequestMessage(args2))
                {

                    // Call Abort() on request1 after it has started
                    handler.OnRequestStarted = (httpRequest) =>
                    {
                        if (httpRequest.RequestUri.AbsolutePath.EndsWith("request1"))
                        {
                            request1.Abort();
                        }
                    };

                    Task<IODataResponseMessage> getResponse1Task =
                            Task.Run(() => Task.Factory.FromAsync(request1.BeginGetResponse, request1.EndGetResponse, null));

                    Task<IODataResponseMessage> getResponse2Task =
                            Task.Run(() => Task.Factory.FromAsync(request2.BeginGetResponse, request2.EndGetResponse, null));

                    // Assert
                    // Request 1 should fail
                    await Assert.ThrowsAsync<DataServiceTransportException>(async () =>
                    {
                        await getResponse1Task;
                    });

                    // Request 2 should succeed
                    var response2 = await getResponse2Task;
                    var stream = response2.GetStream();
                    var reader = new StreamReader(stream);
                    var data = reader.ReadToEnd();
                    Assert.Equal("Success", data);
                }
            }
        }

        [Fact]
        public async Task Timeout_CancelsPendingRequestAfterTimeout()
        {
            // Arrange
            using (var handler = new MockUnresponsiveHttpClientHandler())
            {
                var httpClientFactory = new MockHttpClientFactory(handler);
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                using (var request = new HttpClientRequestMessage(args))
                {
                    request.Timeout = 1;

                    // Act
                    Task<IODataResponseMessage> getResponseTask =
                        Task.Run(() => Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null));

                    // Assert
                    await Assert.ThrowsAsync<DataServiceTransportException>(async () =>
                    {
                        await getResponseTask;
                    });
                }
            }
        }

        [Fact]
        public async Task Timeout_DoesNotCancelOtherRequestsFromTheSameClient()
        {
            // Arrange
            using (var handler = new MockDelayedHttpClientHandler("Success", delayMilliseconds: 5000))
            {
                var httpClientFactory = new MockHttpClientFactory(handler);
                var args1 = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost/request1"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                var args2 = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost/request2"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                using (var request1 = new HttpClientRequestMessage(args1))
                using (var request2 = new HttpClientRequestMessage(args2))
                {
                    request1.Timeout = 1;

                    // Act
                    Task<IODataResponseMessage> getResponse1Task =
                            Task.Run(() => Task.Factory.FromAsync(request1.BeginGetResponse, request1.EndGetResponse, null));

                    Task<IODataResponseMessage> getResponse2Task =
                            Task.Run(() => Task.Factory.FromAsync(request2.BeginGetResponse, request2.EndGetResponse, null));

                    // Assert
                    // Request 1 should timeout
                    await Assert.ThrowsAsync<DataServiceTransportException>(async () =>
                    {
                        await getResponse1Task;
                    });

                    // Request 2 should succeed;
                    var response2 = await getResponse2Task;
                    var stream = response2.GetStream();
                    var reader = new StreamReader(stream);
                    var data = reader.ReadToEnd();
                    Assert.Equal("Success", data);
                }
            }
        }

        [Fact]
        public async Task WhenTimeoutNotSet_HttpClientTimeoutStillApplies()
        {
            // Arrange
            using (var handler = new MockDelayedHttpClientHandler("success", delayMilliseconds: 5000))
            {
                var httpClientFactory = new MockHttpClientFactory(handler, new MockHttpClientFactoryOptions
                {
                    Timeout = 1
                });
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://localhost"),
                    usePostTunneling: false,
                    headers: new Dictionary<string, string>(),
                    httpClientFactory: httpClientFactory);

                using (var request = new HttpClientRequestMessage(args))
                {
                    // Act
                    Task<IODataResponseMessage> getResponseTask =
                        Task.Run(() => Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null));

                    // Assert
                    await Assert.ThrowsAsync<DataServiceTransportException>(async () =>
                    {
                        await getResponseTask;
                    });
                }
            }
        }

        // Focused regression tests for issue #3521. SendInternalAsync awaits HttpClient.SendAsync;
        // without ConfigureAwait(false) the continuation is posted to the captured single-threaded
        // SynchronizationContext, deadlocking GetResponse()'s send.Wait() / send.Result and the
        // APM EndGetResponse's Task.Result. The handler's small Task.Delay forces asynchronous
        // completion — without that, the await sees an already-completed task and runs the
        // continuation synchronously, masking the bug.
        [Fact]
        public async Task GetResponse_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new MockDelayedHttpClientHandler("Hello", delayMilliseconds: 1);
            var args = BuildArgs(handler);

            var run = SingleThreadSynchronizationContext.Run<string>(() =>
            {
                using var request = new HttpClientRequestMessage(args);
                IODataResponseMessage response = request.GetResponse();
                using var stream = response.GetStream();
                using var reader = new StreamReader(stream);
                return Task.FromResult(reader.ReadToEnd());
            });

            string body = await AwaitOrFailDeadlock(run.Task, run.Shutdown);
            Assert.Equal("Hello", body);
        }

        [Fact]
        public async Task BeginEndGetResponse_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new MockDelayedHttpClientHandler("Hello", delayMilliseconds: 1);
            var args = BuildArgs(handler);

            var run = SingleThreadSynchronizationContext.Run<string>(async () =>
            {
                using var request = new HttpClientRequestMessage(args);
                IODataResponseMessage response = await Task.Factory.FromAsync(
                    request.BeginGetResponse, request.EndGetResponse, null);
                using var stream = response.GetStream();
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            });

            string body = await AwaitOrFailDeadlock(run.Task, run.Shutdown);
            Assert.Equal("Hello", body);
        }

        private static DataServiceClientRequestMessageArgs BuildArgs(HttpClientHandler handler)
        {
            return new DataServiceClientRequestMessageArgs(
                "GET",
                new Uri("http://localhost"),
                usePostTunneling: false,
                headers: new Dictionary<string, string>(),
                httpClientFactory: new MockHttpClientFactory(handler));
        }

        private static async Task<T> AwaitOrFailDeadlock<T>(Task<T> call, IDisposable shutdown)
        {
            var timeout = TimeSpan.FromSeconds(5);
            var completed = await Task.WhenAny(call, Task.Delay(timeout));
            if (completed != call)
            {
                shutdown.Dispose();
                Assert.Fail($"Deadlock detected (timed out after {timeout.TotalSeconds}s) — regression of issue #3521 (SynchronizationContext capture).");
            }
            return await call;
        }
    }
}
