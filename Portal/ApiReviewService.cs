using GraphApiReview.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace GraphApiReview;

public class ApiReviewService : IDisposable
{
    public ApiReviewService(IConfiguration config, ILogger<ApiReviewService> logger)
    {
        var saveApiResponses = config.GetValue<bool>("SaveApiResponses");
        
        var reviewers = config.GetSection("GAPIR:Reviewers").Get<string[]>()!;
        logger.LogInformation("reviewers: {0}", string.Join(", ", reviewers));
    }

    public void Dispose()
    {
    }

    /// <summary>
    /// Retrieve the composite api review work item + api review pull request items that are related to the logged in user
    /// </summary>
    /// <returns>The tuple of the composite items related to the logged in user and the composite items related to the logged in user that require action from them</returns>
    public async Task<(IEnumerable<ApiReviewRequest> relatedToMe, IEnumerable<ApiReviewRequest> actionRequired, IEnumerable<ApiReviewRequestError> errors)> GetMyWork(string upn)
    {
        return await Task.FromResult((Enumerable.Empty<ApiReviewRequest>(), Enumerable.Empty<ApiReviewRequest>(), Enumerable.Empty<ApiReviewRequestError>()));
    }
}
