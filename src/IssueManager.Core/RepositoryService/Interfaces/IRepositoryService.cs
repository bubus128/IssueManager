using IssueManager.Core.Models.Interfaces;

namespace IssueManager.Core.RepositoryService.Interfaces;

public interface IRepositoryService
{
	public Task<List<IIssue>> GetAllIssues();
	public Task<IIssue> GetIssueById<T>(T id);
	public Task<T> UpdateIssue<T>(IIssue issue);
	public Task<T> CreateIssue<T>(IIssue issue);
	public Task DeleteIssue<T>(T id);
}
