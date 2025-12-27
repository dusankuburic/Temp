using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder) {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(50)
            .HasDefaultValue("None");


        builder.HasIndex(x => x.Role)
            .HasDatabaseName("IX_Employees_Role");

        builder.HasIndex(x => x.AppUserId)
            .HasDatabaseName("IX_Employees_AppUserId");

        builder.HasIndex(x => x.TeamId)
            .HasDatabaseName("IX_Employees_TeamId");

        builder.HasIndex(x => new { x.FirstName, x.LastName })
            .HasDatabaseName("IX_Employees_Name");
    }
}