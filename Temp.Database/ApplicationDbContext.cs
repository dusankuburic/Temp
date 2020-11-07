using Microsoft.EntityFrameworkCore;
using Temp.Domain.Models;

namespace Temp.Database
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees {get; set;}

        public DbSet<EmploymentStatus> EmploymentStatuses {get; set;}
        public DbSet<Workplace> Workplaces {get; set;}
        public DbSet<Engagement> Engagements {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Engagement>()
                .HasKey(x => new {x.Id, x.EmployeeId, x.WorkplaceId});

            modelBuilder.Entity<Engagement>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Employee>()
                .Property(x => x.Role)
                .HasDefaultValue("None");

        

            
        }
    }
}
