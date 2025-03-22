using IssueManager.Core.Models.Interfaces;
using IssueManager.Core.RepositoryService.Interfaces;

namespace IssueManager.Core.RepositoryService;
public class RepositoryService : IRepositoryService
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task DeleteIssue<T>(T id)
	{
		throw new NotImplementedException();
	}

	public async Task<List<IIssue>> GetAllIssues()
	{
		throw new NotImplementedException();
	}

	public async Task<IIssue> GetIssueById<T>(T id)
	{
		throw new NotImplementedException();
	}

	public async Task<T> CreateIssue<T>(IIssue issue)
	{
		throw new NotImplementedException();
	}

	public async Task UpdateIssue(IIssue issue)
	{
		throw new NotImplementedException();
	}

	Task<T> IRepositoryService.UpdateIssue<T>(IIssue issue)
	{
		throw new NotImplementedException();
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
