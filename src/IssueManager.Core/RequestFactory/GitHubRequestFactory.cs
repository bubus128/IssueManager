using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IssueManager.Core.RequestFactory;
public class GitHubRequestFactory(IOptions<GitHubSourceConfig> options) : IRequestFactory
{
	public HttpRequestMessage CreateBaseRequest()
	{
		var request = new HttpRequestMessage();
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AuthToken);
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MyGitHubApp", "1.0"));
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
		return request;
	}

	public HttpRequestMessage CreateGetAllRequest()
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/issues?state=all");
		return request;
	}

	public HttpRequestMessage CreateGetRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		return request;
	}

	public HttpRequestMessage CreateCloseRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Patch;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		var updateContent = new { state = "closed" };
		request.Content = JsonContent.Create(updateContent);
		return request;
	}

	public HttpRequestMessage CreatePatchRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Patch;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues/{issueNumber}");
		return request;
	}

	public HttpRequestMessage CreatePostRequest(string owner, string repo)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Post;
		request.RequestUri = new($"{options.Value.ApiBaseUrl}/repos/{owner}/{repo}/issues");
		return request;
	}

}
