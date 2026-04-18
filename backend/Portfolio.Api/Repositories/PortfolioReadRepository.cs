using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

public sealed class PortfolioReadRepository(PortfolioDbContext dbContext) : IPortfolioReadRepository
{
    public async Task<IReadOnlyList<PortfolioProject>> GetProjectsAsync(CancellationToken cancellationToken)
        => await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTags)
                .ThenInclude(projectTag => projectTag.Tag)
            .Include(project => project.Links)
            .OrderByDescending(project => project.IsFeatured)
            .ThenBy(project => project.Title)
            .ToListAsync(cancellationToken);

    public async Task<PortfolioProject?> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken)
        => await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTags)
                .ThenInclude(projectTag => projectTag.Tag)
            .Include(project => project.Links)
            .SingleOrDefaultAsync(project => project.Slug == slug, cancellationToken);

    public async Task<IReadOnlyList<PortfolioTag>> GetTagsAsync(CancellationToken cancellationToken)
        => await dbContext.Tags
            .AsNoTracking()
            .OrderBy(tag => tag.TagType)
            .ThenBy(tag => tag.Name)
            .ToListAsync(cancellationToken);
}
