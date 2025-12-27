using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder) {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();


        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Organizations_IsActive");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Organizations_Name");

        builder.HasIndex(x => new { x.IsActive, x.HasActiveGroup })
            .HasDatabaseName("IX_Organizations_IsActive_HasActiveGroup");
    }
}