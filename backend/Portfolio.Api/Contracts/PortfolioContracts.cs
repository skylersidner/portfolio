namespace Portfolio.Api.Contracts;

public sealed record ProjectSummaryResponse(
    string Slug,
    string Title,
    string ShortSummary,
    string Status,
    bool IsFeatured,
    IReadOnlyList<string> Tags,
    string PrimaryLink);

public sealed record ProjectDetailResponse(
    string Slug,
    string Title,
    string ShortSummary,
    string FullOverview,
    string Status,
    bool IsFeatured,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ProjectLinkResponse> Links);

public sealed record ProjectLinkResponse(string Label, string Url, bool IsPrimary);

public sealed record TagResponse(string Slug, string Name, string TagType);

public sealed record ContactSubmissionRequest(
    string Name,
    string Email,
    string Message,
    string? Company,
    string? Topic,
    string? RelatedProjectSlug);

public sealed record ContactSubmissionResult(string SubmissionId, string Status);
