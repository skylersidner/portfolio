using System.Net.Mail;
using Portfolio.Api.Contracts;
using Portfolio.Api.Data;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public interface IContactSubmissionService
{
    Task<ContactSubmissionCreateOutcome> CreateAsync(ContactSubmissionRequest request, CancellationToken cancellationToken);
}

public sealed record ContactSubmissionCreateOutcome(
    string? Location,
    ContactSubmissionResult? Result,
    Dictionary<string, string[]>? Errors)
{
    public bool Succeeded => Errors is null;
}

public sealed class ContactSubmissionService(
    IContactSubmissionRepository contactSubmissionRepository,
    ILogger<ContactSubmissionService> logger) : IContactSubmissionService
{
    public async Task<ContactSubmissionCreateOutcome> CreateAsync(ContactSubmissionRequest request, CancellationToken cancellationToken)
    {
        var errors = Validate(request);

        if (errors is not null)
        {
            return new ContactSubmissionCreateOutcome(null, null, errors);
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

        await contactSubmissionRepository.AddAsync(submission, cancellationToken);

        logger.LogInformation(
            "Accepted contact submission from {Email} for topic {Topic}.",
            submission.Email,
            submission.Topic ?? "general");

        return new ContactSubmissionCreateOutcome(
            $"/api/contact-submissions/{submission.Id:N}",
            new ContactSubmissionResult(submission.Id.ToString("N"), "accepted"),
            null);
    }

    private static Dictionary<string, string[]>? Validate(ContactSubmissionRequest request)
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
        else if (!MailAddress.TryCreate(request.Email.Trim(), out _))
        {
            errors ??= new Dictionary<string, string[]>();
            errors["email"] = ["Email must be valid."];
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            errors ??= new Dictionary<string, string[]>();
            errors["message"] = ["Message is required."];
        }

        return errors;
    }
}
