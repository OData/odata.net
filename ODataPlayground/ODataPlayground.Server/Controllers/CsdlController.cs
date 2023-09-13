namespace Portal.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Json;

    [ApiController]
    [Route("csdls")]
    public sealed class CsdlController : ControllerBase
    {
        private readonly string repositoryOwner;

        private readonly string repositoryName;

        private readonly string branchName;

        private readonly string githubPersonalAccessToken;

        public CsdlController(IConfiguration configuration)
        {
            this.repositoryOwner = configuration["CsdlStorage:RepositoryOwner"] ?? throw new InvalidOperationException("No github repository owner was specified in the configuration");
            this.repositoryName = configuration["CsdlStorage:RepositoryName"] ?? throw new InvalidOperationException("No github repository name was specified in the configuration");
            this.branchName = configuration["CsdlStorage:BranchName"] ?? throw new InvalidOperationException("No github branch name was specified in the configuration");
            this.githubPersonalAccessToken = configuration["CsdlStorage:GithubPersonalAccessToken"] ?? throw new InvalidOperationException("No github personal access key was specified in the configuration");
        }

        [HttpPut]
        public async Task<IActionResult> Put([ValidateNever][FromBody] string data)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.githubPersonalAccessToken}");
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "requiredTODO");

                var requestContent = new
                {
                    message = "adding a playground csdl",
                    committer = new {
                        name = "Monalisa Octocat", //// TODO 
                        email = "octocat@github.com", //// TODO
                    },
                    content = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)), //// TODO add the ags csdls to this
                    branch = this.branchName,
                };
                using (var httpRequestContent = JsonContent.Create(requestContent))
                {
                    var csdlId = new CsdlId();
                    csdlId.FileName = Guid.NewGuid().ToString();
                    using (var httpResponse = await httpClient.PutAsync($"https://api.github.com/repos/{this.repositoryOwner}/{this.repositoryName}/contents/{csdlId.FileName}", httpRequestContent))
                    {
                        httpResponse.EnsureSuccessStatusCode();
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
                        var responseContent = JsonSerializer.Deserialize<CommitResponse>(httpResponseContent);

                        csdlId.CommitId = responseContent.Commit.Sha;
                        var serializedCsdlId = JsonSerializer.Serialize(csdlId);
                        var encodedCsdlId = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedCsdlId));

                        var location = $"https://{HttpContext.Request.Host}?csdl={encodedCsdlId}";
                        Response.Headers.Add("Location", location);
                    }
                }
            }

            return Ok();
        }

        private sealed class CsdlId
        {
            public string FileName { get; set; }

            public string CommitId { get; set; }
        }

        private sealed class CommitResponse
        {
            [JsonPropertyName("commit")]
            public Commit Commit { get; set; }
        }

        private sealed class Commit
        {
            [JsonPropertyName("sha")]
            public string Sha { get; set; }

            [JsonPropertyName("content")]
            public string Content { get; set; }
        }

        [HttpGet]
        [Route("{encodedCsdlId}")]
        public async Task<IActionResult> Get([FromRoute] string encodedCsdlId)
        {
            var serializedCsdlId = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCsdlId));
            var csdlId = JsonSerializer.Deserialize<CsdlId>(serializedCsdlId);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.githubPersonalAccessToken}");
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "requiredTODO");

                using (var httpResponse = await httpClient.GetAsync($"https://api.github.com/repos/{this.repositoryOwner}/{this.repositoryName}/contents/{csdlId.FileName}?ref={csdlId.CommitId}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    var responseContent = JsonSerializer.Deserialize<Commit>(httpResponseContent);

                    return Ok(Encoding.UTF8.GetString(Convert.FromBase64String(responseContent.Content)));
                }
            }
        }
    }
}
