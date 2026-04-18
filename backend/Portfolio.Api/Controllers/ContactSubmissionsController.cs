using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Contracts;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/contact-submissions")]
public sealed class ContactSubmissionsController(IContactSubmissionService contactSubmissionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactSubmissionRequest request, CancellationToken cancellationToken)
    {
        var outcome = await contactSubmissionService.CreateAsync(request, cancellationToken);

        if (!outcome.Succeeded)
        {
            return BadRequest(new ValidationProblemDetails(outcome.Errors!)
            {
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Accepted(outcome.Location, outcome.Result);
    }
}
