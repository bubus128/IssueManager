using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IssueManager.Core.RequestFactory;
public class GitLabRequestFactory(IOptions<GitLabSourceConfig> options) : IRequestFactory
{
	/// <summary>
	/// Creates a basic HttpRequestMessage object with the required headers.
	/// In this case, we set the authorization header, User-Agent, and accepted MIME type.
	/// </summary>
	public HttpRequestMessage CreateBaseRequest()
	{
		var request = new HttpRequestMessage();
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AuthToken);
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MyGitLabApp", "1.0"));
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to get all issues from a project.
	/// Note: In GitLab, issues are related to a project, so we require the owner and repository to be provided.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	public HttpRequestMessage CreateGetAllRequest()
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		var projectId = Uri.EscapeDataString($"somegt/issuemanager");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues?state=all");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to get the details of a single issue.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number (issue IID in GitLab)</param>
	public HttpRequestMessage CreateGetRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues/{issueNumber}");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to update an existing issue.
	/// Note: In GitLab, issue updates are done using the PUT method.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number (issue IID in GitLab)</param>
	public HttpRequestMessage CreatePatchRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Put;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues/{issueNumber}");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to close an issue.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number (issue IID in GitLab)</param>
	public HttpRequestMessage CreateCloseRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Put;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues/{issueNumber}");
		var updateContent = new { state_event = "close" };
		request.Content = JsonContent.Create(updateContent);
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to create a new issue in a project.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	public HttpRequestMessage CreatePostRequest(string owner, string repo)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Post;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues");
		return request;
	}
}
