namespace IssueManager.Core.Configuration;
public class GitHubSourceConfig
{
	public const string Name = "GitHubConfig";
	public string ApiBaseUrl { get; set; } = string.Empty;
	public string AuthToken { get; set; } = string.Empty;
}
