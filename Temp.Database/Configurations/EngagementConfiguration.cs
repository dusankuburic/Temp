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
    }
}
