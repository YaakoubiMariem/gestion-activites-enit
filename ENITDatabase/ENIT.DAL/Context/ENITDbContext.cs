using ENIT.BL.Common;
using ENIT.BL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Context;

public class ENITDbContext : IdentityDbContext<ApplicationUser>
{
    public ENITDbContext(DbContextOptions<ENITDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<ActivityComment> ActivityComments => Set<ActivityComment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(120);
            entity.Property(x => x.Code).HasMaxLength(20);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.LogoPath).HasMaxLength(255);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(x => x.FullName).HasMaxLength(150);
            entity.Property(x => x.Matricule).HasMaxLength(50);
            entity.Property(x => x.ProfilePhotoPath).HasMaxLength(255);

            entity.HasOne(x => x.Department)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Activity>(entity =>
        {
            entity.ToTable("Activities");
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.Location).HasMaxLength(200);
            entity.Property(x => x.Link).HasMaxLength(255);
            entity.Property(x => x.CoverImagePath).HasMaxLength(255);

            entity.HasOne(x => x.Department)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedActivities)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Participation>(entity =>
        {
            entity.ToTable("Participations");
            entity.HasIndex(x => new { x.ActivityId, x.UserId }).IsUnique();

            entity.HasOne(x => x.Activity)
                .WithMany(x => x.Participations)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Participations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ActivityComment>(entity =>
        {
            entity.ToTable("ActivityComments");
            entity.Property(x => x.Content).HasMaxLength(2000);

            entity.HasOne(x => x.Activity)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.Property(x => x.Title).HasMaxLength(150);
            entity.Property(x => x.Message).HasMaxLength(1500);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Activity)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.Property(x => x.Action).HasMaxLength(100);
            entity.Property(x => x.EntityName).HasMaxLength(100);
            entity.Property(x => x.EntityId).HasMaxLength(100);

            entity.HasOne(x => x.PerformedBy)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.PerformedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        ApplySoftDeleteQueryFilters(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder builder)
    {
        builder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Activity>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Participation>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ActivityComment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Notification>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<AuditLog>().HasQueryFilter(x => !x.IsDeleted);
    }
}
