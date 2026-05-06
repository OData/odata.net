//---------------------------------------------------------------------
// <copyright file="SynchronizationContextDeadlockTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Client.Tests.Serialization;
using Microsoft.OData.Edm.Csdl;
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

            var orgs = await AwaitWithTimeout(run);
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

            var org = await AwaitWithTimeout(run);
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

            var orgs = await AwaitWithTimeout(run);
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

            await AwaitWithTimeout(run);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/Organizations"));
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

        // Races the call against a 5 s timeout. On timeout we fail with a clear regression message.
        // Surfaces any inner exception via the awaited task.
        private static async Task AwaitWithTimeout(Task call)
        {
            var completed = await Task.WhenAny(call, Task.Delay(DeadlockTimeout));
            if (completed != call)
            {
                Assert.Fail($"Deadlock detected (timed out after {DeadlockTimeout.TotalSeconds}s) — regression of issue #3521 (SynchronizationContext capture).");
            }
            await call;
        }

        private static async Task<T> AwaitWithTimeout<T>(Task<T> call)
        {
            var completed = await Task.WhenAny(call, Task.Delay(DeadlockTimeout));
            if (completed != call)
            {
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
            public RoutingMockHttpClientHandler(bool filtered = false)
                : base(req => Respond(req, filtered))
            {
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // Force asynchronous completion by hopping to the ThreadPool. The returned task is not
                // yet complete when the caller awaits it, so the await's continuation is posted via
                // SynchronizationContext.Current — exactly the path that deadlocks under issue #3521.
                return Task.Run(() => Send(request, cancellationToken));
            }

            private static HttpResponseMessage Respond(HttpRequestMessage req, bool filtered)
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
