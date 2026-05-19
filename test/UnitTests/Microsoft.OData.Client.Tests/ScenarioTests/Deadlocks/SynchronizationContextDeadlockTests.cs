//---------------------------------------------------------------------
// <copyright file="SynchronizationContextDeadlockTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Tests.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests.ScenarioTests.Deadlocks
{
    // Reproduces the Blazor Server deadlock reported in https://github.com/OData/odata.net/issues/3521.
    // The bug was introduced by PR #3409 ("Replace TaskUtils with idiomatic async/await"): several await
    // call-sites lack ConfigureAwait(false) and capture the caller's SynchronizationContext. On a
    // single-threaded scheduler (Blazor renderer / WPF Dispatcher) the calling thread blocks waiting
    // for a continuation that has been queued onto the same single thread.
    //
    // Each test races the call against a 5 s timeout so a regression fails fast instead of hanging
    // the test runner.
    public class SynchronizationContextDeadlockTests
    {
        private const string ServiceRoot = "http://localhost:8007";
        private static readonly TimeSpan DeadlockTimeout = TimeSpan.FromSeconds(5);

        #region Test Edmx

        private const string Edmx = @"<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
    <edmx:DataServices>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Sample.API.Models"">
            <EntityType Name=""Organization"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            </EntityType>
        </Schema>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Default"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""Organizations"" EntityType=""Sample.API.Models.Organization"" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>";

        private const string OrganizationsResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Organizations"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" },
        { ""Id"": 2, ""Name"": ""Globex"" }
    ]
}";

        private const string OrganizationsFilteredResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Organizations"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" }
    ]
}";

        private const string CreatedOrganizationResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Organizations/$entity"",
    ""Id"": 99,
    ""Name"": ""NewCo""
}";

        private const string OrganizationLoadedNameResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Organizations(1)/Name"",
    ""value"": ""Acme""
}";

        // Multipart/mixed batch response: one wrapped 200 OK GET /Organizations.
        // Boundary string must match the one set on the HttpResponseMessage Content-Type below.
        private const string BatchResponseBody =
            "--batchresponse_test\r\n" +
            "Content-Type: application/http\r\n" +
            "Content-Transfer-Encoding: binary\r\n" +
            "\r\n" +
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: application/json;charset=utf-8\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#Organizations\",\"value\":[{\"Id\":1,\"Name\":\"Acme\"},{\"Id\":2,\"Name\":\"Globex\"}]}\r\n" +
            "--batchresponse_test--\r\n";

        // Save-path batch response: outer batch boundary wraps a changeset boundary, which wraps the
        // single 201 Created for the AddObject. Required because BatchSaveResult expects a changeset
        // when this.Queries is null (save path) and would NRE on a flat query-style batch response.
        private const string BatchSaveResponseBody =
            "--batchresponse_test\r\n" +
            "Content-Type: multipart/mixed; boundary=changesetresponse_1\r\n" +
            "\r\n" +
            "--changesetresponse_1\r\n" +
            "Content-Type: application/http\r\n" +
            "Content-Transfer-Encoding: binary\r\n" +
            "Content-ID: 1\r\n" +
            "\r\n" +
            "HTTP/1.1 201 Created\r\n" +
            "Content-Type: application/json;charset=utf-8\r\n" +
            "Location: http://localhost:8007/Organizations(99)\r\n" +
            "OData-EntityId: http://localhost:8007/Organizations(99)\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#Organizations/$entity\",\"Id\":99,\"Name\":\"NewCo\"}\r\n" +
            "--changesetresponse_1--\r\n" +
            "--batchresponse_test--\r\n";

        #endregion

        // ----------- Async path: ExecuteAsync -----------

        [Fact]
        public async Task ExecuteAsync_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run<List<Organization>>(async () =>
            {
                var ctx = CreateContext(handler);
                IEnumerable<Organization> orgs = await ctx.Organizations.ExecuteAsync();
                return orgs.ToList();
            });

            var orgs = await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Equal(2, orgs.Count);
            Assert.Contains(orgs, o => o.Name == "Acme");
        }

        // ----------- Sync path: SingleOrDefault -----------

        [Fact]
        public async Task SingleOrDefault_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler(filtered: true);
            var run = SingleThreadSynchronizationContext.Run<Organization>(() =>
            {
                var ctx = CreateContext(handler);
                Organization org = ctx.Organizations.Where(o => o.Name == "Acme").SingleOrDefault();
                return Task.FromResult(org);
            });

            var org = await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.NotNull(org);
            Assert.Equal("Acme", org.Name);
        }

        // ----------- Sync path: ToList -----------

        [Fact]
        public async Task ToList_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run<List<Organization>>(() =>
            {
                var ctx = CreateContext(handler);
                List<Organization> orgs = ctx.Organizations.ToList();
                return Task.FromResult(orgs);
            });

            var orgs = await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Equal(2, orgs.Count);
        }

        // ----------- Async write path: SaveChangesAsync -----------

        [Fact]
        public async Task SaveChangesAsync_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("Organizations", new Organization { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync();
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/Organizations"));
        }

        // ----------- Async batch path: ExecuteBatchAsync -----------

        // Async batch path: ODataBatchReader/Writer were heavily rewritten in PR #3409.
        [Fact]
        public async Task ExecuteBatchAsync_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run<DataServiceResponse>(async () =>
            {
                var ctx = CreateContext(handler);
                return await ctx.ExecuteBatchAsync(ctx.Organizations);
            });

            var response = await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.NotNull(response);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/$batch"));
        }

        // ----------- Lazy-loaded property path: LoadPropertyAsync -----------

        // Lazy-loaded property path: exercises another reader entry point.
        [Fact]
        public async Task LoadPropertyAsync_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run<QueryOperationResponse>(async () =>
            {
                var ctx = CreateContext(handler);
                // Materialise an entity first so we have something to load a property on.
                var orgs = (await ctx.Organizations.ExecuteAsync()).ToList();
                var first = orgs.First();
                return await ctx.LoadPropertyAsync(first, nameof(Organization.Name));
            });

            var response = await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.NotNull(response);
            Assert.Contains(handler.Requests, r => r.Contains("/Organizations(1)/Name"));
        }

        // ----------- Batched-write save path: SaveChangesAsync with BatchWithSingleChangeset -----------

        // Batched-write save path: SaveChanges with batching produces a POST /$batch.
        [Fact]
        public async Task SaveChangesAsyncBatched_DoesNotDeadlock_UnderSingleThreadedSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler(useBatchSaveResponse: true);
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("Organizations", new Organization { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/$batch"));
        }

        // ----------- Sanity: no captured context -----------

        [Fact]
        public async Task Calls_StillSucceed_UnderDefaultSynchronizationContext()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var ctx = CreateContext(handler);

            // Async read
            IEnumerable<Organization> asyncRead = await ctx.Organizations.ExecuteAsync();
            Assert.Equal(2, asyncRead.Count());

            // Sync read (LINQ ToList)
            using var handler2 = new RoutingMockHttpClientHandler();
            var ctx2 = CreateContext(handler2);
            var sync = ctx2.Organizations.ToList();
            Assert.Equal(2, sync.Count);

            // Async write
            using var handler3 = new RoutingMockHttpClientHandler();
            var ctx3 = CreateContext(handler3);
            ctx3.AddObject("Organizations", new Organization { Id = 99, Name = "NewCo" });
            await ctx3.SaveChangesAsync();
            Assert.Contains(handler3.Requests, r => r.StartsWith("POST "));
        }

        // ----------- helpers -----------

        private static OrgContainer CreateContext(RoutingMockHttpClientHandler handler)
        {
            var factory = new MockHttpClientFactory(handler);
            return new OrgContainer(new Uri(ServiceRoot), factory);
        }

        // Races the call against a 5 s timeout. On timeout we dispose the pump's shutdown handle
        // (best-effort: signals CompleteAdding so no more work is queued; the worker may still be
        // wedged on the deadlocked callback but exits with the process as a background thread)
        // and fail with a clear regression message. Surfaces any inner exception via the awaited task.
        private static async Task AwaitWithTimeout(Task call, IDisposable shutdown)
        {
            var completed = await Task.WhenAny(call, Task.Delay(DeadlockTimeout));
            if (completed != call)
            {
                shutdown.Dispose();
                Assert.Fail($"Deadlock detected (timed out after {DeadlockTimeout.TotalSeconds}s) — regression of issue #3521 (SynchronizationContext capture).");
            }
            await call;
        }

        private static async Task<T> AwaitWithTimeout<T>(Task<T> call, IDisposable shutdown)
        {
            var completed = await Task.WhenAny(call, Task.Delay(DeadlockTimeout));
            if (completed != call)
            {
                shutdown.Dispose();
                Assert.Fail($"Deadlock detected (timed out after {DeadlockTimeout.TotalSeconds}s) — regression of issue #3521 (SynchronizationContext capture).");
            }
            return await call;
        }

        // Routes by URL: $metadata → EDMX; entity-set GET → JSON; POST → 201 with location.
        // Any unmatched URL produces a clear error rather than a silent hang.
        //
        // Critical: SendAsync must return a Task that completes ASYNCHRONOUSLY (not Task.FromResult).
        // Awaiting an already-completed Task runs the continuation synchronously and never marshals
        // through the captured SynchronizationContext — which would mask the very bug under test.
        private sealed class RoutingMockHttpClientHandler : MockHttpClientHandler
        {
            public RoutingMockHttpClientHandler(bool filtered = false, bool useBatchSaveResponse = false)
                : base(req => Respond(req, filtered, useBatchSaveResponse))
            {
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // Force asynchronous completion by hopping to the ThreadPool. The returned task is not
                // yet complete when the caller awaits it, so the await's continuation is posted via
                // SynchronizationContext.Current — exactly the path that deadlocks under issue #3521.
                return Task.Run(() => Send(request, cancellationToken));
            }

            private static HttpResponseMessage Respond(HttpRequestMessage req, bool filtered, bool useBatchSaveResponse)
            {
                string url = req.RequestUri.AbsoluteUri;

                if (url.EndsWith("/$metadata", StringComparison.Ordinal))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(Edmx)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                    return resp;
                }

                if (req.Method == HttpMethod.Post && url.EndsWith("/$batch", StringComparison.Ordinal))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(useBatchSaveResponse ? BatchSaveResponseBody : BatchResponseBody)
                    };
                    resp.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse(
                            "multipart/mixed; boundary=batchresponse_test");
                    return resp;
                }

                if (req.Method == HttpMethod.Get && url.Contains("/Organizations(1)/Name"))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(OrganizationLoadedNameResponse)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    return resp;
                }

                if (req.Method == HttpMethod.Get && url.Contains("/Organizations"))
                {
                    string body = filtered ? OrganizationsFilteredResponse : OrganizationsResponse;
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(body)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    return resp;
                }

                if (req.Method == HttpMethod.Post && url.EndsWith("/Organizations", StringComparison.Ordinal))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(CreatedOrganizationResponse)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    resp.Headers.Location = new Uri(ServiceRoot + "/Organizations(99)");
                    return resp;
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"Unmocked route: {req.Method} {url}")
                };
            }
        }
    }

    // The inner Container deliberately does NOT set LoadServiceModel — we want $metadata to be
    // fetched over HTTP so the metadata-parsing async path described in the bug is exercised.
    internal sealed class OrgContainer : DataServiceContext
    {
        public OrgContainer(Uri serviceRoot, IHttpClientFactory httpClientFactory)
            : base(serviceRoot, ODataProtocolVersion.V4)
        {
            HttpClientFactory = httpClientFactory;
            Format.UseJson();
            Organizations = base.CreateQuery<Organization>("Organizations");
            ResolveName = type => type == typeof(Organization) ? "Sample.API.Models.Organization" : null;
            ResolveType = name => name == "Sample.API.Models.Organization" ? typeof(Organization) : null;
        }

        public DataServiceQuery<Organization> Organizations { get; }
    }

    [global::Microsoft.OData.Client.Key("Id")]
    public class Organization : BaseEntityType, System.ComponentModel.INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
