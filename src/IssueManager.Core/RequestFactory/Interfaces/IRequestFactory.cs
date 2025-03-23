namespace IssueManager.Core.RequestFactory.Interfaces;
public interface IRequestFactory
{
	public HttpRequestMessage CreateCloseRequest(string owner, string repo, int issueNumber);
	public HttpRequestMessage CreateGetRequest(string owner, string repo, int issueNumber);
	public HttpRequestMessage CreateGetAllRequest();
	public HttpRequestMessage CreatePostRequest(string owner, string repo);
	public HttpRequestMessage CreatePatchRequest(string owner, string repo, int issueNumber);
}
