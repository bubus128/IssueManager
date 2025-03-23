using IssueManager.Core.Enums;
using IssueManager.Core.Models.Interfaces;

namespace IssueManager.Core.RepositoryService.Interfaces;

public interface IRepositoryService
{
	public Task<List<IIssue>> GetAllIssues(SourceType source);
	public Task<IIssue> GetIssue(SourceType source, int issueNumber, string owner, string repo);
	public Task<IIssue?> UpdateIssue(IIssue issue, SourceType source, string owner, string repo, int number);
	public Task<IIssue?> CreateIssue(IIssue issue, SourceType source, string owner, string repo);
	public Task CloseIssue(SourceType source, string owner, string repo, int issueNumber);
}
