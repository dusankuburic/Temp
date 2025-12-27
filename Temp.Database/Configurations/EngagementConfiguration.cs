using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class EngagementConfiguration : IEntityTypeConfiguration<Engagement>
{
    public void Configure(EntityTypeBuilder<Engagement> builder) {
        builder.HasKey(x => new { x.Id, x.EmployeeId, x.WorkplaceId });

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.WorkplaceId)
            .IsRequired();

        builder.Property(x => x.EmploymentStatusId)
            .IsRequired();

        builder.Property(x => x.DateFrom)
            .IsRequired();

        builder.Property(x => x.DateTo)
            .IsRequired();

        builder.Property(x => x.Salary)
            .IsRequired();


        builder.HasIndex(x => x.EmployeeId)
            .HasDatabaseName("IX_Engagements_EmployeeId");

        builder.HasIndex(x => x.DateTo)
            .HasDatabaseName("IX_Engagements_DateTo");

        builder.HasIndex(x => new { x.EmployeeId, x.DateTo })
            .HasDatabaseName("IX_Engagements_EmployeeId_DateTo");

        builder.HasIndex(x => new { x.DateFrom, x.DateTo })
            .HasDatabaseName("IX_Engagements_DateRange");
    }
}