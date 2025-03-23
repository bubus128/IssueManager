namespace IssueManager.Core.Configuration;
public class GitLabSourceConfig
{
	public const string Name = "GitLabConfig";
	public string ApiBaseUrl { get; set; } = string.Empty;
	public string AuthToken { get; set; } = string.Empty;
}
