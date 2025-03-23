using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IssueManager.Core.RequestFactory;
public class GitLabRequestFactory(IOptions<GitLabSourceConfig> options) : IRequestFactory
{
	/// <summary>
	/// Tworzy podstawowy obiekt HttpRequestMessage z wymaganymi nagłówkami.
	/// W tym przypadku ustawiamy nagłówek autoryzacji, User-Agent oraz akceptowany typ MIME.
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
	/// Tworzy zapytanie HTTP do pobrania wszystkich zadań z projektu.
	/// Uwaga: w GitLab zadania są związane z projektem, dlatego wymagamy podania ownera i repozytorium.
	/// </summary>
	/// <param name="owner">Nazwa właściciela (namespace)</param>
	/// <param name="repo">Nazwa repozytorium/projektu</param>
	public HttpRequestMessage CreateGetAllRequest()
	{

		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		var projectId = Uri.EscapeDataString($"somegt/issuemanager");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues?state=all");
		return request;
	}

	/// <summary>
	/// Tworzy zapytanie HTTP do pobrania szczegółów pojedynczego zadania.
	/// </summary>
	/// <param name="owner">Nazwa właściciela (namespace)</param>
	/// <param name="repo">Nazwa repozytorium/projektu</param>
	/// <param name="issueNumber">Numer zadania (issue IID w GitLab)</param>
	public HttpRequestMessage CreateGetRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Get;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues/{issueNumber}");
		return request;
	}

	/// <summary>
	/// Tworzy zapytanie HTTP do aktualizacji istniejącego zadania.
	/// Uwaga: W GitLab aktualizacja zadania odbywa się przy użyciu metody PUT.
	/// </summary>
	/// <param name="owner">Nazwa właściciela (namespace)</param>
	/// <param name="repo">Nazwa repozytorium/projektu</param>
	/// <param name="issueNumber">Numer zadania (issue IID w GitLab)</param>
	public HttpRequestMessage CreatePatchRequest(string owner, string repo, int issueNumber)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Put;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues/{issueNumber}");
		return request;
	}

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
	/// Tworzy zapytanie HTTP do utworzenia nowego zadania w projekcie.
	/// </summary>
	/// <param name="owner">Nazwa właściciela (namespace)</param>
	/// <param name="repo">Nazwa repozytorium/projektu</param>
	public HttpRequestMessage CreatePostRequest(string owner, string repo)
	{
		var request = CreateBaseRequest();
		request.Method = HttpMethod.Post;
		var projectId = Uri.EscapeDataString($"{owner}/{repo}");
		request.RequestUri = new Uri($"{options.Value.ApiBaseUrl}/projects/{projectId}/issues");
		return request;
	}
}
