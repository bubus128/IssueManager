using IssueManager.Core.Exceptions;
using IssueManager.Core.Models.Interfaces;
using IssueManager.Core.RepositoryService;
using IssueManager.Core.RepositoryService.Interfaces;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddSingleton<IRepositoryService, RepositoryService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/issues", async Task<IResult> (IRepositoryService repositoryService) =>
{
	try
	{
		return TypedResults.Ok(await repositoryService.GetAllIssues());
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

app.MapGet("/issues/{id}", async Task<IResult> (int id, IRepositoryService repositoryService) =>
{
	try
	{
		return TypedResults.Ok(await repositoryService.GetIssueById(id));
	}
	catch (Exception exception)
	{
		Console.Error.WriteLine($"Error getting issue with ID {id}: {exception.Message}");
		return TypedResults.Problem($"An unexpected error occurred while getting issue with ID: {id}.", statusCode: 500);
	}
})
.WithName("GetIssueById")
.WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Get issue by ID",
	Description = "Returns an issue with given ID.",
	Tags = new List<OpenApiTag> { new() { Name = "Issue by ID" } }
});

app.MapDelete("/issues/{id}", async Task<IResult> (int id, IRepositoryService repositoryService) =>
{
	try
	{
		await repositoryService.DeleteIssue(id);
	}
	catch (IssueNotFoundException)
	{
		return TypedResults.NotFound($"Issue with ID {id} not found.");
	}
	catch (Exception exception)
	{
		Console.Error.WriteLine($"Error deleting issue {id}: {exception.Message}");
		return TypedResults.Problem("An unexpected error occurred while deleting the issue.", statusCode: 500);
	}
	return TypedResults.Ok($"Issue with ID {id} deleted successfully.");
})
.WithName("DeleteIssue").WithOpenApi(x => new OpenApiOperation(x)
{
	Summary = "Delete issue by ID",
	Description = "Deletes an issue with given ID.",
	Tags = new List<OpenApiTag> { new() { Name = "Delete issue" } }
});

app.MapPost("/issues", async (IIssue issue, IRepositoryService repositoryService) =>
	{
		try
		{
			var id = await repositoryService.CreateIssue<int>(issue);
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

app.MapPut("/issues/{id}", async (IIssue issue, IRepositoryService repositoryService) =>
{
	try
	{
		var id = await repositoryService.UpdateIssue<int>(issue);

		return Results.NoContent();
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
