using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.Configuration;

namespace Portfolio.Api.Controllers;

[ApiController]
public sealed class MetadataController(
    IOptions<PortfolioAppOptions> appOptions,
    IOptions<DatabaseOptions> databaseOptions,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult GetRoot()
        => Ok(new
        {
            name = appOptions.Value.Name,
            environment = environment.EnvironmentName,
            status = "running",
            surface = "bff"
        });

    [HttpGet("/api")]
    public IActionResult GetApiIndex()
        => Ok(new
        {
            resources = new[]
            {
                "/api/meta",
                "/api/projects",
                "/api/tags",
                "/api/contact-submissions",
                "/healthz"
            }
        });

    [HttpGet("/api/meta")]
    public IActionResult GetMetadata()
        => Ok(new
        {
            portfolio = appOptions.Value.Name,
            environment = environment.EnvironmentName,
            adminEnabled = appOptions.Value.AdminEnabled,
            databaseProvider = databaseOptions.Value.Provider
        });
}
