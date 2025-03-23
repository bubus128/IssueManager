using System.Text.Json.Serialization;

namespace IssueManager.Core.Models;
public class User
{
	[JsonPropertyName("login")]
	public string Login { get; set; } = string.Empty;

	[JsonPropertyName("html_url")]
	public string ProfileUrl { get; set; } = string.Empty;
}
