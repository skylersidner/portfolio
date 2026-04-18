using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IPortfolioContentService contentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        => Ok(await contentService.GetProjectsAsync(cancellationToken));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProjectBySlug(string slug, CancellationToken cancellationToken)
    {
        var project = await contentService.GetProjectBySlugAsync(slug, cancellationToken);

        return project is null
            ? NotFound(new { message = $"Project '{slug}' was not found." })
            : Ok(project);
    }
}
