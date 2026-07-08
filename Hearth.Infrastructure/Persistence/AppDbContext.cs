using Hearth.Domain.Common;
using Hearth.Domain.Entities;
using Hearth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdTask> HouseholdTasks => Set<HouseholdTask>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auditing: postavi CreatedAt pri ubacivanju (za sve BaseEntity, uklj. domaćinstva i zadatke).
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
                entry.Entity.CreatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Household>(entity =>
        {
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);

            entity.Property(h => h.AdultJoinCode).IsRequired().HasMaxLength(20);
            entity.Property(h => h.ChildJoinCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(h => h.AdultJoinCode).IsUnique();
            entity.HasIndex(h => h.ChildJoinCode).IsUnique();

            entity.HasMany(h => h.Tasks)
                  .WithOne(t => t.Household)
                  .HasForeignKey(t => t.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.ShoppingItems)
                  .WithOne(s => s.Household)
                  .HasForeignKey(s => s.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<HouseholdTask>(entity =>
        {
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
        });

        builder.Entity<ShoppingItem>(entity =>
        {
            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Message).IsRequired().HasMaxLength(500);

            entity.HasOne(n => n.Household)
                  .WithMany()
                  .HasForeignKey(n => n.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

      
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);

            entity.HasOne(u => u.Household)
                  .WithMany()
                  .HasForeignKey(u => u.HouseholdId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}