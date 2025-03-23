using IssueManager.Core.Models.Interfaces;
using System.Text.Json.Serialization;

namespace IssueManager.Core.Models;
public class GithubIssue : IIssue
{
	[JsonPropertyName("id")]
	public long Id { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("state")]
	public string State { get; set; } = string.Empty;

	[JsonPropertyName("html_url")]
	public string Url { get; set; } = string.Empty;

	[JsonPropertyName("repository_url")]
	public string RepositoryUrl { get; set; } = string.Empty;

	[JsonPropertyName("user")]
	public User User { get; set; } = new();

	[JsonPropertyName("assignee")]
	public User? Assignee { get; set; }

	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("updated_at")]
	public DateTime UpdatedAt { get; set; }

	// New properties for creating/updating issues
	[JsonPropertyName("body")]
	public string? Description { get; set; }

	[JsonPropertyName("assignees")]
	public List<string>? Assignees { get; set; } // GitHub API accepts a list of assignees
}
