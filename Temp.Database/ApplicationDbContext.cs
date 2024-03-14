using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temp.Database.Configurations;
using Temp.Domain.Models;
using Temp.Domain.Models.Applications;
using Temp.Domain.Models.Identity;

namespace Temp.Database;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {

    }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<EmploymentStatus> EmploymentStatuses { get; set; }

    public DbSet<Workplace> Workplaces { get; set; }

    public DbSet<Engagement> Engagements { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<ModeratorGroup> ModeratorGroups { get; set; }

    public DbSet<Application> Applications { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new EmploymentStatusConfiguration());
        modelBuilder.ApplyConfiguration(new EngagementConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new ModeratorGroupConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new WorkplaceConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {

        var entities = ChangeTracker.Entries()
            .Where(x =>
                x.Entity is BaseEntity &&
                (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities) {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added) {
                ((BaseEntity)entity.Entity).CreatedAt = now;
            }
            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
