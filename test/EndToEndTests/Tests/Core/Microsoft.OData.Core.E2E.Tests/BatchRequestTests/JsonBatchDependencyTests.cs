//---------------------------------------------------------------------
// <copyright file="JsonBatchDependencyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.Core.E2E.Tests.BatchRequestTests;

public class JsonBatchDependencyTests : EndToEndTestBase<JsonBatchDependencyTests.TestsStartup>
{
    public JsonBatchDependencyTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
    }

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(JsonBatchDependencyController), typeof(MetadataController));
            services.AddControllers().AddOData(options => options.AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
        }
    }

    public class JsonBatchDependencyController : ODataController
    {
        [HttpGet("odata/Accounts(101)")]
        public IActionResult GetAccount()
        {
            return Ok(new Account { AccountID = 101 });
        }

        [HttpPatch("odata/Accounts(101)")]
        public IActionResult PatchAccount()
        {
            return NoContent();
        }
    }

    [Theory]
    [InlineData(0, "", 1, 2, 0)]
    [InlineData(0, "\"dependsOn\":[],", 1, 2, 0)]
    [InlineData(0, "\"dependsOn\":[\"top\",\"top\",\"top\"],", 1, 2, 0)]
    [InlineData(1, "\"dependsOn\":[\"g0\"],", 1, 2, 0)]
    [InlineData(1, "\"dependsOn\":[\"g0\",\"top\",\"g0\",\"top\"],", 1, 2, 0)]
    [InlineData(3, "\"dependsOn\":[\"top\",\"top\",\"top\"],", 2, 2, 0)]
    [InlineData(0, "", 1, 0, 1)]
    [InlineData(0, "", 1, 0, 1000)]
    [InlineData(3, "", 1, 100, 0)]
    [InlineData(3, "", 1, 100, 1)]
    [InlineData(3, "", 1, 100, 1000)]
    public async Task JsonBatchDispatchesDependenciesConsistently(int groupCount, string dependencyProperty, int batchCount, int membersPerGroup, int dependencyCount)
    {
        Assert.Equal(typeof(ODataMessageReader).Assembly.Location, Path.Combine(AppContext.BaseDirectory, "Microsoft.OData.Core.dll"));
        Assert.Same(typeof(ODataMessageReader).Assembly, Assembly.Load(typeof(DefaultODataBatchHandler).Assembly.GetReferencedAssemblies().Single(assembly => assembly.Name == "Microsoft.OData.Core")));

        var payload = new StringBuilder("""{"requests":[""");
        var expectedIds = new List<string>();
        for (int group = 0; group < groupCount; group++)
        {
            for (int member = 0; member < membersPerGroup; member++)
            {
                string id = $"m{group}_{member}";
                expectedIds.Add(id);
                payload.Append($$$"""{"id":"{{{id}}}","atomicityGroup":"g{{{group}}}","method":"PATCH","url":"Accounts(101)","headers":{"content-type":"application/json"},"body":{}},""");
            }
        }

        expectedIds.Add("top");
        expectedIds.Add("dependent");
        payload.Append("""{"id":"top","method":"GET","url":"Accounts(101)","headers":{}},""");
        payload.Append("""{"id":"dependent",""");
        if (dependencyCount > 0)
        {
            payload.Append("\"dependsOn\":[").AppendJoin(',', Enumerable.Repeat("\"top\"", dependencyCount)).Append("],");
        }
        else
        {
            payload.Append(dependencyProperty);
        }

        payload.Append("\"method\":\"GET\",\"url\":\"Accounts(101)\",\"headers\":{}}]}");

        for (int batch = 0; batch < batchCount; batch++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "odata/$batch");
            request.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
            request.Headers.Add("OData-Version", "4.0");
            using var response = await Client.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = JsonDocument.Parse(body);
            var responses = document.RootElement.GetProperty("responses");
            Assert.Equal(expectedIds.Count, responses.GetArrayLength());
            for (int i = 0; i < expectedIds.Count; i++)
            {
                Assert.Equal(expectedIds[i], responses[i].GetProperty("id").GetString());

                var memberCount = groupCount * membersPerGroup;
                Assert.Equal(i < memberCount ? 204 : 200, responses[i].GetProperty("status").GetInt32());
                if (i >= memberCount)
                {
                    Assert.Equal(101, responses[i].GetProperty("body").GetProperty("accountID").GetInt32());
                }
            }
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(500)]
    public async Task JsonBatchDispatchesEarlierMembersOfSameGroup(int memberCount)
    {
        var payload = new StringBuilder("""{"requests":[""");
        var expectedIds = new List<string>();
        for (int member = 0; member < memberCount; member++)
        {
            string id = $"r{member}";
            if (member > 0)
            {
                payload.Append(',');
            }

            payload.Append($$$"""{"id":"{{{id}}}","atomicityGroup":"group",""");
            if (member == memberCount - 1)
            {
                payload.Append("\"dependsOn\":[").AppendJoin(',', expectedIds.Select(previousId => $"\"{previousId}\"")).Append("],");
            }

            payload.Append("\"method\":\"PATCH\",\"url\":\"Accounts(101)\",\"headers\":{\"content-type\":\"application/json\"},\"body\":{}}");
            expectedIds.Add(id);
        }

        payload.Append("]}");
        using var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
        using var response = await Client.PostAsync("odata/$batch", content);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(body);
        var responses = document.RootElement.GetProperty("responses");
        Assert.Equal(memberCount, responses.GetArrayLength());
        for (int member = 0; member < memberCount; member++)
        {
            Assert.Equal(expectedIds[member], responses[member].GetProperty("id").GetString());
            Assert.Equal(204, responses[member].GetProperty("status").GetInt32());
        }
    }

    [Theory]
    [InlineData("member", true)]
    [InlineData("member", false)]
    [InlineData("missing", true)]
    [InlineData("dependent", true)]
    [InlineData("future", true)]
    public async Task JsonBatchRejectsInvalidDependencyAndNextBatchSucceeds(string dependency, bool includeDependentId)
    {
        string dependentIdProperty = includeDependentId ? "\"id\":\"dependent\"," : "";
        string payload = $$$"""{"requests":[{"id":"member","atomicityGroup":"group","method":"PATCH","url":"Accounts(101)","headers":{"content-type":"application/json"},"body":{}},{{{{dependentIdProperty}}}"dependsOn":["{{{dependency}}}"],"method":"GET","url":"Accounts(101)","headers":{}},{"id":"future","method":"GET","url":"Accounts(101)","headers":{}}]}""";
        using var invalidContent = new StringContent(payload, Encoding.UTF8, "application/json");
        // This host has no exception translator; preserve the handler's original exception.
        var exception = await Assert.ThrowsAsync<ODataException>(() => Client.PostAsync("odata/$batch", invalidContent));
        string expectedMessage = dependency == "member"
            ? Error.Format(SRResources.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed, "member", "group")
            : dependency == "dependent"
                ? Error.Format(SRResources.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed, "dependent", "dependent")
                : Error.Format(SRResources.ODataBatchReader_DependsOnIdNotFound, dependency, "dependent");
        Assert.Equal(expectedMessage, exception.Message);

        using var validContent = new StringContent("""{"requests":[{"id":"member","method":"GET","url":"Accounts(101)","headers":{}}]}""", Encoding.UTF8, "application/json");
        using var validResponse = await Client.PostAsync("odata/$batch", validContent);
        string body = await validResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);
        using var document = JsonDocument.Parse(body);
        var responses = document.RootElement.GetProperty("responses");
        Assert.Equal(1, responses.GetArrayLength());
        Assert.Equal("member", responses[0].GetProperty("id").GetString());
        Assert.Equal(200, responses[0].GetProperty("status").GetInt32());
        Assert.Equal(101, responses[0].GetProperty("body").GetProperty("accountID").GetInt32());
    }
}
