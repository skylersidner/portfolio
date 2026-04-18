using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

public interface IPortfolioReadRepository
{
    Task<IReadOnlyList<PortfolioProject>> GetProjectsAsync(CancellationToken cancellationToken);

    Task<PortfolioProject?> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IReadOnlyList<PortfolioTag>> GetTagsAsync(CancellationToken cancellationToken);
}
