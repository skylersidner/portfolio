using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

public sealed class ContactSubmissionRepository(PortfolioDbContext dbContext) : IContactSubmissionRepository
{
    public async Task AddAsync(PortfolioContactSubmission submission, CancellationToken cancellationToken)
    {
        dbContext.ContactSubmissions.Add(submission);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public sealed class InMemoryContactSubmissionRepository : IContactSubmissionRepository
{
    public Task AddAsync(PortfolioContactSubmission submission, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
