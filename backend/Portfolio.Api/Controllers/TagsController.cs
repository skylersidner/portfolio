using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/tags")]
public sealed class TagsController(IPortfolioContentService contentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTags(CancellationToken cancellationToken)
        => Ok(await contentService.GetTagsAsync(cancellationToken));
}
