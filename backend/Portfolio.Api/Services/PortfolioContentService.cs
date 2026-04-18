using Portfolio.Api.Contracts;
using Portfolio.Api.Data;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public interface IPortfolioContentService
{
    Task<IReadOnlyList<ProjectSummaryResponse>> GetProjectsAsync(CancellationToken cancellationToken);

    Task<ProjectDetailResponse?> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IReadOnlyList<TagResponse>> GetTagsAsync(CancellationToken cancellationToken);
}

public sealed class PortfolioContentService(IPortfolioReadRepository portfolioReadRepository) : IPortfolioContentService
{
    public async Task<IReadOnlyList<ProjectSummaryResponse>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        var projects = await portfolioReadRepository.GetProjectsAsync(cancellationToken);
        return projects.Select(MapSummary).ToList();
    }

    public async Task<ProjectDetailResponse?> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var project = await portfolioReadRepository.GetProjectBySlugAsync(slug, cancellationToken);
        return project is null ? null : MapDetail(project);
    }

    public async Task<IReadOnlyList<TagResponse>> GetTagsAsync(CancellationToken cancellationToken)
    {
        var tags = await portfolioReadRepository.GetTagsAsync(cancellationToken);

        return tags.Select(tag => new TagResponse(tag.Slug, tag.Name, tag.TagType)).ToList();
    }

    private static ProjectSummaryResponse MapSummary(PortfolioProject project)
        => new(
            project.Slug,
            project.Title,
            project.ShortSummary,
            project.Status,
            project.IsFeatured,
            project.ProjectTags
                .OrderBy(projectTag => projectTag.SortOrder)
                .Select(projectTag => projectTag.Tag.Name)
                .ToList(),
            project.Links
                .OrderBy(link => link.SortOrder)
                .FirstOrDefault(link => link.IsPrimary)?.Url ?? string.Empty);

    private static ProjectDetailResponse MapDetail(PortfolioProject project)
        => new(
            project.Slug,
            project.Title,
            project.ShortSummary,
            project.FullOverview,
            project.Status,
            project.IsFeatured,
            project.ProjectTags
                .OrderBy(projectTag => projectTag.SortOrder)
                .Select(projectTag => projectTag.Tag.Name)
                .ToList(),
            project.Links
                .OrderBy(link => link.SortOrder)
                .Select(link => new ProjectLinkResponse(link.Label, link.Url, link.IsPrimary))
                .ToList());
}

public sealed class InMemoryPortfolioContentService : IPortfolioContentService
{
    private static readonly IReadOnlyList<ProjectDetailResponse> Projects =
    [
        new(
            Slug: "quiet-constellation-hybrid",
            Title: "Quiet Constellation Hybrid",
            ShortSummary: "A calm, high-signal portfolio shell combining constellation storytelling with a structured project atlas.",
            FullOverview: "Approved Phase 1 foundation for the portfolio root experience, designed to highlight side projects while leaving room for future admin and content workflows.",
            Status: "planned",
            IsFeatured: true,
            Tags: ["Angular", ".NET 10", "BFF", "Portfolio"],
            Links:
            [
                new ProjectLinkResponse("Project hub", "/projects/quiet-constellation-hybrid", true),
                new ProjectLinkResponse("Design proposals", "/design-proposals/index.md", false)
            ]),
        new(
            Slug: "signal-resume-hybrid",
            Title: "Signal Resume Hybrid",
            ShortSummary: "A resume-forward companion concept kept as a supporting credibility path.",
            FullOverview: "This remains a secondary direction and reference point for content strategy rather than the main portfolio experience.",
            Status: "reference",
            IsFeatured: false,
            Tags: ["Career", "UX", "Content Strategy"],
            Links:
            [
                new ProjectLinkResponse("Prototype", "/ui-prototypes/signal-resume-hybrid.html", true)
            ])
    ];

    private static readonly IReadOnlyList<TagResponse> Tags =
    [
        new("angular", "Angular", "technology"),
        new("dotnet", ".NET", "technology"),
        new("portfolio", "Portfolio", "domain"),
        new("bff", "BFF", "architecture")
    ];

    public Task<IReadOnlyList<ProjectSummaryResponse>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ProjectSummaryResponse> summaries = Projects
            .Select(project => new ProjectSummaryResponse(
                project.Slug,
                project.Title,
                project.ShortSummary,
                project.Status,
                project.IsFeatured,
                project.Tags,
                project.Links.FirstOrDefault(link => link.IsPrimary)?.Url ?? string.Empty))
            .ToList();

        return Task.FromResult(summaries);
    }

    public Task<ProjectDetailResponse?> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var project = Projects.FirstOrDefault(project => string.Equals(project.Slug, slug, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(project);
    }

    public Task<IReadOnlyList<TagResponse>> GetTagsAsync(CancellationToken cancellationToken)
        => Task.FromResult(Tags);
}
