using IssueManager.Api.Contracts.Requests;
using IssueManager.Core.Configuration;
using IssueManager.Core.Enums;
using IssueManager.Core.Exceptions;
using IssueManager.Core.Models;
using IssueManager.Core.RepositoryService;
using IssueManager.Core.RepositoryService.Interfaces;
using IssueManager.Core.RequestFactory;
using IssueManager.Core.RequestFactory.Interfaces;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register OpenApi for endpoint documentation
builder.Services.AddOpenApi();

// Register HttpClient for sending http requests
builder.Services.AddHttpClient();

// Register configs
builder.Services.Configure<GitHubSourceConfig>(builder.Configuration.GetSection(GitHubSourceConfig.Name));
builder.Services.Configure<GitLabSourceConfig>(builder.Configuration.GetSection(GitLabSourceConfig.Name));

// RegisterServices
builder.Services.AddSingleton<IRepositoryService, RepositoryService>();
builder.Services.AddKeyedSingleton<IRequestFactory, GitHubRequestFactory>("GitHub");
builder.Services.AddKeyedSingleton<IRequestFactory, GitLabRequestFactory>("GitLab");

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/issuess/{source}", async Task<IResult> (string source, IRepositoryService repositoryService) =>
{
	if (!Enum.TryParse<SourceType>(source, true, out var sourceType))
	{
		return TypedResults.BadRequest("Invalid source. Allowed values: GitHub, GitLab.");
	}
	try
	{
		return TypedResults.Ok(await repositoryService.GetAllIssues(sourceType));
	}
	catch (Exception exception)
	{
		Console.Error.WriteLine($"Error getting issues: {exception.Message}");
		return TypedResults.Problem("An unexpected error occurred while getting issues.", statusCode: 500);
	}
})
.WithName("GetIssues")
.WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Get all issues",
	Description = "Returns all open issues.",
	Tags = new List<OpenApiTag> { new() { Name = "All issues" } }
});

app.MapGet("/issues/{source}/{owner}/{repo}/{issueNumber}", async Task<IResult> (string source, string owner, string repo, int issueNumber, IRepositoryService repositoryService) =>
{
	if (!Enum.TryParse<SourceType>(source, true, out var sourceType))
	{
		return TypedResults.BadRequest("Invalid source. Allowed values: GitHub, GitLab.");
	}
	try
	{
		return TypedResults.Ok(await repositoryService.GetIssue(sourceType, issueNumber, owner, repo));
	}
	catch (IssueNotFoundException)
	{
		return TypedResults.NotFound($"Issue {issueNumber} not found in {owner} / {repo}.");
	}
	catch (Exception exception)
	{
		Console.Error.WriteLine($"Error getting issue  {issueNumber}  from  {owner} / {repo}: {exception.Message}");
		return TypedResults.Problem($"An unexpected error occurred while getting issue with ID: {issueNumber}.", statusCode: 500);
	}
})
.WithName("GetIssue")
.WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Get issue by ID",
	Description = "Returns an issue with given ID.",
	Tags = new List<OpenApiTag> { new() { Name = "Issue by ID" } }
});

app.MapPatch("/issues/{source}/{owner}/{repo}/{issueNumber}", async Task<IResult> (string source, string owner, string repo, int issueNumber, IRepositoryService repositoryService, bool? close) =>
{
	if (close.HasValue)
	{
		if (!Enum.TryParse<SourceType>(source, true, out var sourceType))
		{
			return TypedResults.BadRequest("Invalid source. Allowed values: GitHub, GitLab.");
		}
		try
		{
			await repositoryService.CloseIssue(sourceType, owner, repo, issueNumber);
		}
		catch (IssueNotFoundException)
		{
			return TypedResults.NotFound($"Issue with ID {issueNumber} not found.");
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Error closing issue {issueNumber}: {exception.Message}");
			return TypedResults.Problem("An unexpected error occurred while closing the issue.", statusCode: 500);
		}
		return TypedResults.Ok($"Issue with ID {issueNumber} closed successfully.");
	}
	return TypedResults.BadRequest("Invalid request. No valid action specified.");
})
.WithName("UpdateIssueStatus")
.WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Update issue status by ID",
	Description = "Updates an issue with given ID. Use the 'close' query parameter to close the issue.",
	Tags = new List<OpenApiTag> { new() { Name = "Update issue" } }
});

app.MapPost("/issues/{source}/{owner}/{repo}", async (string source, string owner, string repo, CreateIssueRequest createIssueRequest, IRepositoryService repositoryService) =>
{
	if (!Enum.TryParse<SourceType>(source, true, out var sourceType))
	{
		return TypedResults.BadRequest("Invalid source. Allowed values: GitHub, GitLab.");
	}
	try
	{
		var issue = new Issue { Title = createIssueRequest.Title, Description = createIssueRequest.Body };
		var id = await repositoryService.CreateIssue(issue, sourceType, owner, repo);
		return Results.Created($"/issues/{id}", new { Id = id });
	}
	catch (Exception exception)
	{
		Console.Error.WriteLine($"Error creating issue: {exception.Message}");
		return TypedResults.Problem("An unexpected error occurred while creating the issue.", statusCode: 500);
	}
}
).WithName("CreateIssue").WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Create issue",
	Description = "Creates an issue",
	Tags = new List<OpenApiTag> { new() { Name = "Create issue" } }
});

app.MapPut("/issues/{source}/{owner}/{repo}/{issueNumer}", async (string source, string owner, string repo, int issueNumer, UpdateIssueRequest updateIssueRequest, IRepositoryService repositoryService) =>
{
	if (!Enum.TryParse<SourceType>(source, true, out var sourceType))
	{
		return TypedResults.BadRequest("Invalid source. Allowed values: GitHub, GitLab.");
	}
	try
	{
		var issue = new Issue { Title = updateIssueRequest.Title, Description = updateIssueRequest.Body };
		return Results.Ok(await repositoryService.UpdateIssue(issue, sourceType, owner, repo, issueNumer));
	}
	catch (IssueNotFoundException)
	{
		return TypedResults.NotFound($"Issue with ID not found.");
	}
	catch (Exception ex)
	{
		return Results.Problem(ex.Message);
	}
}).WithName("UpdateIssue").WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Update issue",
	Description = "Updates an issue",
	Tags = new List<OpenApiTag> { new() { Name = "Update issue" } }
});

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options =>
		options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));
	app.UseReDoc(options =>
		options.SpecUrl("/openapi/v1.json"));
	app.MapScalarApiReference();

}
app.Run();
