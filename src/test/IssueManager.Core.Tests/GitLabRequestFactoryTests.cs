using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory;
using Microsoft.Extensions.Options;
using Moq;

namespace IssueManager.Core.Tests;
[TestFixture]
public class GitLabRequestFactoryTests
{
	private Mock<IOptions<GitLabSourceConfig>> _mockOptions;
	private GitLabRequestFactory _gitLabRequestFactory;

	[SetUp]
	public void SetUp()
	{
		_mockOptions = new Mock<IOptions<GitLabSourceConfig>>();
		_mockOptions.Setup(o => o.Value).Returns(new GitLabSourceConfig { AuthToken = "test_token", ApiBaseUrl = "https://gitlab.com/api/v4" });
		_gitLabRequestFactory = new GitLabRequestFactory(_mockOptions.Object);
	}

	[Test]
	public void CreateBaseRequest_ShouldSetAuthorizationHeader()
	{
		var request = _gitLabRequestFactory.CreateBaseRequest();

		Assert.That(request.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
		Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo("test_token"));
	}

	[Test]
	public void CreateGetAllRequest_ShouldReturnCorrectRequestUri()
	{
		var request = _gitLabRequestFactory.CreateGetAllRequest();

		Assert.That(request.RequestUri.ToString(), Is.EqualTo("https://gitlab.com/api/v4/projects/somegt%2Fissuemanager/issues?state=all"));
		Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
	}
}
