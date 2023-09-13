using Azure;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Portal.Core.CsdlStorage
{
    public sealed class GithubCsdlStorage : ICsdlStorage
    {
        private readonly string repositoryOwner;

        private readonly string repositoryName;

        private readonly string branchName;

        private readonly string personalAccessToken;

        private readonly string userAgentHeader;

        public GithubCsdlStorage(string repositoryOwner, string repositoryName, string branchName, string personalAccessToken, string userAgentHeader)
        {
            this.repositoryOwner = repositoryOwner;
            this.repositoryName = repositoryName;
            this.branchName = branchName;
            this.personalAccessToken = personalAccessToken;
            this.userAgentHeader = userAgentHeader;
        }

        public async Task<byte[]> GetAsync(string identifier)
        {
            var csdlIdentifier = JsonSerializer.Deserialize<CsdlIdentifier>(Encoding.UTF8.GetString(Convert.FromBase64String(identifier)));
            if (csdlIdentifier == null)
            {
                throw new InvalidOperationException("TODO");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {personalAccessToken}");
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentHeader);

                using (var httpResponse = await httpClient.GetAsync(
                    $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/contents/{csdlIdentifier.FileName}?ref={csdlIdentifier.CommitId}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
                    var responseContent = JsonSerializer.Deserialize<Commit>(httpResponseContent);

                    if (!string.IsNullOrEmpty(responseContent?.Content))
                    {
                        return Convert.FromBase64String(responseContent.Content);
                    }

                    if (!string.IsNullOrEmpty(responseContent?.DownloadUrl))
                    {
                        using (var downloadRepsonse = await httpClient.GetAsync(responseContent?.DownloadUrl))
                        {
                            downloadRepsonse.EnsureSuccessStatusCode();
                            var downloadResponseContent = await downloadRepsonse.Content.ReadAsByteArrayAsync();
                            return downloadResponseContent;
                        }
                    }

                    throw new InvalidOperationException("TODO");
                }
            }
        }

        public async Task<string> PutAsync(byte[] data)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {personalAccessToken}");
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentHeader);

                var requestContent = new
                {
                    message = "adding a playground csdl",
                    committer = new
                    {
                        name = "playground", //// TODO
                        email = "playground@playground.com" //// TODO
                    },
                    content = Convert.ToBase64String(data),
                    branch = branchName,
                };
                using (var httpRequestContent = JsonContent.Create(requestContent))
                {
                    var csdlIdentifierBuilder = new CsdlIdentifier.Builder();
                    csdlIdentifierBuilder.FileName = Guid.NewGuid().ToString();
                    using (var httpResponse = await httpClient.PutAsync(
                        $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/contents/{csdlIdentifierBuilder.FileName}",
                        httpRequestContent))
                    {
                        httpResponse.EnsureSuccessStatusCode();
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
                        var responseContent = JsonSerializer.Deserialize<CommitResponse>(httpResponseContent);
                        if (responseContent?.Commit?.Sha == null)
                        {
                            throw new InvalidOperationException("TODO");
                        }

                        csdlIdentifierBuilder.CommitId = responseContent.Commit.Sha;
                        var serializedCsdlId = JsonSerializer.Serialize(csdlIdentifierBuilder.Build());
                        var encodedCsdlId = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedCsdlId));

                        return encodedCsdlId;
                    }
                }
            }
        }

        private sealed class CsdlIdentifier
        {
            public CsdlIdentifier(string fileName, string commitId)
            {
                FileName = fileName;
                CommitId = commitId;
            }

            public string FileName { get; }

            public string CommitId { get; }

            public struct Builder
            {
                public string FileName { get; set; }

                public string CommitId { get; set; }

                public CsdlIdentifier Build()
                {
                    return new CsdlIdentifier(FileName, CommitId);
                }
            }
        }

        private sealed class CommitResponse
        {
            [JsonPropertyName("commit")]
            public Commit? Commit { get; set; }
        }

        private sealed class Commit
        {
            [JsonPropertyName("sha")]
            public string? Sha { get; set; }

            [JsonPropertyName("content")]
            public string? Content { get; set; }

            [JsonPropertyName("download_url")]
            public string? DownloadUrl { get; set; }
        }
    }
}
