namespace IssueManager.Api.Contracts.Requests;

public class CreateIssueRequest
{
	public required string Title { get; set; }
	public string? Body { get; set; }
}
