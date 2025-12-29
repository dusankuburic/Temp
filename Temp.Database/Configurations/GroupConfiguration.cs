using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder) {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.ProfilePictureUrl)
            .HasMaxLength(500)
            .IsRequired(false);


        builder.HasIndex(x => x.OrganizationId)
            .HasDatabaseName("IX_Groups_OrganizationId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Groups_IsActive");

        builder.HasIndex(x => new { x.OrganizationId, x.IsActive })
            .HasDatabaseName("IX_Groups_OrganizationId_IsActive");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Groups_Name");
    }
}