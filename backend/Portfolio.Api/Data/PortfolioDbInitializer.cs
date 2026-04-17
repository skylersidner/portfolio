using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Configuration;

namespace Portfolio.Api.Data;

public static class PortfolioDbInitializer
{
    public static async Task InitializeAsync(
        PortfolioDbContext dbContext,
        DatabaseOptions options,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (!options.SeedSampleData || await dbContext.Projects.AnyAsync(cancellationToken))
        {
            return;
        }

        var angularTag = new PortfolioTag { Slug = "angular", Name = "Angular", TagType = "technology" };
        var dotnetTag = new PortfolioTag { Slug = "dotnet", Name = ".NET", TagType = "technology" };
        var portfolioTag = new PortfolioTag { Slug = "portfolio", Name = "Portfolio", TagType = "domain" };
        var bffTag = new PortfolioTag { Slug = "bff", Name = "BFF", TagType = "architecture" };

        var quietConstellation = new PortfolioProject
        {
            Slug = "quiet-constellation-hybrid",
            Title = "Quiet Constellation Hybrid",
            ShortSummary = "A calm, high-signal portfolio shell combining constellation storytelling with a structured project atlas.",
            FullOverview = "Approved Phase 1 foundation for the portfolio root experience, designed to highlight side projects while leaving room for future admin and content workflows.",
            Status = "planned",
            IsFeatured = true,
            Links =
            [
                new PortfolioProjectLink { Label = "Project hub", Url = "/projects/quiet-constellation-hybrid", IsPrimary = true, SortOrder = 0 },
                new PortfolioProjectLink { Label = "Design proposals", Url = "/design-proposals/index.md", IsPrimary = false, SortOrder = 1 }
            ]
        };

        quietConstellation.ProjectTags =
        [
            new PortfolioProjectTag { Project = quietConstellation, Tag = angularTag, SortOrder = 0 },
            new PortfolioProjectTag { Project = quietConstellation, Tag = dotnetTag, SortOrder = 1 },
            new PortfolioProjectTag { Project = quietConstellation, Tag = bffTag, SortOrder = 2 },
            new PortfolioProjectTag { Project = quietConstellation, Tag = portfolioTag, SortOrder = 3 }
        ];

        var signalResume = new PortfolioProject
        {
            Slug = "signal-resume-hybrid",
            Title = "Signal Resume Hybrid",
            ShortSummary = "A resume-forward companion concept kept as a supporting credibility path.",
            FullOverview = "This remains a secondary direction and reference point for content strategy rather than the main portfolio experience.",
            Status = "reference",
            IsFeatured = false,
            Links =
            [
                new PortfolioProjectLink { Label = "Prototype", Url = "/ui-prototypes/signal-resume-hybrid.html", IsPrimary = true, SortOrder = 0 }
            ]
        };

        signalResume.ProjectTags =
        [
            new PortfolioProjectTag { Project = signalResume, Tag = portfolioTag, SortOrder = 0 }
        ];

        dbContext.Projects.AddRange(quietConstellation, signalResume);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded PostgreSQL portfolio content with starter records.");
    }
}
