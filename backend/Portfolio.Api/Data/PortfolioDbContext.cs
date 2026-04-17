using Microsoft.EntityFrameworkCore;

namespace Portfolio.Api.Data;

public sealed class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public DbSet<PortfolioProject> Projects => Set<PortfolioProject>();

    public DbSet<PortfolioTag> Tags => Set<PortfolioTag>();

    public DbSet<PortfolioProjectTag> ProjectTags => Set<PortfolioProjectTag>();

    public DbSet<PortfolioProjectLink> ProjectLinks => Set<PortfolioProjectLink>();

    public DbSet<PortfolioContactSubmission> ContactSubmissions => Set<PortfolioContactSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PortfolioProject>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(project => project.Id);
            entity.HasIndex(project => project.Slug).IsUnique();

            entity.Property(project => project.Slug).HasMaxLength(160);
            entity.Property(project => project.Title).HasMaxLength(200);
            entity.Property(project => project.ShortSummary).HasMaxLength(400);
            entity.Property(project => project.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<PortfolioTag>(entity =>
        {
            entity.ToTable("project_tags");
            entity.HasKey(tag => tag.Id);
            entity.HasIndex(tag => tag.Slug).IsUnique();

            entity.Property(tag => tag.Slug).HasMaxLength(100);
            entity.Property(tag => tag.Name).HasMaxLength(100);
            entity.Property(tag => tag.TagType).HasMaxLength(50);
        });

        modelBuilder.Entity<PortfolioProjectTag>(entity =>
        {
            entity.ToTable("project_tag_map");
            entity.HasKey(projectTag => new { projectTag.ProjectId, projectTag.TagId });

            entity.HasOne(projectTag => projectTag.Project)
                .WithMany(project => project.ProjectTags)
                .HasForeignKey(projectTag => projectTag.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(projectTag => projectTag.Tag)
                .WithMany(tag => tag.ProjectTags)
                .HasForeignKey(projectTag => projectTag.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PortfolioProjectLink>(entity =>
        {
            entity.ToTable("project_links");
            entity.HasKey(link => link.Id);

            entity.Property(link => link.Label).HasMaxLength(120);
            entity.Property(link => link.Url).HasMaxLength(500);

            entity.HasOne(link => link.Project)
                .WithMany(project => project.Links)
                .HasForeignKey(link => link.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PortfolioContactSubmission>(entity =>
        {
            entity.ToTable("contact_submissions");
            entity.HasKey(submission => submission.Id);
            entity.HasIndex(submission => submission.Email);
            entity.HasIndex(submission => submission.SubmittedAt);

            entity.Property(submission => submission.Name).HasMaxLength(200);
            entity.Property(submission => submission.Email).HasMaxLength(320);
            entity.Property(submission => submission.Company).HasMaxLength(200);
            entity.Property(submission => submission.Topic).HasMaxLength(100);
            entity.Property(submission => submission.RelatedProjectSlug).HasMaxLength(160);
            entity.Property(submission => submission.Status).HasMaxLength(50);
            entity.Property(submission => submission.Message).HasMaxLength(4000);
        });
    }
}

public sealed class PortfolioProject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ShortSummary { get; set; } = string.Empty;

    public string FullOverview { get; set; } = string.Empty;

    public string Status { get; set; } = "draft";

    public bool IsFeatured { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<PortfolioProjectTag> ProjectTags { get; set; } = [];

    public ICollection<PortfolioProjectLink> Links { get; set; } = [];
}

public sealed class PortfolioTag
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string TagType { get; set; } = "technology";

    public ICollection<PortfolioProjectTag> ProjectTags { get; set; } = [];
}

public sealed class PortfolioProjectTag
{
    public Guid ProjectId { get; set; }

    public PortfolioProject Project { get; set; } = null!;

    public Guid TagId { get; set; }

    public PortfolioTag Tag { get; set; } = null!;

    public int SortOrder { get; set; }
}

public sealed class PortfolioProjectLink
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProjectId { get; set; }

    public PortfolioProject Project { get; set; } = null!;

    public string Label { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}

public sealed class PortfolioContactSubmission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? Company { get; set; }

    public string? Topic { get; set; }

    public string? RelatedProjectSlug { get; set; }

    public string Status { get; set; } = "new";

    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
}
