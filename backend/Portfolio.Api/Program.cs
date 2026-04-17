using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Configuration;
using Portfolio.Api.Contracts;
using Portfolio.Api.Data;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables(prefix: "PORTFOLIO_");

var publicBaseUrl = builder.Configuration.GetValue<string>("Application:PublicBaseUrl");
if (Uri.TryCreate(publicBaseUrl, UriKind.Absolute, out var publicUri) && string.Equals(publicUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = publicUri.Port;
    });
}

var databaseSettings = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services
    .AddOptions<PortfolioAppOptions>()
    .Bind(builder.Configuration.GetSection(PortfolioAppOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var corsSettings = builder.Configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (corsSettings.AllowedOrigins.Count == 0)
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            return;
        }

        policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

if (string.Equals(databaseSettings.Provider, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    if (string.IsNullOrWhiteSpace(databaseSettings.ConnectionString))
    {
        throw new InvalidOperationException("Database:ConnectionString must be configured when using the PostgreSQL provider.");
    }

    builder.Services.AddDbContext<PortfolioDbContext>(options =>
        options.UseNpgsql(databaseSettings.ConnectionString));

    builder.Services.AddScoped<IPortfolioContentService, PortfolioContentService>();
}
else
{
    builder.Services.AddSingleton<IPortfolioContentService, InMemoryPortfolioContentService>();
}

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (string.Equals(databaseSettings.Provider, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    await PortfolioDbInitializer.InitializeAsync(dbContext, databaseSettings, app.Logger);
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapGet("/", (IOptions<PortfolioAppOptions> options, IWebHostEnvironment environment) =>
{
    var appOptions = options.Value;

    return Results.Ok(new
    {
        name = appOptions.Name,
        environment = environment.EnvironmentName,
        status = "running",
        surface = "bff"
    });
});

app.MapGet("/api", () => Results.Ok(new
{
    resources = new[]
    {
        "/api/meta",
        "/api/projects",
        "/api/tags",
        "/api/contact-submissions",
        "/healthz"
    }
}));

app.MapGet("/api/meta", (IOptions<PortfolioAppOptions> appOptions, IOptions<DatabaseOptions> databaseOptions, IWebHostEnvironment environment) =>
    Results.Ok(new
    {
        portfolio = appOptions.Value.Name,
        environment = environment.EnvironmentName,
        adminEnabled = appOptions.Value.AdminEnabled,
        databaseProvider = databaseOptions.Value.Provider
    }));

app.MapGet("/api/projects", async (IPortfolioContentService contentService, CancellationToken cancellationToken) =>
    Results.Ok(await contentService.GetProjectsAsync(cancellationToken)));

app.MapGet("/api/projects/{slug}", async (string slug, IPortfolioContentService contentService, CancellationToken cancellationToken) =>
{
    var project = await contentService.GetProjectBySlugAsync(slug, cancellationToken);

    return project is null
        ? Results.NotFound(new { message = $"Project '{slug}' was not found." })
        : Results.Ok(project);
});

app.MapGet("/api/tags", async (IPortfolioContentService contentService, CancellationToken cancellationToken) =>
    Results.Ok(await contentService.GetTagsAsync(cancellationToken)));

app.MapPost("/api/contact-submissions", async (ContactSubmissionRequest request, PortfolioDbContext dbContext, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    Dictionary<string, string[]>? errors = null;

    if (string.IsNullOrWhiteSpace(request.Name))
    {
        errors ??= new Dictionary<string, string[]>();
        errors["name"] = ["Name is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Email))
    {
        errors ??= new Dictionary<string, string[]>();
        errors["email"] = ["Email is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Message))
    {
        errors ??= new Dictionary<string, string[]>();
        errors["message"] = ["Message is required."];
    }

    if (errors is not null)
    {
        return Results.ValidationProblem(errors);
    }

    var submission = new PortfolioContactSubmission
    {
        Name = request.Name.Trim(),
        Email = request.Email.Trim(),
        Message = request.Message.Trim(),
        Company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
        Topic = string.IsNullOrWhiteSpace(request.Topic) ? "general" : request.Topic.Trim(),
        RelatedProjectSlug = string.IsNullOrWhiteSpace(request.RelatedProjectSlug) ? null : request.RelatedProjectSlug.Trim(),
        Status = "new"
    };

    dbContext.ContactSubmissions.Add(submission);
    await dbContext.SaveChangesAsync(cancellationToken);

    loggerFactory.CreateLogger("ContactSubmissions")
        .LogInformation("Accepted contact submission from {Email} for topic {Topic}.", submission.Email, submission.Topic ?? "general");

    return Results.Accepted(
        uri: $"/api/contact-submissions/{submission.Id:N}",
        value: new ContactSubmissionResult(submission.Id.ToString("N"), "accepted"));
});

app.MapHealthChecks("/healthz");

app.Run();
