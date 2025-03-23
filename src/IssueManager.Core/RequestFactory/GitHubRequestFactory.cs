using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IssueManager.Core.RequestFactory;
public class GitHubRequestFactory(IOptions<GitHubSourceConfig> options) : IRequestFactory
{
	/// <summary>
	/// Creates a basic HttpRequestMessage object with the required headers.
	/// In this case, we set the authorization header, User-Agent, and accepted MIME type.
	/// </summary>
	public HttpRequestMessage CreateBaseRequest()
	{
		var request = new HttpRequestMessage();
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AuthToken);
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MyGitHubApp", "1.0"));
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to get all issues from a repository.
	/// </summary>
	public HttpRequestMessage CreateGetAllRequest()
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/issues?state=all");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to get the details of a single issue.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number</param>
	public HttpRequestMessage CreateGetRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to close an issue.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number</param>
	public HttpRequestMessage CreateCloseRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Patch;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		var updateContent = new { state = "closed" };
		request.Content = JsonContent.Create(updateContent);
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to update an existing issue.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number</param>
	public HttpRequestMessage CreatePatchRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Patch;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		return request;
	}

	/// <summary>
	/// Creates an HTTP request to create a new issue in a repository.
	/// </summary>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	public HttpRequestMessage CreatePostRequest(string owner, string repo)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Post;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues");
		return request;
	}
}
