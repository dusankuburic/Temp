using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class EmploymentStatusConfiguration : IEntityTypeConfiguration<EmploymentStatus>
{
    public void Configure(EntityTypeBuilder<EmploymentStatus> builder) {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}
