using IssueManager.Core.Enums;
using IssueManager.Core.Exceptions;
using IssueManager.Core.Models;
using IssueManager.Core.Models.Interfaces;
using IssueManager.Core.RepositoryService.Interfaces;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace IssueManager.Core.RepositoryService;
public class RepositoryService
	(
		HttpClient httpClient,
		[FromKeyedServices("GitHub")] IRequestFactory githubRequestFactory,
		[FromKeyedServices("GitLab")] IRequestFactory gitLabRequestFactory
	) : IRepositoryService
{
	/// <summary>
	/// A dictionary mapping source types (GitHub, GitLab) to their corresponding request factories.
	/// </summary>
	private Dictionary<SourceType, IRequestFactory> _requestFactories = new()
	{
		{SourceType.GitHub, githubRequestFactory},
		{SourceType.GitLab, gitLabRequestFactory}
	};

	/// <summary>
	/// Closes an issue in a given repository.
	/// </summary>
	/// <param name="source">The source (GitHub or GitLab)</param>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number</param>
	public async Task CloseIssue(SourceType source, string owner, string repo, int issueNumber)
	{
		var requestFactory = _requestFactories[source];
		var request = requestFactory.CreateCloseRequest(owner, repo, issueNumber);

		try
		{
			var response = await httpClient.SendAsync(request);

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new IssueNotFoundException();
			}
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"GitHub API returned {response.StatusCode}");
			}

			Console.WriteLine($"Issue {issueNumber} in {owner}/{repo} has been successfully closed.");
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error closing issue {issueNumber} in {owner}/{repo}: {exception.Message}");
			throw;
		}
	}

	/// <summary>
	/// Retrieves all issues from a given repository.
	/// </summary>
	/// <param name="source">The source (GitHub or GitLab)</param>
	/// <returns>A list of issues</returns>
	public async Task<List<IIssue>> GetAllIssues(SourceType source)
	{
		var requestFactory = _requestFactories[source];
		var request = requestFactory.CreateGetAllRequest();

		try
		{
			var response = await httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"GitHub API returned {response.StatusCode}");
			}

			var json = await response.Content.ReadAsStringAsync();
			var issues = JsonSerializer.Deserialize<List<Issue>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			return issues?.Cast<IIssue>().ToList() ?? [];
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error getting issues: {exception.Message}");
			throw;
		}
	}

	/// <summary>
	/// Retrieves details of a single issue from a given repository.
	/// </summary>
	/// <param name="source">The source (GitHub or GitLab)</param>
	/// <param name="issueNumber">The issue number</param>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <returns>The issue details</returns>
	public async Task<IIssue> GetIssue(SourceType source, int issueNumber, string owner, string repo)
	{
		var requestFactory = _requestFactories[source];
		var request = requestFactory.CreateGetRequest(owner, repo, issueNumber);

		try
		{
			var response = await httpClient.SendAsync(request);

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new IssueNotFoundException();
			}
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"GitHub API returned {response.StatusCode}");
			}

			var json = await response.Content.ReadAsStringAsync();
			var issue = JsonSerializer.Deserialize<Issue>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return issue ?? throw new IssueNotFoundException();
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error getting issue {issueNumber} from {owner}/{repo}: {exception.Message}");
			throw;
		}
	}

	/// <summary>
	/// Creates a new issue in a given repository.
	/// </summary>
	/// <param name="issue">The issue to be created</param>
	/// <param name="source">The source (GitHub or GitLab)</param>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <returns>The created issue</returns>
	public async Task<IIssue?> CreateIssue(IIssue issue, SourceType source, string owner, string repo)
	{
		var requestFactory = _requestFactories[source];
		var request = requestFactory.CreatePostRequest(owner, repo);

		var newIssueContent = new
		{
			title = issue.Title,
			body = issue.Description,
		};

		request.Content = JsonContent.Create(newIssueContent);

		try
		{
			var response = await httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"GitHub API returned {response.StatusCode}");
			}

			var json = await response.Content.ReadAsStringAsync();
			var createdIssue = JsonSerializer.Deserialize<Issue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return createdIssue;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error creating issue: {exception.Message}");
			throw;
		}
	}

	/// <summary>
	/// Updates an existing issue in a given repository.
	/// </summary>
	/// <param name="issue">The issue with updated details</param>
	/// <param name="source">The source (GitHub or GitLab)</param>
	/// <param name="owner">The name of the owner (namespace)</param>
	/// <param name="repo">The name of the repository/project</param>
	/// <param name="issueNumber">The issue number</param>
	/// <returns>The updated issue</returns>
	public async Task<IIssue?> UpdateIssue(IIssue issue, SourceType source, string owner, string repo, int issueNumber)
	{
		var requestFactory = _requestFactories[source];
		var request = requestFactory.CreatePatchRequest(owner, repo, issueNumber);

		var updateContent = new
		{
			title = issue.Title,
			body = issue.Description,
		};

		request.Content = JsonContent.Create(updateContent);

		try
		{
			var response = await httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"GitHub API returned {response.StatusCode}");
			}

			var json = await response.Content.ReadAsStringAsync();
			var updatedIssue = JsonSerializer.Deserialize<Issue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return updatedIssue;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error updating issue: {exception.Message}");
			throw;
		}
	}

}
