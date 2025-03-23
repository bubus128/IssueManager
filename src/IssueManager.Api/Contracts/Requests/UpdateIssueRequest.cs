namespace IssueManager.Api.Contracts.Requests;

public class UpdateIssueRequest
{
	public required string Title { get; set; }
	public string? Body { get; set; }
}
