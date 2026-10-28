//---------------------------------------------------------------------
// <copyright file="ServerTypeNameIssueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    // Regression tests guarding against re-introduction of the issue
    // reported in https://github.com/OData/odata.net/issues/3623.
    public class ServerTypeNameIssueTests
    {
        private const string ServiceRoot = "http://localhost:8007";
        private static readonly TimeSpan DeadlockTimeout = TimeSpan.FromSeconds(30);

        #region Test Edmx

        private const string Edmx = @"<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
    <edmx:DataServices>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Sample.API.Models"">
            <EntityType Name=""organization"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            </EntityType>
            <EntityType Name=""BlogPost"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            </EntityType>
        </Schema>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Default"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""organizations"" EntityType=""Sample.API.Models.organization"" />
                <EntitySet Name=""BlogPosts"" EntityType=""Sample.API.Models.BlogPost"" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>";

        private const string OrganizationsResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#organizations"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" },
        { ""Id"": 2, ""Name"": ""Globex"" }
    ]
}";

        private const string OrganizationsFilteredResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#organizations"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" }
    ]
}";

        private const string CreatedOrganizationResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#organizations/$entity"",
    ""Id"": 99,
    ""Name"": ""NewCo""
}";

        private const string BlogPostsResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#BlogPosts"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" },
        { ""Id"": 2, ""Name"": ""Globex"" }
    ]
}";

        private const string BlogPostsFilteredResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#BlogPosts"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Acme"" }
    ]
}";

        private const string CreatedBlogPostResponse = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#BlogPosts/$entity"",
    ""Id"": 99,
    ""Name"": ""NewCo""
}";


        // Multipart/mixed batch response: one wrapped 200 OK GET /organizations.
        // Boundary string must match the one set on the HttpResponseMessage Content-Type below.
        private const string BatchOrganizationResponseBody =
            "--batchresponse_test\r\n" +
            "Content-Type: application/http\r\n" +
            "Content-Transfer-Encoding: binary\r\n" +
            "\r\n" +
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: application/json;charset=utf-8\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#organizations\",\"value\":[{\"Id\":1,\"Name\":\"Acme\"},{\"Id\":2,\"Name\":\"Globex\"}]}\r\n" +
            "--batchresponse_test--\r\n";

        // Save-path batch response: outer batch boundary wraps a changeset boundary, which wraps the
        // single 201 Created for the AddObject. Required because BatchSaveResult expects a changeset
        // when this.Queries is null (save path) and would NRE on a flat query-style batch response.
        private const string BatchSaveOrganizationResponseBody =
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
            "Location: http://localhost:8007/organizations(99)\r\n" +
            "OData-EntityId: http://localhost:8007/organizations(99)\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#organizations/$entity\",\"Id\":99,\"Name\":\"NewCo\"}\r\n" +
            "--changesetresponse_1--\r\n" +
            "--batchresponse_test--\r\n";



        // Multipart/mixed batch response: one wrapped 200 OK GET /BlogPosts.
        // Boundary string must match the one set on the HttpResponseMessage Content-Type below.
        private const string BatchBlogPostResponseBody =
            "--batchresponse_test\r\n" +
            "Content-Type: application/http\r\n" +
            "Content-Transfer-Encoding: binary\r\n" +
            "\r\n" +
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: application/json;charset=utf-8\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#BlogPosts\",\"value\":[{\"Id\":1,\"Name\":\"Acme\"},{\"Id\":2,\"Name\":\"Globex\"}]}\r\n" +
            "--batchresponse_test--\r\n";

        // Save-path batch response: outer batch boundary wraps a changeset boundary, which wraps the
        // single 201 Created for the AddObject. Required because BatchSaveResult expects a changeset
        // when this.Queries is null (save path) and would NRE on a flat query-style batch response.
        private const string BatchSaveBlogPostResponseBody =
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
            "Location: http://localhost:8007/BlogPosts(99)\r\n" +
            "OData-EntityId: http://localhost:8007/BlogPosts(99)\r\n" +
            "\r\n" +
            "{\"@odata.context\":\"http://localhost:8007/$metadata#BlogPosts/$entity\",\"Id\":99,\"Name\":\"NewCo\"}\r\n" +
            "--changesetresponse_1--\r\n" +
            "--batchresponse_test--\r\n";

        #endregion

        // ----------- Async write path: SaveChangesAsync -----------

        [Fact]
        public async Task SaveChangesAsync_SendsProperRequest_WhenEntityNameDiffersFromTypeName()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("organizations", new Organization { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync();
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/organizations"));
            Assert.Contains(handler.RequestBodies, b => b.Contains("\"@odata.type\":\"#Sample.API.Models.organization\""));
        }

        // ----------- Batched-write save path: SaveChangesAsync with BatchWithSingleChangeset -----------

        // Batched-write save path: SaveChanges with batching produces a POST /$batch.
        [Fact]
        public async Task SaveChangesAsyncBatched_SendsProperRequest_WhenEntityNameDiffersFromTypeName()
        {
            using var handler = new RoutingMockHttpClientHandler(useBatchSaveResponse: true, isOrganizationBatch: true);
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("organizations", new Organization { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/$batch"));
            Assert.Contains(handler.RequestBodies, b => b.Contains("\"@odata.type\":\"#Sample.API.Models.organization\""));
        }

        // ----------- Async write path: SaveChangesAsync -----------

        [Fact]
        public async Task SaveChangesAsync_SendsProperRequest_WhenEntityNameMatchesTypeName()
        {
            using var handler = new RoutingMockHttpClientHandler();
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("BlogPosts", new BlogPost { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync();
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/BlogPosts"));
            Assert.Contains(handler.RequestBodies, b => b.Contains("\"@odata.type\":\"#Sample.API.Models.BlogPost\""));
        }

        // ----------- Batched-write save path: SaveChangesAsync with BatchWithSingleChangeset -----------

        // Batched-write save path: SaveChanges with batching produces a POST /$batch.
        [Fact]
        public async Task SaveChangesAsyncBatched_SendsProperRequest_WhenEntityNameMatchesTypeName()
        {
            using var handler = new RoutingMockHttpClientHandler(useBatchSaveResponse: true, isOrganizationBatch: false);
            var run = SingleThreadSynchronizationContext.Run(async () =>
            {
                var ctx = CreateContext(handler);
                ctx.AddObject("BlogPosts", new BlogPost { Id = 99, Name = "NewCo" });
                await ctx.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            });

            await AwaitWithTimeout(run.Task, run.Shutdown);
            Assert.Contains(handler.Requests, r => r.StartsWith("POST ") && r.EndsWith("/$batch"));
            Assert.Contains(handler.RequestBodies, b => b.Contains("\"@odata.type\":\"#Sample.API.Models.BlogPost\""));
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
            ctx3.AddObject("organizations", new Organization { Id = 99, Name = "NewCo" });
            await ctx3.SaveChangesAsync();
            Assert.Contains(handler3.Requests, r => r.StartsWith("POST "));
            Assert.Contains(handler3.RequestBodies, b => b.Contains("\"@odata.type\":\"#Sample.API.Models.organization\""));
        }

        // ----------- helpers -----------

        private static OrgContainer CreateContext(RoutingMockHttpClientHandler handler)
        {
            var factory = new MockHttpClientFactory(handler);
            return new OrgContainer(new Uri(ServiceRoot), factory);
        }

        // Races the call against the deadlock timeout. On timeout we dispose the pump's shutdown handle
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

        // Routes by URL: $metadata → EDMX; entity-set GET → JSON; POST → 201 with location.
        // Any unmatched URL produces a clear error rather than a silent hang.
        //
        // Critical: SendAsync must return a Task that completes ASYNCHRONOUSLY (not Task.FromResult).
        // Awaiting an already-completed Task runs the continuation synchronously and never marshals
        // through the captured SynchronizationContext — which would mask the very bug under test.
        private sealed class RoutingMockHttpClientHandler : MockHttpClientHandler
        {
            private readonly List<string> _requestBodies = new List<string>();
            private readonly object _sync = new object();

            public RoutingMockHttpClientHandler(bool filtered = false, bool useBatchSaveResponse = false, bool isOrganizationBatch = false)
                : base(req => Respond(req, filtered, useBatchSaveResponse, isOrganizationBatch))
            {
            }

            /// <summary>
            /// List of request bodies processed by this handler.
            /// </summary>
            public IReadOnlyList<string> RequestBodies
            {
                get
                {
                    lock (_sync)
                    {
                        return _requestBodies.AsReadOnly();
                    }
                }
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // Fast cancellation check - important for tests calling Abort/Timeout
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                string content = (request.Content == null)
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                lock (_sync)
                {
                    if (content != null)
                        _requestBodies.Add(content);
                }

                // Force asynchronous completion by hopping to the ThreadPool. The returned task is not
                // yet complete when the caller awaits it, so the await's continuation is posted via
                // SynchronizationContext.Current — exactly the path that deadlocks under issue #3521.
                return await base.SendAsync(request, cancellationToken);
            }

            private static HttpResponseMessage Respond(HttpRequestMessage req, bool filtered, bool useBatchSaveResponse, bool isOrganizationBatch)
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
                        Content = new StringContent(isOrganizationBatch
                            ? (useBatchSaveResponse ? BatchSaveOrganizationResponseBody : BatchOrganizationResponseBody)
                            : (useBatchSaveResponse ? BatchSaveBlogPostResponseBody : BatchBlogPostResponseBody)
                            )
                    };
                    resp.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse(
                            "multipart/mixed; boundary=batchresponse_test");
                    return resp;
                }

                if (req.Method == HttpMethod.Get && url.Contains("/organizations"))
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

                if (req.Method == HttpMethod.Post && url.EndsWith("/organizations", StringComparison.Ordinal))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(CreatedOrganizationResponse)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    resp.Headers.Location = new Uri(ServiceRoot + "/organizations(99)");
                    return resp;
                }

                if (req.Method == HttpMethod.Get && url.Contains("/BlogPosts"))
                {
                    string body = filtered ? BlogPostsFilteredResponse : BlogPostsResponse;
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(body)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    return resp;
                }

                if (req.Method == HttpMethod.Post && url.EndsWith("/BlogPosts", StringComparison.Ordinal))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(CreatedBlogPostResponse)
                    };
                    resp.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                        { CharSet = "utf-8" };
                    resp.Headers.Location = new Uri(ServiceRoot + "/BlogPosts(99)");
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
            Organizations = base.CreateQuery<Organization>("organizations");
            BlogPosts = base.CreateQuery<BlogPost>("BlogPosts");
            ResolveName = type
                => type == typeof(Organization) ? "Sample.API.Models.organization"
                : (type == typeof(BlogPost) ? "Sample.API.Models.BlogPost" : null);
            ResolveType = name
                => name == "Sample.API.Models.organization" ? typeof(Organization)
                : (name == "Sample.API.Models.BlogPost" ? typeof(BlogPost) : null);
        }

        public DataServiceQuery<Organization> Organizations { get; }
        public DataServiceQuery<BlogPost> BlogPosts { get; }
    }

    [global::Microsoft.OData.Client.Key("Id")]
    [global::Microsoft.OData.Client.EntitySet("organizations")]
    [global::Microsoft.OData.Client.OriginalNameAttribute("organization")]
    public class Organization : BaseEntityType, System.ComponentModel.INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    [global::Microsoft.OData.Client.Key("Id")]
    [global::Microsoft.OData.Client.EntitySet("BlogPosts")]
    [global::Microsoft.OData.Client.OriginalNameAttribute("BlogPost")]
    public class BlogPost : BaseEntityType, System.ComponentModel.INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
