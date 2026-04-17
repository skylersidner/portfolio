using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Configuration;

public sealed class PortfolioAppOptions
{
    public const string SectionName = "Application";

    [Required]
    public string Name { get; set; } = "Portfolio BFF";

    public string PublicBaseUrl { get; set; } = "https://localhost:7227";

    public bool AdminEnabled { get; set; }
}

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string Provider { get; set; } = "PostgreSQL";

    public string ConnectionString { get; set; } = string.Empty;

    public bool SeedSampleData { get; set; } = true;
}

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    public List<string> AllowedOrigins { get; set; } = ["http://localhost:4200"];
}
