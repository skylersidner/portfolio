using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Configuration;
using Portfolio.Api.Data;
using Portfolio.Api.Repositories;
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
builder.Services.AddControllers();

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

    var npgsqlConnectionString = NormalizeConnectionString(databaseSettings.ConnectionString);

    builder.Services.AddDbContext<PortfolioDbContext>(options =>
        options.UseNpgsql(npgsqlConnectionString));

    builder.Services.AddScoped<IPortfolioReadRepository, PortfolioReadRepository>();
    builder.Services.AddScoped<IContactSubmissionRepository, ContactSubmissionRepository>();
    builder.Services.AddScoped<IPortfolioContentService, PortfolioContentService>();
    builder.Services.AddScoped<IContactSubmissionService, ContactSubmissionService>();
}
else
{
    builder.Services.AddSingleton<IPortfolioContentService, InMemoryPortfolioContentService>();
    builder.Services.AddSingleton<IContactSubmissionRepository, InMemoryContactSubmissionRepository>();
    builder.Services.AddSingleton<IContactSubmissionService, ContactSubmissionService>();
}

var app = builder.Build();

// Converts a postgresql:// URI (as provided by Railway and other PaaS hosts) to the
// Npgsql key=value format expected by EF Core's ADO.NET connection string parser.
static string NormalizeConnectionString(string connectionString)
{
    if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
        !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
    {
        return connectionString;
    }

    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':', 2);
    var database = uri.AbsolutePath.TrimStart('/');
    return $"Host={uri.Host};Port={uri.Port};Database={database};Username={userInfo[0]};Password={userInfo[1]}";
}

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

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();
