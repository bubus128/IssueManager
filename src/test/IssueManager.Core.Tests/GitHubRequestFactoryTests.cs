using IssueManager.Core.Configuration;
using IssueManager.Core.RequestFactory;
using Microsoft.Extensions.Options;
using Moq;

namespace IssueManager.Core.Tests;
[TestFixture]
public class GitHubRequestFactoryTests
{
	private Mock<IOptions<GitHubSourceConfig>> _mockOptions;
	private GitHubRequestFactory _githubRequestFactory;

	[SetUp]
	public void SetUp()
	{
		_mockOptions = new Mock<IOptions<GitHubSourceConfig>>();
		_mockOptions.Setup(o => o.Value).Returns(new GitHubSourceConfig { AuthToken = "test_token", ApiBaseUrl = "https://api.github.com" });
		_githubRequestFactory = new GitHubRequestFactory(_mockOptions.Object);
	}

	[Test]
	public void CreateBaseRequest_ShouldSetAuthorizationHeader()
	{
		var request = _githubRequestFactory.CreateBaseRequest();

		Assert.That(request.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
		Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo("test_token"));
	}

	[Test]
	public void CreateGetAllRequest_ShouldReturnCorrectRequestUri()
	{
		var request = _githubRequestFactory.CreateGetAllRequest();

		Assert.That(request.RequestUri.ToString(), Is.EqualTo("https://api.github.com/issues?state=all"));
		Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
	}
}
