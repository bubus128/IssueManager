namespace IssueManager.Core.Models.Interfaces;
public interface IIssue
{
	long Id { get; set; }
	string Title { get; set; }
	string State { get; set; }
	string Description { get; set; }
	string Url { get; set; }
}
