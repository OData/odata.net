
namespace GraphApiReview.Model;

public abstract class ApiReviewRequestBase
{
}

public class ApiReviewRequestError : ApiReviewRequestBase
{
    public ApiReviewRequestError(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; init; }
}

public class ApiReviewRequest : ApiReviewRequestBase
{
    public long WorkItemId { get; init; }

    public string Url => $"https://microsoftgraph.visualstudio.com/onboarding/_workitems/edit/{WorkItemId}";

    public ApiReviewPullRequest PullRequest { get; init; } = default!;

}
