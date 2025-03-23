using IssueManager.Core.Models.Interfaces;

namespace IssueManager.Core.Models;

public class Issue : IIssue
{
	public long Id { get; set; }
	public required string Title { get; set; }
	public string? State { get; set; }
	public string? Description { get; set; }
	public string? Url { get; set; }
}
