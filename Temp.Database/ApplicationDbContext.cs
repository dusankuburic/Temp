using Microsoft.EntityFrameworkCore;
using Temp.Database.Configurations;
using Temp.Domain.Models;
using Temp.Domain.Models.Applications;

namespace Temp.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {

    }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Moderator> Moderators { get; set; }

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
}
