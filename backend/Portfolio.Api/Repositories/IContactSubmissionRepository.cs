using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

public interface IContactSubmissionRepository
{
    Task AddAsync(PortfolioContactSubmission submission, CancellationToken cancellationToken);
}
